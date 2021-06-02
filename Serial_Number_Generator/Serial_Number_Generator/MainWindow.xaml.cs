using System;
using System.Collections.Generic;
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

        // Stored Procedures
        public const string GET_KIT_IDS = "SP_Get_Kit_IDs";

        private const string INSERT_SN_RANGE = "SP_Insert_SN_Range";
        private const string GET_KIT_INFO = "SP_Get_Kit_Info";
        private const string CHECK_SN = "SP_Check_SN";

        // SQL Columns
        private const string KITID_COLUMN = "Kit ID";

        private const string ASSY_NUM_COLUMN = "Assy No";
        private const string CUSTOMER_COLUMN = "Customer";
        private const string CUSTOMER_PO_COLUMN = "Cust PO";
        private const string ID_COLUMN = "ID";

        // Wash Types
        private const string LEAD_FREE_WASH = "LFW";

        private const string number_of_leading_zeros = "D4";
        private const string PROC_CODE = "Proc Code";
        private const string QTY_COLUMN = "QTY";
        private const string REV_COLUMN = "Rev";
        private const string start_serial_letter = "P";
        private const string TIN_LEAD_NO_CLEAN = "TLN";
        private const string TIN_LEAD_WASH = "TLW";
        private const string LEAD_FREE_NO_CLEAN = "LFN";
        private const string week_column_name = "Week";

        public MainWindow()
        {
            InitializeComponent();
            FillComboBox();
            PAS_Serial_Num_Radio_Button.IsChecked = true;
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

        private void Kit_ID_Combobox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            SN_Data.Kit_ID = Kit_ID_Combobox.SelectedValue.ToString();

            // Create Dictionary
            Dictionary<string, object> Sql_Parameters = new Dictionary<string, object>();

            // Add SQL Parameters Below
            Sql_Parameters.Add("@id", Kit_ID_Combobox.SelectedValue.ToString());

            Sql_Query query = new Sql_Query();
            DataTable dataTable = query.Sql_Select(SQL_CONNECTION, GET_KIT_INFO, Sql_Parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                MainWindow checkboxes = new MainWindow
();
                Set_Checkboxes(row[PROC_CODE].ToString());

                SN_Data.Customer = row[CUSTOMER_COLUMN].ToString();
                SN_Data.Assembly = row[ASSY_NUM_COLUMN].ToString();

                SN_Data.PO = row[CUSTOMER_PO_COLUMN].ToString();
                SN_Data.ID = Guid.Parse(row[ID_COLUMN].ToString());

                if (row[REV_COLUMN].ToString() == "")
                {
                    SN_Data.Rev = "-";
                }
                else
                {
                    SN_Data.Rev = row[REV_COLUMN].ToString();
                }

                try
                {
                    SN_Data.QTY = int.Parse(row[QTY_COLUMN].ToString());
                }
                catch
                {
                    SN_Data.QTY = 1;
                }
            }

            Customer_Textbox.Text = SN_Data.Customer;
            Assembly_Textbox.Text = SN_Data.Assembly;
            Rev_Textbox.Text = SN_Data.Rev;
            Pur_Order_Textbox.Text = SN_Data.PO;

            Serial_Number serial = new Serial_Number();
            serial.Generate_Serial_Numbers(SN_Data.QTY);

            if (SN_Data.Customer_SN)
            {
                SN_Data.Prefix = null;
                SN_Data.SN_Start = null;
                SN_Data.SN_Finish = null;

                Prefix_TextBox.Text = null;
                SN_TextBox.Text = null;
                Suffix_TextBox.Text = null;
                Suffix_TextBox.IsEnabled = true;
            }
            else
            {
                Prefix_TextBox.Text = SN_Data.Prefix;
                SN_TextBox.Text = SN_Data.SN_Start;
                Suffix_TextBox.Text = null;
                Suffix_TextBox.IsEnabled = false;
            }

            // Create Dictionary
            Dictionary<string, object> Sql_Parameters2 = new Dictionary<string, object>();

            // Add SQL Parameters Below
            Sql_Parameters2.Add("@ID", SN_Data.ID.ToString());

            Sql_Query query2 = new Sql_Query();
            DataTable dataTable2 = query2.Sql_Select(SQL_CONNECTION, CHECK_SN, Sql_Parameters2);

            int count = int.Parse(dataTable2.Rows[0][0].ToString());

            if (count > 0)
            {
                Qty_IntegerUpDown.Value = count;
                Qty_IntegerUpDown.Maximum = count;
                Qty_IntegerUpDown.IsEnabled = true;
            }
            else if (count == 0)
            {
                Qty_IntegerUpDown.Value = 0;
                Qty_IntegerUpDown.Maximum = 0;
                Qty_IntegerUpDown.IsEnabled = true;
            }
            else
            {
                Reprint_Button.IsEnabled = false;
                Qty_IntegerUpDown.Maximum = SN_Data.QTY;
                Qty_IntegerUpDown.Value = SN_Data.QTY;
            }
        }

        private void Set_Checkboxes(string code)
        {
            Lead_Free_Checkbox.IsChecked = false;
            Tin_Lead_Checkbox.IsChecked = false;
            Halogen_Free_Checkbox.IsChecked = false;
            No_Clean_Checkbox.IsChecked = false;
            Conformal_Coated_Checkbox.IsChecked = false;

            if (code == LEAD_FREE_WASH)
            {
                Lead_Free_Checkbox.IsChecked = true;
            }
            else if (code == TIN_LEAD_WASH)
            {
                Tin_Lead_Checkbox.IsChecked = true;
            }
            else if (code == TIN_LEAD_NO_CLEAN)
            {
                Tin_Lead_Checkbox.IsChecked = true;
                No_Clean_Checkbox.IsChecked = true;
            }
            else if (code == LEAD_FREE_NO_CLEAN)
            {
                Lead_Free_Checkbox.IsChecked = true;
                Lead_Free_Checkbox.IsChecked = true;
            }
        }

        private void PAS_Serial_Num_Radio_Button_Checked(object sender, RoutedEventArgs e)
        {
            SN_Data.Customer_SN = false;
            SN_Data.Prefix = null;
            SN_Data.SN_Start = null;
            SN_Data.SN_Finish = null;

            Prefix_TextBox.Text = null;
            SN_TextBox.Text = null;
            Suffix_TextBox.Text = null;
            Suffix_TextBox.IsEnabled = false;
        }

        private void Customer_Serial_Num_Radio_Button_Checked(object sender, RoutedEventArgs e)
        {
            SN_Data.Customer_SN = true;
            Prefix_TextBox.Text = null;
            SN_TextBox.Text = null;
            Suffix_TextBox.Text = null;
            Suffix_TextBox.IsEnabled = true;
        }

        private void Create_Button_Click(object sender, RoutedEventArgs e)
        {
            //Reset NumbericUpDown1
            Qty_IntegerUpDown.Maximum = 99999;

            // Create Dictionary
            Dictionary<string, object> Sql_Parameters = new Dictionary<string, object>();

            // Add SQL Parameters Below
            Sql_Parameters.Add("@ID", SN_Data.ID.ToString());

            Sql_Query query = new Sql_Query();
            DataTable dataTable = query.Sql_Select(SQL_CONNECTION, CHECK_SN, Sql_Parameters);
            int count = int.Parse(dataTable.Rows[0][0].ToString());

            if (count == 0)
            {
                //Error_Form error_Form = new Error_Form();
                //error_Form.Show();
            }
            else
            {
                // Create Dictionary
                Dictionary<string, object> Sql_Parameters2 = new Dictionary<string, object>();

                // Add SQL Parameters Below
                Sql_Parameters2.Add("@ID", SN_Data.ID.ToString());

                Sql_Parameters2.Add("@date_", (SN_Data.Date));
                Sql_Parameters2.Add("@kit_id", (SN_Data.Kit_ID));
                Sql_Parameters2.Add("@qty", (SN_Data.QTY));
                Sql_Parameters2.Add("@sn_start", (SN_Data.SN_Start));
                Sql_Parameters2.Add("@sn_finish", (SN_Data.SN_Finish));
                Sql_Parameters2.Add("@week", (SN_Data.Week));
                Sql_Parameters2.Add("@customer", (SN_Data.Customer));
                Sql_Query query2 = new Sql_Query();

                Serial_Number serial = new Serial_Number();
                serial.Generate_Serial_Numbers(SN_Data.QTY, true);

                query2.sql_data(SQL_CONNECTION, INSERT_SN_RANGE, Sql_Parameters2);

                Label_Print traveler = new Label_Print();
                traveler.print_traveler(false);
            }
        }
    }
}