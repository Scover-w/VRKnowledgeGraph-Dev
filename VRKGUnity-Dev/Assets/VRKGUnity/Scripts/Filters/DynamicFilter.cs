using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicFilter
{
    public SPARQLQuery Query { get { return _sparqlQuery; } }

    public HashSet<Node> HiddenNodes;
    public HashSet<Edge> HiddenEdges;


    DynamicFilterType FilterType;

    SPARQLQuery _sparqlQuery;


    public DynamicFilter(HashSet<Node> nodes)
    {
        HiddenNodes = nodes;
        FilterType = DynamicFilterType.ExcludeAll;

        _sparqlQuery = new SPARQLQuery(nodes, FilterType);
    }

    public DynamicFilter(HashSet<Node> displayedNodes, HashSet<Node> hiddenNodes)
    {
        HiddenNodes = hiddenNodes;

        if (displayedNodes.Count > hiddenNodes.Count)
        {
            FilterType = DynamicFilterType.ExcludeAll;
            _sparqlQuery = new SPARQLQuery(hiddenNodes, FilterType);
        }
        else
        {
            FilterType = DynamicFilterType.IncludeOnly;
            _sparqlQuery = new SPARQLQuery(displayedNodes, FilterType);
        }
    }


    public void ReleaseNodges(NodgePool pool)
    {
        foreach(Node node in HiddenNodes)
        {
            pool.Release(node.MainStyler);
            pool.Release(node.SubStyler);
        }

        foreach (Edge edge in HiddenEdges)
        {
            pool.Release(edge.MainStyler);
            pool.Release(edge.SubStyler);
        }
    }

}

public enum DynamicFilterType
{
    IncludeOnly,
    ExcludeAll
}