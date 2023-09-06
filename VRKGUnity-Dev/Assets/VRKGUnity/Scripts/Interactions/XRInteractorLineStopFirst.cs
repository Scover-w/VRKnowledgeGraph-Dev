using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRInteractorLineStopFirst : MonoBehaviour
{
    [SerializeField]
    bool _smoothMovement;
    /// <summary>
    /// Controls whether the rendered segments will be delayed from and smoothly follow the target segments.
    /// </summary>
    /// <seealso cref="followTightness"/>
    /// <seealso cref="snapThresholdDistance"/>
    public bool smoothMovement
    {
        get => _smoothMovement;
        set => _smoothMovement = value;
    }

    [SerializeField]
    float _followTightness = 10f;
    /// <summary>
    /// Controls the speed that the rendered segments follow the target segments when Smooth Movement is enabled.
    /// </summary>
    /// <seealso cref="smoothMovement"/>
    /// <seealso cref="snapThresholdDistance"/>
    public float followTightness
    {
        get => _followTightness;
        set => _followTightness = value;
    }

    [SerializeField]
    float _snapThresholdDistance = 10f;
    /// <summary>
    /// Controls the threshold distance between line points at two consecutive frames to snap rendered segments to target segments when Smooth Movement is enabled.
    /// </summary>
    /// <seealso cref="smoothMovement"/>
    /// <seealso cref="followTightness"/>
    public float snapThresholdDistance
    {
        get => _snapThresholdDistance;
        set => _snapThresholdDistance = value;
    }

    [SerializeField]
    bool _stopLineAtFirstRaycastHit = true;
    /// <summary>
    /// Controls whether this behavior always cuts the line short at the first ray cast hit, even when invalid.
    /// </summary>
    /// <remarks>
    /// The line will always stop short at valid targets, even if this property is set to false.
    /// If you wish this line to pass through valid targets, they must be placed on a different layer.
    /// <see langword="true"/> means to do the same even when pointing at an invalid target.
    /// <see langword="false"/> means the line will continue to the configured line length.
    /// </remarks>
    public bool stopLineAtFirstRaycastHit
    {
        get => _stopLineAtFirstRaycastHit;
        set => _stopLineAtFirstRaycastHit = value;
    }

    [SerializeField]
    bool _stopLineAtSelection;
    /// <summary>
    /// Controls whether the line will stop at the attach point of the closest interactable selected by the interactor, if there is one.
    /// </summary>
    public bool stopLineAtSelection
    {
        get => _stopLineAtSelection;
        set => _stopLineAtSelection = value;
    }

    int _endPositionInLine;

    bool _snapCurve = true;
    bool _performSetup;

    LineRenderer _lineRenderer;

    // interface to get target point
    ILineRenderable _lineRenderable;
    IXRSelectInteractor _lineRenderableAsSelectInteractor;
    XRBaseInteractor _lineRenderableAsBaseInteractor;

    // reusable lists of target points
    Vector3[] _targetPoints;
    int _nbTargetPoints = -1;

    // reusable lists of rendered points
    Vector3[] _renderPoints;
    int _nbRenderPoints = -1;

    // reusable lists of rendered points to smooth movement
    Vector3[] _previousRenderPoints;
    int _nbPreviousRenderPoints = -1;

    readonly Vector3[] _clearArray = { Vector3.zero, Vector3.zero };


    Vector3 _hitPosition;
    Vector3 _normalPosition;

    XROrigin _xrOrigin;

    /// <summary>
    /// Cached reference to an <see cref="XROrigin"/> found with <see cref="Object.FindObjectOfType{Type}()"/>.
    /// </summary>
    static XROrigin _xrOriginCache;

    protected void OnValidate()
    {
        if (Application.isPlaying)
            UpdateSettings();
    }

    protected void Awake()
    {
        _lineRenderable = GetComponent<ILineRenderable>();
        if (_lineRenderable != null)
        {
            _lineRenderableAsBaseInteractor = _lineRenderable as XRBaseInteractor;
            _lineRenderableAsSelectInteractor = _lineRenderable as IXRSelectInteractor;
        }

        FindXROrigin();
        ClearLineRenderer();
        UpdateSettings();
    }


    protected void OnEnable()
    {
        _snapCurve = true;
        Application.onBeforeRender += OnBeforeRenderLineVisual;
    }

    protected void OnDisable()
    {
        if (_lineRenderer != null)
            _lineRenderer.enabled = false;

        Application.onBeforeRender -= OnBeforeRenderLineVisual;
    }

    void ClearLineRenderer()
    {
        if (TryFindLineRenderer())
        {
            _lineRenderer.SetPositions(_clearArray);
            _lineRenderer.positionCount = 0;
        }
    }

    [BeforeRenderOrder(XRInteractionUpdateOrder.k_BeforeRenderLineVisual)]
    void OnBeforeRenderLineVisual()
    {
        UpdateLineVisual();
    }

    private void Update()
    {
        UpdateLineVisual();
    }

    internal void UpdateLineVisual()
    {
        if (_performSetup)
        {
            UpdateSettings();
            _performSetup = false;
        }

        if (_lineRenderer == null)
            return;

        if (_lineRenderer.useWorldSpace && _xrOrigin != null)
        {
            // Update line width with user scale
            var xrOrigin = _xrOrigin.Origin;
        }

        if (_lineRenderable == null)
        {
            _lineRenderer.enabled = false;
            return;
        }

        _nbRenderPoints = 0;

        // Get all the line sample points from the ILineRenderable interface
        if (!_lineRenderable.GetLinePoints(ref _targetPoints, out _nbTargetPoints))
        {
            _lineRenderer.enabled = false;
            ClearLineRenderer();
            return;
        }

        // Sanity check.
        if (_targetPoints == null ||
            _targetPoints.Length == 0 ||
            _nbTargetPoints == 0 ||
            _nbTargetPoints > _targetPoints.Length)
        {
            _lineRenderer.enabled = false;
            ClearLineRenderer();
            return;
        }

        // Make sure we have the correct sized arrays for everything.
        if (_renderPoints == null || _renderPoints.Length < _nbTargetPoints)
        {
            _renderPoints = new Vector3[_nbTargetPoints];
            _previousRenderPoints = new Vector3[_nbTargetPoints];
            _nbRenderPoints = 0;
            _nbPreviousRenderPoints = 0;
        }

        // If there is a big movement (snap turn, teleportation), snap the curve
        if (_previousRenderPoints.Length != _nbTargetPoints)
        {
            _snapCurve = true;
        }
        else
        {
            // Compare the two endpoints of the curve, as that will have the largest delta.
            if (_previousRenderPoints != null &&
                _nbPreviousRenderPoints > 0 &&
                _nbPreviousRenderPoints <= _previousRenderPoints.Length &&
                _targetPoints != null &&
                _nbTargetPoints > 0 &&
                _nbTargetPoints <= _targetPoints.Length)
            {
                var prevPointIndex = _nbPreviousRenderPoints - 1;
                var currPointIndex = _nbTargetPoints - 1;
                if (Vector3.Distance(_previousRenderPoints[prevPointIndex], _targetPoints[currPointIndex]) > _snapThresholdDistance)
                {
                    _snapCurve = true;
                }
            }
        }

        // If the line hits, insert reticle position into the list for smoothing.
        // Remove the last point in the list to keep the number of points consistent.
        if (_lineRenderable.TryGetHitInfo(out _hitPosition, out _normalPosition, out _endPositionInLine, out var isValidTarget))
        {
            // End the line at the current hit point.
            if ((isValidTarget || _stopLineAtFirstRaycastHit) && _endPositionInLine > 0 && _endPositionInLine < _nbTargetPoints)
            {
                // The hit position might not lie within the line segment, for example if a sphere cast is used, so use a point projected onto the
                // segment so that the endpoint is continuous with the rest of the curve.
                var lastSegmentStartPoint = _targetPoints[_endPositionInLine - 1];
                var lastSegmentEndPoint = _targetPoints[_endPositionInLine];
                var lastSegment = lastSegmentEndPoint - lastSegmentStartPoint;
                var projectedHitSegment = Vector3.Project(_hitPosition - lastSegmentStartPoint, lastSegment);

                // Don't bend the line backwards
                if (Vector3.Dot(projectedHitSegment, lastSegment) < 0)
                    projectedHitSegment = Vector3.zero;

                _hitPosition = lastSegmentStartPoint + projectedHitSegment;
                _targetPoints[_endPositionInLine] = _hitPosition;
                _nbTargetPoints = _endPositionInLine + 1;
            }
        }

        var hasSelection = _lineRenderableAsSelectInteractor != null && _lineRenderableAsSelectInteractor.hasSelection;
        if (_stopLineAtSelection && hasSelection)
        {
            // Use the selected interactable closest to the start of the line.
            var interactablesSelected = _lineRenderableAsSelectInteractor.interactablesSelected;
            var firstPoint = _targetPoints[0];
            var closestEndPoint = _lineRenderableAsSelectInteractor.GetAttachTransform(interactablesSelected[0]).position;
            var closestSqDistance = Vector3.SqrMagnitude(closestEndPoint - firstPoint);
            for (var i = 1; i < interactablesSelected.Count; i++)
            {
                var endPoint = _lineRenderableAsSelectInteractor.GetAttachTransform(interactablesSelected[i]).position;
                var sqDistance = Vector3.SqrMagnitude(endPoint - firstPoint);
                if (sqDistance < closestSqDistance)
                {
                    closestEndPoint = endPoint;
                    closestSqDistance = sqDistance;
                }
            }

            // Only stop at selection if it is closer than the current end point.
            var currentEndSqDistance = Vector3.SqrMagnitude(_targetPoints[_endPositionInLine] - firstPoint);
            if (closestSqDistance < currentEndSqDistance || _endPositionInLine == 0)
            {
                // Find out where the selection point belongs in the line points. Use the closest target point.
                var endPositionForSelection = 1;
                var sqDistanceFromEndPoint = Vector3.SqrMagnitude(_targetPoints[endPositionForSelection] - closestEndPoint);
                for (var i = 2; i < _nbTargetPoints; i++)
                {
                    var sqDistance = Vector3.SqrMagnitude(_targetPoints[i] - closestEndPoint);
                    if (sqDistance < sqDistanceFromEndPoint)
                    {
                        endPositionForSelection = i;
                        sqDistanceFromEndPoint = sqDistance;
                    }
                    else
                    {
                        break;
                    }
                }

                _endPositionInLine = endPositionForSelection;
                _nbTargetPoints = _endPositionInLine + 1;
            }
        }

        if (_smoothMovement && (_nbPreviousRenderPoints == _nbTargetPoints) && !_snapCurve)
        {
            // Smooth movement by having render points follow target points
            var length = 0f;
            var maxRenderPoints = _renderPoints.Length;
            for (var i = 0; i < _nbTargetPoints && _nbRenderPoints < maxRenderPoints; ++i)
            {
                var smoothPoint = Vector3.Lerp(_previousRenderPoints[i], _targetPoints[i], _followTightness * Time.deltaTime);

                _renderPoints[_nbRenderPoints] = smoothPoint;
                _nbRenderPoints++;
            }
        }
        else
        {
            Array.Copy(_targetPoints, _renderPoints, _nbTargetPoints);
            _nbRenderPoints = _nbTargetPoints;
        }

        if (_nbRenderPoints >= 2)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = _nbRenderPoints;
            _lineRenderer.SetPositions(_renderPoints);
        }
        else
        {
            _lineRenderer.enabled = false;
            ClearLineRenderer();
            return;
        }

        // Update previous points
        Array.Copy(_renderPoints, _previousRenderPoints, _nbRenderPoints);
        _nbPreviousRenderPoints = _nbRenderPoints;
        _snapCurve = false;
    }

    void UpdateSettings()
    {
        if (TryFindLineRenderer())
        {
            _snapCurve = true;
        }
    }

    bool TryFindLineRenderer()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null)
        {
            Debug.LogWarning("No Line Renderer found for Interactor Line Visual.", this);
            enabled = false;
            return false;
        }
        return true;
    }

    void FindXROrigin()
    {
        if (_xrOrigin != null)
            return;

        if (_xrOriginCache == null)
            _xrOriginCache = FindObjectOfType<XROrigin>();

        _xrOrigin = _xrOriginCache;
    }

}
