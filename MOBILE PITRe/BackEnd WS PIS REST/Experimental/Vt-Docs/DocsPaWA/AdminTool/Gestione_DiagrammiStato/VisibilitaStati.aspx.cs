using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Linq;
using System.Collections.Generic;

namespace DocsPAWA.AdminTool.Gestione_DiagrammiStato
{
    public partial class SessionVisibilitaStati : System.Web.UI.Page
    {
        #region Session
        public List<DocsPaWR.AssRuoloStatiDiagramma> ListaRuoliSel
        {
            get 
            {
                return (Session["LISTA_RUOLI_SEL"] as List<DocsPaWR.AssRuoloStatiDiagramma>);
            }
            set 
            {
                Session.Remove("LISTA_RUOLI_SEL");
                if(value != null)
                    Session.Add("LISTA_RUOLI_SEL", value);
            }
        }

        public DocsPaWR.DiagrammaStato Diagramma
        {
            get
            {
                return (Session["DiagrammaStato"] as DocsPaWR.DiagrammaStato);
            }
        }

        public Hashtable SessionHashTableRuoli
        {
            set
            {
                Session.Remove("HASHTABLE_LISTA_RUOLI");
                if (value != null)
                    Session.Add("HASHTABLE_LISTA_RUOLI", value);
            }
            get
            {
                return (Hashtable)Session["HASHTABLE_LISTA_RUOLI"];
            }
        }

        public string SessionIdRuolo
        {
            set
            {
                Session.Remove("ID_RUOLO_SEL");
                if (value != null)
                    Session.Add("ID_RUOLO_SEL", value);
            }
            get
            {
                return (string)Session["ID_RUOLO_SEL"];
            }
        }

        public void SetSessionListaRuoli(ArrayList listaRuoli)
        {
            RemoveSessionListaRuoli();
            Session.Add("LISTA_RUOLI", listaRuoli);
        }

        public ArrayList GetSessionListaRuoli()
        {
            return (ArrayList)Session["LISTA_RUOLI"];
        }

        public void RemoveSessionListaRuoli()
        {
            Session.Remove("LISTA_RUOLI");
        }
        #endregion Session
    }

    public partial class VisibilitaStati : System.Web.UI.Page
    {
        private ArrayList listaRuoli;
        private List<DocsPaWR.AssRuoloStatiDiagramma> listaRuoliSelezionati;
        private Hashtable HTruoli;
        private List<DocsPaWR.AssRuoloStatiDiagramma> listaDirittiStatiSelezionati;

        private string idAmministrazione;
        private DocsPaWR.Templates template;
        SessionVisibilitaStati sessionObj = new SessionVisibilitaStati();

        #region Class RuoliHT
        public class RuoliHT
        {
            private string cod;
            private string descr;
            private string notVisible;

            public RuoliHT()
            {
            }

            public RuoliHT(string cod, string descr, string notVisible)
            {
                this.cod = cod;
                this.descr = descr;
                this.notVisible = notVisible;
            }

            public string Cod { get { return cod; } }
            public string Descr { get { return descr; } }
            public string NotVisible { get { return notVisible; } }
        }
        #endregion Class RuoliHT

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.MaintainScrollPositionOnPostBack = true;
            if (sessionObj.Diagramma != null)
            {
                lbl_titolo.Text = "DIAGRAMMA : " + sessionObj.Diagramma.DESCRIZIONE;
            }
            if (!IsPostBack)
            {
                sessionObj.ListaRuoliSel = null;
                this.sessionObj.RemoveSessionListaRuoli();
                this.sessionObj.SessionHashTableRuoli = null;
                this.sessionObj.SessionIdRuolo = null;
            }
            else
            {
                impostaSelezioneRuoli();
            }
        }

        private void Inizialize()
        {
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione, this);

            listaRuoli = new ArrayList(ProfilazioneDocManager.getRuoliByAmm(idAmministrazione, "", "", this));
            sessionObj.SetSessionListaRuoli(listaRuoli);

            listaRuoliSelezionati = DiagrammiManager.GetRuoliStatiDiagramma(sessionObj.Diagramma.SYSTEM_ID);
            sessionObj.ListaRuoliSel = listaRuoliSelezionati;
            caricaHTRuoli();
            caricaDgVisibilitaRuoli();

            impostaSelezioneRuoliAssociati();
        }

        private void caricaHTRuoli()
        {
            listaRuoliSelezionati = sessionObj.ListaRuoliSel;
            listaRuoli = sessionObj.GetSessionListaRuoli();

            this.HTruoli = new Hashtable();
            RuoliHT r = new RuoliHT();
            bool ruoloSel;
            for (int i = 0; i < listaRuoli.Count; i++)
            {
                DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)listaRuoli[i];

                if (listaRuoliSelezionati.Count != 0)
                {
                    //il ruolo non ha visibilità sul diagramma
                    if ((from role in listaRuoliSelezionati where role.ID_GRUPPO.Equals(ruolo.idGruppo) &&
                         role.ID_DIAGRAMMA.Equals(sessionObj.Diagramma.SYSTEM_ID.ToString()) && role.ID_STATO.Equals("0") select role.ID_GRUPPO).Count() > 0)
                    {
                        r = new RuoliHT(ruolo.codice, ruolo.descrizione, "1");
                    }
                    //il ruolo ha visibilità sul diagramma
                    else
                    {
                        r = new RuoliHT(ruolo.codice, ruolo.descrizione, "0");
                    }
                    this.HTruoli.Add(ruolo.idGruppo, r);
                }
                else
                {
                    r = new RuoliHT(ruolo.codice, ruolo.descrizione, "0");
                    this.HTruoli.Add(ruolo.idGruppo, r);
                }

            }
            sessionObj.SessionHashTableRuoli = HTruoli;
        }

        private void caricaDgVisibilitaRuoli()
        {
            listaRuoli = sessionObj.GetSessionListaRuoli();

            DataTable dt = new DataTable();
            dt.Columns.Add("ID_GRUPPO");
            dt.Columns.Add("CODICE RUOLO");
            dt.Columns.Add("DESCRIZIONE RUOLO");
            for (int i = 0; i < listaRuoli.Count; i++)
            {
                DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)listaRuoli[i];
                DataRow rw = dt.NewRow();
                rw[0] = ruolo.idGruppo;
                rw[1] = ruolo.codice;
                rw[2] = ruolo.descrizione;
                dt.Rows.Add(rw);
            }
            dt.AcceptChanges();
            dg_Ruoli.DataSource = dt;
            dg_Ruoli.DataBind();

            if (dg_Ruoli.Items.Count == 0)
            {
                dg_Ruoli.Visible = false;
                lbl_ricercaRuoli.Text = "Nessun ruolo per questa ricerca!";
            }
            else
            {
                dg_Ruoli.Visible = true;
                lbl_ricercaRuoli.Text = "";
            }
        }

        private void caricaDgVisibilitaStati()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID_STATO");
            dt.Columns.Add("DESCRIZIONE");
            for (int i = 0; i < sessionObj.Diagramma.STATI.Count(); i++)
            {
                DataRow rw = dt.NewRow();
                rw[0] = sessionObj.Diagramma.STATI[i].SYSTEM_ID;
                rw[1] = sessionObj.Diagramma.STATI[i].DESCRIZIONE;
                dt.Rows.Add(rw);
            }
            dt.AcceptChanges();
            dg_Stati.DataSource = dt;
            dg_Stati.DataBind();
        }

        private void impostaSelezioneRuoli()
        {
            RuoliHT r = new RuoliHT();
            HTruoli = sessionObj.SessionHashTableRuoli;
            for (int j = 0; j < dg_Ruoli.Items.Count; j++)
            {
                if (HTruoli.Count != 0)
                {
                    if (HTruoli.ContainsKey(dg_Ruoli.Items[j].Cells[0].Text))
                    {
                        if (((CheckBox)dg_Ruoli.Items[j].Cells[3].Controls[1]).Checked)
                        {
                            r = new RuoliHT(dg_Ruoli.Items[j].Cells[1].Text, dg_Ruoli.Items[j].Cells[2].Text, "0");
                            //r = new RuoliHT(dg_Ruoli.Items[j].Cells[1].Text, dg_Ruoli.Items[j].Cells[2].Text, "1");
                        }
                        else
                            r = new RuoliHT(dg_Ruoli.Items[j].Cells[1].Text, dg_Ruoli.Items[j].Cells[2].Text, "1");
                        //r = new RuoliHT(dg_Ruoli.Items[j].Cells[1].Text, dg_Ruoli.Items[j].Cells[2].Text, "0");
                    }
                }
                HTruoli.Remove(dg_Ruoli.Items[j].Cells[0].Text);
                HTruoli.Add(dg_Ruoli.Items[j].Cells[0].Text, r);
            }
            sessionObj.SessionHashTableRuoli = HTruoli;
        }

        // prima di switchare salva la visibilità degli stati del ruolo correntemente selezionato
        private void salvaSelezioneStati()
        {
            if (dg_Stati.Items.Count > 0 && !string.IsNullOrEmpty(sessionObj.SessionIdRuolo))
            {
                List<DocsPaWR.AssRuoloStatiDiagramma> assRuoloStatiDia = new List<DocsPaWR.AssRuoloStatiDiagramma>();
                foreach (DataGridItem item in dg_Stati.Items)
                {
                    if ((item.Cells[2].FindControl("cb_visListaStati") as CheckBox).Checked == false)
                        assRuoloStatiDia.Add(new DocsPaWR.AssRuoloStatiDiagramma()
                        {
                            //CHA_NOT_VIS = "0",
                            CHA_NOT_VIS = "1",
                            ID_DIAGRAMMA = sessionObj.Diagramma.SYSTEM_ID.ToString(),
                            ID_GRUPPO = sessionObj.SessionIdRuolo,
                            ID_STATO = item.Cells[0].Text
                        });
                    else
                    {
                        assRuoloStatiDia.Add(new DocsPaWR.AssRuoloStatiDiagramma()
                        {
                            //CHA_NOT_VIS = "1",
                            CHA_NOT_VIS = "0",
                            ID_DIAGRAMMA = sessionObj.Diagramma.SYSTEM_ID.ToString(),
                            ID_GRUPPO = sessionObj.SessionIdRuolo,
                            ID_STATO = item.Cells[0].Text
                        });
                    }
                }
                DiagrammiManager.ModifyRuoloStatiDiagramma(assRuoloStatiDia);
            }
        }

        private void impostaSelezioneRuoliAssociati()
        {
            HTruoli = sessionObj.SessionHashTableRuoli;

            for (int i = 0; i < dg_Ruoli.Items.Count; i++)
            {
                if (HTruoli.Count != 0)
                {
                    if (HTruoli.ContainsKey(dg_Ruoli.Items[i].Cells[0].Text))
                    {
                        RuoliHT r;
                        r = (RuoliHT)HTruoli[dg_Ruoli.Items[i].Cells[0].Text];
                        //if (r.NotVisible.Equals("0"))
                        if (r.NotVisible.Equals("1"))
                        {
                            ((CheckBox)dg_Ruoli.Items[i].Cells[3].Controls[1]).Checked = false;
                        }
                        //Il ruolo ha visibilità su tutti gli stati del diagramma
                        else
                        {
                            ((CheckBox)dg_Ruoli.Items[i].Cells[3].Controls[1]).Checked = true;
                        }
                    }
                    else
                    {
                        ((CheckBox)dg_Ruoli.Items[i].Cells[3].Controls[1]).Checked = true;
                    }
                }
            }
        }

        private void impostaSelezioneStatiAssociati()
        {
            if (sessionObj.Diagramma.STATI != null && sessionObj.Diagramma.STATI.Count() > 0)
            {
                for (int i = 0; i < sessionObj.Diagramma.STATI.Count(); i++)
                {
                    if ((from statiRuolo in listaDirittiStatiSelezionati
                         where statiRuolo.ID_GRUPPO.Equals(sessionObj.SessionIdRuolo)
                             && statiRuolo.ID_STATO.Equals(sessionObj.Diagramma.STATI[i].SYSTEM_ID.ToString())
                         select statiRuolo).Count() > 0)
                    {
                        //(dg_Stati.Items[i].Cells[2].FindControl("cb_visListaStati") as CheckBox).Checked = true;
                        (dg_Stati.Items[i].Cells[2].FindControl("cb_visListaStati") as CheckBox).Checked = false;
                    }
                    else
                    {
                        //(dg_Stati.Items[i].Cells[2].FindControl("cb_visListaStati") as CheckBox).Checked = false;
                        (dg_Stati.Items[i].Cells[2].FindControl("cb_visListaStati") as CheckBox).Checked = true;
                    }
                }
            }
        }

        protected void btn_conferma_Click(object sender, EventArgs e)
        {
            if (!checkCriterioRicerca())
            {
                ClientScript.RegisterStartupScript(this.GetType(), "SelezionareCriterioRicerca", "alert('Selezionare un criterio di ricerca.');", true);
                return;
            }

            List<DocsPaWR.AssRuoloStatiDiagramma> assRuoliDia = new List<DocsPaWR.AssRuoloStatiDiagramma>();
            HTruoli = sessionObj.SessionHashTableRuoli;

            if (HTruoli != null)
            {
                foreach (string codice in HTruoli.Keys)
                {
                    RuoliHT r;
                    r = (RuoliHT)HTruoli[codice];
                    DocsPaWR.AssRuoloStatiDiagramma assRuoloDia = new DocsPaWR.AssRuoloStatiDiagramma();
                    assRuoloDia.ID_DIAGRAMMA = sessionObj.Diagramma.SYSTEM_ID.ToString();
                    assRuoloDia.ID_GRUPPO = codice;
                    assRuoloDia.ID_STATO = "0";
                    assRuoloDia.CHA_NOT_VIS = r.NotVisible;
                    assRuoliDia.Add(assRuoloDia);
                }
                DiagrammiManager.ModifyRuoloStatiDiagramma(assRuoliDia);
                salvaSelezioneStati();
            }
        }

        protected void btn_find_Click(object sender, System.EventArgs e)
        {
            if (!checkCriterioRicerca())
            {
                ClientScript.RegisterStartupScript(this.GetType(), "SelezionareCriterioRicerca", "alert('Selezionare un criterio di ricerca.');", true);
                return;
            }


            if (this.ddl_ricTipo.SelectedItem.Value.Equals("T"))
            {
                this.Inizialize();
            }
            else
            {
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione, this);

                listaRuoli = new ArrayList(ProfilazioneDocManager.getRuoliByAmm(idAmministrazione, txt_ricerca.Text, ddl_ricTipo.SelectedItem.Value, this));
                sessionObj.SetSessionListaRuoli(listaRuoli);
                sessionObj.ListaRuoliSel = DiagrammiManager.GetRuoliStatiDiagramma(sessionObj.Diagramma.SYSTEM_ID);
                caricaHTRuoli();
                caricaDgVisibilitaRuoli();
                impostaSelezioneRuoliAssociati();
                impostaSelezioneRuoli();
            }
            caricaHTRuoli();
            resetPanelStati();
        }

        protected void dg_Ruoli_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            int elSelezionato = e.Item.ItemIndex;

            listaRuoli = sessionObj.GetSessionListaRuoli();
            DocsPaWR.Ruolo ruolo = (DocsPaWR.Ruolo)listaRuoli[elSelezionato];

            switch (e.CommandName)
            {
                case "VisibilitaStati":
                    dg_Stati.SelectedIndex = -1;
                    dg_Ruoli.SelectedIndex = elSelezionato;
                    lbl_ruolo.Text = "RUOLO : " + ruolo.descrizione;

                    //Salvo la selezione dei diritti su gli stati prima di cambiare la selezione del ruolo
                    salvaSelezioneStati();
                    listaDirittiStatiSelezionati = DiagrammiManager.GetRuoliStatiDiagramma(sessionObj.Diagramma.SYSTEM_ID);
                    sessionObj.SessionIdRuolo = ruolo.idGruppo;
                    caricaDgVisibilitaStati();
                    impostaSelezioneStatiAssociati();
                    if (sessionObj.Diagramma.STATI != null && sessionObj.Diagramma.STATI.Count() > 0)
                        panel_listaStati.Visible = true;
                    break;
            }
        }

        protected void resetPanelStati()
        {
            salvaSelezioneStati();
            sessionObj.SessionIdRuolo = null;
            dg_Ruoli.SelectedIndex = -1;
            panel_listaStati.Visible = false;
        }

        protected void btn_estendiARuoli_Click(object sender, EventArgs e)
        {
            if (dg_Stati.Items.Count > 0 && !string.IsNullOrEmpty(sessionObj.SessionIdRuolo))
            {
                listaRuoli = sessionObj.GetSessionListaRuoli();
                List<DocsPaWR.AssRuoloStatiDiagramma> assRuoloStatiDia = new List<DocsPaWR.AssRuoloStatiDiagramma>();
                foreach (DocsPaWR.Ruolo r in listaRuoli)
                {
                    for (int i = 0; i < sessionObj.Diagramma.STATI.Count(); i++)
                    {
                        assRuoloStatiDia.Add(new DocsPaWR.AssRuoloStatiDiagramma()
                        {
                            ID_DIAGRAMMA = sessionObj.Diagramma.SYSTEM_ID.ToString(),
                            ID_GRUPPO = r.idGruppo,
                            ID_STATO = sessionObj.Diagramma.STATI[i].SYSTEM_ID.ToString(),
                            CHA_NOT_VIS = ((CheckBox)dg_Stati.Items[i].Cells[2].FindControl("cb_visListaStati")).Checked ? "0" : "1"
                        });
                    }
                }
                if (listaRuoli != null && listaRuoli.Count > 0 && assRuoloStatiDia != null && assRuoloStatiDia.Count > 0)
                    DiagrammiManager.ModifyRuoloStatiDiagramma(assRuoloStatiDia);
            }
        }

        protected void ddl_ricTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
    
            this.ddl_ricTipo.Items.FindByValue("SEL").Enabled = false;

            if (this.ddl_ricTipo.SelectedItem.Value.Equals("T"))
            {
                txt_ricerca.Text = string.Empty;
                txt_ricerca.BackColor = System.Drawing.Color.Gainsboro;
                txt_ricerca.ReadOnly = true;
            }
            else
            {
                txt_ricerca.BackColor = System.Drawing.Color.White;
                txt_ricerca.ReadOnly = false;
            }
        }

        protected void CheckboxAllVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (dg_Ruoli != null && dg_Ruoli.Items.Count > 0)
            {
                if (((System.Web.UI.WebControls.CheckBox)sender).Checked)
                {
                    for (int i = 0; i < dg_Ruoli.Items.Count; i++)
                    {
                        ((CheckBox)dg_Ruoli.Items[i].Cells[3].Controls[1]).Checked = true;
                    }
                }
                else
                {
                    for (int i = 0; i < dg_Ruoli.Items.Count; i++)
                    {
                        ((CheckBox)dg_Ruoli.Items[i].Cells[3].Controls[1]).Checked = false;
                    }
                }
            }
        }

        protected void CheckAllVisibilitaStati_CheckedChanged(object sender, EventArgs e)
        {
            if (dg_Stati != null && dg_Stati.Items.Count > 0)
            {
                if (((System.Web.UI.WebControls.CheckBox)sender).Checked)
                {
                    for (int i = 0; i < dg_Stati.Items.Count; i++)
                    {
                        ((CheckBox)dg_Stati.Items[i].Cells[2].FindControl("cb_visListaStati")).Checked = true;
                    }
                }
                else
                {
                    for (int i = 0; i < dg_Stati.Items.Count; i++)
                    {
                        ((CheckBox)dg_Stati.Items[i].Cells[2].FindControl("cb_visListaStati")).Checked = false;
                    }
                }
            }
        }

        protected void btn_estendiAStati_Click(object sender, EventArgs e)
        {
            resetPanelStati();
            btn_conferma_Click(sender, e);
            if (checkCriterioRicerca())
            {
                List<DocsPaWR.AssRuoloStatiDiagramma> assRuoliStatiDia = new List<DocsPaWR.AssRuoloStatiDiagramma>();
                 HTruoli = sessionObj.SessionHashTableRuoli;
                 if (HTruoli != null && HTruoli.Count > 0 && sessionObj.Diagramma.STATI != null && sessionObj.Diagramma.STATI.Count() > 0)
                 {
                     foreach (string codice in HTruoli.Keys)
                     {
                         foreach(DocsPaWR.Stato stato in sessionObj.Diagramma.STATI)
                         {
                            assRuoliStatiDia.Add(new DocsPaWR.AssRuoloStatiDiagramma(){
                            ID_DIAGRAMMA = sessionObj.Diagramma.SYSTEM_ID.ToString(),
                            ID_GRUPPO = codice,
                            ID_STATO = stato.SYSTEM_ID.ToString(),
                            CHA_NOT_VIS = (HTruoli[codice] as RuoliHT).NotVisible});
                         }
                     }
                     DiagrammiManager.ModifyRuoloStatiDiagramma(assRuoliStatiDia);
                 }
            }
        }

        protected void dg_Stati_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            int elSelezionato = e.Item.ItemIndex;
            DocsPaWR.Stato stato = sessionObj.Diagramma.STATI[elSelezionato];

            switch (e.CommandName)
            {
                case "VisibilitaRuoliStato":
                    dg_Stati.SelectedIndex = elSelezionato;

                    //Salvo la selezione dei diritti su gli stati
                    salvaSelezioneStati();

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "VisRuoliFromStatoDiagramma", 
                        "window.showModalDialog('DirittiRuoloStatoDiagramma.aspx?idStato=" + stato.SYSTEM_ID.ToString() + "&idDiagramma=" + sessionObj.Diagramma.SYSTEM_ID.ToString() + 
                        "','','dialogWidth:800px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;');", true);
                    break;
            }
        }

        protected bool checkCriterioRicerca()
        {
            bool result = true;
            if (this.ddl_ricTipo.SelectedValue.Equals("SEL"))
                return false;
            return true;
        }
    }
}
