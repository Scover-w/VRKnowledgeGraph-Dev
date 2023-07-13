using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class NodgeCreator : MonoBehaviour
{
    //private void RefreshPositions(NodgesDicId nodges, GraphConfiguration config)
    //{
    //    var idAndNodes = nodges.NodesDicId;
    //    int seed = config.SeedRandomPosition;


    //    foreach (var idAndNode in idAndNodes)
    //    {
    //        idAndNode.Value.ResetAbsolutePosition(seed);
    //    }
    //}

    //private void GetCentralNode(NodgesDicId nodges)
    //{
    //    int nb = -1;
    //    Node centralNode = new("","");

    //    var nodesDicId = nodges.NodesDicId;

    //    foreach (var kvp in nodesDicId)
    //    {
    //        Node node = kvp.Value;

    //        int nbEdge = node.EdgeSource.Count + node.EdgeTarget.Count;

    //        if (nbEdge < nb)
    //            continue;

    //        centralNode = node;
    //        nb = nbEdge;
    //    }
    //}
}
