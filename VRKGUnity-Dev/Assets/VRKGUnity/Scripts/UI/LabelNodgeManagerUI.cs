using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabelNodgeManagerUI : MonoBehaviour
{

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    NodgePool _nodgePool;


    List<LabelNodgeUI> _labelNodgesMainGraph;
    List<LabelNodgeUI> _labelNodgesSubGraph;

    bool _displayInDeskMode = false;
    bool _displayInImmersionMode = true;


    private void Start()
    {
        _labelNodgesMainGraph = new();
    }

    void Update()
    {
        UpdateLabelNodges();
    }

    private void UpdateLabelNodges()
    {
        if (_labelNodgesMainGraph == null)
            return;

        int nb = _labelNodgesMainGraph.Count;

        for (int i = 0; i < nb; i++)
        {
            _labelNodgesMainGraph[i].UpdateTransform();
        }
    }

    public void ClearLabelNodges()
    {
        int nb = _labelNodgesMainGraph.Count;

        for (int i = 0; i < nb; i++)
        {
            _nodgePool.Release(_labelNodgesMainGraph[i]);
        }

        _labelNodgesMainGraph = new();
    }

    public LabelNodgeUI GetLabelNodgeUI(GraphType forGraph)
    {
        var labelNodge = _nodgePool.GetLabelNodge();

        if(forGraph == GraphType.Main)
            _labelNodgesMainGraph.Add(labelNodge);
        else
            _labelNodgesSubGraph.Add(labelNodge);

        return labelNodge;
    }
}
