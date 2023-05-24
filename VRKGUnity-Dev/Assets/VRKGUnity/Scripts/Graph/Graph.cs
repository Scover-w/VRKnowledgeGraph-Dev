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
    GraphUI _graphUI;

    Dictionary<int, Node> _nodesDicId;
    Dictionary<int, Edge> _edgesDicId;

    List<LabelNodgeUI> _labelNodgesUI;

    GraphConfiguration _graphConfiguration;

    Dictionary<Transform, Node> _nodesDicTf;
    Dictionary<Transform, Edge> _edgesDicTf;

    BidirectionalGraph<Node, Edge> _graphDatas;

    Node _selectedNode;    

    float _velocity;
    bool _reachStopVelocity;
    bool _areMetricsCalculated;

    int _metricsCalculated;

    #region CREATION_UPDATE_NODGES
    public Graph(GraphManager graphManager, GraphUI graphUI, GraphConfiguration graphConfiguration , Nodges nodges)
    {
        _nodesDicId = nodges.NodesDicId; 
        _edgesDicId = nodges.EdgesDicId;

        _nodesDicTf = new();
        _edgesDicTf = new();

        _graphManager = graphManager;
        _graphUI = graphUI;
        _labelNodgesUI = new List<LabelNodgeUI>();

        _graphConfiguration = graphConfiguration;

        _graphDatas = new();
        _areMetricsCalculated = false;


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
            SetupNode(idAndNode.Value);
            _graphDatas.AddVertex(idAndNode.Value);
        }
    }

    private void SetupNode(Node node)
    {
        var nodeTf = NodgePool.Instance.GetNode();
        nodeTf.name = "Node " + node.GetName();
        node.Tf = nodeTf;
        nodeTf.position = node.Position;

        _nodesDicTf.Add(nodeTf, node);
    }

    private void SetupEdges()
    {
        foreach(var idAndEdge in _edgesDicId)
        {
            SetupEdge(idAndEdge.Value);
            _graphDatas.AddEdge(idAndEdge.Value);
        }
    }

    private void SetupEdge(Edge edge)
    {
        var line = NodgePool.Instance.GetEdge();
        line.gameObject.name = "Edge " + edge.Value.ToString();
        edge.Line = line;

        _edgesDicTf.Add(line.transform, edge);
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
                SetupNode(newNode);

                newNode.ResetPosition(seed);
            }
        }


        var nodgePool = NodgePool.Instance;

        foreach (var idAndNode in _nodesDicId)
        {
            nodgePool.Release(idAndNode.Value.Tf);
        }

        await _graphManager.NodeUriRetriever.RetrieveNames(newNodesToRetrieveNames);


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
                SetupEdge(idAndEdge.Value);
            }
        }


        var nodgePool = NodgePool.Instance;

        foreach (var idAndEdge in _edgesDicId)
        {
            var edge = idAndEdge.Value;
            nodgePool.Release(edge.Line);
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
            coulombDistance = _graphConfiguration.BigSpringDistance;
            springDistance = _graphConfiguration.BigSpringDistance;
            stopVelocity = _graphConfiguration.BigStopVelocity;
        }
        else
        {
            coulombDistance = _graphConfiguration.CoulombDistance;
            springDistance = _graphConfiguration.SpringDistance;
            stopVelocity = _graphConfiguration.StopVelocity;
        }

        float coulombForce = _graphConfiguration.CoulombForce;
        float springForce = _graphConfiguration.SpringForce;
        float damping = _graphConfiguration.Damping;
        float maxVelocity = _graphConfiguration.MaxVelocity;
        float invMaxVelocity = 1f / _graphConfiguration.MaxVelocity;
        
        float velocitySum = 0f;


        // Apply the repulsion force between all nodes

        // Coulomb Force, Apply the repulsion force between all nodes
        foreach (var idAndNodeA in _nodesDicId)
        {
            var nodeA = idAndNodeA.Value;
            nodeA.Velocity = Vector3.zero;

            foreach (var idAndNodeB in _nodesDicId)
            {
                var nodeB = idAndNodeB.Value;
                if (nodeA == nodeB)
                    continue;

                Vector3 direction = nodeB.Tf.position - nodeA.Tf.position;
                float distance = direction.magnitude;

                if (distance > coulombDistance)
                    continue;

                float repulsiveForce = coulombForce * (1f / distance);

                Vector3 forceVector = repulsiveForce * direction.normalized;
                nodeA.Velocity -= forceVector;
            }

        }

        // Spring force - Attractive Force between connected nodes
        foreach (var idAndEdge in _edgesDicId)
        {
            var edge = idAndEdge.Value;
            Node source = edge.Source;
            Node target = edge.Target;


            Vector3 direction = target.Tf.position - source.Tf.position;

            float distance = direction.magnitude;

            if (distance < springDistance)
                continue;

            float attractiveForce = (distance - springDistance) * springForce;
            Vector3 forceVector = attractiveForce * direction.normalized;
            source.Velocity += forceVector;
            target.Velocity -= forceVector;
        }


        // Apply velocity
        foreach (var idAndNode in _nodesDicId)
        {
            var node = idAndNode.Value;
            node.Velocity *= damping;

            if (node.Velocity.magnitude > maxVelocity)
                node.Velocity /= node.Velocity.magnitude * invMaxVelocity;

            velocitySum += node.Velocity.magnitude;
            node.Position += node.Velocity * Time.deltaTime;
        }

        float velocityGraph = velocitySum / (float)_nodesDicId.Count;

        _reachStopVelocity = velocityGraph < stopVelocity;
    }

    public void RefreshTransformPositionsForeground()
    {
        foreach (var idAndNode in _nodesDicId)
        {
            var node = idAndNode.Value;
            node.Tf.position = node.Position;
        }

        foreach (var idAndEdge in _edgesDicId)
        {
            var edge = idAndEdge.Value;

            var line = edge.Line;
            line.SetPosition(0, edge.Source.Tf.position);
            line.SetPosition(1, edge.Target.Tf.position);
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
        foreach (var idAnData in nodeSimuDatas)
        {
            if (!_nodesDicId.TryGetValue(idAnData.Key, out Node node))
                continue;

            var tf = node.Tf;
            var position = idAnData.Value.Position;
            var lerpPosition = Vector3.Lerp(tf.position, position, .01f);

            node.Position = lerpPosition;
            node.Tf.position = lerpPosition;
        }

        foreach (var idAndEdge in _edgesDicId)
        {
            var edge = idAndEdge.Value;

            var line = edge.Line;
            line.SetPosition(0, edge.Source.Position);
            line.SetPosition(1, edge.Target.Position);
        }
    }

    #endregion



    #region METRICS_CALCULATIONS
    public async void CalculateMetrics()
    {
        DebugChrono.Instance.Start("CalculateMetrics");

        _areMetricsCalculated = false;
        _metricsCalculated = 0;

        var tasks = new List<Task>();

        SemaphoreSlim semaphore = new SemaphoreSlim(0);

        CalculateMetric(CalculateShortestPathsAndCentralities);
        CalculateMetric(CalculateDegrees);
        CalculateMetric(CalculateClusteringCoefficients);

        await semaphore.WaitAsync();

        DebugChrono.Instance.Stop("CalculateMetrics");
        _areMetricsCalculated = true;



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


        foreach (var idAndNodeSource in _nodesDicId)
        {
            var rootNode = idAndNodeSource.Value;

            // ShortestPath
            double totalPathLength = 0;
            int reachableVertices = 0;
            TryFunc<Node, IEnumerable<Edge>> tryGetPaths = _graphDatas.ShortestPathsDijkstra(edgeCost, rootNode);

            
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
            rootNode.AverageShortestPathLength = (float)avgPathLength;
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

        // Normalize BC
        foreach (var idAndNodeSource in _nodesDicId)
        {
            var node = idAndNodeSource.Value;

            float divider = (maxBc - minBc);

            node.BetweennessCentrality = (divider == 0f)?  0f : (node.BetweennessCentrality - minBc) / divider;

            var shortSum = shortPathLengthSumCC[node];
            node.ClosenessCentrality = (shortSum == 0f)? float.MaxValue : (float)nbNodesWithPathsMinusOne / shortSum;
        }
    }

    private void CalculateDegrees()
    {
        foreach (var idAndNodeSource in _nodesDicId)
        {
            var node = idAndNodeSource.Value;
            node.Degree = (node.EdgeSource.Count + node.EdgeTarget.Count);
        }
    }

    private void CalculateClusteringCoefficients()
    {
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
            node.ClusteringCoefficient = edgeCount / (float)possibleConnections;
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

    public void SelectNode(Transform nodeTf)
    {
        if (nodeTf == null)
        {
            ClearSelectedNode();
            return;
        }

        if (_selectedNode != null && nodeTf == _selectedNode.Tf)
            return;

        if (!_nodesDicTf.TryGetValue(nodeTf, out Node node))
        {
            Debug.LogError("Transform not linked to a node");
            ClearSelectedNode();
            return;
        }


        ReleaseLabelNodges();

        _selectedNode = node;
        _graphUI.DisplayInfoNode(_selectedNode);
        PropagateLabelNodge(_selectedNode, _graphConfiguration.LabelNodgePropagation, new HashSet<Node>(), new HashSet<Edge>());

        Selection.activeObject = nodeTf;
    }

    public void ClearSelectedNode()
    {
        if (!HasASelectedNode)
            return;

        ReleaseLabelNodges();
        _graphUI.DisplayInfoNode(null);

        _selectedNode = null;
    }

    private void PropagateLabelNodge(Node node, int propagationValue, HashSet<Node> nodesLabeled, HashSet<Edge> edgesLabeled)
    {
        nodesLabeled.Add(node);

        var labelNodge = NodgePool.Instance.GetLabelNodge();
        labelNodge.SetFollow(node.Tf);
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
                labelNodge.SetFollow(edge.Source.Tf, edge.Target.Tf);
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
}
