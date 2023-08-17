using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class NodgesHelper
{
    #region DataSync
    public static NodgesDicUID ExtractNodgesForDistantUri(this JObject data, GraphDbRepositoryNamespaces repoNamespaces)
    {
        var nodges = new NodgesDicUID();

        var nodesDicUID = nodges.NodesDicUID;
        var edgesDicUID = nodges.EdgesDicUID;

        foreach (JToken binding in data["results"]["bindings"])
        {
            var sToken = binding["s"];
            string sType = sToken["type"].Value<string>();

            var pToken = binding["p"];
            string pType = pToken["type"].Value<string>();
            string pValue = pToken["value"].Value<string>();

            var oToken = binding["o"];
            string oType = oToken["type"].Value<string>();


            Node sNode = GetNodeFromDictOrCreate(sToken, nodesDicUID, repoNamespaces);


            if (oType == "literal" && sType == "uri") // oNode is a literal
                continue;

            if (pValue == "http://xmlns.com/foaf/0.1/depiction") // oNode is a media
                continue;


            Node oNode = GetNodeFromDictOrCreate(oToken, nodesDicUID, repoNamespaces);

            string edgeUID = Edge.GetUID(sNode.Uri, oNode.Uri);

            

            if (edgesDicUID.TryGetValue(edgeUID, out Edge existingEdge))
            {
                existingEdge.AddProperty(pType, pValue, sNode, repoNamespaces);
                continue;
            }

            var edge = new Edge(pType, pValue, sNode, oNode, repoNamespaces);

            edgesDicUID.Add(edge.UID, edge);

            sNode.EdgeSource.Add(edge);
            oNode.EdgeTarget.Add(edge);
        }

        return nodges;
    }

    public static Dictionary<string, Node> GetNoLabeledNodes(this Dictionary<string, Node> uidAndNodes)
    {
        Dictionary<string,Node> nolabeled = new();

        foreach (var uidAndNode in uidAndNodes)
        {
            var node = uidAndNode.Value;

            if (node.Type != NodgeType.Uri)
                continue;

            if (node.DoesPropertiesContainName())
                continue;

            nolabeled.Add(uidAndNode.Key, uidAndNode.Value);
        }

        return nolabeled;
    }

    public static Dictionary<string, Node> GetNoOntoUriNodes(this Dictionary<string, Node> uidAndNodes, IReadOnlyDictionary<string, OntologyTree> ontoTreeDict)
    {

        Dictionary<string, Node> noOntoned = new();

        foreach (var uidAndNode in uidAndNodes)
        {
            var node = uidAndNode.Value;

            if (node.Type != NodgeType.Uri)
                continue;

            var namespce = node.Namespace;

            if (!ontoTreeDict.TryGetValue(namespce, out OntologyTree ontoTree))
            {
                noOntoned.Add(uidAndNode.Key, uidAndNode.Value);
                continue;
            }

            if (!ontoTree.TryGetOntoNode(uidAndNode.Key, out _))
            {
                noOntoned.Add(uidAndNode.Key, uidAndNode.Value);
                continue;
            }
        }

        return noOntoned;
    }
    #endregion


    #region Graph
    public static async Task<NodgesDicUID> RetrieveGraph(string query, GraphDbRepository repo)
    {
        var debugChrono = DebugChrono.Instance;
        debugChrono.Start("RetreiveGraph");
        var api = repo.GraphDBAPI;
        var json = await api.SelectQuery(query, true);
        var data = await JsonConvertHelper.DeserializeObjectAsync<JObject>(json);


        NodgesDicUID nodges = data.ExtractNodges(repo.GraphDbRepositoryNamespaces);
        nodges.AddRetrievedNames(repo.GraphDbRepositoryDistantUris);


        debugChrono.Stop("RetreiveGraph");

        return nodges;
    }

    public static NodgesDicUID ExtractNodges(this JObject data, GraphDbRepositoryNamespaces repoNamespaces)
    {
        var nodges = new NodgesDicUID();

        var nodesDicId = nodges.NodesDicUID;
        var edgesDicId = nodges.EdgesDicUID;

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


            Node sNode = GetNodeFromDictOrCreate(sToken, nodesDicId, repoNamespaces);

            if(isObjectPossiblyAnOnto)
            {
                var simpleOntoNode = new Node(oType, oValue, repoNamespaces);

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
                AddToNodeAsMedia(sNode, oValue);
                continue;
            }

            Node oNode = GetNodeFromDictOrCreate(oToken, nodesDicId, repoNamespaces);


            string edgeUID = Edge.GetUID(sNode.Uri, oNode.Uri);

            if (edgesDicId.TryGetValue(edgeUID, out Edge existingEdge))
            {
                existingEdge.AddProperty(pType, pValue, sNode, repoNamespaces);
                continue;
            }

            var edge = new Edge(pType, pValue, sNode, oNode, repoNamespaces);

            edgesDicId.Add(edge.UID, edge);

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


        Debug.Log("AddToNodeAsProperty : " + edgeUri + "  /  " + propValue);
        node.Properties.Add(edgeUri, propValue);
    }

    private static void AddToNodeAsMedia(Node node, string propValue)
    {
        if (node.Medias.Contains(propValue))
            return;

        node.Medias.Add(propValue);
    }

    private static Node GetNodeFromDictOrCreate(JToken nodeToken, Dictionary<string, Node> nodeDic, GraphDbRepositoryNamespaces repoNamespaces)
    {
        string value = nodeToken["value"].Value<string>();
        string uid = value;

        if (nodeDic.TryGetValue(uid, out Node existingNode))
        {
            return existingNode;
        }
        var type = nodeToken["type"].Value<string>();

        var newNode = new Node(type, value, repoNamespaces);
        nodeDic.Add(uid, newNode);

        return newNode;
    }

    #endregion

    public static void AddRetrievedNames(this NodgesDicUID nodges, GraphDbRepositoryDistantUris graphDbRepositoryDistantUris)
    {
        AddRetrievedNames(nodges.NodesDicUID, graphDbRepositoryDistantUris);
    }
    
    public static void AddRetrievedNames(this Dictionary<string, Node> uidAndNodes, GraphDbRepositoryDistantUris graphDbRepositoryDistantUris)
    {
        var distantUriDict = graphDbRepositoryDistantUris.DistantUriLabels;

        foreach (var uidAndNode in uidAndNodes)
        {
            var node = uidAndNode.Value;

            if (node.Type != NodgeType.Uri)
                continue;

            
            if(!distantUriDict.TryGetValue(node.Uri, out (string, string) distantUriLabel)) // (string,string) -> (skos:prefLabel, Bibliothèque nationale (Francia)
            {
                continue;
            }

            var nodeProperties = node.Properties;

            if (nodeProperties.ContainsKey(distantUriLabel.Item1)) // Already contain edge property
                continue;

            if (distantUriLabel.Item1 == "-1")
                continue;

            nodeProperties.Add(distantUriLabel.Item1, distantUriLabel.Item2);
        }
    }
}
