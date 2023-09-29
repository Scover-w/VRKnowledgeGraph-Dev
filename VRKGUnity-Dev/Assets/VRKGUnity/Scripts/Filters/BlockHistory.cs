using AngleSharp.Dom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Contain all the filters changes between two SPARQL query
/// </summary>
public class BlockHistory
{
    public int NbSubBlock { get { return _subsHistory.Count; } }

    public IReadOnlyList<SubBlockHistory> SubsHistory => _subsHistory;

    readonly List<SubBlockHistory> _subsHistory;

    public BlockHistory() 
    {
        _subsHistory = new();
    }

    public void Add(SubBlockHistory subBlockHistory)
    {
        _subsHistory.Add(subBlockHistory);
    }

    public SubBlockHistory CancelLast()
    {
        var last = _subsHistory[^1];
        _subsHistory.Remove(last);
        return last;
    }

    public void AddQueries(SPARQLAdditiveBuilder sPARQLAdditiveBuilder)
    {
        foreach(var subBlockHistory in _subsHistory) 
        {
            sPARQLAdditiveBuilder.Add(subBlockHistory.Query);
        }
    }
}
