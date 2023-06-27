using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickPC : MonoBehaviour
{
    public bool AllowHover = false;

    [SerializeField]
    ReferenceHolderSO _referenceHolderSO;


    NodeStyler _hoveredNodgeStyler;

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
            if (_hoveredNodgeStyler != null)
                _hoveredNodgeStyler.OnExitHover(null);
            return;
        }

        if (!hit.transform.TryGetComponent<NodeStyler>(out var nodeStyler))
        {
            if (_hoveredNodgeStyler != null)
                _hoveredNodgeStyler.OnExitHover(null);
            return;
        }

        if (_hoveredNodgeStyler != nodeStyler && _hoveredNodgeStyler != null)
            _hoveredNodgeStyler.OnExitHover(null);

        _hoveredNodgeStyler = nodeStyler;
        _hoveredNodgeStyler.OnEnterHover(null);
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

        Debug.Log("raycast");

        if (!Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            
            //_nodgeSelectionManager.TryClearSelection();
            return;
        }

        Debug.Log("Hit");

        if (!hit.transform.TryGetComponent<NodeStyler>(out var nodeStyler))
            return;

        if (nodeStyler.IsSelected)
            nodeStyler.OnSelectExit(null);
        else
            nodeStyler.OnSelectEnter(null);
    }
}
