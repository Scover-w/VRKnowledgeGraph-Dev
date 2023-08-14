using UnityEngine;

namespace AIDEN.TactileUI
{
    public class MultiInputController : MonoBehaviour
    {
        private static MultiInputController _instance;

        public Transform CameraTf { set { _camTf = value; } }


        [SerializeField]
        Transform _controllerTf;

        [SerializeField]
        KeyboardUI _keyboardUI;

        [SerializeField]
        NumpadUI _numpadUI;

        [SerializeField]
        ColorPickerUI _colorPickerUI;

        [SerializeField]
        Transform _camTf;

        UpdateInput OnUpdateInput;
        EnterInput OnEnterInput;

        GameObject _keyboardGo;
        GameObject _numpadGo;
        GameObject _colorpickerGo;

        MultiInputType _usedInput;

        bool _isUsed = false;

        object _oldInputValue;


        private void Start()
        {
            if (_instance != null)
            {
                Debug.LogError("Multiple instance of the KeyboardUI Script exist.");
                return;
            }

            _instance = this;

            _keyboardGo = _keyboardUI.gameObject;
            _numpadGo = _numpadUI.gameObject;
            _colorpickerGo = _colorPickerUI.gameObject;

            HideInputs();
        }


        public static bool Display<T>(KeyboardUIOptions<T> options)
        {
            if (_instance == null)
                return false;

            return _instance.DisplayUnstatic(options);
        }

        public bool DisplayUnstatic<T>(KeyboardUIOptions<T> options)
        {
            if (_isUsed)
                return false;

            if (!UpdateUsedInput(options))
                return false;

            _isUsed = true;
            SetOptions(options);
            Align(options.Alignment);
            UpdateTf(options.Position);

            DisplaySelected();
            TrySetStartValue(options);

            return true;
        }

        private bool UpdateUsedInput<T>(KeyboardUIOptions<T> options)
        {
            var typeValue = typeof(T);

            if (typeValue == typeof(string))
                _usedInput = MultiInputType.Keyboard;
            else if (typeValue == typeof(float))
                _usedInput = MultiInputType.Numpad;
            else if (typeValue == typeof(Color))
                _usedInput = MultiInputType.ColorPicker;
            else
                return false;

            return true;
        }

        private void SetOptions<T>(KeyboardUIOptions<T> options)
        {
            OnUpdateInput = options.UpdateInput;
            OnEnterInput = options.EnterInput;

            _oldInputValue = options.CurrentInputValue;
        }

        private void Align(InputAlignment alignement)
        {
            Transform tf = GetSelectedTransform();

            if (alignement == InputAlignment.Center)
            {
                tf.localPosition = Vector3.zero;
                return;
            }

            var rectTf = tf.GetComponent<RectTransform>();
            float width = rectTf.rect.width;

            tf.localPosition = new Vector3(width * rectTf.localScale.x * .5f * ((alignement == InputAlignment.Left) ? -1f : 1f), 0, 0);
        }

        private Transform GetSelectedTransform()
        {
            switch (_usedInput)
            {
                case MultiInputType.Keyboard:
                    return _keyboardGo.transform;
                case MultiInputType.Numpad:
                    return _numpadGo.transform;
                case MultiInputType.ColorPicker:
                    return _colorpickerGo.transform;
                default:
                    return _keyboardGo.transform;
            }
        }

        private void DisplaySelected()
        {
            switch (_usedInput)
            {
                case MultiInputType.Keyboard:
                    _keyboardGo.SetActive(true);
                    _keyboardGo.GetComponent<RectTransform>().localPosition = Vector3.zero;
                    break;
                case MultiInputType.Numpad:
                    _numpadGo.SetActive(true);
                    _numpadGo.GetComponent<RectTransform>().localPosition = Vector3.zero;
                    break;
                case MultiInputType.ColorPicker:
                    _colorpickerGo.SetActive(true);
                    _colorpickerGo.GetComponent<RectTransform>().localPosition = Vector3.zero;
                    break;
            }
        }

        private void TrySetStartValue<T>(KeyboardUIOptions<T> options)
        {
            switch (_usedInput)
            {
                case MultiInputType.ColorPicker when options.CurrentInputValue is Color color:
                    _colorPickerUI.StartColor(color);
                    break;
            }
        }

        private void UpdateTf(Vector3 position)
        {
            _controllerTf.position = position;

            if (_camTf == null)
                return;

            _controllerTf.LookAt(_camTf.transform);
        }

        public void UpdateInputValue(object inputValue)
        {
            OnUpdateInput?.Invoke(inputValue);
        }

        public void EnterInputValue(object inputValue)
        {
            if (inputValue is bool) // Numpad couldn't parse the float
                OnEnterInput?.Invoke(_oldInputValue);
            else
                OnEnterInput?.Invoke(inputValue);

            _isUsed = false;
            HideInputs();
        }

        public void Close()
        {
            OnEnterInput?.Invoke(_oldInputValue);

            _isUsed = false;
            HideInputs();
        }

        private void HideInputs()
        {
            _keyboardGo.SetActive(false);
            _numpadGo.SetActive(false);
            _colorpickerGo.SetActive(false);
        }

        private enum MultiInputType
        {
            Keyboard,
            Numpad,
            ColorPicker
        }
    }
}
