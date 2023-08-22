using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

public class MicTest : MonoBehaviour
{
    [SerializeField]
    UnityEvent _start;

    [SerializeField]
    UnityEvent _stop;

    AudioClip _clip;

    // Start is called before the first frame update
    void Start()
    {
        if(!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);

        Invoke(nameof(StartMic), 10f);
        Invoke(nameof(StopMic), 15f);
    }


    [ContextMenu("Start Mic")]
    public void StartMic()
    {
        _start.Invoke();
        
        Debug.Log(Microphone.devices.Length);
        Debug.Log(Microphone.devices[0]);
        Debug.Log(Microphone.devices[1]);
        Debug.Log(Microphone.devices[2]);
        _clip = Microphone.Start(Microphone.devices[0], true, 60, 44100); ;
        
    }


    [ContextMenu("Stop Mic")]
    public void StopMic()
    {
        _stop.Invoke();
        Microphone.End(Microphone.devices[0]);

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = _clip;
        audioSource.Play();

        Invoke(nameof(StartMic), 10f);
        Invoke(nameof(StopMic), 15f);
    }


    [ContextMenu("WhisperTest")]
    public async void WhisperTest()
    {
        var bipbop = await new WhisperAPI().TranscribeAudio("C:\\Users\\William\\Desktop\\VRKnowledgeGraph-Dev\\VRKGUnity-Dev\\Assets\\VRKGUnity\\Sounds\\Tests\\test.mp3");
        Debug.Log($"{bipbop}");

    }
}
