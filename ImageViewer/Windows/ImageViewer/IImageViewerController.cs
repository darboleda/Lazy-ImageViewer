using ImageViewer.Windows.Loader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageViewer.Windows.ImageViewer
{
    public interface IImageViewerController
    {
        IImageViewerModel Model { get; set; }
        IImageViewerView View { get; set; }

        void OnWindowLoaded(object sender, RoutedEventArgs e);
        void OnWindowClosing(object sender, CancelEventArgs e);
        void OnWindowActivated(object sender, EventArgs args);
        void OnWindowStateChanged(object sender, EventArgs e);
        void OnWindowSizeChanged(object sender, SizeChangedEventArgs e);

        void OnPreviewKeyDown(object sender, KeyEventArgs e);
        
        void OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e);
        void OnWindowMouseDown(object sender, MouseButtonEventArgs e);
        void OnWindowMouseMove(object sender, MouseEventArgs e);
        void OnWindowMouseEnter(object sender, MouseEventArgs e);
        void OnWindowMouseLeave(object sender, MouseEventArgs e);

        void OnImageChanged(ImageSource previousImage, ImageSource newImage);
        void OnSequenceChanged(IImageSequence previousSequence, IImageSequence newSequence);
    }
}
