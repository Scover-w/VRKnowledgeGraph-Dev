using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using VDS.RDF.Parsing.Tokens;
using Random = UnityEngine.Random;


public class LegendTests : MonoBehaviour
{
    public float Hbandwidth = 1.0f; // Bandwidth
    public int Resolution;

    public float NbValue;
    public LineRenderer LineRenderer;

    public AnimationCurve Curve;

    public Material Material;

    public RawImage RawImage;
    public RectTransform RectTransform;
    public float HalfThickness = 2.5f;

    public bool CreateSVG = false;


    private void Update()
    {
        if (!CreateSVG)
            return;

        CreateSVG = false;

        TestDistributionCurve();
    }

    [ContextMenu("TestDistributionCurve")]
    public void TestDistributionCurve()
    {
        List<float> values = GetRandomList();

        var gaussianKDE = new GaussianKDEEstimator();

        gaussianKDE.BandwidthH = Hbandwidth;

        List<float> points = gaussianKDE.GaussianKDE(values, Resolution);


        int nbPoints = points.Count;

        //LineRenderer.positionCount = nbPoints;

        var x = RectTransform.sizeDelta.x;
        var y = RectTransform.sizeDelta.y;

        List<Vector2> points2D = new();

        for (int i = 0; i < nbPoints; i++)
        {
            float yPos = (points[i] - gaussianKDE.MinDensity) / (gaussianKDE.MaxDensity - gaussianKDE.MinDensity);

            Vector2 pos = new( i/ (float)nbPoints * x, yPos * y);
            DebugDev.Log(pos);
            points2D.Add(pos);

            //LineRenderer.SetPosition(i, pos);
        }

        CreateTexture2D(points2D);
    }


    public List<float> GetRandomList()
    {
        List<float> list = new();

        for (int i = 0; i < NbValue; i++)
        {
            list.Add(Curve.Evaluate(Random.Range(0f, 1f)));
        }

        return list;
    }

    public void CreateTexture2D(List<Vector2> points2D)
    {
        DebugDev.Log("Test");
        int nbPoints = points2D.Count;
        // Create a new BezierContour
        BezierContour contour = new BezierContour();
        contour.Segments = new BezierPathSegment[nbPoints];

        List<BezierPathSegment> segments = new List<BezierPathSegment>();

        for (int i = 0; i < nbPoints - 3; i += 3)
        {
            segments.Add(new BezierPathSegment()
            {
                P0 = points2D[i],
                P1 = points2D[i + 1],
                P2 = points2D[i + 2]
            });
        }

        // Add the last point to complete the path
        segments.Add(new BezierPathSegment()
        {
            P0 = points2D[nbPoints - 1]
        });


        BezierPathSegment[] bezierSegments = segments.ToArray();
        contour.Segments = bezierSegments;

        // Create a shape from the BezierContour
        Shape shape = new Shape();
        shape.Contours = new BezierContour[] { contour };
        shape.PathProps = new PathProperties()
        {
            Stroke = new Stroke() { Color = Color.white, HalfThickness = HalfThickness }
        };

        var scenNode = new SceneNode()
        {
            Shapes = new List<Shape> { shape }
        };

        var scene = new Scene();
        scene.Root = scenNode;


        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        // Tessellate the shape into a mesh
        var geometry = VectorUtils.TessellateScene(scene,
            new VectorUtils.TessellationOptions()
            {
                StepDistance = 100.0f,
                MaxCordDeviation = float.MaxValue,
                MaxTanAngleDeviation = Mathf.PI / 2.0f,
                SamplingStepSize = 0.01f
            });

        var sprite = VectorUtils.BuildSprite(geometry, 100, VectorUtils.Alignment.Center, new Vector2(0.5f, 0.5f), 256);

        var t1 = stopwatch.ElapsedMilliseconds;

        DebugDev.Log("x : " + (int)RectTransform.sizeDelta.x + " , y : " + (int)RectTransform.sizeDelta.y);

        Texture2D text2D = VectorUtils.RenderSpriteToTexture2D(sprite, (int)RectTransform.sizeDelta.x, (int)RectTransform.sizeDelta.y, Material, 8);
        RawImage.texture = text2D;
        stopwatch.Stop();

        var t2 = stopwatch.ElapsedMilliseconds;

        DebugDev.Log("t1 " + t1);
        DebugDev.Log("t2 " + t2);

        //curveObject.transform.SetParent(CanvasTransform, false);

        //MeshFilter meshFilter = curveObject.AddComponent<MeshFilter>();


        //MeshRenderer meshRenderer = curveObject.AddComponent<MeshRenderer>();
        //meshRenderer.material = Material;

        //VectorUtils.FillMesh(meshFilter.mesh, geometry, 100);

    }
}


public class GaussianKDEEstimator
{
    public float BandwidthH = .5f;

    public float MinDensity { get; private set; }
    public float MaxDensity { get; private set; }


    public List<float> GaussianKDE(List<float> data, int resolution)
    {
        // Initialize
        List<float> kdeValues = new List<float>();
        float min = data.Min();
        float max = data.Max();
        float range = max - min;
        float step = range / (resolution - 1);

        MinDensity = float.MaxValue;
        MaxDensity = float.MinValue;

        int n = data.Count; // Number of data points

        // Calculate KDE values for the range
        for (int i = 0; i < resolution; i++)
        {
            float x = min + i * step;
            float sum = 0.0f;

            foreach (float xi in data)
            {
                float z = (x - xi) / BandwidthH;
                sum += (float)(Math.Exp(-0.5 * z * z) / Math.Sqrt(2 * Math.PI));
            }

            float density = sum / (n * BandwidthH);
            kdeValues.Add(density);

            if (density < MinDensity)
                MinDensity = density;
            else if(density > MaxDensity)
                MaxDensity = density;


        }

        return kdeValues;
    }
}