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

        public ImageViewWindow()
        {
            InitializeComponent();
            InitializeCallbacks();
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
                Rescale();
                ImageBox.Source = value;
                flipped = false;
                ImageBox.RenderTransform = hNormal;
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

            double width;
            double factor;
            double height;
            switch (s)
            {
                case ViewMode.FillHorizontal:
                    width = Math.Min(window.Width, image.Width);
                    factor = width / image.Width;
                    dim.Width = factor * image.Width;
                    dim.Height = Math.Min(window.Height, factor * image.Height);
                    break;
                case ViewMode.FitHorizontal:
                    width = window.Width;
                    factor = width / image.Width;
                    dim.Width = factor * image.Width;
                    dim.Height = Math.Min(window.Height, factor * image.Height);
                    break;
                case ViewMode.FitVertical:
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

        public new Window Window { get { return Window.GetWindow(this); } }


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
    }
}
