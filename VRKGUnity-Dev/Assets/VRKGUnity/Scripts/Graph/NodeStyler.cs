using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Wave.Essence.Hand.NearInteraction;

public class NodeStyler : MonoBehaviour
{
    public Transform Tf { get { return _tf; } }

    public Node Node { get; set; }

    public GraphType GraphType { private get; set; }

    public bool IsSelected { get {  return _isSelected; } }

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

    bool _isHovered = false;
    bool _isSelected = false;
    bool _isInPropagation = false;


    private void OnEnable()
    {
        _isHovered = false;
        _isSelected = false;
        _outliner.enabled = false;


        _propertyBlock = new MaterialPropertyBlock();
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    public void SetPropagation(bool isInPropagation)
    {
        _isInPropagation = isInPropagation;
        UpdateMaterial();
    }

    public void UnSelect()
    {
        if (!_isSelected)
            return;

        _isSelected = false;
        // TODO : force XR Simple Interactable to unselect it
        UpdateMaterial();
    }

    #region INTERACTION
    public void OnEnterHover(HoverEnterEventArgs args)
    {
        if (_isHovered)
            return;

        _isHovered = true;
        UpdateMaterial();
    }

    public void OnExitHover(HoverExitEventArgs args)
    {
        if(!_isHovered) 
            return;

        _isHovered = false;
        UpdateMaterial();
    }

    public void OnSelectEnter(SelectEnterEventArgs args)
    {
        if (_isSelected)
            return;

        _isSelected = true;
        UpdateMaterial();

        var nodgeSelection = NodgeSelectionManager.Instance;

        if(nodgeSelection == null)
        {
            Debug.LogError("No NodgeSelectionManager in the scene");
            return;
        }

        nodgeSelection.Select(Node);
    }

    public void OnSelectExit(SelectExitEventArgs args)
    {
        if(!_isSelected)
            return;


        _isSelected = false;
        UpdateMaterial();

        var nodgeSelection = NodgeSelectionManager.Instance;

        if (nodgeSelection == null)
        {
            Debug.LogError("No NodgeSelectionManager in the scene");
            return;
        }

        nodgeSelection.UnSelect(Node);
    }

    private void UpdateMaterial()
    {
        _outliner.UpdateInteraction(_isHovered, _isSelected, _isInPropagation);
    }

    #endregion


    #region STYLING
    public void StyleNodeForFirstTime()
    {
        _renderer.material.color = GraphConfiguration.NodeColor;

        float scale = (GraphType == GraphType.Main) ? GraphConfiguration.NodeSizeDesk
                                                    : GraphConfiguration.NodeSizeImmersion;
        _tf.localScale = new Vector3(scale, scale, scale);
    }

    public void StyleNode(StyleChange styleChange, bool inSimulation)
    {
        if(styleChange.HasChanged(StyleChangeType.Color))
            StyleColor();

        if (styleChange.HasChanged(StyleChangeType.Size))
            StyleSize();

        if (styleChange.HasChanged(StyleChangeType.Position))
        {
            if (!inSimulation)
                StylePosition();
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


    private void StyleSize()
    {
        var selectedMetricType = GraphConfiguration.SelectedMetricTypeSize;

        if (selectedMetricType == GraphMetricType.None)
        {
            float scale = (GraphType == GraphType.Main)? GraphConfiguration.NodeSizeDesk
                                                        : GraphConfiguration.NodeSizeImmersion;     
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

        if(GraphType == GraphType.Main)
        {
            nodeMinSize = GraphConfiguration.NodeMinSizeDesk;
            nodeMaxSize = GraphConfiguration.NodeMinSizeDesk;
        }
        else
        {
            nodeMinSize = GraphConfiguration.NodeMinSizeImmersion;
            nodeMaxSize = GraphConfiguration.NodeMinSizeImmersion;
        }

        scaleB = Mathf.Lerp(nodeMinSize, nodeMaxSize, scaleB);

        _tf.localScale = new Vector3(scaleB, scaleB, scaleB);
    }

    private void StylePosition()
    {
        float scalingFactor = (GraphType == GraphType.Main) ? GraphConfiguration.ImmersionGraphSize
                                                    : GraphConfiguration.GPSGraphSize;

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