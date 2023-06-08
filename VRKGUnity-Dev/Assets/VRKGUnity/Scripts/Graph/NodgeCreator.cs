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
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GraphManager _graphManager;

    GraphDBAPI _api;

    bool _isFirstRetrieval = true;

    private void Start()
    {
        _api = _referenceHolderSo.SelectedGraphDbRepository.GraphDBAPI;
    }

    public async Task<Nodges> RetreiveGraph(string query, GraphConfiguration config)
    {
        var debugChrono = DebugChrono.Instance;

        debugChrono.Start("RetreiveGraph");

        var json = await _api.Query(query);
        var data = JsonConvert.DeserializeObject<JObject>(json);


        var nodges = data.ExtractNodges();
        nodges.AddRetrievedNames(_referenceHolderSo.SelectedGraphDbRepository.GraphDbRepositoryDistantUris);
        nodges.ExtractNodeNamesToProperties();

        debugChrono.Stop("RetreiveGraph");

        return nodges;
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
}
