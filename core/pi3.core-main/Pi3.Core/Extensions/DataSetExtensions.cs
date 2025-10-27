// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Extensions
{
    public static class DataSetExtensions
    {
        public static DataTable AsDataTable<T>(this IEnumerable<T> list, string? tableName = null) where T : class
        {
            DataTable dataTable = null;

            if (!string.IsNullOrWhiteSpace(tableName))
                dataTable = new DataTable(tableName);
            else
                dataTable = new DataTable();

            foreach (var p in typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
            {
                Type columnType = null;
                bool allowDBNull = false;

                if (p.PropertyType.IsNullableType())
                {
                    columnType = Nullable.GetUnderlyingType(p.PropertyType);
                    allowDBNull = true;
                }
                else
                {
                    columnType = p.PropertyType;
                    allowDBNull = (p.PropertyType == typeof(string));
                }

                dataTable.Columns.Add(new DataColumn(p.Name, columnType)
                {
                    AllowDBNull = allowDBNull
                });
            }

            foreach (var itm in list)
            {
                DataRow dataRow = dataTable.NewRow();

                foreach (DataColumn column in dataTable.Columns)
                {
                    var p = itm.GetType().GetProperty(column.ColumnName);

                    var value = p.GetValue(itm);
                    if (value == null)
                        dataRow[column] = DBNull.Value;
                    else
                        dataRow[column] = value;
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }

        public static DataSet AsDataSet<T>(this IEnumerable<T> list, string? dataSetName = null, string? tableName = null) where T : class
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            DataSet dataSet = null;
            if (!string.IsNullOrWhiteSpace(dataSetName))
                dataSet = new DataSet(dataSetName);
            else
                dataSet = new DataSet();

            var dataTable = list.AsDataTable(tableName);
            
            dataSet.Tables.Add(dataTable);
            
            return dataSet;
        }
    }
}
