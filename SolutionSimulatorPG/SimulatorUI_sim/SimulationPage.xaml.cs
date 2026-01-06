using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using SimulatorBL.Enum;
using SimulatorBL.Exceptions;
using SimulatorBL.Interfaces;
using SimulatorBL.Manager;
using SimulatorUI_sim.DomainUI;
using SimulatorUtils;

namespace SimulatorUI_sim
{
    /// <summary>
    /// Interaction logic for SimulationPage.xaml
    /// </summary>
    public partial class SimulationPage : Window
    {
        private DataRequestService _requestService;
        private ICustomerGenerator _service;
        private MetaDataImportDTO _data;
        private ObservableCollection<SimulationInformation> _simulationInformationDTOs;
        private ObservableCollection<MunicipalitySelection> _municipalitiesPercentages;

        public SimulationPage(int? mySeed, MetaDataImportDTO data, ObservableCollection<SimulationInformation> overviewSimulations)
        {
            InitializeComponent();
            this._simulationInformationDTOs = overviewSimulations;
            this._data = data;
            
            this._requestService = new DataRequestService(RepoFactory.GetRequestRepo()) ;
            int? seed = (mySeed == null)? Environment.TickCount : mySeed;
            this._service = RepoFactory.GetSimulator(_requestService, (int)seed);

            _municipalitiesPercentages = new ObservableCollection<MunicipalitySelection>();
            DataGrid_municipalityPerc.ItemsSource = _municipalitiesPercentages;

            if (_data != null)
            {
                Textbox_country.Text = _data.Country;
                Textbox_year.Text = _data.versionYear.ToString();
            }
        }

        private void Click_startSimulation(object sender, RoutedEventArgs e)
        {
            if (_municipalitiesPercentages.Sum(x => x.Percentage) < 100)
            {
                MessageBox.Show("You need a total of 100%", "Percentage");
                return;
            }
            
            bool succes = false;
            try
            {
                string clientName = TextBox_clientName.Text;
                int year = int.Parse(Textbox_year.Text);
                string country = Textbox_country.Text;
                int minAge = int.Parse(TextBox_minAge.Text);
                int maxAge = int.Parse(TextBox_maxAge.Text);
                int numberOfCust = int.Parse(TextBox_amount.Text);
                Dictionary<string, int> municipalityPerc = _municipalitiesPercentages.ToDictionary(x => x.Name, x => x.Percentage);
                int maxHousenr = int.Parse(TextBox_maxHouseNr.Text);
                int percentageLetters = int.Parse(TextBox_percLetterInNr.Text);

                                                                                                            
                succes = _service.StartSimulation(clientName, year, country, minAge, maxAge, numberOfCust, municipalityPerc, maxHousenr, percentageLetters, _simulationInformationDTOs);
            }
            catch (Exception ex) {
                MessageBox.Show("Please fill in all fields.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning); }               

            

            if (succes)
            {
                MessageBox.Show("Simulation succeeded", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearAllFields();
            }
        }



        private void ClearAllFields()
        {
            TextBox_clientName.Clear();
            TextBox_minAge.Clear();
            TextBox_maxAge.Clear();
            TextBox_amount.Clear();
            TextBox_maxHouseNr.Clear();
            TextBox_percLetterInNr.Clear();
            TextBox_percentage.Clear();
            Textbox_year.Clear();
            Textbox_country.Clear();
            ComboBox_selMunicipality.SelectedIndex = -1;
            _municipalitiesPercentages.Clear();
        }



        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            TextBox box = sender as TextBox;
            // gets text in box + tect that just got typed
            string nieuweTekst = box.Text + e.Text;
            Regex regex = new Regex("^[1-9][0-9]*$");                     
            // if text doesnt match regex, block input if there is no match (and switch bool for blocking)
            e.Handled = !regex.IsMatch(nieuweTekst);         
        }

        private void Button_clickChooseDataset(object sender, RoutedEventArgs e)
        {
            DataOverviewWindow w = new DataOverviewWindow(_requestService);
            bool? result = w.ShowDialog();

            if (result == true)
            {
                _data = w._dataImportDTO;
                Textbox_country.Text = _data.Country;
                Textbox_year.Text = _data.versionYear.ToString();
            }

        }


        private void Country_changed(object sender, TextChangedEventArgs e)
        {            
            string country = Textbox_country.Text;
            ComboBox_selMunicipality.ItemsSource = _requestService.GetAllMunicipalities(country);
        }

        private void Button_Select_muniPercentage(object sender, RoutedEventArgs e)
        {
            try
            {
                int percentage = int.Parse(TextBox_percentage.Text);
                if (_municipalitiesPercentages.Sum(x => x.Percentage) + percentage > 100) { MessageBox.Show("Max percentage is 100%", "max percentage"); return; }            
                string municipality = ComboBox_selMunicipality.Text;
                if (string.IsNullOrEmpty(municipality)) throw new SimulationException();

                //if it already exists: skip adding and notify user
                if (!_municipalitiesPercentages.Any(x => x.Name == municipality))
                {
                    _municipalitiesPercentages.Add(new MunicipalitySelection(municipality, percentage));                   
                    ComboBox_selMunicipality.SelectedIndex = -1;
                    TextBox_percentage.Clear();
                    UpdateTotalPercentage();
                }
                else MessageBox.Show("This municipality has already been added.", "Unable to choose municipality");

            }
            catch { MessageBox.Show("Please fill in both fields", "Unable to add municipality"); }
        }

        private void UpdateTotalPercentage()
        {
            Textblock_totalPercentage.Text = _municipalitiesPercentages.Sum(x => x.Percentage).ToString() + "%";
        }

        private void Click_Deletemunicipality(object sender, RoutedEventArgs e)
        {
            var municPer = DataGrid_municipalityPerc.SelectedItem as MunicipalitySelection;
            if (municPer != null)
            {
                _municipalitiesPercentages.Remove(municPer);
                UpdateTotalPercentage();
            }
        }

    }
}
