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
using System.Drawing; 
using DocsPAWA.dataSet;
using System.Text;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.popup
{
    public partial class areaConservazione : DocsPAWA.CssPage
    {
        protected DocsPAWA.dataSet.DataSetRAreaCons dataSetRcons = new DocsPAWA.dataSet.DataSetRAreaCons();
        protected DocsPAWA.dataSet.DataSetRItemsCons dataSetRItemsCons = new DocsPAWA.dataSet.DataSetRItemsCons();
        protected DocsPAWA.DocsPaWR.InfoUtente infoUtente;
        protected DocsPAWA.DocsPaWR.Utente utente= new DocsPAWA.DocsPaWR.Utente();
        protected DocsPAWA.DocsPaWR.Ruolo ruolo = new DocsPAWA.DocsPaWR.Ruolo();
        protected DocsPAWA.DocsPaWR.InfoConservazione[] infoCons;
        protected DocsPAWA.DocsPaWR.ItemsConservazione[] itemsCons;
        protected DocsPAWA.DocsPaWR.TipoSupporto[] tipoSupp;
        protected DocsPaWR.DocsPaWebService _wsInstance = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            if (!Page.IsPostBack)
            {
                infoUtente = new DocsPAWA.DocsPaWR.InfoUtente();
                infoUtente = UserManager.getInfoUtente(this);
                utente = UserManager.getUtente(this);
                ruolo = UserManager.getRuolo(this);
                Session["userData"] = utente;
                Session["userRuolo"] = ruolo;
                Session["infoUtente"] = infoUtente;
                this.idPeo.Value = infoUtente.idPeople;
                this.idRuoloUo.Value = infoUtente.idCorrGlobali;
                string filtro = this.Filtra();
                infoCons = DocumentManager.getListaConservazioneByFiltro(filtro, this);
                //infoCons = DocumentManager.getListaConservazione(idPeo.Value, idRuoloUo.Value, Page);
                this.caricaGridviewIstanze(infoCons, this.gv_istanzeCons);
                tipoSupp = DocumentManager.getListaTipiSupporti(Page);

                // Caricamento lista dei tipi conservazione
                this.FetchTipiConservazione();

                this.CaricaDropDownListPolicy();

                this.chk_consolida.Checked = true;
                this.chk_consolida.Enabled = false;
            }

            if (this.MustSendInstance())
            {
                // Invio istanza di conservazione
                this.SendInstance();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void FetchTipiConservazione()
        {
            this.ddl_tipoCons.DataSource = ProxyManager.getWS().GetTipologieIstanzeConservazione();
            this.ddl_tipoCons.DataBind();
        }


        protected void caricaGridviewIstanze(DocsPAWA.DocsPaWR.InfoConservazione[] infoCons, GridView gv)
        {
            this.dataSetRcons = new DocsPAWA.dataSet.DataSetRAreaCons();
            for (int i = 0; i < infoCons.Length; i++)
            {
                this.dataSetRcons.DataTable1.AddDataTable1Row(infoCons[i].SystemID, infoCons[i].StatoConservazione, infoCons[i].decrSupporto,
                    infoCons[i].Note, infoCons[i].Descrizione, infoCons[i].Data_Apertura, infoCons[i].Data_Invio, infoCons[i].Data_Conservazione, infoCons[i].TipoConservazione, infoCons[i].automatica, infoCons[i].noteRifiuto, (infoCons[i].consolida).ToString(), infoCons[i].idPolicyValidata,(infoCons[i].predefinita).ToString());
            }
            gv.DataSource = this.dataSetRcons.Tables[0];
            gv.DataBind();
            Session["infoCons"] = infoCons;
            //    this.gv_istanzeCons.DataSource = this.dataSetRcons.Tables[0];
            //gv_istanzeCons.DataBind();
        }

        protected void gv_istanzeCons_SelectedIndexChanged(object sender, EventArgs e)
        {
            string idIstanza = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("lbl_idIstanza")).Text.ToString();
            this.idIstanza.Value = idIstanza;

            string stato = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("lb_stato")).Text;
            string policy_validazione = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("idpolicyvalidazione")).Text;

            if (!string.IsNullOrEmpty(policy_validazione))
            {
                bool result = _wsInstance.ValidateIstanzaConservazioneConPolicy(policy_validazione, this.idIstanza.Value, UserManager.getInfoUtente());
            }

            itemsCons = DocumentManager.getItemsConservazioneLite(this.idIstanza.Value, Page, UserManager.getInfoUtente());
            if (itemsCons != null)
            {
                this.caricaGridviewDettaglio(itemsCons, this.gv_dettaglioCons);


                this.statoIstanza.Value = stato;
                if (stato != "Nuova")
                {
                    this.disabilitaCestino();
                    this.btn_sottoscrivi.Enabled = false;
                    this.btn_eliminaTutti.Enabled = false;
                    this.btn_elimina_policy.Enabled = false;
                    this.txt_note.Enabled = false;
                    this.txt_descrizione.Enabled = false;
                    this.ddl_tipoCons.Enabled = false;
                    //this.ddl_supporto.Enabled = false;
                    this.ddl_policy.Enabled = false;
                    this.btn_predefinita.Enabled = false;
                    
                    string descrizione = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("descrizione")).Text;
                    string note = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("note")).Text;
                    string consolidamento = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("consolida")).Text;
                    string tipo_cons = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("tipo_cons2")).Text;
                   
                    bool valCons = false;
                    bool.TryParse(consolidamento, out valCons);
                    this.txt_descrizione.Text = descrizione;
                    this.txt_note.Text = note;
                    if(!string.IsNullOrEmpty(tipo_cons))
                    {
                        if (this.ddl_tipoCons.Items.FindByValue(tipo_cons) != null)
                        {
                            this.ddl_tipoCons.SelectedValue = tipo_cons;
                            if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_CONSOLIDATA")
                            {
                                this.chk_consolida.Checked = true;
                                this.chk_consolida.Enabled = false;
                            }
                            if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_NON_CONSOLIDATA")
                            {
                                this.chk_consolida.Checked = false;
                                this.chk_consolida.Enabled = false;
                            }
                            if (this.ddl_tipoCons.SelectedValue == "ESIBIZIONE")
                            {
                                this.chk_consolida.Checked = false;
                                this.chk_consolida.Enabled = false;
                            }
                            if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_INTERNA")
                            {
                                this.chk_consolida.Checked = false;
                                this.chk_consolida.Enabled = true;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(policy_validazione))
                    {
                        ListItem temp = new ListItem();
                        temp.Value = policy_validazione;
                        if (this.ddl_policy.Items.FindByValue(temp.Value) != null)
                        {
                            this.ddl_policy.SelectedValue = policy_validazione;
                        }
                    }
                    else
                    {
                        this.ddl_policy.SelectedValue = string.Empty;
                    }
                }
                else
                {
                    if (itemsCons.Length > 0)
                    {
                        this.btn_sottoscrivi.Enabled = true;
                        this.btn_eliminaTutti.Enabled = true;
                        this.btn_elimina_policy.Enabled = true;
                        this.txt_note.Enabled = true;
                        this.txt_descrizione.Enabled = true;
                        this.ddl_tipoCons.Enabled = true;
                        //this.ddl_supporto.Enabled = true;
                        this.txt_descrizione.Text = string.Empty;
                        this.txt_note.Text = string.Empty;
                        //this.ddl_tipoCons.Items[0].Selected = true;
                        this.ddl_policy.Enabled = true;
                        this.ddl_policy.SelectedValue = string.Empty;
                        if (!string.IsNullOrEmpty(policy_validazione))
                        {
                            ListItem temp = new ListItem();
                            temp.Value = policy_validazione;
                            if (this.ddl_policy.Items.FindByValue(temp.Value) != null)
                            {
                                this.ddl_policy.SelectedValue = policy_validazione;
                            }
                            this.btn_elimina_policy.Enabled = true;
                        }
                        else
                        {
                            this.ddl_policy.SelectedValue = string.Empty;
                            this.btn_elimina_policy.Enabled = false;
                        }
                        string consolidamento = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("consolida")).Text;
                        string tipo_cons = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("tipo_cons2")).Text;

                        if (!string.IsNullOrEmpty(tipo_cons))
                        {
                            if (this.ddl_tipoCons.Items.FindByValue(tipo_cons) != null)
                            {
                                this.ddl_tipoCons.SelectedValue = tipo_cons;
                                if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_CONSOLIDATA")
                                {
                                    this.chk_consolida.Checked = true;
                                    this.chk_consolida.Enabled = false;
                                }
                                if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_NON_CONSOLIDATA")
                                {
                                    this.chk_consolida.Checked = false;
                                    this.chk_consolida.Enabled = false;
                                }
                                if (this.ddl_tipoCons.SelectedValue == "ESIBIZIONE")
                                {
                                    this.chk_consolida.Checked = false;
                                    this.chk_consolida.Enabled = false;
                                }
                                if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_INTERNA")
                                {
                                    this.chk_consolida.Checked = false;
                                    this.chk_consolida.Enabled = true;
                                }
                            }
                        }

                        string predefinita = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("lbl_preferita")).Text;
                        if (!string.IsNullOrEmpty(predefinita) && predefinita.Equals("True"))
                        {
                            this.btn_predefinita.Enabled = false;
                        }
                        else
                        {
                            this.btn_predefinita.Enabled = true;
                        }
                        this.abilitaCestino();
                    }
                    else
                    {
                        this.btn_sottoscrivi.Enabled = false;
                        this.btn_eliminaTutti.Enabled = false;
                        this.btn_elimina_policy.Enabled = false;
                        this.btn_predefinita.Enabled = false;

                        //
                        this.txt_note.Enabled = true;
                        this.txt_descrizione.Enabled = true;
                        this.ddl_tipoCons.Enabled = true;
                        //this.ddl_supporto.Enabled = true;
                        this.txt_descrizione.Text = string.Empty;
                        this.txt_note.Text = string.Empty;
                        //this.ddl_tipoCons.Items[0].Selected = true;
                        this.ddl_policy.Enabled = true;
                        this.ddl_policy.SelectedValue = string.Empty;
                        if (!string.IsNullOrEmpty(policy_validazione))
                        {
                            ListItem temp = new ListItem();
                            temp.Value = policy_validazione;
                            if (this.ddl_policy.Items.FindByValue(temp.Value) != null)
                            {
                                this.ddl_policy.SelectedValue = policy_validazione;
                            }
                            this.btn_elimina_policy.Enabled = true;
                        }
                        else
                        {
                            this.ddl_policy.SelectedValue = string.Empty;
                            this.btn_elimina_policy.Enabled = false;
                        }
                        string consolidamento = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("consolida")).Text;
                        string tipo_cons = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("tipo_cons2")).Text;

                        if (!string.IsNullOrEmpty(tipo_cons))
                        {
                            if (this.ddl_tipoCons.Items.FindByValue(tipo_cons) != null)
                            {
                                this.ddl_tipoCons.SelectedValue = tipo_cons;
                                if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_CONSOLIDATA")
                                {
                                    this.chk_consolida.Checked = true;
                                    this.chk_consolida.Enabled = false;
                                }
                                if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_NON_CONSOLIDATA")
                                {
                                    this.chk_consolida.Checked = false;
                                    this.chk_consolida.Enabled = false;
                                }
                                if (this.ddl_tipoCons.SelectedValue == "ESIBIZIONE")
                                {
                                    this.chk_consolida.Checked = false;
                                    this.chk_consolida.Enabled = false;
                                }
                                if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_INTERNA")
                                {
                                    this.chk_consolida.Checked = false;
                                    this.chk_consolida.Enabled = true;
                                }
                            }
                        }

                        //
                    }
                }
                if (stato == "Respinta")
                {
                    infoCons = (DocsPAWA.DocsPaWR.InfoConservazione[])Session["infoCons"];
                    bool istanzaNuova = false;
                    //controllo che nn ci sia un'altra istanza in stato nuovo, in questo nn abilito il pulsante per riabilitare una qualsiasi altra istanza
                    for (int i = 0; i < infoCons.Length; i++)
                    {
                        if (infoCons[i].StatoConservazione == "N")
                        {
                            istanzaNuova = true;
                            break;
                        }
                    }
                    if (!istanzaNuova && itemsCons.Length>0)
                    {
                        this.btn_riabilitaIstanza.Enabled = true;
                    }
                    else
                    {
                        this.btn_riabilitaIstanza.Enabled = false;
                    }
                }
                else
                {
                    this.btn_riabilitaIstanza.Enabled = false;
                }
                this.panelDettaglioIstanza.Visible = true;
            }
            this.upIstanze.Update();
            //itemsCons = DocumentManager.getItemsConservazione(this.idIstanza.Value, Page);
            //this.caricaGridviewDettaglio(itemsCons, this.gv_dettaglioCons);                
        }

        protected void caricaGridviewDettaglio(DocsPAWA.DocsPaWR.ItemsConservazione[] itemsCons, GridView gv)
        {
            float size = 0;
            this.dataSetRItemsCons = new DocsPAWA.dataSet.DataSetRItemsCons();
            for (int i = 0; i < itemsCons.Length; i++)
            {
                string segnatura=itemsCons[i].numProt_or_id;
                string data=itemsCons[i].data_prot_or_create;
                string data_doc= segnatura + "\n \n" + data;
                if(string.IsNullOrEmpty(itemsCons[i].dirittiDocumento))
                {
                    itemsCons[i].desc_oggetto = "Non hai visibilità";
                }

                this.dataSetRItemsCons.DataTable1.AddDataTable1Row(itemsCons[i].DocNumber, itemsCons[i].ID_Profile, itemsCons[i].TipoDoc,
                    itemsCons[i].desc_oggetto, itemsCons[i].CodFasc, itemsCons[i].Data_Ins, data_doc, itemsCons[i].SizeItem, itemsCons[i].numProt, itemsCons[i].StatoConservazione, itemsCons[i].tipoFile, itemsCons[i].numAllegati, itemsCons[i].ID_Project, itemsCons[i].immagineAcquisita, itemsCons[i].SystemID, itemsCons[i].dirittiDocumento, itemsCons[i].policyValida);

                //valorizzo la label che indica in megabyte la dimensione totale dei singoli items
                if (!string.IsNullOrEmpty(itemsCons[i].SizeItem))
                {
                    size = size + Convert.ToSingle(itemsCons[i].SizeItem);
                }

            }

            try
            {
                float sizeF = (float)size / 1048576;
                string size_appo = Convert.ToString(sizeF);
                if (size_appo.Contains(","))
                {
                    size_appo = size_appo.Substring(0, size_appo.IndexOf(",") + 2);
                }
                else
                {
                    if (size_appo.Contains("."))
                    {
                        size_appo = size_appo.Substring(0, size_appo.IndexOf(".") + 2);
                    }
                }
                this.lbl_size.Text = size_appo;
                this.lbl_numdocs.Text = itemsCons.Length.ToString();
            }                
            catch(Exception e)
            {

            }
            if (itemsCons.Length == 0)
            {
                this.btn_sottoscrivi.Enabled = false;
                this.btn_riabilitaIstanza.Enabled = false;
                this.btn_eliminaTutti.Enabled = false;
                this.btn_elimina_policy.Enabled = false;
            }
            this.gv_dettaglioCons.DataSource = this.dataSetRItemsCons.Tables[0];
            this.gv_dettaglioCons.DataBind();
            Session["itemsCons"] = itemsCons;
            this.upDettaglio.Update();
        }

        protected void gv_dettaglioCons_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = this.gv_dettaglioCons.SelectedIndex;
            string systemId = ((Label)this.gv_dettaglioCons.SelectedRow.FindControl("lbl_systemId")).Text.ToString();
            DocumentManager.eliminaDaAreaConservazione(Page, "", null, "", false, systemId);
            itemsCons = DocumentManager.getItemsConservazioneLite(this.idIstanza.Value, Page,UserManager.getInfoUtente());
            this.caricaGridviewDettaglio(itemsCons, this.gv_dettaglioCons);
        }

        protected void gv_dettaglioCons_DataBound(object sender, EventArgs e)
        {
            int size = 0;
            for (int i = 0; i < this.gv_dettaglioCons.Rows.Count; i++)
            {
                ((Label)this.gv_dettaglioCons.Rows[i].FindControl("lbl_numProt")).Font.Bold = true;
                if (((Label)this.gv_dettaglioCons.Rows[i].FindControl("lbl_numProt")).Text != String.Empty)
                {
                    ((Label)this.gv_dettaglioCons.Rows[i].FindControl("lbl_data_prot_or_crea")).ForeColor = Color.Red;
                }
                else
                {
                    ((Label)this.gv_dettaglioCons.Rows[i].FindControl("lbl_data_prot_or_crea")).ForeColor = Color.Black;
                }               
            }            
        }

        protected void btn_eliminaTutti_Click(object sender, EventArgs e)
        {
            DocumentManager.eliminaDaAreaConservazione(Page, null, null, this.idIstanza.Value, false, "");
            itemsCons = DocumentManager.getItemsConservazioneLite(this.idIstanza.Value, Page, UserManager.getInfoUtente());
            this.caricaGridviewDettaglio(itemsCons, this.gv_dettaglioCons);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool MustSendInstance()
        {
            bool res;
            bool.TryParse(this.hdForceSend.Value, out res);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void SendInstance()
        {

            string descrizione = this.txt_descrizione.Text;
            descrizione = descrizione.Replace("'", "''");
            string note = this.txt_note.Text;
            note = note.Replace("'", "''");
            //string appo_note = note.Replace("\r\n", " ");
            //note = appo_note;
            string tipo_cons = this.ddl_tipoCons.SelectedValue.ToString();
            //string supporto = this.ddl_supporto.SelectedValue.ToString();

            string supporto = string.Empty;

            if (DocumentManager.updateStatoAreaCons(this.Page, this.idIstanza.Value, tipo_cons, note, descrizione, supporto, UserManager.getInfoUtente(),this.chk_consolida.Checked))
            {
                
                this.btn_sottoscrivi.Enabled = false;
                this.btn_eliminaTutti.Enabled = false;
                this.btn_elimina_policy.Enabled = false;
                this.pulisciCampi();
                string filtro = this.Filtra();
                infoCons = DocumentManager.getListaConservazioneByFiltro(filtro, this);
                // infoCons = DocumentManager.getListaConservazione(idPeo.Value, idRuoloUo.Value, Page);
                this.caricaGridviewIstanze(infoCons, this.gv_istanzeCons);
                itemsCons = DocumentManager.getItemsConservazioneLite(this.idIstanza.Value, Page, UserManager.getInfoUtente());
                //if (this.chk_consolida.Checked)
                //{
                //    _wsInstance.InviaIstanzaInConservazione(itemsCons, UserManager.getInfoUtente());
                //}
                this.caricaGridviewDettaglio(itemsCons, this.gv_dettaglioCons);
                this.disabilitaCestino();
                this.hdForceSend.Value = "false";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "Inviocentroservizi", "alert(\"L\'istanza è stata inviata al Centro Servizi\");", true);

                //if (!this.Page.ClientScript.IsStartupScriptRegistered("Inviocentroservizi"))
                //    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Inviocentroservizi", "<script type=\"text/javascript\" language=\"javascript\">alert(\"L\'istanza è stata inviata al Centro Servizi\");</script>");
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            // Validazione istanza di conservazione
            DocsPaWR.AreaConservazioneValidationResult result = DocumentManager.validateIstanzaConservazione(this.idIstanza.Value, this.Page);

            if (result.IsValid)
            {
                // Invio istanza di conservazione
                this.SendInstance();
            }
            else
            {
                string sessionParam = "args";
                this.Session[sessionParam] = result;

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "showAreaConservazioneValidationMask", string.Format("showAreaConservazioneValidationMask('{0}', '{1}');", this.idIstanza.Value, sessionParam), true);

                //this.Page.ClientScript.RegisterStartupScript(this.GetType(),
                //                    "showAreaConservazioneValidationMask",
                //                    string.Format("showAreaConservazioneValidationMask('{0}', '{1}');", this.idIstanza.Value, sessionParam), 
                //                    true);

                
            }
            this.btn_predefinita.Enabled = false;
            hdForceSend.Value = "false";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fieldLength"></param>
        /// <returns></returns>
        private string GetReportFieldValue(string value, int fieldLength)
        {
            if (value.Length > fieldLength)
                value = string.Concat(value.Substring(0, (fieldLength - 3)), "...");

            return value.PadRight(fieldLength, ' ');
        }

        protected void pulisciCampi()
        {
            this.txt_descrizione.Text = "";
            this.txt_note.Text = "";
        }

        protected void disabilitaCestino()
        {
            for (int i = 0; i < this.gv_dettaglioCons.Rows.Count; i++)
            {
                (this.gv_dettaglioCons.Rows[i].Cells[8]).Enabled = false;
            }
        }

        protected void gv_dettaglioCons_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gv_dettaglioCons.PageIndex = e.NewPageIndex;
            itemsCons = DocumentManager.getItemsConservazioneLite(this.idIstanza.Value, Page, UserManager.getInfoUtente());
            this.caricaGridviewDettaglio(itemsCons, this.gv_dettaglioCons);  
        }

        protected void gv_istanzeCons_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gv_istanzeCons.PageIndex = e.NewPageIndex;
            string filtro = this.Filtra();
            infoCons = DocumentManager.getListaConservazioneByFiltro(filtro, this);
            //infoCons = DocumentManager.getListaConservazione(idPeo.Value, idRuoloUo.Value, Page);
            this.caricaGridviewIstanze(infoCons, this.gv_istanzeCons);
            this.panelDettaglioIstanza.Visible = false;
        }

        protected void gv_istanzeCons_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string idIstanza = ((Label)this.gv_istanzeCons.Rows[e.RowIndex].FindControl("lbl_idIstanza")).Text;
            this.idIstanza.Value = idIstanza;
            DocumentManager.eliminaDaAreaConservazione(Page, null, null, idIstanza, true, "");
            string filtro = this.Filtra();
            infoCons = DocumentManager.getListaConservazioneByFiltro(filtro, this);
            //infoCons = DocumentManager.getListaConservazione(idPeo.Value, idRuoloUo.Value, Page);
            this.caricaGridviewIstanze(infoCons, this.gv_istanzeCons);
            itemsCons = DocumentManager.getItemsConservazioneLite(this.idIstanza.Value, Page, UserManager.getInfoUtente());
            this.caricaGridviewDettaglio(itemsCons, this.gv_dettaglioCons);
            this.panelDettaglioIstanza.Visible = false;
            this.upIstanze.Update();
            this.upDettaglio.Update();
        }

        protected void gv_istanzeCons_PreRender(object sender, EventArgs e)
        {
            for(int i=0; i<this.gv_istanzeCons.Rows.Count; i++)
            {
                string stato = ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).Text;
                if (stato!="N" && stato!="Nuova" && stato!="R" && stato!="Respinta")
                {
                    //((System.Web.UI.WebControls.ImageButton)this.gv_istanzeCons.Rows[i].Cells[11].Controls[1]).Enabled = false;
                    //((System.Web.UI.WebControls.ImageButton)this.gv_istanzeCons.Rows[i].Cells[11].Controls[1]).OnClientClick = "return showModalDialogEliminaIstanza();";   
                    ((System.Web.UI.WebControls.ImageButton)this.gv_istanzeCons.Rows[i].Cells[12].Controls[1]).OnClientClick = "return showModalDialogEliminaIstanza();";   
                }
                if (stato == "N" || stato=="Nuova")
                {
                    ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).Text = "Nuova";
                    string predefinita = ((Label)this.gv_istanzeCons.Rows[i].FindControl("lbl_preferita")).Text;
                    if (!string.IsNullOrEmpty(predefinita) && predefinita.Equals("True"))
                    {
                        string idTemp = ((Label)this.gv_istanzeCons.Rows[i].FindControl("lbl_idIstanza")).Text.ToString();
                        ((Label)this.gv_istanzeCons.Rows[i].FindControl("lbl_idIstanzaVis")).Text = idTemp + "<strong>*</strong>";
                    }
                }
                if (stato == "I" || stato == "Inviata")
                {
                    ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).Text = "Inviata";
                }
                if (stato == "R" || stato=="Respinta")
                {
                    ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).Text = "Respinta";
                    ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).ForeColor = Color.Red;
                    ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).Font.Bold = true;
                }
                if (stato == "L" || stato=="In lavorazione" || stato=="E")
                {
                    ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).Text = "In lavorazione";
                }
                if (stato == "F" || stato=="Firmata")
                {
                    ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).Text = "Firmata";
                }
                if (stato == "C" || stato=="Chiusa")
                {
                    ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).Text = "Chiusa";
                }
                if (stato == "V" || stato == "Conservata")
                {
                    ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).Text = "Conservata";
                }
                //if (stato == "T" || stato == "In transizione")
                //{
                //    ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).Text = "In transizione";
                //}
                if (stato == "T" || stato == "In Chiusura")
                {
                    ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).Text = "In Chiusura";
                }
                if (stato == "Q" || stato == "In fase di verifica")
                {
                    ((Label)this.gv_istanzeCons.Rows[i].FindControl("lb_stato")).Text = "In fase di verifica";
                }

                string tipo_cons = ((Label)this.gv_istanzeCons.Rows[i].FindControl("tipo_cons")).Text;
                ListItem temp = new ListItem();
                temp.Value = tipo_cons;
                if (!string.IsNullOrEmpty(tipo_cons) && this.ddl_tipoCons.Items.FindByValue(temp.Value) != null)
                {
                    ((Label)this.gv_istanzeCons.Rows[i].FindControl("tipo_cons")).Text = this.ddl_tipoCons.Items.FindByValue(temp.Value).Text;
                }
            }
        }

        protected void gv_dettaglioCons_PreRender(object sender, EventArgs e)
        {
            string statoIstanza = "";
            if (!string.IsNullOrEmpty(this.statoIstanza.Value))
                statoIstanza = this.statoIstanza.Value;

            for (int i = 0; i < gv_dettaglioCons.Rows.Count; i++)
            {
                string stato = ((Label)this.gv_dettaglioCons.Rows[i].FindControl("lb_stato")).Text;
                string cha_img = ((Label)this.gv_dettaglioCons.Rows[i].FindControl("lbl_cha_img")).Text;
                string diritti = ((Label)this.gv_dettaglioCons.Rows[i].FindControl("lbl_diritti")).Text;

                // gestione della icona dei dettagli -- tolgo icona se non c'è doc acquisito
                if (cha_img == "0" || string.IsNullOrEmpty(diritti))
                {
                   // ((System.Web.UI.WebControls.ImageButton)this.gv_dettaglioCons.Rows[i].Cells[11].Controls[1]).Visible = false;
                    ((System.Web.UI.WebControls.ImageButton)this.gv_dettaglioCons.Rows[i].Cells[11].Controls[1]).Visible = false;
                }

                if (stato != "N" && statoIstanza!="R" && statoIstanza!="Respinta")
                {
                  //  ((System.Web.UI.WebControls.ImageButton)this.gv_dettaglioCons.Rows[i].Cells[9].Controls[1]).Enabled = false;
                    //((System.Web.UI.WebControls.ImageButton)this.gv_dettaglioCons.Rows[i].Cells[12].Controls[1]).OnClientClick = "return showModalDialogEliminaDoc();";
                    ((System.Web.UI.WebControls.ImageButton)this.gv_dettaglioCons.Rows[i].Cells[12].Controls[1]).OnClientClick = "return showModalDialogEliminaDoc();";
                }

                if (this.gv_dettaglioCons.Rows[i].Cells[2].Text == "G")
                {
                    this.gv_dettaglioCons.Rows[i].Cells[2].Text = "NP";
                }

                try
                {
                    int sizeByte = Convert.ToInt32(this.gv_dettaglioCons.Rows[i].Cells[7].Text);
                    int sizeKb = sizeByte / 1024;
                    string size_appo = Convert.ToString(sizeKb);
                    if (sizeByte > 1024)
                    {
                        this.gv_dettaglioCons.Rows[i].Cells[7].Text = size_appo;
                    }
                    else
                    {
                        this.gv_dettaglioCons.Rows[i].Cells[7].Text = "1";
                    }
                }
                catch(Exception ex)
                {
                }

                string validaPolicy = ((Label)this.gv_dettaglioCons.Rows[i].FindControl("lbl_policyValida")).Text;
                if (!string.IsNullOrEmpty(validaPolicy) && validaPolicy.Equals("1"))
                {
                    for (int t = 0; t < 12; t++)
                    {
                        this.gv_dettaglioCons.Rows[i].Cells[t].Attributes.Add("class", "non_valido");
                    }
                }

            }
        }

        protected void gv_dettaglioCons_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                itemsCons=((DocsPAWA.DocsPaWR.ItemsConservazione[])Session["itemsCons"]);
                if (e.CommandName == "VisualizzaDoc")
                {
                    int index = Convert.ToInt32(e.CommandArgument.ToString());
                    string docNumber = itemsCons[index].DocNumber;//((Label)this.gv_dettaglioCons.Rows[index].FindControl("lbl_docNumber")).Text;
                    string idProfile = itemsCons[index].ID_Profile;//((Label)this.gv_dettaglioCons.Rows[index].FindControl("lbl_idProfile")).Text;
                    DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumentoNoSecurity(this, idProfile, docNumber);
                    DocumentManager.setDocumentoSelezionato(this, schedaDoc);
                    FileManager.setSelectedFile(this, schedaDoc.documenti[0], false);
                    caricaGridviewDettaglio(itemsCons, this.gv_dettaglioCons);
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "lanciaVIs", "loadVisualizzatoreDocModal('" + Session.SessionID + "','" + docNumber + "','" + idProfile + "');", true);
                   // ClientScript.RegisterStartupScript(this.GetType(), "lanciaVIs", "loadVisualizzatoreDocModal('" + Session.SessionID + "','" + docNumber + "','" + idProfile + "');", true);
                }
            }
            catch(Exception ex)
            {
                ErrorManager.redirectToErrorPage(this, ex);
            }
        }

        protected void btn_filtra_Click(object sender, EventArgs e)
        {
            string filtro = this.Filtra();
            infoCons = DocumentManager.getListaConservazioneByFiltro(filtro, this);
            //infoCons = DocumentManager.getListaConservazioneByFiltro(filtro, this);
            if (infoCons != null)
                this.caricaGridviewIstanze(infoCons, this.gv_istanzeCons);
            this.panelDettaglioIstanza.Visible = false;
        }

        protected string Filtra()
        {
            string query = string.Empty;

            query = " WHERE ID_PEOPLE=" + idPeo.Value + " AND ID_RUOLO_IN_UO=" + idRuoloUo.Value;

            bool filterStart = false;

            if (this.cb_istanzeChiuse.Checked)
            {
                if (!filterStart)
                {
                    query += " AND (CHA_STATO ='C' ";
                    filterStart = true;
                }
                else
                {
                    query += " OR CHA_STATO ='C' ";
                }
            }
            if (this.cb_istanzeManuali.Checked)
            {
                if (!filterStart)
                {
                    query += " AND (ID_POLICY is null ";
                    filterStart = true;
                }
                else
                {
                   
                    query += " OR ID_POLICY is null ";
                }
            }
            if (this.cb_istanzeAutomatiche.Checked)
            {
                if (!filterStart)
                {
                    query += " AND (ID_POLICY is not null ";
                    filterStart = true;
                }
                else
                {
                    query += " OR ID_POLICY is not null ";
                }
            }

            if (filterStart)
            {
                query += ")";
            }

            return query;
        }

        protected void btn_riabilitaIstanza_Click(object sender, EventArgs e)
        {           
            itemsCons = (DocsPAWA.DocsPaWR.ItemsConservazione[])Session["itemsCons"];
            string idIstanza = itemsCons[0].ID_Conservazione;
            ArrayList idProjectFascInseriti = new ArrayList();
            int numFasc = 0;

            for (int i = 0; i < itemsCons.Length; i++)
            {
                string id_project = itemsCons[i].ID_Project;

                // se il documento fa parte di un fascicolo
                if (!string.IsNullOrEmpty(id_project))
                {
                    if (itemsCons[i].tipo_oggetto == "F") //se è inserito tutto il fascicolo
                    {
                        if (!isInserito(id_project, idProjectFascInseriti))
                        {
                            //if (DocumentManager.DeleteFromItemsConservazione(this, idIstanza, itemsCons[i].SystemID, id_project))
                            //{
                            this.inserisciFascicoloInConservazione(id_project);
                            idProjectFascInseriti.Add(id_project);
                            //}
                        }
                    }
                    else
                    {
                        this.inserisciDocumentoInConservazione(itemsCons[i].ID_Profile, itemsCons[i].DocNumber, id_project);
                    }
                }
                else
                {
                    this.inserisciDocumentoInConservazione(itemsCons[i].ID_Profile, itemsCons[i].DocNumber, id_project);

                }
            }

            this.btn_riabilitaIstanza.Enabled = false;
            this.panelDettaglioIstanza.Visible = false;
            //elimino l'istanza originale che è stata rifiutata
            DocumentManager.eliminaDaAreaConservazione(Page, null, null, idIstanza, true, "");
            string filtro = this.Filtra();
            infoCons = DocumentManager.getListaConservazioneByFiltro(filtro, this);
            this.caricaGridviewIstanze(infoCons, this.gv_istanzeCons);
            itemsCons = DocumentManager.getItemsConservazioneLite(this.idIstanza.Value, Page, UserManager.getInfoUtente());
            this.caricaGridviewDettaglio(itemsCons, this.gv_dettaglioCons);            
        }

        protected bool isInserito(string idProject,ArrayList idFascInseriti)
        {
            bool result = false;
            if (idFascInseriti.Count>0)
            {
                for (int i = 0; i < idFascInseriti.Count; i++)
                {
                    if (idProject.Trim() == ((string)idFascInseriti[i]).Trim())
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        protected void inserisciFascicoloInConservazione(string idProject)
        {
            string[] listaDoc;
            listaDoc = FascicoliManager.getIdDocumentiFromFascicolo(idProject);
            for (int i = 0; i < listaDoc.Length; i++)
            {
                string errorMessage = string.Empty;
                DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = new DocsPAWA.DocsPaWR.SchedaDocumento();
                schedaDoc = DocumentManager.getDettaglioDocumentoPerRiabilitazioneIstanza(this,listaDoc[i].ToString(),"", out errorMessage);
                if (schedaDoc != null)
                {
                    if (schedaDoc.inCestino == null)
                        schedaDoc.inCestino = string.Empty;
                    if (schedaDoc.inCestino.Trim() != "1")
                    {
                        string sysId = DocumentManager.addAreaConservazione(Page, schedaDoc.systemId, idProject, schedaDoc.docNumber, (DocsPAWA.DocsPaWR.InfoUtente)Session["infoUtente"], "F");
                        int size_xml = DocumentManager.getItemSize(Page, schedaDoc, sysId);
                        int doc_size = Convert.ToInt32(schedaDoc.documenti[0].fileSize);
                        int size_allegati = 0;
                        for (int j = 0; j < schedaDoc.allegati.Length; j++)
                        {
                            size_allegati = size_allegati + Convert.ToInt32(schedaDoc.allegati[j].fileSize);
                        }
                        int total_size = size_allegati + doc_size + size_xml;

                        int numeroAllegati = schedaDoc.allegati.Length;
                        string fileName = schedaDoc.documenti[0].fileName;
                        string tipoFile = System.IO.Path.GetExtension(fileName);

                        DocumentManager.insertSizeInItemCons(Page, sysId, total_size);

                        DocumentManager.updateItemsConservazione(Page, tipoFile, Convert.ToString(numeroAllegati), sysId);
                    }
                }
                if (errorMessage != string.Empty)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + errorMessage + "');", true);
                }
                    //Response.Write("<script>alert('" + errorMessage + "')</script>");
                
            }
        }
        protected void inserisciDocumentoInConservazione(string idProfile, string docNumber, string idProject)
        {
            string errorMessage = string.Empty;
            DocsPAWA.DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDettaglioDocumentoPerRiabilitazioneIstanza(this, idProfile, docNumber, out errorMessage);// .getDettaglioDocumento(this, idProfile, docNumber);
            if (schedaDoc != null)
            {
                if (schedaDoc.inCestino == null)
                    schedaDoc.inCestino = string.Empty;
                if (schedaDoc.inCestino.Trim() != "1")
                {
                    string sysId = DocumentManager.addAreaConservazione(this, idProfile, idProject, docNumber, (DocsPAWA.DocsPaWR.InfoUtente)Session["infoUtente"], "D");

                    if (sysId != "-1")
                    {
                        int size_xml = DocumentManager.getItemSize(Page, schedaDoc, sysId);
                        int doc_size = Convert.ToInt32(schedaDoc.documenti[0].fileSize);

                        int numeroAllegati = schedaDoc.allegati.Length;
                        string fileName = schedaDoc.documenti[0].fileName;
                        string tipoFile = System.IO.Path.GetExtension(fileName);

                        int size_allegati = 0;
                        for (int i = 0; i < schedaDoc.allegati.Length; i++)
                        {
                            size_allegati = size_allegati + Convert.ToInt32(schedaDoc.allegati[i].fileSize);
                        }
                        int total_size = size_allegati + doc_size + size_xml;

                        DocumentManager.insertSizeInItemCons(Page, sysId, total_size);

                        DocumentManager.updateItemsConservazione(Page, tipoFile, Convert.ToString(numeroAllegati), sysId);
                    }
                }
            }
            if (errorMessage != string.Empty)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('" + errorMessage + "');", true);
               // Response.Write("<script>alert('" + errorMessage + "')</script>");
            }
               
        }

        protected void CaricaDropDownListPolicy()
        {
            this._wsInstance.Timeout = System.Threading.Timeout.Infinite;
            Policy[] policyListaDocumenti = this._wsInstance.GetListaPolicy(Int32.Parse(UserManager.getInfoUtente().idAmministrazione), "D");
            Policy[] policyListaFascicoli = this._wsInstance.GetListaPolicy(Int32.Parse(UserManager.getInfoUtente().idAmministrazione), "F");
            Policy[] policyListaStampe = this._wsInstance.GetListaPolicy(Int32.Parse(UserManager.getInfoUtente().idAmministrazione), "R");
            Policy[] policyListaRepertori = this._wsInstance.GetListaPolicy(Int32.Parse(UserManager.getInfoUtente().idAmministrazione), "C");

            ddl_policy.Items.Clear();
            ddl_policy.Items.Add("");
            int y = 0;
            if (policyListaDocumenti != null)
            {
                for (int i = 0; i < policyListaDocumenti.Length; i++,y++)
                {
                    ddl_policy.Items.Add("[D] " + policyListaDocumenti[i].nome);
                    ddl_policy.Items[i + 1].Value = policyListaDocumenti[i].system_id;
                }
            }
            if (policyListaFascicoli != null)
            {
                for (int i = 0; i < policyListaFascicoli.Length; i++,y++)
                {
                    ddl_policy.Items.Add("[F] " + policyListaFascicoli[i].nome);
                    ddl_policy.Items[y + 1].Value = policyListaFascicoli[i].system_id;
                }
            }
            if (policyListaStampe != null)
            {
                for (int i = 0; i < policyListaStampe.Length; i++, y++)
                {
                    ddl_policy.Items.Add("[S] " + policyListaStampe[i].nome);
                    ddl_policy.Items[y + 1].Value = policyListaStampe[i].system_id;
                }
            }
            if (policyListaRepertori != null)
            {
                for (int i = 0; i < policyListaRepertori.Length; i++, y++)
                {
                    ddl_policy.Items.Add("[R] " + policyListaRepertori[i].nome);
                    ddl_policy.Items[y + 1].Value = policyListaRepertori[i].system_id;
                }
            }
        }

        protected void ChangePolicy(object sender, EventArgs e)
        {
            _wsInstance.Timeout = System.Threading.Timeout.Infinite;
            string idPolicy = ddl_policy.SelectedValue;
            string policy_validazione = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("idpolicyvalidazione")).Text;

            if (!string.IsNullOrEmpty(this.ddl_policy.SelectedValue))
            {
                bool result = _wsInstance.ValidateIstanzaConservazioneConPolicy(this.ddl_policy.SelectedValue, this.idIstanza.Value, UserManager.getInfoUtente());
                if (result)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Istanza verificata');", true);
                }
                this.btn_elimina_policy.Enabled = true;
            }
            else
            {
                bool result = _wsInstance.DeleteValidateIstanzaConservazioneConPolicy(this.ddl_policy.SelectedValue, this.idIstanza.Value, UserManager.getInfoUtente());
                if (result)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Verifica rimossa su istanza');", true);
                }
                this.btn_elimina_policy.Enabled = false;
            }

            string filtro = this.Filtra();
            infoCons = DocumentManager.getListaConservazioneByFiltro(filtro, this);
            //infoCons = DocumentManager.getListaConservazione(idPeo.Value, idRuoloUo.Value, Page);
            this.caricaGridviewIstanze(infoCons, this.gv_istanzeCons);
            itemsCons = DocumentManager.getItemsConservazioneLite(this.idIstanza.Value, Page, UserManager.getInfoUtente());
            if (itemsCons != null)
            {
                this.caricaGridviewDettaglio(itemsCons, this.gv_dettaglioCons);
            }

            this.upIstanze.Update();
            this.upDettaglio.Update();
            this.upValidate.Update();
        }

        protected void SelectTipoConservazione(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ddl_tipoCons.SelectedValue))
            {
                if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_CONSOLIDATA")
                {
                    this.chk_consolida.Checked = true;
                    this.chk_consolida.Enabled = false;
                }
                if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_NON_CONSOLIDATA")
                {
                    this.chk_consolida.Checked = false;
                    this.chk_consolida.Enabled = false;
                }
                if (this.ddl_tipoCons.SelectedValue == "ESIBIZIONE")
                {
                    this.chk_consolida.Checked = false;
                    this.chk_consolida.Enabled = false;
                }
                if (this.ddl_tipoCons.SelectedValue == "CONSERVAZIONE_INTERNA")
                {
                    this.chk_consolida.Checked = false;
                    this.chk_consolida.Enabled = true;
                }
            } 
            this.upTipologie.Update();
        }

        protected void btn_predefinita_Click(object sender, EventArgs e)
        {
            if (DocumentManager.UpdatePreferredInstance(this.Page, this.idIstanza.Value, UserManager.getInfoUtente(),UserManager.getRuolo(this)))
            {
                string filtro = this.Filtra();
                infoCons = DocumentManager.getListaConservazioneByFiltro(filtro, this);
                this.caricaGridviewIstanze(infoCons, this.gv_istanzeCons);

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Istanza impostata come predefinita');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Errore nella modifica dell istanza predefinita');", true);
            }
        }

        protected void abilitaCestino()
        {
            for (int i = 0; i < this.gv_dettaglioCons.Rows.Count; i++)
            {
                (this.gv_dettaglioCons.Rows[i].Cells[8]).Enabled = false;
            }
        }

        protected void btn_elimina_policy_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ddl_policy.SelectedValue))
            {
                string policy_validazione = ((Label)this.gv_istanzeCons.SelectedRow.FindControl("idpolicyvalidazione")).Text;
                bool result = _wsInstance.ValidateIstanzaConservazioneConPolicy(this.ddl_policy.SelectedValue, this.idIstanza.Value, UserManager.getInfoUtente());

                result = _wsInstance.EliminaDocumentiNonConformiPolicyDaIstanza(this.ddl_policy.SelectedValue, this.idIstanza.Value, UserManager.getInfoUtente());
                if (result)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Eliminati i documenti non conformi alla Policy');", true);

                    itemsCons = DocumentManager.getItemsConservazioneLite(this.idIstanza.Value, Page,UserManager.getInfoUtente());
                    if (itemsCons != null && itemsCons.Length > 0)
                    {
                        this.caricaGridviewDettaglio(itemsCons, this.gv_dettaglioCons);
                        this.upDettaglio.Update();
                    }
                    else
                    {
                        this.panelDettaglioIstanza.Visible = false;
                        this.upIstanze.Update();
                        this.upDettaglio.Update();
                    }
                }               
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert", "alert('Seleziona una Policy');", true);
            }
        }

    }
}
