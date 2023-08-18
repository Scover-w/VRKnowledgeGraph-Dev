using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTriggerTest : MonoBehaviour
{

    public bool Collide;
    RaycastHit hit;
    int _layer;

    Transform _tf;

    // Start is called before the first frame update
    void Start()
    {
        _tf = transform;
        _layer = 1 << Layers.Node;
    }

    // Update is called once per frame
    void Update()
    {
        Collide= Physics.Raycast(_tf.position, _tf.forward, out hit, 50f, _layer);
    }
}
