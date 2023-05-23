using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuikGraph;

public class QuickGraphTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        var nodeA = new Node("literal", "carotte");
        var nodeB = new Node("literal", "courgette");
        var edge = new QuikGraph.Edge<Node>(nodeA,nodeB);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
