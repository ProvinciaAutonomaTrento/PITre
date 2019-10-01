using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.utils;

namespace SAAdminTool.UserControls
{
    public partial class GridsCheckBox : System.Web.UI.UserControl
    {
        protected void chkSelectDeselect_CheckedChanged(object sender, EventArgs e)
        {
            // Campo nascosto in cui è presente il system id dell'oggetto di cui cambiare
            // lo stato di selezione e checkbox per il cambio stato
            HiddenField hiddenField;
            CheckBox checkBox;

            // Casting della checkbox
            checkBox = sender as CheckBox;

            // Recupero del campo nascosto, situato nel parent della checkbox sender
            hiddenField = checkBox.NamingContainer.FindControl("hfObjectId") as HiddenField;

            // Impostazione dello stato di selezione dell'item selezionato
            ((NewMassiveOperationButtons)Page.FindControl(this.MassiveOperationButtonsId)).SetState(hiddenField.Value, checkBox.Checked);

        }
        /// <summary>
        /// Durante il databinding del campo nascosto, viene impostato come valore del
        /// campo nascosto stesso il system id dell'oggetto cui si riferisce la riga
        /// in cui è posizionato questo controllo
        /// </summary>
        protected void hfObjectId_DataBinding(object sender, EventArgs e)
        {
            // Casting del sender ad HiddenField
            HiddenField hfId = sender as HiddenField;

            // Reperimento del data item in cui è inserito
            DataGridItem container = hfId.NamingContainer.NamingContainer as DataGridItem;

            // Impostazione dell'id dell'oggetto
            hfId.Value = DataBinder.Eval(container.DataItem, this.SystemIdFieldName).ToString();
 
        }

        protected void chkSelectDeselect_DataBinding(object sender, EventArgs e)
        {
            // Casting del sender a ChechBox
            CheckBox chk = sender as CheckBox;

            // Reperimento dell'id dell'oggetto
            DataGridItem container = chk.NamingContainer.NamingContainer as DataGridItem;

            // Impostazione dello stato di selezione
            chk.Checked = MassiveOperationUtils.ItemsStatus[
                DataBinder.Eval(container.DataItem, this.SystemIdFieldName).ToString()].Checked;
 
        }

        /// <summary>
        /// Nome della colonna del dataset da utilizzare per recuperare l'id dell'oggetto 
        /// cui fa riferimento la riga della griglia
        /// </summary>
        public string SystemIdFieldName { get; set; }

        /// <summary>
        /// Id della bottoniera delle azione massive
        /// </summary>
        public string MassiveOperationButtonsId { get; set; }

        /// <summary>
        /// Proprietà per l'impostazione dello stato di checking della
        /// checkbox
        /// </summary>
        public bool Checked 
        {
            get
            {
                return this.chkSelectDeselect.Checked;
            }

            set
            {
                this.chkSelectDeselect.Checked = value;
            }
        }

        /// <summary>
        /// Proprietà per l'impotsazione del valore da inserire nel campo nascosto
        /// </summary>
        public String Value
        {
            get
            {
                return this.hfObjectId.Value;
            }

            set
            {
                this.hfObjectId.Value = value;
            }
        }
    }
}