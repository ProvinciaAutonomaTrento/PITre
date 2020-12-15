using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ConservazioneWA.WSConservazioneLocale;
using System.Web.UI.HtmlControls;

namespace ConservazioneWA.Esibizione
{
    public partial class PopupSceltaIstanze : System.Web.UI.Page
    {
        protected WSConservazioneLocale.InfoUtente infoUtente;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.infoUtente = ((WSConservazioneLocale.InfoUtente)Session["infoutCons"]);

            Response.Expires = -1;

            if (!IsPostBack)
            {
                string listaIstanze = this.Request.QueryString["listaIstanze"];
                // Id oggetto chiamante
                string idObject = this.Request.QueryString["idObject"];

                FetchData(listaIstanze);
            }
        }

        protected void FetchData(string listaIstanze)
        {
            List<InfoConservazione> listInfoCons = new List<InfoConservazione>();

            // Popolo la lista di istanze che contengono lo specifico documento
            if (!string.IsNullOrEmpty(listaIstanze))
            {
                string[] singoleIstanze = listaIstanze.Split('-');
                if (singoleIstanze != null)
                {
                    for (int i = 0; i < singoleIstanze.Length; i++)
                    {
                        //Recupero informazioni sulle istanze di conservazione
                        if (infoUtente != null)
                        {
                            InfoConservazione infoCons = Utils.ConservazioneManager.getInfoConservazione(singoleIstanze[i], infoUtente);
                            if (infoCons != null)
                            {
                                // Popolo la lista delle istanze di conservazione
                                listInfoCons.Add(infoCons);
                            }
                        }
                    }
                }
            }

            HttpContext.Current.Session["listInfoCons"] = listInfoCons;

            this.grvFileType.DataSource = listInfoCons;
            this.grvFileType.CurrentPageIndex = 0;
            this.grvFileType.DataBind();

            this.btn_salva.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            this.btn_salva.Attributes.Add("onmouseout", "this.className='cbtn';");

            this.btn_annulla.Attributes.Add("onmouseover", "this.className='cbtnHover';");
            this.btn_annulla.Attributes.Add("onmouseout", "this.className='cbtn';");
        }

        protected String GetIDIstanza(InfoConservazione temp)
        {
            return temp.SystemID;
        }

        #region oldCode
        //protected void CheckRadio(object sender, DataGridItemEventArgs e)
        //{
        //    string check = string.Empty;
        //    List<InfoConservazione> listInfoCons = null;

        //    listInfoCons = HttpContext.Current.Session["listInfoCons"] as List<InfoConservazione>;

        //    if (listInfoCons != null && listInfoCons.Count > 0)
        //    {
        //        HtmlInputRadioButton irb = e.Item.FindControl("rb_ist") as HtmlInputRadioButton;

        //        if (irb != null)
        //        {
        //            // Solo il primo della lista di default è selezionato
        //            if (listInfoCons[0].SystemID == irb.Value)
        //            {
        //                // Check dell'input
        //                irb.Checked = true;
        //                // Popolo l'hiddenField
        //                hf_rb_ist_selected.Value = irb.Value;
        //            }
        //        }
        //    }
        //}
        #endregion

        protected String GetDescrizioneIstanza(InfoConservazione temp)
        {
            return temp.Descrizione;
        }

        protected String GetDataAperturaIstanza(InfoConservazione temp)
        {
            return temp.Data_Apertura;
        }

        protected String GetDataInvioIstanza(InfoConservazione temp)
        {
            return temp.Data_Invio;
        }

        protected String GetDataConservazioneIstanza(InfoConservazione temp)
        {
            return temp.Data_Conservazione;
        }

        /// Al clic viene salvata la lista dei formato documenti ammessi
        /// </summary>
        protected void BtnSaveDocumentFormat_Click(object sender, EventArgs e)
        {
            string idIstanza = string.Empty;

            #region oldCode
            //if (
            //    (Request.Form["rbl_pref"] != null && !string.IsNullOrEmpty(Request.Form["rbl_pref"].ToString()))
            //    || (this.hf_rb_ist_selected != null && !string.IsNullOrEmpty(hf_rb_ist_selected.Value))
            //    )
            #endregion
            if (Request.Form["rbl_pref"] != null && !string.IsNullOrEmpty(Request.Form["rbl_pref"].ToString()))
            {
                #region oldCode
                //if (Request.Form["rbl_pref"] != null && !string.IsNullOrEmpty(Request.Form["rbl_pref"].ToString()))
                //    idIstanza = Request.Form["rbl_pref"].ToString();
                //if (this.hf_rb_ist_selected != null && !string.IsNullOrEmpty(hf_rb_ist_selected.Value))
                //    idIstanza = hf_rb_ist_selected.Value;
                #endregion
                idIstanza = Request.Form["rbl_pref"].ToString();
                
                InfoConservazione IstanzaSelezionata = null;

                List<InfoConservazione> listInfoConservazione = HttpContext.Current.Session["listInfoCons"] as List<InfoConservazione>;
                foreach (InfoConservazione tempCons in listInfoConservazione)
                {
                    if (tempCons.SystemID.Equals(idIstanza))
                    {
                        IstanzaSelezionata = tempCons;
                        listInfoConservazione = null;
                        listInfoConservazione = new List<InfoConservazione>();
                        listInfoConservazione.Add(IstanzaSelezionata);
                        break;
                    }
                }

                Dictionary<string, string> dictIstSelected = null;
                string idObject = this.Request.QueryString["idObject"];
                
                //Gestione dictionary
                if (HttpContext.Current.Session["dictIstSelected"] != null)
                {
                    //Dictionary già popolato
                    //Controllo che il dictionary per quell'oggetto contiene già l'item selected
                    dictIstSelected = HttpContext.Current.Session["dictIstSelected"] as Dictionary<string, string>;

                    if (dictIstSelected.ContainsKey(idObject)) 
                    {
                        // Aggiorno il valore del dictionary con l'istanza selezionata
                        dictIstSelected[idObject] = IstanzaSelezionata.SystemID;
                    }
                }
                else
                {
                    // Creo il dictionary <objectSelected, idIstanza>
                    dictIstSelected = new Dictionary<string, string>();
                    //Aggiungo valori al dictionary
                    dictIstSelected[idObject] = IstanzaSelezionata.SystemID;
                }
                // Metto il dictionary in sessione
                HttpContext.Current.Session["dictIstSelected"] = dictIstSelected;

                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "close_and_save('" + IstanzaSelezionata.SystemID + "');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "no_istanza", "alert('Selezionare un istanza');", true);
            }

        }

    }
}