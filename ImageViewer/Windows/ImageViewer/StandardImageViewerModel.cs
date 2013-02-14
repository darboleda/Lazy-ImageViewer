using ImageViewer.Windows.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ImageViewer.Windows.ImageViewer
{
    class StandardImageViewerModel : IImageViewerModel
    {
        ViewMode maximizedScaleState;
        ViewMode regularScaleState;

        public WindowState WindowState { get; set; }

        public IImageViewerController Controller { get; set; }
        public IImageViewerView View { get; set; }
        public Dpi TargetDpi { get; set; }

        IImageSequence imageSequence;
        public IImageSequence Sequence
        {
            get { return imageSequence; }
            set
            {
                IImageSequence prev = imageSequence;
                imageSequence = value;
                imageSequence.TargetDpi = TargetDpi;
            }
        }

        public StandardImageViewerModel()
        {
            maximizedScaleState = ViewMode.FitHorizontal;
            regularScaleState = ViewMode.FillHorizontal;
            imageSequence = null;
        }

        public void LoadFile(String path)
        {
            if (Sequence == null || Sequence.CurrentFile.FullName != path)
            {
                DirectoryImageSequence s = new DirectoryImageSequence(Directory.GetParent(path));
                s.TargetDpi = TargetDpi;
                if (!s.Equals(Sequence))
                    Sequence = s;
                Sequence.FindFileByName(path);
                Image = Sequence.CurrentImage;
            }
        }

        public ViewMode IncrementViewMode()
        {
            return (Mode = (ViewMode)(((int)Mode + 1) % Enum.GetNames(typeof(ViewMode)).Length));
        }

        private ImageSource image;
        public ImageSource Image
        {
            get
            {
                return image;
            }
            set
            {
                ImageSource prev = image;
                image = value;
            }
        }

        public ViewMode Mode
        {
            get
            {
                return (View.WindowState == WindowState.Maximized ? maximizedScaleState : regularScaleState);
            }
            set
            {
                switch (View.WindowState)
                {
                    case WindowState.Maximized:
                        maximizedScaleState = value;
                        break;

                    case WindowState.Normal:
                        regularScaleState = value;
                        break;
                }
            }
        }

        public bool SequenceReady
        {
            get { return Sequence != null && Sequence.Valid; }
        }

        public bool SeekLastImage()
        {
            if (Sequence.FindLastImage())
            {
                Image = Sequence.CurrentImage;
                return true;
            }
            return false;
        }

        public bool SeekFirstImage()
        {
            if (Sequence.FindFirstImage())
            {
                Image = Sequence.CurrentImage;
                return true;
            }
            return false;
        }

        public bool SeekPreviousImage()
        {
            if (Sequence.FindPreviousImage())
            {
                Image = Sequence.CurrentImage;
                return true;
            }
            return false;
        }

        public bool SeekNextImage()
        {
            if (Sequence.FindNextImage())
            {
                Image = Sequence.CurrentImage;
                return true;
            }
            return false;
        }

        public string FileName
        {
            get { return Sequence == null || Sequence.CurrentFile == null ? null : Sequence.CurrentFile.FullName; }
        }
    }
}
