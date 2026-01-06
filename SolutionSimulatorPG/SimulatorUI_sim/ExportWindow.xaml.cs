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
using Microsoft.Win32;
using SimulatorBL.Domain;
using SimulatorBL.DTO;
using SimulatorBL.Enum;
using SimulatorBL.Exceptions;
using SimulatorBL.Factories;
using SimulatorBL.Interfaces;
using SimulatorBL.Manager;
using SimulatorBL.Services.Export;

namespace SimulatorUI_sim
{
    /// <summary>
    /// Interaction logic for ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        private ExportConfiguration _config;
        private SimulationInformation _simulationInformationDTO;
        private DataRequestService _dataRequestService;
        public ExportWindow(SimulationInformation simulationInformationDTO, DataRequestService service)
        {
            InitializeComponent();
            _dataRequestService = service;
            _simulationInformationDTO = simulationInformationDTO;
            _config = new ExportConfiguration();
            this.DataContext = _config;

            ComboBox_FileType.ItemsSource = Enum.GetValues(typeof(FileType));
        }

        private void Click_buttonExport(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_config.TextSeparator) && _config.FileType == FileType.TextorCSV)
            {
                MessageBox.Show("Please choose seperator", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }

            _config.Path = null;
            while (_config.Path == null){ _config.Path = SelectPath(); }

            try
            {
                IExportService service = ExportServiceFactory.GetExportService(_config.FileType);

                List<Customer>? customers = null;
                if (_config.IncludeFullInformation || _config.FileType == FileType.Json) customers = _dataRequestService.GetSpecificCustomers(_simulationInformationDTO.Id);
                bool result = service.Export(_simulationInformationDTO, _config, customers);
                if (result) { MessageBox.Show("Export succesfull!"); Close(); }
            }
            catch {MessageBox.Show("Error", "Unable to export simulation", MessageBoxButton.OK, MessageBoxImage.Warning); }

            
        }

        private string SelectPath()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();

            dialog.Title = "Select directory for saving file(s).";
            dialog.Multiselect = false;
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                string selPath = dialog.FolderName;
                return selPath;               
            }
            else { MessageBox.Show("Select path", "Please select a path for saving"); return null; }

        
        }

        private void SelectionChanged_Filetype(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (Enum.Parse<FileType>(ComboBox_FileType.SelectedItem.ToString()) == FileType.Json)
                {
                    _config.FileType = FileType.Json;
                    Textbox_seperator.Visibility = Visibility.Collapsed;
                    Textblock_seperator.Visibility = Visibility.Collapsed;
                    Checkbox_fullInfo.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _config.FileType = FileType.TextorCSV;
                    Textbox_seperator.Visibility = Visibility.Visible;
                    Textblock_seperator.Visibility = Visibility.Visible;
                    Checkbox_fullInfo.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex) { throw new ExportException(ex.Message); }
        }
    }
}
