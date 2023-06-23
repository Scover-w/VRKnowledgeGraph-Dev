using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEditor;
using UnityEngine;

public class NodgeSelectionManager : MonoBehaviour
{
    public bool IsInMultiSelection { get { return _inMultiSelection; } }
    public bool HasASelectedNode { get { return _selectedNode != null; } }
    public bool HasASelectedEdge { get { return _selectedEdge != null; } }

    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    GraphUI _graphUI;

    bool _inMultiSelection = false;

    Node _selectedNode;
    Edge _selectedEdge;

    List<Node> _selectedNodes;
    List<Edge> _selectedEdges;

    List<Node> _propagatedNodes;
    List<Edge> _propagatedEdges;

    List<LabelNodgeUI> _labelNodgesUI;

    IReadOnlyDictionary<Transform, Node> _nodesDicTf;
    IReadOnlyDictionary<Transform, Edge> _edgesDicTf;

    GraphConfiguration _graphConfiguration;


    private void Start()
    {
        _selectedNodes = new();
        _selectedEdges = new();

        _propagatedNodes = new();
        _propagatedEdges = new();

        _graphConfiguration = _graphManager.GraphConfiguration;
    }

    void Update()
    {
        UpdateLabelNodges();
    }

    public void SetNodgeTfs(IReadOnlyDictionary<Transform, Node> nodesDicTf, IReadOnlyDictionary<Transform, Edge> edgesDicTf)
    {
        _nodesDicTf = nodesDicTf;
        _edgesDicTf = edgesDicTf;
    }

    private void UpdateLabelNodges()
    {
        if (_labelNodgesUI == null)
            return;

        int nb = _labelNodgesUI.Count;

        for (int i = 0; i < nb; i++)
        {
            _labelNodgesUI[i].UpdateTransform();
        }
    }

    public void SetMultiSelection(bool inMultiSelection)
    {

    }

    public void TryClearSelection()
    {
        TryClearSelectedNode();
        TryClearSelectedEdge();
    }


    public void SelectEdge(Transform edgeTf)
    {
        if (_selectedEdge != null && edgeTf == _selectedEdge.MainGraphEdgeTf)
            return;

        if (!_edgesDicTf.TryGetValue(edgeTf, out Edge edge))
        {
            Debug.LogError("Transform not linked to a edge");
            TryClearSelectedEdge();
            return;
        }

        TryClearSelectedNode();

        ReleaseLabelNodges();

        _selectedEdge = edge;
        _graphUI.DisplayInfoEdge(edge);

        PropagateLabelNodge(edge, _graphConfiguration.LabelNodgePropagation, new HashSet<Node>(), new HashSet<Edge>());
        Selection.activeObject = edgeTf;
    }


    public void SelectNodeTemp(Transform nodeTf)
    {
        if (_selectedNode != null && nodeTf == _selectedNode.MainGraphNodeTf)
            return;

        if (!_nodesDicTf.TryGetValue(nodeTf, out Node node))
        {
            Debug.LogError("Transform not linked to a node");
            TryClearSelectedNode();
            return;
        }

        TryClearSelectedEdge();

        ReleaseLabelNodges();

        _selectedNode = node;
        _graphUI.DisplayInfoNode(_selectedNode);

        PropagateLabelNodge(_selectedNode, _graphConfiguration.LabelNodgePropagation, new HashSet<Node>(), new HashSet<Edge>());
        Selection.activeObject = nodeTf;
    }

    public void SelectNode(Transform nodeTf)
    {
        if (nodeTf == null)
        {
            TryClearSelectedNode();
            return;
        }

        if (_selectedNode != null && nodeTf == _selectedNode.MainGraphNodeTf)
            return;

        if (!_nodesDicTf.TryGetValue(nodeTf, out Node node))
        {
            Debug.LogError("Transform not linked to a node");
            TryClearSelectedNode();
            return;
        }


        ReleaseLabelNodges();

        _selectedNode = node;
        _graphUI.DisplayInfoNode(_selectedNode);
        PropagateLabelNodge(_selectedNode, _graphConfiguration.LabelNodgePropagation, new HashSet<Node>(), new HashSet<Edge>());

        Selection.activeObject = nodeTf;
    }

    public void TryClearSelectedNode()
    {
        if (!HasASelectedNode)
            return;

        ReleaseLabelNodges();
        _graphUI.DisplayInfoNode(null);

        _selectedNode = null;
    }

    public void TryClearSelectedEdge()
    {
        if (!HasASelectedEdge)
            return;

        ReleaseLabelNodges();
        _graphUI.DisplayInfoEdge(null);

        _selectedEdge = null;
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


    private void PropagateLabelNodge(Edge edge, int propagationValue, HashSet<Node> nodesLabeled, HashSet<Edge> edgesLabeled)
    {
        edgesLabeled.Add(edge);

        var labelNodge = NodgePool.Instance.GetLabelNodge();
        labelNodge.SetFollow(edge.Source.MainGraphNodeTf, edge.Target.MainGraphNodeTf);

        labelNodge.Text = edge.Value;
        _labelNodgesUI.Add(labelNodge);

        propagationValue--;

        // if comes from source, next is targetNode, inverse
        for (int i = 0; i < 2; i++)
        {
            var node = (i == 0) ? edge.Source : edge.Target;


            if (nodesLabeled.Contains(node))
                continue;

            nodesLabeled.Add(node);

            labelNodge = NodgePool.Instance.GetLabelNodge();
            labelNodge.SetFollow(node.MainGraphNodeTf);
            var name = node.GetName();
            labelNodge.Text = (name != null) ? name : node.Value;
            _labelNodgesUI.Add(labelNodge);

            if (propagationValue == 0)
                continue;




            var nextedges = (i == 0) ? node.EdgeTarget : node.EdgeSource;

            int nbEdge = nextedges.Count;


            for (int j = 0; j < nbEdge; j++)
            {
                var nextEdge = nextedges[j];

                if (edgesLabeled.Contains(nextEdge))
                    continue;

                PropagateLabelNodge(nextEdge, propagationValue, nodesLabeled, edgesLabeled);
            }
        }
    }


    public void ReleaseLabelNodges()
    {
        int nb = _labelNodgesUI.Count;

        for (int i = 0; i < nb; i++)
        {
            NodgePool.Instance.Release(_labelNodgesUI[i]);
        }
    }
}
