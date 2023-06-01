using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;


    [SerializeField]
    OntologySwitchTypeUI _switchTypeUI;


    Ontology _ontology;


    // Start is called before the first frame update
    void Start()
    {
        _referenceHolderSo.GameManager = this;
    }

    [ContextMenu("Play")]
    public async void Play()
    {
        var graphDbRepository = _referenceHolderSo.SelectedGraphDbRepository;
        if (graphDbRepository == null)
        {
            Debug.LogWarning("GameManager : can't play, selectedGraphRepo is null");
            return;
        }

        var childs = await graphDbRepository.LoadChilds();


        _ontology = new(childs.ontology);

        _switchTypeUI.Display(_ontology, childs.ontology);
    }
}
