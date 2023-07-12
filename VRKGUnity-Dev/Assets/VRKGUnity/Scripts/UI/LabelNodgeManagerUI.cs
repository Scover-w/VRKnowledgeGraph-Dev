using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LabelNodgeManagerUI : MonoBehaviour
{

    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    NodgePool _nodgePool;

    [SerializeField]
    NodgeSelectionManager _selectionManager;


    Dictionary<Node, LabelNodgeUI> _displayedLabelMainNodesDict;
    Dictionary<Node, LabelNodgeUI> _displayedLabelSubNodesDict;

    Dictionary<Edge, LabelNodgeUI> _displayedLabelMainEdgesDict;
    Dictionary<Edge, LabelNodgeUI> _displayedLabelSubEdgesDict;

    GraphConfiguration _graphConfig;
    Transform _hmdTf;

    LabelNodgeUI _hoveredLabelUI;


    bool _displayLabelsDesk;
    bool _displayLabelsImmersion;
    bool _displayLabelsLens;

    bool _inTransitionForSwitchMode = false;
    bool _isRunningSimulation = false;

    bool _hasAlreadyRunASimulation = false;


    // Value Style
    float _sizeMainNode;
    float _sizeSubNode;

    GraphMode _graphMode = Settings.DEFAULT_GRAPH_MODE;

    Vector2 _baseSizeCanvas = Settings.BASE_SIZE_LABEL_CANVAS;
    float _baseFontSize = Settings.BASE_FONT_SIZE_LABEL;

    bool _isHovered = false;


    void Start()
    {
        _displayedLabelMainNodesDict = new();
        _displayedLabelSubNodesDict = new();

        _displayedLabelMainEdgesDict = new();
        _displayedLabelSubEdgesDict = new();

        _hmdTf = _referenceHolderSO.HMDCamSA.Value.transform;
        _graphManager.OnGraphUpdate += OnGraphUpdated;


        _graphConfig = GraphConfiguration.Instance;

        _displayLabelsDesk = _graphConfig.ShowLabelDesk;
        _displayLabelsImmersion = _graphConfig.ShowLabelImmersion;
        _displayLabelsLens = _graphConfig.ShowLabelLens;

        UpdateValueStyle();

        Invoke(nameof(DelayedSubscribe), 1f);
    }


    void DelayedSubscribe()
    {
        _selectionManager.OnNodgesPropagated += OnNodgesPropagated;

        _hoveredLabelUI = _nodgePool.GetLabelNodge();
        _hoveredLabelUI.SetActive(false);
    }


    #region Update
    void Update()
    {
        if ( (_inTransitionForSwitchMode || _isRunningSimulation) && !_hasAlreadyRunASimulation)
            return;

        Vector3 hmdPosition = _hmdTf.position;

        if ( (_graphMode == GraphMode.Desk && _displayLabelsDesk) || (_graphMode == GraphMode.Immersion && _displayLabelsImmersion) )
            UpdateMainLabels(hmdPosition);

        if(_graphMode == GraphMode.Desk && _displayLabelsLens)
            UpdateSubLabels(hmdPosition);

        if (_isHovered)
            UpdateHoveredLabel(hmdPosition);
    }

 
    private void UpdateMainLabels(Vector3 hmdPosition)
    {
        float mainNodeSize = _sizeMainNode;

        foreach (LabelNodgeUI nodgeUI in _displayedLabelMainNodesDict.Values)
        {
            nodgeUI.UpdateTransform(hmdPosition, mainNodeSize);
        }

        foreach (LabelNodgeUI nodgeUI in _displayedLabelMainEdgesDict.Values)
        {
            nodgeUI.UpdateTransform(hmdPosition);
        }
    }

    private void UpdateSubLabels(Vector3 hmdPosition)
    {
        float subNodeSize = _sizeSubNode;

        foreach (LabelNodgeUI nodgeUI in _displayedLabelSubNodesDict.Values)
        {
            nodgeUI.UpdateTransform(hmdPosition, subNodeSize);
        }

        foreach (LabelNodgeUI nodgeUI in _displayedLabelSubEdgesDict.Values)
        {
            nodgeUI.UpdateTransform(hmdPosition);
        }
    }

    private void UpdateHoveredLabel(Vector3 hmdPosition)
    {
        _hoveredLabelUI.UpdateTransform(hmdPosition, _sizeMainNode);
    }



    #endregion

    #region HoverLabel
    public void SetHover(Node node)
    {
        _isHovered = true;
        _hoveredLabelUI.SetFollow(node.MainGraphNodeTf);
        var name = node.GetName();
        _hoveredLabelUI.Text = (name != null) ? name : node.Value;
        _hoveredLabelUI.SetActive(true);
    }

    public void CancelHover()
    {
        _isHovered = false;
        _hoveredLabelUI.SetActive(false);
    }
    #endregion

    #region Styling
    public void StyleLabels(StyleChange styleChange)
    {
        UpdateValueStyle();

        if (styleChange.HasChanged(StyleChangeType.MainGraph))
            StyleForMainGraph(styleChange);

        if (styleChange.HasChanged(StyleChangeType.SubGraph))
            StyleForSubGraph(styleChange);
    }

    private void StyleForMainGraph(StyleChange styleChange)
    {
        if (_graphMode == GraphMode.Immersion && styleChange.HasChanged(StyleChangeType.ImmersionMode))
            StyleImmersionMode(styleChange);

        if (_graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChangeType.DeskMode))
            StyleDeskMode(styleChange);
    }

    private void StyleImmersionMode(StyleChange styleChange)
    {
        if (styleChange.HasChanged(StyleChangeType.Size))
            SetSizeLabels(GraphType.Main, _graphConfig.LabelNodeSizeImmersion);

        if (styleChange.HasChanged(StyleChangeType.Visibility))
        {
            _displayLabelsImmersion = _graphConfig.ShowLabelImmersion;
            SwitchDisplayMain(_displayLabelsImmersion);
        }
    }

    private void StyleDeskMode(StyleChange styleChange)
    {
        if (styleChange.HasChanged(StyleChangeType.Size))
            SetSizeLabels(GraphType.Main, _graphConfig.LabelNodeSizeDesk);


        if (styleChange.HasChanged(StyleChangeType.Visibility))
        {
            _displayLabelsDesk = _graphConfig.ShowLabelDesk;
            SwitchDisplayMain(_displayLabelsDesk);
        }
    }

    private void StyleForSubGraph(StyleChange styleChange)
    {
        if (_graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChangeType.DeskMode))
            StyleLensMode(styleChange);

        // Don't style watch because don't have labels
    }

    private void StyleLensMode(StyleChange styleChange)
    {
        if (styleChange.HasChanged(StyleChangeType.Size))
            SetSizeLabels(GraphType.Sub, _graphConfig.LabelNodeSizeLens);


        if (styleChange.HasChanged(StyleChangeType.Visibility))
        {
            _displayLabelsLens = _graphConfig.ShowLabelLens;
            SwitchDisplayLens(_displayLabelsLens);
        }
    }


    private void SetSizeLabels(GraphType graphType, float scaleSize)
    {
        var displayedNodesLabels = (graphType == GraphType.Main) ? _displayedLabelMainNodesDict : _displayedLabelSubNodesDict;
        var displayedEdgeslabels = (graphType == GraphType.Main) ? _displayedLabelMainEdgesDict : _displayedLabelSubEdgesDict;

        Vector2 sizeConvas = _baseSizeCanvas * scaleSize;
        float fontSize = _baseFontSize * scaleSize;

        foreach (LabelNodgeUI label in displayedNodesLabels.Values)
        {
            label.SetSize(sizeConvas, fontSize);
        }

        foreach (LabelNodgeUI label in displayedEdgeslabels.Values)
        {
            label.SetSize(sizeConvas, fontSize);
        }
    }

    private void SwitchDisplayMain(bool displayLabels)
    {
        if (displayLabels)
            CreateMainLabels();
        else
            ReleaseMainLabels();
    }


    private void SwitchDisplayLens(bool displayLabels)
    {
        if (displayLabels)
            CreateSubLabels();
        else
            ReleaseSubLabels();
    }

    private float GetLabelSize(GraphType graphType)
    {
        if(_graphMode == GraphMode.Desk)
        {
            return (graphType == GraphType.Main) ? _graphConfig.LabelNodeSizeDesk : _graphConfig.LabelNodeSizeLens;
        }


        // No Labels Watch
        return _graphConfig.LabelNodeSizeImmersion;
       
    }
    #endregion


    #region OnGraphUpdated
    public void OnGraphUpdated(GraphUpdateType updateType)
    {
        switch (updateType)
        {
            case GraphUpdateType.BeforeSimulationStart:
                _isRunningSimulation = true;
                break;
            case GraphUpdateType.AfterSimulationHasStopped:
                _isRunningSimulation = false;
                _hasAlreadyRunASimulation = true;
                break;
            case GraphUpdateType.BeforeSwitchMode:
                HideLabelsForTransition();
                break;
            case GraphUpdateType.AfterSwitchModeToDesk:
                AfterSwitchModeToDesk();
                break;
            case GraphUpdateType.AfterSwitchModeToImmersion:
                AfterSwitchModeToImmersion();
                break;
        }
    }

    private void HideLabelsForTransition()
    {
        _inTransitionForSwitchMode = true;

        foreach (LabelNodgeUI label in _displayedLabelMainNodesDict.Values)
        {
            label.SetActive(false);
        }

        foreach (LabelNodgeUI label in _displayedLabelSubNodesDict.Values)
        {
            label.SetActive(false);
        }

        foreach (LabelNodgeUI label in _displayedLabelMainEdgesDict.Values)
        {
            label.SetActive(false);
        }

        foreach (LabelNodgeUI label in _displayedLabelSubEdgesDict.Values)
        {
            label.SetActive(false);
        }
    }

    private void AfterSwitchModeToDesk()
    {
        _graphMode = GraphMode.Desk;
        _inTransitionForSwitchMode = false;

        UpdateValueStyle();
        AfterSwitchModeToDeskForMain();
        AfterSwitchModeToDeskForLens();
    }

    private void AfterSwitchModeToDeskForMain()
    {
        if (!_displayLabelsDesk && _displayLabelsImmersion) // Release labels like they are not displayed
        {
            ReleaseMainLabels();
            return;
        }

        if (_displayLabelsImmersion) // Labels were displayed before, so they are already created
        {
            DisplayMainLabels();
            return;
        }

        CreateMainLabels(); // Labels weren't displayed before, so need to create them
    }

    private void AfterSwitchModeToDeskForLens()
    {
        if (!_displayLabelsLens) // Don't release labels because watch don't display them
            return;

        CreateSubLabels(); 
    }

    private void AfterSwitchModeToImmersion()
    {
        _graphMode = GraphMode.Immersion;
        _inTransitionForSwitchMode = false;

        UpdateValueStyle();
        AfterSwitchModeToImmersionForMain();
        AfterSwitchModeToImmersionForWatch();
    }

    private void AfterSwitchModeToImmersionForMain()
    {
        if (!_displayLabelsImmersion && _displayLabelsDesk) // Release labels like they are not displayed
        {
            ReleaseMainLabels();
            return;
        }

        if (_displayLabelsDesk) // Labels were displayed before, so they are already created
        {
            DisplayMainLabels();
            return;
        }

        CreateMainLabels(); // Labels weren't displayed before, so need to create them
    }

    private void AfterSwitchModeToImmersionForWatch()
    {
        if (!_displayLabelsLens) // labels weren't displayed before
            return;

        ReleaseSubLabels(); // Don't display labels in watch mode, so release them
    }


    private void ReleaseMainLabels()
    {
        foreach (LabelNodgeUI label in _displayedLabelMainNodesDict.Values)
        {
            _nodgePool.Release(label);
        }

        foreach (LabelNodgeUI label in _displayedLabelMainEdgesDict.Values)
        {
            _nodgePool.Release(label);
        }

        _displayedLabelMainNodesDict = new();
        _displayedLabelMainEdgesDict = new();
    }

    private void ReleaseSubLabels()
    {
        foreach (LabelNodgeUI label in _displayedLabelSubNodesDict.Values)
        {
            _nodgePool.Release(label);
        }

        foreach (LabelNodgeUI label in _displayedLabelSubEdgesDict.Values)
        {
            _nodgePool.Release(label);
        }

        _displayedLabelSubNodesDict = new();
        _displayedLabelSubEdgesDict = new();
    }

    private void DisplayMainLabels()
    {
        float scale = (_graphMode == GraphMode.Immersion) ? _graphConfig.LabelNodeSizeImmersion : _graphConfig.LabelNodeSizeDesk;
        Vector2 sizeConvas = _baseSizeCanvas * scale;
        float fontSize = _baseFontSize * scale;

        foreach (LabelNodgeUI label in _displayedLabelMainNodesDict.Values)
        {
            label.SetAll(_displayLabelsDesk, sizeConvas, fontSize);
            label.SetActive(true);
        }

        foreach (LabelNodgeUI label in _displayedLabelMainEdgesDict.Values)
        {
            label.SetAll(_displayLabelsDesk, sizeConvas, fontSize);
            label.SetActive(true);
        }
    }

    private void CreateMainLabels()
    {
        var nodesToDisplay = _selectionManager.PropagatedNodes;
        var edgesToDisplay = _selectionManager.PropagatedEdges;

        var updatedNodeDisplayedLabels = new Dictionary<Node, LabelNodgeUI>();
        var updatedEdgeDisplayedLabels = new Dictionary<Edge, LabelNodgeUI>();

        foreach (Node nodeToDisplay in nodesToDisplay)
        {
            var labelNode = CreateNodeLabel(nodeToDisplay, GraphType.Main);
            updatedNodeDisplayedLabels.Add(nodeToDisplay, labelNode);
        }

        foreach (Edge edgeToDisplay in edgesToDisplay)
        {
            var labelNode = CreateEdgeLabel(edgeToDisplay, GraphType.Main);
            updatedEdgeDisplayedLabels.Add(edgeToDisplay, labelNode);
        }

        StyleNewPropagatedLabels(updatedNodeDisplayedLabels, GraphType.Main);
        StyleNewPropagatedLabels(updatedEdgeDisplayedLabels, GraphType.Main);

        _displayedLabelMainNodesDict = updatedNodeDisplayedLabels;
        _displayedLabelMainEdgesDict = updatedEdgeDisplayedLabels;

    }

    private void CreateSubLabels()
    {
        var nodesToDisplay = _selectionManager.PropagatedNodes;
        var edgesToDisplay = _selectionManager.PropagatedEdges;

        var updatedNodeDisplayedLabels = new Dictionary<Node, LabelNodgeUI>();
        var updatedEdgeDisplayedLabels = new Dictionary<Edge, LabelNodgeUI>();

        foreach (Node nodeToDisplay in nodesToDisplay)
        {
            var labelNode = CreateNodeLabel(nodeToDisplay, GraphType.Sub);
            updatedNodeDisplayedLabels.Add(nodeToDisplay, labelNode);
        }

        foreach (Edge edgeToDisplay in edgesToDisplay)
        {
            var labelNode = CreateEdgeLabel(edgeToDisplay, GraphType.Sub);
            updatedEdgeDisplayedLabels.Add(edgeToDisplay, labelNode);
        }

        StyleNewPropagatedLabels(updatedNodeDisplayedLabels, GraphType.Sub);
        StyleNewPropagatedLabels(updatedEdgeDisplayedLabels, GraphType.Sub);

        _displayedLabelSubNodesDict = updatedNodeDisplayedLabels;
        _displayedLabelSubEdgesDict = updatedEdgeDisplayedLabels;
    }
    #endregion


    #region OnNodgesPropagated
    public void OnNodgesPropagated(Nodges propagatedNodges)
    {
        var nodesToDisplay = propagatedNodges.Nodes;
        var edgesToDisplay = propagatedNodges.Edges;

        TryDisplayMainNodesLabels(nodesToDisplay);
        TryDisplaySubNodesLabels(nodesToDisplay);

        TryDisplayMainEdgesLabels(edgesToDisplay);
        TryDisplaySubEdgesLabels(edgesToDisplay);
    }

    private void TryDisplayMainNodesLabels(List<Node> labelsToDisplay)
    {
        if (_graphMode == GraphMode.Desk && !_displayLabelsDesk)
            return;

        if (_graphMode == GraphMode.Immersion && !_displayLabelsImmersion)
            return;

        var previousDisplayedLabels = _displayedLabelMainNodesDict;



        var updatedDisplayedLabels = new Dictionary<Node, LabelNodgeUI>();
        var newDisplayedlabels = new Dictionary<Node, LabelNodgeUI>();

        FilterLabelsToDisplay();

        StyleNewPropagatedLabels(newDisplayedlabels, GraphType.Main);

        ReleaseLabels(previousDisplayedLabels);

        _displayedLabelMainNodesDict = updatedDisplayedLabels;




        void FilterLabelsToDisplay()
        {
            foreach (Node nodeToDisplay in labelsToDisplay)
            {
                if (previousDisplayedLabels.TryGetValue(nodeToDisplay, out LabelNodgeUI labelNodge))
                {
                    previousDisplayedLabels.Remove(nodeToDisplay);
                    updatedDisplayedLabels.Add(nodeToDisplay, labelNodge);
                    continue;
                }


                var labelNode = CreateNodeLabel(nodeToDisplay, GraphType.Main);
                updatedDisplayedLabels.Add(nodeToDisplay, labelNode);
                newDisplayedlabels.Add(nodeToDisplay, labelNode);
            }
        }
    }

    private void TryDisplaySubNodesLabels(List<Node> labelsToDisplay)
    {
        if (_graphMode == GraphMode.Desk && !_displayLabelsLens)
            return;

        if (_graphMode == GraphMode.Immersion) // Don't display label for watch
            return;

        var previousDisplayedLabels = _displayedLabelSubNodesDict;

        var updatedDisplayedLabels = new Dictionary<Node, LabelNodgeUI>();
        var newDisplayedlabels = new Dictionary<Node, LabelNodgeUI>();

        FilterLabelsToDisplay();

        StyleNewPropagatedLabels(newDisplayedlabels, GraphType.Sub);

        ReleaseLabels(previousDisplayedLabels);

        _displayedLabelSubNodesDict = updatedDisplayedLabels;


        void FilterLabelsToDisplay()
        {
            foreach (Node nodeToDisplay in labelsToDisplay)
            {
                if (previousDisplayedLabels.TryGetValue(nodeToDisplay, out LabelNodgeUI labelNodge))
                {
                    previousDisplayedLabels.Remove(nodeToDisplay);
                    updatedDisplayedLabels.Add(nodeToDisplay, labelNodge);
                    continue;
                }


                var labelNode = CreateNodeLabel(nodeToDisplay, GraphType.Sub);
                updatedDisplayedLabels.Add(nodeToDisplay, labelNode);
                newDisplayedlabels.Add(nodeToDisplay, labelNode);
            }
        }
    }

    private void TryDisplayMainEdgesLabels(List<Edge> labelsToDisplay)
    {
        if (_graphMode == GraphMode.Desk && !_displayLabelsDesk)
            return;

        if (_graphMode == GraphMode.Immersion && !_displayLabelsImmersion)
            return;

        var previousDisplayedLabels = _displayedLabelMainEdgesDict;

        var updatedDisplayedLabels = new Dictionary<Edge, LabelNodgeUI>();
        var newDisplayedlabels = new Dictionary<Edge, LabelNodgeUI>();

        FilterLabelsToDisplay();
        StyleNewPropagatedLabels(newDisplayedlabels, GraphType.Main);
        ReleaseLabels(previousDisplayedLabels);

        _displayedLabelMainEdgesDict = updatedDisplayedLabels;


        void FilterLabelsToDisplay()
        {
            foreach (Edge edgeToDisplay in labelsToDisplay)
            {
                if (previousDisplayedLabels.TryGetValue(edgeToDisplay, out LabelNodgeUI labelNodge))
                {
                    previousDisplayedLabels.Remove(edgeToDisplay);
                    updatedDisplayedLabels.Add(edgeToDisplay, labelNodge);
                    continue;
                }

                var labelEdge = CreateEdgeLabel(edgeToDisplay, GraphType.Main);
                updatedDisplayedLabels.Add(edgeToDisplay, labelEdge);
                newDisplayedlabels.Add(edgeToDisplay, labelEdge);
            }
        }
    }

    private void TryDisplaySubEdgesLabels(List<Edge> labelsToDisplay)
    {
        if (_graphMode == GraphMode.Desk && !_displayLabelsLens)
            return;

        if (_graphMode == GraphMode.Immersion) // Don't display label for watch
            return;


        var previousDisplayedLabels = _displayedLabelSubEdgesDict;

        var updatedDisplayedLabels = new Dictionary<Edge, LabelNodgeUI>();
        var newDisplayedlabels = new Dictionary<Edge, LabelNodgeUI>();

        FilterLabelsToDisplay();
        StyleNewPropagatedLabels(newDisplayedlabels, GraphType.Sub);
        ReleaseLabels(previousDisplayedLabels);

        _displayedLabelSubEdgesDict = updatedDisplayedLabels;


        void FilterLabelsToDisplay()
        {
            foreach (Edge edgeToDisplay in labelsToDisplay)
            {
                if (previousDisplayedLabels.TryGetValue(edgeToDisplay, out LabelNodgeUI labelNodge))
                {
                    previousDisplayedLabels.Remove(edgeToDisplay);
                    updatedDisplayedLabels.Add(edgeToDisplay, labelNodge);
                    continue;
                }

                var labelEdge = CreateEdgeLabel(edgeToDisplay, GraphType.Sub);
                updatedDisplayedLabels.Add(edgeToDisplay, labelEdge);
                newDisplayedlabels.Add(edgeToDisplay, labelEdge);
            }
        }
    }

    private void ReleaseLabels(Dictionary<Node, LabelNodgeUI> nodeLabelsToRelease)
    {
        foreach (var nodeAndLabel in nodeLabelsToRelease)
        {
            _nodgePool.Release(nodeAndLabel.Value);
        }
    }

    private void ReleaseLabels(Dictionary<Edge, LabelNodgeUI> edgeLabelsToRelease)
    {
        foreach (var edgeAndLabel in edgeLabelsToRelease)
        {
            _nodgePool.Release(edgeAndLabel.Value);
        }
    }

    private void StyleNewPropagatedLabels<T>(Dictionary<T, LabelNodgeUI> newLabels, GraphType graphType)
    {
        float sizeScale = GetLabelSize(graphType);
        Vector2 sizeConvas = _baseSizeCanvas * sizeScale;
        float fontSize = _baseFontSize * sizeScale;

        foreach (var label in newLabels.Values)
        {
            label.SetSize(sizeConvas, fontSize);
        }
    }
    #endregion


    #region LabelCreation

    private LabelNodgeUI CreateNodeLabel(Node node, GraphType graphType)
    {
        var labelNodgeUI = _nodgePool.GetLabelNodge();

        labelNodgeUI.SetFollow((graphType == GraphType.Main)? node.MainGraphNodeTf : node.SubGraphNodeTf);
        var name = node.GetName();
        labelNodgeUI.Text = (name != null) ? name : node.Value;

        return labelNodgeUI;
    }

    private LabelNodgeUI CreateEdgeLabel(Edge edge, GraphType graphType)
    {
        var labelNodgeUI = _nodgePool.GetLabelNodge();


        bool isMainGraph = (graphType == GraphType.Main);

        labelNodgeUI.SetFollow(isMainGraph? edge.Source.MainGraphNodeTf :
                                            edge.Source.SubGraphNodeTf, 
                               isMainGraph? edge.Target.MainGraphNodeTf :
                                            edge.Target.SubGraphNodeTf);
        labelNodgeUI.Text = edge.Value;

        return labelNodgeUI;
    }
    #endregion


    private void UpdateValueStyle()
    {
        _sizeMainNode = GetNodeSize(GraphType.Main);
        _sizeSubNode = GetNodeSize(GraphType.Sub);
    }

    private float GetNodeSize(GraphType graphType)
    {
        bool isChangingSize = _graphConfig.SelectedMetricTypeSize != GraphMetricType.None;


        if(_graphMode == GraphMode.Immersion) // Don't handle watch mode because don't display them in it
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
