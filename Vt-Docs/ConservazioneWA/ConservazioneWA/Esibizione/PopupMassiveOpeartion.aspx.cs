using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConservazioneWA.Esibizione
{
    public partial class PopupMassiveOpeartion : System.Web.UI.Page
    {
        public void Page_Load(object sender, EventArgs e)
        {
            //Primo caricamento della pagina
            if (!Page.IsPostBack)
            {
                // Recupero dalla sessione le liste di oggetti lavorati massivamente
                List<string> ListIdObject_OK = Session["ListIdObject_OK"] as List<string>;
                List<string> ListIdObject_warning = Session["ListIdObject_warning"] as List<string>;
                List<string> ListIdObject_KO = Session["ListIdObject_KO"] as List<string>;

                Dictionary<string, WSConservazioneLocale.ItemsConservazione> dictRiepilogoMass = Session["dictRiepilogoMass"] as Dictionary<string, WSConservazioneLocale.ItemsConservazione>;

                //You should already have a dataset binded with data grid, right? Let say that your                  //dataset's name is 'ds':
                System.Data.DataSet ds = new System.Data.DataSet();
                ds.Tables.Add("MyTable");

                ds.Tables[0].Columns.Add("ObjId");
                ds.Tables[0].Columns.Add("IstId");
                ds.Tables[0].Columns.Add("Obj");
                ds.Tables[0].Columns.Add("Result");
                ds.Tables[0].Columns.Add("Details");

                if (dictRiepilogoMass != null && dictRiepilogoMass.Count > 0)
                {
                    // Oggetti con esito OK
                    if (ListIdObject_OK != null && ListIdObject_OK.Count > 0)
                    {
                        foreach (string idObject in ListIdObject_OK)
                        {
                            if (dictRiepilogoMass.ContainsKey(idObject))
                            {
                                WSConservazioneLocale.ItemsConservazione itmCons;
                                itmCons = dictRiepilogoMass[idObject];

                                if (itmCons != null)
                                {
                                    //create row and fill it:      
                                    System.Data.DataRow rw = ds.Tables[0].NewRow();

                                    // Le colonne mappano con i datafield
                                    rw["ObjId"] = itmCons.ID_Profile.ToString();
                                    rw["IstId"] = itmCons.ID_Conservazione.ToString();
                                    rw["Obj"] = itmCons.desc_oggetto;
                                    rw["Result"] = "OK";
                                    rw["Details"] = "";

                                    //rw["Result"] = "OK";
                                    //rw["Details"] = itmCons.CodFasc;


                                    //add the row:
                                    ds.Tables["MyTable"].Rows.Add(rw);
                                }
                            }
                        }
                    }

                    // Oggetti con esito Warning
                    if (ListIdObject_warning != null && ListIdObject_warning.Count > 0)
                    {
                        foreach (string idObject in ListIdObject_warning)
                        {
                            if (dictRiepilogoMass.ContainsKey(idObject))
                            {
                                WSConservazioneLocale.ItemsConservazione itmCons;
                                itmCons = dictRiepilogoMass[idObject];

                                if (itmCons != null)
                                {
                                    //create row and fill it:      
                                    System.Data.DataRow rw = ds.Tables[0].NewRow();

                                    // Le colonne mappano con i datafield
                                    rw["ObjId"] = itmCons.ID_Profile.ToString();
                                    rw["IstId"] = itmCons.ID_Conservazione.ToString();
                                    rw["Obj"] = itmCons.desc_oggetto;
                                    rw["Result"] = "KO";
                                    rw["Details"] = "Elemento già presente nell'istanza di esibizione";

                                    //rw["Result"] = "KO - Elemento già presente";
                                    //rw["Details"] = itmCons.CodFasc;

                                    //add the row:
                                    ds.Tables["MyTable"].Rows.Add(rw);
                                }
                            }
                        }
                    }

                    // Oggetti con esito Warning
                    if (ListIdObject_KO != null && ListIdObject_KO.Count > 0)
                    {
                        foreach (string idObject in ListIdObject_KO)
                        {
                            if (dictRiepilogoMass.ContainsKey(idObject))
                            {
                                WSConservazioneLocale.ItemsConservazione itmCons;
                                itmCons = dictRiepilogoMass[idObject];

                                if (itmCons != null)
                                {
                                    //create row and fill it:      
                                    System.Data.DataRow rw = ds.Tables[0].NewRow();

                                    // Le colonne mappano con i datafield
                                    rw["ObjId"] = itmCons.ID_Profile.ToString();
                                    rw["IstId"] = itmCons.ID_Conservazione.ToString();
                                    rw["Obj"] = itmCons.desc_oggetto;
                                    rw["Result"] = "KO";
                                    rw["Details"] = "Invio Istanza Esibizione KO";

                                    //rw["Result"] = "KO";
                                    //rw["Details"] = itmCons.CodFasc;

                                    //add the row:
                                    ds.Tables["MyTable"].Rows.Add(rw);
                                }
                            }
                        }
                    }
                }
                else 
                {
                    // Recupero valori dalla sessione
                    List<WSConservazioneLocale.ItemsConservazione> listIstanzeOK = Session["listIstanzeOK"] as List<WSConservazioneLocale.ItemsConservazione>;
                    List<WSConservazioneLocale.ItemsConservazione>  listIstanzeWarning = Session["listIstanzeWarning"] as List<WSConservazioneLocale.ItemsConservazione>;
                    List<WSConservazioneLocale.ItemsConservazione> listIstanzeKO = Session["listIstanzeKO"] as List<WSConservazioneLocale.ItemsConservazione>;

                    // Oggetti con esito OK
                    if (listIstanzeOK != null && listIstanzeOK.Count > 0)
                    {
                        foreach (WSConservazioneLocale.ItemsConservazione itmCons in listIstanzeOK)
                        {
                            if (itmCons != null)
                            {
                                //create row and fill it:      
                                System.Data.DataRow rw = ds.Tables[0].NewRow();

                                // Le colonne mappano con i datafield
                                rw["ObjId"] = itmCons.ID_Profile.ToString();
                                rw["IstId"] = itmCons.ID_Conservazione.ToString();
                                rw["Obj"] = itmCons.desc_oggetto;
                                rw["Result"] = "OK";
                                rw["Details"] = itmCons.CodFasc;

                                //add the row:
                                ds.Tables["MyTable"].Rows.Add(rw);
                            }
                            
                        }
                    }

                    // Oggetti con esito Warning
                    if (listIstanzeWarning != null && listIstanzeWarning.Count > 0)
                    {
                        foreach (WSConservazioneLocale.ItemsConservazione itmCons in listIstanzeWarning)
                        {
                            if (itmCons != null)
                            {
                                //create row and fill it:      
                                System.Data.DataRow rw = ds.Tables[0].NewRow();

                                // Le colonne mappano con i datafield
                                rw["ObjId"] = itmCons.ID_Profile.ToString();
                                rw["IstId"] = itmCons.ID_Conservazione.ToString();
                                rw["Obj"] = itmCons.desc_oggetto;
                                rw["Result"] = "KO - Elemento già presente";
                                rw["Details"] = itmCons.CodFasc;

                                //add the row:
                                ds.Tables["MyTable"].Rows.Add(rw);
                            }
                        }
                    }

                    // Oggetti con esito KO
                    if (listIstanzeKO != null && listIstanzeKO.Count > 0)
                    {
                        foreach (WSConservazioneLocale.ItemsConservazione itmCons in listIstanzeKO)
                        {
                            if (itmCons != null)
                            {
                                //create row and fill it:      
                                System.Data.DataRow rw = ds.Tables[0].NewRow();

                                // Le colonne mappano con i datafield
                                rw["ObjId"] = itmCons.ID_Profile.ToString();
                                rw["IstId"] = itmCons.ID_Conservazione.ToString();
                                rw["Obj"] = itmCons.desc_oggetto;
                                rw["Result"] = "KO";
                                rw["Details"] = itmCons.CodFasc;

                                //add the row:
                                ds.Tables["MyTable"].Rows.Add(rw);
                            }
                        }
                    }
                }

                grdReport.DataSource = ds;
                grdReport.DataBind();
                pnlReport.Visible = true;

            }

            // Pulisce sessione
            CleanSession();
        }

        private void CleanSession()
        {
            // Sezione per documenti e fascicolo massivo
            Session.Remove("ListIdObject_OK");
            Session.Remove("ListIdObject_warning");
            Session.Remove("ListIdObject_KO");

            Session.Remove("dictRiepilogoMass");

            // Sezione per Istanze Massivo
            Session.Remove("listIstanzeOK");
            Session.Remove("listIstanzeWarning");
            Session.Remove("listIstanzeKO");
        }


    }
}