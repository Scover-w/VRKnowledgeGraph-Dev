using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOTestUI : MonoBehaviour
{
    [SerializeField]
    SOContainerTest _soContainerTest;

    SOTest _soTest;

    public float FloatTest = 1f;

    public int IntTest = 1;
    public bool BoolTest = false;

    public Color TestColor;

    public EnumTest EnumTest = EnumTest.TestA;

    // Start is called before the first frame update
    async void Start()
    {
        _soTest = await _soContainerTest.GetSOTest();

        FloatTest = _soTest.FloatTest;

        IntTest = _soTest.IntTest;
        BoolTest = _soTest.BoolTest;

        TestColor = _soTest.TestColor;

        EnumTest = _soTest.EnumTest;
    }


    private void OnDisable()
    {
        _soContainerTest.RefreshEditor();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
            return;

        if (_soTest == null)
            return;

        _soTest.FloatTest = FloatTest;

        _soTest.IntTest = IntTest;
        _soTest.BoolTest = BoolTest;

        _soTest.TestColor = TestColor;

        _soTest.EnumTest = EnumTest;


        _ = _soTest.Save();
    }
}
