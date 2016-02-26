using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ImageViewer
{
    public class ApplicationSettings
    {
        bool appSettingsChanged;

        string defaultDirectory;
        public string DefaultDirectory
        {
            get { return defaultDirectory; }
            set
            {
                defaultDirectory = value;
                appSettingsChanged = true;
            }
        }

        double width, height, left, top;
        public double StartingWidth
        {
            get { return width; }
            set
            {
                width = value;
                appSettingsChanged = true;
            }
        }
        public double StartingHeight
        {
            get { return height; }
            set
            {
                height = value;
                appSettingsChanged = true;
            }
        }
        public double StartingLeft
        {
            get { return left; }
            set
            {
                left = value;
                appSettingsChanged = true;
            }
        }
        public double StartingTop
        {
            get { return top; }
            set
            {
                top = value;
                appSettingsChanged = true;
            }
        }



        public bool SaveAppSettings()
        {
            if (appSettingsChanged)
            {
                using (StreamWriter writer = new StreamWriter(Application.LocalUserAppDataPath + @"\ImageViewer.config", false))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(ApplicationSettings));
                    xml.Serialize(writer, this);
                }
            }
            return appSettingsChanged;
        }

        public static ApplicationSettings LoadAppSettings()
        {
            FileInfo fi = null;
            try { fi = new FileInfo(Application.LocalUserAppDataPath + @"\ImageViewer.config"); }
            catch (Exception) { /* No settings? Just ignore it */ }

            if (fi == null || !fi.Exists)
            {
                ApplicationSettings s = new ApplicationSettings();
                s.DefaultDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
                s.StartingHeight = 800;
                s.StartingWidth = 600;
                s.StartingTop = 30;
                s.StartingLeft = 30;
                return s;
            }

            using (FileStream file = fi.OpenRead())
            {
                XmlSerializer xml = new XmlSerializer(typeof(ApplicationSettings));

                try
                {
                    return (ApplicationSettings)xml.Deserialize(file);
                }
                catch (Exception)
                {
                    ApplicationSettings s = new ApplicationSettings();
                    s.DefaultDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
                    s.StartingHeight = 800;
                    s.StartingWidth = 600;
                    s.StartingTop = 30;
                    s.StartingLeft = 30;
                    return s;
                }
            }
        }
    }
}
