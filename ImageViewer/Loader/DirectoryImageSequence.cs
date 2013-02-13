using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageViewer.Loader
{
    public class DirectoryImageSequence : IImageSequence
    {
        DirectoryInfo directory = null;
        FileInfo[] files = null;
        int? fileIndex = null;
        BitmapSource current;
        
        public DirectoryImageSequence(string directoryName)
        {
            directory = new DirectoryInfo(directoryName);
            files = (from file in directory.GetFiles() where IsExtensionImage(file.Extension) select file).ToArray();
            Array.Sort(files, new FileSorter());
            FindFirstImage();
        }

        public DirectoryImageSequence(DirectoryInfo directoryInfo)
        {
            directory = directoryInfo;
            files = (from file in directory.GetFiles() where IsExtensionImage(file.Extension) select file).ToArray();
            Array.Sort(files, new FileSorter());
            FindFirstImage();
        }

        private class FileSorter : IComparer<FileInfo>
        {
            public int Compare(FileInfo x, FileInfo y)
            {
                return StringCompare.CompareStringLogical(x.Name, y.Name);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is DirectoryImageSequence && ((DirectoryImageSequence)obj).directory.FullName == directory.FullName;
        }

        public override int GetHashCode()
        {
            return directory.GetHashCode();
        }

        private BitmapSource AttemptLoadImage(FileInfo file)
        {
            if (!IsExtensionImage(file.Extension)) return null;
            try { return ConvertBitmapTo96DPI(new BitmapImage(new Uri(file.FullName))); }
            catch (Exception) { return null; }
        }

        private BitmapSource ConvertBitmapTo96DPI(BitmapImage bitmapImage)
        {
            if (bitmapImage.DpiX <= TargetDpi.X && bitmapImage.DpiY <= TargetDpi.Y)
                return bitmapImage;

            int width = bitmapImage.PixelWidth;
            int height = bitmapImage.PixelHeight;

            return new TransformedBitmap(bitmapImage, new ScaleTransform(bitmapImage.DpiX / TargetDpi.X, bitmapImage.DpiY / TargetDpi.Y));
        }

        private bool IsExtensionImage(string extension)
        {
            switch (extension.ToLowerInvariant())
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".gif":
                    return true;
            }
            return false;
        }

        private int? FindFile(ref BitmapSource image, int startingIndex = 0, bool goBackward = false, bool wrapAround = false)
        {
            BitmapSource temp = null;
            for (int i = startingIndex; (goBackward ? i >= 0 : i < files.Length); i += (goBackward ? -1 : 1))
            {
                if ((temp = AttemptLoadImage(files[i])) != null)
                {
                    image = temp;
                    return i;
                }
            }
            if (wrapAround)
            {
                for (int i = (!goBackward ? 0 : files.Length - 1); i != startingIndex; i += (goBackward ? -1 : 1))
                {
                    if ((temp = AttemptLoadImage(files[i])) != null)
                    {
                        image = temp;
                        return i;
                    }
                }
            }
            return null;
        }

        public ImageSource CurrentImage
        {
            get { return current; }
        }

        public bool FindNextImage()
        {
            fileIndex = FindFile(ref current, fileIndex.Value + 1, false, false);
            return fileIndex != null && fileIndex.HasValue;
        }

        public bool FindPreviousImage()
        {
            fileIndex = FindFile(ref current, fileIndex.Value - 1, true, false);
            return fileIndex != null && fileIndex.HasValue;
        }

        public bool FindLastImage()
        {
            fileIndex = FindFile(ref current, files.Length - 1, true, false);
            return fileIndex != null && fileIndex.HasValue;
        }

        public bool FindFirstImage()
        {
            fileIndex = FindFile(ref current, 0, false, false);
            return fileIndex != null && fileIndex.HasValue;
        }

        public Dpi TargetDpi { get; set; }


        public bool FindFileByName(string fullName)
        {
            int i;
            for (i = 0; files[i].FullName != fullName; i++)
                if (i >= files.Length) return false;

            fileIndex = FindFile(ref current, i, false, true);
            return fileIndex != null && fileIndex.HasValue;
        }

        public bool Valid
        {
            get { return files.Length > 0; }
        }

        public FileInfo CurrentFile
        {
            get {
                if (files == null || fileIndex == null || !fileIndex.HasValue) return null;
                return files[fileIndex.Value];
            }
        }

        public DirectoryInfo Directory
        {
            get {
                return directory;
            }
        }
    }
}
