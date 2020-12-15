using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NttDataWA.UIManager;
using NttDataWA.Utils;

namespace NttDataWA.UserControls
{
    public partial class Correspondent : System.Web.UI.UserControl
    {
        #region ICtrlListaDestinatari Members
        protected System.Web.UI.WebControls.DropDownList ddl_caselle_corr_est;
        protected string currentLanguage;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spedizione"></param>
        /// <param name="tipoDestinatario"></param>
        public void FetchData(NttDataWA.DocsPaWR.SpedizioneDocumento spedizione, CorrespondentTypeEnum tipoDestinatario, NttDataWA.DocsPaWR.Registro[] rf, NttDataWA.DocsPaWR.Registro[] registri,string tipoDest, NttDataWA.DocsPaWR.StatoInvio[] listaSped_opt)
        {
            this.Spedizione = spedizione;
            this.TipoDestinatario = tipoDestinatario;
            //DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            this.ListaSpedizioni = listaSped_opt;
            this.DataGrid.DataSource = this.GetDestinatari(spedizione);
            this.DataGrid.DataBind();

            if (DataGrid.Rows.Count > 0)
                this.reloadContentText(spedizione,rf,registri,tipoDest);
        }

        public void setLanguage(string language)
        {
            this.currentLanguage = language;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spedizione"></param>
        public void SaveData(NttDataWA.DocsPaWR.SpedizioneDocumento spedizione)
        {

            foreach (GridViewRow item in this.DataGrid.Rows)
            {
                CheckBox chkIncludiInSpedizione = this.GetCheckBoxIncludiInSpedizione(item);
                if (chkIncludiInSpedizione != null)
                {
                    DocsPaWR.Destinatario destinatario = this.GetDestinatario(spedizione, this.GetIdDestinatario(item));
                    if (destinatario != null)
                        destinatario.IncludiInSpedizione = chkIncludiInSpedizione.Checked;

                    bool go = true;
                    for (int i = 0; i < spedizione.DestinatariEsterni.Length; i++)
                    {
                        foreach (DocsPaWR.Corrispondente corr in spedizione.DestinatariEsterni[i].DatiDestinatari)
                        {
                            if (corr.systemId.Equals(destinatario.Id))
                            {
                                spedizione.DestinatariEsterni[i].Email = (item.FindControl("ddl_caselle_corr_est") as DropDownList).SelectedValue.Trim();
                                go = false;
                                break;
                            }
                        }
                        if (!go)
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Impostazione dello stato di spedizione
        /// </summary>
        /// <param name="spedito"></param>
        public virtual void SetStatoSpedizione(bool spedito)
        {
            foreach (GridViewRow item in this.DataGrid.Rows)
            {
                CheckBox chkIncludiInSpedizione = this.GetCheckBoxIncludiInSpedizione(item);
                if (chkIncludiInSpedizione.Visible && !spedito)
                {
                    // Se il documento non risulta ancora spedito almeno una volta,
                    // tutti i checkbox di "Includi in spedizione" vengono disabilitati 
                    // marcati come da spedire
                    chkIncludiInSpedizione.Checked = true;
                    //chkIncludiInSpedizione.Enabled = false;
                    chkIncludiInSpedizione.Enabled = true;
                }
            }
        }

        public virtual void DisableStatoSpedizione()
        {
            foreach (GridViewRow item in this.DataGrid.Rows)
            {
                CheckBox chkIncludiInSpedizione = this.GetCheckBoxIncludiInSpedizione(item);
                if (chkIncludiInSpedizione.Visible)
                {
                    chkIncludiInSpedizione.Checked = false;
                    chkIncludiInSpedizione.Enabled = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Items
        {
            get
            {
                return this.DataGrid.Rows.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AlmostOneChecked
        {
            get
            {
                foreach (GridViewRow item in this.DataGrid.Rows)
                {
                    CheckBox chkIncludiInSpedizione = this.GetCheckBoxIncludiInSpedizione(item);

                    if (chkIncludiInSpedizione != null && chkIncludiInSpedizione.Checked)
                        return true;
                }

                return false;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        private void SetControlsVisibility()
        {
            this.DataGrid.Visible = (this.Items > 0);
        }

        #endregion


        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        protected NttDataWA.DocsPaWR.SpedizioneDocumento Spedizione
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la tipologia di utenti da visualizzare.
        /// Se true, lavora con i destinatari interni di un protocollo.
        /// </summary>
        protected CorrespondentTypeEnum TipoDestinatario
        {
            get
            {
                if (this.ViewState["TipoDestinatario"] != null)
                    return (CorrespondentTypeEnum)this.ViewState["TipoDestinatario"];
                else
                    return CorrespondentTypeEnum.Interno;
            }
            private set
            {
                this.ViewState["TipoDestinatario"] = value;
            }
        }

        //Contiene i destinatari a cui non è stato possibile fare la spedizione
        private string DestinatariNonRaggiunti
        {
            get
            {
                if ((HttpContext.Current.Session["DestinatariNonRaggiunti"]) != null)
                    return HttpContext.Current.Session["DestinatariNonRaggiunti"].ToString();
                else
                    return string.Empty;
            }
            set
            {
                HttpContext.Current.Session["DestinatariNonRaggiunti"] = value;
            }
        }

        protected NttDataWA.DocsPaWR.StatoInvio[] ListaSpedizioni
        {
            get
            {
                if (this.ViewState["listaSpedizione_opt"] != null)
                    return (NttDataWA.DocsPaWR.StatoInvio[])this.ViewState["listaSpedizione_opt"];
                else
                {
                    DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                    return ws.GetListaSpedizioni(DocumentManager.getSelectedRecord().systemId);
                }
            }
            private set
            {
                this.ViewState["listaSpedizione_opt"] = value;
            }
        }

        

        /// <summary>
        /// Oggetto DataGrid utilizzato per la visualizzazione dei dati dei destinatari
        /// dagli usercontrols concreti
        /// </summary>
        protected virtual GridView DataGrid
        {
            get
            {
                return this.new_grdSpedizioni;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DataGrid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            try {
                DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                ListItemType itemType = (ListItemType)e.Row.RowType;

                if (itemType == ListItemType.Item || itemType == ListItemType.SelectedItem || itemType == ListItemType.AlternatingItem)
                {
                    if (e.Row.DataItem.GetType() != null && e.Row.DataItem.GetType() == typeof(NttDataWA.DocsPaWR.DestinatarioEsterno))
                    {
                        if ((e.Row.DataItem as NttDataWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0] != null)
                        {
                            string lastMailSend = string.Empty;
                            string idCorrLastMailSend = string.Empty;
                            //NttDataWA.DocsPaWR.StatoInvio[] statoInvio = ws.GetListaSpedizioni(DocumentManager.getSelectedRecord().systemId);
                            NttDataWA.DocsPaWR.StatoInvio[] statoInvio = this.ListaSpedizioni;
                            if (statoInvio != null && statoInvio.Length > 0)
                            {
                                if ((from c in (statoInvio as NttDataWA.DocsPaWR.StatoInvio[])
                                     where
                                         c.destinatario.Equals((e.Row.DataItem as NttDataWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].descrizione)
                                     select c.indirizzo).Count() > 0)
                                {
                                    lastMailSend = (from c in (statoInvio as NttDataWA.DocsPaWR.StatoInvio[])
                                                    where
                                                        c.destinatario.Equals((e.Row.DataItem as NttDataWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].descrizione)
                                                    select c.indirizzo).Last();

                                    idCorrLastMailSend = (from c in (statoInvio as NttDataWA.DocsPaWR.StatoInvio[])
                                                          where
                                                              c.destinatario.Equals((e.Row.DataItem as NttDataWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].descrizione)
                                                          select c.idCorrispondente).Last();
                                }
                            }
                            NttDataWA.DocsPaWR.MailCorrispondente[] caselle = (e.Row.DataItem as NttDataWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].Emails;
                            DropDownList ddl = e.Row.FindControl("ddl_caselle_corr_est") as DropDownList;
                            if (ddl != null)
                            {
                                //INC000000777644
                                //Se ho spedito ad un corrispondente e successivamente l'ho modificato, allora non seleziono la mail a cui avevo spedito precedentemente
                                //ma selezione la mail preferita
                                bool isCorrUpdeted = !string.IsNullOrEmpty(lastMailSend) && (from c in caselle where c.Email.Equals(lastMailSend) && c.systemId.Equals(idCorrLastMailSend) select c).FirstOrDefault() == null;
                                foreach (NttDataWA.DocsPaWR.MailCorrispondente c in caselle)
                                {
                                    System.Text.StringBuilder textCasella = new System.Text.StringBuilder();
                                    if (c.Principale.Equals("1"))
                                        textCasella.Append("* ");
                                    textCasella.Append(c.Email);
                                    if (!string.IsNullOrEmpty(c.Note))
                                        textCasella.Append(" - " + c.Note);
                                    ddl.Items.Add(new ListItem
                                    {
                                        Selected = (!string.IsNullOrEmpty(lastMailSend) && lastMailSend.Equals(c.Email) && !isCorrUpdeted) ? true :
                                        (c.Principale.Equals("1") && (string.IsNullOrEmpty(lastMailSend) || isCorrUpdeted)) ? true : false,
                                        Value = c.Email.Trim(),
                                        Text = textCasella.ToString()
                                    });
                                }
                                if ((e.Row.DataItem as NttDataWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].tipoCorrispondente == "O" &&
                                !string.IsNullOrEmpty((e.Row.DataItem as NttDataWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].email))
                                {
                                    ddl.Items.Add(new ListItem
                                    {
                                        Value = (e.Row.DataItem as NttDataWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].email.Trim(),
                                        Text = (e.Row.DataItem as NttDataWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].email.Trim()
                                        
                                    });
                                }
                                
                            }
                            if ((e.Row.DataItem as NttDataWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].tipoIE != null &&
                                (e.Row.DataItem as NttDataWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].tipoIE.Equals("I"))
                            {
                                ddl.Enabled = false;
                                ddl.Visible = false;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spedizione"></param>
        /// <returns></returns>
        protected virtual DocsPaWR.Destinatario[] GetDestinatari(NttDataWA.DocsPaWR.SpedizioneDocumento spedizione)
        {
            if (this.TipoDestinatario == CorrespondentTypeEnum.Interno)
                return spedizione.DestinatariInterni;
            else if (this.TipoDestinatario == CorrespondentTypeEnum.Esterno)
                return (from c in spedizione.DestinatariEsterni
                        where c.Interoperante && (c.DatiDestinatari[0].tipoIE != null && c.DatiDestinatari[0].tipoIE.Equals("I") || 
                        (c.DatiDestinatari[0].canalePref!=null && !c.DatiDestinatari[0].canalePref.typeId.Equals(SimplifiedInteroperabilityManager.SimplifiedInteroperabilityId))||
                        (c.DatiDestinatari[0].tipoCorrispondente=="O" && !string.IsNullOrEmpty(c.DatiDestinatari[0].email)))
                        orderby c.DatiDestinatari[0].descrizione, c.DatiDestinatari[0].codiceRubrica                        
                        select c).ToArray();
            else if (this.TipoDestinatario == CorrespondentTypeEnum.EsternoNonInteroperante)
                return (from c in spedizione.DestinatariEsterni
                        where !c.Interoperante
                        orderby c.DatiDestinatari[0].descrizione, c.DatiDestinatari[0].codiceRubrica 
                        select c).ToArray();
            else if (this.TipoDestinatario == CorrespondentTypeEnum.SimplifiedInteroperability)
                return (from c in spedizione.DestinatariEsterni
                        where c.Interoperante &&
                        c.DatiDestinatari[0].canalePref != null &&
                        c.DatiDestinatari[0].canalePref.typeId.Equals(SimplifiedInteroperabilityManager.SimplifiedInteroperabilityId) &&
                        c.DatiDestinatari[0].Url != null &&
                        c.DatiDestinatari[0].Url.Length > 0 &&
                        !String.IsNullOrEmpty(c.DatiDestinatari[0].Url[0].Url)
                        orderby c.DatiDestinatari[0].descrizione, c.DatiDestinatari[0].codiceRubrica 
                        select c).ToArray();

            return null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void reloadContentText(DocsPaWR.SpedizioneDocumento infoSped, NttDataWA.DocsPaWR.Registro[] rf, NttDataWA.DocsPaWR.Registro[] registri, string tipoDest)
        {
            int i = 0;
            //GridViewRow item = DataGrid.Rows[i];
            //CheckBox chkSelezionaDeseleziona = this.GetChkSelezionaDeseleziona(item);
            //chkSelezionaDeseleziona.Visible = VisibilitySelectDeselectRecipients();
            //i = i + 1;
            CheckBox chkSelezionaDeseleziona = DataGrid.HeaderRow.FindControl("chkSelezionaDeseleziona") as CheckBox;
            chkSelezionaDeseleziona.Visible = VisibilitySelectDeselectRecipients(infoSped);

            foreach (DocsPaWR.Destinatario itemD in this.GetDestinatari(this.Spedizione))
            {
                DocsPaWR.Destinatario destinatario = this.GetDestinatario(this.Spedizione, this.GetIdDestinatario(itemD));

                if (destinatario != null)
                {
                    GridViewRow hederGrid = DataGrid.HeaderRow;
                    CheckBox chkSelDeSel = hederGrid.FindControl("chkSelezionaDeseleziona") as CheckBox;
                    chkSelDeSel.ToolTip = UIManager.LoginManager.GetLabelFromCode("SenderChkSelDeSel_ToolTip", currentLanguage);
                    hederGrid.Cells[1].Text = UIManager.LoginManager.GetLabelFromCode("SenderDataUltimaSpedizione", currentLanguage);
                    hederGrid.Cells[2].Text = (TipoDestinatario != CorrespondentTypeEnum.SimplifiedInteroperability ? UIManager.LoginManager.GetLabelFromCode("SenderEmail", currentLanguage) : " ");
                    hederGrid.Cells[4].Text = UIManager.LoginManager.GetLabelFromCode("SenderDestinatario", currentLanguage);
                    hederGrid.Cells[5].Text = UIManager.LoginManager.GetLabelFromCode("SenderStato", currentLanguage);

                    GridViewRow item = DataGrid.Rows[i];

                    CheckBox chkIncludiInSpedizione = this.GetCheckBoxIncludiInSpedizione(item);
                    Label lblIdDestinatario = this.GetlblIdDestinatario(item);
                    Label lblDestinatario = this.GetlblDestinatario(item);
                    Label lblStatoSpedizione = this.GetlblStatoSpedizione(item);
                    Label lblemail = this.GetlblEmail(item);
                    DropDownList drpdw = this.GetCaselle_corr_est(item);

                    if (chkIncludiInSpedizione != null)
                    {
                        chkIncludiInSpedizione.Checked = IncludiDestinatarioInSpedizione(destinatario,rf,registri);
                        chkIncludiInSpedizione.Visible = IsDestinatarioValidoPerSpedizione(destinatario,rf,registri);
                    }
                    if (lblIdDestinatario != null)
                    {
                        lblIdDestinatario.Text = GetIdDestinatario(destinatario);
                    }
                    if (lblDestinatario != null)
                    {
                        lblDestinatario.Text = GetDescrizioneDestinatario(destinatario);
                    }
                    if (lblStatoSpedizione != null)
                    {
                        lblStatoSpedizione.Text = GetStatoSpedizione(destinatario,tipoDest);
                    }
                    if (drpdw != null)
                    {
                        drpdw.Visible = this.IsVisibleListMail();
                    }
                    if (lblemail != null)
                    {
                        if (this.IsVisibleFieldMail())
                        {
                            lblemail.Text = (destinatario.Email != null ? destinatario.Email.ToString() : "");
                            lblemail.Visible = true;
                        }
                        else
                        {
                            lblemail.Visible = false;
                        }
                    }

                    if (destinatario.IncludiInSpedizione && !lblStatoSpedizione.Text.Equals("Spedito"))
                    {
                        this.DestinatariNonRaggiunti += "<li> " + lblDestinatario.Text + "</li>";
                    }
                    i = (i + 1);
                }
            }
        }
        /// <summary>
        /// Indica se al destinatario può essere inviato il documento
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual bool IsDestinatarioValidoPerSpedizione(DocsPaWR.Destinatario item, NttDataWA.DocsPaWR.Registro[] rf, NttDataWA.DocsPaWR.Registro[] registri)
        {
            bool resultValue = true;
            try
            {

                DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                if (item is DocsPaWR.DestinatarioEsterno && !(item as DocsPaWR.DestinatarioEsterno).Interoperante)
                    resultValue = false;

                if (item is DocsPaWR.DestinatarioEsterno)
                {
                    DocsPaWR.DestinatarioEsterno destinatarioEsterno = item as DocsPaWR.DestinatarioEsterno;
                    if ((destinatarioEsterno.DatiDestinatari[0].tipoIE != null && destinatarioEsterno.DatiDestinatari[0].tipoIE.Equals("E") &&
                        !MultiBoxManager.RoleIsAuthorizedSend("E",rf,registri) &&
                        destinatarioEsterno.DatiDestinatari[0].canalePref != null &&
                        (destinatarioEsterno.DatiDestinatari[0].canalePref.descrizione.Equals("MAIL") ||
                        destinatarioEsterno.DatiDestinatari[0].canalePref.descrizione.Equals("INTEROPERABILITA"))) ||
                        (destinatarioEsterno.DatiDestinatari[0].tipoIE != null && destinatarioEsterno.DatiDestinatari[0].tipoIE.Equals("I") && ws.IsEnabledInteropInterna() &&
                        !MultiBoxManager.RoleIsAuthorizedSend("I",rf,registri)))
                        resultValue = false;
                }
                if (item is DocsPaWR.DestinatarioInterno)
                {
                    DocsPaWR.DestinatarioInterno destinatarioInterno = item as DocsPaWR.DestinatarioInterno;
                    if (destinatarioInterno != null && destinatarioInterno.DisabledTrasm)
                        resultValue = false;
                }
            }
            catch (Exception ex)
            {
                resultValue = false;
            }

            return resultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual bool IsEnabledDestinatarioInSpedizione(DocsPaWR.Destinatario item)
        {
            return !string.IsNullOrEmpty(item.DataUltimaSpedizione);
        }

        /// <summary>
        /// Determina se il corrispondente è tra i destinatari principali del documento
        /// </summary>
        /// <param name="corrispondente"></param>
        /// <returns></returns>
        private bool IsDestinatarioPrincipale(DocsPaWR.Corrispondente corrispondente)
        {
            DocsPaWR.SchedaDocumento documento = DocumentManager.getSelectedRecord();

            DocsPaWR.ProtocolloUscita protocolloUscita = (DocsPaWR.ProtocolloUscita)documento.protocollo;

            return (protocolloUscita.destinatari.Count(e => e.systemId == corrispondente.systemId) > 0);
        }

        /// <summary>
        /// Reperimento descrizione del destinatario
        /// </summary>
        /// <param name="destinatario"></param>
        /// <returns></returns>
        protected string GetDescrizioneDestinatario(DocsPaWR.Destinatario destinatario)
        {
            if (destinatario is DocsPaWR.DestinatarioInterno)
            {
                DocsPaWR.DestinatarioInterno dest = (DocsPaWR.DestinatarioInterno)destinatario;

                string cc = string.Empty;

                if (!IsDestinatarioPrincipale(dest.DatiDestinatario))
                    cc = " - In CC";

                return string.Format("{0} ({1}){2}",
                    dest.DatiDestinatario.codiceRubrica,
                    dest.DatiDestinatario.descrizione,
                    cc);
            }
            else
            {
                string descrizione = string.Empty;

                foreach (DocsPaWR.Corrispondente corr in ((DocsPaWR.DestinatarioEsterno)destinatario).DatiDestinatari)
                {
                    if (!string.IsNullOrEmpty(descrizione))
                        descrizione = string.Format("{0}<BR />", descrizione);

                    string cc = string.Empty;

                    if (!IsDestinatarioPrincipale(corr))
                        cc = " - In CC";

                    if (corr.canalePref != null && corr.canalePref.descrizione.ToUpper().Equals("PORTALE"))
                    {
                        descrizione = string.Format("{0}{1} ({2}) - PORTALE{3}",
                                            descrizione,
                                            corr.codiceRubrica,
                                            corr.descrizione,
                                            cc);
                    }
                    else
                    {

                        descrizione = string.Format("{0}{1} ({2}){3}",
                                            descrizione,
                                            corr.codiceRubrica,
                                            corr.descrizione,
                                            cc);
                    }
                }

                return descrizione;
            }
        }


        /// <summary>
        /// Reperimento dello stato della spedizione al destinatario
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual string GetStatoSpedizione(DocsPaWR.Destinatario item, string tipoDest)
        {
            string result = item.StatoSpedizione.Descrizione;
            DocsPaWR.SchedaDocumento scheda = DocumentManager.getSelectedRecord();
            if (!string.IsNullOrEmpty(scheda.eredita) && scheda.eredita.Equals("0") && !string.IsNullOrEmpty(tipoDest) && tipoDest == "INTERNO")
                result += "\n(Estensione gerarchica della visibiltà bloccata dall'utente)";

            return result;

        }

        /// <summary>
        /// Reperimento controllo checkbox per selezione del destinatario cui inviare il documento
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected CheckBox GetCheckBoxIncludiInSpedizione(GridViewRow item)
        {
            return item.FindControl("chkIncludiInSpedizione") as CheckBox;
        }

        /// <summary>
        /// Reperimento controllo checkbox per selezione del destinatario cui inviare il documento
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected CheckBox GetChkSelezionaDeseleziona(GridViewRow item)
        {
            return item.FindControl("chkSelezionaDeseleziona") as CheckBox;
        }

        /// <summary>
        /// Reperimento controllo checkbox per selezione del destinatario cui inviare il documento
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected Label GetlblIdDestinatario(GridViewRow item)
        {
            return item.FindControl("lblIdDestinatario") as Label;
        }

        /// <summary>
        /// Reperimento controllo checkbox per selezione del destinatario cui inviare il documento
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected Label GetlblStatoSpedizione(GridViewRow item)
        {
            return item.FindControl("lblStatoSpedizione") as Label;
        }

        protected Label GetlblEmail(GridViewRow item)
        {
            return item.FindControl("email") as Label;
        }

        protected Label GetlblDestinatario(GridViewRow item)
        {
            return item.FindControl("lblDestinatario") as Label;
        }
        protected DropDownList GetCaselle_corr_est(GridViewRow item)
        {
            return item.FindControl("ddl_caselle_corr_est") as DropDownList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void chkSelezionaDeseleziona_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.OnCheckSelezionaDeseleziona(((CheckBox)sender).Checked);
            }
            catch (System.Exception ex)
            {
                UIManager.AdministrationManager.DiagnosticError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isChecked"></param>
        protected virtual void OnCheckSelezionaDeseleziona(bool isChecked)
        {
            foreach (GridViewRow item in this.DataGrid.Rows)
            {
                CheckBox chkIncludiInSpedizione = this.GetCheckBoxIncludiInSpedizione(item);
                if (chkIncludiInSpedizione != null && chkIncludiInSpedizione.Enabled)
                    chkIncludiInSpedizione.Checked = isChecked;
            }
        }

        /// <summary>
        /// Reperimento del destinatario a partire dall'id
        /// </summary>
        /// <param name="spedizione"></param>
        /// <param name="idDestinatario"></param>
        /// <returns></returns>
        protected DocsPaWR.Destinatario GetDestinatario(DocsPaWR.SpedizioneDocumento spedizione, string idDestinatario)
        {
            foreach (DocsPaWR.Destinatario item in this.GetDestinatari(spedizione))
                if (item.Id == idDestinatario)
                    return item;
            return null;
        }

        /// <summary>
        /// Reperimento dell'id del destinatario
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected string GetIdDestinatario(GridViewRow item)
        {
            Label lblIdDestinatario = item.FindControl("lblIdDestinatario") as Label;

            if (lblIdDestinatario != null)
                return lblIdDestinatario.Text;
            else
                return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected bool IncludiDestinatarioInSpedizione(DocsPaWR.Destinatario item, NttDataWA.DocsPaWR.Registro[] rf, NttDataWA.DocsPaWR.Registro[] registri)
        {
            bool resultValue = true;
            try
            {
                resultValue = item.IncludiInSpedizione;

                DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                if (item is DocsPaWR.DestinatarioEsterno && !(item as DocsPaWR.DestinatarioEsterno).Interoperante)
                    resultValue = false;

                if (item is DocsPaWR.DestinatarioEsterno)
                {
                    DocsPaWR.DestinatarioEsterno destinatarioEsterno = item as DocsPaWR.DestinatarioEsterno;
                    if ((!MultiBoxManager.RoleIsAuthorizedSend("E",rf,registri) &&
                        destinatarioEsterno.DatiDestinatari[0].canalePref != null &&
                        (destinatarioEsterno.DatiDestinatari[0].canalePref.descrizione.Equals("MAIL") ||
                        destinatarioEsterno.DatiDestinatari[0].canalePref.descrizione.Equals("INTEROPERABILITA"))) ||
                        (destinatarioEsterno.DatiDestinatari[0].tipoIE != null && destinatarioEsterno.DatiDestinatari[0].tipoIE.Equals("I") && ws.IsEnabledInteropInterna() &&
                        !MultiBoxManager.RoleIsAuthorizedSend("I",rf,registri)))
                        resultValue = false;
                }
                if (item is DocsPaWR.DestinatarioInterno)
                {
                    DocsPaWR.DestinatarioInterno destinatarioInterno = item as DocsPaWR.DestinatarioInterno;
                    if (destinatarioInterno != null && destinatarioInterno.DisabledTrasm)
                        resultValue = false;
                }
            }
            catch (Exception ex)
            {
                resultValue = false;
            }

            return resultValue;
        }

        /// <summary>
        /// Reperimento id del destinatario della spedizione
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected string GetIdDestinatario(DocsPaWR.Destinatario item)
        {
            return item.Id;
        }

        /// <summary>
        /// True se non si tratta di destinatari esterni interoperanti.
        /// Nel caso di interoperanti esterni sarà visualizzata la drop down list per il multicasella 
        /// </summary>
        /// <returns></returns>
        protected bool IsVisibleFieldMail()
        {
            bool isVisible = true;
            if ((TipoDestinatario != CorrespondentTypeEnum.Esterno) && (TipoDestinatario != CorrespondentTypeEnum.SimplifiedInteroperability))
                return isVisible;
            else
                return !isVisible;
        }

        /// <summary>
        /// True se si tratta di destinatari esterni interoperanti
        /// </summary>
        /// <returns></returns>
        protected bool IsVisibleListMail()
        {
            bool isVisible = true;
            if ((TipoDestinatario != CorrespondentTypeEnum.Esterno))
                return !isVisible;
            else
                return isVisible;
        }

        /// <summary>
        /// Imposta la visibilità sul flag di selezione/deselezione dei destinatari 
        /// </summary>
        protected bool VisibilitySelectDeselectRecipients(DocsPaWR.SpedizioneDocumento infoSped)
        {
            bool select = true;
            try
            {
                DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
                DocsPaWR.SpedizioneDocumento infoSpedizione = new DocsPaWR.SpedizioneDocumento();

                if (infoSped == null)
                    infoSpedizione = SenderManager.GetSpedizioneDocumento(DocumentManager.getSelectedRecord());
                else
                    infoSpedizione = infoSped;   

                if (this.TipoDestinatario == NttDataWA.Utils.CorrespondentTypeEnum.EsternoNonInteroperante)
                    select = false;

                if (this.TipoDestinatario == NttDataWA.Utils.CorrespondentTypeEnum.Esterno &&
                    ((infoSpedizione.DestinatariEsterni.Count(d => d.DatiDestinatari[0].tipoIE != null && d.DatiDestinatari[0].tipoIE.Equals("I")) > 0 &&
                        (infoSpedizione.DestinatariEsterni.Count(d => d.DatiDestinatari[0].tipoIE != null && d.DatiDestinatari[0].tipoIE.Equals("E")) > 0))))
                    select = false;
            }
            catch (Exception ex)
            {
                select = false;
            }
            return select;
        }
        #endregion

    }
}