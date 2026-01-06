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
        private ObservableCollection<SimulationConciseStatisticsDTO> _filteredCollection;

        public SimulationsOverviewWindow(DataRequestService service, ObservableCollection<SimulationInformation> overviewSimulations)
        {
            InitializeComponent();
            _dataRequestService = service;          
            _sourceCollection = overviewSimulations;

            // map items
            var initialItems = _sourceCollection.Select(x => DTOMapper.MapToConciseStatistics(x)).ToList();
            // make observable collection for items (for UI and for chages in main list
            // it is also the backup collection for searching
            DisplayCollection = new ObservableCollection<SimulationConciseStatisticsDTO>(initialItems);
            // put everything in filtered collection; later filtered versions will be in here
            _filteredCollection = new ObservableCollection<SimulationConciseStatisticsDTO>(initialItems); ;
            Datagrid_overviewSimulations.ItemsSource = _filteredCollection; // add to ui

            // Listen to changes in source and update UI collection
            _sourceCollection.CollectionChanged += SourceCollection_CollectionChanged;

            ComboBox_SelectionSearch.ItemsSource = new List<string>() { "client", "country" };

            combobox_clients.ItemsSource = DisplayCollection.Select(x => x.Client).Distinct().ToList();
            combobox_country.ItemsSource =  DisplayCollection.Select(x => x.Country).Distinct().ToList();
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

        private void click_buttonSearch(object sender, RoutedEventArgs e)
        {
            _filteredCollection.Clear();
            ObservableCollection<SimulationConciseStatisticsDTO> data;
            if (ComboBox_SelectionSearch.SelectedItem as string == "client")
                data = new ObservableCollection<SimulationConciseStatisticsDTO>(DisplayCollection.Where(x => x.Client == combobox_clients.SelectedItem as string).ToList());
            else if (ComboBox_SelectionSearch.SelectedItem as string == "country")
                data = new ObservableCollection<SimulationConciseStatisticsDTO>(DisplayCollection.Where(x => x.Country == combobox_country.SelectedItem as string));
            else { MessageBox.Show("Search was not completed", "invalid"); return; }

            foreach (var item in data)
            {
                _filteredCollection.Add(item);
            }
        }

        private void comoboBox_searchSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox_SelectionSearch.SelectedItem as string == "client") 
            { 
                combobox_country.Visibility = Visibility.Collapsed; 
                combobox_clients.Visibility = Visibility.Visible; 
            }
            else if (ComboBox_SelectionSearch.SelectedItem as string == "country")
            {
                combobox_country.Visibility = Visibility.Visible;
                combobox_clients.Visibility = Visibility.Collapsed;
            }
        }
    }
}
