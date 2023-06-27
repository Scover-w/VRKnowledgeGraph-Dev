using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEditor;
using UnityEngine;

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


    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    GraphUI _graphUI;

    static NodgeSelectionManager _instance;

    SelectionMode _selectionMode = SelectionMode.Single;

    Node _singleSelectedNode;

    List<Node> _multipleSelectedNodes;

    List<Node> _propagatedNodes;

    List<LabelNodgeUI> _labelNodgesUI;

    IReadOnlyDictionary<Transform, Node> _nodesDicTf;

    GraphConfiguration _graphConfiguration;


    private void Start()
    {
        if(_instance != null)
        {
            Debug.LogError("NodgeSelectionManager: Start->Multiple instance in the scene(s)");
            Destroy(this); 
            return;
        }

        _instance = this;

        _multipleSelectedNodes = new();
        _propagatedNodes = new();
        _labelNodgesUI = new();

        _graphConfiguration = _graphManager.GraphConfiguration;
    }

    void Update()
    {
        UpdateLabelNodges();
    }

    public void SetNodgeTfs(IReadOnlyDictionary<Transform, Node> nodesDicTf, IReadOnlyDictionary<Transform, Edge> edgesDicTf)
    {
        _nodesDicTf = nodesDicTf;
    }

    private void UpdateLabelNodges()
    {
        if (_labelNodgesUI == null)
            return;

        int nb = _labelNodgesUI.Count;

        for (int i = 0; i < nb; i++)
        {
           // _labelNodgesUI[i].UpdateTransform();
        }
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

        _singleSelectedNode = _multipleSelectedNodes[_multipleSelectedNodes.Count - 1];
        ClearSelection(SelectionMode.Multiple);
        ClearPropagation();

        Propagate(_singleSelectedNode);
    }

    private void SwitchToMultiple()
    {
        if (_singleSelectedNode == null)
            return;

        _multipleSelectedNodes.Add(_singleSelectedNode);
        _singleSelectedNode = null;
    }

    public void TryClearSelection()
    {
        TryClearSelectedNode();
    }


    public void Select(Node node)
    {
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
        Propagate(_singleSelectedNode);
    }

    private void MultipleSelect(Node node)
    {
        _multipleSelectedNodes.Add(node);
        Propagate(node);
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
            Propagate(nodeToPropagate);
        }
    }



    public void SelectNodeTemp(Transform nodeTf)
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
    }


    private void ClearPropagation()
    {
        foreach (Node node in _propagatedNodes)
        {
            node.SetPropagation(false);
        }

        _propagatedNodes = new();
        ClearLabelNodges();
    }

    private void ClearSelection(SelectionMode selectionMode)
    {
        if (selectionMode == SelectionMode.Single)
        {
            _singleSelectedNode.UnSelect();
            _singleSelectedNode = null;
            return;
        }

        int i = 0;
        int toSkip = _multipleSelectedNodes.Count;

        foreach (Node node in _multipleSelectedNodes)
        {
            i++;

            if (i == toSkip)
                continue;

            node.UnSelect();
        }
        _multipleSelectedNodes = new();
    }

    private void Propagate(Node node)
    {
        PropagateLabelNodge(node, _graphConfiguration.LabelNodgePropagation, new HashSet<Node>(), new HashSet<Edge>());
    }

    private void PropagateLabelNodge(Node node, int propagationValue, HashSet<Node> nodesLabeled, HashSet<Edge> edgesLabeled)
    {
        nodesLabeled.Add(node);

        var labelNodge = NodgePool.Instance.GetLabelNodge();
        labelNodge.SetFollow(node.MainGraphNodeTf);
        var name = node.GetName();
        labelNodge.Text = (name != null) ? name : node.Value;
        _labelNodgesUI.Add(labelNodge);

        propagationValue--;

        // if comes from source, next is targetNode, inverse
        for (int i = 0; i < 2; i++)
        {
            var edges = (i == 0) ? node.EdgeSource : node.EdgeTarget;
            int nbEdge = edges.Count;


            for (int j = 0; j < nbEdge; j++)
            {
                var edge = edges[j];

                if (edgesLabeled.Contains(edge))
                    continue;

                edgesLabeled.Add(edge);

                labelNodge = NodgePool.Instance.GetLabelNodge();
                labelNodge.SetFollow(edge.Source.MainGraphNodeTf, edge.Target.MainGraphNodeTf);
                labelNodge.Text = edge.Value;
                _labelNodgesUI.Add(labelNodge);

                if (propagationValue == 0)
                    continue;

                var nextNode = (i == 0) ? edge.Target : edge.Source;

                if (nodesLabeled.Contains(nextNode))
                    continue;

                PropagateLabelNodge(nextNode, propagationValue, nodesLabeled, edgesLabeled);
            }
        }
    }

    public void ClearLabelNodges()
    {
        int nb = _labelNodgesUI.Count;

        for (int i = 0; i < nb; i++)
        {
            NodgePool.Instance.Release(_labelNodgesUI[i]);
        }

        _labelNodgesUI = new();
    }


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
