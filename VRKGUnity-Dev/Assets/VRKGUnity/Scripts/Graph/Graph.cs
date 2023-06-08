using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.Observers;
using QuikGraph.Algorithms.ShortestPath;
using QuikGraph.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TMPro;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Graph
{
    public float Velocity { get { return _velocity; } }
    public bool ReachStopVelocity {  get { return _reachStopVelocity; } }

    public Node SelectedNode { get { return _selectedNode; } }

    public bool HasASelectedNode { get { return _selectedNode != null; } }
    public bool HasASelectedEdge { get { return _selectedEdge != null; } }

    public IReadOnlyDictionary<int, Node> NodesDicId => _nodesDicId;
    public IReadOnlyDictionary<int, Edge> EdgesDicId => _edgesDicId;

    public GraphConfiguration Configuration 
    {
        get 
        {
            return _graphConfiguration;
        } 
        set 
        { 
            _graphConfiguration = value; 
        } 
    }


    GraphManager _graphManager;
    GraphStyling _graphStyling;
    GraphUI _graphUI;

    Dictionary<int, Node> _nodesDicId;
    Dictionary<int, Edge> _edgesDicId;

    List<LabelNodgeUI> _labelNodgesUI;

    GraphConfiguration _graphConfiguration;

    Dictionary<Transform, Node> _nodesDicTf;
    Dictionary<Transform, Edge> _edgesDicTf;

    BidirectionalGraph<Node, Edge> _graphDatas;

    GraphDbRepository _repository;

    Node _selectedNode;
    Edge _selectedEdge;

    Transform _miniGraphTf;
    Transform _megaGraphTf;

    float _velocity;
    bool _reachStopVelocity;

    int _metricsCalculated;

    #region CREATION_UPDATE_NODGES
    public Graph(GraphManager graphManager, GraphUI graphUI, GraphStyling graphStyling, Nodges nodges, GraphDbRepository repo)
    {
        _nodesDicId = nodges.NodesDicId; 
        _edgesDicId = nodges.EdgesDicId;

        _nodesDicTf = new();
        _edgesDicTf = new();

        _graphManager = graphManager;
        _graphUI = graphUI;
        _labelNodgesUI = new List<LabelNodgeUI>();


        _graphConfiguration = _graphManager.GraphConfiguration;
        _graphStyling = graphStyling;
        _graphDatas = new();

        _miniGraphTf = _graphManager.MiniGraph.Tf;
        _megaGraphTf = _graphManager.MegaGraph.Tf;


        SetupNodes();
        SetupEdges();



        int seed = Configuration.SeedRandomPosition;
        foreach (var idAndNode in _nodesDicId)
        {
            idAndNode.Value.ResetPosition(seed);
        }
    }

    private void SetupNodes()
    {
        foreach(var idAndNode in _nodesDicId)
        {
            SetupNode(idAndNode.Value, true);
            SetupNode(idAndNode.Value, false);
            _graphDatas.AddVertex(idAndNode.Value);
        }
    }

    private void SetupNode(Node node, bool isMegaGraph)
    {
        var nodeStyler = NodgePool.Instance.GetNodeStyler();
        nodeStyler.Node = node;

        var nodeTf = nodeStyler.Tf;
        nodeTf.name = "Node " + node.GetName();
        nodeTf.position = node.AbsolutePosition;

        if(isMegaGraph)
        {
            nodeStyler.GraphType = GraphType.Mega;
            node.MegaStyler = nodeStyler;
            node.MegaTf = nodeTf;
            nodeTf.parent = _megaGraphTf;
            
        }
        else
        {
            nodeStyler.GraphType = GraphType.Mini;
            node.MiniStyler = nodeStyler;
            node.MiniTf = nodeTf;
            nodeTf.parent = _miniGraphTf;
        }

        nodeTf.localPosition = node.AbsolutePosition;

        _nodesDicTf.Add(nodeTf, node);
    }

    private void SetupEdges()
    {
        foreach(var idAndEdge in _edgesDicId)
        {
            SetupEdge(idAndEdge.Value, true);
            SetupEdge(idAndEdge.Value, false);
            _graphDatas.AddEdge(idAndEdge.Value);
        }
    }

    private void SetupEdge(Edge edge,bool isMegaGraph)
    {
        var edgeStyler = NodgePool.Instance.GetEdgeStyler();
        edgeStyler.Edge = edge;

        edgeStyler.gameObject.name = "Edge " + edge.Value.ToString();
        
        if(isMegaGraph)
        {
            edgeStyler.GraphType = GraphType.Mega;
            edge.MegaLine = edgeStyler.LineRenderer;
            edge.MegaStyler = edgeStyler;
            edge.MegaTf = edgeStyler.ColliderTf;
            edgeStyler.Tf.parent = _megaGraphTf;
        }
        else
        {
            edgeStyler.GraphType = GraphType.Mini;
            edge.MiniLine = edgeStyler.LineRenderer;
            edge.MiniStyler = edgeStyler;
            edge.MiniTf = edgeStyler.ColliderTf;
            edgeStyler.Tf.parent = _miniGraphTf;
        }

        edgeStyler.Tf.localPosition = Vector3.zero;


        _edgesDicTf.Add(edgeStyler.ColliderTf, edge);
    }

    public async Task UpdateNodges(Nodges nodges)
    {
        await UpdateNodes(nodges.NodesDicId);
        UpdateEdges(nodges.EdgesDicId);
    }

    private async Task UpdateNodes(Dictionary<int,Node> nodesDicId)
    {
        Dictionary<int, Node> newNodes = new();
        Dictionary<int, Node> newNodesToRetrieveNames = new();

        bool doResetPosition = Configuration.ResetPositionNodeOnUpdate;
        int seed = Configuration.SeedRandomPosition;

        foreach (var idAndNode in nodesDicId)
        {
            var id = idAndNode.Key;

            if (_nodesDicId.TryGetValue(id, out Node node))
            {
                // Already in the graph
                newNodes.Add(id, node);
                _nodesDicId.Remove(id);

                if (doResetPosition)
                    node.ResetPosition(seed);
            }
            else
            {
                // New from the graph
                var newNode = idAndNode.Value;
                newNodes.Add(id, newNode);
                newNodesToRetrieveNames.Add(id, newNode);

                SetupNode(newNode,true);
                SetupNode(newNode,false);

                newNode.ResetPosition(seed);
            }
        }


        var nodgePool = NodgePool.Instance;

        foreach (var idAndNode in _nodesDicId)
        {
            nodgePool.Release(idAndNode.Value.MegaStyler);
        }

        newNodesToRetrieveNames.AddRetrievedNames(_repository.GraphDbRepositoryDistantUris);
        newNodesToRetrieveNames.ExtractNodeNamesToProperties();


        _nodesDicId = newNodes;
    }

    private void UpdateEdges(Dictionary<int, Edge> edgesDicId)
    {
        Dictionary<int, Edge> newEdges = new();

        foreach (var idAndEdge in edgesDicId)
        {
            var id = idAndEdge.Key;

            if (_edgesDicId.TryGetValue(id, out Edge edge))
            {
                newEdges.Add(id, edge);
                _edgesDicId.Remove(id);
            }
            else
            {
                newEdges.Add(id, idAndEdge.Value);
                SetupEdge(idAndEdge.Value, true);
                SetupEdge(idAndEdge.Value, false);
            }
        }


        var nodgePool = NodgePool.Instance;

        foreach (var idAndEdge in _edgesDicId)
        {
            var edge = idAndEdge.Value;
            nodgePool.Release(edge.MegaStyler);
            edge.CleanFromNodes(); // Clean unused edge from staying Nodes

        }

        _edgesDicId = newEdges;
    }

    public void ReleaseLabelNodges()
    {
        int nb = _labelNodgesUI.Count;

        for (int i = 0; i < nb; i++)
        {
            NodgePool.Instance.Release(_labelNodgesUI[i]);
        }
    }
    #endregion


    #region SIMULATION
    public void CalculatePositionsTickForeground()
    {
        float coulombDistance;
        float springDistance;
        float stopVelocity;


        if (_nodesDicId.Count > 500)
        {
            coulombDistance = _graphConfiguration.DenseSpringDistance;
            springDistance = _graphConfiguration.DenseSpringDistance;
            stopVelocity = _graphConfiguration.DenseStopVelocity;
        }
        else
        {
            coulombDistance = _graphConfiguration.LightCoulombDistance;
            springDistance = _graphConfiguration.LightSpringDistance;
            stopVelocity = _graphConfiguration.LightStopVelocity;
        }

        float coulombForce = _graphConfiguration.LightCoulombForce;
        float springForce = _graphConfiguration.LightSpringForce;
        float damping = _graphConfiguration.LightDamping;
        float maxVelocity = _graphConfiguration.LightMaxVelocity;
        float invMaxVelocity = 1f / _graphConfiguration.LightMaxVelocity;
        
        float velocitySum = 0f;


        // Apply the repulsion force between all nodes

        // Coulomb Force, Apply the repulsion force between all nodes
        foreach (var idAndNodeA in _nodesDicId)
        {
            var nodeA = idAndNodeA.Value;
            nodeA.AbsoluteVelocity = Vector3.zero;

            foreach (var idAndNodeB in _nodesDicId)
            {
                var nodeB = idAndNodeB.Value;
                if (nodeA == nodeB)
                    continue;

                Vector3 direction = nodeB.MegaTf.position - nodeA.MegaTf.position;
                float distance = direction.magnitude;

                if (distance > coulombDistance)
                    continue;

                float repulsiveForce = coulombForce * (1f / distance);

                Vector3 forceVector = repulsiveForce * direction.normalized;
                nodeA.AbsoluteVelocity -= forceVector;
            }

        }

        // Spring force - Attractive Force between connected nodes
        foreach (var idAndEdge in _edgesDicId)
        {
            var edge = idAndEdge.Value;
            Node source = edge.Source;
            Node target = edge.Target;


            Vector3 direction = target.MegaTf.position - source.MegaTf.position;

            float distance = direction.magnitude;

            if (distance < springDistance)
                continue;

            float attractiveForce = (distance - springDistance) * springForce;
            Vector3 forceVector = attractiveForce * direction.normalized;
            source.AbsoluteVelocity += forceVector;
            target.AbsoluteVelocity -= forceVector;
        }


        // Apply velocity
        foreach (var idAndNode in _nodesDicId)
        {
            var node = idAndNode.Value;
            node.AbsoluteVelocity *= damping;

            if (node.AbsoluteVelocity.magnitude > maxVelocity)
                node.AbsoluteVelocity /= node.AbsoluteVelocity.magnitude * invMaxVelocity;

            velocitySum += node.AbsoluteVelocity.magnitude;
            node.AbsolutePosition += node.AbsoluteVelocity * Time.deltaTime;
        }

        float velocityGraph = velocitySum / (float)_nodesDicId.Count;

        _reachStopVelocity = velocityGraph < stopVelocity;
    }

    public void RefreshTransformPositionsForeground()
    {
        var scalingFactor = _graphConfiguration.MegaGraphSize;

        foreach (var idAndNode in _nodesDicId)
        {
            var node = idAndNode.Value;
            node.MegaTf.localPosition = node.AbsolutePosition * scalingFactor;
        }

        foreach (var idAndEdge in _edgesDicId)
        {
            var edge = idAndEdge.Value;

            var line = edge.MegaLine;
            line.SetPosition(0, edge.Source.MegaTf.position * scalingFactor);
            line.SetPosition(1, edge.Target.MegaTf.position * scalingFactor);
        }

        UpdateLabelNodges();
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


    public void RefreshTransformPositionsBackground(Dictionary<int, NodeSimuData> nodeSimuDatas)
    {

        //DebugChrono.Instance.Start("RefreshTransformPositionsBackground");
        var megaScalingFactor = _graphConfiguration.MegaGraphSize;
        var miniScalingFactor = _graphConfiguration.MiniGraphSize;

        foreach (var idAnData in nodeSimuDatas)
        {
            if (!_nodesDicId.TryGetValue(idAnData.Key, out Node node))
                continue;

            var megaTf = node.MegaTf;
            var miniTf = node.MiniTf;

            var newCalculatedPosition = idAnData.Value.Position;
            var absolutePosition = node.AbsolutePosition;

            var lerpPosition = Vector3.Lerp(absolutePosition, newCalculatedPosition, .01f);
            var megaLerpPosition = Vector3.Lerp(megaTf.localPosition, newCalculatedPosition * megaScalingFactor, .01f);
            var miniLerpPosition = Vector3.Lerp(miniTf.localPosition, newCalculatedPosition * miniScalingFactor, .01f);

            node.AbsolutePosition = lerpPosition;
            megaTf.localPosition = megaLerpPosition;
            miniTf.localPosition = miniLerpPosition;
        }

        foreach (var idAndEdge in _edgesDicId)
        {
            var edge = idAndEdge.Value;

            var absoluteSourcePos = edge.Source.AbsolutePosition;
            var absoluteTargetPos = edge.Target.AbsolutePosition;

            var megaLine = edge.MegaLine;
            megaLine.SetPosition(0, absoluteSourcePos * megaScalingFactor);
            megaLine.SetPosition(1, absoluteTargetPos * megaScalingFactor);

            var miniLine = edge.MiniLine;
            miniLine.SetPosition(0, absoluteSourcePos * miniScalingFactor);
            miniLine.SetPosition(1, absoluteTargetPos * miniScalingFactor);
        }

        //DebugChrono.Instance.Stop("RefreshTransformPositionsBackground");

    }

    #endregion



    #region METRICS_CALCULATIONS
    public async void CalculateMetrics()
    {
        _metricsCalculated = 0;

        var tasks = new List<Task>();

        SemaphoreSlim semaphore = new SemaphoreSlim(0);

        CalculateMetric(CalculateShortestPathsAndCentralities);
        CalculateMetric(CalculateDegrees);
        CalculateMetric(CalculateClusteringCoefficients);

        await semaphore.WaitAsync();


        void CalculateMetric(Action metricCalculation)
        {
            tasks.Add(Task.Run(() =>
            {
                metricCalculation();

                // Increment the finished thread count atomically
                if (Interlocked.Increment(ref _metricsCalculated) == 3)
                {
                    // Signal the semaphore when all threads have finished
                    semaphore.Release();
                }
            }));
        }


        int threadId = Thread.CurrentThread.ManagedThreadId;

        _graphStyling.StyleGraph();
    }

    private void CalculateShortestPathsAndCentralities()
    {
        Func<Edge, double> edgeCost = edge => 1;

        // Betweenness Centrality
        Dictionary<Node, int> inShortestPathCountBC = new Dictionary<Node, int>();
        int nbOfPaths = 0;

        // Closeness Centrality
        Dictionary<Node, int> shortPathLengthSumCC = new Dictionary<Node, int>();

        foreach (var idAndNodeSource in _nodesDicId)
        {
            var node = idAndNodeSource.Value;
            inShortestPathCountBC.Add(node, 0);
            shortPathLengthSumCC.Add(node, 0);
        }


        float maxASP = 0;
        float minASP = float.MaxValue;

        UndirectedBidirectionalGraph<Node, Edge> undirectedGraphDatas = new(_graphDatas);

        foreach (var idAndNodeSource in _nodesDicId)
        {
            var rootNode = idAndNodeSource.Value;

            // ShortestPath
            double totalPathLength = 0;
            int reachableVertices = 0;
            TryFunc<Node, IEnumerable<Edge>> tryGetPaths = undirectedGraphDatas.ShortestPathsDijkstra(edgeCost, rootNode);

            
            foreach (var idAndNodeTarget in _nodesDicId)
            {
                var targetNode = idAndNodeTarget.Value;

                if (targetNode.Equals(rootNode) || !tryGetPaths(targetNode, out IEnumerable<Edge> path))
                    continue;


                // Shortest Path
                totalPathLength += path.Sum(edgeCost);
                reachableVertices++;
                nbOfPaths++;


                // Betweeness Centrality
                inShortestPathCountBC[rootNode]++;

                foreach (var edge in path)
                {
                    inShortestPathCountBC[edge.Target]++;
                }

                // Closeness Centrality
                shortPathLengthSumCC[rootNode] += path.Count();
            }


            // Shortest Path
            double avgPathLength = reachableVertices > 0 ? totalPathLength / reachableVertices : 0;

            float asp = (float)avgPathLength;
            rootNode.AverageShortestPathLength = (float)avgPathLength;

            if (asp > maxASP)
                maxASP = asp;

            if (asp < minASP)
                minASP = asp;
        }


        // Betweeness Centrality
        float maxBc = 0;
        float minBc = float.MaxValue;

        foreach (var nodeAndNbInShortPath in inShortestPathCountBC)
        {
            var node = nodeAndNbInShortPath.Key;

            var bc = (float)(nodeAndNbInShortPath.Value) / nbOfPaths;
            node.BetweennessCentrality = bc;

            if(bc > maxBc)
                maxBc = bc;
            
            if(bc < minBc) 
                minBc = bc;
        }

        // Closeness Centrality
        int nbNodesWithPathsMinusOne = nbOfPaths - 1;

        float maxCC = 0;
        float minCC = float.MaxValue;

        // Normalize BC
        foreach (var idAndNodeSource in _nodesDicId)
        {
            var node = idAndNodeSource.Value;

            float divider = (maxBc - minBc);

            node.BetweennessCentrality = (divider == 0f)?  0f : (node.BetweennessCentrality - minBc) / divider;

            var shortSum = shortPathLengthSumCC[node];
            var cc = (shortSum == 0f) ? float.MaxValue : (float)nbNodesWithPathsMinusOne / shortSum;
            node.ClosenessCentrality = cc;

            if (cc > maxCC)
                maxCC = cc;

            if (cc < minCC)
                minCC = cc;

            node.AverageShortestPathLength = (node.AverageShortestPathLength - minASP) / (maxASP - minASP);
        }

        foreach (var idAndNodeSource in _nodesDicId)
        {
            var node = idAndNodeSource.Value;
            //Debug.Log("----------------------");
            //Debug.Log(node.AverageShortestPathLength);
            //Debug.Log(node.BetweennessCentrality);
            //Debug.Log(node.ClosenessCentrality);
        }


        // Normalize CC
        foreach (var idAndNodeSource in _nodesDicId)
        {
            var node = idAndNodeSource.Value;
            node.ClosenessCentrality = (node.ClosenessCentrality - minCC) / (maxCC- minCC);
        }
    }

    private void CalculateDegrees()
    {
        int maxDegree = 0;
        int minDegree = int.MaxValue;


        foreach (var idAndNodeSource in _nodesDicId)
        {
            var node = idAndNodeSource.Value;
            var degree = (node.EdgeSource.Count + node.EdgeTarget.Count);
            node.Degree = degree;

            if (degree > maxDegree)
                maxDegree = degree;

            if (degree < minDegree)
                minDegree = degree;
        }

        foreach (var idAndNodeSource in _nodesDicId)
        {
            var node = idAndNodeSource.Value;
            node.Degree = (node.Degree -  minDegree) / (maxDegree - minDegree);
        }
    }

    private void CalculateClusteringCoefficients()
    {
        float maxCluster = 0;
        float minCluster = float.MaxValue;

        foreach (var idAndNodeSource in _nodesDicId)
        {
            var node = idAndNodeSource.Value;

            var neighbors = node.GetNeighbors();

            if (neighbors.Count < 2)
            {
                node.ClusteringCoefficient = 0f;
                continue;
            }


            int edgeCount = 0;

            for (int i = 0; i < neighbors.Count; i++)
            {
                for (int j = i + 1; j < neighbors.Count; j++)
                {

                    var neighborA = neighbors[i];
                    var neighborB = neighbors[j];

                    var neighborOfNeighborB = neighborB.GetNeighbors();

                    if (neighborOfNeighborB.Contains(neighborA))
                    {
                        edgeCount++;
                    }
                }
            }

            // C_i = 2n / (k_i * (k_i - 1))
            // With n : connection between neighbors
            //      k_i : number of neighbors

            double possibleConnections = neighbors.Count * (neighbors.Count - 1) / 2;
            var cluster = edgeCount / (float)possibleConnections;
            node.ClusteringCoefficient = cluster;

            if (cluster > maxCluster)
                maxCluster = cluster;

            if (cluster < minCluster)
                minCluster = cluster;
        }
    }
    #endregion

    public void Update()
    {
        UpdateLabelNodges();
    }

    public NodgesSimuData GetSimuDatas()
    {
        Dictionary<int, NodeSimuData> nodeSimuDatas = new();

        foreach (var idAndNode in _nodesDicId)
        {
            nodeSimuDatas.Add(idAndNode.Key, idAndNode.Value.ToSimuData());
        }

        Dictionary<int, EdgeSimuData> edgeSimuDatas = new();

        foreach (var idAndEdge in _edgesDicId)
        {
            edgeSimuDatas.Add(idAndEdge.Key, idAndEdge.Value.ToSimuData());
        }

        return new NodgesSimuData(nodeSimuDatas, edgeSimuDatas);
    }


    public bool IsInGraph(Transform nodeTf)
    {
        return _nodesDicTf.ContainsKey(nodeTf);
    }


    #region SELECTION
    public void TryClearSelection()
    {
        TryClearSelectedNode();
        TryClearSelectedEdge();
    }


    public void SelectEdge(Transform edgeTf)
    {
        if (_selectedEdge != null && edgeTf == _selectedEdge.MegaTf)
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
        if (_selectedNode != null && nodeTf == _selectedNode.MegaTf)
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

        if (_selectedNode != null && nodeTf == _selectedNode.MegaTf)
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
        labelNodge.SetFollow(node.MegaTf);
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
                labelNodge.SetFollow(edge.Source.MegaTf, edge.Target.MegaTf);
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
        labelNodge.SetFollow(edge.Source.MegaTf, edge.Target.MegaTf);

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
            labelNodge.SetFollow(node.MegaTf);
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

    #endregion

}
