using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using BusinessLogic.Reporting.Exceptions;
using DocsPaVO.Report;

namespace BusinessLogic.Reporting.Behaviors.DataExtraction
{
    class ObjectTrasformationDataExtractionBehavior : IReportDataExtractionBehavior
    {
        public System.Data.DataSet ExtractData(DocsPaVO.Report.PrintReportRequest request)
        {
            // Casting dell'oggetto request ad oggetto PrintReportRequestObjectTrasformation
            PrintReportObjectTransformationRequest casted = request as PrintReportObjectTransformationRequest;

            // Se casted è null -> eccezione
            if (casted == null)
                throw new RequestNotValidException();

            // Inizializzazione del dataset (una tabella con una colonna per ogni proprietà da esportare)
            DataSet dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            dataSet.Tables.Add(dataTable);

            // Esplorazione dell'oggetto da cui prelevare i dati e creazione delle colonne
            // nella tabella del dataset e popolamento del dataset
            if (casted.DataObject != null && casted.DataObject.Count > 0)
            {
                this.InitializeTable(casted.DataObject[0], dataTable);
                this.PopulateDataSet(dataTable, casted.DataObject);
            }

            // Restituzione del dataset inizializzato
            return dataSet;

        }

        /// <summary>
        /// Metodo per l'inizializzazione delle colonne del dataset
        /// </summary>
        /// <param name="obj">Oggetto da cui estrarre le informazioni sulle colonne da creare</param>
        /// <param name="dataTable">Datatable in cui impostare le colonne</param>
        private void InitializeTable(object obj, DataTable dataTable)
        {
            // Prelevamento delle proprietà decorate con l'attributo PropertyToExportAttribute
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties().Where(p => p.GetCustomAttributes(typeof(PropertyToExportAttribute), false).Count() == 1).ToArray<PropertyInfo>();

            // Per ogni proprietà, viene prelevato l'attributo PropertyToExport che la decora e viene
            // costruita una DataColumn con nome pari al nome della proprietà e nome colonna e tipo dato pari al nome
            // e al tipo dato impostati nell'attributo
            foreach (PropertyInfo property in properties)
            {
                PropertyToExportAttribute propertyAttribute = property.GetCustomAttributes(typeof(PropertyToExportAttribute), false)[0] as PropertyToExportAttribute;

                // Impostazione di nome e tipo colonna
                DataColumn dataColumn = new DataColumn(propertyAttribute.Name, propertyAttribute.Type);

                // Impostazione del valore da mostrare
                dataColumn.Caption = propertyAttribute.Name;

                // Aggiunta della colonna alla collection delle colonne
                dataTable.Columns.Add(dataColumn);

            }

        }

        /// <summary>
        /// Metodo per il popolamento del dataset
        /// </summary>
        /// <param name="dataTable">Tabella da popolare</param>
        /// <param name="objList">Lista di oggetti da cui prelevare i dati</param>
        private void PopulateDataSet(DataTable dataTable, List<Object> objList)
        {
            foreach (object obj in objList)
            {
                // Prelevamento delle proprietà decorate con l'attributo PropertyToExportAttribute
                PropertyInfo[] properties = (PropertyInfo[])obj.GetType().GetProperties().Where(p => p.GetCustomAttributes(typeof(PropertyToExportAttribute), false).Count() > 0).ToArray<PropertyInfo>();

                DataRow row = dataTable.NewRow();
                foreach (PropertyInfo prop in properties)
                {
                    String columnName = ((PropertyToExportAttribute)(prop.GetCustomAttributes(typeof(PropertyToExportAttribute), false)[0])).Name;
                    row[columnName] = prop.GetValue(obj, null).ToString().Replace("<br />", " ");
                }
                dataTable.Rows.Add(row);
                
            }
            
        }

    }
}