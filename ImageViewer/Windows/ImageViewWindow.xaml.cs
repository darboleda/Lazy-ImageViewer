using ImageViewer.Loader;
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
namespace ImageViewer
{
    /// <summary>
    /// Interaction logic for WindowBorderLess.xaml
    /// </summary>
    public partial class ImageViewWindow
    {
        private const double BASE_DPI = 96;

        ApplicationSettings appSettings;
        DirectoryPickWindow directoryPicker;
        IImageSequence imageSequence;
        IImageSequence ImageSequence
        {
            get { return imageSequence; }
            set
            {
                imageSequence = value;
                appSettings.DefaultDirectory = imageSequence.Directory.FullName;
            }
        }
        Dpi dpi;

        int maximizedScaleState;
        int regularScaleState;
        public ImageViewWindow()
        {
            directoryPicker = null;
            InitializeComponent();
            InitializeCallbacks();
            maximizedScaleState = 0;
            regularScaleState = 1;
            imageSequence = null;
        }

        private void HandleScale(ImageSource image)
        {
            Window window = Window.GetWindow(this);
            int s = (window.WindowState == System.Windows.WindowState.Maximized ? maximizedScaleState : regularScaleState);

            double width;
            double factor;
            double height;
            switch (s)
            {
                case 0:
                    width = Math.Min(window.Width, image.Width);
                    factor = width / image.Width;
                    Scroller.Width = factor * image.Width;
                    Scroller.Height = Math.Min(window.Height, factor * image.Height);
                    break;
                case 1:
                    width = window.Width;
                    factor = width / image.Width;
                    Scroller.Width = factor * image.Width;
                    Scroller.Height = Math.Min(window.Height, factor * image.Height);
                    break;
                case 2:
                    width = Math.Min(window.Width, image.Width);
                    height = Math.Min(window.Height, image.Height);
                    factor = Math.Min(width / image.Width, height / image.Height);
                    Scroller.Height = factor * image.Height;
                    Scroller.Width = factor * image.Width;
                    break;
            }
            window.InvalidateVisual();
        }

        private void ToggleScale()
        {
            if (ImageBox.Source == null) return;
            if (GetWindow(this).WindowState == System.Windows.WindowState.Maximized)
                maximizedScaleState = (maximizedScaleState + 1) % 3;
            else
                regularScaleState = (regularScaleState + 1) % 3;
            Scroller.ScrollToTop();
            HandleScale(ImageBox.Source);
        }

        private void SetImage(ImageSource image)
        {
            Window w = Window.GetWindow(this);
            Scroller.Visibility = System.Windows.Visibility.Visible;
            HandleScale(image);
            ImageBox.Source = image;
            flipped = false;
            ImageBox.RenderTransform = hNormal;
            Scroller.ScrollToTop();
        }

        private void OpenImage(string fullName)
        {
            DirectoryImageSequence s = new DirectoryImageSequence(Directory.GetParent(fullName));
            s.TargetDpi = dpi;
            if (!s.Equals(ImageSequence))
                ImageSequence = s;
            ImageSequence.FindFileByName(fullName);
            SetImage(ImageSequence.CurrentImage);
        }

        private void OpenImage()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = appSettings.DefaultDirectory;
            FileInfo currentFile;
            if (ImageSequence != null && (currentFile = ImageSequence.CurrentFile) != null)
                dlg.FileName = currentFile.Name;
            dlg.Filter = "Image Files (.jpg, .png, .bmp)|*.jpg;*.png;*.jpeg;*.bmp";
            
            bool? result = dlg.ShowDialog();
            if (result.GetValueOrDefault())
            {
                if (ImageSequence == null || ImageSequence.CurrentFile.FullName != dlg.FileName)
                {
                    DirectoryImageSequence s = new DirectoryImageSequence(Directory.GetParent(dlg.FileName));
                    s.TargetDpi = dpi;
                    if (!s.Equals(ImageSequence))
                        ImageSequence = s;
                    ImageSequence.FindFileByName(dlg.FileName);
                }
                SetImage(ImageSequence.CurrentImage);
            }
        }

        void OpenDirectoryPicker(int direction)
        {
            if (directoryPicker != null && !directoryPicker.Done) return;

            DirectoryPickWindow dp = new DirectoryPickWindow(appSettings.DefaultDirectory, GetWindow(this));
            dp.DirectoryOpened += 
                (DirectoryInfo info, bool sameDir, bool pickedDir) => OnDirectoryOpened(info, sameDir, pickedDir, direction);
            dp.Show();
            directoryPicker = dp;
        }

        void OnDirectoryOpened(DirectoryInfo info, bool sameDir, bool pickedDir, int direction)
        {
            directoryPicker = null;
            if (sameDir && ImageSequence != null && ImageSequence.Valid)
            {
                if (direction > 0) ImageSequence.FindFirstImage();
                if (direction < 0) ImageSequence.FindLastImage();
            }
            else if (pickedDir)
            {
                ImageSequence = new DirectoryImageSequence(info.FullName);
                ImageSequence.TargetDpi = dpi;
                ImageSequence.FindFirstImage();
            }
            
            if (ImageSequence != null)
                SetImage(ImageSequence.CurrentImage);
            GetWindow(this).Activate();
        }
    }
}
