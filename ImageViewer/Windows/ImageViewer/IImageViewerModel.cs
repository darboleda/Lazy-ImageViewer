using ImageViewer.Windows.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ImageViewer.Windows.ImageViewer
{
    public enum ViewMode
    {
        FillHorizontal = 0,
        FitHorizontal = 1,
        FitBoth = 2
    }

    public interface IImageViewerModel
    {
        IImageViewerController Controller { get; set; }
        IImageViewerView View { get; set; }

        WindowState WindowState { get; set; }
        ViewMode Mode { get; set; }
        ViewMode IncrementViewMode();

        IImageSequence Sequence { get; set; }
        ImageSource Image { get; set; }
        Dpi TargetDpi { get; set; }

        bool SequenceReady { get; }

        bool SeekLastImage();
        bool SeekFirstImage();
        bool SeekPreviousImage();
        bool SeekNextImage();

        String FileName { get; }
        void LoadFile(String path);
    }
}
