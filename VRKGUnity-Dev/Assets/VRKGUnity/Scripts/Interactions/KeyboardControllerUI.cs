using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static KeyboardControllerUI;

public class KeyboardControllerUI : MonoBehaviour
{
    private static KeyboardControllerUI _instance;

    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    [SerializeField]
    Transform _keyboardTf;

    [SerializeField]
    KeyboardUI _keyboardUI;

    [SerializeField]
    NumpadUI _numpadUI;

    public delegate void UpdateInput(string input);
    public delegate void EnterInput(string input);

    UpdateInput OnUpdateInput;
    EnterInput OnEnterInput;

    GameObject _keyboardGo;
    GameObject _numpadGo;

    Transform _camTf;


    bool _isUsed = false;
    string _oldInputValue;


    private void Start()
    {
        if(_instance != null)
        {
            Debug.LogError("Multiple instance of the KeyboardUI Script exist.");
            return;
        }

        _instance = this;

        _keyboardGo = _keyboardUI.gameObject;
        _numpadGo = _numpadUI.gameObject;

        _camTf = _referenceHolderSo.HMDCamSA.Value.transform;

        HideKeyboard();
    }


    public static bool DisplayKeyboard(KeyboardUIOptions options)
    {
        if (_instance == null)
            return false;

        return _instance.DisplayKeyboardUnstatic(options);
    }

    public static bool DisplayNumpad(KeyboardUIOptions options)
    {
        if (_instance == null)
            return false;

        return _instance.DisplayNumpadUnstatic(options);
    }

    public bool DisplayKeyboardUnstatic(KeyboardUIOptions options)
    {
        if (_isUsed)
            return false;

        _isUsed = true;
        SetOptions(options);
        Align(options.Alignment, true);
        UpdateTf(options.Position);

        _keyboardGo.SetActive(true);

        return true;
    }

    public bool DisplayNumpadUnstatic(KeyboardUIOptions options)
    {
        if (_isUsed)
            return false;

        _isUsed = true;
        SetOptions(options);
        Align(options.Alignment, false);
        UpdateTf(options.Position);

        _numpadGo.SetActive(true);

        return true;
    }

    private void SetOptions(KeyboardUIOptions options)
    {
        OnUpdateInput = options.UpdateInput; 
        OnEnterInput = options.EnterInput;
        _oldInputValue = options.CurrentInputValue;
    }

    private void Align(KeyboardAlignment alignement, bool isKeyboard)
    {
        Transform tf = isKeyboard? _keyboardGo.transform : _numpadGo.transform;

        if (alignement == KeyboardAlignment.Center)
        {
            tf.localPosition = Vector3.zero;
            return;
        }

        var rectTf = tf.GetComponent<RectTransform>();
        float width = rectTf.rect.width;

        tf.localPosition = new Vector3(width * rectTf.localScale.x * .5f * ((alignement == KeyboardAlignment.Left) ? -1f : 1f), 0, 0);
    }

    private void UpdateTf(Vector3 position)
    {
        _keyboardTf.position = position;

        if (_camTf == null)
            return;

        _keyboardTf.LookAt(_camTf.transform);
    }

    public void UpdateInputValue(string inputValue)
    {
        OnUpdateInput?.Invoke(inputValue);
    }

    public void EnterInputValue(string inputValue)
    {
        OnEnterInput?.Invoke(inputValue);

        _isUsed = false;
        HideKeyboard();
    }

    public void Close()
    {
        OnEnterInput?.Invoke(_oldInputValue);

        _isUsed = false;
        HideKeyboard();
    }

    private void HideKeyboard()
    {
        _keyboardGo.SetActive(false);
        _numpadGo.SetActive(false);
    }

    public enum KeyboardAlignment
    {
        Left,
        Center,
        Right
    }
}

public class KeyboardUIOptions
{
    public Vector3 Position { get; private set; }
    public KeyboardAlignment Alignment { get; private set; }

    public UpdateInput UpdateInput { get; private set; }
    public EnterInput EnterInput { get; private set; }

    public string CurrentInputValue { get; private set; }

    public KeyboardUIOptions(Vector3 position, KeyboardAlignment alignment, UpdateInput updateInput, EnterInput enterInput, string currentInputValue)
    {
        Position = position;
        Alignment = alignment;
        UpdateInput = updateInput;
        EnterInput = enterInput;
        CurrentInputValue = currentInputValue;
    }
}