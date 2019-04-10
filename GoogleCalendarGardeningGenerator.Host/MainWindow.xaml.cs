using System.Threading.Tasks;
using System.Windows;

namespace GoogleCalendarGardeningGenerator.Host
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GardenCalendarGeneratorVm _vm;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new GardenCalendarGeneratorVm();
        }

        private void MainWindow_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _vm = DataContext as GardenCalendarGeneratorVm;
        }

        private async void Save_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => _vm.AddOrUpdateDates());
        }
    }
}