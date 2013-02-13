using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ImageViewer.Loader
{
    public struct Dpi
    {
        public double X, Y;
    }

    public interface IImageSequence
    {
        DirectoryInfo Directory { get; }
        FileInfo CurrentFile { get; }
        ImageSource CurrentImage { get; }
        bool FindFileByName(string fullName);
        bool FindNextImage();
        bool FindPreviousImage();
        bool FindLastImage();
        bool FindFirstImage();
        Dpi TargetDpi { get; set; }
        bool Valid { get; }
    }
}
