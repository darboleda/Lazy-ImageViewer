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

        private void HandleScale(ImageSource image)
        {
            Window window = Window.GetWindow(this);
            ViewMode s = Model.Mode;

            double width;
            double factor;
            double height;
            switch (s)
            {
                case ViewMode.FillHorizontal:
                    width = Math.Min(window.Width, image.Width);
                    factor = width / image.Width;
                    Scroller.Width = factor * image.Width;
                    Scroller.Height = Math.Min(window.Height, factor * image.Height);
                    break;
                case ViewMode.FitHorizontal:
                    width = window.Width;
                    factor = width / image.Width;
                    Scroller.Width = factor * image.Width;
                    Scroller.Height = Math.Min(window.Height, factor * image.Height);
                    break;
                case ViewMode.FitVertical:
                    width = Math.Min(window.Width, image.Width);
                    height = Math.Min(window.Height, image.Height);
                    factor = Math.Min(width / image.Width, height / image.Height);
                    Scroller.Height = factor * image.Height;
                    Scroller.Width = factor * image.Width;
                    break;
            }
            window.InvalidateVisual();
        }


        

        public IImageViewerController Controller
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IImageViewerModel Model
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

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
                HandleScale(value);
                ImageBox.Source = value;
                flipped = false;
                ImageBox.RenderTransform = hNormal;
                Scroller.ScrollToTop();
            }
        }

        public Dimensions Dimensions
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void ScrollToTop()
        {
            throw new NotImplementedException();
        }

        public void Rescale()
        {
            throw new NotImplementedException();
        }
    }
}
