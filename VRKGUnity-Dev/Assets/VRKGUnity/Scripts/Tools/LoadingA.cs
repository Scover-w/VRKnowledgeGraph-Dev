using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadingA : MonoBehaviour
{
    [SerializeField]
    EasingType _easingType;

    [SerializeField]
    Transform _parentTf;

    [SerializeField]
    GameObject _linePf;

    [SerializeField]
    List<Transform> _pointTfs;

    EasingDel _easingFunc;

    Dictionary<Vector3, LinePath> _linePathsSource;
    Dictionary<Vector3, LinePath> _linePathsTarget;
    List<SphereRoad> _sphereRoads;


    void Start()
    {
        _easingFunc = Easing.GetEasing(_easingType);

        _linePathsSource = new();
        _linePathsTarget = new();
        _sphereRoads = new();


        for (int i = 0; i < _pointTfs.Count; ) 
        {
            Transform tf = _pointTfs[i];
            Transform tf2 = tf;

            if (i + 1 < _pointTfs.Count)
                tf2 = _pointTfs[i + 1];

            Vector3 pos = tf.position;

            var sphereRoad = new SphereRoad(tf);
            sphereRoad.SourcePos = pos;
            _sphereRoads.Add(sphereRoad);

            i++;

            if (i % 4 == 0)
            {
                tf2 = _pointTfs[i - 4];
            }

            Vector3 pos2 = tf2.position;

            var line = CreateLine();

            var linePath = new LinePath(pos, pos2, line);
            linePath.SphereSourceTf = tf;
            linePath.SphereTargetTf = tf2;
            _linePathsSource.Add(pos, linePath);
            _linePathsTarget.Add(pos2 , linePath);
        }

        SetNewTargetToSphereRoad();

        StartCoroutine(SwapingPositions());
    }

    private LineRenderer CreateLine()
    {
        var lineGo = Instantiate(_linePf);
        lineGo.transform.parent = _parentTf;

        return lineGo.GetComponent<LineRenderer>();
    }

    IEnumerator SwapingPositions()
    {
        while(true)
        {

            float t = 0f;
            float speed = 1f / 3f;
            bool hasSwitched = false;

            while(t < 1f)
            {

                UpdateSphereRoads(_easingFunc(t));
                UpdateLinePaths();
                UpdateMaterial(t);
                yield return null;
                t += Time.deltaTime * speed;

                if(t > .5f && !hasSwitched)
                {
                    hasSwitched = true;
                    SwitchLinePaths();
                }

            }

            SphereRoadReachedTarget();
            SetNewTargetToSphereRoad();

            yield return new WaitForSeconds(2f);

        }
    }

    private void SwitchLinePaths()
    {
        LinePath linePath;
        foreach (SphereRoad sphereRoad in _sphereRoads)
        {
            if (_linePathsSource.TryGetValue(sphereRoad.TargetPos, out linePath))
                linePath.SphereSourceTf = sphereRoad.Tf;

            if (_linePathsTarget.TryGetValue(sphereRoad.TargetPos, out linePath))
                linePath.SphereTargetTf = sphereRoad.Tf;
        }
    }

    private void SphereRoadReachedTarget()
    {
        foreach (SphereRoad sphereRoad in _sphereRoads)
        {
            sphereRoad.Update(1f);
            sphereRoad.SourcePos = sphereRoad.TargetPos;
        }
    }

    private void SetNewTargetToSphereRoad()
    {
        var linePaths = _linePathsSource.ToList();

        foreach (var sphereRoad in _sphereRoads)
        {

            int i = 0;
            KeyValuePair<Vector3, LinePath> kvp = linePaths[0];
            Vector3 newPos = sphereRoad.TargetPos;
            LinePath linePath = linePaths[0].Value;

            while (sphereRoad.TargetPos == newPos && i < 5)
            {
                i++;
                kvp = linePaths[Random.Range(0, linePaths.Count)];
                linePath = kvp.Value;
                newPos = linePath.TargetPos;
            }

            linePaths.Remove(kvp);
            sphereRoad.TargetPos = newPos;
        }
    }

    private void UpdateSphereRoads(float t)
    {
        foreach(SphereRoad sphereRoad in _sphereRoads)
        {
            sphereRoad.Update(t);
        }
    }

    private void UpdateLinePaths()
    {
        foreach(LinePath linePath in _linePathsSource.Values)
        {
            linePath.Update();
        }
    }

    private void UpdateMaterial(float t)
    {
        Color color = Color.white;
        color.a = (Mathf.Cos(2 * Mathf.PI * t) + 1) * .5f;

        foreach (LinePath linePath in _linePathsSource.Values)
        {
            linePath.SetColor(color);
        }
    }
}


class LinePath
{
    public Vector3 SourcePos { get { return _sourcePos; } }
    public Vector3 TargetPos { get { return _targetPos; } }

    public Transform SphereSourceTf;
    public Transform SphereTargetTf;

    Vector3 _sourcePos;
    Vector3 _targetPos;

    LineRenderer _line;

    public LinePath(Vector3 sourcePos, Vector3 targetPos, LineRenderer lineRenderer)
    {
        _sourcePos = sourcePos;
        _targetPos = targetPos;
        _line = lineRenderer;
    }

    public void Update()
    {
        _line.SetPosition(0, SphereSourceTf.position);
        _line.SetPosition(1, SphereTargetTf.position);
    }

    public void SetColor(Color color)
    {
        _line.startColor = color;
        _line.endColor = color;
    }
}

class SphereRoad
{
    public Transform Tf { get { return _sphereTf; } }

    Transform _sphereTf;

    public Vector3 SourcePos;
    public Vector3 TargetPos;

    public SphereRoad(Transform sphereTf)
    {
        _sphereTf = sphereTf;
        SourcePos = Vector3.zero;
        TargetPos = Vector3.zero;
    }


    public void Update(float t)
    {
        _sphereTf.position = Vector3.Lerp(SourcePos, TargetPos, t);
    }

}