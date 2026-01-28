using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhoneticTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            viewModel = new MainWindowViewModel();
            DataContext = viewModel;
            InitializeComponent();
        }

        private void ButtonStartClick(object sender, RoutedEventArgs e)
        { 
            viewModel.Start();
        }

        private void ButtonStopClick(object sender, RoutedEventArgs e)
        {
            viewModel.Stop();
        }

        private void ButtonRefreshClick(object sender, RoutedEventArgs e)
        {
            viewModel.RefreshAvailableDevices();
        }
    }
}