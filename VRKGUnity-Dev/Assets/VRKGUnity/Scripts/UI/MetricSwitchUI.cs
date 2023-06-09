using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetricSwitchUI : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    TMP_Dropdown _dropdownSize;

    [SerializeField]
    TMP_Dropdown _dropdownColor;

    [SerializeField]
    GraphStyling _graphStyling;

    GraphConfiguration _graphConfig;


    private void Start()
    {
        _graphConfig = GraphConfiguration.Instance;

        _dropdownSize.value = (int)_graphConfig.SelectedMetricTypeSize;
        _dropdownColor.value = (int)_graphConfig.SelectedMetricTypeColor;
    }

    public async void OnSizeMetricChanged()
    {
        int id = _dropdownSize.value;

        GraphMetricType newMetric = (GraphMetricType)id;


        _graphConfig.SelectedMetricTypeSize = newMetric;


        var styleChange = new StyleChange().Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Size);

        _graphStyling.StyleGraph(styleChange, _graphManager.GraphMode);

        await _graphConfig.Save();
    }

    public async void OnColorMetricChanged()
    {
        int id = _dropdownColor.value;
        GraphMetricType newMetric = (GraphMetricType)id;


        _graphConfig.SelectedMetricTypeColor = newMetric;


        var styleChange = new StyleChange().Add(StyleChangeType.SubGraph)
                .Add(StyleChangeType.MainGraph)
                .Add(StyleChangeType.DeskMode)
                .Add(StyleChangeType.ImmersionMode)
                .Add(StyleChangeType.Node)
                .Add(StyleChangeType.Color);

        _graphStyling.StyleGraph(styleChange, _graphManager.GraphMode);


        await _graphConfig.Save();
    }

}
