using ImageViewer.Windows.DirectoryPicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageViewer.Windows.ImageViewer
{
    class StandardImageViewerController : IImageViewerController
    {
        IImageViewerModel Model { get; set; }
        IImageViewerView View { get; set; }

        ApplicationSettings appSettings;
        DirectoryPickWindow directoryPicker;

        public StandardImageViewerController()
        {
            appSettings = null;
            directoryPicker = null;
        }

        private void ToggleScale()
        {
            if (Model.Image == null) return;

            View.ScrollToTop();
            View.Rescale();
        }



        public void OnWindowLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnWindowActivated(object sender, EventArgs args)
        {
            throw new NotImplementedException();
        }

        public void OnWindowStateChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnWindowSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnPreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnWindowMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnWindowMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnWindowMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnWindowMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            throw new NotImplementedException();
        }


        public void OnImageChanged(System.Windows.Media.ImageSource previousImage, System.Windows.Media.ImageSource newImage)
        {
            throw new NotImplementedException();
        }

        public void OnSequenceChanged(Loader.IImageSequence previousSequence, Loader.IImageSequence newSequence)
        {
            appSettings.DefaultDirectory = newSequence.Directory.FullName;
        }
    }
}
