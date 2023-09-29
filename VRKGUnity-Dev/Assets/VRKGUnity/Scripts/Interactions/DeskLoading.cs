using System.Collections;
using UnityEngine;

/// <summary>
/// Rotate the Desk when the Graph is retrieved or in simulation.
/// </summary>
public class DeskLoading : MonoBehaviour
{
    [SerializeField]
    GraphManager _graphManager;

    [SerializeField]
    Transform _loadingDiskTf;

    [SerializeField]
    Transform _centerGraphTf;

    //[SerializeField]
    public float _rotationSpeed = 1f;

    [SerializeField]
    float _durationMove = 1f;

    [SerializeField]
    EasingType _easingMove;


    private void Start()
    {
        _loadingDiskTf.gameObject.SetActive(false);
        _graphManager.OnGraphUpdate += OnGraphUpdated;
    }

    [ContextMenu("StartLoad")]
    private void StartLoad()
    {
        StartCoroutine(LodingDisk());
    }

    [ContextMenu("StopLoad")]
    private void StopLoad()
    {
        StopAllCoroutines();
        _loadingDiskTf.gameObject.SetActive(false);
        _loadingDiskTf.localPosition = Vector3.zero;
        _loadingDiskTf.rotation = Quaternion.identity;
    }

    public void OnGraphUpdated(GraphUpdateType updateType)
    {
        if(updateType == GraphUpdateType.RetrievingFromDb)
        {
            StartCoroutine(LodingDisk());
        }
        else
        {
            StopAllCoroutines();
            _loadingDiskTf.gameObject.SetActive(false);
            _loadingDiskTf.localPosition = Vector3.zero;
            _loadingDiskTf.rotation = Quaternion.identity;
        }
    }

    IEnumerator LodingDisk()
    {
        float t = 0f;
        float speed = 1f / _durationMove;

        Vector3 startPos = _loadingDiskTf.position;
        Vector3 endPos = _centerGraphTf.position;

        EasingDel easing = Easing.GetEasing(_easingMove);

        _loadingDiskTf.gameObject.SetActive(true);

        while (t < 1f)
        {
            yield return null;
            t += Time.deltaTime * speed;
            _loadingDiskTf.position = Vector3.Lerp(startPos, endPos, easing(t));
        }

        _loadingDiskTf.position = endPos;


        while (true)
        {
            yield return null;
            _loadingDiskTf.rotation *= Quaternion.Euler(_rotationSpeed * Time.deltaTime, _rotationSpeed * Time.deltaTime, _rotationSpeed * Time.deltaTime);
        }
    }
}
