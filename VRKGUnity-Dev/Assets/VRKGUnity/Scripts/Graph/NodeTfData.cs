using UnityEngine;

public class NodeTfData : MonoBehaviour
{
    public Transform NodeATf;
    public Transform NodeBTf;


    [ContextMenu("Get Distance")]
    public void GetDistance()
    {
        Debug.Log("Distance : " + (NodeATf.transform.position - NodeBTf.transform.position).magnitude);
    }
}
