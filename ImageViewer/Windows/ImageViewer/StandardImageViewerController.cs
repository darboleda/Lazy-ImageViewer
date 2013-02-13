using ImageViewer.Windows.DirectoryPicker;
using ImageViewer.Windows.Loader;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
            Model.IncrementViewMode();
            View.ScrollToTop();
            View.Rescale();
        }

        private void OpenImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = appSettings.DefaultDirectory;
            String name = Model.FileName;
            if (name != null) dlg.FileName = name;
            dlg.Filter = "Image Files (.jpg, .png, .bmp)|*.jpg;*.png;*.jpeg;*.bmp";

            bool? result = dlg.ShowDialog();
            if (result.GetValueOrDefault())
            {
                Model.LoadFile(dlg.FileName);
                View.Image = Model.Image;
            }
        }

        void OpenDirectoryPicker(int direction)
        {
            if (directoryPicker != null && !directoryPicker.Done) return;

            DirectoryPickWindow dp = new DirectoryPickWindow(appSettings.DefaultDirectory, View.Window);
            dp.DirectoryOpened +=
                (DirectoryInfo info, bool sameDir, bool pickedDir) => OnDirectoryOpened(info, sameDir, pickedDir, direction);
            dp.Show();
            directoryPicker = dp;
        }

        void OnDirectoryOpened(DirectoryInfo info, bool sameDir, bool pickedDir, int direction)
        {
            directoryPicker = null;
            if (!(sameDir && (direction == 0 || (direction > 0 ? Model.SeekFirstImage() : Model.SeekLastImage()))) && pickedDir)
            {
                Model.Sequence = new DirectoryImageSequence(info.FullName);
                Model.SeekFirstImage();
            }

            View.Image = Model.Image;
            View.ActivateWindow();
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
