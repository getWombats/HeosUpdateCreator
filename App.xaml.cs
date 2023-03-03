﻿using System;
using System.Threading.Tasks;
using System.Windows;

namespace HeosUpdateCreator
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //string s_hostname = "ANDRES";
            string s_hostname = "HRDWS59";

            //splash Fenster initialisieren und als MainWindow deklarieren
            var splashScreen = new Programmstart();
            this.MainWindow = splashScreen;
            splashScreen.Show();

            if (Environment.MachineName != s_hostname)
            {
                MessageBox.Show("Das Programm kann nur auf dem Setup-Rechner gestartet werden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                splashScreen.Close();
            }

            //Task starten als delay
            Task.Factory.StartNew(() =>
            {
                //Ladevorgang simulieren
                System.Threading.Thread.Sleep(1500);

                // zum UI Thread zurückkehren
                this.Dispatcher.Invoke(() =>
                {
                    // MainWindow initialisieren und als MainWindow deklarieren
                    // splash Fenster schliessen und MainWindow anzeigen
                    var mainWindow = new MainWindow();
                    this.MainWindow = mainWindow;

                    mainWindow.Show();
                    splashScreen.Close();
                });
            });
        }
    }
}