using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace HeosUpdateCreator
{
    /// <summary>
    /// Interaktionslogik für sql.xaml
    /// </summary>
    public partial class sql : Page
    {
        public sql()
        {
            InitializeComponent();
        }

        private void buttonWeiter_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService ns = NavigationService.GetNavigationService(this);
            //ns.Navigate(new Uri("WEITERE_SEITEN.xaml", UriKind.Relative)); // weitere Seite einfügen
        }

        private void buttonZurueck_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("DateienKopieren.xaml", UriKind.Relative));
        }
    }
}
