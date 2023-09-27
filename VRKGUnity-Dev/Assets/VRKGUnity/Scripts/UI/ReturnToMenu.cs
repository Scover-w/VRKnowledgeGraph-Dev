using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToMenu : MonoBehaviour
{
    [SerializeField]
    ReferenceHolderSO _referenceHolderSo;

    public void ReturnToMenuClick()
    {
        _referenceHolderSo.LifeCycleSceneManagerSA?.Value?.LoadScene(Scenes.MainMenu);
    }
}
