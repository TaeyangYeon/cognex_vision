using System.Collections.Generic;
using System.Windows.Controls;

namespace FormEvent
{
    public interface IPositionChangeEvent : IButtonEvent
    {
        void changePosition(string position, Dictionary<string, Button> buttons, Dictionary<string, ListBox> lists);
    }
}
