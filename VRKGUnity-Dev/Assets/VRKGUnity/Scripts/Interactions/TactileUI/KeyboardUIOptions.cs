using UnityEngine;

namespace AIDEN.TactileUI
{
    public class KeyboardUIOptions<T>
    {
        public Vector3 Position { get; private set; }
        public InputAlignment Alignment { get; private set; }

        public UpdateInput UpdateInput { get; private set; }
        public EnterInput EnterInput { get; private set; }

        public T CurrentInputValue { get; private set; }

        public KeyboardUIOptions(Vector3 position, InputAlignment alignment, UpdateInput updateInput, EnterInput enterInput, T currentInputValue)
        {
            Position = position;
            Alignment = alignment;
            UpdateInput = updateInput;
            EnterInput = enterInput;
            CurrentInputValue = currentInputValue;
        }
    }
}
