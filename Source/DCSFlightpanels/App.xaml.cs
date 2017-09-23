﻿using System;
using System.Threading;
using System.Windows;
using NonVisuals;

namespace DCSFlightpanels
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {

                const string appName = "DCSFlightpanels.exe";
                bool createdNew;

                _mutex = new Mutex(true, appName, out createdNew);

                if (!createdNew)
                {
                    //app is already running! Exiting the application  
                    Current.Shutdown();
                    MessageBox.Show("DCSFlightpanels is already running..");
                }
                else
                {
                    base.OnStartup(e);
                }
            }
            catch (Exception ex)
            {
                Common.ShowErrorMessageBox(45454545, ex);
            }
        }
    }
}
