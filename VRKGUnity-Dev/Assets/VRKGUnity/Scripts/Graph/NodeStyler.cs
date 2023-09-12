using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Wave.Essence.Hand.NearInteraction;

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

    bool _isMoving = false;

    MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        _renderer.SetPropertyBlock(_propertyBlock);
    }


    public void TryForceUnselect()
    {
        // if XR Simple Interactable has select it
        // TODO : force XR Simple Interactable to unselect it
    }

    #region INTERACTION
    public void OnHover()
    {
        Node.OnHover();
    }

    public void OnSelect()
    {
        Node.OnSelect();
    }

    public void OnStartMoving()
    {
        _isMoving = true;
        StartCoroutine(MovingNode());
    }

    public void OnStopMoving()
    {
        _isMoving = false;
    }

    public void UpdateMaterial(bool isHovered, bool isSelected, bool isInPropagation)
    {
        _outliner.UpdateInteraction(isHovered, isSelected, isInPropagation, GraphType == GraphType.Main);
    }

    IEnumerator MovingNode()
    {
        while(_isMoving)
        {
            yield return null;
            Node.MoveEdgeWithNode(_tf.position, GraphType == GraphType.Main);
        }
    }

    #endregion


    #region STYLING
    public void StyleNodeBeforeFirstSimu(StyleChange styleChange, GraphMode graphMode)
    {
        StyleNode(styleChange, graphMode);
    }

    public void StyleNode(StyleChange styleChange, GraphMode graphMode, bool inSimulation = false)
    {
        if (GraphType == GraphType.Main && !styleChange.HasChanged(StyleChange.MainGraph))
            return;

        if (GraphType == GraphType.Sub && !styleChange.HasChanged(StyleChange.SubGraph))
            return;


        if (styleChange.HasChanged(StyleChange.Color))
            StyleColor();

        if (styleChange.HasChanged(StyleChange.Size))
            StyleSize(styleChange, graphMode);

        if (styleChange.HasChanged(StyleChange.Position))
        {
            if (!inSimulation)
                StylePosition(styleChange, graphMode);
        }
    }

    private void StyleColor()
    {
        var selectedMetricType = GraphConfiguration.SelectedMetricTypeColor;

        if (selectedMetricType == GraphMetricType.None)
        {
            SetColor(GraphConfiguration.NodeColor);
            return;
        }


        if (selectedMetricType == GraphMetricType.Ontology)
            StyleColorOnOntology();
        else
            StyleColorOnMetrics(selectedMetricType);
    }


    private void StyleColorOnMetrics(GraphMetricType selectedMetricType)
    {
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
            case GraphMetricType.LocalClusteringCoefficient:
                value = Node.ClusteringCoefficient;
                break;
            case GraphMetricType.Degree:
                value = Node.Degree;
                break;
        }


        SetColor((value == -1f)? GraphConfiguration.NodeColorNoValueMetric :
                                 GraphConfiguration.NodeColorMapping.Lerp(value));
    }

    private void StyleColorOnOntology()
    {
        if(Node.OntoNodeGroup == null)
        {
            SetColor(GraphConfiguration.NodeColorNoValueMetric);
            return;
        }

        float hue = Node.OntoNodeGroup.ColorValue;

        Color color = Color.HSVToRGB(hue, GraphConfiguration.SaturationOntologyColor, GraphConfiguration.ValueOntologyColor);
        SetColor(color);
    }


    private void SetColor(Color newColor)
    {
        newColor.a = Node.IsPropagated ? GraphConfiguration.AlphaNodeColorPropagated : GraphConfiguration.AlphaNodeColorUnPropagated;

        _propertyBlock.SetColor("_Color", newColor);
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    private void StyleSize(StyleChange styleChange, GraphMode graphMode)
    {
        var selectedMetricType = GraphConfiguration.SelectedMetricTypeSize;

        if (selectedMetricType == GraphMetricType.None)
        {
            StyleNormalSize(styleChange, graphMode);
            return;
        }

        StyleMetricSize(selectedMetricType, styleChange, graphMode);
    }

    private void StyleNormalSize(StyleChange styleChange, GraphMode graphMode)
    {
        if (graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChange.DeskMode))
        {
            if (GraphType == GraphType.Main)
                SetScale(GraphConfiguration.EffectiveNodeSizeDesk);

            if (GraphType == GraphType.Sub)
                SetScale(GraphConfiguration.EffectiveNodeSizeLens);
        }

        if (graphMode == GraphMode.Immersion && styleChange.HasChanged(StyleChange.ImmersionMode))
        {
            if (GraphType == GraphType.Main)
                SetScale(GraphConfiguration.EffectiveNodeSizeImmersion);

            if (GraphType == GraphType.Sub)
                SetScale(GraphConfiguration.EffectiveNodeSizeGPS);
        }
    }

    private void StyleMetricSize(GraphMetricType selectedMetricType, StyleChange styleChange, GraphMode graphMode)
    {
        float tScale = 0f;

        switch (selectedMetricType)
        {
            case GraphMetricType.AverageShortestPath:
                tScale = Node.AverageShortestPathLength;
                break;
            case GraphMetricType.BetweennessCentrality:
                tScale = Node.BetweennessCentrality;
                break;
            case GraphMetricType.ClosenessCentrality:
                tScale = Node.ClosenessCentrality;
                break;
            case GraphMetricType.LocalClusteringCoefficient:
                tScale = Node.ClusteringCoefficient;
                break;
            case GraphMetricType.Degree:
                tScale = Node.Degree;
                break;
        }


        if(graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChange.DeskMode))
        {
            if (GraphType == GraphType.Main)
                SetScale(GraphConfiguration.EffectiveNodeSizeDesk, GraphConfiguration.NodeMinMaxSizeDesk, tScale);
            else if(GraphType == GraphType.Sub)
                SetScale(GraphConfiguration.EffectiveNodeSizeLens, GraphConfiguration.NodeMinMaxSizeLens, tScale);

            return;
        }

        if (graphMode == GraphMode.Immersion && styleChange.HasChanged(StyleChange.ImmersionMode))
        {
            if (GraphType == GraphType.Main)
                SetScale(GraphConfiguration.EffectiveNodeSizeImmersion, GraphConfiguration.NodeMinMaxSizeImmersion, tScale);
            else if (GraphType == GraphType.Sub)
                SetScale(GraphConfiguration.EffectiveNodeSizeGPS);

            return;
        }    
    }

    private void StylePosition(StyleChange styleChange, GraphMode graphMode)
    {
        if (graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChange.DeskMode))
        {
            if (GraphType == GraphType.Main)
                SetPosition(GraphConfiguration.EffectiveDeskGraphSize);
            else if(GraphType == GraphType.Sub)
                SetPosition(GraphConfiguration.EffectiveLensGraphSize);
        }


        if (graphMode == GraphMode.Immersion && styleChange.HasChanged(StyleChange.ImmersionMode))
        {
            if (GraphType == GraphType.Main)
                SetPosition(GraphConfiguration.EffectiveImmersionGraphSize);
            else if (GraphType == GraphType.Sub)
                SetPosition(GraphConfiguration.EffectiveGPSGraphSize);
        }

    }


    public void StyleTransitionNode(float t, bool isNextDesk)
    {
        if (GraphType == GraphType.Sub)
            return;

        StyleTransitionPosition(t, isNextDesk);
        StyleTransitionSize(t, isNextDesk);
    }


    private void StyleTransitionPosition(float t, bool isNextDesk)
    {
        float scale = isNextDesk ? Mathf.Lerp(GraphConfiguration.EffectiveImmersionGraphSize, GraphConfiguration.EffectiveDeskGraphSize, t) :
                                    Mathf.Lerp(GraphConfiguration.EffectiveDeskGraphSize, GraphConfiguration.EffectiveImmersionGraphSize, t);
        SetPosition(scale);
    }

    private void StyleTransitionSize(float t, bool isNextDesk)
    {
        var selectedMetricType = GraphConfiguration.SelectedMetricTypeSize;

        if (selectedMetricType == GraphMetricType.None)
        {
            StyleTransitionNormalSize(t, isNextDesk);
            return;
        }

        StyleTransitionMetricSize(t, isNextDesk);
    }

    private void StyleTransitionNormalSize(float t, bool isNextDesk)
    {
        float scale = isNextDesk ? Mathf.Lerp(GraphConfiguration.EffectiveNodeSizeImmersion, GraphConfiguration.EffectiveNodeSizeDesk, t) :
                                    Mathf.Lerp(GraphConfiguration.EffectiveNodeSizeDesk, GraphConfiguration.EffectiveNodeSizeImmersion, t);
        SetScale(scale);
    }

    private void StyleTransitionMetricSize(float t, bool isNextDesk)
    {
        var selectedMetricType = GraphConfiguration.SelectedMetricTypeSize;
        float tScale = 0f;

        switch (selectedMetricType)
        {
            case GraphMetricType.AverageShortestPath:
                tScale = Node.AverageShortestPathLength;
                break;
            case GraphMetricType.BetweennessCentrality:
                tScale = Node.BetweennessCentrality;
                break;
            case GraphMetricType.ClosenessCentrality:
                tScale = Node.ClosenessCentrality;
                break;
            case GraphMetricType.LocalClusteringCoefficient:
                tScale = Node.ClusteringCoefficient;
                break;
            case GraphMetricType.Degree:
                tScale = Node.Degree;
                break;
        }


        float nodeSize = isNextDesk ? Mathf.Lerp(GraphConfiguration.EffectiveNodeSizeImmersion, GraphConfiguration.EffectiveNodeSizeDesk, t) :
                                    Mathf.Lerp(GraphConfiguration.EffectiveNodeSizeDesk, GraphConfiguration.EffectiveNodeSizeImmersion, t);

        float minMaxScale = isNextDesk ? Mathf.Lerp(GraphConfiguration.NodeMinMaxSizeImmersion, GraphConfiguration.NodeMinMaxSizeDesk, t) :
                                    Mathf.Lerp(GraphConfiguration.NodeMinMaxSizeDesk, GraphConfiguration.NodeMinMaxSizeImmersion, t);

        SetScale(nodeSize, minMaxScale, tScale);
    }
    #endregion


    private void SetScale(float nodeSize,float nodeMinMax, float tScale)
    {
        // nodeMinMax is between 0 and 1
        float max = nodeSize * (nodeMinMax + 1f);
        float min = nodeSize * nodeMinMax;
        float scale = Mathf.Lerp(min, max, tScale);
        _tf.localScale = new Vector3(scale, scale, scale);
    }

    private void SetScale(float scale)
    {
        _tf.localScale = new Vector3(scale, scale, scale);
    }

    private void SetPosition(float scale)
    {
        _tf.localPosition = Node.AbsolutePosition * scale;
    }

    #region EDITOR
    [ContextMenu("OnHover")]
    private void OnHoverEditor()
    {
        OnHover();
    }


    [ContextMenu("OnSelect")]
    private void OnSelectEditor()
    {
        OnSelect();
    }
    #endregion
}