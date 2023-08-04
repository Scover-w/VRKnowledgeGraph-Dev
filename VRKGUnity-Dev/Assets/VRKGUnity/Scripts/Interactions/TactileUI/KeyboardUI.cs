using AngleSharp.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AIDEN.TactileUI
{
    public class KeyboardUI : MonoBehaviour
    {
        [SerializeField]
        MultiInputController _controllerUI;

        [SerializeField]
        TMP_Text _inputTxt;

        [SerializeField]
        TMP_Text _modeKeyTxt;

        [SerializeField]
        List<KeyUI> _keys;

        [SerializeField]
        Image _capsImg;

        [SerializeField]
        List<Sprite> _capsIcons;

        [SerializeField]
        List<char> _lettersKeyboard;

        [SerializeField]
        List<char> _specialsCharKeyboard;

        CapslockState _capslockState;

        string _inputValue;

        bool _isSpecialChar;

        private void OnEnable()
        {
            _inputValue = "";
            _inputTxt.text = "";
            _isSpecialChar = false;
            _capslockState = CapslockState.Normal;
            SetCharacters();
        }

        public void AddChar(char c)
        {
            _inputValue += c;
            _controllerUI.UpdateInputValue(_inputValue);
            _inputTxt.text = _inputValue;

            if (_capslockState != CapslockState.Maj)
                return;

            _capslockState = CapslockState.Normal;
            _capsImg.sprite = _capsIcons[(int)_capslockState];
            UpdateKeyCase();
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
            _controllerUI.EnterInputValue(_inputValue);
        }

        public void OnSwitchKeyboardType()
        {
            _isSpecialChar = !_isSpecialChar;

            SetCharacters();
        }

        private void SetCharacters()
        {
            bool isNormalCase = _capslockState == CapslockState.Normal;

            int nbKey = _keys.Count;

            List<char> chars = _isSpecialChar ? _specialsCharKeyboard : _lettersKeyboard;

            for (int i = 0; i < nbKey; i++)
            {
                KeyUI key = _keys[i];
                char character = chars[i];

                if (!isNormalCase && character.IsLetter())
                    character = char.ToUpper(character);

                key.SetValue(character);
            }

            if (_isSpecialChar)
                _modeKeyTxt.text = "abc";
            else
                _modeKeyTxt.text = ".?12";
        }

        public void OnSwitchCapslock()
        {
            switch (_capslockState)
            {
                case CapslockState.Normal:
                    _capslockState = CapslockState.Maj;
                    UpdateKeyCase();
                    break;

                case CapslockState.Maj:
                    _capslockState = CapslockState.LockMaj;
                    break;

                case CapslockState.LockMaj:
                    _capslockState = CapslockState.Normal;
                    UpdateKeyCase();
                    break;
            }

            _capsImg.sprite = _capsIcons[(int)_capslockState];
        }

        private void UpdateKeyCase()
        {
            bool isNormal = _capslockState == CapslockState.Normal;

            foreach (var key in _keys)
            {
                if (isNormal)
                    key.ToLower();
                else
                    key.ToUpper();
            }
        }

        public void OnSelectAll()
        {

        }

        public void OnClose()
        {
            _controllerUI.Close();
        }


        public enum CapslockState
        {
            Normal,
            Maj,
            LockMaj
        }


        [ContextMenu("LoadNormalChar")]
        private void LoadNormalChar()
        {
            _isSpecialChar = false;
            _capslockState = CapslockState.Normal;
            SetCharacters();
        }

        [ContextMenu("LoadSpecialChar")]
        private void LoadSpecialChar()
        {
            _isSpecialChar = true;
            _capslockState = CapslockState.Normal;
            SetCharacters();
        }
    }
}