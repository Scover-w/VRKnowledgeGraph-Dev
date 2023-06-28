using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LabelNodgeManagerUI : MonoBehaviour
{

    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    GraphConfigurationContainerSO _graphConfigurationContainerSO;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    NodgePool _nodgePool;


    List<LabelNodgeUI> _labelNodgesMainGraph;
    List<LabelNodgeUI> _labelNodgesSubGraph;

    GraphConfiguration _graphConfig;
    Transform _hmdTf;

    bool _displayInDeskMode = false;
    bool _displayInImmersionMode = true;

    bool _inTransitionForSwitchMode = false;

    GraphMode _graphMode = GraphMode.Desk;

    Vector2 _baseSizeCanvas = Settings.BASE_SIZE_LABEL_CANVAS;
    float _baseFontSize = Settings.BASE_FONT_SIZE_LABEL;


    async void Start()
    {
        _labelNodgesMainGraph = new();
        _labelNodgesSubGraph = new();
        _hmdTf = _referenceHolderSO.HMDCamSA.Value.transform;
        _graphManager.OnGraphUpdate += OnGraphUpdated;

        _graphConfig = await _graphConfigurationContainerSO.GetGraphConfiguration();
    }

    void Update()
    {
        UpdateLabelNodges();
    }


    public void StyleLabels(StyleChange styleChange)
    {
        if (styleChange.HasChanged(StyleChangeType.MainGraph))
            StyleLabelFromMainGraph(styleChange);

        if (styleChange.HasChanged(StyleChangeType.SubGraph))
            StyleLabelFromSubGraph(styleChange);
    }

    private void StyleLabelFromMainGraph(StyleChange styleChange)
    {

        if (_graphMode == GraphMode.Immersion && styleChange.HasChanged(StyleChangeType.ImmersionMode))
        {
            if (styleChange.HasChanged(StyleChangeType.Size))
                SetSizeLabels(_labelNodgesMainGraph, _graphConfig.LabelNodeSizeImmersion);

            if (styleChange.HasChanged(StyleChangeType.Visibility))
            {
                _displayInImmersionMode = _graphConfig.ShowLabelImmersion;
                SetVisibilityLabels(_labelNodgesMainGraph, _displayInImmersionMode);
            }
            return;
        }

        if (!(_graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChangeType.DeskMode)))
            return;

        if (styleChange.HasChanged(StyleChangeType.Size))
            SetSizeLabels(_labelNodgesMainGraph, _graphConfig.LabelNodeSizeDesk);


        if (styleChange.HasChanged(StyleChangeType.Visibility))
        {
            _displayInDeskMode = _graphConfig.ShowLabelDesk;
            SetVisibilityLabels(_labelNodgesMainGraph, _graphConfig.ShowLabelDesk);
        }
    }

    private void StyleLabelFromSubGraph(StyleChange styleChange)
    {
        // No labels for subgraph immersion mode/gps 


        if (!(_graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChangeType.DeskMode)))
            return;

        if (styleChange.HasChanged(StyleChangeType.Size))
            SetSizeLabels(_labelNodgesSubGraph, _graphConfig.LabelNodeSizeDesk);


        if (styleChange.HasChanged(StyleChangeType.Visibility))
        {
            _displayInDeskMode = _graphConfig.ShowLabelDesk;
            SetVisibilityLabels(_labelNodgesSubGraph, _displayInDeskMode);
        }
    }


    private void SetSizeLabels(List<LabelNodgeUI> labels, float scale)
    {
        Vector2 sizeConvas = _baseSizeCanvas * scale;
        float fontSize = _baseFontSize * scale;

        foreach (LabelNodgeUI label in labels)
        {
            label.SetSize(sizeConvas, fontSize);
        }
    }

    private void SetVisibilityLabels(List<LabelNodgeUI> labels, bool isVisible)
    {
        foreach (LabelNodgeUI label in labels)
        {
            label.SetActive(isVisible);
        }
    }

   

    public void OnGraphUpdated(GraphUpdateType updateType)
    {
        switch (updateType)
        {
            case GraphUpdateType.BeforeSimulationStart:

                break;
            case GraphUpdateType.AfterSimulationHasStopped:

                break;
            case GraphUpdateType.BeforeSwitchMode:
                HideLabelsForTransition();
                break;
            case GraphUpdateType.AfterSwitchModeToDesk:
                _graphMode = GraphMode.Immersion;
                SetLabelsAfterSwitchMode();
                break;
            case GraphUpdateType.AfterSwitchModeToImmersion:
                _graphMode = GraphMode.Immersion;
                SetLabelsAfterSwitchMode();
                break;
        }
    }

    private void HideLabelsForTransition()
    {
        _inTransitionForSwitchMode = true;

        foreach (LabelNodgeUI label in _labelNodgesMainGraph)
        {
            label.SetActive(false);
        }

        foreach (LabelNodgeUI label in _labelNodgesSubGraph)
        {
            label.SetActive(false);
        }
    }

    private void SetLabelsAfterSwitchMode()
    {
        if(_graphMode == GraphMode.Immersion)
        {
            float scale = _graphConfig.LabelNodeSizeImmersion;
            Vector2 sizeConvas = _baseSizeCanvas * scale;
            float fontSize = _baseFontSize * scale;

            foreach (LabelNodgeUI label in _labelNodgesMainGraph)
            {
                label.SetAll(_displayInImmersionMode, sizeConvas, fontSize);
            }

            return;
        }


        if(_graphMode == GraphMode.Desk && _displayInDeskMode)
        {

            float scale = _graphConfig.LabelNodeSizeDesk;
            Vector2 sizeConvas = _baseSizeCanvas * scale;
            float fontSize = _baseFontSize * scale;

            foreach (LabelNodgeUI label in _labelNodgesMainGraph)
            {
                label.SetAll(_displayInDeskMode, sizeConvas, fontSize);
            }

            scale = _graphConfig.LabelNodeSizeLens;
            sizeConvas = _baseSizeCanvas * scale;
            fontSize = _baseFontSize * scale;

            foreach (LabelNodgeUI label in _labelNodgesSubGraph)
            {
                label.SetAll(_displayInDeskMode, sizeConvas, fontSize);
            }
        }
    }

    private void UpdateLabelNodges()
    {
        if (_labelNodgesMainGraph == null)
            return;

        Vector3 hmdPosition = _hmdTf.position;

        float nodeSizeMain = GetNodeSize(GraphType.Main);


        foreach(LabelNodgeUI nodgeUI in _labelNodgesMainGraph)
        {
            nodgeUI.UpdateTransform(hmdPosition, nodeSizeMain);
        }


        if (_graphMode == GraphMode.Immersion) // if subgraph is in gps mode
            return;

        float nodeSizeSub = GetNodeSize(GraphType.Sub);

        foreach (LabelNodgeUI nodgeUI in _labelNodgesSubGraph)
        {
            nodgeUI.UpdateTransform(hmdPosition, nodeSizeMain);
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

    private float GetNodeSize(GraphType graphType)
    {
        bool isChangingSize = _graphConfig.SelectedMetricTypeSize != GraphMetricType.None;


        if(_graphMode == GraphMode.Immersion) // Don't handle gps mode because don't display them in it
        {
            if (isChangingSize)
                return _graphConfig.NodeMaxSizeImmersion;
            else
                return _graphConfig.NodeSizeImmersion;
        }


        if (graphType == GraphType.Main) // Desk Mode
        {
            if (isChangingSize)
                return _graphConfig.NodeMaxSizeDesk;
            else
                return _graphConfig.NodeSizeDesk;
        }
        else // Lens Mode
        {
            if (isChangingSize)
                return _graphConfig.NodeMaxSizeLens;
            else
                return _graphConfig.NodeSizeLens;
        }
    }
}
