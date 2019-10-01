using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;
using System.Collections;
using System.Data;
using NttDataWA.UIManager;

namespace NttDataWA.ImportExport
{
    public partial class ExportProject : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Questo metodo si occupa di interpretare il report di
        /// esportazione e crea un log
        /// </summary>
        /// <param name="report"></param>
        public void PrintLog(string report)
        {
            // Creo un dataset
            DataSet ds = new DataSet();
            // Creo una datatable
            DataTable table = new DataTable();

            // Aggiungo la colonna per ospitare il tipo di oggetto
            table.Columns.Add("Type", typeof(string));
            // Aggiungo la colonna per ospitare il path
            table.Columns.Add("Path", typeof(string));
            // Aggiungo la colonna per ospitare l'esito
            table.Columns.Add("Result", typeof(string));
            // Aggiungo la colonna per ospitare le note
            table.Columns.Add("Notes", typeof(string));
            // Aggiungo il data table al data set
            ds.Tables.Add(table);

            using (StringReader reader = new StringReader(report))
            {
                // Leggo una linea di testo
                string line = reader.ReadLine();

                // Finchè ci sono linee di testo...
                while (!string.IsNullOrEmpty(line))
                {
                    // ...creo una riga nel data table...
                    DataRow row = table.NewRow();
                    // ...imposto i valori per la riga corrente...
                    this.FetchRowEntry(line, row);
                    // ...aggiungo la riga alla tabella...
                    table.Rows.Add(row);
                    // ...leggo la prossima riga
                    line = reader.ReadLine();

                }

            }

            // Se c'è almeno una riga...
            if(table.Rows.Count > 0)
                // ...elimino l'ultima in quanto è una riga vuota
                table.Rows.RemoveAt(table.Rows.Count - 1);

            Session["dsData"] = ds;

            // Imposto il data set come sorgente dati per il data grid
            this.dgrLog.DataSource = ds;
            // Effettuo il bind con la sorgente dati
            this.dgrLog.DataBind();

        }

        /// <summary>
        /// Questa funzione interpreta i dati presenti in una stringa
        /// e li aggiunge alla riga della data table
        /// </summary>
        /// <param name="line">La linea di testo da interpretare</param>
        /// <param name="row">La riga del data table a cui aggiundere
        ///     i dati interpretati</param>
        private void FetchRowEntry(string line, DataRow row)
        {
            // Ogni riga tranne l'ultima rispetta il pattern
            // type path result notes

            // Splitto la riga
            string[] splitted = line.Split(new String[] { "@-@" }, StringSplitOptions.None);

            // Se la riga contiene quattro valori di cui il primo è pari a 
            // Directory o File...
            if (splitted.Length == 4 &&
                (splitted[0].Equals("Directory") ||
                    splitted[0].Equals("File")))
            {
                // Imposto il tipo di oggetto
                row["Type"] = splitted[0];
                // Imposto il path
                row["Path"] = splitted[1];
                // Imposto il result
                row["Result"] = splitted[2];
                // Imposto le note
                row["Notes"] = splitted[3];
            }

            else
                // Se l'array ha tre elementi di cui il primo è pari
                // a Result...
                if (splitted.Length == 3 &&
                    splitted[0].Equals("Result"))
                {
                    // ...visualizzo un messaggio con il report
                    //msg_result.Alert(splitted[2].Replace("\\", "\\\\"));
                }
        }

    }

}