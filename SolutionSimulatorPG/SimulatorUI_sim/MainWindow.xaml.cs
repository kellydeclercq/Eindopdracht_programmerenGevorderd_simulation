using System.Collections.ObjectModel;
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
using SimulatorBL.DTO;
using SimulatorBL.Interfaces;
using SimulatorBL.Manager;
using SimulatorUtils;

namespace SimulatorUI_sim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataRequestService _service;     
        private ObservableCollection<SimulationInformation> _overviewSimulations;
        public MainWindow()
        {
            InitializeComponent();         
            _service = new DataRequestService(RepoFactory.GetRequestRepo());
        }

        private void Click_startSimulation(object sender, RoutedEventArgs e)
        {
            // only make observable collection if it has not yet been made.
            // Not in constructor in case user never needs simulationinformation
            // result: less query's to DB
            if(_overviewSimulations == null) _overviewSimulations = new ObservableCollection<SimulationInformation>(_service.GetAllSimulations());
            SimulationPage s =  new SimulationPage(null, null, _overviewSimulations);
            s.ShowDialog();

        }

       

        private void Click_overviewSimulations(object sender, RoutedEventArgs e)
        {
            if (_overviewSimulations == null) _overviewSimulations = new ObservableCollection<SimulationInformation>(_service.GetAllSimulations());
            SimulationsOverviewWindow w = new SimulationsOverviewWindow(_service, _overviewSimulations);
            w.Show();
        }

        private void Click_viewDataOverview(object sender, RoutedEventArgs e)
        {
           
            DataOverviewWindow w = new DataOverviewWindow(_service);
            bool? result = w.ShowDialog();
            if (result == true)
            {
                if (_overviewSimulations == null) _overviewSimulations = new ObservableCollection<SimulationInformation>(_service.GetAllSimulations());
                MetaDataImportDTO data = w._dataImportDTO;
                SimulationPage sim = new(null, data, _overviewSimulations);
                sim.Show();
            }
        }
    }
}