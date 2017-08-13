using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicsLayer
{
    public interface ICanvas : IProduceMouseEvents
    {
        void SetClipboardText(string text);
        string GetClipboardText();
        bool IsControlKeyDown { get; }
        IEnumerable<object> SelectedObjects { set; }
        void Invalidate();
        string Title { set; }
    }
}
