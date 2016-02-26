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
        public IImageViewerModel Model { get; set; }
        public IImageViewerView View { get; set; }

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

        public void OpenDirectoryPicker(int direction)
        {
            if (directoryPicker != null && !directoryPicker.Done) return;

            DirectoryPickWindow dp = new DirectoryPickWindow(appSettings.DefaultDirectory, View.Window);
            dp.DirectoryOpened +=
                (DirectoryInfo info, bool sameDir, bool pickedDir) => OnDirectoryOpened(info, sameDir, pickedDir, direction);
            dp.Show();
            directoryPicker = dp;
        }

        public void OnDirectoryOpened(DirectoryInfo info, bool sameDir, bool pickedDir, int direction)
        {
            directoryPicker = null;
            if (!sameDir || (direction == 0 && pickedDir))
            {
                Model.Sequence = new DirectoryImageSequence(info.FullName);
                Model.SeekFirstImage();
            }
            else if (pickedDir)
            {
                if (direction > 0) Model.SeekFirstImage();
                else if (direction < 0) Model.SeekLastImage();
            }
            appSettings.DefaultDirectory = info.FullName;
            View.Image = Model.Image;
            View.ActivateWindow();
            View.Rescale();
        }

        public void OnWindowActivated(object sender, EventArgs args)
        {
            if (directoryPicker == null) return;
            if (directoryPicker.Done) return;
            Window.GetWindow(directoryPicker).Activate();
        }

        public void OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                View.ToggleWindowState();
            }
        }

        public void OnWindowStateChanged(object sender, EventArgs e)
        {
            View.ScrollToTop();
        }

        public void OnWindowMouseLeave(object sender, MouseEventArgs e)
        {
            //mousein = false;
        }

        public void OnWindowMouseEnter(object sender, MouseEventArgs e)
        {
            //mousein = true;
        }

        Point prevMousePos;
        public void OnWindowMouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition((IInputElement)sender);
            if (e.LeftButton == MouseButtonState.Pressed)
                View.ScrollToRelative(-(pos.Y - prevMousePos.Y));
            prevMousePos = pos;
        }

        public void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                ToggleScale();
            }
        }

        public void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Matrix m = PresentationSource.FromVisual(View.Window).CompositionTarget.TransformToDevice;
            Model.TargetDpi = new Dpi() { X = m.M11 * 96, Y = m.M22 * 96 };

            appSettings = ApplicationSettings.LoadAppSettings();
            View.SetWindowDimensions(appSettings.StartingLeft,
                                     appSettings.StartingTop,
                                     appSettings.StartingWidth,
                                     appSettings.StartingHeight);

            if (App.StartupArguments.Length > 0)
            {
                Model.LoadFile(App.StartupArguments[0]);
            }
        }

        public void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            appSettings.StartingWidth = ((Window)sender).Width;
            appSettings.StartingHeight = ((Window)sender).Height;
            appSettings.StartingLeft = ((Window)sender).Left;
            appSettings.StartingTop = ((Window)sender).Top;
            appSettings.SaveAppSettings();
        }

        public void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            View.Rescale();
        }

        public void OnPreviewKeyDown(object sender, KeyEventArgs e)
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

                    if (!Model.SequenceReady)
                        OpenDirectoryPicker(0);
                    else if (!Model.SeekNextImage())
                        OpenDirectoryPicker(1);
                    else
                        View.Image = Model.Image;
                    break;

                case Key.Left:
                    if ((Keyboard.Modifiers & (ModifierKeys.Control)) != 0)
                        goto case Key.Home;

                    if (!Model.SequenceReady)
                        OpenDirectoryPicker(0);
                    else if (!Model.SeekPreviousImage())
                        OpenDirectoryPicker(-1);
                    else
                        View.Image = Model.Image;
                    break;

                case Key.Home:
                    if (!Model.SequenceReady || !Model.SeekFirstImage())
                        OpenDirectoryPicker(0);
                    else
                        View.Image = Model.Image;
                    break;

                case Key.End:
                    if (!Model.SequenceReady || !Model.SeekLastImage())
                        OpenDirectoryPicker(0);
                    else
                        View.Image = Model.Image;
                    break;

                case Key.Space:
                    ToggleScale();
                    break;

                case Key.Enter:
                    View.ToggleWindowState();
                    break;

                case Key.Escape:
                    if (View.WindowState == WindowState.Maximized)
                        View.ToggleWindowState();
                    else
                        View.CloseWindow();
                    break;

                case Key.Up:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                        View.ScrollToTop();
                    else if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                        OpenDirectoryPicker(0);
                    else
                        View.ScrollToRelative(-50);
                    break;
                case Key.Down:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
                        View.ScrollToBottom();
                    else if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                        goto case Key.Space;
                    else
                        View.ScrollToRelative(50);
                    break;


                default:
                    return false;
            }
            return true;
        }


        public void OnImageChanged(System.Windows.Media.ImageSource previousImage, System.Windows.Media.ImageSource newImage)
        {
            View.Image = newImage;
        }

        public void OnSequenceChanged(Loader.IImageSequence previousSequence, Loader.IImageSequence newSequence)
        {
            appSettings.DefaultDirectory = newSequence.Directory.FullName;
        }
    }
}
