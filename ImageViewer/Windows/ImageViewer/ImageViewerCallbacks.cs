using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ImageViewer.Windows.Loader;

namespace ImageViewer.Windows.ImageViewer
{
    public partial class ImageViewWindow : Window
    {
        void InitializeCallbacks()
        {
            Window window = Window.GetWindow(this);
            window.SizeChanged += OnWindowSizeChanged;
            window.Closing += OnWindowClosing;
            window.Loaded += OnWindowLoaded;
            window.PreviewMouseDoubleClick += OnPreviewMouseDoubleClick;
            window.PreviewMouseDown += OnWindowMouseDown;
            window.PreviewMouseMove += OnWindowMouseMove;
            window.MouseEnter += OnWindowMouseEnter;
            window.MouseLeave += OnWindowMouseLeave;
            window.StateChanged += OnWindowStateChanged;
            window.Activated += OnWindowActivated;
        }

        
    }
}
