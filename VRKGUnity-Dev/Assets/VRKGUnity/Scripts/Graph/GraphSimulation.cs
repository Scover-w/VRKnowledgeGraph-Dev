using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GraphSimulation : MonoBehaviour
{
    public bool IsRunningSimulation { get { return _isRunningSimulation; } }

    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    Dictionary<string, NodeSimuData> _nodeSimuDatas;
    Dictionary<string, NodeSimuData> _newNodeSimuDatas;

    SemaphoreSlim _threadEndedSemaphore;

    GraphConfiguration _graphConfiguration;

    bool _isRunningSimulation;
    bool _wantStopSimulation;

    bool _refreshGraph;

    private void Start()
    {
        _graphConfiguration = GraphConfiguration.Instance;
        _isRunningSimulation = false;
    }

    public void Run(Graph graph)
    {
        _newNodeSimuDatas = null;
        _refreshGraph = true;
        _wantStopSimulation = false;
        _threadEndedSemaphore = new SemaphoreSlim(0);

        var nodgesSimuDatas = graph.CreateSimuDatas();
        _nodeSimuDatas = nodgesSimuDatas.NodeSimuDatas.Clone();

        ThreadPool.QueueUserWorkItem(CalculatingPositionsBackground, nodgesSimuDatas);
        StartCoroutine(RefreshingNodesPositions(graph));
    }

    public void ForceStop()
    {
        _wantStopSimulation = false;
    }

    IEnumerator RefreshingNodesPositions(Graph graph)
    {
        _isRunningSimulation = true;

        float time = 0f;
        float speed = 1f / _graphConfiguration.SimuParameters.MaxSimulationTime;

        while (_isRunningSimulation && time < 1f && !_wantStopSimulation)
        {
            if(_newNodeSimuDatas != null)
            {
                _nodeSimuDatas = _newNodeSimuDatas;
                _newNodeSimuDatas = null;
            }
            
            graph.RefreshMainNodePositions(_nodeSimuDatas); // Take 1 to 2ms

            yield return null;

            time += Time.deltaTime * speed;
        }

        _wantStopSimulation = true;

        _ = EndRefreshingPosition(graph);
    }

    private async Task EndRefreshingPosition(Graph graph)
    {
        await _threadEndedSemaphore.WaitAsync();

        graph.RefreshMainNodePositions(_newNodeSimuDatas, false);

        if(_graphManager != null)
            _graphManager.SimulationStopped();
    }

    private void CalculatingPositionsBackground(object state)
    {
        var nodgesSimuDatas = (NodgesSimuData) state;
        var hasReachStopVelocity = false;

        float timer = 0f;

        _isRunningSimulation = true;

        
        while (_isRunningSimulation && !hasReachStopVelocity && !_wantStopSimulation)
        {
            DebugChrono.Instance.Start("tickGRaph");

            hasReachStopVelocity = CalculateNodeSimuData(nodgesSimuDatas);

            var duration = DebugChrono.Instance.Stop("tickGRaph", false);
            timer += duration;

            if(timer > .0001f && _refreshGraph)
            {
                _newNodeSimuDatas = nodgesSimuDatas.NodeSimuDatas.Clone();
                timer = 0f;
            }
        }


        float maxRadius = GetMaxRadius(nodgesSimuDatas);
        Debug.Log("MaxRadius : " + maxRadius);
        _referenceHolderSO.MaxRadiusGraph = maxRadius;

        _newNodeSimuDatas = nodgesSimuDatas.NodeSimuDatas.Clone();
        _isRunningSimulation = false;

        _threadEndedSemaphore.Release();
    }

    private bool CalculateNodeSimuData(NodgesSimuData nodgesSimuData)
    {
        var nodesSimuData = nodgesSimuData.NodeSimuDatas;
        var edgesSimuData = nodgesSimuData.EdgeSimuDatas;


        var config = _graphConfiguration.SimuParameters;

        float coulombDistance = config.CoulombDistance;
        float springDistance = config.SpringDistance;
        float stopVelocity = config.StopVelocity;

        float coulombForce = config.CoulombForce;
        float springForce = config.SpringForce;
        float damping = config.Damping;
        float maxVelocity = config.MaxVelocity;
        float invMaxVelocity = 1f / config.MaxVelocity;

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
                if (nodeDataA.UID == nodeDataB.UID)
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

            if (!nodesSimuData.TryGetValue(edge.UidA, out NodeSimuData nodeDataA))
                continue;

            if (!nodesSimuData.TryGetValue(edge.UidB, out NodeSimuData nodeDataB))
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


    private float GetMaxRadius(NodgesSimuData nodgesSimuData)
    {
        float maxSqrDistance = 0f;

        var nodesSimuData = nodgesSimuData.NodeSimuDatas;

        foreach (var nodeSimuData in nodesSimuData.Values)
        {
            var sqrDistance = nodeSimuData.Position.sqrMagnitude;

            if (sqrDistance > maxSqrDistance)
                maxSqrDistance = sqrDistance;
        }

        return Mathf.Sqrt(maxSqrDistance);
    }


    private void OnDisable()
    {
        _wantStopSimulation = true;
    }
}
