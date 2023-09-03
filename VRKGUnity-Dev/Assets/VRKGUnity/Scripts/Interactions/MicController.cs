using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static MicController;

public class MicController : MonoBehaviour
{
    [SerializeField]
    InputActionReference _performMicActionRef;

    InputAction _performMicAction;


    public delegate void NewAudioClipMic(AudioClip ac);
    public static NewAudioClipMic OnNewAudioClipMic;


    [SerializeField]
    UnityEvent<AudioClip> _newVoiceClip;

    bool _isRegistered = false;
    bool _isRegisteringMic = false;

    AudioClip _clip;

    private void Awake()
    {
        _performMicAction = _performMicActionRef.action;
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

    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);
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
        _performMicAction.performed += PerformMic;
        _isRegisteringMic = false;
        _isRegistered = true;
    }

    private void Unregister()
    {
        _performMicAction.performed -= PerformMic;
        _isRegistered = false;


        if (!_isRegisteringMic)
            return;

        _isRegisteringMic = false;
        Microphone.End(Microphone.devices[0]);
    }


    private void PerformMic(InputAction.CallbackContext context)
    {
        _isRegisteringMic = !_isRegisteringMic;

        DebugDev.Log("PerformMic " + _isRegisteringMic);

        if(_isRegisteringMic)
        {
            _clip = Microphone.Start(Microphone.devices[0], true, 60, 44100);
        }
        else
        {
            Microphone.End(Microphone.devices[0]);
            _newVoiceClip?.Invoke(_clip);
            OnNewAudioClipMic?.Invoke(_clip);
            _clip = null;
        }
    }

}
