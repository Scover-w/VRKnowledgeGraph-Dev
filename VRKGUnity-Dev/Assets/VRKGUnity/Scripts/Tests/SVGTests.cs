using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class SVGTests : MonoBehaviour
{
    public float curveThickness = 2.0f;  // Set the thickness of the curve

    public Transform CanvasTransform;
    public Material Material;

    public Image Img;
    public RawImage RawImage;

    public bool CreateSVG = false;
    //void Start()
    //{
    //    var path = new List<Vector2> { new Vector2(0, 0), new Vector2(100, 100), new Vector2(200, 0) };  // Replace with your list of 2D points

    //    var segments = new List<BezierPathSegment>();
    //    for (int i = 0; i < path.Count - 1; i++)
    //    {
    //        segments.Add(new BezierPathSegment { P0 = path[i], P1 = path[i + 1] });
    //    }


    //    var pathProps = new PathProperties
    //    {
    //        Stroke = new Stroke
    //        {
    //            Color = Color.black,
    //            HalfThickness = curveThickness / 2.0f,
    //        }
    //    };

    //    var geom = VectorUtils.TessellatePath(bezierPath, pathProps, 4.0f);

    //    // Create a GameObject to display the curve
    //    var sprite = VectorUtils.BuildSprite(geom, 100.0f, VectorUtils.Alignment.Center, Vector2.zero, 128);
    //    var vectorObj = gameObject.AddComponent<VectorObject>();
    //    vectorObj.Shape = sprite;
    //}


    private void Update()
    {
        if (!CreateSVG)
            return;

        CreateSVG = false;

        Test();
    }

    [ContextMenu("Test")]
    public void Test()
    {
        DebugDev.Log("Test");
        // Create a new BezierContour
        BezierContour contour = new BezierContour();
        contour.Segments = new BezierPathSegment[3];

        // Set the control points for the curve
        contour.Segments[0].P0 = new Vector2(0, 0);
        contour.Segments[0].P1 = new Vector2(50, 100);
        contour.Segments[0].P2 = new Vector2(100, 100);

        contour.Segments[1].P0 = new Vector2(100, 100);
        contour.Segments[1].P1 = new Vector2(150, 100);
        contour.Segments[1].P2 = new Vector2(200, 0);

        // Create a shape from the BezierContour
        Shape shape = new Shape();
        shape.Contours = new BezierContour[] { contour };
        shape.PathProps = new PathProperties()
        {
            Stroke = new Stroke() { Color = Color.white, HalfThickness = 5.0f }
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
                MaxCordDeviation = 0.5f,
                MaxTanAngleDeviation = 0.1f,
                SamplingStepSize = 0.01f
            });

        var sprite = VectorUtils.BuildSprite(geometry, 100, VectorUtils.Alignment.Center, new Vector2(0.5f, 0.5f), 256);

        var t1 = stopwatch.ElapsedMilliseconds;

        Texture2D text2D = VectorUtils.RenderSpriteToTexture2D(sprite, 300, 300, Material, 8);
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
