using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using DocsPAWA.DocsPaWR;

namespace DocsPAWA.Spedizione
{
    public partial class Destinatari : UserControl, ICtrlListaDestinatari
    {
        #region ICtrlListaDestinatari Members
        protected System.Web.UI.WebControls.DropDownList ddl_caselle_corr_est;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spedizione"></param>
        /// <param name="tipoDestinatario"></param>
        public void FetchData(DocsPAWA.DocsPaWR.SpedizioneDocumento spedizione, TipoDestinatarioEnum tipoDestinatario)
        {
            this.Spedizione = spedizione;
            this.TipoDestinatario = tipoDestinatario;
            this.DataGrid.DataSource = this.GetDestinatari(spedizione);
            this.DataGrid.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spedizione"></param>
        public void SaveData(DocsPAWA.DocsPaWR.SpedizioneDocumento spedizione)
        {
            foreach (DataGridItem item in this.DataGrid.Items)
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
            foreach (DataGridItem item in this.DataGrid.Items)
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

        /// <summary>
        /// 
        /// </summary>
        public int Items
        {
            get
            {
                return this.DataGrid.Items.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AlmostOneChecked 
        {
            get
            {
                foreach (DataGridItem item in this.DataGrid.Items)
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

        /// <summary>
        /// 
        /// </summary>
        private void SetTitle()
        {
            if (this.TipoDestinatario == TipoDestinatarioEnum.Interno)
                this.lblTitle.Text = string.Format("Destinatari interni: {0}", this.DataGrid.Items.Count);
            else if (this.TipoDestinatario == TipoDestinatarioEnum.Esterno && !((new DocsPaWR.DocsPaWebService()).IsEnabledInteropInterna()))
                this.lblTitle.Text = string.Format("Destinatari interoperanti PEC: {0}", this.DataGrid.Items.Count);
            else if(this.TipoDestinatario == TipoDestinatarioEnum.Esterno && (new DocsPaWR.DocsPaWebService()).IsEnabledInteropInterna())
                this.lblTitle.Text = string.Format("Destinatari interoperanti: {0}", this.DataGrid.Items.Count);
            else if (this.TipoDestinatario == TipoDestinatarioEnum.EsternoNonInteroperante)
                this.lblTitle.Text = string.Format("Destinatari esterni non interoperanti: {0}", this.DataGrid.Items.Count);
            else if (this.TipoDestinatario == TipoDestinatarioEnum.SimplifiedInteroperability)
                this.lblTitle.Text = String.Format("Destinatari interoperanti PITRE: {0}", this.DataGrid.Items.Count);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        protected DocsPAWA.DocsPaWR.SpedizioneDocumento Spedizione
        {
            get;
            set;
        }

        /// <summary>
        /// Indica la tipologia di utenti da visualizzare.
        /// Se true, lavora con i destinatari interni di un protocollo.
        /// </summary>
        protected TipoDestinatarioEnum TipoDestinatario
        {
            get
            { 
                if (this.ViewState["TipoDestinatario"] != null)
                    return (TipoDestinatarioEnum)this.ViewState["TipoDestinatario"];
                else
                    return TipoDestinatarioEnum.Interno;
            }
            private set
            {
                this.ViewState["TipoDestinatario"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            this.SetControlsVisibility();
            this.SetTitle();

        }
        
        /// <summary>
        /// Oggetto DataGrid utilizzato per la visualizzazione dei dati dei destinatari
        /// dagli usercontrols concreti
        /// </summary>
        protected virtual DataGrid DataGrid 
        {
            get
            {
                return this.grdSpedizioni;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.SelectedItem || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.Item.DataItem.GetType() != null && e.Item.DataItem.GetType()  == typeof(DocsPAWA.DocsPaWR.DestinatarioEsterno))
                {
                    if ((e.Item.DataItem as DocsPAWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0] != null)
                    {
                        string lastMailSend = string.Empty;
                        StatoInvio[] statoInvio = ws.GetListaSpedizioni(DocumentManager.getDocumentoSelezionato().systemId);
                        if (statoInvio != null && statoInvio.Length > 0)
                        {
                            if ((from c in (statoInvio as StatoInvio[])
                                 where
                                     c.destinatario.Equals((e.Item.DataItem as DocsPAWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].descrizione)
                                 select c.indirizzo).Count() > 0)
                                lastMailSend = (from c in (statoInvio as StatoInvio[])
                                                where
                                                    c.destinatario.Equals((e.Item.DataItem as DocsPAWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].descrizione)
                                                select c.indirizzo).First();
                        }
                        DocsPAWA.DocsPaWR.MailCorrispondente[] caselle = (e.Item.DataItem as DocsPAWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].Emails;
                        
                        //se attivo il monocasella devo considerare solo la mail principale
                        if (!DocsPAWA.utils.MultiCasellaManager.IsEnabledMultiMail(UserManager.getRuolo().idAmministrazione))   
                        {
                            if (caselle != null && caselle.Count() > 0)
                            {
                                MailCorrispondente corrPrincipale = (from c in caselle where c.Principale.Equals("1") select c).FirstOrDefault();
                                if (corrPrincipale != null)
                                {
                                    caselle = new DocsPAWA.DocsPaWR.MailCorrispondente[1];
                                    caselle[0] = corrPrincipale;
                                }
                            }
                        }
                        DropDownList ddl = e.Item.FindControl("ddl_caselle_corr_est") as DropDownList;
                        if (ddl != null)
                        {
                            foreach (DocsPAWA.DocsPaWR.MailCorrispondente c in caselle)
                            {
                                System.Text.StringBuilder textCasella = new System.Text.StringBuilder();
                                if (c.Principale.Equals("1"))
                                    textCasella.Append("* ");
                                textCasella.Append(c.Email);
                                if (!string.IsNullOrEmpty(c.Note))
                                    textCasella.Append(" - " + c.Note);
                                ddl.Items.Add(new ListItem
                                {
                                    Selected = (!string.IsNullOrEmpty(lastMailSend) && lastMailSend.Equals(c.Email)) ? true : 
                                    (c.Principale.Equals("1") && string.IsNullOrEmpty(lastMailSend)) ? true : false,
                                    Value = c.Email.Trim(),
                                    Text = textCasella.ToString()
                                });
                            }
                            if ((e.Item.DataItem as DocsPAWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].tipoCorrispondente == "O" &&
                                !string.IsNullOrEmpty((e.Item.DataItem as DocsPAWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].email))
                            {
                                ddl.Items.Add(new ListItem
                                {
                                    Value = (e.Item.DataItem as DocsPAWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].email.Trim(),
                                    Text = (e.Item.DataItem as DocsPAWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].email.Trim()

                                });
                            }
                        }
                        if ((e.Item.DataItem as DocsPAWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].tipoIE != null &&
                            (e.Item.DataItem as DocsPAWA.DocsPaWR.DestinatarioEsterno).DatiDestinatari[0].tipoIE.Equals("I"))
                        {
                            ddl.Enabled = false;
                            ddl.Visible = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spedizione"></param>
        /// <returns></returns>
        protected virtual DocsPaWR.Destinatario[] GetDestinatari(DocsPAWA.DocsPaWR.SpedizioneDocumento spedizione)
        {
            if (this.TipoDestinatario == TipoDestinatarioEnum.Interno)
                return spedizione.DestinatariInterni;
            else if (this.TipoDestinatario == TipoDestinatarioEnum.Esterno)
                return (from c in spedizione.DestinatariEsterni
                        where c.Interoperante && (c.DatiDestinatari[0].tipoIE != null && c.DatiDestinatari[0].tipoIE.Equals("I") ||
                        (c.DatiDestinatari[0].canalePref != null && !c.DatiDestinatari[0].canalePref.descrizione.Equals("Interoperabilità PITRE")) ||
                        (c.DatiDestinatari[0].tipoCorrispondente == "O" && !string.IsNullOrEmpty(c.DatiDestinatari[0].email)))
                        select c).ToArray();
            else if (this.TipoDestinatario == TipoDestinatarioEnum.EsternoNonInteroperante)
                return (from c in spedizione.DestinatariEsterni
                        where !c.Interoperante
                        select c).ToArray();
            else if (this.TipoDestinatario == TipoDestinatarioEnum.SimplifiedInteroperability)
                return (from c in spedizione.DestinatariEsterni
                        where c.Interoperante &&
                        c.DatiDestinatari[0].canalePref != null && 
                        c.DatiDestinatari[0].canalePref.descrizione.Equals("Interoperabilità PITRE") && 
                        c.DatiDestinatari[0].Url != null && 
                        c.DatiDestinatari[0].Url.Length > 0 && 
                        !String.IsNullOrEmpty(c.DatiDestinatari[0].Url[0].Url) 
                        select c).ToArray();

                return null;
        }

        /// <summary>
        /// Indica se al destinatario può essere inviato il documento
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual bool IsDestinatarioValidoPerSpedizione(DocsPaWR.Destinatario item)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            if (item is DocsPaWR.DestinatarioEsterno && !(item as DocsPaWR.DestinatarioEsterno).Interoperante)
                return false;

            if (item is DocsPaWR.DestinatarioEsterno)
            {
                DocsPaWR.DestinatarioEsterno destinatarioEsterno = item as DocsPaWR.DestinatarioEsterno;
                if ((destinatarioEsterno.DatiDestinatari[0].tipoIE != null && destinatarioEsterno.DatiDestinatari[0].tipoIE.Equals("E") &&
                    !DocsPAWA.utils.MultiCasellaManager.RoleIsAuthorizedSend(this.Page, "E") &&
                    destinatarioEsterno.DatiDestinatari[0].canalePref != null &&
                    (destinatarioEsterno.DatiDestinatari[0].canalePref.descrizione.Equals("MAIL") ||
                    destinatarioEsterno.DatiDestinatari[0].canalePref.descrizione.Equals("INTEROPERABILITA"))) ||
                    (destinatarioEsterno.DatiDestinatari[0].tipoIE != null && destinatarioEsterno.DatiDestinatari[0].tipoIE.Equals("I") && ws.IsEnabledInteropInterna() &&
                    !DocsPAWA.utils.MultiCasellaManager.RoleIsAuthorizedSend(this.Page, "I")))
                    return false;
            }
            if (item is DocsPaWR.DestinatarioInterno)
            {
                DocsPaWR.DestinatarioInterno destinatarioInterno = item as DocsPaWR.DestinatarioInterno;
                if (destinatarioInterno != null && destinatarioInterno.DisabledTrasm)
                    return false;
            }
            return true;
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
            DocsPaWR.SchedaDocumento documento = DocumentManager.getDocumentoSelezionato();

            DocsPaWR.ProtocolloUscita protocolloUscita = (DocsPaWR.ProtocolloUscita) documento.protocollo;

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
                    dest.DatiDestinatario.descrizione,
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

                    descrizione = string.Format("{0}{1} ({2}){3}",
                                        descrizione,
                                        corr.codiceRubrica,
                                        corr.descrizione,
                                        cc);
                }

                return descrizione;
            }
        }


        /// <summary>
        /// Reperimento dello stato della spedizione al destinatario
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual string GetStatoSpedizione(DocsPaWR.Destinatario item)
        {
            string result = item.StatoSpedizione.Descrizione;
            DocsPaWR.SchedaDocumento scheda = DocumentManager.getDocumentoSelezionato();
            if (!string.IsNullOrEmpty(scheda.eredita) && scheda.eredita.Equals("0"))
                result += "\n(Estensione gerarchica della visibiltà bloccata dall'utente)";
            
            return result;

        }

        /// <summary>
        /// Reperimento controllo checkbox per selezione del destinatario cui inviare il documento
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected CheckBox GetCheckBoxIncludiInSpedizione(DataGridItem item)
        {
            return item.FindControl("chkIncludiInSpedizione") as CheckBox;
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
            catch (Exception ex)
            {
                ErrorManager.OpenErrorPage(this.Page, ex, "chkSelezionaDeseleziona_CheckedChanged");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isChecked"></param>
        protected virtual void OnCheckSelezionaDeseleziona(bool isChecked)
        {
            foreach (DataGridItem item in this.DataGrid.Items)
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
        protected string GetIdDestinatario(DataGridItem item)
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
        protected bool IncludiDestinatarioInSpedizione(DocsPaWR.Destinatario item)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            if (item is DocsPaWR.DestinatarioEsterno && !(item as DocsPaWR.DestinatarioEsterno).Interoperante)
                return false;

            if (item is DocsPaWR.DestinatarioEsterno)            
            {
                DocsPaWR.DestinatarioEsterno destinatarioEsterno = item as DocsPaWR.DestinatarioEsterno;
                if ((!DocsPAWA.utils.MultiCasellaManager.RoleIsAuthorizedSend(this.Page, "E") &&
                    destinatarioEsterno.DatiDestinatari[0].canalePref != null &&
                    (destinatarioEsterno.DatiDestinatari[0].canalePref.descrizione.Equals("MAIL") ||
                    destinatarioEsterno.DatiDestinatari[0].canalePref.descrizione.Equals("INTEROPERABILITA"))) ||
                    (destinatarioEsterno.DatiDestinatari[0].tipoIE != null && destinatarioEsterno.DatiDestinatari[0].tipoIE.Equals("I") && ws.IsEnabledInteropInterna() && 
                    !DocsPAWA.utils.MultiCasellaManager.RoleIsAuthorizedSend(this.Page, "I")))
                    return false;
            }
            if (item is DocsPaWR.DestinatarioInterno)
            {
                DocsPaWR.DestinatarioInterno destinatarioInterno = item as DocsPaWR.DestinatarioInterno;
                if (destinatarioInterno != null && destinatarioInterno.DisabledTrasm)
                    return false;
            }
            return item.IncludiInSpedizione;
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
            if (TipoDestinatario != TipoDestinatarioEnum.Esterno)
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
            if (TipoDestinatario != TipoDestinatarioEnum.Esterno)
                return !isVisible;
            else
                return isVisible;
        }

        /// <summary>
        /// Imposta la visibilità sul flag di selezione/deselezione dei destinatari 
        /// </summary>
        protected bool VisibilitySelectDeselectRecipients()
        {
            DocsPaWR.DocsPaWebService ws = new DocsPaWR.DocsPaWebService();
            DocsPaWR.SpedizioneDocumento infoSpedizione = SpedizioneManager.GetSpedizioneDocumento(DocumentManager.getDocumentoSelezionato());
            bool select = true;

            if (this.TipoDestinatario == DocsPAWA.Spedizione.TipoDestinatarioEnum.EsternoNonInteroperante)
                return false;

            if(this.TipoDestinatario == DocsPAWA.Spedizione.TipoDestinatarioEnum.Esterno &&
                ((infoSpedizione.DestinatariEsterni.Count(d => d.DatiDestinatari[0].tipoIE != null && d.DatiDestinatari[0].tipoIE.Equals("I")) > 0 &&
                    !DocsPAWA.utils.MultiCasellaManager.RoleIsAuthorizedSend(this.Page, "I")) ||
                    (infoSpedizione.DestinatariEsterni.Count(d => d.DatiDestinatari[0].tipoIE != null && d.DatiDestinatari[0].tipoIE.Equals("E")) > 0 &&
                    !DocsPAWA.utils.MultiCasellaManager.RoleIsAuthorizedSend(this.Page, "E"))))
                select = false;

            return select;
        }
        #endregion
    }
}