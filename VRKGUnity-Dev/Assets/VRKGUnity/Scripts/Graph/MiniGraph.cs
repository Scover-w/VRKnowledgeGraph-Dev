using UnityEngine;

public class MiniGraph : MonoBehaviour
{

    public Transform Tf { get { return _miniGraphTf; } }

    [SerializeField]
    Transform _miniGraphTf;

    [SerializeField]
    Transform _watchTf;

    [SerializeField]
    Transform _gpsPointTf;

    bool _inGPSMode = false;
    bool _displayGPS = true;

    float _deltaHeight = .5f;


    private void Update()
    {
        if (!_inGPSMode)
            return;


        _miniGraphTf.position = _watchTf.position + _watchTf.up * _deltaHeight;
    }

    [ContextMenu("SwitchMode")]
    public void SwitchMode()
    {
        _inGPSMode = !_inGPSMode;
    }

    [ContextMenu("SwitchGPSVisibility")]
    public void SwitchGPSVisibility()
    {
        if (!_inGPSMode)
            return;

        _displayGPS = !_displayGPS;
        _miniGraphTf.gameObject.SetActive(_displayGPS);
    }

    public void UpdateGPSPoint(Vector3 normPosition)
    {
        // TODO : convert to miniGraph scale
    }
}
