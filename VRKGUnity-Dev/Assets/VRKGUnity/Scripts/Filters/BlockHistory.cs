using AngleSharp.Dom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VDS.RDF.Query;

public class BlockHistory
{
    public int NbSubBlock { get { return _subsHistory.Count; } }

    public SPARQLAdditiveBuilder SPARQLAdditiveBuilder;

    List<SubBlockHistory> _subsHistory;

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
}
