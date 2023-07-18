using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class NodgesHelper
{
    #region DataSync
    public static async Task<HashSet<string>> RetrieveOnto(GraphDbRepository repo)
    {
        var api = repo.GraphDBAPI;

        string query = "SELECT * FROM <http://ontology> WHERE { ?s ?p ?o .}";

        var json = await api.SelectQuery(query, true);
        var data = JsonConvert.DeserializeObject<JObject>(json);

        HashSet<string> uriOnto = data.ExtractUriOnto();

        return uriOnto;
    }

    public static async Task<NodgesDicId> RetrieveData(GraphDbRepository repo, HashSet<string> ontoUris)
    {
        var api = repo.GraphDBAPI;

        string query = "SELECT * FROM <http://data> WHERE { ?s ?p ?o .}";

        var json = await api.SelectQuery(query, true);
        var data = JsonConvert.DeserializeObject<JObject>(json);


        NodgesDicId nodges = data.ExtractNodgesWithout(repo.GraphDbRepositoryNamespaces, ontoUris);
        nodges.AddRetrievedNames(repo.GraphDbRepositoryDistantUris);
        nodges.ExtractNodeNamesToProperties();

        return nodges;
    }

    private static HashSet<string> ExtractUriOnto(this JObject data)
    {
        HashSet<string> uriOnto = new();

        foreach (JToken binding in data["results"]["bindings"])
        {
            string sValue = binding["s"]["value"].Value<string>();
            string oValue = binding["o"]["value"].Value<string>();

            if(!uriOnto.Contains(sValue))
                uriOnto.Add(sValue);
            
            if(!uriOnto.Contains(oValue))
                uriOnto.Add(oValue); 
        }


        return uriOnto;
    }

    private static NodgesDicId ExtractNodgesWithout(this JObject data, GraphDbRepositoryNamespaces repoNamespaces, HashSet<string> ontoUris)
    {
        var nodges = new NodgesDicId();

        var nodesDicId = nodges.NodesDicId;
        var edgesDicId = nodges.EdgesDicId;

        foreach (JToken binding in data["results"]["bindings"])
        {
            var sToken = binding["s"];
            string sType = sToken["type"].Value<string>();
            string sValue = sToken["value"].Value<string>();

            var pToken = binding["p"];
            string pType = pToken["type"].Value<string>();
            string pValue = pToken["value"].Value<string>();

            var oToken = binding["o"];
            string oType = oToken["type"].Value<string>();
            string oValue = oToken["value"].Value<string>();


            if (ontoUris.Contains(sValue) || ontoUris.Contains(oValue))
                continue;


            Node sNode = GetNodeFromDictOrCreate(sToken, nodesDicId, repoNamespaces);

            if (oType == "literal" && sType == "uri")
            {
                AddToNodeAsProperty(sNode, pValue, oValue);
                continue;
            }

            if(pValue == "http://xmlns.com/foaf/0.1/depiction")
            {
                AddToNodeAsMedia(sNode, pValue, oValue);
                continue;
            }


            Node oNode = GetNodeFromDictOrCreate(oToken, nodesDicId, repoNamespaces);

            var edge = new Edge(pType, pValue, sNode, oNode, repoNamespaces);

            if (edgesDicId.TryGetValue(edge.Id, out _))
            {
                continue;
            }

            edgesDicId.Add(edge.Id, edge);

            sNode.EdgeSource.Add(edge);
            oNode.EdgeTarget.Add(edge);
        }

        Debug.Log("Retrieve Data : Nb Nodes : " + nodesDicId.Count + " , Nb Edges : " + edgesDicId.Count);

        return nodges;
    }


    public static NodgesDicId ExtractNodgesForDistantUri(this JObject data, GraphDbRepositoryNamespaces repoNamespaces)
    {
        var nodges = new NodgesDicId();

        var nodesDicId = nodges.NodesDicId;
        var edgesDicId = nodges.EdgesDicId;

        foreach (JToken binding in data["results"]["bindings"])
        {
            var sToken = binding["s"];
            string sType = sToken["type"].Value<string>();
            string sValue = sToken["value"].Value<string>();

            var pToken = binding["p"];
            string pType = pToken["type"].Value<string>();
            string pValue = pToken["value"].Value<string>();

            var oToken = binding["o"];
            string oType = oToken["type"].Value<string>();
            string oValue = oToken["value"].Value<string>();


            Node sNode = GetNodeFromDictOrCreate(sToken, nodesDicId, repoNamespaces);


            if (oType == "literal" && sType == "uri") // oNode is a literal
                continue;

            if (pValue == "http://xmlns.com/foaf/0.1/depiction") // oNode is a media
                continue;


            Node oNode = GetNodeFromDictOrCreate(oToken, nodesDicId, repoNamespaces);

            var edge = new Edge(pType, pValue, sNode, oNode, repoNamespaces);

            if (edgesDicId.TryGetValue(edge.Id, out Edge edgeExisting))
            {
                continue;
            }

            edgesDicId.Add(edge.Id, edge);

            sNode.EdgeSource.Add(edge);
            oNode.EdgeTarget.Add(edge);
        }

        return nodges;
    }

    public static Dictionary<int, Node> GetNoLabeledNodes(this Dictionary<int, Node> idAndNodes)
    {
        Dictionary<int, Node> nolabeled = new();

        foreach (var idAndNode in idAndNodes)
        {
            var node = idAndNode.Value;

            if (node.Type != NodgeType.Uri)
                continue;

            if (node.DoesPropertiesContainName())
                continue;

            nolabeled.Add(idAndNode.Key, idAndNode.Value);
        }

        return nolabeled;
    }

    public static Dictionary<int, Node> GetNoOntoUriNodes(this Dictionary<int, Node> idAndNodes, IReadOnlyDictionary<string, OntologyTree> ontoTreeDict)
    {

        Dictionary<int, Node> noOntoned = new();

        foreach (var idAndNode in idAndNodes)
        {
            var node = idAndNode.Value;

            if (node.Type != NodgeType.Uri)
                continue;

            var namespce = node.Namespace;

            if (!ontoTreeDict.TryGetValue(namespce, out OntologyTree ontoTree))
            {
                noOntoned.Add(idAndNode.Key, idAndNode.Value);
                continue;
            }

            if (!ontoTree.TryGetOntoNode(idAndNode.Key, out _))
            {
                noOntoned.Add(idAndNode.Key, idAndNode.Value);
                continue;
            }
        }

        return noOntoned;
    }
    #endregion


    #region Graph
    public static async Task<NodgesDicId> RetrieveGraph(string query, GraphDbRepository repo)
    {
        var debugChrono = DebugChrono.Instance;
        debugChrono.Start("RetreiveGraph");
        var api = repo.GraphDBAPI;
        var json = await api.SelectQuery(query, true);
        var data = JsonConvert.DeserializeObject<JObject>(json);


        NodgesDicId nodges = data.ExtractNodges(repo.GraphDbRepositoryNamespaces);
        nodges.AddRetrievedNames(repo.GraphDbRepositoryDistantUris);
        nodges.ExtractNodeNamesToProperties();


        debugChrono.Stop("RetreiveGraph");

        return nodges;
    }

    public static NodgesDicId ExtractNodges(this JObject data, GraphDbRepositoryNamespaces repoNamespaces)
    {
        var nodges = new NodgesDicId();

        var nodesDicId = nodges.NodesDicId;
        var edgesDicId = nodges.EdgesDicId;

        repoNamespaces.DetachNodesFromOntoNodes();

        DepthOntologyLinker ontologyLinker = new(repoNamespaces);

        foreach (JToken binding in data["results"]["bindings"])
        {
            var sToken = binding["s"];
            string sType = sToken["type"].Value<string>();
            string sValue = sToken["value"].Value<string>();

            if (repoNamespaces.IsUriAnOnto(sValue))
                continue;

            var pToken = binding["p"];
            string pType = pToken["type"].Value<string>();
            string pValue = pToken["value"].Value<string>();

            var oToken = binding["o"];
            string oType = oToken["type"].Value<string>();
            string oValue = oToken["value"].Value<string>();


            bool isObjectPossiblyAnOnto = repoNamespaces.IsUriAnOnto(oValue) && pValue == "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";

            int oId = oValue.GetHashCode();


            Node sNode = GetNodeFromDictOrCreate(sToken, nodesDicId, repoNamespaces);

            if(isObjectPossiblyAnOnto)
            {
                var simpleOntoNode = new Node(oId, oType, oValue, repoNamespaces);

                if (ontologyLinker.TryEstablishLink(sNode, simpleOntoNode))
                    continue;
            }


            if(oType == "literal" && sType == "uri")
            {
                AddToNodeAsProperty(sNode, pValue, oValue);
                continue;
            }

            if (pValue == "http://xmlns.com/foaf/0.1/depiction")
            {
                AddToNodeAsMedia(sNode, pValue, oValue);
                continue;
            }

            Node oNode = GetNodeFromDictOrCreate(oToken, nodesDicId, repoNamespaces);


            var edge = new Edge(pType, pValue, sNode, oNode, repoNamespaces);

            if (edgesDicId.TryGetValue(edge.Id, out _))
            {
                continue;
            }

            edgesDicId.Add(edge.Id, edge);

            sNode.EdgeSource.Add(edge);
            oNode.EdgeTarget.Add(edge);
        }

        ontologyLinker.AttachNodesToOntoNodes();

        return nodges;
    }


    private static void AddToNodeAsProperty(Node node, string edgeUri, string propValue)
    {
        if (node.Properties.ContainsKey(edgeUri))
            return;

        node.Properties.Add(edgeUri, propValue);
    }

    private static void AddToNodeAsMedia(Node node, string edgeUri, string propValue)
    {
        if (node.Medias.ContainsKey(propValue))
            return;

        node.Medias.Add(propValue, null);
    }

    private static Node GetNodeFromDictOrCreate(JToken nodeToken, Dictionary<int, Node> nodeDic, GraphDbRepositoryNamespaces repoNamespaces)
    {
        string value = nodeToken["value"].Value<string>();
        int id = value.GetHashCode();

        if (nodeDic.TryGetValue(id, out Node existingNode))
        {
            return existingNode;
        }
        var type = nodeToken["type"].Value<string>();

        var newNode = new Node(id, type, value, repoNamespaces);
        nodeDic.Add(id, newNode);

        return newNode;
    }

    #endregion

    public static void ExtractNodeNamesToProperties(this NodgesDicId nodges)
    {
        var nodesDicId = nodges.NodesDicId;

        nodesDicId.ExtractNodeNamesToProperties();
    }

    public static void ExtractNodeNamesToProperties(this Dictionary<int, Node> idAndNodes)
    {
        foreach (var kvp in idAndNodes)
        {
            Node node = kvp.Value;

            if (node.Type == NodgeType.Literal)
                continue;

            node.NodeNamesToProperties();
        }
    }

    public static void AddRetrievedNames(this NodgesDicId nodges, GraphDbRepositoryDistantUris graphDbRepositoryDistantUris)
    {
        AddRetrievedNames(nodges.NodesDicId, graphDbRepositoryDistantUris);
    }
    
    public static void AddRetrievedNames(this Dictionary<int, Node> idAndNodes, GraphDbRepositoryDistantUris graphDbRepositoryDistantUris)
    {
        var distantUriDict = graphDbRepositoryDistantUris.DistantUriLabels;

        foreach (var idAndNode in idAndNodes)
        {
            var node = idAndNode.Value;

            if (node.Type != NodgeType.Uri)
                continue;

            
            if(!distantUriDict.TryGetValue(node.Uri, out (string, string) distantUriLabel)) // (string,string) -> (skos:prefLabel, Bibliothèque nationale (Francia)
            {
                continue;
            }

            var nodeProperties = node.Properties;

            if (nodeProperties.ContainsKey(distantUriLabel.Item1)) // Already contain edge property
                continue;

            nodeProperties.Add(distantUriLabel.Item1, distantUriLabel.Item2);
        }
    }
}
