using System.Windows;
using System.Windows.Input;

namespace Typoetry_WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            EventManager.RegisterClassHandler(typeof(UIElement), Keyboard.PreviewKeyDownEvent, new KeyEventHandler(GlobalPreviewKeyDownHandler));
            EventManager.RegisterClassHandler(typeof(UIElement), DragDrop.PreviewDragOverEvent, new DragEventHandler(GlobalPreviewDragOverHandler));
            EventManager.RegisterClassHandler(typeof(UIElement), DragDrop.PreviewDropEvent, new DragEventHandler(GlobalPreviewDropHandler));
        }

        private void GlobalPreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
            }
        }

        private void GlobalPreviewDragOverHandler(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void GlobalPreviewDropHandler(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}
