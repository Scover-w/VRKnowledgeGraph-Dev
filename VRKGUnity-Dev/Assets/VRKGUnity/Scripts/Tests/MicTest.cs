using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using Utilities.Audio;
using Utilities.Encoding.OggVorbis;

public class MicTest : MonoBehaviour
{
    [SerializeField]
    UnityEvent _start;

    [SerializeField]
    UnityEvent _stop;

    [SerializeField]
    TMP_Text _text;

    public int MicId;
    public bool AutoRestart = false;
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
        _clip = Microphone.Start(Microphone.devices[MicId], true, 60, 44100); ;
        
    }

    [ContextMenu("ListMics")]
    public void ListMics()
    {
        int nbMic = Microphone.devices.Length;
        for (int i = 0; i < nbMic; i++)
        {
            Debug.Log(i+ " : " + Microphone.devices[i]);
        }
    }


    [ContextMenu("Stop Mic")]
    public async void StopMic()
    {
        _stop.Invoke();
        Microphone.End(Microphone.devices[0]);



        var byteAudio = _clip.EncodeToOggVorbisStream(true);
        var bipbop = await new WhisperAPI().TranscribeAudio(byteAudio);
        _text.text = bipbop;

        if (!AutoRestart)
            return;

        Invoke(nameof(StartMic), 10f);
        Invoke(nameof(StopMic), 15f);
    }


    [ContextMenu("WhisperTest")]
    public async void WhisperTest()
    {
        var bipbop = await new WhisperAPI().TranscribeAudio("C:\\Users\\William\\Desktop\\VRKnowledgeGraph-Dev\\VRKGUnity-Dev\\Assets\\VRKGUnity\\Sounds\\Tests\\test.mp3");
        Debug.Log($"{bipbop}");

    }

    [ContextMenu("TestMics")]
    public void TestMics()
    {
        int nbMic = Microphone.devices.Length;

        for (int i = 0; i < nbMic; i++)
        {
            Debug.Log(Microphone.devices[i]);
        }

    }


    [ContextMenu("StartMicComputer")]
    public void StartMicComputer()
    {
        _clip = Microphone.Start(Microphone.devices[1], true, 60, 44100); ;
    }

    [ContextMenu("StopMicComputer")]
    public async void StopMicComputer()
    {
        Microphone.End(Microphone.devices[1]);

        string path = Path.Combine(Application.persistentDataPath, "Tests", "Audio");

        if(!Directory.Exists(path))
            Directory.CreateDirectory(path);

        Debug.Log(path);

        var byteAudio = _clip.EncodeToOggVorbisStream(true);
        //await File.WriteAllBytesAsync(Path.Combine(path, "test.ogg"), byteAudio);

        Debug.Log("Encoded");
        var bipbop = await new WhisperAPI().TranscribeAudio(byteAudio);
        Debug.Log($"{bipbop}");
    }
}
