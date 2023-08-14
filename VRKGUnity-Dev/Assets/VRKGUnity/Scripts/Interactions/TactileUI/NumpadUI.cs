using TMPro;
using UnityEngine;

namespace AIDEN.TactileUI
{
    public class NumpadUI : MonoBehaviour
    {
        [SerializeField]
        MultiInputController _controllerUI;

        [SerializeField]
        TMP_Text _inputTxt;

        string _inputValue;

        private void OnEnable()
        {
            _inputValue = "";
            _inputTxt.text = "";
        }

        public void AddChar(char c)
        {
            _inputValue += c;
            _controllerUI.UpdateInputValue(_inputValue);
            _inputTxt.text = _inputValue;
        }

        public void OnDeleteEnd()
        {
            if (_inputValue.Length == 0)
                return;

            _inputValue = _inputValue[..^1];
            _controllerUI.UpdateInputValue(_inputValue);
            _inputTxt.text = _inputValue;
        }

        public void OnEnter()
        {
            if (float.TryParse(_inputValue, out float result))
            {
                _controllerUI.EnterInputValue(result);
            }
            else
            {
                _controllerUI.EnterInputValue(false);
            }
        }

        public void OnClose()
        {
            _controllerUI.Close();
        }

        public void OnSelectAll()
        {

        }
    }
}