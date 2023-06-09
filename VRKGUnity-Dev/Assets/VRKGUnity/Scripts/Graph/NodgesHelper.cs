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
            string sType = binding["s"]["type"].Value<string>();
            string sValue = binding["s"]["value"].Value<string>();

            string pType = binding["p"]["type"].Value<string>();
            string pValue = binding["p"]["value"].Value<string>();

            string oType = binding["o"]["type"].Value<string>();
            string oValue = binding["o"]["value"].Value<string>();


            if (ontoUris.Contains(sValue) || ontoUris.Contains(oValue))
                continue;

            int sId = sValue.GetHashCode();
            int oId = oValue.GetHashCode();

            Node s;
            Node o;

            if (nodesDicId.TryGetValue(sId, out Node sNodeExisting))
            {
                s = sNodeExisting;
            }
            else
            {
                s = new Node(sId, sType, sValue, repoNamespaces);
                nodesDicId.Add(sId, s);
            }

            if (nodesDicId.TryGetValue(oId, out Node oNodeExisting))
            {
                o = oNodeExisting;
            }
            else
            {
                o = new Node(oId, oType, oValue, repoNamespaces);
                nodesDicId.Add(oId, o);
            }


            var edge = new Edge(pType, pValue, s, o, repoNamespaces);

            if (edgesDicId.TryGetValue(edge.Id, out Edge edgeExisting))
            {
                continue;
            }

            edgesDicId.Add(edge.Id, edge);

            s.EdgeSource.Add(edge);
            o.EdgeTarget.Add(edge);
        }

        Debug.Log("Retrieve Data : Nb Nodes : " + nodesDicId.Count + " , Nb Edges : " + edgesDicId.Count);

        nodges.MergePropertiesNodes();

        return nodges;
    }


    public static NodgesDicId ExtractNodgesForDistantUri(this JObject data, GraphDbRepositoryNamespaces repoNamespaces)
    {
        var nodges = new NodgesDicId();

        var nodesDicId = nodges.NodesDicId;
        var edgesDicId = nodges.EdgesDicId;

#if UNITY_EDITOR && FALSE
        var folderPath = Path.Combine(Application.dataPath, "VRKGUnity", "Data");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        await File.WriteAllTextAsync(Path.Combine(folderPath, "AllQuery.json"), json);
#endif

        foreach (JToken binding in data["results"]["bindings"])
        {
            string sType = binding["s"]["type"].Value<string>();
            string sValue = binding["s"]["value"].Value<string>();

            string pType = binding["p"]["type"].Value<string>();
            string pValue = binding["p"]["value"].Value<string>();

            string oType = binding["o"]["type"].Value<string>();
            string oValue = binding["o"]["value"].Value<string>();


            int sId = sValue.GetHashCode();
            int oId = oValue.GetHashCode();

            Node s;
            Node o;

            if (nodesDicId.TryGetValue(sId, out Node sNodeExisting))
            {
                s = sNodeExisting;
            }
            else
            {
                s = new Node(sId, sType, sValue, repoNamespaces);
                nodesDicId.Add(sId, s);
            }

            if (nodesDicId.TryGetValue(oId, out Node oNodeExisting))
            {
                o = oNodeExisting;
            }
            else
            {
                o = new Node(oId, oType, oValue, repoNamespaces);
                nodesDicId.Add(oId, o);
            }

            var edge = new Edge(pType, pValue, s, o, repoNamespaces);

            if (edgesDicId.TryGetValue(edge.Id, out Edge edgeExisting))
            {
                continue;
            }

            edgesDicId.Add(edge.Id, edge);

            s.EdgeSource.Add(edge);
            o.EdgeTarget.Add(edge);
        }

        nodges.MergePropertiesNodes();

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

            if (!ontoTree.TryGetOntoNode(idAndNode.Key, out OntoNode ontoNode))
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
            string sType = binding["s"]["type"].Value<string>();
            string sValue = binding["s"]["value"].Value<string>();

            string pType = binding["p"]["type"].Value<string>();
            string pValue = binding["p"]["value"].Value<string>();

            string oType = binding["o"]["type"].Value<string>();
            string oValue = binding["o"]["value"].Value<string>();

            if (repoNamespaces.IsUriAnOnto(sValue))
                continue;

            bool isObjectPossiblyAnOnto = repoNamespaces.IsUriAnOnto(oValue) && pValue == "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";

            int sId = sValue.GetHashCode();
            int oId = oValue.GetHashCode();

            Node s;
            Node o;

            if (nodesDicId.TryGetValue(sId, out Node sNodeExisting))
            {
                s = sNodeExisting;
            }
            else
            {
                s = new Node(sId, sType, sValue, repoNamespaces);
                nodesDicId.Add(sId, s);
            }

            if(isObjectPossiblyAnOnto)
            {
                var simpleOntoNode = new Node(oId, oType, oValue, repoNamespaces);

                if (ontologyLinker.TryEstablishLink(s, simpleOntoNode))
                    continue;
            }

            if (nodesDicId.TryGetValue(oId, out Node oNodeExisting))
            {
                o = oNodeExisting;
            }
            else
            {
                o = new Node(oId, oType, oValue, repoNamespaces);
                nodesDicId.Add(oId, o);
            }


            var edge = new Edge(pType, pValue, s, o, repoNamespaces);

            if (edgesDicId.TryGetValue(edge.Id, out Edge edgeExisting))
            {
                continue;
            }

            edgesDicId.Add(edge.Id, edge);

            s.EdgeSource.Add(edge);
            o.EdgeTarget.Add(edge);
        }

        ontologyLinker.AttachNodesToOntoNodes();

        nodges.MergePropertiesNodes();

        return nodges;
    }

    #endregion


    /// <summary>
    /// Merge litteral nodes to its only node connection in its properties.
    /// Allow to compact the graph by removing solo edge nodes.
    /// </summary>
    /// <param name="nodges"></param>
    private static void MergePropertiesNodes(this NodgesDicId nodges)
    {
        // Remove Nodes than can be properties for other nodes
        List<Node> nodeToRemove = new();
        List<Edge> edgeToRemove = new();


        var edgesDicId = nodges.EdgesDicId;
        var nodesDicId = nodges.NodesDicId;


        foreach (var kvp in nodesDicId)
        {
            Node node = kvp.Value;

            if (node.Type != NodgeType.Literal)
                continue;

            bool onlyOneTarget = (node.EdgeSource.Count == 0 && node.EdgeTarget.Count == 1);
            bool onlyOneSource = (node.EdgeSource.Count == 1 && node.EdgeTarget.Count == 0);

            if (!(onlyOneSource || onlyOneTarget)) // Is link to multiple nodes
            {
                // TODO : Get labels and all to properties
                continue;
            }

            bool nodeToMergeIsATarget = onlyOneTarget;

            Edge edge;
            Node nodeToAddProperty;

            if(nodeToMergeIsATarget)
            {
                edge = node.EdgeTarget[0];
                nodeToAddProperty = edge.Source;

                if (nodeToAddProperty.Type == NodgeType.Literal)
                    continue;

                nodeToAddProperty.EdgeSource.Remove(edge);
            }
            else
            {
                edge = node.EdgeSource[0];
                nodeToAddProperty = edge.Target;

                if (nodeToAddProperty.Type == NodgeType.Literal)
                    continue;

                nodeToAddProperty.EdgeTarget.Remove(edge);
            }

            if (!nodeToAddProperty.Properties.ContainsKey(edge.Uri))
                nodeToAddProperty.Properties.Add(edge.Uri, node.Uri);

            nodeToRemove.Add(node);
            edgeToRemove.Add(edge);

        }

        foreach (var node in nodeToRemove)
        {
            nodesDicId.Remove(node.Id);
        }

        foreach (var edge in edgeToRemove)
        {
            edgesDicId.Remove(edge.Id);
        }
    }

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

   

    


    public static Dictionary<int, Node> ExtractNodes(this JObject data, GraphDbRepositoryNamespaces repoNamespaces)
    {
        Dictionary<int, Node> nodesDicId = new();

        foreach (JToken binding in data["results"]["bindings"])
        {
            string sType = binding["s"]["type"].Value<string>();
            string sValue = binding["s"]["value"].Value<string>();

            string oType = binding["o"]["type"].Value<string>();
            string oValue = binding["o"]["value"].Value<string>();

            int sId = sValue.GetHashCode();
            int oId = oValue.GetHashCode();

            Node s;
            Node o;

            if (!nodesDicId.ContainsKey(sId))
            {
                s = new Node(sId, sType, sValue, repoNamespaces);
                nodesDicId.Add(sId, s);
            }

            if (!nodesDicId.ContainsKey(oId))
            {
                o = new Node(oId, oType, oValue, repoNamespaces);
                nodesDicId.Add(oId, o);
            }

        }

        return nodesDicId;
    }

    public static void RemoveNodes(this Dictionary<int, Node> nodeIds, Dictionary<int, Node> nodeIdsToRemove)
    {
        foreach(int idNode in nodeIdsToRemove.Keys)
        {
            if(nodeIds.ContainsKey(idNode))
                nodeIds.Remove(idNode);
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
