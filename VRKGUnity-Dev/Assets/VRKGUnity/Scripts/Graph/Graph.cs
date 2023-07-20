using QuikGraph;
using QuikGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Graph
{
    public IReadOnlyDictionary<string, Node> NodesDicUID => _nodesDicUID;
    public IReadOnlyDictionary<string, Edge> EdgesDicUID => _edgesDicUID;

    public GraphConfiguration Configuration 
    {
        get 
        {
            return _graphConfiguration;
        } 
    }

    readonly GraphConfiguration _graphConfiguration;
    readonly GraphDbRepository _repository;
    readonly GraphManager _graphManager;
    readonly GraphStyling _graphStyling;
    readonly NodgePool _nodgePool;

    readonly Transform _mainGraphTf;
    readonly Transform _subGraphTf;

    Dictionary<string, Node> _nodesDicUID;
    Dictionary<string, Edge> _edgesDicUID;


    BidirectionalGraph<Node, Edge> _graphDatas;

    OntoNodeGroupTree _ontoNodeTree;

    GraphDbRepositoryNamespaces _repoNamespaces;

    int _metricsCalculated;

    #region NodgesUpdateOrCreation
    public Graph(GraphManager graphManager, GraphStyling graphStyling, NodgesDicUID nodges, NodgePool nodgePool, GraphDbRepository repository)
    {
        _nodesDicUID = nodges.NodesDicUID; 
        _edgesDicUID = nodges.EdgesDicUID;

        _graphManager = graphManager;


        _graphConfiguration = _graphManager.GraphConfiguration;
        _graphStyling = graphStyling;
        _graphDatas = new();

        _subGraphTf = _graphManager.SubGraph.Tf;
        _mainGraphTf = _graphManager.MainGraph.Tf;

        _nodgePool = nodgePool;
        _repository = repository;

        SetupNodes();
        SetupEdges();



        int seed = Configuration.SeedRandomPosition;
        foreach (var idAndNode in _nodesDicUID)
        {
            idAndNode.Value.ResetAbsolutePosition(seed);
        }
    }

    private void SetupNodes()
    {
        foreach(var idAndNode in _nodesDicUID)
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
        nodeTf.name = "Node " + node.GetShorterName();
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
    }

    private void SetupEdges()
    {
        foreach(var idAndEdge in _edgesDicUID)
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

        edgeStyler.gameObject.name = "Edge " + edge.NameWithPrefix;
        
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
    }

    public void UpdateNodges(NodgesDicUID nodges)
    {
        UpdateNodesInGraph(nodges.NodesDicUID);
        UpdateEdgesInGraph(nodges.EdgesDicUID);
    }

    private void UpdateNodesInGraph(Dictionary<string, Node> nodesDicUID)
    {
        Dictionary<string, Node> newNodes = new();
        Dictionary<string, Node> newNodesToRetrieveNames = new();

        bool doResetPosition = Configuration.ResetPositionNodeOnUpdate;
        int seed = Configuration.SeedRandomPosition;

        foreach (var idAndNode in nodesDicUID)
        {
            var uid = idAndNode.Key;

            if (_nodesDicUID.TryGetValue(uid, out Node keepNode))
            {
                // Already in the graph
                keepNode.OntoNodeGroup = null;
                newNodes.Add(uid, keepNode);
                _nodesDicUID.Remove(uid);

                if (doResetPosition)
                    keepNode.ResetAbsolutePosition(seed);
            }
            else
            {
                // New in the graph
                var newNode = idAndNode.Value;
                newNodes.Add(uid, newNode);
                newNodesToRetrieveNames.Add(uid, newNode);

                SetupNode(newNode, true);
                SetupNode(newNode, false);

                newNode.ResetAbsolutePosition(seed);
            }
        }

        ReleaseUnusedNodes();
        

        newNodesToRetrieveNames.AddRetrievedNames(_repository.GraphDbRepositoryDistantUris);


        _nodesDicUID = newNodes;


        void ReleaseUnusedNodes()
        {

            // Remove nodes that aren't in the graph anymore
            foreach (var idAndNode in _nodesDicUID)
            {
                var mainGraphStyler = idAndNode.Value.MainNodeStyler;
                var subGraphStyler = idAndNode.Value.SubNodeStyler;

                _nodgePool.Release(mainGraphStyler);
                _nodgePool.Release(subGraphStyler);

                _graphDatas.RemoveVertex(idAndNode.Value);
            }
        }
    }

    private void UpdateEdgesInGraph(Dictionary<string, Edge> edgesDicUID)
    {
        Dictionary<string, Edge> newEdges = new();

        foreach (var idAndEdge in edgesDicUID)
        {
            string uid = idAndEdge.Key;

            if (_edgesDicUID.TryGetValue(uid, out Edge edge))
            {
                // Already in the graph
                newEdges.Add(uid, edge);
                _edgesDicUID.Remove(uid);
            }
            else
            {
                // New in the graph
                newEdges.Add(uid, idAndEdge.Value);
                SetupEdge(idAndEdge.Value, true);
                SetupEdge(idAndEdge.Value, false);
            }
        }

        ReleaseUnusedEdges();
       

        _edgesDicUID = newEdges;

        void ReleaseUnusedEdges()
        {
            // Remove edges that aren't in the graph anymore
            foreach (var idAndEdge in _edgesDicUID)
            {
                var edge = idAndEdge.Value;
                var mainGraphStyler = edge.MainEdgeStyler;
                var subGraphStyler = edge.SubEdgeStyler;

                _nodgePool.Release(mainGraphStyler);
                _nodgePool.Release(subGraphStyler);

                // Clean unused edge from staying Nodes
                edge.CleanFromNodes(); 

                _graphDatas.RemoveEdge(idAndEdge.Value);
            }
        }

    }
    #endregion


    #region DynamicFilter
    public DynamicFilter Hide(HashSet<Node> nodeToHide)
    {
        DynamicFilter filter = new (nodeToHide);

        HashSet<Node> nodeToRemove = new();
        HashSet<Edge> edgeToHide = new();

        foreach (Node node in nodeToHide) 
        {
            node.HideNode();
            node.HideNode();

            nodeToRemove.Add(node);
        }

        foreach (Node node in nodeToRemove)
        {
            _nodesDicUID.Remove(node.UID);

            var edges = node.Edges;

            foreach (Edge edge in edges)
            {
                _edgesDicUID.Remove(edge.UID);
                edgeToHide.Add(edge);
            }
        }

        filter.HiddenEdges = edgeToHide;
        return filter;
    }

    public DynamicFilter HideAllExcept(HashSet<Node> nodeToKeepDisplay)
    {
        HashSet<Node> nodeToHide = new();

        HashSet<Node> nodeToRemove = new();
        HashSet<Edge> edgeToHide = new();


        foreach (Node node in _nodesDicUID.Values)
        {

            if (nodeToKeepDisplay.Contains(node))
                continue;

            nodeToHide.Add(node);
            node.HideNode();
            node.HideNode();

            nodeToRemove.Add(node);
        }

        foreach(Node node in nodeToRemove)
        {
            _nodesDicUID.Remove(node.UID);

            var edges = node.Edges;

            foreach (Edge edge in edges)
            {
                _edgesDicUID.Remove(edge.UID);
                edgeToHide.Add(edge);
            }
        }

        DynamicFilter filter = new(nodeToKeepDisplay, nodeToHide)
        {
            HiddenEdges = edgeToHide
        };

        return filter;
    }

    public void CancelFilter(DynamicFilter filter)
    {
        var nodesToUnhide = filter.HiddenNodes;

        foreach(Node node in nodesToUnhide)
        {
            if (_nodesDicUID.ContainsKey(node.UID))
                continue;

            _nodesDicUID.Add(node.UID, node);
            node.Unhide(_graphManager.GraphMode);

            var edges = node.Edges;

            foreach (Edge edge in edges)
            {
                if (_edgesDicUID.ContainsKey(edge.UID))
                    continue;

                _edgesDicUID.Add(edge.UID, edge);
            }
        }
    }

    #endregion



    #region MetricCalculations
    public async void CalculateMetrics(GraphDbRepositoryNamespaces graphRepoUris)
    {
        _repoNamespaces = graphRepoUris;
        _metricsCalculated = 0;

        var tasks = new List<Task>();

        SemaphoreSlim semaphore = new(0);

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
        static double edgeCost(Edge edge) => 1;

        // Betweenness Centrality
        Dictionary<Node, int> inShortestPathCountBC = new();
        int nbOfPaths = 0;

        // Closeness Centrality
        Dictionary<Node, int> shortPathLengthSumCC = new();

        foreach (var idAndNodeSource in _nodesDicUID)
        {
            var node = idAndNodeSource.Value;
            inShortestPathCountBC.Add(node, 0);
            shortPathLengthSumCC.Add(node, 0);
        }


        float maxASP = 0;
        float minASP = float.MaxValue;

        UndirectedBidirectionalGraph<Node, Edge> undirectedGraphDatas = new(_graphDatas);

        foreach (var idAndNodeSource in _nodesDicUID)
        {
            var rootNode = idAndNodeSource.Value;

            // ShortestPath
            double totalPathLength = 0;
            int reachableVertices = 0;
            TryFunc<Node, IEnumerable<Edge>> tryGetPaths = undirectedGraphDatas.ShortestPathsDijkstra(edgeCost, rootNode);

            
            foreach (var idAndNodeTarget in _nodesDicUID)
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
        foreach (var idAndNodeSource in _nodesDicUID)
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

        foreach (var idAndNodeSource in _nodesDicUID)
        {
            var node = idAndNodeSource.Value;
            //Debug.Log("----------------------");
            //Debug.Log(node.AverageShortestPathLength);
            //Debug.Log(node.BetweennessCentrality);
            //Debug.Log(node.ClosenessCentrality);
        }


        // Normalize CC
        foreach (var idAndNodeSource in _nodesDicUID)
        {
            var node = idAndNodeSource.Value;
            node.ClosenessCentrality = (node.ClosenessCentrality - minCC) / (maxCC- minCC);
        }
    }

    private void CalculateDegrees()
    {
        int maxDegree = 0;
        int minDegree = int.MaxValue;


        foreach (var idAndNodeSource in _nodesDicUID)
        {
            var node = idAndNodeSource.Value;
            var degree = (node.EdgeSource.Count + node.EdgeTarget.Count);
            node.Degree = degree;

            if (degree > maxDegree)
                maxDegree = degree;

            if (degree < minDegree)
                minDegree = degree;
        }

        foreach (var idAndNodeSource in _nodesDicUID)
        {
            var node = idAndNodeSource.Value;
            node.Degree = (node.Degree -  minDegree) / (maxDegree - minDegree);
        }
    }

    private void CalculateClusteringCoefficients()
    {
        float maxCluster = 0;
        float minCluster = float.MaxValue;

        foreach (var idAndNodeSource in _nodesDicUID)
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


    public void RefreshMainNodePositions(Dictionary<string, NodeSimuData> nodeSimuDatas)
    {
        var scalingFactor = (_graphManager.GraphMode == GraphMode.Desk) ? _graphConfiguration.DeskGraphSize : _graphConfiguration.ImmersionGraphSize;
        var lerpSmooth = _graphConfiguration.SimuParameters.LerpSmooth;

        foreach (var idAnData in nodeSimuDatas)
        {
            if (!_nodesDicUID.TryGetValue(idAnData.Key, out Node node))
                continue;

            Transform mainTf = node.MainGraphNodeTf;

            Vector3 newCalculatedPosition = idAnData.Value.Position;
            Vector3 absolutePosition = node.AbsolutePosition;

            Vector3 lerpPosition = Vector3.Lerp(absolutePosition, newCalculatedPosition, lerpSmooth);
            Vector3 megaLerpPosition = Vector3.Lerp(mainTf.localPosition, newCalculatedPosition * scalingFactor, lerpSmooth);

            node.AbsolutePosition = lerpPosition;
            mainTf.localPosition = megaLerpPosition;
        }

        foreach (var idAndEdge in _edgesDicUID)
        {
            Edge edge = idAndEdge.Value;

            Vector3 sourcePos = edge.Source.AbsolutePosition * scalingFactor;
            Vector3 targetPos = edge.Target.AbsolutePosition * scalingFactor;

            Vector3 direction = targetPos - sourcePos;

            var mainLine = edge.MainGraphLine;
            mainLine.SetPosition(0, sourcePos);
            mainLine.SetPosition(1, sourcePos + direction * .2f);
            mainLine.SetPosition(2, sourcePos + direction * .8f);
            mainLine.SetPosition(3, targetPos);
        }
    }


    public void ReleaseAll()
    {
        foreach(Node node  in _nodesDicUID.Values)
        {
            _nodgePool.Release(node.MainNodeStyler);
            _nodgePool.Release(node.SubNodeStyler);
        }

        foreach (Edge edge in _edgesDicUID.Values)
        {
            _nodgePool.Release(edge.MainEdgeStyler);
            _nodgePool.Release(edge.SubEdgeStyler);
        }

        _nodesDicUID = new();
        _edgesDicUID = new();
    }

    public NodgesSimuData CreateSimuDatas()
    {
        Dictionary<string, NodeSimuData> nodeSimuDatas = _nodesDicUID.ToSimuDatas();
        Dictionary<string, EdgeSimuData> edgeSimuDatas = _edgesDicUID.ToSimuDatas();

        return new NodgesSimuData(nodeSimuDatas, edgeSimuDatas);
    }
}
