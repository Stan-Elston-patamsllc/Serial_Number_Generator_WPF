using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Serial_Number_Generator
{
    internal class Serial_Number
    {
        private const string DATABASE = "PAS_Logs";

        // Stored Procedures
        private const string GET_REPRINTED_SN = "SP_Get_Reprinted_SN";

        private const string NUMBER_OF_LEADING_ZEROS = "D4";

        private const string SERIAL_NUM_COUNT = "SP_Serial_Num_Count";

        // SQL Columns from PAS_Serial_Log Table
        private const string serial_number_column_name = "S/N Finish";

        private const string SERVER = "PAS-SQL";

        private const string SQL_CONNECTION = "Server=" + SERVER +
                              ";Database=" + DATABASE +
                              ";Integrated Security=True;";

        private const string sql_stored_procedure = "SP_Next_Serial_Number";

        //SQL Table
        private const string SQL_TABLE = "PAS_Kit_ID_Log";

        // Constants
        private const int start_serial_num = 1;

        //FIELDS

        private int count;
        private const string two_digit_year = "yy";
        private int week_number;
        private string year;
        private string year_week;

        private string sn;
        private int sn_end;
        private int sn_start;
        private string week;

        public void Generate_Serial_Numbers(int How_Many, bool Insert_SQL = false, bool Fetch_Serial = true)
        {
            SN_Data.Date = DateTime.Now;

            // Get the current week number of the year
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;
            week_number = cal.GetWeekOfYear(SN_Data.Date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            year = SN_Data.Date.ToString(two_digit_year, CultureInfo.InvariantCulture);
            year_week = year + week_number.ToString();
            SN_Data.Week = int.Parse(year_week);

            // Set sn to "1" incase SQL Table is empty
            sn = "1";

            // Serial Number Prefix
            SN_Data.Prefix = "P" + SN_Data.Week.ToString();

            if (Fetch_Serial)
            {
                New_Serial();
            }
            else
            {
                try
                {
                    sn = int.Parse(SN_Data.SN_Start).ToString();
                }
                catch
                {
                    sn = "1";
                }

                SN_Data.SN_Start = int.Parse(sn).ToString(NUMBER_OF_LEADING_ZEROS);
            }

            for (int i = 0; i < How_Many; i++)
            {
                sn_end = int.Parse(sn) + i;

                if (Insert_SQL)
                {
                    SqlConnection sql_SaveData = new SqlConnection(SQL_CONNECTION);

                    SqlCommand cmd = new SqlCommand("SP_Insert_Serial_Numbers", sql_SaveData)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@ID", SN_Data.ID);
                    cmd.Parameters.AddWithValue("@sn", SN_Data.Prefix + sn_end.ToString(NUMBER_OF_LEADING_ZEROS));

                    sql_SaveData.Open();
                    cmd.ExecuteNonQuery();
                    sql_SaveData.Close();
                }
            }

            SN_Data.SN_Finish = sn_end.ToString(NUMBER_OF_LEADING_ZEROS);
            SN_Data.Full_SN_Finish = SN_Data.Prefix + SN_Data.SN_Finish;
            SN_Data.Full_SN_Start = SN_Data.Prefix + SN_Data.SN_Start;

            //SN_Data.Full_SN_Start = prefix + SN_Data.SN_Finish;
            //SN_Data.Full_SN_Finish = prefix + SN_Data.Week;
        }

        private void New_Serial()
        {
            SqlConnection sql_QueryData = new SqlConnection(SQL_CONNECTION);

            SqlCommand cmd = new SqlCommand(sql_stored_procedure, sql_QueryData)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@week", (SN_Data.Week));

            sql_QueryData.Open();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);
            sql_QueryData.Close();

            foreach (DataRow row in dataTable.Rows)
            {
                sn = row["SN_Finish"].ToString();
                week = row["Week"].ToString();
            }

            // Reset the start serial number to 0001 if it is a new week
            if (week != SN_Data.Week.ToString())
            {
                sn_start = start_serial_num;
            }
            else
            {
                sn_start = int.Parse(sn) + 1;
            }

            SN_Data.SN_Start = sn_start.ToString(NUMBER_OF_LEADING_ZEROS);
        }
    }
}