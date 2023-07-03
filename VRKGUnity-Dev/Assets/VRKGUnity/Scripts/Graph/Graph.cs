using QuikGraph;
using QuikGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class Graph
{
    public float Velocity { get { return _velocity; } }
    public bool ReachStopVelocity {  get { return _reachStopVelocity; } }

    public IReadOnlyDictionary<int, Node> NodesDicId => _nodesDicId;
    public IReadOnlyDictionary<int, Edge> EdgesDicId => _edgesDicId;

    public IReadOnlyDictionary<Transform, Node> NodesDicTf => _nodesDicTf;
    public IReadOnlyDictionary<Transform, Edge> EdgesDicTf => _edgesDicTf;

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
    

    Dictionary<int, Node> _nodesDicId;
    Dictionary<int, Edge> _edgesDicId;

    GraphConfiguration _graphConfiguration;

    Dictionary<Transform, Node> _nodesDicTf;
    Dictionary<Transform, Edge> _edgesDicTf;

    BidirectionalGraph<Node, Edge> _graphDatas;

    GraphDbRepository _repository;
    GraphDbRepositoryNamespaces _repoNamespaces;

    OntoNodeGroupTree _ontoNodeTree;

    NodgePool _nodgePool;

    Transform _mainGraphTf;
    Transform _subGraphTf;

    float _velocity;
    bool _reachStopVelocity;

    int _metricsCalculated;

    #region CREATION_UPDATE_NODGES
    public Graph(GraphManager graphManager, GraphStyling graphStyling, NodgesDicId nodges, NodgePool nodgePool)
    {
        _nodesDicId = nodges.NodesDicId; 
        _edgesDicId = nodges.EdgesDicId;

        _nodesDicTf = new();
        _edgesDicTf = new();

        _graphManager = graphManager;


        _graphConfiguration = _graphManager.GraphConfiguration;
        _graphStyling = graphStyling;
        _graphDatas = new();

        _subGraphTf = _graphManager.SubGraph.Tf;
        _mainGraphTf = _graphManager.MainGraph.Tf;

        _nodgePool = nodgePool;

        SetupNodes();
        SetupEdges();



        int seed = Configuration.SeedRandomPosition;
        foreach (var idAndNode in _nodesDicId)
        {
            idAndNode.Value.ResetAbsolutePosition(seed);
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

    private void SetupNode(Node node, bool isForMainGraph)
    {
        var nodeStyler = _nodgePool.GetNodeStyler();
        nodeStyler.Node = node;

        var nodeTf = nodeStyler.Tf;
        nodeTf.name = "Node " + node.GetName();
        nodeTf.position = node.AbsolutePosition;

        if(isForMainGraph)
        {
            nodeStyler.GraphType = GraphType.Main;
            node.MainNodeStyler = nodeStyler;
            node.MainGraphNodeTf = nodeTf;
            nodeTf.parent = _mainGraphTf;
            
        }
        else
        {
            nodeStyler.GraphType = GraphType.Sub;
            node.SubNodeStyler = nodeStyler;
            node.SubGraphNodeTf = nodeTf;
            nodeTf.parent = _subGraphTf;
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

    private void SetupEdge(Edge edge,bool isForMainGraph)
    {
        var edgeStyler = _nodgePool.GetEdgeStyler();
        edgeStyler.Edge = edge;

        edgeStyler.gameObject.name = "Edge " + edge.Value.ToString();
        
        if(isForMainGraph)
        {
            edgeStyler.GraphType = GraphType.Main;
            edge.MainGraphLine = edgeStyler.LineRenderer;
            edge.MainEdgeStyler = edgeStyler;
            edge.MainGraphEdgeTf = edgeStyler.ColliderTf;
            edgeStyler.Tf.parent = _mainGraphTf;
        }
        else
        {
            edgeStyler.GraphType = GraphType.Sub;
            edge.SubGraphLine = edgeStyler.LineRenderer;
            edge.SubEdgeStyler = edgeStyler;
            edge.SubGraphEdgeTf = edgeStyler.ColliderTf;
            edgeStyler.Tf.parent = _subGraphTf;
        }

        edgeStyler.Tf.localPosition = Vector3.zero;

        _edgesDicTf.Add(edgeStyler.ColliderTf, edge);
    }

    public async Task UpdateNodges(NodgesDicId nodges)
    {
        UpdateNodesInGraph(nodges.NodesDicId);
        UpdateEdgesInGraph(nodges.EdgesDicId);
    }

    private void UpdateNodesInGraph(Dictionary<int,Node> nodesDicId)
    {
        Dictionary<int, Node> newNodes = new();
        Dictionary<int, Node> newNodesToRetrieveNames = new();

        bool doResetPosition = Configuration.ResetPositionNodeOnUpdate;
        int seed = Configuration.SeedRandomPosition;

        foreach (var idAndNode in nodesDicId)
        {
            var id = idAndNode.Key;

            if (_nodesDicId.TryGetValue(id, out Node keepNode))
            {
                // Already in the graph
                keepNode.OntoNodeGroup = null;
                newNodes.Add(id, keepNode);
                _nodesDicId.Remove(id);

                if (doResetPosition)
                    keepNode.ResetAbsolutePosition(seed);
            }
            else
            {
                // New in the graph
                var newNode = idAndNode.Value;
                newNodes.Add(id, newNode);
                newNodesToRetrieveNames.Add(id, newNode);

                SetupNode(newNode, true);
                SetupNode(newNode, false);

                newNode.ResetAbsolutePosition(seed);
            }
        }

        ReleaseUnusedNodes();
        

        newNodesToRetrieveNames.AddRetrievedNames(_repository.GraphDbRepositoryDistantUris);
        newNodesToRetrieveNames.ExtractNodeNamesToProperties();


        _nodesDicId = newNodes;


        void ReleaseUnusedNodes()
        {

            // Remove nodes that aren't in the graph anymore
            foreach (var idAndNode in _nodesDicId)
            {
                var mainGraphStyler = idAndNode.Value.MainNodeStyler;
                var subGraphStyler = idAndNode.Value.SubNodeStyler;

                _nodgePool.Release(mainGraphStyler);
                _nodgePool.Release(subGraphStyler);
                _nodesDicTf.Remove(mainGraphStyler.Tf);
                _nodesDicTf.Remove(subGraphStyler.Tf);

                _graphDatas.RemoveVertex(idAndNode.Value);
            }
        }
    }

    private void UpdateEdgesInGraph(Dictionary<int, Edge> edgesDicId)
    {
        Dictionary<int, Edge> newEdges = new();

        foreach (var idAndEdge in edgesDicId)
        {
            var id = idAndEdge.Key;

            if (_edgesDicId.TryGetValue(id, out Edge edge))
            {
                // Already in the graph
                newEdges.Add(id, edge);
                _edgesDicId.Remove(id);
            }
            else
            {
                // New in the graph
                newEdges.Add(id, idAndEdge.Value);
                SetupEdge(idAndEdge.Value, true);
                SetupEdge(idAndEdge.Value, false);
            }
        }

        ReleaseUnusedEdges();
       

        _edgesDicId = newEdges;

        void ReleaseUnusedEdges()
        {
            // Remove edges that aren't in the graph anymore
            foreach (var idAndEdge in _edgesDicId)
            {
                var edge = idAndEdge.Value;
                var mainGraphStyler = edge.MainEdgeStyler;
                var subGraphStyler = edge.SubEdgeStyler;

                _nodgePool.Release(mainGraphStyler);
                _nodgePool.Release(subGraphStyler);
                _edgesDicTf.Remove(mainGraphStyler.ColliderTf);
                _edgesDicTf.Remove(subGraphStyler.ColliderTf);

                // Clean unused edge from staying Nodes
                edge.CleanFromNodes(); 

                _graphDatas.RemoveEdge(idAndEdge.Value);
            }
        }

    }
    #endregion


    public void RefreshMainNodePositions(Dictionary<int, NodeSimuData> nodeSimuDatas)
    {

        // DebugChrono.Instance.Start("RefreshTransformPositionsBackground");
        var scalingFactor =  (_graphManager.GraphMode == GraphMode.Desk)? _graphConfiguration.DeskGraphSize : _graphConfiguration.ImmersionGraphSize;

        foreach (var idAnData in nodeSimuDatas)
        {
            if (!_nodesDicId.TryGetValue(idAnData.Key, out Node node))
                continue;

            var megaTf = node.MainGraphNodeTf;

            var newCalculatedPosition = idAnData.Value.Position;
            var absolutePosition = node.AbsolutePosition;

            var lerpPosition = Vector3.Lerp(absolutePosition, newCalculatedPosition, .01f);
            var megaLerpPosition = Vector3.Lerp(megaTf.localPosition, newCalculatedPosition * scalingFactor, .01f);

            node.AbsolutePosition = lerpPosition;
            megaTf.localPosition = megaLerpPosition;
        }

        foreach (var idAndEdge in _edgesDicId)
        {
            var edge = idAndEdge.Value;

            var absoluteSourcePos = edge.Source.AbsolutePosition;
            var absoluteTargetPos = edge.Target.AbsolutePosition;

            var megaLine = edge.MainGraphLine;
            megaLine.SetPosition(0, absoluteSourcePos * scalingFactor);
            megaLine.SetPosition(1, absoluteTargetPos * scalingFactor);
        }

        // DebugChrono.Instance.Stop("RefreshTransformPositionsBackground");
    }


    #region METRICS_CALCULATIONS
    public async void CalculateMetrics(GraphDbRepositoryNamespaces graphRepoUris)
    {
        _repoNamespaces = graphRepoUris;
        _metricsCalculated = 0;

        var tasks = new List<Task>();

        SemaphoreSlim semaphore = new SemaphoreSlim(0);

        CalculateMetric(CalculateShortestPathsAndCentralities);
        CalculateMetric(CalculateDegrees);
        CalculateMetric(CalculateClusteringCoefficients);
        CalculateMetric(CalculateOntology);

        await semaphore.WaitAsync();

        int threadId = Thread.CurrentThread.ManagedThreadId;

        _graphStyling.StyleGraph(new StyleChange().Add(StyleChangeType.All), _graphManager.GraphMode);


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

    private void CalculateOntology()
    {
        _ontoNodeTree = OntoNodeGroupTree.CreateOntoNodeTree(_repoNamespaces.OntoTreeDict, Configuration);
    }
    #endregion

    public NodgesSimuData CreateSimuDatas()
    {
        Dictionary<int, NodeSimuData> nodeSimuDatas = _nodesDicId.ToSimuDatas();
        Dictionary<int, EdgeSimuData> edgeSimuDatas = _edgesDicId.ToSimuDatas();

        return new NodgesSimuData(nodeSimuDatas, edgeSimuDatas);
    }
}
