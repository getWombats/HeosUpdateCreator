using System;
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
        private static string s_sourcePath = @"O:\rENT\ENT Soria\Temp\Quellverzeichnis";
        private static string s_targetPath = @"C:\Zielverzeichnis";

        private readonly DirectoryInfo diSource = new DirectoryInfo(s_sourcePath);
        private readonly DirectoryInfo diTarget = new DirectoryInfo(s_targetPath);

        // Backgroundworker initialisieren
        private BackgroundWorker worker = null;

        // ---------- [MAIN ANFANG] ----------
        public DateienKopieren()
        {
            InitializeComponent();

            worker = new BackgroundWorker();
            //worker.DoWork += new DoWorkEventHandler(lalala);
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

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

        // ---------- [MAIN ENDE] ----------


        // ---------- [BUTTONS ANFANG] ----------
        private void buttonWeiter_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("sqlSetup.xaml", UriKind.Relative));
        }

        private void buttonKopieren_Click(object sender, RoutedEventArgs e)
        {
            //VerzeichnisLoeschen();
            if (worker.IsBusy != true)
            {
                labelCopyInProgress.Visibility = Visibility.Visible;
                labelCopyProgressPercent.Visibility = Visibility.Visible;
                labelCopyFileInfo.Visibility = Visibility.Visible;
                borderButtonAbbrechen.Visibility = Visibility.Visible;

                copyProgressBar.Value = 0;
                copyProgressBar.Visibility = Visibility.Visible;

                buttonKopieren.IsEnabled = false;

                worker.RunWorkerAsync(10000);
            }
            else
            {
                // nicht verwendet
            }
        }

        private void buttonAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            worker.CancelAsync();
        }

        // ---------- [BUTTONS ENDE] ----------

        // ---------- [BACKGROUNDWORKER ANFANG] ----------
        private void worker_DoWork(object sender, DoWorkEventArgs eDoWork)
        {
            int max = (int)eDoWork.Argument;
            int result = 0;
            for (int i = 0; i < max; i++)
            {
                if (worker.CancellationPending == true)
                {
                    eDoWork.Cancel = true;
                    return;
                }
                int progressPercentage = Convert.ToInt32(((double)i / max) * 100);
                if (i % 42 == 0)
                {
                    result++;
                    (sender as BackgroundWorker).ReportProgress(progressPercentage, i);
                }
                else
                {
                    (sender as BackgroundWorker).ReportProgress(progressPercentage);
                }

                System.Threading.Thread.Sleep(1);
            }
            eDoWork.Result = result;
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs uiUpdate)
        {
            copyProgressBar.Value = uiUpdate.ProgressPercentage;
            labelCopyFileInfo.Content = uiUpdate.UserState;
            labelCopyProgressPercent.Content = uiUpdate.ProgressPercentage.ToString() + "%";
            //if (uiUpdate.UserState != null)
            //    lbResults.Items.Add(uiUpdate.UserState);
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                labelCopyInProgress.Content = "Kopiervrogang abgebrochen";
                labelCopyInProgress.FontWeight = FontWeights.Bold;
                labelCopyInProgress.Foreground = Brushes.Red;

                labelCopyProgressPercent.Visibility = Visibility.Hidden;
                labelCopyFileInfo.Visibility = Visibility.Hidden;

                borderButtonAbbrechen.Visibility = Visibility.Hidden;
                buttonKopieren.Visibility = Visibility.Visible;
            }
            else
            {
                //var window = Application.Current.MainWindow;
                //(window as MainWindow).menuLabelChecked_1.Visibility = Visibility.Visible;
                labelCopyInProgress.Content = "Daten erfolgreich kopiert";
                labelCopyProgressPercent.Visibility = Visibility.Hidden;
                labelCopyFileInfo.Visibility = Visibility.Hidden;
                borderButtonAbbrechen.Visibility = Visibility.Hidden;
                copyProgressBar.Value = 100;
            }
        }

        // ---------- [BACKGROUNDWORKER ENDE] ----------

        // ---------- [METHODEN ANFANG] ----------
        public void VerzeichnisLoeschen()
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
                // nicht verwendet
            }
        }

        public static void StartKopieren(string sourceDirectory, string targetDirectory)
        {
            //CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            //Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        // ---------- [METHODEN ENDE] ----------
    }
}