using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAAdminTool.DocsPaWR;
using SAAdminTool.SiteNavigation;

namespace SAAdminTool.UserControls
{
    public partial class MultiDestinatari : System.Web.UI.UserControl
    {
        private List<SAAdminTool.DocsPaWR.Corrispondente> list = null;
        protected void Page_Load(object sender, EventArgs e)
        {

            //Utils.startUp(this);
            
            if (!IsPostBack)
            {
                this.tipo = string.Empty;
                //this.ListCorr = null;
                GetTheme();
                GetInitData();
                this.tipo = Request.QueryString["tipo"];
            }
        }

        protected void BtnSaveCorr_Click(object sender, EventArgs e)
        {
            string idCorr = Request.Form["rbl_pref"];
            bool standard = false;
            DocsPaWR.Corrispondente[] listaDest;

            if (!string.IsNullOrEmpty(idCorr))
            {
                DocsPaWR.SchedaDocumento schedaDoc = DocumentManager.getDocumentoSelezionato();
                DocsPaWR.Corrispondente tempCorr = UserManager.getCorrispondenteBySystemID(this.Page, idCorr);
                if (tempCorr == null)
                {
                    tempCorr = UserManager.getCorrispondenteByCodRubricaRubricaComune(this.Page, idCorr);
                }

                if (schedaDoc != null && schedaDoc.protocollo != null && !string.IsNullOrEmpty(schedaDoc.tipoProto))
                {
                    if (schedaDoc.tipoProto.Equals("A"))
                    {
                        if (((SAAdminTool.DocsPaWR.ProtocolloEntrata) schedaDoc.protocollo).mittenti != null &&
                            ((SAAdminTool.DocsPaWR.ProtocolloEntrata) schedaDoc.protocollo).mittenti.Length > 0 &&
                            UserManager.esisteCorrispondente(
                                ((SAAdminTool.DocsPaWR.ProtocolloEntrata) schedaDoc.protocollo).mittenti, tempCorr))
                        {
                            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi",
                                                                "alert('Attenzione! Corrispondente già presente nei mittenti multipli');",
                                                                true);
                        }
                        else
                        {
                            ((SAAdminTool.DocsPaWR.ProtocolloEntrata) schedaDoc.protocollo).mittente = tempCorr;
                            ((SAAdminTool.DocsPaWR.ProtocolloEntrata) schedaDoc.protocollo).daAggiornareMittente = true;
                        }
                    }

                    if (schedaDoc.tipoProto.Equals("P"))
                    {
                        if (tipo.Equals("M"))
                        {
                            ((SAAdminTool.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo).mittente = tempCorr;
                            ((SAAdminTool.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo).daAggiornareMittente = true;
                        }
                        else
                        {
                            if (((SAAdminTool.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo).destinatari != null &&
                                ((SAAdminTool.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo).destinatari.Length > 0 &&
                                UserManager.esisteCorrispondente(
                                    ((SAAdminTool.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo).destinatari, tempCorr))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi",
                                                                    "alert('Attenzione! Corrispondente già presente nei destinatari');",
                                                                    true);
                            }
                            else
                            {
                                if (((SAAdminTool.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo).destinatariConoscenza !=
                                    null &&
                                    ((SAAdminTool.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo).destinatariConoscenza.
                                        Length > 0 &&
                                    UserManager.esisteCorrispondente(
                                        ((SAAdminTool.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo).
                                            destinatariConoscenza, tempCorr))
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi",
                                                                        "alert('Attenzione! Corrispondente già presente nei destinatari in conoscenza');",
                                                                        true);
                                }
                                else
                                {
                                    listaDest = ((SAAdminTool.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo).destinatari;
                                    ((SAAdminTool.DocsPaWR.ProtocolloUscita) schedaDoc.protocollo).destinatari =
                                        UserManager.addCorrispondente(listaDest, tempCorr);
                                }
                            }
                        }
                    }

                    if (schedaDoc.tipoProto.Equals("I"))
                    {
                        if (tipo.Equals("M"))
                        {
                            ((SAAdminTool.DocsPaWR.ProtocolloInterno) schedaDoc.protocollo).mittente = tempCorr;
                            ((SAAdminTool.DocsPaWR.ProtocolloInterno) schedaDoc.protocollo).daAggiornareMittente = true;
                        }
                        else
                        {
                            if (((SAAdminTool.DocsPaWR.ProtocolloInterno) schedaDoc.protocollo).destinatari != null &&
                                ((SAAdminTool.DocsPaWR.ProtocolloInterno) schedaDoc.protocollo).destinatari.Length > 0 &&
                                UserManager.esisteCorrispondente(
                                    ((SAAdminTool.DocsPaWR.ProtocolloInterno) schedaDoc.protocollo).destinatari, tempCorr))
                            {
                                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi",
                                                                    "alert('Attenzione! Corrispondente già presente nei destinatari');",
                                                                    true);
                            }
                            else
                            {
                                if (
                                    ((SAAdminTool.DocsPaWR.ProtocolloInterno) schedaDoc.protocollo).destinatariConoscenza !=
                                    null &&
                                    ((SAAdminTool.DocsPaWR.ProtocolloInterno) schedaDoc.protocollo).destinatariConoscenza.
                                        Length > 0 &&
                                    UserManager.esisteCorrispondente(
                                        ((SAAdminTool.DocsPaWR.ProtocolloInterno) schedaDoc.protocollo).
                                            destinatariConoscenza, tempCorr))
                                {
                                    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi",
                                                                        "alert('Attenzione! Corrispondente già presente nei destinatari in conoscenza');",
                                                                        true);
                                }
                                else
                                {
                                    listaDest = ((SAAdminTool.DocsPaWR.ProtocolloInterno) schedaDoc.protocollo).destinatari;
                                    ((SAAdminTool.DocsPaWR.ProtocolloInterno) schedaDoc.protocollo).destinatari =
                                        UserManager.addCorrispondente(listaDest, tempCorr);
                                }
                            }
                        }
                    }
                }


                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "chiudi", "window.close();", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alert",
                                                    "alert('Attenzione! Selezionare un corrispondente');", true);
            }
        }

        protected void GetTheme()
        {
            string Tema = string.Empty;
            string idAmm = string.Empty;
            if ((string)Session["AMMDATASET"] != null)
                idAmm = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");
            else
            {
                if (UserManager.getInfoUtente() != null)
                    idAmm = UserManager.getInfoUtente().idAmministrazione;
            }

            UserManager userM = new UserManager();
            Tema = userM.getCssAmministrazione(idAmm);

            if (!string.IsNullOrEmpty(Tema))
            {
                string[] colorsplit = Tema.Split('^');
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.titlePage.ForeColor = System.Drawing.Color.White;
                this.titlePage.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#" + colorsplit[2]);

            }
            else
            {
                System.Drawing.ColorConverter colConvert = new ColorConverter();
                this.titlePage.ForeColor = System.Drawing.Color.White;
                this.titlePage.BackColor = (System.Drawing.Color)colConvert.ConvertFromString("#810d06");
            }

        }

        protected String GetCorrName(DocsPaWR.Corrispondente elem)
        {
            return elem.descrizione;
        }

        protected String GetCorrCodice(DocsPaWR.Corrispondente elem)
        {
            return elem.codiceRubrica;
        }

        protected String GetCorrID(DocsPaWR.Corrispondente elem)
        {
            
                return elem.systemId;
           

        }

        protected String GetCorrmail(DocsPaWR.Corrispondente elem)
        {
            return elem.email;
        }

        //protected String GetCorrTipo(DocsPaWR.Corrispondente elem)
        //{
        //    string result = string.Empty;
        //    string codRegTemp = elem.codiceRegistro;
        //    if (elem.isRubricaComune == true)
        //    {
        //        codRegTemp = "RC";
        //    }
        //    else
        //    {
        //        if (codRegTemp == null || codRegTemp.Equals(""))
        //        {
        //            if (elem.interno == true)
        //            {
        //                codRegTemp = string.Empty;
        //            }
        //            else
        //            {
        //                codRegTemp = "TUTTI";
        //            }
        //        }
        //        else
        //        {
        //            codRegTemp = elem.codiceRegistro;
        //        }
        //    }

        //    return codRegTemp;
        //}

        private List<SAAdminTool.DocsPaWR.Corrispondente> ListCorr
        {
            get { return list; }
            set
            {
                list = value;
            }
        }

        protected void GetInitData()
        {
            if (Session["ListCorrByMail"] != null)
            {
                this.ListCorr = (List<SAAdminTool.DocsPaWR.Corrispondente>) Session["ListCorrByMail"];

                this.grvListaCorr.DataSource = this.ListCorr;
                this.grvListaCorr.DataBind();
            }

        }

        private string tipo
        {
            get
            {
                return CallContextStack.CurrentContext.ContextState["tipo"] as string;
            }
            set
            {
                CallContextStack.CurrentContext.ContextState["tipo"] = value;
            }
        }

    }
}