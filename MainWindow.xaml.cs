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
        public int i_versionsnummer = 396;

        public MainWindow()
        {
            InitializeComponent();

            string s_timeDate = DateTime.Now.ToString("dd.MM.yyyy");
            string s_hostname = Convert.ToString(Environment.MachineName);
            bottomTextblockTextboxLeft.Text = "Host: " + s_hostname;
            bottomTextblockTextboxRight.Text = s_timeDate;

            frameMainContent.Source = new Uri("DateienKopieren.xaml", UriKind.Relative); // Frame mit "DateienKopieren" page initialisieren

            //NavigationService ns = NavigationService.GetNavigationService(this);
        }
    }
}
