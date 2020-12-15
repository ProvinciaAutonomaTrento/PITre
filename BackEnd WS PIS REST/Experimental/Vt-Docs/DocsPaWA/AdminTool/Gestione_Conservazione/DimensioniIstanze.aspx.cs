using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DocsPAWA.AdminTool.Gestione_Conservazione
{
    public partial class DimensioniIstanze : System.Web.UI.Page
    {
        DocsPAWA.DocsPaWR.ChiaveConfigurazione configString;
        #region MEV CS 1.5 - F03_01
        DocsPAWA.DocsPaWR.ChiaveConfigurazione configString_percToller;
        #endregion
        protected int IdAmministrazione
        {
            get
            {
                string idAmministrazione = AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "3");

                return Convert.ToInt32(idAmministrazione);
            }
        }

        private DocsPAWA.DocsPaWR.ChiaveConfigurazione GetChiaveConfigurazione(string idChiaveConfig, string idAmministrazione)
        {
            utils.ConfigRepository cR = utils.InitConfigurationKeys.getInstance(idAmministrazione);
            return (DocsPAWA.DocsPaWR.ChiaveConfigurazione)cR[idChiaveConfig];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                configString = GetChiaveConfigurazione("BE_CONSERVAZIONE_MAX_DIM_ISTANZA", IdAmministrazione.ToString());
                #region MEV CS 1.5 - F03_01
                configString_percToller = GetChiaveConfigurazione("BE_CONS_PERC_TOLL_MAX_DIM_IST", IdAmministrazione.ToString());
                #endregion
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (configString != null)
            {
                string[] configVals = configString.Valore.Split('§');
                tb_num_docs.Text = configVals[0];
                tb_dim_istanza.Text = configVals[1];
            }
            else
            {
                tb_num_docs.Text = "250";
                tb_dim_istanza.Text = "650";
            }

            #region MEV CS 1.5 - F03_01
            if (configString_percToller != null)
            {
                tb_perc.Text = configString_percToller.Valore;
            }
            else 
            {
                tb_perc.Text = "10";
            }
            #endregion
        }

        /// <summary>
        /// Aggiornamento di una chiave
        /// </summary>
        /// <param name="tipoRuolo"></param>
        /// <returns></returns>
        private DocsPAWA.DocsPaWR.ValidationResultInfo UpdateChiaveConfig(ref DocsPAWA.DocsPaWR.ChiaveConfigurazione chiaveConfig)
        {
            
            AmmUtils.WebServiceLink wsl = new AmmUtils.WebServiceLink();
            return wsl.updateChiaveConfig(chiaveConfig);
            
        }

        protected void btn_salva_dimensioni_Click(object sender, EventArgs e)
        {
            #region OldCode
            //int num_docs, dim_istanza;
            //if (string.IsNullOrEmpty(tb_num_docs.Text) || string.IsNullOrEmpty(tb_dim_istanza.Text) || !Int32.TryParse(tb_num_docs.Text, out num_docs) || !Int32.TryParse(tb_dim_istanza.Text, out dim_istanza))
            //{
            //    ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_valori", "alert('Inserire i valori richiesti, in formato numerico!');", true);
            //}
            //else
            //{
            //    if (configString == null)
            //    {
            //        configString = new DocsPaWR.ChiaveConfigurazione
            //        {
            //            Codice = "BE_CONSERVAZIONE_MAX_DIM_ISTANZA",
            //            TipoChiave = "B",
            //            IDAmministrazione = IdAmministrazione.ToString(),
            //            Modificabile = "0",
            //            Visibile = "0",
            //            IsGlobale = "0",
            //            Descrizione = "Chiave di configurazione per definire la massima dimensione delle istanze di conservazione.",
            //            Valore = "0"
            //        };

            //        DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            //        ws.addChiaveConfigurazione(configString);
            //        this.Clear(IdAmministrazione.ToString());
            //        configString = GetChiaveConfigurazione("BE_CONSERVAZIONE_MAX_DIM_ISTANZA", IdAmministrazione.ToString());
            //    }

            //    configString.Valore = string.Format("{0}§{1}", tb_num_docs.Text, tb_dim_istanza.Text);
            //    DocsPaWR.ValidationResultInfo result = null;

            //    result = UpdateChiaveConfig(ref configString);

            //    if (!result.Value)
            //    {
            //        // this.ShowValidationMessage(result);

            //        //
            //        // MEV CS 1.3 - LOG Dimensione istanze
            //        try
            //        {
            //            DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
            //            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            //            ws.WriteLog(sessionManager.getUserAmmSession(), "AMM_DIMENSIONI_ISTANZE", string.Empty, "Definizione dimensioni massime istanze per Ente " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"), false);
            //        }
            //        catch (Exception ex) 
            //        {
            //        }
            //        // End MEV CS 1.3 - LOG Dimensione istanze
            //        //
            //    }
            //    else
            //    {
            //        this.Clear(IdAmministrazione.ToString());

            //        //
            //        // MEV CS 1.3 - LOG Dimensione istanze
            //        try
            //        {
            //            DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
            //            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            //            ws.WriteLog(sessionManager.getUserAmmSession(), "AMM_DIMENSIONI_ISTANZE", string.Empty, "Definizione dimensioni massime istanze per Ente " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"), true);
            //        }
            //        catch (Exception ex) 
            //        {
            //        }
            //        // End MEV CS 1.3 - LOG Dimensione istanze
            //        //
            //    }
            //}
            #endregion

            #region MEV CS 1.5 - F03_01 - NewCode
            int num_docs, dim_istanza, perc_Toll;
            if (string.IsNullOrEmpty(tb_num_docs.Text) || 
                string.IsNullOrEmpty(tb_dim_istanza.Text) ||
                string.IsNullOrEmpty(tb_perc.Text) ||
                !Int32.TryParse(tb_num_docs.Text, out num_docs) || 
                !Int32.TryParse(tb_dim_istanza.Text, out dim_istanza) ||
                !Int32.TryParse(tb_perc.Text, out perc_Toll)
                )
            {
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "alt_valori", "alert('Inserire i valori richiesti, in formato numerico!');", true);
            }
            else
            {
                if (configString == null)
                {
                    configString = new DocsPaWR.ChiaveConfigurazione
                    {
                        Codice = "BE_CONSERVAZIONE_MAX_DIM_ISTANZA",
                        TipoChiave = "B",
                        IDAmministrazione = IdAmministrazione.ToString(),
                        Modificabile = "0",
                        Visibile = "0",
                        IsGlobale = "0",
                        Descrizione = "Chiave di configurazione per definire la massima dimensione delle istanze di conservazione.",
                        Valore = "0"
                    };

                    DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    ws.addChiaveConfigurazione(configString);
                    this.Clear(IdAmministrazione.ToString());
                    configString = GetChiaveConfigurazione("BE_CONSERVAZIONE_MAX_DIM_ISTANZA", IdAmministrazione.ToString());
                }

                configString.Valore = string.Format("{0}§{1}", tb_num_docs.Text, tb_dim_istanza.Text);
                DocsPaWR.ValidationResultInfo result = null;

                result = UpdateChiaveConfig(ref configString);

                if (!result.Value)
                {
                    // this.ShowValidationMessage(result);

                    //
                    // MEV CS 1.3 - LOG Dimensione istanze
                    try
                    {
                        DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                        DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                        ws.WriteLog(sessionManager.getUserAmmSession(), "AMM_DIMENSIONI_ISTANZE", string.Empty, "Definizione dimensioni massime istanze per Ente " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"), false);
                    }
                    catch (Exception ex)
                    {
                    }
                    // End MEV CS 1.3 - LOG Dimensione istanze
                    //
                }
                else
                {
                    this.Clear(IdAmministrazione.ToString());

                    //
                    // MEV CS 1.3 - LOG Dimensione istanze
                    try
                    {
                        DocsPAWA.AdminTool.Manager.SessionManager sessionManager = new DocsPAWA.AdminTool.Manager.SessionManager();
                        DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                        ws.WriteLog(sessionManager.getUserAmmSession(), "AMM_DIMENSIONI_ISTANZE", string.Empty, "Definizione dimensioni massime istanze per Ente " + AmmUtils.UtilsXml.GetAmmDataSession((string)Session["AMMDATASET"], "0"), true);
                    }
                    catch (Exception ex)
                    {
                    }
                    // End MEV CS 1.3 - LOG Dimensione istanze
                    //
                }

                #region Percentuale di Tolleranza
                if (configString_percToller == null)
                {
                    configString_percToller = new DocsPaWR.ChiaveConfigurazione
                    {
                        Codice = "BE_CONS_PERC_TOLL_MAX_DIM_IST",
                        TipoChiave = "B",
                        IDAmministrazione = IdAmministrazione.ToString(),
                        Modificabile = "0",
                        Visibile = "0",
                        IsGlobale = "0",
                        Descrizione = "Chiave di configurazione per definire la percentuale di tolleranza delle istanze di conservazione.",
                        Valore = "0"
                    };

                    DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    ws.addChiaveConfigurazione(configString_percToller);
                    this.Clear(IdAmministrazione.ToString());
                    configString_percToller = GetChiaveConfigurazione("BE_CONS_PERC_TOLL_MAX_DIM_IST", IdAmministrazione.ToString());
                }

                if (configString_percToller != null)
                {
                    configString_percToller.Valore = tb_perc.Text;
                    DocsPaWR.ValidationResultInfo result_percToll = null;

                    result_percToll = UpdateChiaveConfig(ref configString_percToller);

                    if (!result_percToll.Value)
                    {
                        // this.ShowValidationMessage(result);
                    }
                    else
                    {
                        this.Clear(IdAmministrazione.ToString());
                    }
                }
                #endregion
            }
            #endregion
        }

        private void Clear(string idAmm)
        {
            DocsPaWR.DocsPaWebService ws = new DocsPAWA.DocsPaWR.DocsPaWebService();
            ws.clearHashTableChiaviConfig(idAmm);
            utils.InitConfigurationKeys.remove(idAmm);
        }
    }
}