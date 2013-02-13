using ImageViewer.Windows.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ImageViewer.Windows.ImageViewer
{
    enum ViewMode
    {
        FillHorizontal = 0,
        FitHorizontal = 1,
        FitVertical = 2
    }

    interface IImageViewerModel
    {
        IImageViewerController Controller { get; set; }
        IImageViewerView View { get; set; }

        WindowState WindowState { get; set; }
        ViewMode Mode { get; set; }
        ViewMode IncrementViewMode();

        IImageSequence Sequence { get; set; }
        ImageSource Image { get; set; }
        Dpi TargetDpi { get; set; }

        bool SeekLastImage();
        bool SeekFirstImage();

        String FileName { get; }
        void LoadFile(String path);
    }
}
