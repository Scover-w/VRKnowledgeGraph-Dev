using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StylingManager : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    GraphStyling _graphStyling;

    [SerializeField]
    SubGraph _subGraph;

    [SerializeField]
    LabelNodgeManagerUI _labelNodgeManagerUI;

    GraphConfiguration _graphConfig;


    void Start()
    {
        _graphConfig = GraphConfiguration.Instance;
    }


    public void UpdateStyling(StyleChange styleChange)
    {
        if (_graphConfig == null)
            return;

        if (styleChange.HasChanged(StyleChange.Label))
            _labelNodgeManagerUI.StyleLabels(styleChange);


        if (styleChange.HasChanged(StyleChange.SubGraph)
            && styleChange.HasChanged(StyleChange.ImmersionMode)
            && styleChange.HasChanged(StyleChange.Visibility))
            _subGraph.SwitchWatchVisibility();

        _graphStyling.StyleGraph(styleChange, _graphManager.GraphMode);

        _ = _graphConfig.Save();
    }
}
