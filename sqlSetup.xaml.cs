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
            if (!NavigationService.CanGoForward)
            {
                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new Uri("Page3.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.GoForward();
            }
            var window = Application.Current.MainWindow;
            (window as MainWindow).labelReihenfolgeChecked_2.Visibility = Visibility.Visible;
        }

        private void buttonZurueck_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}