using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using SimulatorBL.Enum;
using SimulatorBL.Manager;
using SimulatorUtils;

namespace SimulatorUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ReaderService _reader;
        private List<char> characters = new List<char>()
        {
            ';', '\t', ',', '|', '/'
        };

        private List<UIElement> _textControls; //to adapt window to specific choices (appearing/disapearing of block/text)
        DataRequestService _dataRequestService;

        public MainWindow()
        {
            InitializeComponent();
            _reader = new ReaderService(RepoFactory.GetRepo());
            _dataRequestService = new DataRequestService(RepoFactory.GetRequestRepo());

            FillInTextOnlyControls();
            Combobox_type.ItemsSource = Enum.GetNames(typeof(FileType));
            Combobox_nametype.ItemsSource = Enum.GetNames(typeof(NameType));
            Combobox_Country.ItemsSource = _dataRequestService.GetAllCountries();
                Combobox_Country.Text = "Choose country";
                Combobox_Country.Foreground = Brushes.Gray;
            ComboBox_Gender.ItemsSource = Enum.GetNames(typeof(Gender));
            ComboBox_character.ItemsSource = characters;
        }

        private void FillInTextOnlyControls()
        {
            //add controls that have to appear with text file and dissapear with json file
            _textControls = new List<UIElement>
                {
                    TextBlock_NameType,
                    TextBlock_Country,
                    TextBlock_LinesToSkip,
                    TextBlock_Separator,
                    Combobox_nametype,
                    Textblock_name,             
                    TextBox_NameColumn,
                    textblock_frequency,        
                    TextBox_frequencyColumn,
                    TextBox_linesToSkip,
                    ComboBox_character,
                    Textblock_gender,
                    ComboBox_Gender,
                    Textbox_higwayTypeColumn
                };
        }

        private void Click_ButtonUploadFile(object sender, RoutedEventArgs e)
        {
            string filetypetxt = Combobox_type.Text.ToLower().Trim();
            string nametypetxt = Combobox_nametype.Text.ToLower().Trim();
            string countrytxt = Combobox_Country.Text.ToLower().Trim();
            string path = PathTextBox.Text.ToLower().Trim();
            int linesToSkip = string.IsNullOrWhiteSpace(TextBox_linesToSkip.Text)? 0 : int.Parse(TextBox_linesToSkip.Text);
            int nameColumn = string.IsNullOrWhiteSpace(TextBox_NameColumn.Text) ? 0: int.Parse(TextBox_NameColumn.Text);
            int? frequencyColumn = string.IsNullOrWhiteSpace(TextBox_frequencyColumn.Text) ? null
                                                                : int.Parse(TextBox_frequencyColumn.Text);
            char seperator = ComboBox_character.SelectedIndex == -1 ? ',' : (char)ComboBox_character.SelectedItem;
            string gendertxt = ComboBox_Gender.Text.ToLower().Trim();

            int? highwaytypeColumn = string.IsNullOrWhiteSpace(Textbox_higwayTypeColumn.Text) ? null : int.Parse(Textbox_higwayTypeColumn.Text);

            FileType fileType;         
            NameType nameType;
            Gender gender;
            bool isFileTypeValid = Enum.TryParse(filetypetxt, true, out fileType);
            bool isNameTypeValid = Enum.TryParse(nametypetxt, true, out nameType);
            bool isGenderValid = Enum.TryParse(gendertxt, true, out gender);

            if ((string.IsNullOrEmpty(filetypetxt) || string.IsNullOrEmpty(countrytxt) || string.IsNullOrWhiteSpace(nametypetxt) || string.IsNullOrWhiteSpace(path)) && fileType == FileType.TextorCSV)
            {
                MessageBox.Show("Each box needs one selection.",
                                    "Requierements", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool readCompleted = false;
            
            if ((isFileTypeValid && isNameTypeValid) || fileType == FileType.Json)
            {                    
                if (isGenderValid || highwaytypeColumn != null || fileType == FileType.Json)
                    readCompleted = _reader.Read(path, linesToSkip, seperator, nameColumn, gender, fileType, nameType, countrytxt, frequencyColumn, highwaytypeColumn);
                //TODO fix it so it becomes a file
                if (readCompleted)
                {
                    ClearAllInputFields();
                    MessageBox.Show("Reading of the file succeeded.", "Succes", MessageBoxButton.OK);
                }
                else MessageBox.Show("Reading of the file failed.", "Failed", MessageBoxButton.OK);
            }
           
        }
                
        private void Click_ButtonGetPath(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Data Files (*.csv;*.txt;*.json)|*.csv;*.txt;*.json|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {              
                string bestandspad = openFileDialog.FileName;
                PathTextBox.Text = bestandspad;

                MessageBox.Show($"Selected path: {bestandspad}", "Path", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Please select a path", "Requierements");
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9\\s]");           //match everything that is not a number, and match a "spatie"
            e.Handled = regex.IsMatch(e.Text);              //Match: dont allow it to be typed
        }
      
        private void Data_changed_NameType(object sender, SelectionChangedEventArgs e)
        {
            if (Combobox_nametype.SelectedIndex == -1 ||
                Combobox_nametype.SelectedItem.ToString() == NameType.Firstname.ToString() || 
                Combobox_nametype.SelectedItem.ToString() == NameType.Lastname.ToString())
            {

                Textblock_name.Text = "Name Column";
                textblock_frequency.Text = "Frequency Column";

                Textbox_higwayTypeColumn.Visibility = Visibility.Collapsed;
                ComboBox_Gender.Visibility = Visibility.Visible;
                Textblock_gender.Visibility = Visibility.Visible;
                Textblock_gender.Text = "Gender";
            }
            else if (Combobox_nametype.SelectedItem.ToString() == NameType.Address.ToString())
            {


                Textblock_name.Text = "Municipality column";
                textblock_frequency.Text = "Street column";
             
                Textbox_higwayTypeColumn.Visibility = Visibility.Visible;   
                ComboBox_Gender.Visibility = Visibility.Collapsed;
                Textblock_gender.Text = "Highwaytype column";

            }
        }

        private void ClearAllInputFields()
        {            
            TextBox_NameColumn.Clear();
            TextBox_frequencyColumn.Clear();
            TextBox_linesToSkip.Clear();
            PathTextBox.Clear();
            Textbox_higwayTypeColumn.Clear();
            Combobox_type.SelectedIndex = -1;
            Combobox_nametype.SelectedIndex = -1;
            Combobox_Country.SelectedIndex = -1;
            ComboBox_character.SelectedIndex = -1;
            ComboBox_Gender.SelectedIndex = -1;
        }

        private void SelectionChanged_FileType(object sender, SelectionChangedEventArgs e)
        {
            if (Combobox_type.SelectedItem == null) return;

            //get type
            string selectedString = Combobox_type.SelectedItem.ToString();

            if (Enum.TryParse(selectedString, out FileType selectedType))
            {
                // if JSON: Collapsed else Visible + changing all elements in controls list
                Visibility targetVisibility = (selectedType == FileType.Json) ? Visibility.Collapsed : Visibility.Visible;
                foreach (var element in _textControls)
                {
                    element.Visibility = targetVisibility;
                }

                // EXTRA CHECK: when going back to text
                // check if it's address or name to choos what we show               
                if (selectedType != FileType.Json)
                {
                    Data_changed_NameType(null, null);
                }
            }
        }

        private string _placeholderText = "Choose country";

        private void Combobox_Country_GotFocus(object sender, RoutedEventArgs e)
        {      
            //if text is still placeholder
            if (Combobox_Country.Text == _placeholderText)
            {
                Combobox_Country.Text = string.Empty; 
                Combobox_Country.Foreground = Brushes.Black; // color black
            }
        }

        private void Combobox_Country_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Combobox_Country.Text))
            {
                Combobox_Country.Text = _placeholderText; // put placeholder back
                Combobox_Country.Foreground = Brushes.Gray; //color = grey
            }
        }
    }
}