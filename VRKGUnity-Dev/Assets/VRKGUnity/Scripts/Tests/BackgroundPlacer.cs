using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPlacer : MonoBehaviour
{
    public GameObject[] BackObjects;
    public Transform Player;
    public float MinDistance = 10f;
    public float MaxDistance = 20f;

    public Transform BackParentTf;

    public int ToPlace = 50;

    [ContextMenu("PlaceObjects")]
    void PlaceObjects()
    {
        for(int i = 0; i < ToPlace; i++)
        {
            float rdDegree = Random.Range(0f, 360f);
            Vector3 forward = Vector3.forward;
            Quaternion rotation = Quaternion.Euler(0, rdDegree, 0f);
            forward = rotation * forward;

            float distance = Random.Range(MinDistance, MaxDistance);
            forward *= distance;


            var pf = SelectObject(rdDegree);

            var go = Instantiate(pf, BackParentTf);
            var tf = go.transform;
            tf.localPosition = forward;
            tf.LookAt(BackParentTf);
        }


        
    }

    GameObject SelectObject(float deg)
    {
        
        if(deg < 30f || deg > 330f)
            return BackObjects[0];

        if(deg < 120f && deg > 60f)
            return BackObjects[1];  

        if(deg < 210f && deg > 150f)
            return BackObjects[2];

        if(deg < 300f && deg > 240f)
            return BackObjects[3];

        float rdObject = Random.Range(0f, 1f);

        if (deg < 60f)
        {
            deg -= 60f;
            float probA = deg / 30f;
            
            return (probA > rdObject)? BackObjects[0]: BackObjects[1];

        }

        if(deg < 150f)
        {
            deg -= 150f;
            float probB = deg / 30f;

            return (probB > rdObject) ? BackObjects[1] : BackObjects[2];
        }

        if(deg < 240f)
        {
            deg -= 240f;
            float probC = deg / 30f;

            return (probC > rdObject) ? BackObjects[2] : BackObjects[3];
        }


        deg -= 330f;
        float probD = deg / 30f;

        return (probD > rdObject) ? BackObjects[3] : BackObjects[0];




    }
}
