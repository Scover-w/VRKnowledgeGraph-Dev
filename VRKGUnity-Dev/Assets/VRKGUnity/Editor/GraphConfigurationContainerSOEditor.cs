using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;





/*

[CustomEditor(typeof(GraphConfigurationContainerSO))]
public class GraphConfigurationContainerSOEditor : Editor
{
    private SerializedProperty _graphConfigurationProperty;
    private SerializedProperty _colorLerpMapperProperty;

    private GraphConfiguration _graphConfiguration;

    private async void OnEnable()
    {
        GraphConfigurationContainerSO sO = (GraphConfigurationContainerSO)target;
        await sO.ForceLoad();

        _graphConfigurationProperty = serializedObject.FindProperty("_graphConfiguration");
        _colorLerpMapperProperty = _graphConfigurationProperty.FindPropertyRelative("NodeColorMapping");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (_graphConfigurationProperty == null || _colorLerpMapperProperty == null)
        {
            GraphConfigurationContainerSO sO = (GraphConfigurationContainerSO)target;
            sO.GetGraphConfiguration();

            _graphConfigurationProperty = serializedObject.FindProperty("_graphConfiguration");
            _colorLerpMapperProperty = _graphConfigurationProperty.FindPropertyRelative("NodeColorMapping");

            if (_graphConfigurationProperty == null || _colorLerpMapperProperty == null)
                return;
        }

        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("Graph Colors", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(_graphConfigurationProperty.FindPropertyRelative("NodeColor"));
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(_colorLerpMapperProperty.FindPropertyRelative("ColorA"));
        EditorGUILayout.PropertyField(_colorLerpMapperProperty.FindPropertyRelative("ColorB"));
        EditorGUILayout.PropertyField(_colorLerpMapperProperty.FindPropertyRelative("ColorC"));
        EditorGUILayout.Space(5);

        EditorGUILayout.Slider(_colorLerpMapperProperty.FindPropertyRelative("BoundaryColorA"), 0f, 1f, "Boundary Color A");
        EditorGUILayout.Slider(_colorLerpMapperProperty.FindPropertyRelative("BoundaryColorB"), 0f, 1f, "Boundary Color B");
        EditorGUILayout.Slider(_colorLerpMapperProperty.FindPropertyRelative("BoundaryColorC"), 0f, 1f, "Boundary Color C");

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Edge", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_graphConfigurationProperty.FindPropertyRelative("EdgeColor"));

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            _graphConfiguration.Save();
        }
    }

    private void OnValidate()
    {
        int i = 0;
        Debug.Log(_graphConfiguration);
        var so = (GraphConfigurationContainerSO)target;
        Debug.Log(so);
    }
}
*/




/*

[CustomEditor(typeof(GraphConfigurationContainerSO))]
public class GraphConfigurationContainerSOEditor : Editor
{

    private GraphConfiguration _graphConfiguration;
    private ColorLerpMapper _colorLerpMapper;

    private async void OnEnable()
    {
        GraphConfigurationContainerSO sO = (GraphConfigurationContainerSO)target;
        _graphConfiguration = await sO.GetGraphConfiguration();
        _colorLerpMapper = _graphConfiguration.NodeColorMapping;
    }

    public override async void OnInspectorGUI()
    {
        if(_graphConfiguration == null || _colorLerpMapper == null)
        {
            GraphConfigurationContainerSO sO = (GraphConfigurationContainerSO)target;
            _graphConfiguration = await sO.GetGraphConfiguration();
            _colorLerpMapper = _graphConfiguration.NodeColorMapping;

            if (_graphConfiguration == null || _colorLerpMapper == null)
                return;
        }


        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("Graph Colors", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Node", EditorStyles.boldLabel);


        _graphConfiguration.NodeColor = EditorGUILayout.ColorField("Node Color", _graphConfiguration.NodeColor);
        EditorGUILayout.Space(5);
        _colorLerpMapper.ColorA = EditorGUILayout.ColorField("Color Mapping A", _colorLerpMapper.ColorA);
        _colorLerpMapper.ColorB = EditorGUILayout.ColorField("Color Mapping B", _colorLerpMapper.ColorB);
        _colorLerpMapper.ColorC = EditorGUILayout.ColorField("Color Mapping C", _colorLerpMapper.ColorC);
        EditorGUILayout.Space(5);

        _colorLerpMapper.BoundaryColorA = EditorGUILayout.Slider("Boundary Color A", _colorLerpMapper.BoundaryColorA, 0f, 1f);
        _colorLerpMapper.BoundaryColorB = EditorGUILayout.Slider("Boundary Color B", _colorLerpMapper.BoundaryColorB, 0f, 1f);
        _colorLerpMapper.BoundaryColorC = EditorGUILayout.Slider("Boundary Color C", _colorLerpMapper.BoundaryColorC, 0f, 1f);


        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Edge", EditorStyles.boldLabel);
        _graphConfiguration.EdgeColor = EditorGUILayout.ColorField("Edge Color", _graphConfiguration.EdgeColor);

        if (EditorGUI.EndChangeCheck())
        {
            _graphConfiguration.Save();
        }
    }

    private void OnValidate()
    {
        int i = 0;
        Debug.Log(_graphConfiguration);
        var so = (GraphConfigurationContainerSO)target;
        Debug.Log(so);

    }
}*/