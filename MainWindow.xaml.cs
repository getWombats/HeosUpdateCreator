using System;
using System.Windows;
using System.Windows.Navigation;

namespace HeosUpdateCreator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            // Frame mit "DateienKopieren" Page initialisieren
            frameMainContent.Source = new Uri("DateienKopieren.xaml", UriKind.Relative);
        }
    }
}
