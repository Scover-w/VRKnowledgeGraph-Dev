using UnityEngine;
using UnityEngine.EventSystems;

public class ClickPC : MonoBehaviour
{
    public bool AllowHover = false;

    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;

    [SerializeField]
    NodgeSelectionManager _selectionManager;

    Node _hoveredNode;

    Transform _camTf;
    Camera _cam;



    private void Start()
    {
        _cam = _referenceHolderSO.HMDCamSA.Value;
        _camTf = _cam.transform;
    }

    // Update is called once per frame
    void Update()
    {
        TryRaycastNode();

        if (AllowHover)
            TryHoverNode();
    }


    private void TryHoverNode()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layerMask = (1 << Layers.Node);

        if (!Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            if (_hoveredNode != null)
                _hoveredNode.OnHover();
            return;
        }

        if (!hit.transform.TryGetComponent<NodeStyler>(out var nodeStyler))
        {
            if (_hoveredNode != null)
                _hoveredNode.OnHover();
            return;
        }

        var node = nodeStyler.Node;

        if (_hoveredNode != node && _hoveredNode != null)
            _hoveredNode.OnHover();

        _hoveredNode = node;
        _hoveredNode.OnHover();
    }

    private void TryRaycastNode()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        // Check if pointer is over UI (function name misleading)
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layerMask = (1 << Layers.Node);

        if (!Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            _selectionManager.TryClearSelectionFromEmptyUserClick();
            return;
        }

        if (!hit.transform.TryGetComponent<NodeStyler>(out var nodeStyler))
            return;

        nodeStyler.Node.OnSelect();
    }
}
