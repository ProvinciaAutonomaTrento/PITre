using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ConservazioneWA.Utils;

namespace ConservazioneWA.UserControl
{
    public partial class Menu : System.Web.UI.UserControl
    {
        public string paginaChiamante;
        public string profiloUtente;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region oldcode
            //if (this.paginaChiamante == "HOME")
            //{
            //    this.MenuHome.Visible = true;
            //    this.MenuIstanze.Visible = false;
            //    this.MenuSupporti.Visible = false;
            //    this.MenuNotifiche.Visible = false;
            //    this.MenuDocumenti.Visible = false;
            //    this.MenuFascicoli.Visible = false;
            //    this.MenuLog.Visible = false;
            //}
            //if (this.paginaChiamante == "ISTANZE")
            //{
            //    this.MenuHome.Visible = false;
            //    this.MenuIstanze.Visible = true;
            //    this.MenuSupporti.Visible = false;
            //    this.MenuNotifiche.Visible = false;
            //    this.MenuDocumenti.Visible = false;
            //    this.MenuFascicoli.Visible = false;
            //    this.MenuLog.Visible = false;
            //}
            //if (this.paginaChiamante == "SUPPORTI")
            //{
            //    this.MenuHome.Visible = false;
            //    this.MenuIstanze.Visible = false;
            //    this.MenuSupporti.Visible = true;
            //    this.MenuNotifiche.Visible = false;
            //    this.MenuDocumenti.Visible = false;
            //    this.MenuFascicoli.Visible = false;
            //    this.MenuLog.Visible = false;
            //}
            //if (this.paginaChiamante == "NOTIFICHE")
            //{
            //    this.MenuHome.Visible = false;
            //    this.MenuIstanze.Visible = false;
            //    this.MenuSupporti.Visible = false;
            //    this.MenuNotifiche.Visible = true;
            //    this.MenuDocumenti.Visible = false;
            //    this.MenuFascicoli.Visible = false;
            //    this.MenuLog.Visible = false;
            //}
            //if (this.paginaChiamante == "DOCUMENTI")
            //{
            //    this.MenuHome.Visible = false;
            //    this.MenuIstanze.Visible = false;
            //    this.MenuSupporti.Visible = false;
            //    this.MenuNotifiche.Visible = false;
            //    this.MenuDocumenti.Visible = true;
            //    this.MenuFascicoli.Visible = false;
            //    this.MenuLog.Visible = false;
            //}
            //if (this.paginaChiamante == "FASCICOLI")
            //{
            //    this.MenuHome.Visible = false;
            //    this.MenuIstanze.Visible = false;
            //    this.MenuSupporti.Visible = false;
            //    this.MenuNotifiche.Visible = false;
            //    this.MenuDocumenti.Visible = false;
            //    this.MenuFascicoli.Visible = true;
            //    this.MenuLog.Visible = false;
            //}
            //if (this.paginaChiamante == "LOG")
            //{
            //    this.MenuHome.Visible = false;
            //    this.MenuIstanze.Visible = false;
            //    this.MenuSupporti.Visible = false;
            //    this.MenuNotifiche.Visible = false;
            //    this.MenuDocumenti.Visible = false;
            //    this.MenuFascicoli.Visible = false;
            //    this.MenuLog.Visible = true;
            //}

            //if (this.paginaChiamante == "REGISTRO")
            //{
            //    this.MenuHome.Visible = false;
            //    this.MenuIstanze.Visible = false;
            //    this.MenuSupporti.Visible = false;
            //    this.MenuNotifiche.Visible = false;
            //    this.MenuDocumenti.Visible = false;
            //    this.MenuFascicoli.Visible = false;
            //    this.MenuRegistro.Visible = true;
            //    this.MenuLog.Visible = false;

            //}
            #endregion

            #region newCode
            if (((this.profiloUtente != null && this.profiloUtente == "CONSERVAZIONE") || this.profiloUtente == null))
            {
                // Home Profiolo Utente CONSERVAZIONE
                //if (this.paginaChiamante == "HOME" && ((this.profiloUtente!= null && this.profiloUtente == "CONSERVAZIONE") || this.profiloUtente == null))
                if (this.paginaChiamante == "HOME")
                {
                    this.MenuHome.Visible = true;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = false;
                    this.MenuLog.Visible = false;
                    this.MenuEsibizione.Visible = false;

                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = false;
                }

                //Ricerca Istanze Profilo Conservazione
                //if (this.paginaChiamante == "ISTANZE" && ((this.profiloUtente != null && this.profiloUtente == "CONSERVAZIONE") || this.profiloUtente == null))
                if (this.paginaChiamante == "ISTANZE")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = true;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = false;
                    this.MenuLog.Visible = false;
                    this.MenuEsibizione.Visible = false;

                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = false;
                }

                // Ricerca supporti Profilo Conservazione
                //if (this.paginaChiamante == "SUPPORTI" && ((this.profiloUtente != null && this.profiloUtente == "CONSERVAZIONE") || this.profiloUtente == null))
                if (this.paginaChiamante == "SUPPORTI")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = true;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = false;
                    this.MenuLog.Visible = false;


                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = false;
                }

                // Ricerca notifiche Profilo Conservazione
                //if (this.paginaChiamante == "NOTIFICHE" && ((this.profiloUtente != null && this.profiloUtente == "CONSERVAZIONE") || this.profiloUtente == null))
                if (this.paginaChiamante == "NOTIFICHE")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = true;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = false;
                    this.MenuLog.Visible = false;
                    this.MenuEsibizione.Visible = false;

                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = false;
                }

                // Ricerca documenti Profilo Conservazione
                //if (this.paginaChiamante == "DOCUMENTI" && ((this.profiloUtente != null && this.profiloUtente == "CONSERVAZIONE") || this.profiloUtente == null))
                if (this.paginaChiamante == "DOCUMENTI")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = true;
                    this.MenuFascicoli.Visible = false;
                    this.MenuLog.Visible = false;
                    this.MenuEsibizione.Visible = false;

                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = false;
                }

                // Ricerca fascicoli Profilo Conservazione
                //if (this.paginaChiamante == "FASCICOLI" && ((this.profiloUtente != null && this.profiloUtente == "CONSERVAZIONE") || this.profiloUtente == null))
                if (this.paginaChiamante == "FASCICOLI")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = true;
                    this.MenuLog.Visible = false;
                    this.MenuEsibizione.Visible = false;

                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = false;
                }

                // Ricerca log Profilo Conservazione
                //if (this.paginaChiamante == "LOG" && ((this.profiloUtente != null && this.profiloUtente == "CONSERVAZIONE") || this.profiloUtente == null))
                if (this.paginaChiamante == "LOG")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = false;
                    this.MenuLog.Visible = true;
                    this.MenuEsibizione.Visible = false;

                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = false;
                }

                // Ricerca registro Profilo Conservazione
                //if (this.paginaChiamante == "REGISTRO" && ((this.profiloUtente != null && this.profiloUtente == "CONSERVAZIONE") || this.profiloUtente == null))
                if (this.paginaChiamante == "REGISTRO")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = false;
                    this.MenuRegistro.Visible = true;
                    this.MenuLog.Visible = false;
                    this.MenuEsibizione.Visible = false;

                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = false;
                }

                // Ricerca istanze di esibizione Profilo Conservazione
                if (this.paginaChiamante == "ESIBIZIONE")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = false;
                    this.MenuRegistro.Visible = false;
                    this.MenuLog.Visible = false;
                    this.MenuEsibizione.Visible = true;

                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = false;
                }
            }

            if (this.profiloUtente != null && this.profiloUtente == "ESIBIZIONE")
            {
                // Home Profilo Utente Esibizione
                //if (this.paginaChiamante == "HOME" && (this.profiloUtente != null && this.profiloUtente == "ESIBIZIONE"))
                if (this.paginaChiamante == "HOME_ESIBIZIONE")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = false;
                    this.MenuLog.Visible = false;
                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = true;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = false;
                }

                // Menù di Ricerca Istanze Profilo Esibizione
                //if (this.paginaChiamante == "PROVA" && (this.profiloUtente != null && this.profiloUtente == "ESIBIZIONE"))
                if (this.paginaChiamante == "ISTANZE_ESIBIZIONE")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = false;
                    this.MenuRegistro.Visible = false;
                    this.MenuLog.Visible = false;

                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = true;
                    this.PnlGestioneEsibizione.Visible = false;
                }

                // Menù di Ricerca Documenti Profilo Esibizione
                if (this.paginaChiamante == "DOCUMENTI_ESIBIZIONE")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = false;
                    this.MenuRegistro.Visible = false;
                    this.MenuLog.Visible = false;

                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = true;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = false;
                }

                // Menù di Ricerca Fascicoli Profilo Esibizione
                if (this.paginaChiamante == "FASCICOLI_ESIBIZIONE")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = false;
                    this.MenuRegistro.Visible = false;
                    this.MenuLog.Visible = false;

                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = true;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = false;
                }

                // Menù di Ricerca e gestione istanze esibizione
                if (this.paginaChiamante == "ESIBIZIONE")
                {
                    this.MenuHome.Visible = false;
                    this.MenuIstanze.Visible = false;
                    this.MenuSupporti.Visible = false;
                    this.MenuNotifiche.Visible = false;
                    this.MenuDocumenti.Visible = false;
                    this.MenuFascicoli.Visible = false;
                    this.MenuRegistro.Visible = false;
                    this.MenuLog.Visible = false;

                    // Pannelli ESIBIZIONE
                    this.PnlHomeEsibizione.Visible = false;
                    this.PnlRicercaDocumentiEsibizione.Visible = false;
                    this.PnlRicercaFascicoliEsibizione.Visible = false;
                    this.PnlRicercaIstanzeEsibizione.Visible = false;
                    this.PnlGestioneEsibizione.Visible = true;
                }
            }
            #endregion
        }

        protected void Exit(object sender, EventArgs e)
        {
            this.Session.Abandon();

            string sessionID = Session.SessionID;

            UserManager.logOff(((WSConservazioneLocale.InfoUtente)Session["infoutCons"]), Page);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "close", "window.close();", true);
        }

        public string PaginaChiamante
        {
            get
            {
                return this.paginaChiamante;
            }
            set
            {
                this.paginaChiamante = value;
            }
        }

        public string ProfiloUtente
        {
            get
            {
                return this.profiloUtente;

                //if (HttpContext.Current.Session["profiloUtente"] != null)
                //{
                //    this.profiloUtente = HttpContext.Current.Session["profiloUtente"] as string;
                //}
                //return this.profiloUtente;
            }
            set
            {
                //HttpContext.Current.Session["profiloUtente"] = value;
                this.profiloUtente = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LinkAction_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            #region oldcode
            //
            // Old Code

            //if (btn.CommandName == "HOME")
            //{
            //    this.Response.Redirect("~/HomePageNew.aspx");
            //}
            //else
            //{
            //    if (btn.CommandName == "ISTANZE")
            //    {
            //        this.Response.Redirect("~/RicercaIstanze.aspx");
            //    }
            //    else
            //    {
            //        if (btn.CommandName == "SUPPORTI")
            //        {
            //            this.Response.Redirect("~/RicercaSupportiIstanze.aspx");
            //        }
            //        else
            //        {
            //            if (btn.CommandName == "DOCUMENTI")
            //            {
            //                this.Response.Redirect("~/RicercaDocumenti.aspx");
            //            }
            //            else
            //            {
            //                if (btn.CommandName == "FASCICOLI")
            //                {
            //                    this.Response.Redirect("~/RicercaFascicoli.aspx");
            //                }
            //                else
            //                {
            //                    if (btn.CommandName == "LOG")
            //                    {
            //                        this.Response.Redirect("~/RicercaLog.aspx");
            //                    }
            //                    else
            //                    {
            //                        if (btn.CommandName == "NOTIFICHE")
            //                        {
            //                            this.Response.Redirect("~/RicercaNotifiche.aspx");
            //                        }

            //                        else
            //                        {
            //                            if (btn.CommandName == "REPORT")
            //                                this.Response.Redirect("~/RicercaReport.aspx");
            //                            else
            //                            {
            //                                if (btn.CommandName == "STAMPE")
            //                                {
            //                                    this.Response.Redirect("~/RicercaStampe.aspx");
            //                                }
            //                                // PROVA
            //                                else
            //                                {
            //                                    if (btn.CommandName == "ISTANZE_ESIBIZIONE")
            //                                    {
            //                                        this.Response.Redirect("~/Esibizione/RicercaIstanzeEsibizione.aspx");
            //                                    }
            //                                }
            //                                // END PROVA
            //                            }
            //                        }


            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            // End Old Code
            //
            #endregion

            if ((this.profiloUtente != null && this.profiloUtente.Equals("CONSERVAZIONE")) || this.profiloUtente == null) 
            {
                if (btn.CommandName == "HOME")
                {
                    this.Response.Redirect("~/HomePageNew.aspx");
                }
                else
                {
                    if (btn.CommandName == "ISTANZE")
                    {
                        this.Response.Redirect("~/RicercaIstanze.aspx");
                    }
                    else
                    {
                        if (btn.CommandName == "SUPPORTI")
                        {
                            this.Response.Redirect("~/RicercaSupportiIstanze.aspx");
                        }
                        else
                        {
                            if (btn.CommandName == "DOCUMENTI")
                            {
                                this.Response.Redirect("~/RicercaDocumenti.aspx");
                            }
                            else
                            {
                                if (btn.CommandName == "FASCICOLI")
                                {
                                    this.Response.Redirect("~/RicercaFascicoli.aspx");
                                }
                                else
                                {
                                    if (btn.CommandName == "LOG")
                                    {
                                        this.Response.Redirect("~/RicercaLog.aspx");
                                    }
                                    else
                                    {
                                        if (btn.CommandName == "NOTIFICHE")
                                        {
                                            this.Response.Redirect("~/RicercaNotifiche.aspx");
                                        }

                                        else
                                        {
                                            if (btn.CommandName == "REPORT")
                                                this.Response.Redirect("~/RicercaReport.aspx");
                                            else
                                            {
                                                if (btn.CommandName == "STAMPE")
                                                {
                                                    this.Response.Redirect("~/RicercaStampe.aspx");
                                                }
                                                else
                                                {
                                                    if (btn.CommandName == "ESIBIZIONE")
                                                    {
                                                        this.Response.Redirect("~/Esibizione/RicercaIstanzeEsibizione.aspx");
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.profiloUtente != null && this.profiloUtente.Equals("ESIBIZIONE")) 
            {
                if (btn.CommandName == "HOME_ESIBIZIONE")
                {
                    this.Response.Redirect("~/Esibizione/HomePageEsibizione.aspx");
                }
                else 
                {
                    if (btn.CommandName == "ISTANZE_ESIBIZIONE")
                    {
                        this.Response.Redirect("~/Esibizione/RicercaIstanzeConsEsibizione.aspx");
                    }
                    else
                    {
                        if (btn.CommandName == "DOCUMENTI_ESIBIZIONE")
                        {
                            this.Response.Redirect("~/Esibizione/RicercaDocumentiEsibizione.aspx");
                        }
                        else 
                        {
                            if (btn.CommandName == "FASCICOLI_ESIBIZIONE") 
                            {
                                this.Response.Redirect("~/Esibizione/RicercaFascicoliEsibizione.aspx");
                            }
                            else
                            {
                                if (btn.CommandName == "ESIBIZIONE")
                                {
                                    this.Response.Redirect("~/Esibizione/RicercaIstanzeEsibizione.aspx");
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
