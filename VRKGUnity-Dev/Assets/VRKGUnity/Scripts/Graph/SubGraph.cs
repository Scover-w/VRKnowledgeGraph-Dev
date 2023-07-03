using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SubGraph : MonoBehaviour
{

    public Transform Tf { get { return _subGraphTf; } }

    [SerializeField]
    EasingType _easingType = EasingType.EaseInOutQuint;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    Transform _subGraphTf;

    [SerializeField]
    Transform _watchTf;

    [SerializeField]
    Transform _lensTf;

    [SerializeField]
    Transform _gpsPointTf;

    [SerializeField]
    NodgeSelectionManager _selectionManager;

    HashSet<Node> _displayedNodes;
    HashSet<Edge> _displayedEdges;

    SubGraphMode _subGraphMode;

    EasingDel _easingFunction;

    bool _displayWatch = true;
    bool _inTransition = false;

    float _deltaHeightWatch = .105f;
    float _deltaHeightDesk = .5f;

    float _sizeWatchGraph = .15f;

    bool _isFirstSimulationStopped = true;

    private void Start()
    {
        _subGraphMode = (Settings.DEFAULT_GRAPH_MODE == GraphMode.Desk)? SubGraphMode.Lens : SubGraphMode.Watch;
        _displayedNodes = new();
        _displayedEdges = new();

        _easingFunction = Easing.GetEasing(_easingType);

        _graphManager.OnGraphUpdate += OnGraphUpdated;

        if(_subGraphMode == SubGraphMode.Lens)
            _subGraphTf.gameObject.SetActive(false);

        Invoke(nameof(DelayedSubscribe), 1f);
    }

    void DelayedSubscribe()
    {
        _displayWatch = _graphManager.GraphConfiguration.ShowWatch;
        _selectionManager.OnNodgesPropagated += OnNodgesPropagated;
    }


    #region Update
    private void Update()
    {
        if (_subGraphMode == SubGraphMode.Watch)
            UpdateWatch();
        else
            UpdateLens();
    }


    void UpdateWatch()
    {
        UpdateSubGraphPositionToWatch();
    }

    void UpdateLens()
    {

    }

    private void UpdateSubGraphPositionToWatch()
    {
        if (!_displayWatch)
            return;

        _subGraphTf.position = _watchTf.position + _watchTf.up * _deltaHeightWatch;
    }
    #endregion


    #region OnGraphUpdated
    public void OnGraphUpdated(GraphUpdateType updateType)
    {
        switch (updateType)
        {
            case GraphUpdateType.BeforeSimulationStart:
                BeforeSimulationStart();
                break;
            case GraphUpdateType.AfterSimulationHasStopped:
                SimulationStopped();
                break;
            case GraphUpdateType.BeforeSwitchMode:
                BeforeSwitchMode();
                break;
            case GraphUpdateType.AfterSwitchModeToDesk:
                Debug.Log("AfterSwitchModeToDesk");
                AfterSwitchModeToDesk();
                break; 
            
            case GraphUpdateType.AfterSwitchModeToImmersion:
                Debug.Log("AfterSwitchModeToImmersion");
                AfterSwitchModeToImmersion();
                break;
        }            
    }

    private void BeforeSimulationStart()
    {

    }

    private void SimulationStopped()
    {
        if (_isFirstSimulationStopped)
        {
            _isFirstSimulationStopped = false;
            _subGraphTf.position = _lensTf.position;

            if (_subGraphMode == SubGraphMode.Lens)
                AfterSwitchModeToDesk();
            else
                AfterSwitchModeToImmersion();

            return;
        }


        // TODO : SimulationStopped
        if (_subGraphMode == SubGraphMode.Lens)
        {

        }
        else // Watch
        {

        }
    }

    private void BeforeSwitchMode()
    {
        _subGraphMode = SubGraphMode.InTransition;
        _subGraphTf.gameObject.SetActive(false);
        _gpsPointTf.gameObject.SetActive(false);


        var graph = _graphManager.Graph;
        var edges = graph.EdgesDicId.Values;

        foreach (Edge edge in edges)
        {
            edge.DisplaySubEdge(false);
        }
    }

    private void AfterSwitchModeToDesk()
    {
        
        var graph = _graphManager.Graph;
        var nodes = graph.NodesDicId.Values;
        var edges = graph.EdgesDicId.Values;

        var propagatedNodes = _selectionManager.PropagatedNodes;
        var propagatedEdges = _selectionManager.PropagatedEdges;

        _displayedNodes = new();
        _displayedEdges = new();

        foreach(Node node in nodes)
        {
            bool displayNode = propagatedNodes.Contains(node);
            node.DisplaySubNode(displayNode);

            if (displayNode)
                _displayedNodes.Add(node);
        }

        foreach(Edge edge in edges)
        {
            bool displayEdge = propagatedEdges.Contains(edge);
            edge.DisplaySubEdge(displayEdge);

            if(displayEdge)
                _displayedEdges.Add(edge);
        }

        _subGraphMode = SubGraphMode.Lens;
        _subGraphTf.gameObject.SetActive(true);

        RecenterLensGraph();
    }

    private void AfterSwitchModeToImmersion()
    {
        
        var graph = _graphManager.Graph;
        var nodes = graph.NodesDicId.Values;
        var edges = graph.EdgesDicId.Values;

        foreach (Node node in nodes)
        {
            node.DisplaySubNode(true);
        }

        foreach (Edge edge in edges)
        {
            edge.DisplaySubEdge(true);
        }

        UpdateSubGraphPositionToWatch();
        _subGraphMode = SubGraphMode.Watch;
        _subGraphTf.gameObject.SetActive(true);
        _gpsPointTf.gameObject.SetActive(true);
    }
    #endregion


    #region OnNodgesPropagated
    private void OnNodgesPropagated(Nodges propagatedNodges)
    {
        if (_subGraphMode != SubGraphMode.Lens)
            return;

        DisplayNewPropagatedNodes(propagatedNodges.Nodes);
        DisplayNewPropagatedEdges(propagatedNodges.Edges);

        RecenterLensGraph();
    }

    private void DisplayNewPropagatedNodes(List<Node> newNodesToDisplay)
    {
        var newDisplayedNodes = new HashSet<Node>();

        foreach (Node nodeToDisplay in newNodesToDisplay)
        {
            if (_displayedNodes.Contains(nodeToDisplay))
            {
                _displayedNodes.Remove(nodeToDisplay);
                newDisplayedNodes.Add(nodeToDisplay);
                continue;
            }

            nodeToDisplay.DisplaySubNode(true);
            newDisplayedNodes.Add(nodeToDisplay);
        }

        foreach (Node nodeToHide in _displayedNodes)
        {
            nodeToHide.DisplaySubNode(false);
        }

        _displayedNodes = newDisplayedNodes;
    }

    private void DisplayNewPropagatedEdges(List<Edge> newEdgesToDisplay) 
    {
        var newDisplayedEdges = new HashSet<Edge>();

        foreach (Edge edgeToDisplay in newEdgesToDisplay)
        {
            if (_displayedEdges.Contains(edgeToDisplay))
            {
                _displayedEdges.Remove(edgeToDisplay);
                newDisplayedEdges.Add(edgeToDisplay);
                continue;
            }

            edgeToDisplay.DisplaySubEdge(true);
            newDisplayedEdges.Add(edgeToDisplay);
        }

        foreach (Edge edgeToHide in _displayedEdges)
        {
            edgeToHide.DisplaySubEdge(false);
        }

        _displayedEdges = newDisplayedEdges;
    }

    public void RecenterLensGraph()
    {

        if (_subGraphMode == SubGraphMode.Watch)
            return;

        Debug.Log("Displayed Nodes,subgraph : " + _displayedNodes.Count);

        if (_displayedNodes.Count == 0)
            return;

        Vector3 centerGraph = Vector3.zero;

        var scaleGraph = _graphManager.GraphConfiguration.LensGraphSize;

        foreach(Node node in _displayedNodes)
        {
            centerGraph += node.AbsolutePosition;
        }


        centerGraph /= _displayedNodes.Count;
        centerGraph *= scaleGraph;
        _subGraphTf.position = _lensTf.position - centerGraph;
    }
    #endregion



    [ContextMenu("SwitchWatchVisibility")]
    public void SwitchWatchVisibility()
    {
        if (_subGraphMode != SubGraphMode.Watch)
            return;

        _displayWatch = _graphManager.GraphConfiguration.ShowWatch;
        _subGraphTf.gameObject.SetActive(_displayWatch);
    }

    public void UpdateGPSPoint(Vector3 normPosition)
    {
        // TODO : convert to miniGraph scale
    }

    enum SubGraphMode
    {
        Lens,
        Watch,
        InTransition
    }

}
