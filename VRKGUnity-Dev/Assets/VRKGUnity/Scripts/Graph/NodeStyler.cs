using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class NodeStyler : MonoBehaviour
{
    public Transform Tf { get { return _tf; } }

    public Node Node { get; set; }

    public GraphType GraphType { private get; set; }

    public static GraphConfiguration GraphConfiguration;

    [SerializeField]
    Transform _tf;

    [SerializeField]
    MeshRenderer _renderer;

    [SerializeField]
    Outliner _outliner;

    MaterialPropertyBlock _propertyBlock;

    bool _isHovered = false;
    bool _isSelected = false;


    private void OnEnable()
    {
        _isHovered = false;
        _isSelected = false;
        _outliner.enabled = false;


        _propertyBlock = new MaterialPropertyBlock();
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    #region INTERACTION
    public void OnEnterHover(HoverEnterEventArgs args)
    {
        _isHovered = true;
        UpdateMaterial();
    }

    public void OnExitHover(HoverExitEventArgs args)
    {
        _isHovered = false;
        UpdateMaterial();
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        _isSelected = true;
        UpdateMaterial();
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        _isSelected = false;
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        _outliner.UpdateInteraction(_isHovered, _isSelected);
    }

    #endregion


    #region STYLING
    public void StyleNodeForFirstTime()
    {
        _renderer.material.color = GraphConfiguration.NodeColor;

        float scale = (GraphType == GraphType.Mega) ? GraphConfiguration.NodeSizeMegaGraph
                                                    : GraphConfiguration.NodeSizeMiniGraph;
        _tf.localScale = new Vector3(scale, scale, scale);
    }

    public void StyleNode(bool inSimulation)
    {
        StyleColor();
        StyleSize();

        if(!inSimulation)
            StylePosition();
    }

    private void StyleColor()
    {
        var selectedMetricType = GraphConfiguration.SelectedMetricTypeColor;
        if (selectedMetricType == GraphMetricType.None)
        {
            _propertyBlock.SetColor("_Color", GraphConfiguration.NodeColor);
            _renderer.SetPropertyBlock(_propertyBlock);
            return;
        }

        float value = 0;

        switch (selectedMetricType)
        {
            case GraphMetricType.AverageShortestPath:
                value = Node.AverageShortestPathLength;
                break;
            case GraphMetricType.BetweennessCentrality:
                value = Node.BetweennessCentrality;
                break;
            case GraphMetricType.ClosenessCentrality:
                value = Node.ClosenessCentrality;
                break;
            case GraphMetricType.ClusteringCoefficient:
                value = Node.ClusteringCoefficient;
                break;
            case GraphMetricType.Degree:
                value = Node.Degree;
                break;
        }


        _propertyBlock.SetColor("_Color", GraphConfiguration.NodeColorMapping.Lerp(value));
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    private void StyleSize()
    {
        var selectedMetricType = GraphConfiguration.SelectedMetricTypeSize;

        if (selectedMetricType == GraphMetricType.None)
        {
            float scale = (GraphType == GraphType.Mega)? GraphConfiguration.NodeSizeMegaGraph
                                                        : GraphConfiguration.NodeSizeMiniGraph;     
            _tf.localScale = new Vector3(scale, scale, scale);
            return;
        }

        float scaleB = 0f;

        switch (selectedMetricType)
        {
            case GraphMetricType.AverageShortestPath:
                scaleB = Node.AverageShortestPathLength;
                break;
            case GraphMetricType.BetweennessCentrality:
                scaleB = Node.BetweennessCentrality;
                break;
            case GraphMetricType.ClosenessCentrality:
                scaleB = Node.ClosenessCentrality;
                break;
            case GraphMetricType.ClusteringCoefficient:
                scaleB = Node.ClusteringCoefficient;
                break;
            case GraphMetricType.Degree:
                scaleB = Node.Degree;
                break;
        }

        float nodeMinSize;
        float nodeMaxSize;

        if(GraphType == GraphType.Mega)
        {
            nodeMinSize = GraphConfiguration.NodeMinSizeMegaGraph;
            nodeMaxSize = GraphConfiguration.NodeMinSizeMegaGraph;
        }
        else
        {
            nodeMinSize = GraphConfiguration.NodeMinSizeMiniGraph;
            nodeMaxSize = GraphConfiguration.NodeMinSizeMiniGraph;
        }

        scaleB = Mathf.Lerp(nodeMinSize, nodeMaxSize, scaleB);

        _tf.localScale = new Vector3(scaleB, scaleB, scaleB);
    }

    private void StylePosition()
    {
        float scalingFactor = (GraphType == GraphType.Mega) ? GraphConfiguration.MegaGraphSize
                                                    : GraphConfiguration.MiniGraphSize;

        _tf.localPosition = Node.AbsolutePosition * scalingFactor;
    }
    #endregion


    #region EDITOR
    [ContextMenu("OnEnterHover")]
    private void OnEnterHoverEditor()
    {
        HoverEnterEventArgs arg = new();
        OnEnterHover(arg);
    }

    [ContextMenu("OnExitHover")]
    private void OnExitHoverEditor()
    {
        HoverExitEventArgs arg = new();
        OnExitHover(arg);
    }

    [ContextMenu("OnSelectEnter")]
    private void OnSelectEnterEditor()
    {
        SelectEnterEventArgs arg = new();
        OnSelectEnter(arg);
    }

    [ContextMenu("OnSelectExit")]
    private void OnSelectExitEditor()
    {
        SelectExitEventArgs arg = new();
        OnSelectExit(arg);
    }
    #endregion
}
