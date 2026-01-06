using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using SimulatorBL.Interfaces;
using SimulatorBL.Manager;

namespace SimulatorUI_sim
{
    /// <summary>
    /// Interaction logic for SimulationsOverviewWindow.xaml
    /// </summary>
    public partial class SimulationsOverviewWindow : Window
    {
        DataRequestService _dataRequestService;
        private ObservableCollection<SimulationInformation> _sourceCollection;
        public ObservableCollection<SimulationConciseStatisticsDTO> DisplayCollection { get; set; }
        public SimulationsOverviewWindow(DataRequestService service, ObservableCollection<SimulationInformation> overviewSimulations)
        {
            InitializeComponent();
            _dataRequestService = service;          
            _sourceCollection = overviewSimulations;

            var initialItems = _sourceCollection.Select(x => DTOMapper.MapToConciseStatistics(x));
            DisplayCollection = new ObservableCollection<SimulationConciseStatisticsDTO>(initialItems);
            Datagrid_overviewSimulations.ItemsSource = DisplayCollection;

            // Listen to changes in source and update UI collection
            _sourceCollection.CollectionChanged += SourceCollection_CollectionChanged;
        }

        private void Click_Menuitem_ViewInfoSimulation(object sender, RoutedEventArgs e)
        {
            int selId = (Datagrid_overviewSimulations.SelectedItem as SimulationConciseStatisticsDTO).Id;
            SimulationFullStatisticsWindow w = new SimulationFullStatisticsWindow(_sourceCollection.Where(x => x.Id == selId).First(), _dataRequestService);
            w.Show();
        }

        private void SourceCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // if items get added
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (SimulationInformation newItem in e.NewItems)
                {
                    // Map new item
                    DisplayCollection.Add(DTOMapper.MapToConciseStatistics(newItem));
                }
            } 
        }
    }
}
