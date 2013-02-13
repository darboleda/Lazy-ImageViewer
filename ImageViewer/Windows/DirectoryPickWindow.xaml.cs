using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageViewer
{
    /// <summary>
    /// Interaction logic for DirectoryPicker.xaml
    /// </summary>
    public partial class DirectoryPickWindow : Window
    {
        DirectoryInfo startingDirectory;
        FileInfo[] files;

        string currentDirectoryName;
        string[] subDirectories;
        Stack<string> previousDirectories;
        public bool Done { get; private set; }
        void SetDirectory(string name)
        {
            if (Directory.GetDirectoryRoot(name) == name)
            {
                if (startingDirectory == null)
                    startingDirectory = new DirectoryInfo(name);
                currentDirectoryName = name;
            }
            else
            {
                if (startingDirectory == null)
                    startingDirectory = new DirectoryInfo(name);
                currentDirectoryName = name;

            }
            DirectoryInfo di = new DirectoryInfo(currentDirectoryName);
            CurrentDirName.Text = di.Name;
            subDirectories = Directory.GetDirectories(di.FullName);

            subDirectories = subDirectories.Where((s) => (new DirectoryInfo(s).Attributes & FileAttributes.System) != FileAttributes.System).ToArray();
            Array.Sort(subDirectories, new StringComparer());
            files = GetImageFiles(di.GetFiles());
            PopulateList();
        }

        public class StringComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return StringCompare.CompareStringLogical(x, y);
            }
        }

        DriveInfo[] drives;

        private FileInfo[] GetImageFiles(FileInfo[] fileInfo)
        {
            return fileInfo.Where((FileInfo file) => IsExtensionImage(file.Extension)).ToArray();
        }

        private void PopulateList()
        {
            ListBox l = DirectorySelectionList;
            l.Items.Clear();
            if (drives == null)
            {
                ListBoxItem current = MakeListItem(". (current directory)", "Images:", files.Sum((f) => ((IsExtensionImage(f.Extension) ? 1 : 0))));
                l.Items.Add(current);
                l.SelectedItem = current;
                for (int i = 0; i < subDirectories.Length; i++)
                {
                    string dir = subDirectories[i];
                    int len = 0;
                    try
                    {
                        len = Directory.GetFiles(dir).Sum((f) => ((IsExtensionImage(new FileInfo(f).Extension) ? 1 : 0)));
                    }
                    catch (Exception) { }

                    current = MakeListItem(new DirectoryInfo(dir).Name, "Images:", len);
                    l.Items.Add(current);
                    if (previousDirectories != null && previousDirectories.Count > 0 && dir == previousDirectories.Peek())
                    {
                        l.SelectedItem = current;
                        l.ScrollIntoView(l.Items[l.SelectedIndex]);
                    }
                }
            }
            else
            {
                ListBoxItem current = MakeListItem(". (current directory)", "Images:", files.Sum((f) => ((IsExtensionImage(f.Extension) ? 1 : 0))));
                for (int i = 0; i < drives.Length; i++)
                {
                    DriveInfo dir = drives[i];
                    current = MakeListItem(dir.Name, "Images:", 
                        (dir.RootDirectory.GetFiles()).Sum((f)=>((IsExtensionImage(f.Extension) ? 1 : 0))));
                    l.Items.Add(current);
                    if (previousDirectories != null && previousDirectories.Count > 0 && dir.Name == previousDirectories.Peek())
                    {
                        l.SelectedItem = current;
                        l.ScrollIntoView(l.Items[l.SelectedIndex]);
                    }
                }
            }
        }

        private ListBoxItem MakeListItem(string firstValue, string secondValue, int thirdValue)
        {
            ListBoxItem item = new ListBoxItem();
            DockPanel dock = new DockPanel();

            TextBlock first = new TextBlock();
            TextBlock second = new TextBlock();
            TextBlock third = new TextBlock();

            item.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            dock.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            first.Text = firstValue;
            second.Text = secondValue;
            third.Text = thirdValue.ToString();
            third.TextAlignment = TextAlignment.Right;
            third.Padding = second.Padding = new Thickness(5, 0, 5, 0);
            third.TextWrapping = second.TextWrapping = TextWrapping.NoWrap;

            second.Width = 45; third.Width = 35;
            first.TextTrimming = TextTrimming.CharacterEllipsis;

            DockPanel.SetDock(third, Dock.Right);
            DockPanel.SetDock(second, Dock.Right);

            dock.Children.Add(third);
            dock.Children.Add(second);
            dock.Children.Add(first);

            item.Content = dock;
            item.PreviewMouseDoubleClick += new MouseButtonEventHandler(
                (sender, args) =>
                {
                    DirectorySelectionList.SelectedItem = sender;
                    ChooseCurrentDirectory();
                });

            return item;
        }

        Window parent;
        public DirectoryPickWindow(string startingDirectory, Window parent)
        {
            InitializeComponent();
            SetDirectory(startingDirectory);
            PreviewKeyDown += DirectoryPicker_PreviewKeyDown;
            
            this.parent = parent;
            Window w = GetWindow(this);
            previousDirectories = new Stack<string>();
            w.Loaded += w_Loaded;
            Done = false;
        }

        void w_Loaded(object sender, RoutedEventArgs e)
        {
            Window w = (Window)sender;
            w.Left = parent.Left + (parent.Width - w.Width) / 2;
            w.Top = parent.Top + (parent.Height - w.Height) / 2;
        }

        void ChooseCurrentDirectory()
        {
            if (drives == null)
            {
                if (DirectorySelectionList.SelectedIndex > 0)
                {
                    string di = subDirectories[DirectorySelectionList.SelectedIndex - 1];
                    try
                    {
                        Directory.GetFiles(di);
                        if (previousDirectories.Count > 0 && di == previousDirectories.Peek())
                            previousDirectories.Pop();
                        else
                            previousDirectories.Clear();
                        SetDirectory(di);
                    }
                    catch (Exception) { }
                }
                else if (files.Length > 0)
                {
                    Close();
                    if (DirectoryOpened != null)
                    {
                        DirectoryOpened(new DirectoryInfo(currentDirectoryName), startingDirectory.FullName == currentDirectoryName, true);
                    }
                }
            }
            else
            {
                DriveInfo di = drives[DirectorySelectionList.SelectedIndex];
                if (previousDirectories.Count > 0 && di.Name == previousDirectories.Peek())
                    previousDirectories.Pop();
                else
                    previousDirectories.Clear();
                drives = null;
                SetDirectory(di.Name);
            }
        }

        void DirectoryPicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (Directory.GetParent(currentDirectoryName) != null)
                    {
                        previousDirectories.Push(currentDirectoryName);
                        SetDirectory(Directory.GetParent(currentDirectoryName).FullName);
                    }
                    else if (drives == null)
                    {
                        CurrentDirName.Text = "My Computer";
                        previousDirectories.Push(currentDirectoryName);
                        drives = DriveInfo.GetDrives();
                        drives = drives.Where((d) => d.IsReady).ToArray();
                        PopulateList();
                    }
                    break;

                case Key.Right:
                    ChooseCurrentDirectory();
                    break;
                case Key.Up:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        DirectorySelectionList.SelectedIndex = Math.Max(0, DirectorySelectionList.SelectedIndex - 10);
                    else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        DirectorySelectionList.SelectedIndex = 0;
                    else
                        DirectorySelectionList.SelectedIndex = (DirectorySelectionList.SelectedIndex - 1 + DirectorySelectionList.Items.Count) % DirectorySelectionList.Items.Count;
                        
                    DirectorySelectionList.ScrollIntoView(DirectorySelectionList.Items[DirectorySelectionList.SelectedIndex]);
                    break;

                case Key.Down:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        DirectorySelectionList.SelectedIndex = Math.Min(DirectorySelectionList.Items.Count - 1, DirectorySelectionList.SelectedIndex + 10);
                    else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        DirectorySelectionList.SelectedIndex = DirectorySelectionList.Items.Count - 1;
                    else
                        DirectorySelectionList.SelectedIndex = (DirectorySelectionList.SelectedIndex + 1) % DirectorySelectionList.Items.Count;
                    DirectorySelectionList.ScrollIntoView(DirectorySelectionList.Items[DirectorySelectionList.SelectedIndex]);
                    break;

                case Key.Escape:
                    Close();
                    DirectoryOpened(startingDirectory, true, false);
                    break;
            }
            Root.InvalidateVisual();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Done = true;
            base.OnClosing(e);
        }

        private bool IsExtensionImage(string extension) {
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

        public delegate void DirectoryHandler(DirectoryInfo info, bool sameDir, bool pickedDir);
        public event DirectoryHandler DirectoryOpened;
    }
}
