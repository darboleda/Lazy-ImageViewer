using ImageViewer.Windows.DirectoryPicker;
using ImageViewer.Windows.Loader;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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

        void OnWindowActivated(object sender, EventArgs args)
        {
            if (directoryPicker == null) return;
            if (directoryPicker.Done) return;
            Window.GetWindow(directoryPicker).Activate();
        }

        void OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                View.ToggleWindowState();
            }
        }

        void OnWindowStateChanged(object sender, EventArgs e)
        {
            View.ScrollToTop();
        }

        void OnWindowMouseLeave(object sender, MouseEventArgs e)
        {
            //mousein = false;
        }

        void OnWindowMouseEnter(object sender, MouseEventArgs e)
        {
            //mousein = true;
        }

        Point prevMousePos;
        void OnWindowMouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition((IInputElement)sender);
            if (e.LeftButton == MouseButtonState.Pressed)
                View.ScrollToRelative(-(pos.Y - prevMousePos.Y));
            prevMousePos = pos;
        }

        void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                ToggleScale();
            }
        }

        void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Matrix m = PresentationSource.FromVisual(View.Window).CompositionTarget.TransformToDevice;
            Model.TargetDpi = new Dpi() { X = m.M11 * 96, Y = m.M22 * 96 };

            appSettings = ApplicationSettings.LoadAppSettings();
            View.SetWindowDimensions(appSettings.StartingWidth,
                                     appSettings.StartingHeight,
                                     appSettings.StartingLeft,
                                     appSettings.StartingTop);

            if (App.StartupArguments.Length > 0)
            {
                Model.LoadFile(App.StartupArguments[0]);
            }
        }

        void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            appSettings.StartingWidth = ((Window)sender).Width;
            appSettings.StartingHeight = ((Window)sender).Height;
            appSettings.StartingLeft = ((Window)sender).Left;
            appSettings.StartingTop = ((Window)sender).Top;
            appSettings.SaveAppSettings();
        }

        void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            View.Rescale();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            HandleKeyDown(e.Key);
        }

        private bool HandleKeyDown(Key key)
        {
            View.FocusScroller();

            switch (key)
            {
                case Key.H:
                    View.FlipImage();
                    break;

                case Key.O:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        OpenImage();
                    else
                        OpenDirectoryPicker(0);
                    break;

                case Key.Right:
                    if ((Keyboard.Modifiers & (ModifierKeys.Control)) != 0)
                        goto case Key.End;

                    if (ImageSequence == null || !ImageSequence.Valid)
                        OpenDirectoryPicker(0);
                    else if (!ImageSequence.FindNextImage())
                        OpenDirectoryPicker(1);
                    else
                        SetImage(ImageSequence.CurrentImage);
                    break;

                case Key.Left:
                    if ((Keyboard.Modifiers & (ModifierKeys.Control)) != 0)
                        goto case Key.Home;

                    if (ImageSequence == null || !ImageSequence.Valid)
                        OpenDirectoryPicker(0);
                    else if (!ImageSequence.FindPreviousImage())
                        OpenDirectoryPicker(-1);
                    else
                        SetImage(ImageSequence.CurrentImage);
                    break;

                case Key.Home:
                    if (ImageSequence == null)
                        OpenDirectoryPicker(0);
                    else
                    {
                        ImageSequence.FindFirstImage();
                        SetImage(ImageSequence.CurrentImage);
                    }
                    break;

                case Key.End:
                    if (ImageSequence == null)
                        OpenDirectoryPicker(0);
                    else
                    {
                        ImageSequence.FindLastImage();
                        SetImage(ImageSequence.CurrentImage);
                    }
                    break;

                case Key.Space:
                    ToggleScale();
                    break;

                case Key.Enter:
                    _parent = Window.GetWindow(this);
                    _parent.WindowState = _parent.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                    break;

                case Key.Escape:
                    _parent = Window.GetWindow(this);
                    if (_parent.WindowState == WindowState.Maximized)
                        _parent.WindowState = WindowState.Normal;
                    else
                        _parent.Close();
                    break;
                case Key.Up:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                        Scroller.ScrollToTop();
                    else if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                        OpenDirectoryPicker(0);
                    else
                        Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset - 50);
                    break;
                case Key.Down:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                        Scroller.ScrollToBottom();
                    else if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                        goto case Key.Space;
                    else
                        Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset + 50);
                    break;


                default:
                    return false;
            }
            return true;
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
