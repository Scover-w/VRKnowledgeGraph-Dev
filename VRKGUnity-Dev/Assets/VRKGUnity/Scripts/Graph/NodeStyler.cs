using System;
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

    [SerializeField]
    XRSimpleInteractable _interactable;

    MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    //private void OnEnable()
    //{
    //    _propertyBlock = new MaterialPropertyBlock();
    //    _renderer.SetPropertyBlock(_propertyBlock);
    //}


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


    public void UpdateMaterial(bool isHovered, bool isSelected, bool isInPropagation)
    {
        _outliner.UpdateInteraction(isHovered, isSelected, isInPropagation, GraphType == GraphType.Main);
    }

    #endregion


    #region STYLING
    public void StyleNodeBeforeFirstSimu(StyleChange styleChange, GraphMode graphMode)
    {
        StyleNode(styleChange, graphMode);
    }

    public void StyleNode(StyleChange styleChange, GraphMode graphMode, bool inSimulation = false)
    {
        if (GraphType == GraphType.Main && !styleChange.HasChanged(StyleChangeType.MainGraph))
            return;

        if (GraphType == GraphType.Sub && !styleChange.HasChanged(StyleChangeType.SubGraph))
            return;


        if (styleChange.HasChanged(StyleChangeType.Color))
            StyleColor();

        if (styleChange.HasChanged(StyleChangeType.Size))
            StyleSize(styleChange, graphMode);

        if (styleChange.HasChanged(StyleChangeType.Position))
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
            _propertyBlock.SetColor("_Color", GraphConfiguration.NodeColor);
            _renderer.SetPropertyBlock(_propertyBlock);
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
            case GraphMetricType.ClusteringCoefficient:
                value = Node.ClusteringCoefficient;
                break;
            case GraphMetricType.Degree:
                value = Node.Degree;
                break;
        }

        if(value == -1f)
            _propertyBlock.SetColor("_Color", GraphConfiguration.NodeColorNoOntology);
        else
            _propertyBlock.SetColor("_Color", GraphConfiguration.NodeColorMapping.Lerp(value));

        _renderer.SetPropertyBlock(_propertyBlock);
    }

    private void StyleColorOnOntology()
    {
        if(Node.OntoNodeGroup == null)
        {
            _propertyBlock.SetColor("_Color", GraphConfiguration.NodeColorNoOntology);
            _renderer.SetPropertyBlock(_propertyBlock);
            return;
        }

        float hue = Node.OntoNodeGroup.ColorValue;

        Color color = Color.HSVToRGB(hue, GraphConfiguration.SaturationOntologyColor, GraphConfiguration.ValueOntologyColor);
        _propertyBlock.SetColor("_Color", color);
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
        if (graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChangeType.DeskMode))
        {
            if (GraphType == GraphType.Main)
                SetScale(GraphConfiguration.NodeSizeDesk);

            if (GraphType == GraphType.Sub)
                SetScale(GraphConfiguration.NodeSizeLens);
        }

        if (graphMode == GraphMode.Immersion && styleChange.HasChanged(StyleChangeType.ImmersionMode))
        {
            if (GraphType == GraphType.Main)
                SetScale(GraphConfiguration.NodeSizeImmersion);

            if (GraphType == GraphType.Sub)
                SetScale(GraphConfiguration.NodeSizeWatch);
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
            case GraphMetricType.ClusteringCoefficient:
                tScale = Node.ClusteringCoefficient;
                break;
            case GraphMetricType.Degree:
                tScale = Node.Degree;
                break;
        }


        if(graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChangeType.DeskMode))
        {
            if (GraphType == GraphType.Main)
                SetScale(GraphConfiguration.NodeMinSizeDesk, GraphConfiguration.NodeMaxSizeDesk, tScale);
            else if(GraphType == GraphType.Sub)
                SetScale(GraphConfiguration.NodeMinSizeLens, GraphConfiguration.NodeMaxSizeLens, tScale);

            return;
        }

        if (graphMode == GraphMode.Immersion && styleChange.HasChanged(StyleChangeType.ImmersionMode))
        {
            if (GraphType == GraphType.Main)
                SetScale(GraphConfiguration.NodeMinSizeImmersion, GraphConfiguration.NodeMaxSizeImmersion, tScale);
            else if (GraphType == GraphType.Sub)
                SetScale(GraphConfiguration.NodeSizeWatch);

            return;
        }    
    }

    private void StylePosition(StyleChange styleChange, GraphMode graphMode)
    {
        if (graphMode == GraphMode.Desk && styleChange.HasChanged(StyleChangeType.DeskMode))
        {
            if (GraphType == GraphType.Main)
                SetPosition(GraphConfiguration.DeskGraphSize);
            else if(GraphType == GraphType.Sub)
                SetPosition(GraphConfiguration.LensGraphSize);
        }


        if (graphMode == GraphMode.Immersion && styleChange.HasChanged(StyleChangeType.ImmersionMode))
        {
            if (GraphType == GraphType.Main)
                SetPosition(GraphConfiguration.ImmersionGraphSize);
            else if (GraphType == GraphType.Sub)
                SetPosition(GraphConfiguration.WatchGraphSize);
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
        float scale = isNextDesk ? Mathf.Lerp(GraphConfiguration.ImmersionGraphSize, GraphConfiguration.DeskGraphSize, t) :
                                    Mathf.Lerp(GraphConfiguration.DeskGraphSize, GraphConfiguration.ImmersionGraphSize, t);
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
        float scale = isNextDesk ? Mathf.Lerp(GraphConfiguration.NodeSizeImmersion, GraphConfiguration.NodeSizeDesk, t) :
                                    Mathf.Lerp(GraphConfiguration.NodeSizeDesk, GraphConfiguration.NodeSizeImmersion, t);
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
            case GraphMetricType.ClusteringCoefficient:
                tScale = Node.ClusteringCoefficient;
                break;
            case GraphMetricType.Degree:
                tScale = Node.Degree;
                break;
        }


        float minScale = isNextDesk ? Mathf.Lerp(GraphConfiguration.NodeMinSizeImmersion, GraphConfiguration.NodeMinSizeDesk, t) :
                                    Mathf.Lerp(GraphConfiguration.NodeMinSizeDesk, GraphConfiguration.NodeMinSizeImmersion, t);

        float maxScale = isNextDesk ? Mathf.Lerp(GraphConfiguration.NodeMaxSizeImmersion, GraphConfiguration.NodeMaxSizeDesk, t) :
                                    Mathf.Lerp(GraphConfiguration.NodeMaxSizeDesk, GraphConfiguration.NodeMaxSizeImmersion, t);

        SetScale(minScale, maxScale, tScale);
    }
    #endregion


    private void SetScale(float nodeMinSize,float nodeMaxSize, float tScale)
    {
        float scale = Mathf.Lerp(nodeMinSize, nodeMaxSize, tScale);
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