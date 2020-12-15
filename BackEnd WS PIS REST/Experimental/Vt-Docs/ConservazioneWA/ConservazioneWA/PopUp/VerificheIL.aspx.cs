using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace ConservazioneWA.PopUp
{
    public partial class VerificheIL : System.Web.UI.Page
    {
        string idConservazione;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Response.Expires = -1;


                btn_verifica.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_verifica.Attributes.Add("onmouseout", "this.className='cbtn';");

                btn_chiudi.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_chiudi.Attributes.Add("onmouseout", "this.className='cbtn';");

                btn_verifica_integrita.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_verifica_integrita.Attributes.Add("onmouseout", "this.className='cbtn';");

                btn_verifica_unificata.Attributes.Add("onmouseover", "this.className='cbtnHover';");
                btn_verifica_unificata.Attributes.Add("onmouseout", "this.className='cbtn';");

                idConservazione = this.Request.QueryString["idConservazione"];
                this.hd_idIstanza.Value = idConservazione;
                WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(idConservazione, infoUtente);
                int i = 0;

                DataTable tabella1 = new DataTable();
                tabella1.Columns.Add(new DataColumn("docnumber"));
                tabella1.Columns.Add(new DataColumn("desc"));
                tabella1.Columns.Add(new DataColumn("tipo"));

                string[] riga1 = new string[3];
                foreach (WSConservazioneLocale.ItemsConservazione item in items)
                {
                    riga1[0] = item.DocNumber;
                    riga1[1] = item.desc_oggetto;
                    riga1[2] = item.tipoFile;

                    tabella1.Rows.Add(riga1);
                    sel_file_da_aprire.Items.Add(item.DocNumber/* + item.tipoFile*/);
                    //sel_numero_file.Items.Add((i+1).ToString());
                    i++;
                }
                hd_totDocs.Value = (i).ToString();
                if (i < 3)
                {
                    rbtn_numero_file.Enabled = false;
                    lbl_numero_file.Enabled = false;
                    tb_num_file.Enabled = false;
                }
                else
                {
                    tb_num_file.Text = "3";
                }
                tb_percent_file.Text = "100";
                this.FetchDataProssimaVerificaIntegrita();
                this.FetchDataProssimaVerificaLeggibilita();
                GridView1.DataSource = tabella1;
                GridView1.DataBind();
            }
        }

        protected void btn_chiudi_Click(object sender, EventArgs e)
        {
            Response.Write("<script>window.close();</script>");
        }



        protected void btn_verifica_Click(object sender, EventArgs e)
        {
            bool errore = false; int number; int totDocs;
            Int32.TryParse(hd_totDocs.Value, out totDocs);
            Int32.TryParse(tb_num_file.Text, out number);
            if (!rbtn_file_da_aprire.Checked && !rbtn_numero_file.Checked && !rbtn_percent_file.Checked)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_selezione", "alert('Selezionare un opzione!');", true);
                errore = true;
            }
            else
            {
                if (rbtn_numero_file.Checked && string.IsNullOrEmpty(tb_num_file.Text))
                {
                    errore = true;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_insNum", "alert('Inserire il numero di file da verificare!');", true);
                }
                else if (rbtn_numero_file.Checked && !Int32.TryParse(tb_num_file.Text, out number))
                {
                    errore = true;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_insNum2", "alert('Inserire un valore numerico.');", true);
                }
                else if (rbtn_numero_file.Checked && (number > totDocs || number < 3))
                {
                    errore = true;
                    if (totDocs > 2)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_insNum3", "alert('Il valore inserito non è valido. Inserire un valore compreso tra 3 e " + totDocs + "');", true);
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_inNum4", "alert('Il valore inserito non è valido. Numero di documenti non sufficiente per la selezione casuale.');", true);

                }
                else
                    if (rbtn_percent_file.Checked && (string.IsNullOrEmpty(tb_percent_file.Text) || !Int32.TryParse(tb_percent_file.Text, out number)))
                    {
                        errore = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_percent1", "alert('Inserire una percentuale valida.');", true);
                    }
                    else if (rbtn_percent_file.Checked && (number > 100 || number < 1))
                    {
                        errore = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_percent2", "alert('Inserire una percentuale valida.');", true);
                    }
                    else

                        if (rbtn_file_da_aprire.Checked)
                        {
                            errore = true;
                            foreach (GridViewRow grv in GridView1.Rows)
                            {
                                if (((CheckBox)grv.FindControl("chk_doc")).Checked) errore = false;
                            }
                            if (errore)
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_selezione", "alert('Selezionare quali file verificare!');", true);
                        }

            }
            if (!errore)
            {
                // MEV CS 1.5
                // alert su esecuzione anticipata verifica leggibilità
                // verifico se l'alert è attivo
                string idAmministrazione = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]).idAmministrazione;
                if (Utils.ConservazioneManager.IsAlertConservazioneAttivo(idAmministrazione, "LEGGIBILITA_ANTICIPATA"))
                {
                    bool checkVerificaAnticipata = Utils.ConservazioneManager.IsVerificaLeggibilitaAnticipata(this.hd_idIstanza.Value, IdSupporto, idAmministrazione);
                    if (checkVerificaAnticipata)
                    {
                        //invio alert                        
                        Utils.ConservazioneManager.InvioAlertAsync((WSConservazioneLocale.InfoUtente)Session["infoutCons"], "LEGGIBILITA_ANTICIPATA", this.hd_idIstanza.Value, IdSupporto);
                    }
                }
                // fine MEV CS 1.5

                if (rbtn_numero_file.Checked)
                {
                    string files = "";
                    WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                    WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(this.hd_idIstanza.Value, infoUtente);
                    number = Int32.Parse(tb_num_file.Text);
                    Random rdm = new Random();
                    string[] presi = new string[number];
                    int i = 0;
                    while (i < number)
                    {
                        int j = rdm.Next(totDocs);
                        if (!presi.Contains(items[j].DocNumber))
                        {
                            files += items[j].DocNumber + ",";
                            presi[i] = items[j].DocNumber;
                            i++;
                        }
                    }
                    // MEV CS 1.5
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verificaLeggibilita", "showVerificaLeggibilita('" + this.hd_idIstanza.Value + "','" + files + "',0,'" + IdSupporto + "','" + txtNoteDiVerifica.Text + "','" + txtDataProssimaVerifica.Text + "');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verificaLeggibilita", "showVerificaLeggibilita('" + this.hd_idIstanza.Value + "','" + files + "',0,'" + IdSupporto + "','" + txtNoteDiVerifica.Text + "','" + txtDataProssimaVerificaLegg.Text + "');", true);

                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verificaLeggibilita", "showVerificaLeggibilita('" + this.hd_idIstanza.Value + "','no'," + sel_numero_file.Items[sel_numero_file.SelectedIndex].Text + ");", true);
                }
                if (rbtn_percent_file.Checked)
                {
                    string files = "";
                    WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);
                    WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(this.hd_idIstanza.Value, infoUtente);

                    int percent = Int32.Parse(tb_percent_file.Text);
                    double op = (totDocs * 100 / percent);
                    number = (int)Math.Ceiling((double)((double)(totDocs * percent) / 100));
                    Random rdm = new Random();
                    string[] presi = new string[number];
                    int i = 0;
                    while (i < number)
                    {
                        int j = rdm.Next(totDocs);
                        if (!presi.Contains(items[j].DocNumber))
                        {
                            files += items[j].DocNumber + ",";
                            presi[i] = items[j].DocNumber;
                            i++;
                        }
                    }
                    // MEV CS 1.5
                    // verifico se è attivo l'alert sulla percentuale massima di documenti verificabili
                    if (Utils.ConservazioneManager.IsAlertConservazioneAttivo(infoUtente.idAmministrazione, "LEGGIBILITA_PERC"))
                    {
                        // parametro soglia percentuale
                        int sogliaDoc = Int32.Parse(Utils.ConservazioneManager.GetParametriAlertConservazione(infoUtente.idAmministrazione, "LEGGIBILITA_PERC"));
                        
                        //se la percentuale scelta è maggiore della soglia invio l'alert
                        if (percent >= sogliaDoc)
                        {
                            //alert                            
                            Utils.ConservazioneManager.InvioAlertAsync(infoUtente, "LEGGIBILITA_PERC", this.hd_idIstanza.Value, IdSupporto);
                        }
                    }
                    //MEV CS 1.5
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verificaLeggibilita", "showVerificaLeggibilita('" + this.hd_idIstanza.Value + "','" + files + "',0,'" + IdSupporto + "','" + txtNoteDiVerifica.Text + "','" + txtDataProssimaVerifica.Text + "');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verificaLeggibilita", "showVerificaLeggibilita('" + this.hd_idIstanza.Value + "','" + files + "',0,'" + IdSupporto + "','" + txtNoteDiVerifica.Text + "','" + txtDataProssimaVerificaLegg.Text + "');", true);
                }
                if (rbtn_file_da_aprire.Checked)
                {
                    string files = "";
                    foreach (GridViewRow gvr in GridView1.Rows)
                    {
                        if (((CheckBox)gvr.FindControl("chk_doc")).Checked)
                        {
                            files += gvr.Cells[0].Text + ",";
                        }
                    }
                    //MEV CS 1.5
                    //ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verificaLeggibilita", "showVerificaLeggibilita('" + this.hd_idIstanza.Value + "','" + files + "',0,'"+IdSupporto+"','"+txtNoteDiVerifica.Text+"','"+txtDataProssimaVerifica.Text+"');", true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verificaLeggibilita", "showVerificaLeggibilita('" + this.hd_idIstanza.Value + "','" + files + "',0,'" + IdSupporto + "','" + txtNoteDiVerifica.Text + "','" + txtDataProssimaVerificaLegg.Text + "');", true);

                }

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiusura", "window.close();", true);

            }
        }

        private void FetchDataProssimaVerificaLeggibilita()
        {
            
            // MEV CS 1.5
            // la data di prossima verifica è stabilita a partire dal parametro di configurazione stabilito in amministrazione

            string idAmministrazione = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]).idAmministrazione;

            //valore in GIORNI
            int intervalloProssimaVerifica = Convert.ToInt32(Utils.ConservazioneManager.GetChiaveConfigurazione(idAmministrazione, "BE_CONS_VER_LEG_INTERVALLO"));

            WSConservazioneLocale.Policy policy = Utils.ConservazioneManager.GetPolicyIstanza((WSConservazioneLocale.InfoUtente)Session["infoutCons"], this.IdIstanza);

            try
            {

                if (policy != null)
                {
                    // Istanza creata tramite policy: impostazione del numero di mesi definiti in essa

                    int mesiPolicy;
                    Int32.TryParse(policy.avvisoMesiLegg, out mesiPolicy);
                    if (mesiPolicy == 0)
                    {
                        //mesiPolicy = mesiDefault;
                        this.txtDataProssimaVerificaLegg.Text = DateTime.Today.AddDays(intervalloProssimaVerifica).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        this.txtDataProssimaVerificaLegg.Text = DateTime.Today.AddMonths(mesiPolicy).ToString("dd/MM/yyyy");
                    }
                }
                else
                {
                    // Default: prossimo controllo a 6 mesi da oggi
                    //this.txtDataProssimaVerifica.Text = DateTime.Today.AddMonths(mesiDefault).ToString("dd/MM/yyyy");

                    //MEV CS 1.5
                    //imposto il parametro definito in amministrazione
                    this.txtDataProssimaVerificaLegg.Text = DateTime.Today.AddDays(intervalloProssimaVerifica).ToString("dd/MM/yyyy");
                }

            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "fetchDataProxLeggKO", "alert('Errore nel recupero della data di prossima verifica di leggibilità.');", true);
                this.txtDataProssimaVerificaLegg.Text = string.Empty;
            }
        }

        private void FetchDataProssimaVerificaIntegrita()
        {
            // Se l'istanza è stata creata tramite policy, riporta il numero dei mesi definiti nella stessa, altrimenti il default è 6 mesi

            // MEV CS 1.5
            // la data di prossima verifica è stabilita a partire dal parametro di configurazione stabilito in amministrazione

            string idAmministrazione = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]).idAmministrazione;

            //valore in GIORNI
            int intervalloProssimaVerifica = Convert.ToInt32(Utils.ConservazioneManager.GetChiaveConfigurazione(idAmministrazione, "BE_CONSERVAZIONE_INTERVALLO"));

            WSConservazioneLocale.Policy policy = Utils.ConservazioneManager.GetPolicyIstanza((WSConservazioneLocale.InfoUtente)Session["infoutCons"], this.IdIstanza);

            //int mesiDefault = 6;

            try
            {

                if (policy != null)
                {
                    // Istanza creata tramite policy: impostazione del numero di mesi definiti in essa

                    int mesiPolicy;
                    Int32.TryParse(policy.avvisoMesi, out mesiPolicy);
                    if (mesiPolicy == 0)
                    {
                        //mesiPolicy = mesiDefault;
                        this.txtDataProssimaVerifica.Text = DateTime.Today.AddDays(intervalloProssimaVerifica).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        this.txtDataProssimaVerifica.Text = DateTime.Today.AddMonths(mesiPolicy).ToString("dd/MM/yyyy");
                    }
                }
                else
                {
                    // Default: prossimo controllo a 6 mesi da oggi
                    //this.txtDataProssimaVerifica.Text = DateTime.Today.AddMonths(mesiDefault).ToString("dd/MM/yyyy");

                    //MEV CS 1.5
                    //imposto il parametro definito in amministrazione
                    this.txtDataProssimaVerifica.Text = DateTime.Today.AddDays(intervalloProssimaVerifica).ToString("dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "fetchDataProxIntKO", "alert('Errore nel recupero della data di prossima verifica di integrità.');", true);
                txtDataProssimaVerifica.Text = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string IdIstanza
        {
            get
            {
                return this.Request.QueryString["idConservazione"];
            }
        }


        /// <summary>
        /// 
        /// </summary>
        protected string IdDocumento
        {
            get
            {
                return this.Request.QueryString["idDocumento"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string IdSupporto
        {
            get
            {
                return this.Request.QueryString["idSupporto"];
            }
        }

        protected void btn_verifica_integrita_Click(object sender, EventArgs e)
        {
            bool errore = false; int number; int totDocs;
            Int32.TryParse(hd_totDocs.Value, out totDocs);
            Int32.TryParse(tb_num_file.Text, out number);
            int percent = 0;
            string descrizione, esito;
            WSConservazioneLocale.Esito Cha_Esito;

            if (!rbtn_file_da_aprire.Checked && !rbtn_numero_file.Checked && !rbtn_percent_file.Checked)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_selezione", "alert('Selezionare un opzione!');", true);
                errore = true;
            }
            else
            {
                if (rbtn_numero_file.Checked && string.IsNullOrEmpty(tb_num_file.Text))
                {
                    errore = true;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_insNum", "alert('Inserire il numero di file da verificare!');", true);
                }
                else if (rbtn_numero_file.Checked && !Int32.TryParse(tb_num_file.Text, out number))
                {
                    errore = true;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_insNum2", "alert('Inserire un valore numerico.');", true);
                }
                else if (rbtn_numero_file.Checked && (number > totDocs || number < 3))
                {
                    errore = true;
                    if (totDocs > 2)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_insNum3", "alert('Il valore inserito non è valido. Inserire un valore compreso tra 3 e " + totDocs + "');", true);
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_inNum4", "alert('Il valore inserito non è valido. Numero di documenti non sufficiente per la selezione casuale.');", true);

                }
                else
                    if (rbtn_percent_file.Checked && (string.IsNullOrEmpty(tb_percent_file.Text) || !Int32.TryParse(tb_percent_file.Text, out number)))
                    {
                        errore = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_percent1", "alert('Inserire una percentuale valida.');", true);
                    }
                    else if (rbtn_percent_file.Checked && (number > 100 || number < 1))
                    {
                        errore = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_percent2", "alert('Inserire una percentuale valida.');", true);
                    }
                    else

                        if (rbtn_file_da_aprire.Checked)
                        {
                            errore = true;
                            foreach (GridViewRow grv in GridView1.Rows)
                            {
                                if (((CheckBox)grv.FindControl("chk_doc")).Checked) errore = false;
                            }
                            if (errore)
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_selezione", "alert('Selezionare quali file verificare!');", true);
                        }

            }
            if (!errore)
            {
                bool integritaVerificata = true;

                WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);

                Dictionary<String, String> documentiMemorizzati = ConservazioneWA.Utils.ConservazioneManager.getFilesFromUniSincro(infoUtente, IdIstanza,false);
                WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(IdIstanza, infoUtente);
                int totDocs2 = items.Length;
                
                string[] presi = null;
                if (rbtn_numero_file.Checked || rbtn_percent_file.Checked)
                {

                    if (rbtn_numero_file.Checked)
                    {
                        number = Int32.Parse(tb_num_file.Text);
                        percent = (int)Math.Ceiling((double)((double)(100 * number) / totDocs));
                        
                    }
                    else
                    {
                        percent = Int32.Parse(tb_percent_file.Text);
                        double op = (totDocs * 100 / percent);
                        number = (int)Math.Ceiling((double)((double)(totDocs * percent) / 100));
                    }
                    Random rdm = new Random();
                    presi = new string[number];
                    int i = 0;
                    while (i < number)
                    {
                        int j = rdm.Next(totDocs);
                        if (!presi.Contains(items[j].DocNumber))
                        {
                            
                            presi[i] = items[j].DocNumber;
                            i++;
                        }
                    }
                }
                if (rbtn_file_da_aprire.Checked)
                {
                    string files = "";
                    number = 0;
                    foreach (GridViewRow gvr in GridView1.Rows)
                    {
                        if (((CheckBox)gvr.FindControl("chk_doc")).Checked)
                        {
                            files += gvr.Cells[0].Text + ",";
                            number++;
                        }
                    }
                    presi = files.Split(',');
                    percent = (int)Math.Ceiling((double)((double)(100 * number) / totDocs));
                        
                }
                string unisincroitem = "", path = "", hash = "";
                string[] uniSincroItems = null;
                string hashValue = null; int validi = 0, invalidi = 0;

                foreach (string docnum in presi)
                {
                    if (!string.IsNullOrEmpty(docnum))
                    {
                        unisincroitem = documentiMemorizzati[docnum];
                        uniSincroItems = unisincroitem.Split('§');
                        path = uniSincroItems[2];
                        hash = uniSincroItems[3];

                        hashValue = ConservazioneWA.Utils.ConservazioneManager.getFileHashFromStore(infoUtente, IdIstanza, path,false);
                        string segnaturaOrId = ConservazioneWA.Utils.ConservazioneManager.getSegnatura_ID_Doc(docnum);

                        if (hash.ToLower() != hashValue.ToLower()) 
                        { integritaVerificata = false; 
                          invalidi++;

                          
                          // Modifica scrittura Registro di Conservazione per la scrittura di Verifica Integrità documento
                          WSConservazioneLocale.RegistroCons regCons = new WSConservazioneLocale.RegistroCons();
                          regCons.idAmm = infoUtente.idAmministrazione;
                          regCons.idIstanza = hd_idIstanza.Value;
                          regCons.idOggetto =  docnum;
                          regCons.tipoOggetto = "D";
                          regCons.tipoAzione = "";
                          regCons.userId = infoUtente.userId;
                          regCons.codAzione = "INTEGRITA_STORAGE";
                          regCons.descAzione = "Esecuzione della verifica di integrità del documento" + segnaturaOrId + "  dell’istanza " + hd_idIstanza.Value;
                          regCons.esito = "0";
                          ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons, infoUtente);  
 

                        }
                        else 
                        { validi++;
                          // Modifica scrittura Registro di Conservazione per la scrittura di Verifica Integrità documento
                          WSConservazioneLocale.RegistroCons regCons = new WSConservazioneLocale.RegistroCons();
                         regCons.idAmm = infoUtente.idAmministrazione;
                         regCons.idIstanza = hd_idIstanza.Value;
                         regCons.idOggetto = docnum;
                         regCons.tipoOggetto = "D";
                         regCons.tipoAzione = "";
                         regCons.userId = infoUtente.userId;
                         regCons.codAzione = "INTEGRITA_STORAGE";
                         regCons.descAzione = "Esecuzione della verifica di integrità del documento" + segnaturaOrId + "  dell’istanza " + hd_idIstanza.Value;
                         regCons.esito = "1";
                         ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons, infoUtente);  

                        }
                    }

                }
                if (!integritaVerificata)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_fallimento", "alert('Verifica Integrità fallita.');", true);
                    //ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(hd_idCons.Value, false);
                    descrizione = "Esecuzione della verifica di integrità dei documenti dell’istanza  " + hd_idIstanza.Value;
                    esito = "0";
                    Cha_Esito = WSConservazioneLocale.Esito.KO;

                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_successo", "alert('Verifica Integrità eseguita con successo.');", true);
                    //ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(hd_idCons.Value, true);
                    descrizione = "Esecuzione della verifica di integrità dei documenti dell’istanza " + hd_idIstanza.Value;
                    esito = "1";
                    Cha_Esito = WSConservazioneLocale.Esito.OK;

                }

                // Memorizzazione esito verifica solamente per le verifiche sull'intero supporto, non sul singolo documento

                Utils.ConservazioneManager.RegistraEsitoVerificaSupportoRegistrato(
                                    (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                                    this.IdIstanza,
                                    this.IdSupporto,
                                    integritaVerificata,
                                    percent.ToString(),
                                    this.txtDataProssimaVerifica.Text,
                                    "Verifica Integrità: "+this.txtNoteDiVerifica.Text,
                                    "I");

                this.ClientScript.RegisterStartupScript(this.GetType(), "close", "window.returnValue=true; window.close();", true);
                ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroControlli(IdIstanza, "", infoUtente, "Verifica Integrità storage", integritaVerificata, number, validi, invalidi);

                // Modifica scrittura Registro di Conservazione per la scrittura di Verifica Integrità supporto intero
                WSConservazioneLocale.RegistroCons regCons2 = new WSConservazioneLocale.RegistroCons();
                regCons2.idAmm = infoUtente.idAmministrazione;
                regCons2.idIstanza = hd_idIstanza.Value;
                regCons2.tipoOggetto = "I";
                regCons2.tipoAzione = "";
                regCons2.userId = infoUtente.userId;
                regCons2.codAzione = "INTEGRITA_STORAGE";
                regCons2.descAzione = descrizione;
                regCons2.esito = esito;
                ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons2, infoUtente);

                // Inserisce nel DPA_LOG la Verifica Integrià dell'istanza
                ConservazioneWA.Utils.ConservazioneManager.inserimentoInDpaLog(infoUtente, "INTEGRITA_STORAGE", IdIstanza, descrizione, Cha_Esito);


            
            }

        }

        protected void btn_verifica_unificata_Click(object sender, EventArgs e)
        {
            bool errore = false; int number; int totDocs;
            Int32.TryParse(hd_totDocs.Value, out totDocs);
            Int32.TryParse(tb_num_file.Text, out number);
            int percent = 0;
            string descrizione, esito;
            WSConservazioneLocale.Esito Cha_Esito;

            if (!rbtn_file_da_aprire.Checked && !rbtn_numero_file.Checked && !rbtn_percent_file.Checked)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_selezione", "alert('Selezionare un opzione!');", true);
                errore = true;
            }
            else
            {
                if (rbtn_numero_file.Checked && string.IsNullOrEmpty(tb_num_file.Text))
                {
                    errore = true;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_insNum", "alert('Inserire il numero di file da verificare!');", true);
                }
                else if (rbtn_numero_file.Checked && !Int32.TryParse(tb_num_file.Text, out number))
                {
                    errore = true;
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_insNum2", "alert('Inserire un valore numerico.');", true);
                }
                else if (rbtn_numero_file.Checked && (number > totDocs || number < 3))
                {
                    errore = true;
                    if (totDocs > 2)
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_insNum3", "alert('Il valore inserito non è valido. Inserire un valore compreso tra 3 e " + totDocs + "');", true);
                    else
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_inNum4", "alert('Il valore inserito non è valido. Numero di documenti non sufficiente per la selezione casuale.');", true);

                }
                else
                    if (rbtn_percent_file.Checked && (string.IsNullOrEmpty(tb_percent_file.Text) || !Int32.TryParse(tb_percent_file.Text, out number)))
                    {
                        errore = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_percent1", "alert('Inserire una percentuale valida.');", true);
                    }
                    else if (rbtn_percent_file.Checked && (number > 100 || number < 1))
                    {
                        errore = true;
                        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_percent2", "alert('Inserire una percentuale valida.');", true);
                    }
                    else

                        if (rbtn_file_da_aprire.Checked)
                        {
                            errore = true;
                            foreach (GridViewRow grv in GridView1.Rows)
                            {
                                if (((CheckBox)grv.FindControl("chk_doc")).Checked) errore = false;
                            }
                            if (errore)
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_selezione", "alert('Selezionare quali file verificare!');", true);
                        }

            }
            if (!errore)
            {
                bool integritaVerificata = true;

                WSConservazioneLocale.InfoUtente infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);

                Dictionary<String, String> documentiMemorizzati = ConservazioneWA.Utils.ConservazioneManager.getFilesFromUniSincro(infoUtente, IdIstanza,false);
                WSConservazioneLocale.ItemsConservazione[] items = ConservazioneWA.Utils.ConservazioneManager.getItemsConservazione(IdIstanza, infoUtente);
                int totDocs2 = items.Length;
                string files = "";
                string[] presi = null;
                if (rbtn_numero_file.Checked || rbtn_percent_file.Checked)
                {

                    if (rbtn_numero_file.Checked)
                    {
                        number = Int32.Parse(tb_num_file.Text);
                        percent = (int)Math.Ceiling((double)((double)(100 * number) / totDocs));

                    }
                    else
                    {
                        percent = Int32.Parse(tb_percent_file.Text);
                        double op = (totDocs * 100 / percent);
                        number = (int)Math.Ceiling((double)((double)(totDocs * percent) / 100));
                    }
                    Random rdm = new Random();
                    presi = new string[number];
                    int i = 0;
                    while (i < number)
                    {
                        int j = rdm.Next(totDocs);
                        if (!presi.Contains(items[j].DocNumber))
                        {
                            files += items[j].DocNumber + ",";                            
                            presi[i] = items[j].DocNumber;
                            i++;
                        }
                    }
                }
                if (rbtn_file_da_aprire.Checked)
                {
                    
                    number = 0;
                    foreach (GridViewRow gvr in GridView1.Rows)
                    {
                        if (((CheckBox)gvr.FindControl("chk_doc")).Checked)
                        {
                            files += gvr.Cells[0].Text + ",";
                            number++;
                        }
                    }
                    presi = files.Split(',');
                    percent = (int)Math.Ceiling((double)((double)(100 * number) / totDocs));

                }
                string unisincroitem = "", path = "", hash = "";
                string[] uniSincroItems = null;
                string hashValue = null; int validi = 0, invalidi = 0;

                foreach (string docnum in presi)
                {
                    if (!string.IsNullOrEmpty(docnum))
                    {
                        unisincroitem = documentiMemorizzati[docnum];
                        uniSincroItems = unisincroitem.Split('§');
                        path = uniSincroItems[2];
                        hash = uniSincroItems[3];

                        hashValue = ConservazioneWA.Utils.ConservazioneManager.getFileHashFromStore(infoUtente, IdIstanza, path,false);
                        string segnaturaOrId = ConservazioneWA.Utils.ConservazioneManager.getSegnatura_ID_Doc(docnum);

                        if (hash.ToLower() != hashValue.ToLower()) 
                        {   
                            integritaVerificata = false; 
                            invalidi++;

                            // Modifica scrittura Registro di Conservazione per la scrittura di Verifica Unificata documento
                            WSConservazioneLocale.RegistroCons regCons = new WSConservazioneLocale.RegistroCons();
                            regCons.idAmm = infoUtente.idAmministrazione;
                            regCons.idIstanza = hd_idIstanza.Value;
                            regCons.idOggetto = docnum;
                            regCons.tipoOggetto = "D";
                            regCons.tipoAzione = "";
                            regCons.userId = infoUtente.userId;
                            regCons.codAzione = "INTEGRITA_STORAGE_";
                            regCons.descAzione = "Verifica fallita integrità documento " + segnaturaOrId + " per istanza id: " + hd_idIstanza.Value;
                            regCons.esito = "0";
                            ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons, infoUtente);  
                        
                        }
                        else 
                        {

                            // Modifica scrittura Registro di Conservazione per la scrittura di Verifica Unificata documento
                            WSConservazioneLocale.RegistroCons regCons = new WSConservazioneLocale.RegistroCons();
                            regCons.idAmm = infoUtente.idAmministrazione;
                            regCons.idIstanza = hd_idIstanza.Value;
                            regCons.idOggetto = docnum;
                            regCons.tipoOggetto = "D";
                            regCons.tipoAzione = "";
                            regCons.userId = infoUtente.userId;
                            regCons.codAzione = "INTEGRITA_STORAGE";
                            regCons.descAzione = "Verifica corretta integrità documento " + segnaturaOrId + " per istanza id: " + hd_idIstanza.Value;
                            regCons.esito = "1";
                            ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons, infoUtente);  

                            validi++; 
                        }
                    }

                }
                if (!integritaVerificata)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_fallimento", "alert('Verifica Integrità fallita.');", true);
                    //ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(hd_idCons.Value, false);
                    Utils.ConservazioneManager.RegistraEsitoVerificaSupportoRegistrato(
                                    (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                                    this.IdIstanza,
                                    this.IdSupporto,
                                    integritaVerificata,
                                    percent.ToString(),
                                    this.txtDataProssimaVerifica.Text,
                                    this.txtNoteDiVerifica.Text,
                                    "U");

                    descrizione = "Esecuzione della verifica di integrità dei documenti dell’istanza "+ hd_idIstanza.Value + " nel repository del sistema" ;
                    esito = "0";
                    Cha_Esito = WSConservazioneLocale.Esito.KO;


                }
                else
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_successo", "alert('Verifica Integrità eseguita con successo.');", true);
                    // MEV CS 1.5
                    // mancava la registrazione in caso di verifica con esito positivo
                    Utils.ConservazioneManager.RegistraEsitoVerificaSupportoRegistrato(
                                    (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                                    this.IdIstanza,
                                    this.IdSupporto,
                                    integritaVerificata,
                                    percent.ToString(),
                                    this.txtDataProssimaVerifica.Text,
                                    "Verifica unificata (Integrità): "+this.txtNoteDiVerifica.Text,
                                    "U");
                    // end MEV CS 1.5

                    //ConservazioneWA.Utils.ConservazioneManager.esitoLeggibilita(hd_idCons.Value, true);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "verificaLeggibilita", "showVerificaLeggibilita('" + this.hd_idIstanza.Value + "','" + files + "',1,'" + IdSupporto + "','" + txtNoteDiVerifica.Text + "','" + txtDataProssimaVerifica.Text + "');", true);

                    descrizione = "Esecuzione della verifica di integrità dei documenti dell’istanza " + hd_idIstanza.Value + " nel repository del sistema";
                    esito = "1";
                    Cha_Esito = WSConservazioneLocale.Esito.OK;
                    
                }

                // Memorizzazione esito verifica solamente per le verifiche sull'intero supporto, non sul singolo documento

                //Utils.ConservazioneManager.RegistraEsitoVerificaSupportoRegistrato(
                //                    (WSConservazioneLocale.InfoUtente)Session["infoutCons"],
                //                    this.IdIstanza,
                //                    this.IdSupporto,
                //                    integritaVerificata,
                //                    percent.ToString(),
                //                    this.txtDataProssimaVerifica.Text,
                //                    this.txtNoteDiVerifica.Text);

                this.ClientScript.RegisterStartupScript(this.GetType(), "close", "window.returnValue=true; window.close();", true);
                ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroControlli(IdIstanza, "", infoUtente, "Verifica Integrità storage", integritaVerificata, number, validi, invalidi);

                // Modifica scrittura Registro di Conservazione per la scrittura di Verifica Integrità unificata supporto intero
                WSConservazioneLocale.RegistroCons regCons2 = new WSConservazioneLocale.RegistroCons();
                regCons2.idAmm = infoUtente.idAmministrazione;
                regCons2.idIstanza = hd_idIstanza.Value;
                regCons2.tipoOggetto = "I";
                regCons2.tipoAzione = "";
                regCons2.userId = infoUtente.userId;
                regCons2.codAzione = "INTEGRITA_STORAGE";
                regCons2.descAzione = descrizione;
                regCons2.esito = esito;
                ConservazioneWA.Utils.ConservazioneManager.inserimentoInRegistroCons(regCons2, infoUtente);

                // Inserisce nel DPA_LOG la Verifica Integrità Unificata dell'istanza
                ConservazioneWA.Utils.ConservazioneManager.inserimentoInDpaLog(infoUtente, "INTEGRITA_STORAGE", IdIstanza, descrizione, Cha_Esito);
                        
            }

        }
    }
}
