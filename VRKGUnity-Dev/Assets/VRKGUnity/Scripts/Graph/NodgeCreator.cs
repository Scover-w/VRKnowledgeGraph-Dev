using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Codice.Client.Common.TreeGrouper;
using Codice.CM.Common.Tree;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using Unity.VisualScripting.YamlDotNet.RepresentationModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class NodgeCreator : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;

    GraphDBAPI _api;

    bool _isFirstRetrieval = true;

    [Range(1,10)]
    public int LabelNodgePropagation = 1;


    HashSet<string> _propertiesNameToMerge;

    private void Start()
    {
        _api = new GraphDBAPI();
        CreatePropertiesName();

    }

    public async Task<Nodges> RetreiveGraph(string query, GraphConfiguration config)
    {
        var debugChrono = DebugChrono.Instance;

        debugChrono.Start("RetrieveAll");
        var nodges = await Retrieve(query);
        debugChrono.Stop("RetrieveAll");

        try
        {
            if(_isFirstRetrieval)
            {
                await _graphManager.NodeUriRetriever.RetrieveNames(nodges.NodesDicId);
                _isFirstRetrieval = false;
            }
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogError(e.Message);
        }


        debugChrono.Start("MergeProperties");
        MergePropertiesNode(nodges);
        debugChrono.Stop("MergeProperties");

        return nodges;
    }

    private async Task<Nodges> Retrieve(string query)
    {
        var json = await _api.Query(query); 

        var data = JsonConvert.DeserializeObject<JObject>(json);

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


            int sId = (sType + sValue).GetHashCode();
            int oId = (oType + oValue).GetHashCode();

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

            if(edgesDicId.TryGetValue(edge.Id, out Edge edgeExisting))
            {
                continue;
            }

            edgesDicId.Add(edge.Id, edge);

            s.EdgeSource.Add(edge);
            o.EdgeTarget.Add(edge);
        }

        return nodges;
    }


    private void MergePropertiesNode(Nodges nodges)
    {
        // Remove Nodes than can be properties for other nodes
        List<Node> nodeToRemove = new();
        List<Edge> edgeToRemove = new();


        var edgesDicId = nodges.EdgesDicId;
        var nodesDicId = nodges.NodesDicId;


        foreach (var kvp in nodesDicId)
        {
            Node node = kvp.Value;

            if (node.Type != "literal")
                continue;

            bool onlyOnetarget = (node.EdgeSource.Count == 0 && node.EdgeTarget.Count == 1);
            bool onlyOneSource = (node.EdgeSource.Count == 1 && node.EdgeTarget.Count == 0);

            if ( !(onlyOneSource || onlyOnetarget))
                continue;

            Edge edge = onlyOnetarget? node.EdgeTarget[0] : node.EdgeSource[0];

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


    private void RefreshPositions(Nodges nodges, GraphConfiguration config)
    {
        var idAndNodes = nodges.NodesDicId;
        int seed = config.SeedRandomPosition;


        foreach (var idAndNode in idAndNodes)
        {
            idAndNode.Value.ResetPosition(seed);
        }
    }

    private void GetCentralNode(Nodges nodges)
    {
        int nb = -1;
        Node centralNode = new("","");

        var nodesDicId = nodges.NodesDicId;

        foreach (var kvp in nodesDicId)
        {
            Node node = kvp.Value;

            int nbEdge = node.EdgeSource.Count + node.EdgeTarget.Count;

            if (nbEdge < nb)
                continue;

            centralNode = node;
            nb = nbEdge;
        }
    }

    private void CreatePropertiesName()
    {
        _propertiesNameToMerge = new();
        _propertiesNameToMerge.Add("http://purl.org/dc/terms/description");
        _propertiesNameToMerge.Add("http://www.w3.org/2004/02/skos/core#altLabel");
        _propertiesNameToMerge.Add("http://www.w3.org/2004/02/skos/core#prefLabel");
        _propertiesNameToMerge.Add("http://www.w3.org/2000/01/rdf-schema#label"); 
        _propertiesNameToMerge.Add("http://purl.org/dc/terms/title");
    }
}
