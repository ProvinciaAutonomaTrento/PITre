using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace DocsPAWA.popup
{
    public partial class ScegliDestinatari : DocsPAWA.CssPage
   {

      protected DataSet ds;
      protected DataTable dt;
      protected Hashtable ht_destinatariTO_CC;
      protected DocsPaWR.SchedaDocumento schedadoc;
      protected DocsPaWR.SchedaDocumento schedadocIngressoNew;
      protected int itemIndex;

      protected void Page_Load(object sender, EventArgs e)
      {
         this.Response.Expires = -1;
         schedadoc = DocumentManager.getDocumentoInLavorazione(this);
         if (!IsPostBack)
         {

           
            DocsPaWR.Corrispondente[] listaCorrTo = null;
            DocsPaWR.Corrispondente[] listaCorrCC = null;
                
            //prendo i destinatari in To
            listaCorrTo = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedadoc.protocollo).destinatari;
            //prendo i destinatari in CC
            listaCorrCC = ((DocsPAWA.DocsPaWR.ProtocolloUscita)schedadoc.protocollo).destinatariConoscenza;

            FillDataGrid(listaCorrTo, listaCorrCC);
          
         }
         
      }

      #region Web Form Designer generated code
      override protected void OnInit(EventArgs e)
      {
          //
          // CODEGEN: This call is required by the ASP.NET Web Form Designer.
          //
          InitializeComponent();
          base.OnInit(e);
      }

        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
            this.dg_lista_corr.PreRender += new EventHandler(dg_lista_corr_PreRender);
        }

        void dg_lista_corr_PreRender(object sender, EventArgs e)
        {
            foreach (DataGridItem item in dg_lista_corr.Items)
            {
                string idDestinatario = item.Cells[0].Text;
                DocsPaWR.Corrispondente corr = UserManager.getCorrispondenteBySystemID(this.Page, idDestinatario);
                if (!string.IsNullOrEmpty(corr.dta_fine))
                {
                    RadioButton optCorr = item.Cells[3].FindControl("optCorr") as RadioButton;
                    optCorr.Visible = false;
                    item.Cells[1].ToolTip = "Elemento non selezionabile in quanto storicizzato";
                }
            }
        }
      #endregion


      /// <summary>
      /// Caricamento griglia destinatari del protocollo in uscita selezionato
      /// </summary>
      /// <param name="uoApp"></param>
      private void FillDataGrid(DocsPAWA.DocsPaWR.Corrispondente[] listaCorrTo, DocsPAWA.DocsPaWR.Corrispondente[] listaCorrCC)
      {
         ds = this.CreateGridDataSetDestinatari();
         this.CaricaGridDataSetDestinatari(ds, listaCorrTo, listaCorrCC);
         this.dg_lista_corr.DataSource = ds;
         DocumentManager.setDataGridDestinatari(this, dt);
         this.dg_lista_corr.DataBind();

         // Impostazione corrispondente predefinito
         this.SelectDefaultCorrispondente();
      }

      private DataSet CreateGridDataSetDestinatari()
      {
         DataSet retValue = new DataSet();

         dt = new DataTable("GRID_TABLE_DESTINATARI");
         dt.Columns.Add("SYSTEM_ID", typeof(string));
         dt.Columns.Add("TIPO_CORR", typeof(string));
         dt.Columns.Add("DESC_CORR", typeof(string));
         retValue.Tables.Add(dt);

         return retValue;
      }

      /// <summary>
      /// Caricamento dataset utilizzato per le griglie
      /// </summary>
      /// <param name="ds"></param>
      /// <param name="uo"></param>
      private void CaricaGridDataSetDestinatari(DataSet ds, DocsPAWA.DocsPaWR.Corrispondente[] listaCorrTo, DocsPAWA.DocsPaWR.Corrispondente[] listaCorrCC)
      {
         DataTable dt = ds.Tables["GRID_TABLE_DESTINATARI"];
         ht_destinatariTO_CC = new Hashtable();
         string tipoURP = "";

         if (listaCorrTo != null && listaCorrTo.Length > 0)
         {
            for (int i = 0; i < listaCorrTo.Length; i++)
            {
               if (listaCorrTo[i].tipoCorrispondente != null && listaCorrTo[i].tipoCorrispondente.Equals("O"))
               {
                  this.AppendDataRow(dt, listaCorrTo[i].tipoCorrispondente, listaCorrTo[i].systemId, "&nbsp;" + listaCorrTo[i].descrizione);
               }
               else
               {
                  if (listaCorrTo[i].GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)))
                  {
                     tipoURP = "U";
                  }
                  if (listaCorrTo[i].GetType().Equals(typeof(DocsPAWA.DocsPaWR.Ruolo)))
                  {
                     tipoURP = "R";
                  }
                  if (listaCorrTo[i].GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
                  {
                     tipoURP = "P";
                  }
                  this.AppendDataRow(dt, listaCorrTo[i].tipoIE, listaCorrTo[i].systemId, GetImage(tipoURP) + " - " + listaCorrTo[i].descrizione);
               }
               ht_destinatariTO_CC.Add(listaCorrTo[i].systemId, listaCorrTo[i]);
            }
         }
         if (listaCorrCC != null && listaCorrCC.Length > 0)
         {
            for (int i = 0; i < listaCorrCC.Length; i++)
            {
               if (listaCorrCC[i].tipoCorrispondente != null && listaCorrCC[i].tipoCorrispondente.Equals("O"))
               {
                  this.AppendDataRow(dt, listaCorrCC[i].tipoCorrispondente, listaCorrCC[i].systemId, "&nbsp;" + listaCorrCC[i].descrizione + " (CC)");
               }
               else
               {
                  if (listaCorrCC[i].GetType().Equals(typeof(DocsPAWA.DocsPaWR.UnitaOrganizzativa)))
                  {
                     tipoURP = "U";
                  }
                  if (listaCorrCC[i].GetType().Equals(typeof(DocsPAWA.DocsPaWR.Ruolo)))
                  {
                     tipoURP = "R";
                  }
                  if (listaCorrCC[i].GetType().Equals(typeof(DocsPAWA.DocsPaWR.Utente)))
                  {
                     tipoURP = "P";
                  }
                  this.AppendDataRow(dt, listaCorrCC[i].tipoIE, listaCorrCC[i].systemId, GetImage(tipoURP) + " - " + listaCorrCC[i].descrizione + " (CC)");
               }
               ht_destinatariTO_CC.Add(listaCorrCC[i].systemId, listaCorrCC[i]);
            }
         }
         if ((listaCorrTo != null && listaCorrTo.Length > 0) || (listaCorrCC != null && listaCorrCC.Length > 0))
         {
            this.pnl_corr.Visible = true;
         }
         DocumentManager.setHash(this, ht_destinatariTO_CC);

      }

      private void AppendDataRow(DataTable dt, string tipoCorr, string systemId, string descCorr)
      {
         DataRow row = dt.NewRow();
         row["SYSTEM_ID"] = systemId;
         row["TIPO_CORR"] = tipoCorr;
         row["DESC_CORR"] = descCorr;
         dt.Rows.Add(row);
         row = null;
      }

      private string GetImage(string rowType)
      {
         string retValue = string.Empty;
         string spaceIndent = string.Empty;

         switch (rowType)
         {
            case "U":
               retValue = "uo_noexp";
               spaceIndent = "&nbsp;";
               break;

            case "R":
               retValue = "ruolo_noexp";
               spaceIndent = "&nbsp;";
               break;

            case "P":
               retValue = "utente_noexp";
               spaceIndent = "&nbsp;";
               break;
         }

         retValue = spaceIndent + "<img src='../images/smistamento/" + retValue + ".gif' border='0'>";

         return retValue;
      }

      /// <summary>
      /// In presenza di un solo corrispondente in griglia,
      /// lo seleziona per default
      /// </summary>
      private void SelectDefaultCorrispondente()
      {
         if (this.dg_lista_corr.Items.Count == 1)
         {
            DataGridItem dgItem = this.dg_lista_corr.Items[0];

            RadioButton optCorr = dgItem.Cells[3].FindControl("optCorr") as RadioButton;
            if (optCorr != null)
               optCorr.Checked = true;
         }
      }

      private bool verificaSelezione(out int itemIndex)
      {
         bool verificaSelezione = false;
         itemIndex = -1;
         foreach (DataGridItem dgItem in this.dg_lista_corr.Items)
         {
            RadioButton optCorr = dgItem.Cells[3].FindControl("optCorr") as RadioButton;
            if ((optCorr != null) && optCorr.Checked == true)
            {
               itemIndex = dgItem.ItemIndex;
               verificaSelezione = true;
               break;
            }
         }
         return verificaSelezione;
      }

      protected void btn_ok_Click(object sender, EventArgs e)
      {
         DocsPaWR.Corrispondente destSelected = null;
         bool avanzaCor = verificaSelezione(out itemIndex);
         if (avanzaCor)
         {
            string key = dg_lista_corr.Items[itemIndex].Cells[0].Text;

            //prendo la hashTable che contiene i corrisp dalla sesisone
            ht_destinatariTO_CC = DocumentManager.getHash(this);

            if (ht_destinatariTO_CC != null)
            {
               if (ht_destinatariTO_CC.ContainsKey(key))
               {
                  //prendo il corrispondente dalla hashTable secondo quanto è stato richiesto dall'utente
                  destSelected = (DocsPAWA.DocsPaWR.Corrispondente)ht_destinatariTO_CC[key];
               }
            }

            if (string.IsNullOrEmpty(destSelected.dta_fine))
            {
                //creo il documento
                schedadocIngressoNew = DocumentManager.riproponiDatiRispIngresso(this, schedadoc, destSelected);
                FileManager.removeSelectedFile(this);
                schedadocIngressoNew.predisponiProtocollazione = true;
                DocumentManager.setDocumentoInLavorazione(this, schedadocIngressoNew);
            }
            Page.RegisterStartupScript("", "<script>window.close();</script>");
         }
         else
         {
            //avviso l'utente che non ha selezionato nessun corrispondente
            Page.RegisterStartupScript("alert", "<SCRIPT>alert('ATTENZIONE: selezionare un corrispondente dalla lista');</SCRIPT>");
         }

        
      }

      protected void btn_chiudi_Click(object sender, EventArgs e)
      {
          Page.RegisterStartupScript("", "<script>window.close();</script>");
      }

      protected void dg_lista_corr_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
      {
         this.dg_lista_corr.SelectedIndex = -1;
         this.dg_lista_corr.CurrentPageIndex = e.NewPageIndex;
         dg_lista_corr.DataSource = DocumentManager.getDataGridDestinatari(this);
         dg_lista_corr.DataBind();
      }

   }
}
