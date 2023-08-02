using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static KeyboardUI;

public class NumpadUI : MonoBehaviour
{
    [SerializeField]
    KeyboardControllerUI _controllerUI;

    [SerializeField]
    TMP_Text _inputTxt;

    string _inputValue;

    private void OnEnable()
    {
        _inputValue = "";
        _inputTxt.text = "";
    }

    public void AddChar(char c)
    {
        _inputValue += c;
        _controllerUI.UpdateInputValue(_inputValue);
        _inputTxt.text = _inputValue;
    }

    public void OnDeleteEnd()
    {
        if (_inputValue.Length == 0)
            return;

        _inputValue = _inputValue[..^1];
        _controllerUI.UpdateInputValue(_inputValue);
        _inputTxt.text = _inputValue;
    }

    public void OnEnter()
    {
        _controllerUI.EnterInputValue(_inputValue);
    }

    public void OnClose()
    {
        _controllerUI.Close();
    }

    public void OnSelectAll()
    {

    }
}
