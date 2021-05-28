using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Serial_Number_Generator
{
    internal class Sql_Query
    {
        private static Dictionary<Type, SqlDbType> typeMap;

        public void sql_data(string sql_connection, string sql_command, Dictionary<string, object> SQL_Parameters)
        {
            SqlConnection sql_InsertData = new SqlConnection(sql_connection);

            SqlCommand cmd = new SqlCommand(sql_command, sql_InsertData);
            cmd.CommandType = CommandType.StoredProcedure;

            sql_InsertData.Open();

            foreach (var item in SQL_Parameters)
            {
                try
                {
                    cmd.Parameters.Add(new SqlParameter(item.Key, GetDbType(item.Value.GetType())));
                    cmd.Parameters[item.Key].Value = item.Value;
                }
                catch
                {
                    cmd.Parameters.Add(new SqlParameter(item.Key, DBNull.Value));
                    cmd.Parameters[item.Key].Value = DBNull.Value;
                }
            }

            cmd.ExecuteNonQuery();
        }

        public DataTable Sql_Select(string sql_connection, string sql_command, Dictionary<string, object> SQL_Parameters = null)
        {
            SqlConnection sql_QueryData = new SqlConnection(sql_connection);

            SqlCommand cmd = new SqlCommand(sql_command, sql_QueryData)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (SQL_Parameters != null)
            {
                foreach (var item in SQL_Parameters)
                {
                    cmd.Parameters.Add(new SqlParameter(item.Key, item.Value));
                }
            }

            sql_QueryData.Open();
            DataTable dataTable = new DataTable();
            SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
            dataAdapter.Fill(dataTable);
            sql_QueryData.Close();

            return dataTable;
        }

        public static SqlDbType GetDbType(Type giveType)
        {
            typeMap = new Dictionary<Type, SqlDbType>();

            typeMap[typeof(string)] = SqlDbType.NVarChar;
            typeMap[typeof(char[])] = SqlDbType.NVarChar;
            typeMap[typeof(byte)] = SqlDbType.TinyInt;
            typeMap[typeof(short)] = SqlDbType.SmallInt;
            typeMap[typeof(int)] = SqlDbType.Int;
            typeMap[typeof(long)] = SqlDbType.BigInt;
            typeMap[typeof(byte[])] = SqlDbType.Image;
            typeMap[typeof(bool)] = SqlDbType.Bit;
            typeMap[typeof(DateTime)] = SqlDbType.DateTime2;
            typeMap[typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset;
            typeMap[typeof(decimal)] = SqlDbType.Money;
            typeMap[typeof(float)] = SqlDbType.Real;
            typeMap[typeof(double)] = SqlDbType.Float;
            typeMap[typeof(TimeSpan)] = SqlDbType.Time;
            typeMap[typeof(Guid)] = SqlDbType.UniqueIdentifier;

            // Allow nullable types to be handled
            giveType = Nullable.GetUnderlyingType(giveType) ?? giveType;

            if (typeMap.ContainsKey(giveType))
            {
                return typeMap[giveType];
            }

            throw new ArgumentException($"{giveType.FullName} is not a supported .NET class");
        }
    }
}