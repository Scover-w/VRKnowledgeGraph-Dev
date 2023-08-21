using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePlacer : MonoBehaviour
{
    public List<GameObject> LinesA;
    public List<GameObject> LinesB;
    public List<GameObject> LinesC;

    public List<Material> Materials;

    public float MinDistance = 10f;
    public float MaxDistance = 20f;

    public float ZRange = 20f;

    public int ToPlace = 50;
    public Transform BackParentTf;

    [ContextMenu("PlaceLines")]
    void PlaceLines()
    {
        for (int i = 0; i < ToPlace; i++)
        {
            float rdDegreeY = Random.Range(0f, 360f);
            float rdDegreeX = Random.Range(-ZRange, ZRange);


            Vector3 position = Vector3.forward;
            Quaternion rotationY = Quaternion.Euler(0, rdDegreeY, 0f);
            Quaternion rotationX = Quaternion.Euler(rdDegreeX, 0f, 0f);
            position = rotationX * position;
            position = rotationY * position;

            float distance = Random.Range(MinDistance, MaxDistance);
            position *= distance;


            var pf = SelectLine(distance);
            var material = SelectMat(rdDegreeY);


            var go = Instantiate(pf, BackParentTf);
            var tf = go.transform;
            tf.localPosition = position;
            tf.LookAt(BackParentTf);

            var renderer = go.GetComponent<Renderer>();
            renderer.material = material;
        }
    }

    GameObject SelectLine(float distance)
    {
        float delta = (MaxDistance - MinDistance) * .5f;
        float middle = delta + MinDistance;

        if (distance < middle)
        {
            float norm = (distance - middle) / delta;

            bool isFirst = Random.Range(0f, 1f) > norm;

            return isFirst ? LinesA[Random.Range(0, LinesA.Count)] : LinesB[Random.Range(0, LinesB.Count)];
        }


        float normB = (distance - MaxDistance) / delta;

        bool isFirstB = Random.Range(0f, 1f) > normB;

        return isFirstB ? LinesB[Random.Range(0, LinesB.Count)] : LinesC[Random.Range(0, LinesC.Count)];
    }

    Material SelectMat(float deg)
    {
        if (deg < 30f || deg > 330f)
            return Materials[0];

        if (deg < 120f && deg > 60f)
            return Materials[1];

        if (deg < 210f && deg > 150f)
            return Materials[2];

        if (deg < 300f && deg > 240f)
            return Materials[3];

        float rdObject = Random.Range(0f, 1f);

        if (deg < 60f)
        {
            deg -= 60f;
            float probA = deg / 30f;

            return (probA > rdObject) ? Materials[0] : Materials[1];

        }

        if (deg < 150f)
        {
            deg -= 150f;
            float probB = deg / 30f;

            return (probB > rdObject) ? Materials[1] : Materials[2];
        }

        if (deg < 240f)
        {
            deg -= 240f;
            float probC = deg / 30f;

            return (probC > rdObject) ? Materials[2] : Materials[3];
        }


        deg -= 330f;
        float probD = deg / 30f;

        return (probD > rdObject) ? Materials[3] : Materials[0];
    }
}
