using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DocsPAWA.DocsPaWR;
using DocsPAWA.NotificationCenterReference;
using DocsPAWA.utils;

namespace DocsPAWA.UserControls
{
    public partial class NotificationCenterItemList : System.Web.UI.UserControl
    {
        private static String URL_LEFT_ARROW = "~/images/NotificationCenter/arrow_left{0}.png";  
        private static String URL_RIGHT_ARROW = "~/images/NotificationCenter/arrow_right{0}.png";

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Expires = -1;

            // Se è la prima volta che viene caricato il controllo, viene eseguita la
            // procedura di inizializzatione del controollo
            if(!IsPostBack)
                this.Initialize();

            // Caricamento degli item ancora non visualizzati dall'utente, solo se l'utente è abilitato alla visualizzazione
            if(NotificationCenterHelper.IsUserEnabledToViewNotificationsCenter(this.Page))
                this.SearchAndShowItems();

        }

        /// <summary>
        /// Metodo per l'inizializzazione del controllo
        /// </summary>
        private void Initialize()
        {
            this.PageNumber = 1;
        }

        /// <summary>
        /// Metodo per la ricerca e la visualizzazione degli item non ancora visti dall'utente
        /// </summary>
        private void SearchAndShowItems()
        {
            // Numero di elementi totali da visualizzare
            int itemCount = 0;

            // Ricerca degli elementi
            InfoUtente userInfo = UserManager.getInfoUtente(this.Page);
            List<Item> notificationCenterItems = null;

            try
            {
                notificationCenterItems = NotificationCenterHelper.LoadItemsNotViewedByUser(
                    Int32.Parse(userInfo.idAmministrazione),
                    UserManager.getInfoAmmCorrente(UserManager.getInfoUtente().idAmministrazione).Codice,
                    Int32.Parse(userInfo.idPeople),
                    this.PageNumber,
                    this.PageSize,
                    out itemCount);
            }
            catch (Exception e)
            {
                
            }

            // Visualizzazione degli elemnti trovati
            this.dlNotificationCenter.DataSource = notificationCenterItems;
            this.dlNotificationCenter.DataBind();

            // Gestione visualizzazione frecce per lo scorrimento dei risultati in avanti ed indietro
            // in base al numero di elementi ed al numero di pagina attuale
            this.imgPreviousPage.Enabled = this.PageNumber > 1;
            this.imgNextPage.Enabled = (itemCount > (this.PageNumber * this.PageSize));

            // Impostazione dell'immagine per le due frecce
            this.imgPreviousPage.ImageUrl = !this.imgPreviousPage.Enabled ?
                String.Format(URL_LEFT_ARROW, String.Empty) :
                String.Format(URL_LEFT_ARROW, "_black");
            this.imgNextPage.ImageUrl = !this.imgNextPage.Enabled ?
                String.Format(URL_RIGHT_ARROW, String.Empty) :
                String.Format(URL_RIGHT_ARROW, "_black");

            // Forzatura dell'aggiornamento dei due update panel con le frecce per fare in modo che
            // rispecchino lo stato attuale di abilitazione
            upLeftArrow.Update();
            upRightArrow.Update();
            
        }

        /// <summary>
        /// Il centro notifiche viene visualizzato solo se attivo per l'utente corrente
        /// </summary>
        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible =  value && NotificationCenterHelper.IsUserEnabledToViewNotificationsCenter(Page);
                int height = !base.Visible ? 150 : 215;
                String showDiv = base.Visible ? "" : "hidden";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Ridimensiona",
                    String.Format("iFrameResize({0}, '{1}');", height, showDiv),
                    true);
                    //String.Format("try { document.getElementById('divNotificationCenter').style.visibility = '{0}'; top.principale.iFrame_sx.frameElement.height = {1}; } catch(ex) {}", showDiv, height), true);
            }
        }

        /// <summary>
        /// Numero della pagina da visualizzare
        /// </summary>
        public int PageNumber
        {
            get
            {
                return Int32.Parse(ViewState["PageNumber"].ToString());
            }
            set
            {
                ViewState["PageNumber"] = value;
            }
        }

        /// <summary>
        /// Dimensione della pagina
        /// </summary>
        public int PageSize
        {
            get
            {
                return 4;
            }
        }

        /// <summary>
        /// Metodo per il caricamento degli item alla pagina successiva
        /// </summary>
        protected void imgNextPage_Click(object sender, ImageClickEventArgs e)
        {
            this.PageNumber++;
            this.SearchAndShowItems();

        }

        /// <summary>
        /// Metodo per il caricamento degli item alla pagina precedente
        /// </summary>
        protected void imgPreviousPage_Click(object sender, ImageClickEventArgs e)
        {
            this.PageNumber--;
            this.SearchAndShowItems();

        }

        /// <summary>
        /// Metodo per l'impostazione di un item come visto
        /// </summary>
        protected void imgDelete_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton convertedSource = sender as ImageButton;
            if (convertedSource != null)
                NotificationCenterHelper.SetItemAsViewed(
                    Int32.Parse(convertedSource.CommandArgument), 
                    Int32.Parse(UserManager.getInfoUtente().idPeople),
                    UserManager.getInfoAmmCorrente(UserManager.getInfoUtente().idAmministrazione).Codice);
            this.Initialize();
            this.SearchAndShowItems();
        }

        protected String FormatLink(object item)
        {
            String retVal = String.Empty;
            Item convertedItem = item as Item;

            if (convertedItem != null)
                retVal = String.Format(Utils.getHttpFullPath() + "/documento/gestioneDoc.aspx?tab=protocollo&from=newRicDoc&idProfile={0}&protoType=A';", convertedItem.MessageId);

            return retVal;
 
        }
    }
}