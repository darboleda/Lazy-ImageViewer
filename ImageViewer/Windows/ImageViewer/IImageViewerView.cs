using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ImageViewer.Windows.ImageViewer
{
    struct Dimensions
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public Dimensions(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }

    interface IImageViewerView
    {
        IImageViewerController Controller { get; set; }
        IImageViewerModel Model { get; set; }

        ImageSource Image { get; set; }
        WindowState WindowState { get; set; }

        void ScrollToTop();
        void Rescale();
        void ActivateWindow();
        void FocusScroller();
        void SetWindowDimensions(double left, double top, double width, double height);
        void FlipImage();

        void ToggleWindowState();
        void ScrollToRelative(double delta);

        Window Window { get; }
    }
}
