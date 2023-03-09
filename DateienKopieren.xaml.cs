using System;
using System.Collections.Generic;
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

        // Pfade Werte initialisieren
        private static string s_quellVerzeichnis = @"O:\ENT\ENT Soria\Temp\Quellverzeichnis";
        private static string s_zielVerzeichnis = @"C:\Zielverzeichnis";
        private static string s_logpfad = @"C:\Programmentwicklung\Logs\";
        private static string s_configpfad = @"C:\Programmentwicklung\HeosUpdateCreator.heo";

        // Anzahl Dateien in Verzeichnissen initialisieren
        private static int totalAnzahlDateien = Directory.GetFiles(s_quellVerzeichnis, "*.*", SearchOption.AllDirectories).Count();
        private static int totalAnzahlOrdner = Directory.GetDirectories(s_quellVerzeichnis, "*", SearchOption.AllDirectories).Count();
        private static int totalAnzahlElemente = totalAnzahlDateien + totalAnzahlOrdner;

        // Backgroundworker deklarieren
        private BackgroundWorker KopierWorker = null;
        private BackgroundWorker LoeschWorker = null;

        #region Main

        public DateienKopieren()
        {
            InitializeComponent();
            // Prüft Verzeichnispfade
            Check_Verzeichnispfade();
            // Infolabels unter Quellpfad darstellen
            DateiInfoQuellpfad();
        }

        #endregion Main

        #region Button Events

        private void buttonWeiter_Click(object sender, RoutedEventArgs e)
        {
            // Navigiert zur nächsten Page
            if (NavigationService.CanGoForward)
            {
                NavigationService.GoForward();
            }
            // oder erstellt Instanz der nächsten Page, wenn sie noch nicht existiert
            else
            {
                NavigationService ns = NavigationService.GetNavigationService(this);
                ns.Navigate(new Uri("sqlSetup.xaml", UriKind.Relative));
            }
        }

        private void buttonKopieren_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(s_quellVerzeichnis) && Directory.Exists(s_zielVerzeichnis))
            {
                MessageBoxResult msgRes = MessageBox.Show("Alle Dateien im Verzeichnis [ " + s_zielVerzeichnis + " ] werden gelöscht!", "Achtung", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (msgRes == MessageBoxResult.OK)
                {
                    // Worker für Kopierprozess und Löschprozess instanzieren
                    KopierWorkerInitialisieren();
                    LoeschWorkerInitialisieren();

                    // Löschprozess starten
                    LoeschWorker.RunWorkerAsync();
                }
                else
                {
                    // Zurück zu MainWindow-Instanz ohne Lösch- und Kopierprozess
                    return;
                }
            }
        }

        private void buttonAbbrechen_Click(object sender, RoutedEventArgs e)
        {
            // Abbrechen, wenn Worker für Löschprozess läuft
            if (LoeschWorker.IsBusy)
            {
                LoeschWorker.CancelAsync();
            }
            // Abbrechen, wenn Worker für Kopierprozess läuft
            else if (KopierWorker.IsBusy)
            {
                KopierWorker.CancelAsync();
            }
        }

        #endregion Button Events

        #region Grafik Labelaenderungen

        private void DateiInfoQuellpfad() // Informationen Quellverzeichnis erstellen
        {
            // Verzeichnisinformationen darstellen
            labelAnzahlDateien.Content = totalAnzahlDateien + " Dateien, " + totalAnzahlOrdner + " Ordner";

            // Gesamtgrösse aller Dateien im Quellverzeichnis erstellen
            long size = 0;
            DirectoryInfo dir = new DirectoryInfo(s_quellVerzeichnis);
            foreach (FileInfo fi in dir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                size += fi.Length;
            }
            labelQuellverzeichnisDateiGroesse.Content = "Grösse: " + Math.Round(size / Math.Pow(1024, 2), MidpointRounding.ToEven).ToString() + " MB" + " " + "(" + size.ToString() + " Bytes)";
        }

        private void DateiInfoZielpfad() // Informationen Zielverzeichnis nach Kopierprozess erstellen
        {
            labelAnzahlDateienZielverzeichnis.Content = totalAnzahlDateien + " Dateien, " + totalAnzahlOrdner + " Ordner";

            // Gesamtgrösse aller Dateien im Zielverzeichnis nach Kopierprozess erstellen
            long size = 0;
            DirectoryInfo dir = new DirectoryInfo(s_zielVerzeichnis);
            foreach (FileInfo fi in dir.GetFiles("*.*", SearchOption.AllDirectories))
            {
                size += fi.Length;
            }
            labelGroesseDateienZielverzeichnis.Visibility = Visibility.Visible;
            labelGroesseDateienZielverzeichnis.Content = "Grösse: " + Math.Round(size / Math.Pow(1024, 2), MidpointRounding.ToEven).ToString() + " MB" + " " + "(" + size.ToString() + " Bytes)";
        }

        private void Labelstyle_Verzeichnis_loeschen() // UI-Elemente für Löschprozess darstellen
        {
            labelDatenWerdenKopiert.Visibility = Visibility.Visible;
            labelDatenWerdenKopiert.FontWeight = FontWeights.Normal;
            labelDatenWerdenKopiert.Foreground = Brushes.Black;
            labelDatenWerdenKopiert.Content = "Daten werden gelöscht...";
            buttonAbbrechen.Visibility = Visibility.Visible;
            labelLoeschfortschrittProzent.Visibility = Visibility.Visible;
            progressBarLoeschvorgang.Visibility = Visibility.Visible;
        }

        private void Labelstyle_Kopiervorgang_start() // UI-Elemente für Kopierprozessprozess darstellen
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

        private void Labelstyle_Kopiervorgang_abgebrochen() // UI-Elemente darstellen, wenn Kopier- oder Löschprozess abgebrochen
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

        private void LabelstyleKopiervorgangErfolgreich() // UI-Elemente darstellen, wenn Kopierprozess beendet
        {
            labelDatenWerdenKopiert.Content = "Daten erfolgreich kopiert";
            labelKopierfortschrittProzent.Visibility = Visibility.Hidden;

            labelKopierteElemente.Visibility = Visibility.Visible;

            buttonAbbrechen.Visibility = Visibility.Hidden;
            progressBarKopiervorgang.Value = 100;
        }

        private void Check_Verzeichnispfade() // Prüft Verzeichnispfade auf Existenz und stellt UI-Elemente dar
        {
            // Prüft Existenz Quellverzeichnis, stellt UI-Elemente dar
            if (Directory.Exists(s_quellVerzeichnis))
            {
                labelQuellverzeichnis.Content = "Quellverzeichnis: " + s_quellVerzeichnis;
            }
            // Quellverzeichnis wählen, wenn inexistent / nicht gefunden
            else
            {
                MessageBox.Show("Das Quellverzeichnis wurde nicht gefunden.\n \nNavigieren Sie im folgenden Fenster zum Quellverzeichnis und drücken Sie OK.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        s_quellVerzeichnis = dialog.SelectedPath;
                        labelQuellverzeichnis.Content = "Quellverzeichnis: " + s_quellVerzeichnis;
                    }
                    else
                    {
                        Application.Current.MainWindow.Close();
                    }
                }
            }

            // Prüft Existenz Zielverzeichnis, stellt UI-Elemente dar
            if (Directory.Exists(s_zielVerzeichnis))
            {
                labelZielverzeichnis.Content = "Zielverzeichnis: " + s_zielVerzeichnis;
            }
            else
            {
                MessageBox.Show("Das Zielverzeichnis konnte nicht gefunden werden.\nEs wurde ein neues Verzeichnis mit dem angegebenen Pfad angelegt.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                DirectoryInfo di = Directory.CreateDirectory(s_zielVerzeichnis);
                labelZielverzeichnis.Content = "Zielverzeichnis: " + s_zielVerzeichnis;
            }
            //if (s_zielVerzeichnis == @"C:\" || s_zielVerzeichnis == @"C:\Windows\*" || s_zielVerzeichnis == @"C:\Users" || s_zielVerzeichnis == @"C:\Program Files (x86)" || s_zielVerzeichnis == @"C:\Program Files")
            //{
            //    labelZielverzeichnis.Content = "Ungültiges Zielverzeichnis: " + s_zielVerzeichnis;
            //    labelZielverzeichnis.Foreground = Brushes.Red;
            //    buttonKopieren.IsEnabled = false;
            //    return;
            //}
        }

        #endregion Grafik Labelaenderungen

        #region Backgroundworker Verzeichnis löschen

        private void LoeschWorkerInitialisieren() // Löschprozess initialisieren und UI-Elemente darstellen
        {
            LoeschWorker = new BackgroundWorker();
            LoeschWorker.WorkerSupportsCancellation = true;
            LoeschWorker.WorkerReportsProgress = false;

            LoeschWorker.DoWork += LoeschWorkerStart;
            LoeschWorker.RunWorkerCompleted += LoeschWorkerFertig;
            Labelstyle_Verzeichnis_loeschen();
        }

        public void LoeschWorkerStart(object sender, DoWorkEventArgs eDoWork) // Löschprozess starten
        {
            var dirInfo = new DirectoryInfo(s_zielVerzeichnis);
            // 1. Iteration: Dateiattribute rekursiv neu setzen
            setAttributesNormal(dirInfo);

            // 2. Iteration: Dateien löschen
            foreach (string file in Directory.GetFiles(s_zielVerzeichnis))
            {
                if (LoeschWorker.CancellationPending == true)
                {
                    // Löschprozess abbrechen
                    eDoWork.Cancel = true;
                    return;
                }
                try
                {
                    // Datei pro Iteration löschen
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    // Bei Fehler Logdatei schreiben
                    ExceptionLogfileSchreiben(ex, "Dateien_loeschen_");
                }
            }

            // 3. Iteration: Ordnerstruktur löschen
            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            {
                if (LoeschWorker.CancellationPending == true)
                {
                    // Löschprozess abbrechen
                    eDoWork.Cancel = true;
                    return;
                }
                try
                {
                    // Ordner pro Iteration löschen
                    dir.Delete(true);
                }
                catch (Exception ex)
                {
                    // Bei Fehler Logdatei schreiben
                    ExceptionLogfileSchreiben(ex, "Ordner_loeschen_");
                }
            }
        }

        private void setAttributesNormal(DirectoryInfo dir) // Methode Schreibschutz Dateien für Löschprozess entfernen
        {
            foreach (var subDir in dir.GetDirectories())
            {
                setAttributesNormal(subDir);
            }
            foreach (var file in dir.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }
        }

        private void LoeschWorkerFertig(object sender, RunWorkerCompletedEventArgs eComplete) // Löschprozess beendet
        {
            // UI-Elemente darstellen, wenn Worker abgebrochen
            if (eComplete.Cancelled)
            {
                progressBarLoeschvorgang.Value = 0;
                Labelstyle_Kopiervorgang_abgebrochen();
                labelDatenWerdenKopiert.Content = "Löschvorgang abgebrochen";
            }
            // oder UI-Elemente darstellen, wenn Worker fertig
            else
            {
                progressBarLoeschvorgang.Visibility = Visibility.Hidden;
                labelLoeschfortschrittProzent.Visibility = Visibility.Hidden;
                Labelstyle_Kopiervorgang_start();

                // Wartezeit für UI-Darstellung
                Thread.Sleep(1000);

                // Kopierprozess starten
                KopierWorker.RunWorkerAsync();
            }
        }
        #endregion Backgroundworker Verzeichnis löschen

        #region Backgroundworker Verzeichnis kopieren

        private void KopierWorkerInitialisieren() // Kopierprozess initialisieren und UI-Elemente darstellen
        {
            KopierWorker = new BackgroundWorker();
            KopierWorker.WorkerSupportsCancellation = true;
            KopierWorker.WorkerReportsProgress = true;

            KopierWorker.DoWork += KopierWorkerStart;
            KopierWorker.ProgressChanged += KopierWorkerProgressBarUpdate;
            KopierWorker.RunWorkerCompleted += KopierWorkerFertig;
            Labelstyle_Kopiervorgang_start();
        }

        public void KopierWorkerStart(object sender, DoWorkEventArgs eDoWork) // Kopierprozess starten
        {
            // 1. Iteration: Ordnerstruktur in Zielverzeichnis erstellen
            int count = 0;
            foreach (string dir in Directory.GetDirectories(s_quellVerzeichnis, "*", SearchOption.AllDirectories))
            {
                if (KopierWorker.CancellationPending == true)
                {
                    // Kopierprozess abbrechen
                    eDoWork.Cancel = true;
                    return;
                }
                Directory.CreateDirectory(dir.Replace(s_quellVerzeichnis, s_zielVerzeichnis));
                count++;
                KopierWorker.ReportProgress(count * 100 / totalAnzahlElemente);
            }
            // 2. Iteration: Dateien kopieren
            foreach (string file in Directory.GetFiles(s_quellVerzeichnis, "*.*", SearchOption.AllDirectories))
            {
                if (KopierWorker.CancellationPending == true)
                {
                    // Kopierprozess abbrechen
                    eDoWork.Cancel = true;
                    return;
                }
                File.Copy(file, file.Replace(s_quellVerzeichnis, s_zielVerzeichnis), true);
                count++;
                KopierWorker.ReportProgress(count * 100 / totalAnzahlElemente);
            }
            eDoWork.Result = count;
        }

        private void KopierWorkerProgressBarUpdate(object sender, ProgressChangedEventArgs uiUpdate) // Ladebalken Kopierprozess aktualisieren
        {
            progressBarKopiervorgang.Value = uiUpdate.ProgressPercentage;
            labelKopierfortschrittProzent.Content = uiUpdate.ProgressPercentage.ToString() + "%";
        }

        private void KopierWorkerFertig(object sender, RunWorkerCompletedEventArgs eComplete) // Kopierprozess beendet
        {
            // UI-Elemente darstellen, wenn Worker abgebrochen
            if (eComplete.Cancelled)
            {
                progressBarKopiervorgang.Value = 0;
                Labelstyle_Kopiervorgang_abgebrochen();
            }
            // oder UI-Elemente darstellen, wenn Worker fertig
            else
            {
                DateiInfoZielpfad();
                LabelstyleKopiervorgangErfolgreich();
                buttonWeiter.IsEnabled = true;
                var window = Application.Current.MainWindow;
                (window as MainWindow).labelReihenfolgeChecked_1.Visibility = Visibility.Visible;

                // Logfile nach Kopierprozess schreiben
                KopierLogfileSchreiben(totalAnzahlDateien, totalAnzahlOrdner);
            }
        }

        #endregion Backgroundworker Verzeichnis kopieren

        #region Logfiles

        private void ExceptionLogfileSchreiben(Exception ex, string name) // Logfile bei Fehler schreiben
        {
            string logName = "ExceptionLog_" + name + DateTime.Now.ToString("ddMMyyyyHHmm") + ".log";
            string logdir = s_logpfad + logName;

            MessageBox.Show("Ein Fehler ist aufgetreten:\nLogfile in" + logdir + " geschrieben\n\n" + Convert.ToString(ex), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);

            using (StreamWriter writer = new StreamWriter(logdir, false)) // erstellt neue Logfile
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Datum : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null) // erweitert Logfile bei mehreren / weiteren Fehlern
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }

        private void KopierLogfileSchreiben(int anzahlDateien, int anzahlOrdner) // Logfile nach Kopierprozess schreiben
        {
            string logName = "Kopierprozess.log";
            string logdir = s_logpfad + logName;

            using (StreamWriter writer = new StreamWriter(logdir, true)) // erstellt neue Logfile oder erweitert bestehende (true)
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

        #endregion Logfiles

        #region Config Datei lesen
        private void ConfigDateiLesen()
        {
            // blabla
        }
        #endregion
    }
}