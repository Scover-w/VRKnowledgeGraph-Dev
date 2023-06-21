using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class NodgeCreator : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GraphManager _graphManager;

    GraphDBAPI _api;

    bool _isFirstRetrieval = true;


    public async Task<Nodges> RetreiveGraph(string query, GraphConfiguration config)
    {
        var debugChrono = DebugChrono.Instance;
        var repo = _referenceHolderSo.SelectedGraphDbRepository;
        debugChrono.Start("RetreiveGraph");
        _api = repo.GraphDBAPI;
        var json = await _api.SelectQuery(query,true);
        var data = JsonConvert.DeserializeObject<JObject>(json);


        var nodges = data.ExtractNodges(repo.GraphDbRepositoryUris);
        nodges.AddRetrievedNames(repo.GraphDbRepositoryDistantUris);
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
