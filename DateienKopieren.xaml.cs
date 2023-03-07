using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace HeosUpdateCreator
{
    /// <summary>
    /// Interaktionslogik für DateienKopieren.xaml
    /// </summary>
    public partial class DateienKopieren : Page
    {
        private static string s_sourcePath = @"O:\ENT\ENT Soria\Temp\Quellverzeichnis";
        private static string s_targetPath = @"C:\Zielverzeichnis";

        // Backgroundworker initialisieren
        private BackgroundWorker worker = null;

        #region Main

        public DateienKopieren() // Main
        {
            InitializeComponent();

            DateiInfoQuellpfad();
            Check_Verzeichnispfade();
        }

        #endregion Main

        #region Buttons

        private void buttonWeiter_Click(object sender, RoutedEventArgs e)
        {
            if (!NavigationService.CanGoForward)
            {
                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new Uri("sqlSetup.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.GoForward();
            }
        }

        private void buttonKopieren_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(s_sourcePath) && Directory.Exists(s_targetPath))
            {
                MessageBoxResult msgRes = MessageBox.Show("Alle Dateien im Verzeichnis [ " + s_targetPath + " ] werden gelöscht!", "Achtung", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (msgRes == MessageBoxResult.OK)
                {
                    Worker_Initialisieren();

                    if (worker.IsBusy != true)
                    {
                        Labelstyle_Kopiervorgang_start();

                        worker.RunWorkerAsync();
                    }
                    else
                    {
                        MessageBox.Show("Backgroundworker-Thread ist noch aktiv");
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void buttonAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            worker.CancelAsync();
        }

        #endregion Buttons

        #region Grafik (Labelaenderungen)

        private void DateiInfoQuellpfad()
        {
            int totalFiles = Directory.GetFiles(s_sourcePath, "*.*", SearchOption.AllDirectories).Count();
            int totalDirectories = Directory.GetDirectories(s_sourcePath, "*", SearchOption.AllDirectories).Count();

            labelCountFiles.Content = totalFiles + " Dateien, " + totalDirectories + " Ordner";

            long size = 0;
            DirectoryInfo dir = new DirectoryInfo(s_sourcePath);
            foreach (FileInfo fi in dir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                size += fi.Length;
            }
            labelFileSize.Content = "Grösse: " + Math.Round(size / Math.Pow(1024, 2), MidpointRounding.ToEven).ToString() + " MB" + " " + "(" + size.ToString() + " Bytes)";
        }

        private void DateiInfoZielpfad()
        {
            int totalFiles = Directory.GetFiles(s_targetPath, "*.*", SearchOption.AllDirectories).Count();
            int totalDirectories = Directory.GetDirectories(s_targetPath, "*", SearchOption.AllDirectories).Count();

            labelCountFilesTargetPath.Content = totalFiles + " Dateien, " + totalDirectories + " Ordner";

            long size = 0;
            DirectoryInfo dir = new DirectoryInfo(s_targetPath);
            foreach (FileInfo fi in dir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                size += fi.Length;
            }
            labelFileSizeTargetPath.Visibility = Visibility.Visible;
            labelFileSizeTargetPath.Content = "Grösse: " + Math.Round(size / Math.Pow(1024, 2), MidpointRounding.ToEven).ToString() + " MB" + " " + "(" + size.ToString() + " Bytes)";
        }

        private void Labelstyle_Verzeichnis_loeschen()
        {
            labelCopyInProgress.Visibility = Visibility.Visible;
            labelCopyInProgress.Content = "Verzeichnisinhalt wird gelöscht...";
            copyProgressBar.Visibility = Visibility.Visible;
            copyProgressBar.Foreground = Brushes.Red;
            copyProgressBar.IsIndeterminate = true;
        }

        private void Labelstyle_Kopiervorgang_start()
        {
            var converter = new BrushConverter();

            buttonKopieren.Visibility = Visibility.Hidden;

            buttonAbbrechen.Visibility = Visibility.Visible;

            //labelCopyFileTotal.Visibility = Visibility.Visible;
            //labelCopyFileTotal.Content = "/ " + totalAllFiles.ToString() + " Dateien";

            //labelCopyFileCount.Visibility = Visibility.Visible;

            labelCopyInProgress.Content = "Dateien werden kopiert...";
            labelCopyInProgress.FontWeight = FontWeights.Regular;
            labelCopyInProgress.Foreground = Brushes.Black;
            labelCopyInProgress.Visibility = Visibility.Visible;

            labelCopyProgressPercent.Visibility = Visibility.Visible;

            copyProgressBar.Visibility = Visibility.Visible;
            copyProgressBar.Foreground = (Brush)converter.ConvertFromString("#FF0097fb");
            copyProgressBar.IsIndeterminate = false;
            copyProgressBar.Value = 0;
        }

        private void Labelstyle_Kopiervorgang_abgebrochen()
        {
            labelCopyInProgress.Content = "Kopiervrogang abgebrochen";
            labelCopyInProgress.FontWeight = FontWeights.Bold;
            labelCopyInProgress.Foreground = Brushes.Red;
            labelCopyProgressPercent.Visibility = Visibility.Hidden;
            labelCopyProgressPercent.Content = "";
            //labelCopyFileCount.Visibility = Visibility.Hidden;
            buttonAbbrechen.Visibility = Visibility.Hidden;
            buttonKopieren.Visibility = Visibility.Visible;
            buttonKopieren.Content = "Wiederholen";
            copyProgressBar.Value = 0;
        }

        private void Labelstyle_Kopiervorgang_erfolgreich()
        {
            labelCopyInProgress.Content = "Daten erfolgreich kopiert";
            labelCopyProgressPercent.Visibility = Visibility.Hidden;
            labelCopyFileCount.Visibility = Visibility.Hidden;
            buttonAbbrechen.Visibility = Visibility.Hidden;
            copyProgressBar.Value = 100;
        }

        private void Check_Verzeichnispfade()
        {
            if (!Directory.Exists(s_sourcePath))
            {
                MessageBox.Show("Das Quellverzeichnis konnte nicht gefunden werden.\n Die Anwendung wird beendet.\n Wendern Sie sich an PinkPanther", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            else
            {
                labelOriginPath.Content = "Quellverzeichnis: " + s_sourcePath;
            }

            if (!Directory.Exists(s_targetPath))
            {
                DirectoryInfo di = Directory.CreateDirectory(s_targetPath);
                MessageBox.Show("Das Zielverzeichnis konnte nicht gefunden werden.\n Es wurde ein neues Verzeichnis angelegt.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                labelTargetPath.Content = "Zielverzeichnis: " + s_targetPath;
            }
            else
            {
                labelTargetPath.Content = "Zielverzeichnis: " + s_targetPath;
            }
        }

        #endregion Grafik (Labelaenderungen)

        #region Verzeichnis loeschen und Dateien kopieren

        private void Worker_Initialisieren()
        {
            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;

            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_copyProgressBarUpdate;
            worker.RunWorkerCompleted += worker_copyCompleted;
        }

        public void worker_DoWork(object sender, DoWorkEventArgs eDoWork)
        {
            int totalAllFiles = Directory.GetDirectories(s_sourcePath, "*", SearchOption.AllDirectories).Count() + Directory.GetFiles(s_sourcePath, "*.*", SearchOption.AllDirectories).Count();

            try
            {
                var dirInfo = new DirectoryInfo(s_targetPath);

                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    if (worker.CancellationPending == true)
                    {
                        eDoWork.Cancel = true;
                        return;
                    }
                    file.Delete();
                }

                foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                {
                    if (worker.CancellationPending == true)
                    {
                        eDoWork.Cancel = true;
                        return;
                    }
                    dir.Delete(true);
                }
                Thread.Sleep(1500);
            }
            catch (Exception e)
            {
                MessageBox.Show("Ein Fehler ist aufgetreten:\n" + Convert.ToString(e), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            int count = 0;

            foreach (string dirPath in Directory.GetDirectories(s_sourcePath, "*", SearchOption.AllDirectories))
            {
                if (worker.CancellationPending == true)
                {
                    eDoWork.Cancel = true;
                    return;
                }
                Directory.CreateDirectory(dirPath.Replace(s_sourcePath, s_targetPath));
                count++;
                worker.ReportProgress(count * 100 / totalAllFiles);
            }
            foreach (string newPath in Directory.GetFiles(s_sourcePath, "*.*", SearchOption.AllDirectories))
            {
                if (worker.CancellationPending == true)
                {
                    eDoWork.Cancel = true;
                    return;
                }
                File.Copy(newPath, newPath.Replace(s_sourcePath, s_targetPath), true);
                count++;
                worker.ReportProgress(count * 100 / totalAllFiles);
            }
            eDoWork.Result = count;
        }

        private void worker_copyProgressBarUpdate(object sender, ProgressChangedEventArgs uiUpdate)
        {
            copyProgressBar.Value = uiUpdate.ProgressPercentage;
            labelCopyProgressPercent.Content = uiUpdate.ProgressPercentage.ToString() + "%";
        }

        private void worker_copyCompleted(object sender, RunWorkerCompletedEventArgs eComplete)
        {
            if (eComplete.Cancelled)
            {
                Labelstyle_Kopiervorgang_abgebrochen();
            }
            else
            {
                DateiInfoZielpfad();
                Labelstyle_Kopiervorgang_erfolgreich();
                buttonWeiter.IsEnabled = true;
                var window = Application.Current.MainWindow;
                (window as MainWindow).menuLabelChecked_1.Visibility = Visibility.Visible;
            }
        }
    }

    #endregion Verzeichnis loeschen und Dateien kopieren
}