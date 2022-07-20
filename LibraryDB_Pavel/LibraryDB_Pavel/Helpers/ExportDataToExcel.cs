using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LibraryDB_Pavel.Utils.Constants;

namespace LibraryDB_Pavel.Helpers
{
    public class ExportDataToExcel
    {
        public static DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                var type = (prop.PropertyType.IsGenericType &&
                            prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? Nullable.GetUnderlyingType(prop.PropertyType)
                    : prop.PropertyType);
                dataTable.Columns.Add(prop.Name, type);
            }

            foreach (var item in items)
            {
                var values = new object[properties.Length];
                for (var i = 0; i < properties.Length; i++)
                {
                    values[i] = properties[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static void ToExcelFile(DataTable dataTable, string filePath, bool overwriteFile = true)
        {
            if (File.Exists(filePath) && overwriteFile)
                File.Delete(filePath);

            using (var connection = new OleDbConnection())
            {
                connection.ConnectionString = string.Format(BookConstants.ExcelPathConstant, filePath);
                connection.Open();
                using (var command = new OleDbCommand())
                {
                    command.Connection = connection;
                    var columnNames = (from DataColumn dataColumn in dataTable.Columns select dataColumn.ColumnName)
                        .ToList();
                    var tableName = !string.IsNullOrWhiteSpace(dataTable.TableName)
                        ? dataTable.TableName
                        : Guid.NewGuid().ToString();
                    command.CommandText =
                        $"CREATE TABLE [{tableName}] ({string.Join(",", columnNames.Select(c => $"[{c}] VARCHAR").ToArray())});";
                    command.ExecuteNonQuery();
                    foreach (DataRow row in dataTable.Rows)
                    {
                        var rowValues = (from DataColumn column in dataTable.Columns
                                         select (row[column] != null && row[column] != DBNull.Value)
                                             ? row[column].ToString()
                                             : string.Empty).ToList();
                        command.CommandText =
                            $"INSERT INTO [{tableName}]({string.Join(",", columnNames.Select(c => $"[{c}]"))}) VALUES ({string.Join(",", rowValues.Select(r => $"'{r}'").ToArray())});";
                        command.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }
        }

    }
}
