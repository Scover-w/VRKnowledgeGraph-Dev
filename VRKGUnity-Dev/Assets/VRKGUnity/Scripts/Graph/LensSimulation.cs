using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using UnityEngine;

public class LensSimulation : MonoBehaviour
{
    GraphConfiguration _graphConfiguration;

    Dictionary<int, NodeSimuData2D> _nodeSimuDatas;
    Dictionary<int, NodeSimuData2D> _newNodeSimuDatas;

    Dictionary<int, Node> _displayedNodesDicId;
    Dictionary<int, Edge> _displayedEdgesDicId;



    bool _isRunningSimulation;
    bool _wantStopSimulation;
    float _refreshDuration;

    private void Start()
    {
        _graphConfiguration = GraphConfiguration.Instance;
    }

    public void Run(Dictionary<int, Node> nodeAndIds, Dictionary<int,Edge> edgeAndIds)
    {
        if(_isRunningSimulation) 
        {
            _wantStopSimulation = true;
            StartCoroutine(WaitingToRun(nodeAndIds, edgeAndIds));
            return;
        }

        StartRun(nodeAndIds, edgeAndIds);
    }

    IEnumerator WaitingToRun(Dictionary<int, Node> nodeAndIds, Dictionary<int, Edge> edgeAndIds)
    {
        while( _isRunningSimulation) 
        {
            yield return null;
        }

        StartRun(nodeAndIds, edgeAndIds);
    }

    private void StartRun(Dictionary<int, Node> nodeAndIds, Dictionary<int, Edge> edgeAndIds)
    {
        _refreshDuration = -1f;
        _newNodeSimuDatas = null;
        _wantStopSimulation = false;


        _displayedNodesDicId = nodeAndIds;
        _displayedEdgesDicId = edgeAndIds;

        _nodeSimuDatas = nodeAndIds.ToSimuDatas2D();
        var edgeSimuData = edgeAndIds.ToSimuDatas();
        var simuData2D = new NodgesSimuData2D(_nodeSimuDatas, edgeSimuData);

        CalculateStartPositions(_nodeSimuDatas);

        ThreadPool.QueueUserWorkItem(CalculatingPositionsBackground, simuData2D);
        StartCoroutine(RefreshingNodesPositions());
    }

    private void CalculateStartPositions(Dictionary<int, NodeSimuData2D> nodesSimuData2D)
    {
        Vector2 center = Vector2.zero;


        foreach (NodeSimuData2D nodeSimuData2D in nodesSimuData2D.Values)
        {
            center += nodeSimuData2D.Position;
        }


        center /= nodesSimuData2D.Count;

        var scalingFactor = _graphConfiguration.LensGraphSize;

        foreach (NodeSimuData2D nodeSimuData2D in nodesSimuData2D.Values)
        {
            nodeSimuData2D.Position -= center;


            if (!_displayedNodesDicId.TryGetValue(nodeSimuData2D.Id, out Node node))
                continue;

            var subTf = node.SubGraphNodeTf;
            subTf.localPosition = nodeSimuData2D.Position * scalingFactor;

        }
    }


    private void CalculatingPositionsBackground(object state)
    {
        DebugChrono.Instance.Start("firstTickGRaph");

        var nodgesSimuDatas = (NodgesSimuData2D)state;
        var hasReachStopVelocity = false;
        bool firstTick = true;

        float timer = 0f;
        var lastTimer = DateTime.Now;

        _isRunningSimulation = true;

        while (_isRunningSimulation && ( !(hasReachStopVelocity && timer > 1f) ) && !_wantStopSimulation)
        {
            hasReachStopVelocity = CalculateNodeSimuData(nodgesSimuDatas);

            if (firstTick)
            {
                firstTick = false;
                var durationB = DebugChrono.Instance.Stop("firstTickGRaph", true);
                _refreshDuration = durationB * 3f;
                continue;
            }

            _newNodeSimuDatas = nodgesSimuDatas.NodeSimuDatas.Clone();

            timer += (float)(DateTime.Now - lastTimer).TotalSeconds;

            lastTimer = DateTime.Now;
        }


        _newNodeSimuDatas = nodgesSimuDatas.NodeSimuDatas.Clone();
        _isRunningSimulation = false;
    }



    private bool CalculateNodeSimuData(NodgesSimuData2D simuData2D)
    {
        float coulombDistance;
        float springDistance;
        float stopVelocity;

        var nodesSimuData = simuData2D.NodeSimuDatas;
        var edgesSimuData = simuData2D.EdgeSimuDatas;


        var config = _graphConfiguration.LensSimuParameters;

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
            nodeDataA.Velocity = Vector2.zero;

            foreach (var idAndNodeDataB in nodesSimuData)
            {
                var nodeDataB = idAndNodeDataB.Value;
                if (nodeDataA.Id == nodeDataB.Id)
                    continue;

                Vector2 direction = nodeDataB.Position - nodeDataA.Position;
                float distance = direction.magnitude;

                if (distance > coulombDistance)
                    continue;

                float repulsiveForce = coulombForce * (1f / distance);

                Vector2 forceVector = repulsiveForce * direction.normalized;
                nodeDataA.Velocity -= forceVector;
            }
        }

        // Spring force - Attractive Force between connected nodes
        foreach (var idAndEdgeData in edgesSimuData)
        {
            var edge = idAndEdgeData.Value;

            if (!nodesSimuData.TryGetValue(edge.IdA, out NodeSimuData2D nodeDataA))
                continue;

            if (!nodesSimuData.TryGetValue(edge.IdB, out NodeSimuData2D nodeDataB))
                continue;



            Vector2 direction = nodeDataB.Position - nodeDataA.Position;

            float distance = direction.magnitude;

            if (distance < springDistance)
                continue;

            float attractiveForce = (distance - springDistance) * springForce;
            Vector2 forceVector = attractiveForce * direction.normalized;
            nodeDataA.Velocity += forceVector;
            nodeDataB.Velocity -= forceVector;
        }


        // Apply velocity
        foreach (var idAndNodeData in nodesSimuData)
        {
            var nodeData = idAndNodeData.Value;
            nodeData.Velocity *= damping;
            velocitySum += nodeData.Velocity.magnitude;

            if (nodeData.Velocity.magnitude > maxVelocity)
                nodeData.Velocity /= nodeData.Velocity.magnitude * invMaxVelocity;

            nodeData.Position += nodeData.Velocity * tickDeltaTime;
        }

        float velocityGraph = velocitySum / (float)nodesSimuData.Count;

        return velocityGraph < stopVelocity;
    }

    IEnumerator RefreshingNodesPositions()
    {
        _isRunningSimulation = true;

        float time = 0f;
        float speed = 1f / _graphConfiguration.LensSimuParameters.MaxSimulationTime;

        while (_isRunningSimulation && time < 1f && !_wantStopSimulation)
        {
            if (_newNodeSimuDatas != null)
            {
                _nodeSimuDatas = _newNodeSimuDatas;
                _newNodeSimuDatas = null;

                
            }

            RefreshLensNodePositions();

            yield return null;


            time += Time.deltaTime * speed;
        }

        RefreshLensNodePositions();

        _wantStopSimulation = true;
    }

    private void RefreshLensNodePositions()
    {
        var scalingFactor = _graphConfiguration.LensGraphSize;
        var lerpSmooth = _graphConfiguration.LensSimuParameters.LerpSmooth;

        foreach (var idAnData in _nodeSimuDatas)
        {
            if (!_displayedNodesDicId.TryGetValue(idAnData.Key, out Node node))
                continue;

            var subTf = node.SubGraphNodeTf;

            var newCalculatedPosition = idAnData.Value.Position;
            var subLerpPosition = Vector3.Lerp(subTf.localPosition, newCalculatedPosition * scalingFactor, lerpSmooth);
            subTf.localPosition = subLerpPosition;
        }

        foreach (var idAndEdge in _displayedEdgesDicId)
        {
            var edge = idAndEdge.Value;

            var sourcePos = edge.Source.SubGraphNodeTf.localPosition;
            var targetPos = edge.Target.SubGraphNodeTf.localPosition;

            Vector3 direction = targetPos - sourcePos;

            var subLine = edge.SubGraphLine;
            subLine.SetPosition(0, sourcePos);
            subLine.SetPosition(1, sourcePos + direction * .2f);
            subLine.SetPosition(2, sourcePos + direction * .8f);
            subLine.SetPosition(3, targetPos);
        }
    }

}