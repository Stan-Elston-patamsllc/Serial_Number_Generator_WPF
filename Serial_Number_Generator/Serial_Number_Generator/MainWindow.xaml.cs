using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Serial_Number_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Constants

        public const string DATABASE = "PAS_Logs";

        //SQL Table
        public const string SERVER = "PAS-SQL";

        public const string SQL_CONNECTION = "Server=" + SERVER +
                              ";Database=" + DATABASE +
                              ";Integrated Security=True;";

        public const string GET_KIT_IDS = "SP_Get_Kit_IDs";

        public MainWindow()
        {
            InitializeComponent();
            FillComboBox();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        public void FillComboBox()
        {
            Sql_Query query = new Sql_Query();
            DataTable dataTable = query.Sql_Select(SQL_CONNECTION, GET_KIT_IDS);

            Kit_ID_Combobox.ItemsSource = dataTable.AsDataView();
        }
    }
}