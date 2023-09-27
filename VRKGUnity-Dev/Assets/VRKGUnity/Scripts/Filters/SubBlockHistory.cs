using System.Collections.Generic;

public class SubBlockHistory
{
    public SPARQLQuery Query { get { return _sparqlQuery; } }

    public IReadOnlyList<string> NodesUidsHidden => _nodesUidsHidden;
    public IReadOnlyList<string> EdgesUidsHidden => _edgesUidsHidden;

    readonly List<string> _nodesUidsHidden;
    readonly List<string> _edgesUidsHidden;

    readonly DynamicFilterType FilterType;
    readonly SPARQLQuery _sparqlQuery;

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
            if(hiddenNode.IsHiddenFromFilter)
            {
                DebugDev.LogWarning("SubBlockHistory : Tried to hide a node already hidden.");
                continue;
            }


            foreach(var hiddenEdge in hiddenNode.Edges)
            {
                string edgeUId = hiddenEdge.UID;

                if (hiddenEdge.IsHiddenFromFilter) // Already hidden by another filter
                    continue;

                if (edgesToHideDict.ContainsKey(edgeUId))
                    continue;

                edgesToHideDict.Add(edgeUId, hiddenEdge);
                _edgesUidsHidden.Add(edgeUId);
            }

            string nodeUId = hiddenNode.UID;

            if (nodesToHideDict.ContainsKey(nodeUId))
                continue;

            nodesToHideDict.Add(nodeUId, hiddenNode);
            _nodesUidsHidden.Add(nodeUId);
        }
    }
}
