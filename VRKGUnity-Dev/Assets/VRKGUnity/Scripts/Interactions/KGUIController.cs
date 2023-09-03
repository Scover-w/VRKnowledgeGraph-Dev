using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class KGUIController : MonoBehaviour
{
    [SerializeField]
    GameObject _kguiPf;

    [SerializeField]
    Transform _uiPositionTf;

    [SerializeField]
    InputActionReference _displayUIActionRef;

    InputAction _displayUIAction;

    KGUI _currentKGUI;
    GameObject _currentUIGo;

    Transform _tf;
    bool _isUIDisplayed = false;
    bool _isRegistered = false;

    private void Awake()
    {
        _tf = transform;
        _displayUIAction = _displayUIActionRef.action;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (_isRegistered)
            Unregister();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != Scenes.KG && _isRegistered)
        {
            Unregister();
            return;
        }

        if (scene.name != Scenes.KG)
            return;

        Register();
    }

    private void Register()
    {
        _displayUIAction.performed += SwitchDisplayUI;
        _isRegistered = true;

        if (_currentUIGo == null)
            CreateNewUI();
    }

    private void Unregister()
    {
        _displayUIAction.performed -= SwitchDisplayUI;
        _isRegistered = false;

        if (_isUIDisplayed)
            SwitchDisplayUI(new InputAction.CallbackContext());
    }

    public void UIHasBeenDetached()
    {
        CreateNewUI();
        _isUIDisplayed = false;
    }

    private void CreateNewUI()
    {
        _currentUIGo = Instantiate(_kguiPf, _tf);
        _currentKGUI = _currentUIGo.GetComponent<KGUI>();
        _currentKGUI.KGUIController = this;
        _currentUIGo.SetActive(false);
        
        var tf = _currentUIGo.transform;
        tf.position = _uiPositionTf.position;
        tf.localRotation = _uiPositionTf.localRotation;
    }

    private void SwitchDisplayUI(InputAction.CallbackContext context)
    {
        _isUIDisplayed = !_isUIDisplayed;
        _currentUIGo.SetActive(_isUIDisplayed);
    }
}
