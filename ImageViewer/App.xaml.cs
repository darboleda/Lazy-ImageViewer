﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ImageViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static String[] StartupArguments { get; private set; }
        private void App_Startup(object sender, StartupEventArgs e)
        {
            StartupArguments = e.Args;
        }
    }
}
