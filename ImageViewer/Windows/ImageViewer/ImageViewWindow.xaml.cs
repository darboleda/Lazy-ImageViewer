using ImageViewer.Windows.DirectoryPicker;
using ImageViewer.Windows.Loader;
using Microsoft.Win32;
using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
namespace ImageViewer.Windows.ImageViewer
{
    /// <summary>
    /// Interaction logic for WindowBorderLess.xaml
    /// </summary>
    public partial class ImageViewWindow : IImageViewerView
    {
        private const double BASE_DPI = 96;

        ScaleTransform hFlip = new ScaleTransform(-1, 1);
        ScaleTransform hNormal = new ScaleTransform(1, 1);

        public ImageViewWindow()
        {
            InitializeComponent();

            Controller = new StandardImageViewerController();
            Model = new StandardImageViewerModel();

            Controller.View = this;
            Controller.Model = Model;
            Model.View = this;
            Model.Controller = Controller;

            InitializeCallbacks();
        }

        void InitializeCallbacks()
        {
            Window window = Window.GetWindow(this);
            window.SizeChanged += Controller.OnWindowSizeChanged;
            window.Closing += Controller.OnWindowClosing;
            window.Loaded += Controller.OnWindowLoaded;
            window.PreviewMouseDoubleClick += Controller.OnPreviewMouseDoubleClick;
            window.PreviewMouseDown += Controller.OnWindowMouseDown;
            window.PreviewMouseMove += Controller.OnWindowMouseMove;
            window.MouseEnter += Controller.OnWindowMouseEnter;
            window.MouseLeave += Controller.OnWindowMouseLeave;
            window.StateChanged += Controller.OnWindowStateChanged;
            window.Activated += Controller.OnWindowActivated;
            window.PreviewKeyDown += Controller.OnPreviewKeyDown;
        }


        public IImageViewerController Controller { get; set; }
        public IImageViewerModel Model { get; set; }

        public ImageSource Image
        {
            get
            {
                return ImageBox.Source;
            }
            set
            {
                Window w = Window.GetWindow(this);
                Scroller.Visibility = System.Windows.Visibility.Visible;
                ImageBox.Source = value;
                ImageBox.RenderTransform = hNormal;
                Rescale();
                Scroller.ScrollToTop();
            }
        }

        public void ScrollToTop()
        {
            Scroller.ScrollToTop();
        }

        private Dimensions CalculateDimensions()
        {
            Window window = Window;
            ImageSource image = Image;
            ViewMode s = Model.Mode;

            Dimensions dim = new Dimensions();
            if (image == null) return dim;

            double width;
            double factor;
            double height;
            switch (s)
            {
                case ViewMode.FitHorizontal:
                    width = Math.Min(window.Width, image.Width);
                    factor = width / image.Width;
                    dim.Width = factor * image.Width;
                    dim.Height = Math.Min(window.Height, factor * image.Height);
                    break;
                case ViewMode.FillHorizontal:
                    width = window.Width;
                    factor = width / image.Width;
                    dim.Width = factor * image.Width;
                    dim.Height = Math.Min(window.Height, factor * image.Height);
                    break;
                case ViewMode.FitBoth:
                    width = Math.Min(window.Width, image.Width);
                    height = Math.Min(window.Height, image.Height);
                    factor = Math.Min(width / image.Width, height / image.Height);
                    dim.Height = factor * image.Height;
                    dim.Width = factor * image.Width;
                    break;
            }
            return dim;
        }


        public void ActivateWindow() { Window.Activate(); }

        public Window Window { get { return Window.GetWindow(this); } }

        public void Rescale()
        {
            Dimensions dim = CalculateDimensions();
            Scroller.Width = dim.Width;
            Scroller.Height = dim.Height;
            Window.InvalidateVisual();
        }


        public void ToggleWindowState()
        {
            Window window = Window;
            window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }


        public void ScrollToBottom()
        {
            Scroller.ScrollToBottom();
        }

        public void FocusScroller()
        {
            Scroller.Focus();
        }

        public void SetWindowDimensions(double left, double top, double width, double height)
        {
            Window window = Window;
            window.Left = left;
            window.Top = top;
            window.Width = width;
            window.Height = height;
        }

        public void FlipImage()
        {
            ImageBox.RenderTransform = (ImageBox.RenderTransform.Equals(hNormal) ? hFlip : hNormal);
        }

        public void ScrollToRelative(double delta)
        {
            Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset + delta);
        }

        public void CloseWindow()
        {
            Window.Close();
        }
    }
}
