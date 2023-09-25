using System.Collections.Generic;

public class SubBlockHistory
{
    public SPARQLQuery Query { get { return _sparqlQuery; } }

    public IReadOnlyList<string> NodesUidsHidden => _nodesUidsHidden;
    public IReadOnlyList<string> EdgesUidsHidden => _edgesUidsHidden;

    List<string> _nodesUidsHidden;
    List<string> _edgesUidsHidden;

    DynamicFilterType FilterType;

    SPARQLQuery _sparqlQuery;

    public SubBlockHistory(HashSet<Node> hiddenNodes, out NodgesDicUID nodgeDicUID) 
    {
        FilterType = DynamicFilterType.ExcludeAll;

        _nodesUidsHidden = new();
        _edgesUidsHidden = new();

        nodgeDicUID = new();
        var nodesToHideDict = nodgeDicUID.NodesDicUID;
        var edgesToHideDict = nodgeDicUID.EdgesDicUID;

        _sparqlQuery = new SPARQLQuery(hiddenNodes, FilterType);

        foreach (var hiddenNode in hiddenNodes) 
        {
            string nodeUId = hiddenNode.UID;
            nodesToHideDict.Add(nodeUId, hiddenNode);
            _nodesUidsHidden.Add(nodeUId);

            foreach(var hidenEdge in hiddenNode.Edges)
            {
                string edgeUId = hidenEdge.UID;
                edgesToHideDict.Add(edgeUId, hidenEdge);
                _edgesUidsHidden.Add(edgeUId);
            }
        }
    }
}
