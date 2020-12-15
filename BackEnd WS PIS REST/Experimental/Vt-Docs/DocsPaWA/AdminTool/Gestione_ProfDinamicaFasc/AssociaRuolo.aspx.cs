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

namespace DocsPAWA.AdminTool.Gestione_ProfDinamicaFasc
{
    public partial class SessionAssociaRuolo : System.Web.UI.Page
    {
        public void SetSessionListaSel(ArrayList listaRuoliSelezionati)
        {
            RemoveSessionListaSel();
            Session.Add("A_T_P_D_LISTASELFasc1", listaRuoliSelezionati);
        }

        public ArrayList GetSessionListaSel()
        {
            return (ArrayList)Session["A_T_P_D_LISTASELFasc1"];
        }

        public void RemoveSessionListaSel()
        {
            Session.Remove("A_T_P_D_LISTASELFasc1");
        }

        public void SetSessionLista(ArrayList listaRuoli)
        {
            RemoveSessionLista();
            Session.Add("A_T_P_D_LISTAFasc1", listaRuoli);
        }

        public ArrayList GetSessionLista()
        {
            return (ArrayList)Session["A_T_P_D_LISTAFasc1"];
        }

        public void RemoveSessionLista()
        {
            Session.Remove("A_T_P_D_LISTAFasc1");
        }

        public void SetSessionHashTable(Hashtable hashTableRuoli)
        {
            RemoveSessionHashTable();
            Session.Add("A_T_P_D_HASHTABLEFasc1", hashTableRuoli);
        }

        public Hashtable GetSessionHashTable()
        {
            return (Hashtable)Session["A_T_P_D_HASHTABLEFasc1"];
        }

        public void RemoveSessionHashTable()
        {
            Session.Remove("A_T_P_D_HASHTABLEFasc1");
        }
    }

    public partial class AssociaRuolo : System.Web.UI.Page
    {
        //private DocsPAWA.DocsPaWR.DocsPaWebService wws = new DocsPAWA.DocsPaWR.DocsPaWebService();
		private ArrayList listaRuoli;
        private ArrayList listaRuoliSelezionati;
        private Hashtable HTruoli;
        private string idAmministrazione;
        private DocsPaWR.Templates template;
        SessionAssociaRuolo sessionObj = new SessionAssociaRuolo();
        protected string idOggettoCustom = string.Empty;


        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.MaintainScrollPositionOnPostBack = true;
            string idOggetto = Request.QueryString["IdOggCustom"];
            if (!string.IsNullOrEmpty(idOggetto))
                this.idOggettoCustom = idOggetto;

            template = (DocsPAWA.DocsPaWR.Templates)Session["template"];
            lbl_titolo.Text = "Abilitazione incremento differito";

            if (!IsPostBack)
            {
                Session.Add("reloadHTFasc", false);
                this.Inizialize();
            }
            else
            {
                impostaSelezione();
            }
        }

        private void Inizialize()
        {                       
            string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
            string codiceAmministrazione = amministrazione[0];
            idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione,this);

            listaRuoli = new ArrayList(ProfilazioneFascManager.getRuoliByAmm(idAmministrazione, "", "",this));
            sessionObj.SetSessionLista(listaRuoli);

            //listaRuoliSelezionati = new ArrayList(ProfilazioneFascManager.getRuoliOggCustomFasc(this.idOggettoCustom,this));
            sessionObj.SetSessionListaSel(listaRuoliSelezionati);

            bool reloadHT = (Boolean)Session["reloadHTFasc"];
            if (!reloadHT)
                caricaHTRuoli();
           
            caricaDgAssociazione();

            impostaSelezioneAssociati();
        }

        private void caricaHTRuoli()
        {
            listaRuoliSelezionati = sessionObj.GetSessionListaSel();
            listaRuoli = sessionObj.GetSessionLista();

            this.HTruoli = new Hashtable();
            RuoliHT r = new RuoliHT();
            for (int i = 0; i < listaRuoli.Count; i++)
            {
                DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)listaRuoli[i];
                if (listaRuoliSelezionati.Count != 0)
                {
                    for (int j = 0; j < listaRuoliSelezionati.Count; j++)
                    {
                        if(ruolo.idGruppo == listaRuoliSelezionati[j].ToString())
                        {
                            r = new RuoliHT(ruolo.descrizione, "1");
                            if(!HTruoli.Contains(ruolo.idGruppo))
                                this.HTruoli.Add(ruolo.idGruppo, r);
                        }
                    }
                }
            }
            //sessionObj.RemoveSessionHashTable();
            sessionObj.SetSessionHashTable(HTruoli);
        }

        private void caricaDgAssociazione()
        {
            listaRuoli = sessionObj.GetSessionLista();

            DataTable dt = new DataTable();
            dt.Columns.Add("ID_GRUPPO");
            dt.Columns.Add("DESCRIZIONE RUOLO");
            for (int i = 0; i < listaRuoli.Count; i++)
            {
                DocsPaWR.Ruolo ruolo = (DocsPAWA.DocsPaWR.Ruolo)listaRuoli[i];
                DataRow rw = dt.NewRow();
                rw[0] = ruolo.idGruppo;
                rw[1] = ruolo.descrizione;
                dt.Rows.Add(rw);
            }
            dt.AcceptChanges();
            dg_Ruoli.DataSource = dt;
            dg_Ruoli.DataBind();

            if (dg_Ruoli.Items.Count == 0)
            {
                dg_Ruoli.Visible = false;
                lbl_ricerca.Text = "Nessun ruolo per questa ricerca!";
            }
            else
            {
                dg_Ruoli.Visible = true;
                lbl_ricerca.Text = "";
            }
        }

        private void impostaSelezione()
        {
            RuoliHT r = new RuoliHT();
            HTruoli = sessionObj.GetSessionHashTable();
            for (int j = 0; j < dg_Ruoli.Items.Count; j++)
            {
                if (HTruoli.Count != 0)
                {
                    if (HTruoli.ContainsKey(dg_Ruoli.Items[j].Cells[0].Text))
                    {
                        if (((CheckBox)dg_Ruoli.Items[j].Cells[2].Controls[1]).Checked)
                        {
                            r = new RuoliHT(dg_Ruoli.Items[j].Cells[1].Text, "1");
                        }
                        else
                            r = new RuoliHT(dg_Ruoli.Items[j].Cells[1].Text, "0");
                    }
                }
                HTruoli.Remove(dg_Ruoli.Items[j].Cells[0].Text);
                HTruoli.Add(dg_Ruoli.Items[j].Cells[0].Text, r);
            }
            sessionObj.SetSessionHashTable(HTruoli);
        }
 
        private void impostaSelezioneAssociati()
        {
            HTruoli = sessionObj.GetSessionHashTable();
            
            for (int i = 0; i < dg_Ruoli.Items.Count; i++)
            {
                if (HTruoli.Count != 0)
                {
                    if (HTruoli.ContainsKey(dg_Ruoli.Items[i].Cells[0].Text))
                    {
                        RuoliHT r;
                        r = (RuoliHT)HTruoli[dg_Ruoli.Items[i].Cells[0].Text];
                        if (r.Ins == "1")
                        {
                        ((CheckBox)dg_Ruoli.Items[i].Cells[2].Controls[1]).Checked = true;
                    }
                        if (r.Ins == "0")
                        {
                        ((CheckBox)dg_Ruoli.Items[i].Cells[2].Controls[1]).Checked = false;
                }
                    }
                }
            }
        }

        protected void btn_selezione_Click(object sender, EventArgs e)
        {
            if (btn_selezione.Text == "Sel. Tutti")
            {
                for (int j = 0; j < dg_Ruoli.Items.Count; j++)
                {
                    ((CheckBox)dg_Ruoli.Items[j].Cells[2].Controls[1]).Checked = true;
                }

                btn_selezione.Text = "Desel. Tutti";
                return;
            }
            
            if (btn_selezione.Text == "Desel. Tutti")
            {
                for (int j = 0; j < dg_Ruoli.Items.Count; j++)
                {
                    ((CheckBox)dg_Ruoli.Items[j].Cells[2].Controls[1]).Checked = false;
                }

                btn_selezione.Text = "Sel. Tutti";
                return;
            }

        }

        protected void btn_conferma_Click(object sender, EventArgs e)
        {
            ArrayList assDocRuoli = new ArrayList();
            HTruoli = sessionObj.GetSessionHashTable();
            //listaRuoli = sessionObj.GetSessionLista();
            //string[] appo = new string[dg_Ruoli.Items.Count];
            string[] appo = new string[HTruoli.Count];
            int i = 0;
            foreach (string codice in HTruoli.Keys)
            {
                RuoliHT r;
                r = (RuoliHT)HTruoli[codice];
                if (r.Ins == "1")
            {
                    appo.SetValue(codice, i);
                }
                else
                    appo.SetValue("", i);
                i++;
            }
            
            //ProfilazioneFascManager.salvaAssociazioneRuoloOggettoCustomFasc(this.idOggettoCustom, appo, template.ID_TIPO_FASC, this);
            
            sessionObj.RemoveSessionLista();
            sessionObj.RemoveSessionListaSel();
            sessionObj.RemoveSessionHashTable();
            Session.Remove("reloadHTFasc");
            ClientScript.RegisterStartupScript(this.GetType(), "chiusura", "<script>window.close();</script>");
        }


        protected void btn_find_Click(object sender, System.EventArgs e)
        {
            if (this.ddl_ricTipo.SelectedItem.Value.Equals("T"))
            {
                Session["reloadHTFasc"] = true;
                this.Inizialize();
            }
            else
            {
                string[] amministrazione = ((string)Session["AMMDATASET"]).Split('@');
                string codiceAmministrazione = amministrazione[0];
                idAmministrazione = Utils.getIdAmmByCod(codiceAmministrazione,this);

                listaRuoli = new ArrayList(ProfilazioneFascManager.getRuoliByAmm(idAmministrazione, txt_ricerca.Text, ddl_ricTipo.SelectedItem.Value,this));
                sessionObj.SetSessionLista(listaRuoli);

                //listaRuoliSelezionati = new ArrayList(ProfilazioneFascManager.getRuoliOggCustomFasc(this.idOggettoCustom,this));
                sessionObj.SetSessionListaSel(listaRuoliSelezionati);

                //caricaHTRuoli();

                caricaDgAssociazione();

                impostaSelezioneAssociati();

                impostaSelezione();
            }
        }

        protected void ddl_ricTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ddl_ricTipo.SelectedItem.Value.Equals("T"))
            {
                this.txt_ricerca.Enabled = false;
                this.txt_ricerca.Text = "";
                Session["reloadHTFasc"] = true;
                this.Inizialize();
            }
            else
            {
                this.txt_ricerca.Enabled = true;
            }
        }

        public class RuoliHT
        {
            private string descr;
            private string ins;

            public RuoliHT()
            {
            }

            public RuoliHT(string descr, string ins)
            {
                this.descr = descr;
                this.ins = ins;
             }

            public string Descr { get { return descr; } }
            public string Ins { get { return ins; } }
        }

    }
}
