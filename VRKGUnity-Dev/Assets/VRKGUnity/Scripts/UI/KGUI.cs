using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KGUI : MonoBehaviour
{
    public KGUIController KGUIManager { set { _kgUiController = value; } }

    [SerializeField]
    GameObject _containerGo;

    [SerializeField]
    HorizontalLayoutGroup _horizontalGoup;

    [SerializeField]
    Transform _multiInputSpawnTf;

    KGUIController _kgUiController;

    public void HasBeenDetached()
    {
        if (_kgUiController == null)
            return;

        _kgUiController.UIHasBeenDetached();
    }

    private enum CanvasAttachment
    {
        Controller,
        Belt,
        World
    }
}
