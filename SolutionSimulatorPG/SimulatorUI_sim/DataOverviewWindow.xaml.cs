using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using Accessibility;
using Microsoft.VisualBasic;
using SimulatorBL.DTO;
using SimulatorBL.Enum;
using SimulatorBL.Interfaces;
using SimulatorBL.Manager;

namespace SimulatorUI_sim
{
    /// <summary>
    /// Interaction logic for DataOverviewWindow.xaml
    /// </summary>
    public partial class DataOverviewWindow : Window
    {
        private DataRequestService _service;
        public MetaDataImportDTO _dataImportDTO;
       
        public DataOverviewWindow(DataRequestService service)
        {
            InitializeComponent();
            _service = service;
            var data = _service.GetAllDatasets().GroupBy(x => new { x.Country, x.versionYear })
                    .Select(g => new MetaDataImportDTO(
                        g.First().Id,
                        g.Max(x => x.ImportDate),
                        string.Join(Environment.NewLine, g.Select(x => x.SourceFile)), 
                        g.Any(x => x.IsNamesData),
                        g.Any(x => x.IsAddresData),
                        g.Key.Country,
                        g.Key.versionYear
                    )).ToList();
        
                    

            Datagrid_data.ItemsSource = data;

           
        }

        private void DataGrid_data_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            //Fixing format of dates in overview trough autogeneratingColumn method        
            if (e.PropertyName == "ImportDate")
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
            }
        }

        private void MenuItem_selectsim(object sender, RoutedEventArgs e)
        {
            _dataImportDTO = Datagrid_data.SelectedItem as MetaDataImportDTO;
            DialogResult = true;
            Close();
        }
    }
}
