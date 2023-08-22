using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotateTf : MonoBehaviour
{
    public EasingType EasingType;

    [SerializeField]
    private Transform _tf;

    EasingDel _easingFunction;

    public float SpeedRot = 1f;
    public float Rotation = 1f;



    private void Start()
    {
        _easingFunction = Easing.GetEasing(EasingType);
    }

    private void Update()
    {
        float rot = _easingFunction(Time.time * SpeedRot % 1f);

        _tf.localRotation = Quaternion.Euler(new Vector3(0f, 0f, rot * Rotation));
    }

    private void OnValidate()
    {
        _easingFunction = Easing.GetEasing(EasingType);
    }
}
