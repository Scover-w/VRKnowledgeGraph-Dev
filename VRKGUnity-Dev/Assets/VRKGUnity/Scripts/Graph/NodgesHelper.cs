using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NodgesHelper
{
    public static Nodges ExtractNodges(this JObject data, GraphDbRepositoryUris repoUri)
    {
        var nodges = new Nodges();

        var nodesDicId = nodges.NodesDicId;
        var edgesDicId = nodges.EdgesDicId;

        repoUri.ResetDefinedNodes();

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


            bool isObjectAnOnto = repoUri.IsUriAnOnto(oValue);

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
                s = new Node(sId, sType, sValue);
                nodesDicId.Add(sId, s);
            }

            if(isObjectAnOnto)
            {
                repoUri.AddNodeToOntoNode(s, new Node(oId, oType, oValue));
                continue;
                
            }

            if (nodesDicId.TryGetValue(oId, out Node oNodeExisting))
            {
                o = oNodeExisting;
            }
            else
            {
                o = new Node(oId, oType, oValue);
                nodesDicId.Add(oId, o);
            }


            var edge = new Edge(pType, pValue, s, o);

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

    public static Nodges ExtractNodgesForDistantUri(this JObject data)
    {
        var nodges = new Nodges();

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
                s = new Node(sId, sType, sValue);
                nodesDicId.Add(sId, s);
            }

            if (nodesDicId.TryGetValue(oId, out Node oNodeExisting))
            {
                o = oNodeExisting;
            }
            else
            {
                o = new Node(oId, oType, oValue);
                nodesDicId.Add(oId, o);
            }

            var edge = new Edge(pType, pValue, s, o);

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
    /// <summary>
    /// Merge litteral nodes to its only node connection in its properties.
    /// Allow to compact the graph by removing solo edge nodes.
    /// </summary>
    /// <param name="nodges"></param>
    private static void MergePropertiesNodes(this Nodges nodges)
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

            bool onlyOnetarget = (node.EdgeSource.Count == 0 && node.EdgeTarget.Count == 1);
            bool onlyOneSource = (node.EdgeSource.Count == 1 && node.EdgeTarget.Count == 0);

            if (!(onlyOneSource || onlyOnetarget))
                continue;

            Edge edge = onlyOnetarget ? node.EdgeTarget[0] : node.EdgeSource[0];

            //bool containPropToMerge = _propertiesNameToMerge.Contains(edge.Value);

            //if (!containPropToMerge)
            //    continue;

            var sourceNode = edge.Source;

            if (!sourceNode.Properties.ContainsKey(edge.Value))
                sourceNode.Properties.Add(edge.Value, node.Value);

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

    public static void ExtractNodeNamesToProperties(this Nodges nodges)
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


    public static void AddRetrievedNames(this Nodges nodges, GraphDbRepositoryDistantUris graphDbRepositoryDistantUris)
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

            
            if(!distantUriDict.TryGetValue(node.Value, out (string, string) distantUriLabel)) // (string,string) -> (skos:prefLabel, Biblioth�que nationale (Francia)
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