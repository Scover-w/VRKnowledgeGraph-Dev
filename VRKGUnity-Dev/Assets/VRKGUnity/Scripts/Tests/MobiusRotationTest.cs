using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MobiusRotationTest : MonoBehaviour
{
    [SerializeField]
    private Transform _tf;

    public EasingType EasingType;
    public bool Rotate = false;

    public bool StopRotation = false;

    EasingDel _easingFunction;

    private void Start()
    {
        _easingFunction = Easing.GetEasing(EasingType);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Rotate)
            return;

        Rotate = false;
        StopRotation = false;
        StartCoroutine(Rotating());
    }

    IEnumerator Rotating()
    {
        while(!StopRotation) 
        {
            yield return null;
            float rot = _easingFunction(Time.time * .5f % 1f);

            _tf.localRotation = Quaternion.Euler(new Vector3(0f, rot * 360f, 0f));
        }

        float velocity = 0;
        float currentYRotation = _tf.eulerAngles.y;
        float newYRotation = Mathf.SmoothDampAngle(currentYRotation, 0, ref velocity, .5f);

        while (Mathf.Abs(newYRotation) > 0.1f)
        {
            yield return null;
            currentYRotation = _tf.eulerAngles.y;
            newYRotation = Mathf.SmoothDampAngle(currentYRotation, 0, ref velocity, .5f);
            _tf.eulerAngles = new Vector3(_tf.eulerAngles.x, newYRotation, _tf.eulerAngles.z);
        }

        _tf.rotation = Quaternion.identity;
        Debug.Log("Stopped");
    }
}
