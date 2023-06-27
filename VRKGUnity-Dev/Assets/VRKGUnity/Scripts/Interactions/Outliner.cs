using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Outliner : MonoBehaviour
{
    private static HashSet<Mesh> _registeredMeshes = new HashSet<Mesh>();

    public enum Mode
    {
        OutlineAll,
        OutlineVisible,
        OutlineHidden,
        OutlineAndSilhouette,
        SilhouetteOnly
    }

    [SerializeField]
    private Mode _outlineMode;

    private Color _outlineColor = Color.white;

    [SerializeField, Range(0f, 10f)]
    private float _outlineWidth = 2f;

    [SerializeField]
    Material _outlineFillMat;
    [SerializeField]
    Material _outlineMaskMat;

    [SerializeField]
    Color _selectedColor;

    [SerializeField]
    Color _hoverColor;

    [SerializeField]
    Color _inPropagationColor;

    MaterialPropertyBlock _propertyBlock;
    Renderer[] _renderers;

    bool _hasOutline = false;
    bool _lastIsSelect = false;


    void Awake()
    {
        _propertyBlock = new();
        _renderers = GetComponentsInChildren<Renderer>();
        LoadSmoothNormals();
    }

 

    private void TryAddOutline()
    {
        if (_renderers == null || _renderers.Length == 0)
            return;

        foreach (var renderer in _renderers)
        {
            // Append outline shaders
            var materials = renderer.sharedMaterials.ToList();

            if (materials.Count > 1)
                continue;

            materials.Add(_outlineFillMat);
            materials.Add(_outlineMaskMat);

            renderer.sharedMaterials = materials.ToArray();
        }
    }


    private void TryRemoveOutline()
    {
        foreach (var renderer in _renderers)
        {

            // Remove outline shaders
            var materials = renderer.sharedMaterials.ToList();

            if (materials.Count < 2)
                return;

            materials.Remove(_outlineFillMat);
            materials.Remove(_outlineMaskMat);

            renderer.sharedMaterials = materials.ToArray();
        }
    }

    void OnDisable()
    {
        TryRemoveOutline();
    }

    public void SetOutlineWidth(float outlineWidth)
    {
        _outlineWidth = outlineWidth;
        UpdateMaterialProperties();
    }

    public void SetOutlineColor(Color outlineColor)
    {
        _outlineColor = outlineColor;
        UpdateMaterialProperties();
    }

    public void UpdateInteraction(bool isHovered, bool isSelected, bool isInPropagation)
    {
        Color color = Color.white;


        // Allow to color in the selected when click on node, and not stay in the isHovered Color
        if(isSelected && _lastIsSelect == false)
        {
            color = _selectedColor;
        }
        else if(isHovered)
        {
            color = _hoverColor;
        }
        else if(isInPropagation && !isSelected) 
        {
            color = _inPropagationColor;
        }
        else if(isSelected)
        {
            color = _selectedColor;
        }

        if(isHovered | isSelected | isInPropagation)
        {
            if(!_hasOutline)
                TryAddOutline();

            _hasOutline = true;
            SetColor(color);
        }
        else if(!isHovered && !isSelected && !isInPropagation)
        {
            if(_hasOutline)
                TryRemoveOutline();

            _hasOutline = false;
        }

        _lastIsSelect = isSelected;
    }

    private void SetColor(Color color)
    {
        _propertyBlock.SetColor("_OutlineColor", color);

        foreach (var renderer in _renderers)
        {

            if (renderer.sharedMaterials.Length < 2)
                continue;


            renderer.SetPropertyBlock(_propertyBlock, 1);
            //var material = renderer.sharedMaterials[1];
            //material.SetColor("_OutlineColor", color);
        }
    }

    void LoadSmoothNormals()
    {

        // Retrieve or generate smooth normals
        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {

            // Skip if smooth normals have already been adopted
            if (!_registeredMeshes.Add(meshFilter.sharedMesh))
            {
                continue;
            }

            // Retrieve or generate smooth normals
            var smoothNormals = SmoothNormals(meshFilter.sharedMesh);

            // Store smooth normals in UV3
            meshFilter.sharedMesh.SetUVs(3, smoothNormals);

            // Combine submeshes
            var renderer = meshFilter.GetComponent<Renderer>();

            if (renderer != null)
            {
                CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
            }
        }

        // Clear UV3 on skinned mesh renderers
        foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {

            // Skip if UV3 has already been reset
            if (!_registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
            {
                continue;
            }

            // Clear UV3
            skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

            // Combine submeshes
            CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
        }
    }

    List<Vector3> SmoothNormals(Mesh mesh)
    {

        // Group vertices by location
        var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

        // Copy normals to a new list
        var smoothNormals = new List<Vector3>(mesh.normals);

        // Average normals for grouped vertices
        foreach (var group in groups)
        {

            // Skip single vertices
            if (group.Count() == 1)
            {
                continue;
            }

            // Calculate the average normal
            var smoothNormal = Vector3.zero;

            foreach (var pair in group)
            {
                smoothNormal += smoothNormals[pair.Value];
            }

            smoothNormal.Normalize();

            // Assign smooth normal to each vertex
            foreach (var pair in group)
            {
                smoothNormals[pair.Value] = smoothNormal;
            }
        }

        return smoothNormals;
    }

    void CombineSubmeshes(Mesh mesh, Material[] materials)
    {

        // Skip meshes with a single submesh
        if (mesh.subMeshCount == 1)
        {
            return;
        }

        // Skip if submesh count exceeds material count
        if (mesh.subMeshCount > materials.Length)
        {
            return;
        }

        // Append combined submesh
        mesh.subMeshCount++;
        mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
    }

    void UpdateMaterialProperties()
    {

        // Apply properties according to mode
        _outlineFillMat.SetColor("_OutlineColor", _outlineColor);

        switch (_outlineMode)
        {
            case Mode.OutlineAll:
                _outlineMaskMat.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMat.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMat.SetFloat("_OutlineWidth", _outlineWidth);
                break;

            case Mode.OutlineVisible:
                _outlineMaskMat.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMat.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                _outlineFillMat.SetFloat("_OutlineWidth", _outlineWidth);
                break;

            case Mode.OutlineHidden:
                _outlineMaskMat.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMat.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                _outlineFillMat.SetFloat("_OutlineWidth", _outlineWidth);
                break;

            case Mode.OutlineAndSilhouette:
                _outlineMaskMat.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                _outlineFillMat.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
                _outlineFillMat.SetFloat("_OutlineWidth", _outlineWidth);
                break;

            case Mode.SilhouetteOnly:
                _outlineMaskMat.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
                _outlineFillMat.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
                _outlineFillMat.SetFloat("_OutlineWidth", 0f);
                break;
        }
    }
}
