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

        // Allgemeine Werte initialisieren
        private static string s_quellVerzeichnis = @"O:\ENT\ENT Soria\Temp\Quellverzeichnis";
        private static string s_zielVerzeichnis = @"C:\Zielverzeichnis";
        private static string logpfad = @"C:\Programmentwicklung\Logs\";

        private static int totalDateienCount = Directory.GetFiles(s_quellVerzeichnis, "*.*", SearchOption.AllDirectories).Count();
        private static int totalOrdnerCount = Directory.GetDirectories(s_quellVerzeichnis, "*", SearchOption.AllDirectories).Count();
        private static int totalElementeCount = totalDateienCount + totalOrdnerCount;

        // Backgroundworker initialisieren
        private BackgroundWorker KopierWorker = null;
        private BackgroundWorker LoeschWorker = null;

        #region Main

        public DateienKopieren() // Main
        {
            InitializeComponent();
            Check_Verzeichnispfade();
            DateiInfoQuellpfad();
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
            if (Directory.Exists(s_quellVerzeichnis) && Directory.Exists(s_zielVerzeichnis))
            {
                MessageBoxResult msgRes = MessageBox.Show("Alle Dateien im Verzeichnis [ " + s_zielVerzeichnis + " ] werden gelöscht!", "Achtung", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (msgRes == MessageBoxResult.OK)
                {
                    KopierWorkerInitialisieren();
                    LoeschWorkerInitialisieren();
                    LoeschWorker.RunWorkerAsync();
                }
                else
                {
                    return;
                }
            }
        }

        private void buttonAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            if (KopierWorker.IsBusy)
            {
                KopierWorker.CancelAsync();
            }
            else if (LoeschWorker.IsBusy)
            {
                LoeschWorker.CancelAsync();
            }
        }

        #endregion Buttons

        #region Grafik Labelaenderungen

        private void DateiInfoQuellpfad()
        {
            labelAnzahlDateien.Content = totalDateienCount + " Dateien, " + totalOrdnerCount + " Ordner";

            long size = 0;
            DirectoryInfo dir = new DirectoryInfo(s_quellVerzeichnis);
            foreach (FileInfo fi in dir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                size += fi.Length;
            }
            labelQuellverzeichnisDateiGroesse.Content = "Grösse: " + Math.Round(size / Math.Pow(1024, 2), MidpointRounding.ToEven).ToString() + " MB" + " " + "(" + size.ToString() + " Bytes)";
        }

        private void DateiInfoZielpfad()
        {
            labelAnzahlDateienZielverzeichnis.Content = totalDateienCount + " Dateien, " + totalOrdnerCount + " Ordner";

            long size = 0;
            DirectoryInfo dir = new DirectoryInfo(s_zielVerzeichnis);
            foreach (FileInfo fi in dir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                size += fi.Length;
            }
            labelGroesseDateienZielverzeichnis.Visibility = Visibility.Visible;
            labelGroesseDateienZielverzeichnis.Content = "Grösse: " + Math.Round(size / Math.Pow(1024, 2), MidpointRounding.ToEven).ToString() + " MB" + " " + "(" + size.ToString() + " Bytes)";
        }

        private void Labelstyle_Verzeichnis_loeschen()
        {
            labelDatenWerdenKopiert.Visibility = Visibility.Visible;
            labelDatenWerdenKopiert.FontWeight = FontWeights.Normal;
            labelDatenWerdenKopiert.Foreground = Brushes.Black;
            labelDatenWerdenKopiert.Content = "Daten werden gelöscht...";
            buttonAbbrechen.Visibility = Visibility.Visible;
            labelLoeschfortschrittProzent.Visibility = Visibility.Visible;
            progressBarLoeschvorgang.Visibility = Visibility.Visible;
        }

        private void Labelstyle_Kopiervorgang_start()
        {
            var converter = new BrushConverter();

            buttonKopieren.Visibility = Visibility.Hidden;

            buttonAbbrechen.Visibility = Visibility.Visible;

            labelDatenWerdenKopiert.Content = "Daten werden kopiert...";
            labelDatenWerdenKopiert.FontWeight = FontWeights.Regular;
            labelDatenWerdenKopiert.Foreground = Brushes.Black;
            labelDatenWerdenKopiert.Visibility = Visibility.Visible;

            labelKopierfortschrittProzent.Visibility = Visibility.Visible;

            progressBarKopiervorgang.Visibility = Visibility.Visible;
            progressBarKopiervorgang.Foreground = (Brush)converter.ConvertFromString("#FF0097fb");
            progressBarKopiervorgang.IsIndeterminate = false;
            progressBarKopiervorgang.Value = 0;
        }

        private void Labelstyle_Kopiervorgang_abgebrochen()
        {
            labelDatenWerdenKopiert.Content = "Kopiervrogang abgebrochen";
            labelDatenWerdenKopiert.FontWeight = FontWeights.Bold;
            labelDatenWerdenKopiert.Foreground = Brushes.Red;
            labelKopierfortschrittProzent.Visibility = Visibility.Hidden;
            labelLoeschfortschrittProzent.Visibility = Visibility.Hidden;
            labelKopierfortschrittProzent.Content = "";

            buttonAbbrechen.Visibility = Visibility.Hidden;
            buttonKopieren.Visibility = Visibility.Visible;
            buttonKopieren.Content = "Wiederholen";
        }

        private void LabelstyleKopiervorgangErfolgreich()
        {
            labelDatenWerdenKopiert.Content = "Daten erfolgreich kopiert";
            labelKopierfortschrittProzent.Visibility = Visibility.Hidden;

            labelKopierteElemente.Visibility = Visibility.Visible;

            buttonAbbrechen.Visibility = Visibility.Hidden;
            progressBarKopiervorgang.Value = 100;
        }

        private void Check_Verzeichnispfade()
        {
            if (!Directory.Exists(s_quellVerzeichnis))
            {
                MessageBox.Show("Das Quellverzeichnis wurde nicht gefunden.\n \nNavigieren Sie im folgenden Fenster zum Quellverzeichnis und drücken Sie OK.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                    if (result != System.Windows.Forms.DialogResult.OK)
                    {
                        Application.Current.MainWindow.Close();
                    }
                    else
                    {
                        s_quellVerzeichnis = dialog.SelectedPath;
                        labelQuellverzeichnis.Content = "Quellverzeichnis: " + s_quellVerzeichnis;
                    }
                }
            }
            if (!Directory.Exists(s_zielVerzeichnis))
            {
                MessageBox.Show("Das Zielverzeichnis konnte nicht gefunden werden.\nEs wurde ein neues Verzeichnis mit dem angegebenen Pfad angelegt.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                DirectoryInfo di = Directory.CreateDirectory(s_zielVerzeichnis);
                labelZielverzeichnis.Content = "Zielverzeichnis: " + s_zielVerzeichnis;
            }
            else if (s_zielVerzeichnis == @"C:\" || s_zielVerzeichnis == @"C:\Windows\*" || s_zielVerzeichnis == @"C:\Users" || s_zielVerzeichnis == @"C:\Program Files (x86)" || s_zielVerzeichnis == @"C:\Program Files")
            {
                labelZielverzeichnis.Content = "Ungültiges Zielverzeichnis: " + s_zielVerzeichnis;
                labelZielverzeichnis.Foreground = Brushes.Red;
                buttonKopieren.IsEnabled = false;
                return;
            }
            else
            {
                labelQuellverzeichnis.Content = "Quellverzeichnis: " + s_quellVerzeichnis;
                labelZielverzeichnis.Content = "Zielverzeichnis: " + s_zielVerzeichnis;
            }
        }

        #endregion Grafik Labelaenderungen

        #region Backgroundworker Verzeichnis löschen

        private void LoeschWorkerInitialisieren()
        {
            LoeschWorker = new BackgroundWorker();
            LoeschWorker.WorkerSupportsCancellation = true;
            LoeschWorker.WorkerReportsProgress = false;

            LoeschWorker.DoWork += LoeschWorkerStart;
            LoeschWorker.RunWorkerCompleted += LoeschWorkerFertig;
            Labelstyle_Verzeichnis_loeschen();
        }

        public void LoeschWorkerStart(object sender, DoWorkEventArgs eDoWork)
        {
            //int totalZielDateienCount = Directory.GetFiles(s_zielVerzeichnis, "*.*", SearchOption.AllDirectories).Count();
            //int totalZielOrdnerCount = Directory.GetDirectories(s_zielVerzeichnis, "*", SearchOption.AllDirectories).Count();
            //int totalZielpfadElementeCount = totalZielDateienCount + totalZielOrdnerCount;
            var dirInfo = new DirectoryInfo(s_zielVerzeichnis);

            foreach (string file in Directory.GetFiles(s_zielVerzeichnis))
            {
                if (LoeschWorker.CancellationPending == true)
                {
                    eDoWork.Cancel = true;
                    return;
                }
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    ExceptionLogfile(ex, "Datei");
                }
            }

            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            {
                if (LoeschWorker.CancellationPending == true)
                {
                    eDoWork.Cancel = true;
                    return;
                }
                try
                {
                    dir.Attributes = FileAttributes.Normal;
                    dir.Delete(true);
                }
                catch (Exception ex)
                {
                    ExceptionLogfile(ex, "Ordner");
                }
            }
        }


        private void LoeschWorkerFertig(object sender, RunWorkerCompletedEventArgs eComplete)
        {
            if (eComplete.Cancelled)
            {
                progressBarLoeschvorgang.Value = 0;
                Labelstyle_Kopiervorgang_abgebrochen();
                labelDatenWerdenKopiert.Content = "Löschvorgang abgebrochen";
            }
            else
            {
                progressBarLoeschvorgang.Visibility = Visibility.Hidden;
                labelLoeschfortschrittProzent.Visibility = Visibility.Hidden;
                Labelstyle_Kopiervorgang_start();
                Thread.Sleep(1000); // Wartezeit für UI
                KopierWorker.RunWorkerAsync();
            }
        }
        #endregion Backgroundworker Verzeichnis löschen

        #region Backgroundworker Verzeichnis kopieren

        private void KopierWorkerInitialisieren()
        {
            KopierWorker = new BackgroundWorker();
            KopierWorker.WorkerSupportsCancellation = true;
            KopierWorker.WorkerReportsProgress = true;

            KopierWorker.DoWork += KopierWorkerStart;
            KopierWorker.ProgressChanged += KopierWorkerProgressBarUpdate;
            KopierWorker.RunWorkerCompleted += KopierWorkerFertig;
            Labelstyle_Kopiervorgang_start();
        }

        public void KopierWorkerStart(object sender, DoWorkEventArgs eDoWork)
        {
            int count = 0;
            foreach (string dirPath in Directory.GetDirectories(s_quellVerzeichnis, "*", SearchOption.AllDirectories))
            {
                if (KopierWorker.CancellationPending == true)
                {
                    eDoWork.Cancel = true;
                    return;
                }
                Directory.CreateDirectory(dirPath.Replace(s_quellVerzeichnis, s_zielVerzeichnis));
                count++;
                KopierWorker.ReportProgress(count * 100 / totalElementeCount);
            }
            foreach (string newPath in Directory.GetFiles(s_quellVerzeichnis, "*.*", SearchOption.AllDirectories))
            {
                if (KopierWorker.CancellationPending == true)
                {
                    eDoWork.Cancel = true;
                    return;
                }
                File.Copy(newPath, newPath.Replace(s_quellVerzeichnis, s_zielVerzeichnis), true);
                count++;
                KopierWorker.ReportProgress(count * 100 / totalElementeCount);
            }
            eDoWork.Result = count;
        }

        private void KopierWorkerProgressBarUpdate(object sender, ProgressChangedEventArgs uiUpdate)
        {
            progressBarKopiervorgang.Value = uiUpdate.ProgressPercentage;
            labelKopierfortschrittProzent.Content = uiUpdate.ProgressPercentage.ToString() + "%";
        }

        private void KopierWorkerFertig(object sender, RunWorkerCompletedEventArgs eComplete)
        {
            if (eComplete.Cancelled)
            {
                progressBarKopiervorgang.Value = 0;
                Labelstyle_Kopiervorgang_abgebrochen();
            }
            else
            {
                DateiInfoZielpfad();
                LabelstyleKopiervorgangErfolgreich();
                buttonWeiter.IsEnabled = true;
                var window = Application.Current.MainWindow;
                (window as MainWindow).labelReihenfolgeChecked_1.Visibility = Visibility.Visible;

                KopierLogfileSchreiben(totalDateienCount, totalOrdnerCount);
            }
        }

        #endregion Backgroundworker Verzeichnis kopieren

        #region Exception Handling / Logfiles

        private void ExceptionLogfile(Exception ex, string name)
        {
            string logName = "ExceptionLog_" + name + DateTime.Now.ToString("ddMMyyyyHHmm") + ".log";
            string logdir = logpfad + logName;

            MessageBox.Show("Ein Fehler ist aufgetreten:\nLogfile in" + logdir + " geschrieben\n\n" + Convert.ToString(ex), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);

            using (StreamWriter writer = new StreamWriter(logdir, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Datum : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }

        private void KopierLogfileSchreiben(int anzahlDateien, int anzahlOrdner)
        {
            string logName = "Kopierprozess.log";
            string logdir = logpfad + logName;

            using (StreamWriter writer = new StreamWriter(logdir, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine();
                writer.WriteLine("Datum : " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                writer.WriteLine("Quellverzeichnis: " + s_quellVerzeichnis);
                writer.WriteLine("Zielverzeichnis: " + s_zielVerzeichnis);
                writer.WriteLine(anzahlDateien + " Dateien und " + anzahlOrdner + " Ordner erfolgreich kopiert");
                writer.WriteLine();
            }
        }

        #endregion Exception Handling / Logfiles
    }
}