using UnityEngine;

public class AutoRotateUI : MonoBehaviour
{
    public EasingType EasingType;

    [SerializeField]
    private RectTransform _rectTf;

    EasingDel _easingFunction;

    public float SpeedRot = 1f;
    public float Rotation = 1f;

    

    private void Start()
    {
        _easingFunction = Easing.GetEasing(EasingType);
        _rectTf = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float rot = _easingFunction(Time.time * SpeedRot % 1f);

        _rectTf.rotation =  Quaternion.Euler(new Vector3(0f, 0f, rot * Rotation));
    }

    private void OnValidate()
    {
        _easingFunction = Easing.GetEasing(EasingType);
    }
}
