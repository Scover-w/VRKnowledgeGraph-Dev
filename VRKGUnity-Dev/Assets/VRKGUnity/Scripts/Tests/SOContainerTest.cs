using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "GraphConfig", menuName = "ScriptableObjects/SOContainerTest", order = 1)]
public class SOContainerTest : ScriptableObject
{
    [SerializeField]
    SOTest _soTest;

    public Color TestColor = Color.white;

    private async void Awake()
    {
        RefreshEditor();
        
    }

    private void OnEnable()
    {
        ForceLoad();
    }

    public async void RefreshEditor()
    {
        _soTest = await SOTest.Load();
        TestColor = _soTest.TestColor;
    }




    [ContextMenu("ForceLoad")]
    public async Task ForceLoad()
    {
        Debug.LogWarning("ForceLoad");

        try
        {
            _soTest = await SOTest.Load();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    public async void Save()
    {
        if (_soTest == null) return;

        await Task.Run(async () =>
        {
            await _soTest.Save();
        });
    }


    public async Task<SOTest> GetSOTest()
    {

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            _soTest = await SOTest.Load();
            return _soTest;
        }
#endif



        if (_soTest == null)
            _soTest = await SOTest.Load();

        return _soTest;
    }


    private async void OnValidate()
    {
        _soTest.TestColor = TestColor;

        await _soTest.Save();
    }

}
