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
        public int Width { get; set; }
        public int Height { get; set; }
        public Dimensions(int width, int height)
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

        Dimensions Dimensions { get; set; }
        void ScrollToTop();
        void Rescale();
    }
}
