using ImageViewer.Loader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace ImageViewer
{
    public partial class ImageViewWindow
    {
        void InitializeCallbacks()
        {
            Window window = Window.GetWindow(this);
            window.SizeChanged += OnWindowSizeChanged;
            window.Closing += OnWindowClosing;
            window.Loaded += OnWindowLoaded;
            window.PreviewMouseDoubleClick += OnPreviewMouseDoubleClick;
            window.PreviewMouseDown += OnWindowMouseDown;
            window.PreviewMouseMove += OnWindowMouseMove;
            window.MouseEnter += OnWindowMouseEnter;
            window.MouseLeave += OnWindowMouseLeave;
            window.StateChanged += OnWindowStateChanged;
            window.Activated += OnWindowActivated;
        }

        void OnWindowActivated(object sender, EventArgs args)
        {
            if (directoryPicker == null) return;
            if (directoryPicker.Done) return;
            GetWindow(directoryPicker).Activate();
        }

        void OnPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Window _parent = Window.GetWindow(this);
                _parent.WindowState = _parent.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }

        void OnWindowStateChanged(object sender, EventArgs e)
        {
            Scroller.ScrollToTop();
        }

        bool mousein = true;
        void OnWindowMouseLeave(object sender, MouseEventArgs e)
        {
            //mousein = false;
        }

        void OnWindowMouseEnter(object sender, MouseEventArgs e)
        {
            mousein = true;
        }

        Point prevMousePos;
        void OnWindowMouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(this);
            if (mousein && e.LeftButton == MouseButtonState.Pressed)
                Scroller.ScrollToVerticalOffset(Scroller.VerticalOffset - (pos.Y - prevMousePos.Y));
            prevMousePos = pos;
        }

        void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            //mousein = true;
            if (e.RightButton == MouseButtonState.Pressed)
            {
                ToggleScale();
            }
        }

        void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            dpi = new Dpi() { X = m.M11 * 96, Y = m.M22 * 96 };

            appSettings = ApplicationSettings.LoadAppSettings();
            ((Window)sender).Width = appSettings.StartingWidth;
            ((Window)sender).Height = appSettings.StartingHeight;
            ((Window)sender).Left = appSettings.StartingLeft;
            ((Window)sender).Top = appSettings.StartingTop;

            if (App.StartupArguments.Length > 0)
            {
                OpenImage(App.StartupArguments[0]);
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
            Size w = e.NewSize;
            if (ImageBox.Source != null)
            {
                HandleScale(ImageBox.Source);
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!HandleKeyDown(e.Key))
                base.OnPreviewKeyDown(e);
        }

        ScaleTransform hFlip = new ScaleTransform(-1, 1);
        ScaleTransform hNormal = new ScaleTransform(1, 1);
        bool flipped = false;
        private bool HandleKeyDown(Key key)
        {
            Scroller.Focus();
            Window _parent;

            switch (key)
            {
                case Key.H:
                    ImageBox.RenderTransform = (flipped ? hNormal : hFlip);
                    flipped = !flipped;
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
    }
}
