﻿using ImageViewer.Windows.Loader;
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

        WindowState WindowState { get; set; }

        public IImageViewerController Controller { get; set; }
        public IImageViewerView View { get; set; }
        public Dpi TargetDpi { get; set; }

        IImageSequence imageSequence;
        IImageSequence ImageSequence
        {
            get { return imageSequence; }
            set
            {
                IImageSequence prev = imageSequence;
                imageSequence = value;
                Controller.OnSequenceChanged(prev, imageSequence);
            }
        }

        public StandardImageViewerModel()
        {
            maximizedScaleState = ViewMode.FitHorizontal;
            regularScaleState = ViewMode.FitHorizontal;
            imageSequence = null;
        }

        public ViewMode GetMode(System.Windows.WindowState windowState)
        {
            throw new NotImplementedException();
        }

        public ViewMode IncrementViewMode()
        {
            if (View.WindowState == System.Windows.WindowState.Maximized)
            {
                maximizedScaleState = (ViewMode)(((int)maximizedScaleState + 1) % Enum.GetNames(typeof(ViewMode)).Length);
                return maximizedScaleState;
            }
            else
            {
                regularScaleState = (ViewMode)(((int)regularScaleState + 1) % Enum.GetNames(typeof(ViewMode)).Length);
                return regularScaleState;
            }
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
                
                Controller.OnImageChanged(prev, image);
            }
        }

        private void OpenImage(string fullName)
        {
            DirectoryImageSequence s = new DirectoryImageSequence(Directory.GetParent(fullName));
            s.TargetDpi = TargetDpi;
            if (!s.Equals(ImageSequence))
                ImageSequence = s;
            ImageSequence.FindFileByName(fullName);
            Image = ImageSequence.CurrentImage;
        }
    }
}