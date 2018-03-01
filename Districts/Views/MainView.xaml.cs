using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Districts.Settings;
using Districts.ViewModel;
using Districts.WebRequest;

namespace Districts.Views
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private ApplicationSettings settings;

        public MainView()
        {
            InitializeComponent();
            settings = ApplicationSettings.ReadOrCreate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditHomes(object sender, RoutedEventArgs e)
        {
            var window = new TreeTableView {DataContext = new EditTreeViewModel()};
            window.ShowDialog();
        }

        private void GenerateCards(object sender, RoutedEventArgs e)
        {
            ToggleIsEnabled(sender);

            var generator = new CardGenerator.CardGenerator();
            generator.Generate();

            var checker = new Checker.DoorChecker();
            var repeated = checker.FindRepeated();

            ToggleIsEnabled(sender);

            if (!repeated.Any())
                ShowDone();
            else
            {
                FileInfo file = new FileInfo("repeated.log");
                var all = repeated.Select(x => x.ToString() + "\n")
                    .Aggregate(string.Empty, (source, x) => source += x);
                File.WriteAllText(file.FullName, all);
                MessageBox.Show("Есть повторяемые значения, сохранены в файл " + file.FullName);
            }
        }

        private void OpenStreetFolder(object sender, RoutedEventArgs e)
        {
            var s = Directory.GetParent(settings.StreetsPath).FullName;
            Process.Start(s);
        }

        private async void DownloadHomes(object sender, RoutedEventArgs e)
        {
            ToggleIsEnabled(sender);

            var downloader = new MainDownloader();
            await downloader.DownloadInfo();

            ToggleIsEnabled(sender);
            ShowDone();
        }

        private void ToggleIsEnabled(object button)
        {
            if (button is Button b)
            {
                b.IsEnabled = !b.IsEnabled;
            }
        }

        private void ShowDone()
        {
            MessageBox.Show("Закончено");
        }

        private void OpenCardsFolder(object sender, RoutedEventArgs e)
        {
            var s = settings.CardsPath;
            Process.Start(s);
        }

        private void OpenHomesFolder(object sender, RoutedEventArgs e)
        {
            var s = settings.BuildingPath;
            Process.Start(s);
        }

        private void OpenRulesPath(object sender, RoutedEventArgs e)
        {
            var s = settings.RestrictionsPath;
            Process.Start(s);
        }

        private void OpenCodesPath(object sender, RoutedEventArgs e)
        {
            var s = settings.CodesPath;
            Process.Start(s);
        }

        private void LoadStreets(object sender, RoutedEventArgs e)
        {
            var s = Helper.Helper.RemoveEmptyLines(File.ReadAllText(settings.StreetsPath));
            streets.Text = s;
        }

        private void SaveStreets(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(settings.StreetsPath, streets.Text);

            CancelStreetsEditing(sender, e);
        }

        private void CancelStreetsEditing(object sender, RoutedEventArgs e)
        {
            streetredactor.IsChecked = false;
        }

        private void OnPrint(object sender, RoutedEventArgs e)
        {
            ToggleIsEnabled(sender);

            var generator = new CardGenerator.CardGenerator();
            generator.PrintVisual();

            ToggleIsEnabled(sender);
            ShowDone();
        }
    }
}
