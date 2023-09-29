using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// Manager the Lens and GPS Graph
/// </summary>
public class SubGraph : MonoBehaviour
{

    public Transform Tf { get { return _subGraphTf; } }

    [SerializeField]
    EasingType _easingType = EasingType.EaseInOutQuint;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;
    
    [SerializeField]
    LensSimulation _lensSimulation;

    [SerializeField]
    Transform _subGraphTf;

    [SerializeField]
    Transform _mainGraphTf;

    [SerializeField]
    Transform _gpsTf;

    [SerializeField]
    Transform _lensTf;

    [SerializeField]
    Transform _gpsPointTf;

    [SerializeField]
    NodgeSelectionManager _selectionManager;

    Transform _playerHeadTf;
    Transform _wristTf;
    GraphConfiguration _graphConfig;

    Dictionary<string, Node> _displayedNodes;
    Dictionary<string, Edge> _displayedEdges;

    SubGraphMode _subGraphMode;

    EasingDel _easingFunction;

    bool _displayGPS = true;

    readonly float _deltaHeightGPS = .105f;
    readonly float _deltaHeightDesk = .5f;

    bool _isFirstSimulationStopped = true;

    private void Start()
    {
        _subGraphMode = (Settings.DEFAULT_GRAPH_MODE == GraphMode.Desk)? SubGraphMode.Lens : SubGraphMode.GPS;
        _displayedNodes = new();
        _displayedEdges = new();

        _playerHeadTf = _referenceHolderSO.HMDCamSA.Value.transform;
        _wristTf = _referenceHolderSO.WristTf.Value;

        _gpsTf.parent = _wristTf;
        _gpsTf.ResetLocal();

        _easingFunction = Easing.GetEasing(_easingType);

        _graphManager.OnGraphUpdate += OnGraphUpdated;

        if(_subGraphMode == SubGraphMode.Lens)
            _subGraphTf.gameObject.SetActive(false);

        _graphConfig = GraphConfiguration.Instance;

        Invoke(nameof(DelayedSubscribe), 1f);
    }

    void DelayedSubscribe()
    {
        _displayGPS = _graphManager.GraphConfiguration.DisplayGPS;
        _selectionManager.OnNodgesPropagated += OnNodgesPropagated;
    }


    #region Update
    private void Update()
    {
        if (_subGraphMode == SubGraphMode.GPS)
            UpdateGPS();
        else
            UpdateLens();
    }


    void UpdateGPS()
    {
        UpdateSubGraphPositionToGPS();
        UpdateGpsPoint();
    }

    void UpdateLens()
    {

    }

    private void UpdateSubGraphPositionToGPS()
    {
        if (!_displayGPS)
            return;

        _subGraphTf.position = _gpsTf.position + _gpsTf.up * _deltaHeightGPS;
    }

    private void UpdateGpsPoint()
    {
         var relativePosition = _playerHeadTf.position - _mainGraphTf.position;
        relativePosition /= _graphConfig.EffectiveImmersionGraphSize;

        relativePosition *= _graphConfig.EffectiveGPSGraphSize;

        _gpsPointTf.localPosition = relativePosition;


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

        if (_subGraphMode == SubGraphMode.Lens)
        {
            SimulationStoppedLens();    
        }
        else 
        {
            SimulationStoppedGps();
        }
    }

    private void SimulationStoppedLens()
    {
        ResetVisibilityLensNodges();
        TryConstructLayoutLensGraph();
    }

    private void SimulationStoppedGps()
    {

    }

    private void BeforeSwitchMode()
    {
        _subGraphMode = SubGraphMode.InTransition;
        _subGraphTf.gameObject.SetActive(false);
        _gpsPointTf.gameObject.SetActive(false);
        _lensTf.gameObject.SetActive(false);
       

        var graph = _graphManager.Graph;
        var edges = graph.EdgesDicUID.Values;

        foreach (Edge edge in edges)
        {
            edge.DisplaySubEdge(false);
        }
    }

    private void AfterSwitchModeToDesk()
    {
        _lensTf.gameObject.SetActive(true);

        ResetVisibilityLensNodges();

        _subGraphMode = SubGraphMode.Lens;
        _subGraphTf.position = _lensTf.position;
        _subGraphTf.gameObject.SetActive(true);

        TryConstructLayoutLensGraph();
    }

    private void ResetVisibilityLensNodges()
    {
        var graph = _graphManager.Graph;
        var nodes = graph.NodesDicUID.Values;
        var edges = graph.EdgesDicUID.Values;

        var propagatedNodes = _selectionManager.PropagatedNodes;
        var propagatedEdges = _selectionManager.PropagatedEdges;

        _displayedNodes = new();
        _displayedEdges = new();

        foreach (Node node in nodes)
        {
            bool displayNode = propagatedNodes.Contains(node);
            node.DisplaySubNode(displayNode);

            if (displayNode)
                _displayedNodes.Add(node.UID, node);
        }

        foreach (Edge edge in edges)
        {
            bool displayEdge = propagatedEdges.Contains(edge);
            edge.DisplaySubEdge(displayEdge);

            if (displayEdge)
                _displayedEdges.Add(edge.UID, edge);
        }
    }

    private void AfterSwitchModeToImmersion()
    {
        
        var graph = _graphManager.Graph;
        var nodes = graph.NodesDicUID.Values;
        var edges = graph.EdgesDicUID.Values;

        foreach (Node node in nodes)
        {
            node.DisplaySubNode(true);
        }

        foreach (Edge edge in edges)
        {
            edge.DisplaySubEdge(true);
        }

        UpdateSubGraphPositionToGPS();
        _subGraphMode = SubGraphMode.GPS;

        _subGraphTf.gameObject.SetActive(_graphConfig.DisplayGPS);
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

        TryConstructLayoutLensGraph();
    }

    private void DisplayNewPropagatedNodes(List<Node> newNodesToDisplay)
    {
        var newDisplayedNodes = new Dictionary<string, Node>();
        var oldDisplayedNodes = _displayedNodes;

        foreach (Node nodeToDisplay in newNodesToDisplay)
        {
            string nodeToDisplayUID = nodeToDisplay.UID;
            if (oldDisplayedNodes.ContainsKey(nodeToDisplayUID))
            {
                oldDisplayedNodes.Remove(nodeToDisplayUID);
                newDisplayedNodes.Add(nodeToDisplayUID, nodeToDisplay);
                continue;
            }

            nodeToDisplay.DisplaySubNode(true);
            newDisplayedNodes.Add(nodeToDisplayUID, nodeToDisplay);
        }

        foreach (Node nodeToHide in oldDisplayedNodes.Values)
        {
            nodeToHide.DisplaySubNode(false);
        }

        _displayedNodes = newDisplayedNodes;
    }

    private void DisplayNewPropagatedEdges(List<Edge> newEdgesToDisplay) 
    {
        var newDisplayedEdges = new Dictionary<string, Edge>();
        var oldDisplayedEdges = _displayedEdges;

        foreach (Edge edgeToDisplay in newEdgesToDisplay)
        {
            var edgeToDisplaId = edgeToDisplay.UID;

            if (oldDisplayedEdges.ContainsKey(edgeToDisplaId))
            {
                oldDisplayedEdges.Remove(edgeToDisplaId);
                newDisplayedEdges.Add(edgeToDisplaId, edgeToDisplay);
                continue;
            }

            edgeToDisplay.DisplaySubEdge(true);
            newDisplayedEdges.Add(edgeToDisplaId, edgeToDisplay);
        }

        foreach (Edge edgeToHide in oldDisplayedEdges.Values)
        {
            edgeToHide.DisplaySubEdge(false);
        }

        _displayedEdges = newDisplayedEdges;
    }


    public void TryConstructLayoutLensGraph()
    {
        if (_displayedNodes.Count == 0)
            return;

        var displaydNodeClone = new Dictionary<string, Node>(_displayedNodes);
        var displaydEdgeClone = new Dictionary<string, Edge>(_displayedEdges);

        _lensSimulation.Run(displaydNodeClone, displaydEdgeClone);
    }
    #endregion


    [ContextMenu("SwitchGPSVisibility")]
    public void SwitchGPSVisibility()
    {
        if (_subGraphMode != SubGraphMode.GPS)
            return;

        _displayGPS = _graphManager.GraphConfiguration.DisplayGPS;
        _subGraphTf.gameObject.SetActive(_displayGPS);
    }

    public void FilterChanged()
    {
        TryConstructLayoutLensGraph();
    }

    public void UpdateGPSPoint(/*Vector3 normPosition*/)
    {
        // TODO : convert to miniGraph scale
    }

    enum SubGraphMode
    {
        Lens,
        GPS,
        InTransition
    }

}
