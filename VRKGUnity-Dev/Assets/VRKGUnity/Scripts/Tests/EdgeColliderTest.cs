using UnityEngine;

public class EdgeColliderTest : MonoBehaviour
{

    [SerializeField]
    Transform _tfA;

    [SerializeField]
    Transform _tfB;

    [SerializeField]
    CapsuleCollider _capsuleCollider;

    // Update is called once per frame
    void Update()
    {
        var posA = _tfA.position;
        var posB = _tfB.position;

        Vector3 point1 = _tfA.position;
        Vector3 point2 = _tfB.position;

        transform.position = Vector3.Lerp(point1, point2, 0.5f);

        _capsuleCollider.height = (point2 - point1).magnitude;

        // The capsule's orientation should be along the line between the spheres
        transform.LookAt(_tfB);

    }
}
