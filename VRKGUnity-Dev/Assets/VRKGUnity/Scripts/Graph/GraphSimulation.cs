using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GraphSimulation : MonoBehaviour
{
    public bool IsRunningSimulation { get { return _isRunningSimulation; } }

    [SerializeField]
    GraphManager _graphManager;

    Dictionary<int, NodeSimuData> _nodeSimuDatas;
    Dictionary<int, NodeSimuData> _newNodeSimuDatas;

    Graph _graph;

    SemaphoreSlim _threadEndedSemaphore;

    GraphConfiguration _graphConfiguration;

    bool _isRunningSimulation = false;
    float _refreshDuration;
    bool _refreshGraph;

    private void Start()
    {
        _graphConfiguration = GraphConfiguration.Instance;
    }

    public void Run(Graph graph)
    {
        _graph = graph;

        _refreshDuration = -1f;
        _newNodeSimuDatas = null;
        _refreshGraph = true;
        _threadEndedSemaphore = new SemaphoreSlim(0);

        var nodgesSimuDatas = graph.CreateSimuDatas();
        _nodeSimuDatas = nodgesSimuDatas.NodeSimuDatas.Clone();

        ThreadPool.QueueUserWorkItem(CalculatingPositionsBackground, nodgesSimuDatas);

        StartCoroutine(RefreshingGraphPositions(graph));
    }

    public async Task Run(NodgesSimuData nodgesSimuDatas)
    {
        _newNodeSimuDatas = null;
        _threadEndedSemaphore = new SemaphoreSlim(0);
        _refreshGraph = false;

        ThreadPool.QueueUserWorkItem(CalculatingPositionsBackground, nodgesSimuDatas);
        StartCoroutine(MonitoringSimulationTime());

        await _threadEndedSemaphore.WaitAsync();
        _threadEndedSemaphore = null;
    }


    public void ForceStop()
    {
        _isRunningSimulation = false;
    }

    IEnumerator RefreshingGraphPositions(Graph graph)
    {
        _isRunningSimulation = true;

        float time = 0f;
        float speed = 1f / _graphConfiguration.MaxSimulationTime;

        while (_isRunningSimulation && time < 1f)
        {
            if(_newNodeSimuDatas != null)
            {
                _nodeSimuDatas = _newNodeSimuDatas;
                _newNodeSimuDatas = null;
            }

            graph.RefreshMainNodePositions(_nodeSimuDatas);
            yield return null;

            time += Time.deltaTime * speed;
        }

        _isRunningSimulation = false;

        EndRefreshingPosition(graph);
    }

    IEnumerator MonitoringSimulationTime()
    {
        _isRunningSimulation = true;

        float time = 0f;
        float speed = 1f / _graphConfiguration.MaxSimulationTime;

        while (_isRunningSimulation && time < 1f)
        {
            yield return null;

            time += Time.deltaTime * speed;
        }

        _isRunningSimulation = false;
    }

    private async Task EndRefreshingPosition(Graph graph)
    {
        await _threadEndedSemaphore.WaitAsync();

        graph.RefreshMainNodePositions(_newNodeSimuDatas);

        if(_graphManager != null)
            _graphManager.SimulationStopped();

    }

    private void CalculatingPositionsBackground(object state)
    {
        DebugChrono.Instance.Start("firstTickGRaph");
        var nodgesSimuDatas = (NodgesSimuData) state;
        var hasReachStopVelocity = false;
        bool firstTick = true;
        float timer = 0f;
        _isRunningSimulation = true;

        while (_isRunningSimulation && !hasReachStopVelocity)
        {
            DebugChrono.Instance.Start("tickGRaph");

            hasReachStopVelocity = CalculateNodeSimuData(nodgesSimuDatas);

            if (firstTick && _refreshGraph)
            {
                firstTick = false;
                var durationB = DebugChrono.Instance.Stop("firstTickGRaph", true);
                _refreshDuration = durationB * 3f;
                DebugChrono.Instance.Start("tickGRaph");
                continue;
            }


            var duration = DebugChrono.Instance.Stop("tickGRaph", false);
            timer += duration;

            if(timer > .0001f && _refreshGraph)
            {
                _newNodeSimuDatas = nodgesSimuDatas.NodeSimuDatas.Clone();
                timer = 0f;
            }
        }

        _newNodeSimuDatas = nodgesSimuDatas.NodeSimuDatas.Clone();
        _isRunningSimulation = false;

        _threadEndedSemaphore.Release();
    }

    private bool CalculateNodeSimuData(NodgesSimuData nodgesSimuData)
    {
        float coulombDistance;
        float springDistance;
        float stopVelocity;

        var nodesSimuData = nodgesSimuData.NodeSimuDatas;
        var edgesSimuData = nodgesSimuData.EdgeSimuDatas;


        var config = _graphConfiguration;

        if (nodesSimuData.Count > 500)
        {
            coulombDistance = config.DenseSpringDistance;
            springDistance = config.DenseSpringDistance;
            stopVelocity = config.DenseStopVelocity;
        }
        else
        {
            coulombDistance = config.LightCoulombDistance;
            springDistance = config.LightSpringDistance;
            stopVelocity = config.LightStopVelocity;
        }

        float coulombForce = config.LightCoulombForce;
        float springForce = config.LightSpringForce;
        float damping = config.LightDamping;
        float maxVelocity = config.LightMaxVelocity;
        float invMaxVelocity = 1f / config.LightMaxVelocity;

        float velocitySum = 0f;
        float tickDeltaTime = config.TickDeltaTime;

        // Apply the repulsion force between all nodes

        // Coulomb Force, Apply the repulsion force between all nodes
        foreach (var idAndNodeDataA in nodesSimuData)
        {
            var nodeDataA = idAndNodeDataA.Value;
            nodeDataA.Velocity = Vector3.zero;

            foreach (var idAndNodeDataB in nodesSimuData)
            {
                var nodeDataB = idAndNodeDataB.Value;
                if (nodeDataA.Id == nodeDataB.Id)
                    continue;

                Vector3 direction = nodeDataB.Position - nodeDataA.Position;
                float distance = direction.magnitude;

                if (distance > coulombDistance)
                    continue;

                float repulsiveForce = coulombForce * (1f / distance);

                Vector3 forceVector = repulsiveForce * direction.normalized;
                nodeDataA.Velocity -= forceVector;
            }

        }

        // Spring force - Attractive Force between connected nodes
        foreach (var idAndEdgeData in edgesSimuData)
        {
            var edge = idAndEdgeData.Value;

            NodeSimuData nodeDataA;
            NodeSimuData nodeDataB;


            if (!nodesSimuData.TryGetValue(edge.IdA, out nodeDataA))
                continue;

            if (!nodesSimuData.TryGetValue(edge.IdB, out nodeDataB))
                continue;



            Vector3 direction = nodeDataB.Position - nodeDataA.Position;

            float distance = direction.magnitude;

            if (distance < springDistance)
                continue;

            float attractiveForce = (distance - springDistance) * springForce;
            Vector3 forceVector = attractiveForce * direction.normalized;
            nodeDataA.Velocity += forceVector;
            nodeDataB.Velocity -= forceVector;
        }


        // Apply velocity
        foreach (var idAndNodeData in nodesSimuData)
        {
            var nodeData = idAndNodeData.Value;
            nodeData.Velocity *= damping;

            if (nodeData.Velocity.magnitude > maxVelocity)
                nodeData.Velocity /= nodeData.Velocity.magnitude * invMaxVelocity;

            velocitySum += nodeData.Velocity.magnitude;
            nodeData.Position += nodeData.Velocity * tickDeltaTime;
        }

        float velocityGraph = velocitySum / (float)nodesSimuData.Count;

        return velocityGraph < stopVelocity;
    }

    private void OnDisable()
    {
        _isRunningSimulation = false;
    }
}
