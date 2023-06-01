using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GraphDbRepository SelectedRepository { private get; set; }


    // Start is called before the first frame update
    void Start()
    {


    }

    [ContextMenu("Play")]
    public void Play()
    {
        GraphDBAPI.GraphDbRepository = SelectedRepository;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
