using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NodgeSelectionManager : MonoBehaviour
{
    public static NodgeSelectionManager Instance { get { return _instance; } }
    public SelectionMode SelectionMode { get { return _selectionMode; } }
    public bool HasASelectedNode 
    { 
        get 
        {
            if (_selectionMode == SelectionMode.Single)
                return _singleSelectedNode != null;
            else
                return _multipleSelectedNodes.Count > 0;
        } 
    }

    public ReadOnlyHashSet<Node> SelectedNodes 
    { 
        get 
        { 
            if(_selectionMode == SelectionMode.Single)
            {
                if (_singleSelectedNode == null)
                    return null;

                var hashset = new HashSet<Node>
                {
                    _singleSelectedNode
                };

                return new ReadOnlyHashSet<Node>(hashset);
            }


            if (_multipleSelectedNodes.Count == 0)
                return null;

            return new ReadOnlyHashSet<Node>(_multipleSelectedNodes); 
        } 
    }

    public ReadOnlyHashSet<Node> PropagatedNodes { get { return new ReadOnlyHashSet<Node>(_propagatedNodes); } }
    public ReadOnlyHashSet<Edge> PropagatedEdges { get { return new ReadOnlyHashSet<Edge>(_propagatedEdges); } }

    public delegate void NodeSelectedDel(Node selectedNode);
    public delegate void NodgesPropagatedDel(Nodges propagatedNodges);

    public NodeSelectedDel OnNodeSelected;
    public NodgesPropagatedDel OnNodgesPropagated;


    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    GraphUI _graphUI;

    [SerializeField]
    LabelNodgeManagerUI _labelNodgeManager;

    static NodgeSelectionManager _instance;

    SelectionMode _selectionMode = SelectionMode.Single;

    Node _singleSelectedNode;
    Node _lastMultipleAdded;

    Node _currentHoveredNode;

    HashSet<Node> _multipleSelectedNodes;

    HashSet<Node> _propagatedNodes;
    HashSet<Edge> _propagatedEdges;

    HashSet<Node> _newPropagatedNodes;
    HashSet<Edge> _newPropagatedEdges;

    GraphConfiguration _graphConfiguration;


    private void Start()
    {
        if(_instance != null)
        {
            Debug.LogError("NodgeSelectionManager: Start -> Multiple instance in the scene(s)");
            Destroy(this); 
            return;
        }

        _instance = this;

        _multipleSelectedNodes = new();

        _propagatedEdges = new();
        _propagatedNodes = new();

        _graphConfiguration = _graphManager.GraphConfiguration;
    }

    [ContextMenu("Switch Mode")]
    public void SwitchSelectionMode()
    {
        if(_selectionMode == SelectionMode.Single)
        {
            _selectionMode = SelectionMode.Multiple;
            SwitchToMultiple();
        }
        else
        {
            _selectionMode = SelectionMode.Single;
            SwitchToSingle();
        }
    }

    private void SwitchToSingle()
    {
        if (_multipleSelectedNodes.Count == 0)
            return;

        _singleSelectedNode = _lastMultipleAdded;
        ClearSelectionFromSwitchMode(SelectionMode.Multiple);

        StartPropagate(_singleSelectedNode);

        TriggerOnPropagated();
    }

    private void SwitchToMultiple()
    {
        if (_singleSelectedNode == null)
            return;

        _multipleSelectedNodes.Add(_singleSelectedNode);
        _lastMultipleAdded = _singleSelectedNode;
        _singleSelectedNode = null;
    }


    public void TryClearSelectionFromEmptyUserClick()
    {
        
        if (_selectionMode == SelectionMode.Single)
        {
            if (_singleSelectedNode == null)
                return;

            _singleSelectedNode.UnSelect();
            _singleSelectedNode = null;
            ClearPropagation();
            TriggerOnPropagated();
            return;
        }

        if (_multipleSelectedNodes.Count == 0)
            return;

        foreach (Node node in _multipleSelectedNodes)
        {
            node.UnSelect();
        }
        _multipleSelectedNodes = new();
        ClearPropagation();
        TriggerOnPropagated();
    }


    public void Hover(Node node)
    {
        if (_currentHoveredNode == node)
            return;

        _currentHoveredNode = node;
        _labelNodgeManager.SetHover(node);

    }

    public void UnHover(Node node)
    {
        if (_currentHoveredNode != node)
            return;

        _currentHoveredNode = null;
        _labelNodgeManager.CancelHover();
    }

    public void Select(Node node)
    {
        OnNodeSelected?.Invoke(node);
        Debug.Log("Select : " + node.Uri);
        if(_selectionMode == SelectionMode.Single)
        {
            SingleSelect(node);
            return;
        }

        MultipleSelect(node);
    }

    private void SingleSelect(Node node)
    {
        if( _singleSelectedNode != null)
        {
            _singleSelectedNode.UnSelect();
            ClearPropagation();
        }

        _singleSelectedNode = node;
        StartPropagate(_singleSelectedNode);

        TriggerOnPropagated();
    }

    private void MultipleSelect(Node node)
    {
        _multipleSelectedNodes.Add(node);
        _lastMultipleAdded = node;
        StartPropagate(node);

        TriggerOnPropagated();
    }



    public void UnSelect(Node node)
    {
        if (_selectionMode == SelectionMode.Single)
        {
            SingleUnSelect(node);
            return;
        }

        MultipleUnSelect(node);
    }

    private void SingleUnSelect(Node node)
    {
        if(_singleSelectedNode== null)
        {
            Debug.LogWarning("NodgeSelectionManager : SingleUnSelect -> _singleSelectedNode is null");
            return;
        }

        if(node != _singleSelectedNode)
        {
            Debug.LogWarning("NodgeSelectionManager : SingleUnSelect -> node and _singleSelectedNode are different");
            return;
        }

        ClearPropagation();
        _singleSelectedNode = null;

        TriggerOnPropagated();
    }

    private void MultipleUnSelect(Node node)
    {
        bool isIn = _multipleSelectedNodes.Contains(node);
        if(!isIn)
        {
            Debug.LogWarning("NodgeSelectionManager : MultipleUnSelect -> node is not in _multipleSelectedNodes");
            return;
        }

        _multipleSelectedNodes.Remove(node);
        ClearPropagation();

        foreach (Node nodeToPropagate in _multipleSelectedNodes)
        {
            StartPropagate(nodeToPropagate);
        }

        TriggerOnPropagated();
    }

    
    private void ClearSelectionFromSwitchMode(SelectionMode selectionMode)
    {
        if (selectionMode == SelectionMode.Single)
        {

            if (_singleSelectedNode == null)
                return;

            _singleSelectedNode.UnSelect();
            _singleSelectedNode = null;
            ClearPropagation();
            return;
        }


        if (_multipleSelectedNodes.Count == 0)
            return;

        int i = 0;
        int toSkip = _multipleSelectedNodes.Count;


        foreach (Node node in _multipleSelectedNodes)
        {
            i++;

            if (i == toSkip)
                continue;

            node.UnSelect();
        }

        ClearPropagation();
        _multipleSelectedNodes = new();
    }


    private void StartPropagate(Node node)
    {
        _newPropagatedNodes = new();
        _newPropagatedEdges = new();

        Propagate(node, _graphConfiguration.LabelNodgePropagation);
    }

    private void Propagate(Node node, int propagationValue)
    {
        if (_newPropagatedNodes.Contains(node))
            return;

        _newPropagatedNodes.Add(node);

        if (!_propagatedNodes.Contains(node))
        {
            _propagatedNodes.Add(node);
            node.SetPropagation(_graphManager.GraphMode, true);
        }


        if (propagationValue == 0)
            return;

        propagationValue--;

        // if comes from source, next is targetNode, inverse
        for (int i = 0; i < 2; i++)
        {
            var edges = (i == 0) ? node.EdgeSource : node.EdgeTarget;
            int nbEdge = edges.Count;


            for (int j = 0; j < nbEdge; j++)
            {
                Edge edge = edges[j];

                if (_newPropagatedEdges.Contains(edge))
                    continue;

                _newPropagatedEdges.Add(edge);

                if (!_propagatedEdges.Contains(edge))
                {
                    _propagatedEdges.Add(edge);
                    edge.SetPropagation(_graphManager.GraphMode, true);
                }

                var nextNode = (i == 0) ? edge.Target : edge.Source;

                if(nextNode.IsHidden)
                    Propagate(nextNode, propagationValue);
            }
        }
    }

    private void ClearPropagation()
    {
        foreach (Node node in _propagatedNodes)
        {
            node.SetPropagation(_graphManager.GraphMode, false);
        }

        foreach (Edge edge in _propagatedEdges)
        {
            edge.SetPropagation(_graphManager.GraphMode, false);
        }

        _propagatedNodes = new();
        _propagatedEdges = new();
    }

    private void TriggerOnPropagated()
    {
        Nodges nodges = new Nodges(_propagatedNodes.ToList(), _propagatedEdges.ToList());
        OnNodgesPropagated?.Invoke(nodges);
    }


    public void NodgesHidden(HashSetNodges nodges)
    {
        HashSet<Node> hiddenNodes = nodges.Nodes;


        // Clear from selection
        if(_selectionMode == SelectionMode.Single)
        {
            if(hiddenNodes.Contains(_singleSelectedNode))
                _singleSelectedNode = null;
        }
        else
        {

            HashSet<Node> selectedNodeToRemove = new();

            foreach(Node node in _multipleSelectedNodes)
            {
                if (!hiddenNodes.Contains(node))
                    continue;

                selectedNodeToRemove.Add(node);
            }


            foreach(Node node in selectedNodeToRemove)
            {
                _multipleSelectedNodes.Remove(node);
            }
        }

        ClearPropagation();


        if(_selectionMode == SelectionMode.Single && _singleSelectedNode != null)
        {
            StartPropagate(_singleSelectedNode);
        }
        else if(_selectionMode == SelectionMode.Multiple && _multipleSelectedNodes.Count > 0)
        {
            foreach (Node nodeToPropagate in _multipleSelectedNodes)
            {
                StartPropagate(nodeToPropagate);
            }
        }

        TriggerOnPropagated();
    }


    #region OLD
    /*public void SelectNodeTemp(Transform nodeTf)
    {
        if (_singleSelectedNode != null && nodeTf == _singleSelectedNode.MainGraphNodeTf)
            return;

        if (!_nodesDicTf.TryGetValue(nodeTf, out Node node))
        {
            Debug.LogError("Transform not linked to a node");
            TryClearSelectedNode();
            return;
        }

        //TryClearSelectedEdge();

        ClearLabelNodges();

        _singleSelectedNode = node;
        _graphUI.DisplayInfoNode(_singleSelectedNode);

        PropagateLabelNodge(_singleSelectedNode, _graphConfiguration.LabelNodgePropagation, new HashSet<Node>(), new HashSet<Edge>());
        Selection.activeObject = nodeTf;
    }

    public void SelectNode(Transform nodeTf)
    {
        if (nodeTf == null)
        {
            TryClearSelectedNode();
            return;
        }

        if (_singleSelectedNode != null && nodeTf == _singleSelectedNode.MainGraphNodeTf)
            return;

        if (!_nodesDicTf.TryGetValue(nodeTf, out Node node))
        {
            Debug.LogError("Transform not linked to a node");
            TryClearSelectedNode();
            return;
        }


        ClearLabelNodges();

        _singleSelectedNode = node;
        _graphUI.DisplayInfoNode(_singleSelectedNode);
        Propagate(_singleSelectedNode);

        Selection.activeObject = nodeTf;
    }


    public void TryClearSelectedNode()
    {
        if (!HasASelectedNode)
            return;

        ClearLabelNodges();
        _graphUI.DisplayInfoNode(null);

        _singleSelectedNode = null;
    }*/
    #endregion



    #region Edge
    //public void TryClearSelectedEdge()
    //{
    //    if (!HasASelectedEdge)
    //        return;

    //    ReleaseLabelNodges();
    //    _graphUI.DisplayInfoEdge(null);

    //    _selectedEdge = null;
    //}

    //public void SelectEdge(Transform edgeTf)
    //{
    //    if (_selectedEdge != null && edgeTf == _selectedEdge.MainGraphEdgeTf)
    //        return;

    //    if (!_edgesDicTf.TryGetValue(edgeTf, out Edge edge))
    //    {
    //        Debug.LogError("Transform not linked to a edge");
    //        TryClearSelectedEdge();
    //        return;
    //    }

    //    TryClearSelectedNode();

    //    ReleaseLabelNodges();

    //    _selectedEdge = edge;
    //    _graphUI.DisplayInfoEdge(edge);

    //    PropagateLabelNodge(edge, _graphConfiguration.LabelNodgePropagation, new HashSet<Node>(), new HashSet<Edge>());
    //    Selection.activeObject = edgeTf;
    //}



    //private void PropagateLabelNodge(Edge edge, int propagationValue, HashSet<Node> nodesLabeled, HashSet<Edge> edgesLabeled)
    //{
    //    edgesLabeled.Add(edge);

    //    var labelNodge = NodgePool.Instance.GetLabelNodge();
    //    labelNodge.SetFollow(edge.Source.MainGraphNodeTf, edge.Target.MainGraphNodeTf);

    //    labelNodge.Text = edge.Value;
    //    _labelNodgesUI.Add(labelNodge);

    //    propagationValue--;

    //    // if comes from source, next is targetNode, inverse
    //    for (int i = 0; i < 2; i++)
    //    {
    //        var node = (i == 0) ? edge.Source : edge.Target;


    //        if (nodesLabeled.Contains(node))
    //            continue;

    //        nodesLabeled.Add(node);

    //        labelNodge = NodgePool.Instance.GetLabelNodge();
    //        labelNodge.SetFollow(node.MainGraphNodeTf);
    //        var name = node.GetName();
    //        labelNodge.Text = (name != null) ? name : node.Value;
    //        _labelNodgesUI.Add(labelNodge);

    //        if (propagationValue == 0)
    //            continue;




    //        var nextedges = (i == 0) ? node.EdgeTarget : node.EdgeSource;

    //        int nbEdge = nextedges.Count;


    //        for (int j = 0; j < nbEdge; j++)
    //        {
    //            var nextEdge = nextedges[j];

    //            if (edgesLabeled.Contains(nextEdge))
    //                continue;

    //            PropagateLabelNodge(nextEdge, propagationValue, nodesLabeled, edgesLabeled);
    //        }
    //    }
    //}
    #endregion



}

public enum SelectionMode
{
    Single,
    Multiple
}
