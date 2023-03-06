using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

            Load_Verzeichnisinfo();

            Check_Path_exists();
        }

        #endregion Main

        #region Buttons

        private void buttonWeiter_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("sqlSetup.xaml", UriKind.Relative));
        }

        private void buttonKopieren_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(s_sourcePath) && Directory.Exists(s_targetPath))
            {
                Worker_Initialisieren();

                Verzeichnisinhalt_Loeschen();
                if (worker.IsBusy != true)
                {
                    Labelstyle_Kopiervorgang_start();

                    worker.RunWorkerAsync(1000);
                }
                else
                {
                    //sdsdf
                }
            }
        }

        private void buttonAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            worker.CancelAsync();
        }

        #endregion Buttons

        #region Styles

        private void Labelstyle_Kopiervorgang_start()
        {
            labelCopyInProgress.Content = "Dateien werden kopiert...";
            labelCopyInProgress.FontWeight = FontWeights.Regular;
            labelCopyInProgress.Foreground = Brushes.Black;
            labelCopyInProgress.Visibility = Visibility.Visible;
            labelCopyProgressPercent.Visibility = Visibility.Visible;
            labelCopyFileInfo.Visibility = Visibility.Visible;
            buttonAbbrechen.Visibility = Visibility.Visible;
            copyProgressBar.Visibility = Visibility.Visible;
            copyProgressBar.Value = 0;
            buttonKopieren.Visibility = Visibility.Hidden;
        }

        private void Labelstyle_Kopiervorgang_abgebrochen()
        {
            labelCopyInProgress.Content = "Kopiervrogang abgebrochen";
            labelCopyInProgress.FontWeight = FontWeights.Bold;
            labelCopyInProgress.Foreground = Brushes.Red;
            labelCopyProgressPercent.Visibility = Visibility.Hidden;
            labelCopyFileInfo.Visibility = Visibility.Hidden;
            buttonAbbrechen.Visibility = Visibility.Hidden;
            buttonKopieren.Visibility = Visibility.Visible;
            copyProgressBar.Value = 0;
        }

        private void Labelstyle_Kopiervorgang_erfolgreich()
        {
            labelCopyInProgress.Content = "Daten erfolgreich kopiert";
            labelCopyProgressPercent.Visibility = Visibility.Hidden;
            labelCopyFileInfo.Visibility = Visibility.Hidden;
            buttonAbbrechen.Visibility = Visibility.Hidden;
            copyProgressBar.Value = 100;
        }

        private void Check_Path_exists()
        {
            if (!Directory.Exists(s_sourcePath))
            {
                labelOriginPath.Content = "Verzeichnis konnte nicht gefunden werden!";

                if (!Directory.Exists(s_targetPath))
                {
                    labelDestinationPath.Content = "Verzeichnis konnte nicht gefunden werden!";
                }
            }
            else
            {
                labelOriginPath.Content = s_sourcePath;
                labelDestinationPath.Content = s_targetPath;
            }
        }

        #endregion Styles

        #region Worker

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
            string[] fileList = Directory.GetFiles(s_sourcePath);
            string[] dirList = Directory.GetDirectories(s_sourcePath);

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
                (sender as BackgroundWorker).ReportProgress(count);
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
                (sender as BackgroundWorker).ReportProgress(count);
            }
            eDoWork.Result = count;
        }


        //private void worker_DoWork(object sender, DoWorkEventArgs eDoWork)
        //{
        //    int max = (int)eDoWork.Argument;
        //    int result = 0;
        //    for (int i = 0; i < max; i++)
        //    {
        //        if (worker.CancellationPending == true)
        //        {
        //            eDoWork.Cancel = true;
        //            return;
        //        }
        //        int progressPercentage = Convert.ToInt32(((double)i / max) * 100);
        //        if (i % 42 == 0)
        //        {
        //            result++;
        //            (sender as BackgroundWorker).ReportProgress(progressPercentage, i);
        //        }
        //        else
        //        {
        //            (sender as BackgroundWorker).ReportProgress(progressPercentage);
        //        }

        //        System.Threading.Thread.Sleep(1);
        //    }
        //    eDoWork.Result = result;
        //}

        private void worker_copyProgressBarUpdate(object sender, ProgressChangedEventArgs uiUpdate)
        {
            copyProgressBar.Value = uiUpdate.ProgressPercentage;
            labelCopyFileInfo.Content = uiUpdate.UserState;
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
                Labelstyle_Kopiervorgang_erfolgreich();
                var window = Application.Current.MainWindow;
                (window as MainWindow).menuLabelChecked_1.Visibility = Visibility.Visible;
            }
        }

        #endregion Worker

        #region Verzeichnisinhalt löschen

        public void Verzeichnisinhalt_Loeschen()
        {
            MessageBoxResult msgRes = MessageBox.Show("Alle Dateien im Verzeichnis [ " + s_targetPath + " ] werden gelöscht!", "Achtung", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (msgRes == MessageBoxResult.OK)
            {
                try
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(s_targetPath);

                    foreach (FileInfo fi in dirInfo.EnumerateFiles())
                    {
                        fi.Delete();
                    }
                    foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Ein Fehler ist aufgetreten:\n" + Convert.ToString(e), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                //dsds
            }
        }

        #endregion Verzeichnisinhalt löschen

        #region Verzeichnisinhalt in dataGrid darstellen

        private void Load_Verzeichnisinfo()
        {
            if (Directory.Exists(s_sourcePath))
            {

                string[] sld = Directory.GetFiles(s_sourcePath, "*.exe");
                //DirectoryInfo dirInfo = new DirectoryInfo(s_sourcePath);

                //List<string> sld = new List<string>();

                //foreach (FileInfo fi in dirInfo.EnumerateFiles())
                //{
                //    sld.Add(fi.ToString());
                //    //dataGridSourcePath.Items.Add(fi);
                //}
                //dataGridSourcePath.ItemsSource = sld;

                //foreach (DirectoryInfo dir in dirInfo.GetDirectories())
                //{
                //    sld.Add(dir.ToString());
                //}
                dataGridSourcePath.ItemsSource = sld;
            }
        }

        #endregion Ordnerinhalt darstellen
    }
}