using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SimulatorBL.DTO;
using SimulatorBL.Manager;

namespace SimulatorUI_sim
{
    /// <summary>
    /// Interaction logic for SimulationFullStatisticsWindow.xaml
    /// </summary>
    public partial class SimulationFullStatisticsWindow : Window
    {
        private SimulationInformation _simulationInformationDTO;
        private DataRequestService _dataRequestService;
        public SimulationFullStatisticsWindow(SimulationInformation simulationInformationDTO, DataRequestService service)
        {
            InitializeComponent();
            _dataRequestService = service;
            _simulationInformationDTO = simulationInformationDTO;
            this.DataContext = _simulationInformationDTO;
        }

        private void Click_buttonExport(object sender, RoutedEventArgs e)
        {
            ExportWindow w = new ExportWindow(_simulationInformationDTO, _dataRequestService );
            w.Show();
        }


    }
}
