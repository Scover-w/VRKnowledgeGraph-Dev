using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.Graphs;
using UnityEngine;

public class GraphSimulation : MonoBehaviour
{
    public bool IsRunning { get { return _isRunning; } }

    [SerializeField]
    float _tickDeltaTime = 0.016f;

    Dictionary<int, NodeSimuData> _nodeSimuDatas;
    Dictionary<int, NodeSimuData> _newNodeSimuDatas;

    CancellationTokenSource _cancellationTokenS;

    Graph _graph;


    bool _isRunning = false;
    bool _threadMode = true;
    float _refreshDurationBackground;



    public void Run(Graph graph)
    {
        _graph = graph;

        if (!_threadMode)
        {
            StartCoroutine(ExpandingGraphForeground(graph));
            return;
        }

        _refreshDurationBackground = -1f;
        _newNodeSimuDatas = null;
        _cancellationTokenS = new CancellationTokenSource();
        var nodgesSimuDatas = graph.GetSimuDatas();
        _nodeSimuDatas = nodgesSimuDatas.NodeSimuDatas.Clone();
        ThreadPool.QueueUserWorkItem(CalculatingPositionsBackground, nodgesSimuDatas);
        StartCoroutine(RefreshingGraphPositionsBackground(graph));
    }


    public void ForceStop()
    {
        _isRunning = false;

    }

    IEnumerator ExpandingGraphForeground(Graph graph)
    {
        _isRunning = true;
        bool reachStopVelocity = false;

        float time = 0f;
        float speed = 1f / 5f;

        while(_isRunning && !reachStopVelocity && time < 1f)
        {
            graph.CalculatePositionsTickForeground();
            graph.RefreshTransformPositionsForeground();
            yield return null;

            reachStopVelocity = graph.ReachStopVelocity;
            time += Time.deltaTime * speed;
        }

        _isRunning = false;
    }


    IEnumerator RefreshingGraphPositionsBackground(Graph graph)
    {
        _isRunning = true;

        float time = 0f;
        float speed = 1f / 15f;

        while (_isRunning && time < 1f)
        {
            if(_newNodeSimuDatas != null)
            {
                _nodeSimuDatas = _newNodeSimuDatas;
                _newNodeSimuDatas = null;
            }

            graph.RefreshTransformPositionsBackground(_nodeSimuDatas);
            yield return null;

            time += Time.deltaTime * speed;
        }

        _isRunning = false;
    }

    private void CalculatingPositionsBackground(object state)
    {
        DebugChrono.Instance.Start("firstTickGRaph");
        var nodgesSimuDatas = (NodgesSimuData) state;
        var reachStopVelocity = false;
        bool firstTick = true;
        float timer = 0f;
        _isRunning = true;

        while (_isRunning && !reachStopVelocity)
        {
            DebugChrono.Instance.Start("tickGRaph");
            reachStopVelocity = CalculateNodeSimuData(nodgesSimuDatas);

            if(firstTick)
            {
                firstTick = false;
                var durationB = DebugChrono.Instance.Stop("firstTickGRaph", true);
                _refreshDurationBackground = durationB * 3f;
                DebugChrono.Instance.Start("tickGRaph");
                continue;
            }

            var duration = DebugChrono.Instance.Stop("tickGRaph", false);
            timer += duration;

            if(timer > .0001f)
            {
                _newNodeSimuDatas = nodgesSimuDatas.NodeSimuDatas.Clone();
                timer = 0f;
            }


            if (_cancellationTokenS.IsCancellationRequested)
            {
                _cancellationTokenS = null;
                return;
            }
        }

    }

    private bool CalculateNodeSimuData(NodgesSimuData nodgesSimuData)
    {
        float coulombDistance;
        float springDistance;
        float stopVelocity;

        var nodesSimuData = nodgesSimuData.NodeSimuDatas;
        var edgesSimuData = nodgesSimuData.EdgeSimuDatas;


        var config = _graph.Configuration;

        if (nodesSimuData.Count > 500)
        {
            coulombDistance = config.BigSpringDistance;
            springDistance = config.BigSpringDistance;
            stopVelocity = config.BigStopVelocity;
        }
        else
        {
            coulombDistance = config.CoulombDistance;
            springDistance = config.SpringDistance;
            stopVelocity = config.StopVelocity;
        }

        float coulombForce = config.CoulombForce;
        float springForce = config.SpringForce;
        float damping = config.Damping;
        float maxVelocity = config.MaxVelocity;
        float invMaxVelocity = 1f / config.MaxVelocity;

        float velocitySum = 0f;


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
            nodeData.Position += nodeData.Velocity * _tickDeltaTime;
        }

        float velocityGraph = velocitySum / (float)nodesSimuData.Count;

        return velocityGraph < stopVelocity;
    }

    private void OnDisable()
    {
        _isRunning = false;
    }

    /*
    Multiple tick per frame
    float timeElapsed = 0f;
        bool expandGraph = true;

        while (expandGraph && timeElapsed < .005f)
        {
            var startTime = DateTime.Now;
            expandGraph = CalculatePositions();
            timeElapsed += (float)(DateTime.Now - startTime).TotalSeconds;
        }

        return expandGraph;



    */
}


public static class NodeSimuDatasExtensions
{
    public static Dictionary<int, NodeSimuData> Clone(this Dictionary<int, NodeSimuData> nodeSimuDatas)
    {
        Dictionary<int, NodeSimuData> cloned = new();

        foreach (var idAndData in nodeSimuDatas)
        {
            cloned.Add(idAndData.Key, idAndData.Value.Clone());
        }


        return cloned;
    }
}
