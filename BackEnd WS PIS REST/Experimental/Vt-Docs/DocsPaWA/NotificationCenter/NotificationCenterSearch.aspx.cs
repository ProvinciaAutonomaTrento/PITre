using System;
using System.Collections.Generic;
using System.Web.UI;
using DocsPAWA.NotificationCenterReference;
using DocsPAWA.utils;
using DocsPAWA;
using System.Text;

namespace Test
{
    public partial class NotificationCenterSearch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Associazione evento onChange javascript alle due drop down list che prevedono ricerca
            // precisa e per intervallo
            this.ddlProtoNum.Attributes["onChange"] = "showIntervalInfo('ProtoNum');";
            this.ddlRecDate.Attributes["onChange"] = "showIntervalInfo('RecDate');";

        }

        /// <summary>
        /// Ricerca degli item e visualizzazione dei risultati nel data grid
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Conversione dei valori inseriti nei filtri
            DateTime lowDate, hightDate;
            int lowNum, maxNum;

            DateTime.TryParse(this.txtRecDateFrom.Text, out lowDate);
            DateTime.TryParse(this.txtRecDateTo.Text, out hightDate);
            Int32.TryParse(this.txtProtoNumFrom.Text, out lowNum);
            Int32.TryParse(this.txtProtoNumTo.Text, out maxNum);

            List<Item> items = new List<Item>();

            // Validazione dei filtri ed eventuale ricerca o segnalazione di errore
            if (this.ValidateSearchFilters())
                items = NotificationCenterHelper.SearchItem(
                    Int32.Parse(UserManager.getInfoUtente(this).idPeople),
                    lowNum > 0,
                    lowNum,
                    maxNum > lowNum ? maxNum : lowNum,
                    lowDate > new DateTime(1972, 01, 01),
                    lowDate,
                    hightDate > lowDate ? hightDate : lowDate,
                    this.ddlEvent.SelectedIndex != 0,
                    this.ddlEvent.SelectedValue == "M" ? "consegna" : "eccezione",
                    UserManager.getInfoAmmCorrente(UserManager.getInfoUtente().idAmministrazione).Codice);

            if (items.Count == 0)
            {
                items = null;
                string noItemText = "Nessun risultato trovato.";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "NoItems",
                    String.Format("alert('{0}');", noItemText.ToString()), true);
            }
            this.dgResult.DataSource = items;
            this.dgResult.DataBind();

        }

        /// <summary>
        /// Metodo per la validazione dei valori inseriti nei filtri di ricerca
        /// </summary>
        /// <returns>Esito della validazione</returns>
        private bool ValidateSearchFilters()
        {
            // Esito della validazione
            bool validationResult = true;
            StringBuilder validationText = new StringBuilder();

            #region Validazione filtro numero di protocollo

            // Se valorizzate, le caselle di ricerca per numero di protocollo devono contenere
            // numeri e, se la ricerca è per intervallo, il valore massimo deve essere maggiore
            // di quello minimo
            if (!String.IsNullOrEmpty(this.txtProtoNumFrom.Text))
            {
                try
                {
                    int protoFrom = Int32.Parse(this.txtProtoNumFrom.Text);

                    if (this.ddlProtoNum.SelectedValue == "I" && !String.IsNullOrEmpty(this.txtProtoNumTo.Text))
                    {
                        try
                        {
                            int protoTo = Int32.Parse(this.txtProtoNumTo.Text);
                            if (protoTo < protoFrom)
                            {
                                validationResult = false;
                                validationText.Append("Intervallo di numero di protocollo non valido; ");

                            }


                        }
                        catch (Exception nan)
                        {
                            validationResult = false;
                            validationText.Append("Il numero di protocollo deve essere un numero; ");
                        }
                    }

                }
                catch (Exception nan)
                {
                    validationResult = false;
                    validationText.Append("Il numero di protocollo deve essere un numero; ");

                }
            }

            #endregion

            #region Validazione intervallo date

            // Se la ricerca delle date è per intervallo, il valore massimo deve essere maggiore
            // di quello minimo
            if (!String.IsNullOrEmpty(this.txtRecDateFrom.Text))
            {
                try
                {
                    DateTime publishDateFrom = DateTime.Parse(this.txtRecDateFrom.Text);

                    if (this.ddlRecDate.SelectedValue == "I" && !String.IsNullOrEmpty(this.txtRecDateTo.Text))
                    {
                        try
                        {
                            DateTime publishDateTo = DateTime.Parse(this.txtRecDateTo.Text);
                            if (publishDateTo < publishDateFrom)
                            {
                                validationResult = false;
                                validationText.Append("Intervallo di date di pubblicazione non valido; ");

                            }


                        }
                        catch (Exception nad)
                        {
                            validationResult = false;
                            validationText.Append("La data di pubblicazione deve essere una data; ");
                        }
                    }

                }
                catch (Exception nan)
                {
                    validationResult = false;
                    validationText.Append("La data di pubblicazione deve essere una data; ");

                }
            }

            #endregion

            // Se è stato rilevato almeno un problema, viene visualizzato un messaggio all'utente
            if (!validationResult)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "validationError",
                    String.Format("alert('{0}');", validationText.ToString()), true);

            return validationResult;
        }

        /// <summary>
        /// Metodo per la formattazione della segnatura di un documento cui si riferisce una notifica
        /// </summary>
        /// <param name="item">Item per cui ricostruire la segnatura</param>
        /// <returns>Segnatura relativa all'item</returns>
        protected String GetSignature(object item)
        {
            String retVal = String.Empty;
            Item convertedItem = item as Item;

            if (convertedItem != null)
                retVal = DocumentManager.GetDocumentSignatureByProfileId(convertedItem.MessageId.ToString());

            return retVal;
        }

        /// <summary>
        /// Metodo per la creazione dello script per chiusura della finestra e redirezionamento alla
        /// pagina di dettaglio del documento
        /// </summary>
        /// <param name="item">Item per cui generare lo script</param>
        /// <returns>Script relativo all'item</returns>
        protected String GetSignatureScript(object item)
        {
            String retVal = String.Empty;
            Item convertedItem = item as Item;

            if(convertedItem != null)
                retVal = String.Format("self.close(); window.returnValue = '{0}';", convertedItem.MessageId);

            return retVal;

        }

    }
}