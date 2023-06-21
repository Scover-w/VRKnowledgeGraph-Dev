using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingBarUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text _msgTxt;

    [SerializeField]
    RectTransform _rectTf;


    public float Reactivity = 2f;

    float _targetValue = 0f;
    float _currentValue = 0f;
    Vector2 _loadingbarSize;

    private void Start()
    {
        _loadingbarSize = _rectTf.sizeDelta;
        _rectTf.sizeDelta = new Vector2(0f, _loadingbarSize.y);
    }

    public void Refresh(float newTargetValue, string msg = "")
    {
        if (msg.Length > 0)
            _msgTxt.text = msg;

        _targetValue = newTargetValue;
    }

    private void Update()
    {
        _currentValue = Mathf.Lerp(_currentValue, _targetValue, Reactivity * Time.deltaTime);
        var newX = Mathf.Lerp(0f, _loadingbarSize.x, _currentValue);

        _rectTf.sizeDelta = new Vector2(newX, _loadingbarSize.y);
    }


    [ContextMenu("TestLoadingBar")]
    private void TestLoadingBar()
    {
        StartCoroutine(TestingLoadingBar());
    }

    IEnumerator TestingLoadingBar()
    {
        _rectTf.sizeDelta = new Vector2(0f, _loadingbarSize.y);
        _targetValue = 0f;
        _currentValue = 0f;

        
        yield return new WaitForSeconds(2f);
        Refresh(.2f);
        yield return new WaitForSeconds(2f);

        Refresh(.5f);
        yield return new WaitForSeconds(2f);

        Refresh(.8f);
        yield return new WaitForSeconds(1f);

        Refresh(1f);
    }

}
