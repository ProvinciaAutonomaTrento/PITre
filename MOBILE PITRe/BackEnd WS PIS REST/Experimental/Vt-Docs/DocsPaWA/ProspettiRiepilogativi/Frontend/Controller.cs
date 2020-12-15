using System;
using System.Collections;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using ProspettiRiepilogativi;
using System.Collections.Generic;
using ReflectionExcelReportEngine;


namespace ProspettiRiepilogativi.Frontend
{
    /// <summary>
    /// Summary description for controller.
    /// </summary>
    public class Controller
    {
        #region Dichiarazione e costruttore
        //private static Model _objModel;
        private static DocsPAWA.DocsPaWR.DocsPaWebService _docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();

        protected DocsPAWA.DocsPaWR.DocsPaWebService WS
        {
            get
            {
                if (_docsPaWS == null)
                {
                    _docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
                    _docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                }

                return _docsPaWS;
            }
        }

        public Controller()
        {

        }
        #endregion

        #region DO_GetAmministrazioni
        /// <summary>
        /// DO_GetAmministrazioni
        /// </summary>
        /// <param name="list">Lista da popolare</param>
        /// <returns>Lista popolata</returns>
        public static DropDownList DO_GetAmministrazioni(DropDownList list)
        {
            try
            {
                ArrayList amministrazioni = new ArrayList(_docsPaWS.DO_GetAmministrazioni());


                foreach (DocsPAWA.DocsPaWR.PR_Amministrazione amm in amministrazioni)
                {
                    list.Items.Add(new ListItem(amm.Descrizione, amm.Codice));
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region DO_GetRegistriByAmministrazione
        /// <summary>
        /// DO_GetRegistriByAmministrazione
        /// </summary>
        /// <param name="list">Lista da Popolare</param>
        /// <param name="id_amm">System_id dell'amministrazione di cui recuperar i registri</param>
        /// <returns>Lista popolata</returns>
        public static DropDownList DO_GetRegistriByAmministrazione(DropDownList list, string cod_amm)
        {
            try
            {
                int id_amm = DO_GetIdAmmByCodAmm(cod_amm);
                ArrayList registri = new ArrayList(_docsPaWS.DO_GetRegistri(id_amm));
                list.Items.Clear();
                list.Items.Add("");
                foreach (DocsPAWA.DocsPaWR.PR_Registro reg in registri)
                {
                    list.Items.Add(new ListItem(reg.Descrizione, reg.System_id));
                }
                return list;
            }

            catch (Exception ex)
            {
                //errore nel recupero dei dati
                throw ex;
            }

        }
        #endregion

        #region DO_GetAnniProfilazione
        /// <summary>
        /// DO_GetAnniProfilazione
        /// </summary>
        /// <param name="list">lista da popolare</param>
        /// <param name="idReg">System_Id del registro di cui recuperare gli anni</param>
        /// <returns>DropDownList popolata</returns>
        public static DropDownList DO_GetAnniProfilazione(DropDownList list, int idReg)
        {
            ///Codice che inserisce gli anni manualmente 
            ///(attivo dal 1900 all'anno corrente, valido fino al 2030)
            for (int i = 1900; i < 2030; i++)
            {
                if (i <= DateTime.Now.Year)
                {
                    list.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
            }

            list.SelectedIndex = list.Items.Count - 1;
            #region Codice Che recupera gli anni dal DB
            //			_objModel = new Model();
            //			ArrayList anni = _objModel.DO_GetAnniProfilazione(idReg);
            //			list.Items.Clear();
            //			list.Items.Add("");
            //			for(int i = 0;i<anni.Count;i++)
            //			{
            //				list.Items.Add(new ListItem(anni[i].ToString(), anni[i].ToString()));
            //			}
            #endregion

            return list;
        }
        #endregion

        #region Do_GetSedi
        /// <summary>
        /// DO_Do_GetSedi
        /// </summary>
        /// <returns>sedi</returns>
        /// 

        public static DropDownList DO_GetSedi(int idAmm, DropDownList list, out bool popolata)
        {
            ArrayList sedi = new ArrayList(_docsPaWS.DO_GetSedi());
            //list.Items.Clear();

            if (sedi.Count != 0)
            {
                //list.Items.Add("Tutte le Sedi");
                for (int i = 0; i < sedi.Count; i++)
                {
                    list.Items.Add(new ListItem(sedi[i].ToString()));
                }
                popolata = true;
                return list;
            }
            else
            {
                popolata = false;
                return list;
            }

        }

        #endregion

        #region DO_StampaReport
        /// <summary>
        /// DO_StampaReport
        /// </summary>
        /// <param name="page">Pagina di provenienza</param>
        /// <param name="tipoReport">tipo Report</param>
        /// <param name="filtri">parametri per il recupero del DataTable</param>
        /// <returns>DocsPaWR.FileDocumento</returns>
        public static DocsPAWA.DocsPaWR.FileDocumento DO_StampaReport(string templateFilePath, ReportDisponibili tipoReport, ArrayList filtri, int idPeople, string timeStamp)
        {
            #region Dichiarazioni
            //string templateFilePath = "";
            DocsPAWA.DocsPaWR.FileDocumento fileRep = new DocsPAWA.DocsPaWR.FileDocumento();
            //Creiamo un arraylist con i parametri che verranno scritti 
            //nei vari paragrafi di ciascun report
            int idreg = 0, anno = 0, id_amm = 0, mese = 0, idTitolario = 0, numPratica = 0;
            string idRegCC = string.Empty, dataSpedDa = "", dataSpedA = "", confermaProt = "";
            string CDCDataDa = string.Empty, CDCDataA = string.Empty, CDCUffici = string.Empty, CDCMagistrato = string.Empty, CDCRevisore = string.Empty;
            string TOTDeferimenti = string.Empty;
            string TOTDECRETIESAMINATI = string.Empty;
            string descRegistro = string.Empty;

            string sede = "";
            #endregion

            #region Creazione dei parametri
            ArrayList parametriPDF = new ArrayList();
            Parametro reg = new Parametro();
            Parametro _anno = new Parametro();
            Parametro _sede = new Parametro();
            Parametro _amm = new Parametro();
            Parametro _mese = new Parametro();
            Parametro _titolario = new Parametro();
            Parametro _classifica = new Parametro();
            Parametro _pratica = new Parametro();
            Parametro _docSpeditiInterop = new Parametro();
            Parametro _confermeRicevute = new Parametro();
            Parametro _confermeMancanti = new Parametro();
            Parametro _codiceReg = new Parametro();
            Parametro _CDCDataDa = new Parametro();
            Parametro _CDCDataA = new Parametro();
            Parametro _CDCUffici = new Parametro();
            Parametro _CDCMagistrato = new Parametro();
            Parametro _CDCRevisore = new Parametro();
            Parametro _TOTDEFERIMENTI = new Parametro();
            Parametro _TOTDECRETIESAMINATI = new Parametro();
            Parametro _DESCREGISTRO = new Parametro();

            string modalita = "";

            foreach (Parametro p in filtri)
            {
                switch (p.Nome)
                {
                    case "amm":
                        id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                        break;
                    case "reg":
                        idreg = Convert.ToInt32(p.Valore);
                        break;
                    case "anno":
                        anno = Convert.ToInt32(p.Valore);
                        break;
                    case "sede":
                        sede = p.Valore;
                        break;
                    case "mod":
                        modalita = p.Valore;
                        break;
                    case "mese":
                        _mese.Valore = p.Valore;
                        break;
                    case "titolario":
                        idTitolario = Convert.ToInt32(p.Valore);
                        break;
                    case "pratica":
                        numPratica = Convert.ToInt32(p.Valore);
                        break;
                    case "registroCC":
                        idRegCC = p.Valore;
                        break;
                    case "dataSpedDa":
                        dataSpedDa = p.Valore;
                        break;
                    case "dataSpedA":
                        dataSpedA = p.Valore;
                        break;
                    case "confermaProt":
                        confermaProt = p.Valore;
                        break;
                    case "CDCDataDa":
                        CDCDataDa = p.Valore;
                        break;
                    case "CDCDataA":
                        CDCDataA = p.Valore;
                        break;
                    case "CDCUffici":
                        CDCUffici = p.Valore;
                        break;
                    case "CDCMagistrato":
                        CDCMagistrato = p.Valore;
                        break;
                    case "CDCRevisore":
                        CDCRevisore = p.Valore;
                        break;
                    case "TOTDEFERIMENTI":
                        TOTDeferimenti = p.Valore;
                        break;
                    case "TOTDECRETIESAMINATI":
                        TOTDECRETIESAMINATI = p.Valore;
                        break;
                    case "DESCREGISTRO":
                        descRegistro = p.Valore;
                        break;
                }
            }
            #endregion

            #region Recuperiamo il DataSet
            DataSet ds = new DataSet();
            _docsPaWS.Timeout = System.Threading.Timeout.Infinite;
            switch (tipoReport)
            {
                #region ReportDisponibili.Annuale_By_Registro
                case ReportDisponibili.Annuale_By_Registro:
                    try
                    {
                        //						if(_mese.Valore != null && _mese.Valore != "")
                        //						{
                        //							ds = _docsPaWS.DO_GetDSReportAnnualeByReg(id_amm,idreg,anno,Convert.ToInt32(_mese.Valore),sede,true);
                        //						}
                        //						else
                        //						{
                        //							ds = _docsPaWS.DO_GetDSReportAnnualeByReg(id_amm,idreg,anno,DateTime.Now.Month,sede,false);
                        //						}

                        if (_mese.Valore != null && _mese.Valore != "") //� stato selezionato un mese oppure per tutti i mesi
                        {
                            ds = _docsPaWS.DO_GetDSReportAnnualeByReg(id_amm, idreg, anno, Convert.ToInt32(_mese.Valore), sede, true, idTitolario);
                        }
                        else //nessun mese selezionato
                        {
                            //elisa 04/01/2006
                            if (anno < DateTime.Now.Year)
                            {
                                mese = 12; // se l'anno selezionato � minore del corrente deve fare la statistica di tutti i mesi			
                            }
                            else
                            {
                                mese = DateTime.Now.Month;
                            }

                            ds = _docsPaWS.DO_GetDSReportAnnualeByReg(id_amm, idreg, anno, mese, sede, false, idTitolario);
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "amm":
                                        id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                                        _amm.Descrizione = "@param3";
                                        _amm.Valore = Controller.Do_GetVarDescAmmByIdAmm(id_amm);
                                        break;
                                    case "reg":
                                        idreg = Convert.ToInt32(p.Valore);
                                        reg.Descrizione = "@param1";
                                        reg.Valore = p.Descrizione;
                                        break;
                                    case "anno":
                                        anno = Convert.ToInt32(p.Valore);
                                        _anno.Descrizione = "@param2";
                                        _anno.Valore = anno.ToString();
                                        break;
                                    case "sede":
                                        if ((p.Valore != "") && (reg.Valore != null))
                                        {
                                            reg.Valore = reg.Valore + " - Sede di: " + p.Valore;
                                        }
                                        break;
                                    case "mese":
                                        _mese.Descrizione = "mese";
                                        _mese.Valore = p.Valore;
                                        break;

                                }
                            }

                            parametriPDF.Add(reg);
                            parametriPDF.Add(_anno);
                            parametriPDF.Add(_amm);
                            if (_mese.Valore != null && _mese.Valore != "")
                            {
                                parametriPDF.Add(_mese);
                            }


                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.Documenti_Classificati
                case ReportDisponibili.Documenti_Classificati:
                    try
                    {
                        if (modalita != "Compatta")
                        {
                            ds = _docsPaWS.DO_GetDSReportDocClass(idreg, anno, id_amm, sede, idTitolario);
                        }
                        else
                        {
                            ds = _docsPaWS.DO_GetDSReportDocClassCompact(idreg, anno, id_amm, sede, idTitolario);
                        }

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //string totDoc = ds.Tables[0].Rows[0][0].ToString(); 
                            string totDoc = _docsPaWS.DO_GetCountReportDocClassCompact(idreg, anno, id_amm, sede, idTitolario);
                            Parametro _tot = new Parametro("PAR_TOT_DOC", "@param2", totDoc);
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "amm":
                                        id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                                        _amm.Valore = Controller.Do_GetVarDescAmmByIdAmm(id_amm);
                                        _amm.Descrizione = "@param4";
                                        break;
                                    case "reg":
                                        idreg = Convert.ToInt32(p.Valore);
                                        reg.Descrizione = "@param1";
                                        reg.Valore = p.Descrizione;
                                        break;
                                    case "sede":
                                        if (p.Valore != "")
                                        {
                                            if ((p.Valore != "") && (reg.Valore != null))
                                            {
                                                reg.Valore = reg.Valore + " - Sede di: " + p.Valore;
                                            }
                                        }
                                        break;
                                    case "anno":
                                        anno = Convert.ToInt32(p.Valore);
                                        _anno.Descrizione = "@param5";
                                        _anno.Valore = anno.ToString();
                                        break;

                                }
                            }

                            parametriPDF.Add(reg);
                            parametriPDF.Add(_tot);
                            parametriPDF.Add(_amm);
                            parametriPDF.Add(_anno);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.Documenti_Trasmessi_Altre_AOO
                case ReportDisponibili.Documenti_Trasmessi_Altre_AOO:
                    try
                    {
                        //templateFilePath = page.Server.MapPath("../Backend/TemplateXML/R_DocTrasmToAOO.xml");
                        ds = _docsPaWS.DO_GetDSReportDocTrasmToAOO(idreg, anno, id_amm);
                        if (ds.Tables[0].Rows.Count > 1)
                        {
                            string _totDoc = ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["TOT_SPED"].ToString();
                            Parametro _tot1 = new Parametro("TOT_SPED", "@param3", _totDoc);
                            ds.Tables[0].Rows.RemoveAt(ds.Tables[0].Rows.Count - 1);
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "amm":
                                        id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                                        _amm.Descrizione = "@param4";
                                        _amm.Valore = Controller.Do_GetVarDescAmmByIdAmm(id_amm);
                                        break;
                                    case "reg":
                                        idreg = Convert.ToInt32(p.Valore);
                                        reg.Descrizione = "@param1";
                                        reg.Valore = p.Descrizione;
                                        break;
                                    case "anno":
                                        anno = Convert.ToInt32(p.Valore);
                                        _anno.Descrizione = "@param2";
                                        _anno.Valore = anno.ToString();
                                        break;
                                }
                            }

                            parametriPDF.Add(reg);
                            parametriPDF.Add(_anno);
                            parametriPDF.Add(_tot1);
                            parametriPDF.Add(_amm);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.Fascicoli_Per_VT
                case ReportDisponibili.Fascicoli_Per_VT:
                    try
                    {
                        //templateFilePath = page.Server.MapPath("../Backend/TemplateXML/R_FascPerVT.xml");
                        //						if(modalita != "Compatta")
                        //						{
                        //							ds = _docsPaWS.DO_GetDSReportFascicoliPerVT(id_amm,idreg,anno,DateTime.Now.Month);
                        //						}
                        //						else
                        //						{
                        //							ds = _docsPaWS.DO_GetDSReportFascicoliPerVTCompact(id_amm,idreg,anno,DateTime.Now.Month);
                        //						}
                        //elisa 04/01/2005
                        if (anno < DateTime.Now.Year)
                        {
                            mese = 12; // se l'anno selezionato � minore del corrente deve fare la statistica di tutti i mesi			
                        }
                        else
                        {
                            mese = DateTime.Now.Month;
                        }

                        if (modalita != "Compatta")
                        {
                            ds = _docsPaWS.DO_GetDSReportFascicoliPerVT(id_amm, idreg, anno, mese, idTitolario);
                        }
                        else
                        {
                            ds = _docsPaWS.DO_GetDSReportFascicoliPerVTCompact(id_amm, idreg, anno, mese, idTitolario);
                        }
                        if ((ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
                        {
                            string _totFascCr = ds.Tables[0].Rows[0][0].ToString();
                            string _totFascCh = ds.Tables[0].Rows[0][1].ToString();

                            Parametro _totFCr = new Parametro("TOT_FASC_CREATI", "@param3", _totFascCr);
                            Parametro _totFCh = new Parametro("TOT_FASC_CHIUSI", "@param4", _totFascCh);

                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "amm":
                                        id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                                        _amm.Descrizione = "@param5";
                                        _amm.Valore = Controller.Do_GetVarDescAmmByIdAmm(id_amm);
                                        break;
                                    case "reg":
                                        idreg = Convert.ToInt32(p.Valore);
                                        reg.Descrizione = "@param1";
                                        reg.Valore = p.Descrizione;
                                        break;
                                    case "anno":
                                        anno = Convert.ToInt32(p.Valore);
                                        _anno.Descrizione = "@param2";
                                        _anno.Valore = anno.ToString();
                                        break;
                                }
                            }

                            parametriPDF.Add(reg);
                            parametriPDF.Add(_anno);
                            parametriPDF.Add(_totFCr);
                            parametriPDF.Add(_totFCh);
                            parametriPDF.Add(_amm);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.Annuale_By_Fascicolo
                case ReportDisponibili.Annuale_By_Fascicolo:
                    try
                    {
                        //templateFilePath = page.Server.MapPath("../Backend/TemplateXML/R_annualeByFasc.xml");
                        if (_mese.Valore != null && _mese.Valore != "")
                        {
                            ds = _docsPaWS.DO_GetDSReportAnnualeByFasc(id_amm, idreg, anno, Convert.ToInt32(_mese.Valore), idTitolario, true);
                        }
                        else
                        {
                            //elisa 04/01/2006
                            if (anno < DateTime.Now.Year)
                            {
                                mese = 12; // se l'anno selezionato � minore del corrente deve fare la statistica di tutti i mesi			
                            }
                            else
                            {
                                mese = DateTime.Now.Month;
                            }
                            ds = _docsPaWS.DO_GetDSReportAnnualeByFasc(id_amm, idreg, anno, mese, idTitolario, false);
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            string _totFasc = ds.Tables[0].Rows[0][0].ToString();
                            string _totFascCr = ds.Tables[0].Rows[0][1].ToString();
                            string _totFascCh = ds.Tables[0].Rows[0][2].ToString();

                            Parametro _totF = new Parametro("TOT_FASCICOLI", "@param3", _totFasc);
                            Parametro _totFCr = new Parametro("TOT_FASCICOLI_CREATI", "@param4", _totFascCr);
                            Parametro _totFCh = new Parametro("TOT_FASCICOLI_CHIUSI", "@param5", _totFascCh);

                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "amm":
                                        id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                                        _amm.Descrizione = "@param6";
                                        _amm.Valore = Controller.Do_GetVarDescAmmByIdAmm(id_amm);
                                        break;
                                    case "reg":
                                        idreg = Convert.ToInt32(p.Valore);
                                        reg.Descrizione = "@param1";
                                        reg.Valore = p.Descrizione;
                                        break;
                                    case "anno":
                                        anno = Convert.ToInt32(p.Valore);
                                        _anno.Descrizione = "@param2";
                                        _anno.Valore = anno.ToString();
                                        break;
                                }
                            }

                            parametriPDF.Add(reg);
                            parametriPDF.Add(_anno);
                            parametriPDF.Add(_totF);
                            parametriPDF.Add(_totFCr);
                            parametriPDF.Add(_totFCh);
                            parametriPDF.Add(_amm);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.TempiMediLavFascicoli
                case ReportDisponibili.TempiMediLavFascicoli:
                    try
                    {
                        //templateFilePath = page.Server.MapPath("../Backend/TemplateXML/R_TempiMediLavFasc.xml");
                        //elisa 04/01/2006
                        if (anno < DateTime.Now.Year)
                        {
                            mese = 12; // se l'anno selezionato � minore del corrente deve fare la statistica di tutti i mesi			
                        }
                        else
                        {
                            mese = DateTime.Now.Month;
                        }
                        //templateFilePath = page.Server.MapPath("../Backend/TemplateXML/R_TempiMediLavFasc.xml");
                        if (modalita != "Compatta")
                        {
                            ds = _docsPaWS.DO_GetDSTempiMediLavorazione(id_amm, idreg, anno, mese, idTitolario);
                        }
                        else
                        {
                            ds = _docsPaWS.DO_GetDSTempiMediLavorazioneCompact(id_amm, idreg, anno, mese, idTitolario);
                        }
                        if ((ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "amm":
                                        id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                                        _amm.Descrizione = "@param3";
                                        _amm.Valore = Controller.Do_GetVarDescAmmByIdAmm(id_amm);
                                        break;
                                    case "reg":
                                        idreg = Convert.ToInt32(p.Valore);
                                        reg.Descrizione = "@param1";
                                        reg.Valore = p.Descrizione;
                                        break;
                                    case "anno":
                                        anno = Convert.ToInt32(p.Valore);
                                        _anno.Descrizione = "@param2";
                                        _anno.Valore = anno.ToString();
                                        break;
                                }
                            }

                            parametriPDF.Add(reg);
                            parametriPDF.Add(_anno);
                            parametriPDF.Add(_amm);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.ReportDocXSede
                case ReportDisponibili.ReportDocXSede:
                    try
                    {
                        ds = _docsPaWS.DO_GetDSReportDocXSede(id_amm, idreg, anno, idPeople, timeStamp);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "amm":
                                        id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                                        _amm.Descrizione = "@param3";
                                        _amm.Valore = Controller.Do_GetVarDescAmmByIdAmm(id_amm);
                                        break;
                                    case "reg":
                                        idreg = Convert.ToInt32(p.Valore);
                                        reg.Descrizione = "@param1";
                                        reg.Valore = p.Descrizione;
                                        break;
                                    case "anno":
                                        anno = Convert.ToInt32(p.Valore);
                                        _anno.Descrizione = "@param2";
                                        _anno.Valore = anno.ToString();
                                        break;
                                }
                            }

                            parametriPDF.Add(reg);
                            parametriPDF.Add(_anno);
                            parametriPDF.Add(_amm);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;

                #endregion

                #region ReportDisponibili.ReportDocXUo
                case ReportDisponibili.ReportDocXUo:
                    try
                    {
                        ds = _docsPaWS.DO_GetDSReportDocXUo(id_amm, idreg, anno);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "amm":
                                        id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                                        _amm.Descrizione = "@param3";
                                        _amm.Valore = Controller.Do_GetVarDescAmmByIdAmm(id_amm);
                                        break;
                                    case "reg":
                                        idreg = Convert.ToInt32(p.Valore);
                                        reg.Descrizione = "@param1";
                                        reg.Valore = p.Descrizione;
                                        break;
                                    case "anno":
                                        anno = Convert.ToInt32(p.Valore);
                                        _anno.Descrizione = "@param2";
                                        _anno.Valore = anno.ToString();
                                        break;
                                }
                            }

                            parametriPDF.Add(reg);
                            parametriPDF.Add(_anno);
                            parametriPDF.Add(_amm);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;

                #endregion

                #region ReportDisponibile.ReportContatoriDocumento
                case ReportDisponibili.ReportContatoriDocumento:
                    try
                    {
                        ds = _docsPaWS.DO_GetDSReportContatoriDoc(id_amm, anno);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "amm":
                                        id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                                        _amm.Descrizione = "@param3";
                                        _amm.Valore = Controller.Do_GetVarDescAmmByIdAmm(id_amm);
                                        break;
                                    case "anno":
                                        anno = Convert.ToInt32(p.Valore);
                                        _anno.Descrizione = "@param2";
                                        _anno.Valore = anno.ToString();
                                        break;
                                }
                            }
                            parametriPDF.Add(_anno);
                            parametriPDF.Add(_amm);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.ReportContatoriFascicolo
                case ReportDisponibili.ReportContatoriFascicolo:
                    try
                    {
                        ds = _docsPaWS.DO_GetDSReportContatoriFasc(id_amm, anno);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "amm":
                                        id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                                        _amm.Descrizione = "@param3";
                                        _amm.Valore = Controller.Do_GetVarDescAmmByIdAmm(id_amm);
                                        break;
                                    case "anno":
                                        anno = Convert.ToInt32(p.Valore);
                                        _anno.Descrizione = "@param2";
                                        _anno.Valore = anno.ToString();
                                        break;
                                }
                            }
                            parametriPDF.Add(_anno);
                            parametriPDF.Add(_amm);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.ReportProtocolloArma
                case ReportDisponibili.ReportProtocolloArma:
                    try
                    {
                        ds = _docsPaWS.DO_GetDSReportProtocolloArma(idRegCC, idTitolario, id_amm);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "titolario":
                                        // id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                                        _titolario.Descrizione = "@param3";
                                        _titolario.Valore = p.Descrizione;
                                        break;
                                    case "registroCC":
                                        idRegCC = p.Valore;
                                        reg.Descrizione = "@param2";
                                        reg.Valore = p.Descrizione;
                                        break;
                                    case "codReg":
                                        _codiceReg.Descrizione = "@param1";
                                        _codiceReg.Valore = p.Descrizione;
                                        break;
                                }
                            }
                            parametriPDF.Add(_titolario);
                            parametriPDF.Add(_codiceReg);
                            parametriPDF.Add(reg);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.ReportDettaglioPratica
                case ReportDisponibili.ReportDettaglioPratica:

                    try
                    {
                        ds = _docsPaWS.DO_GetDSReportDettaglioPratica(idRegCC, idTitolario, numPratica, id_amm);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "titolario":
                                        // id_amm = Convert.ToInt32(Controller.DO_GetIdAmmByCodAmm(p.Valore).ToString());
                                        _titolario.Descrizione = "@param1";
                                        _titolario.Valore = p.Descrizione;
                                        break;
                                    case "registroCC":
                                        idRegCC = p.Valore;
                                        reg.Descrizione = "@param4";
                                        reg.Valore = p.Descrizione;
                                        break;
                                    case "classifica":
                                        _classifica.Descrizione = "@param2";
                                        _classifica.Valore = p.Valore;
                                        break;
                                    case "pratica":
                                        _pratica.Descrizione = "@param3";
                                        _pratica.Valore = p.Valore;
                                        break;
                                    case "codReg":
                                        _codiceReg.Descrizione = "@param5";
                                        _codiceReg.Valore = p.Descrizione;
                                        break;
                                }
                            }
                            parametriPDF.Add(_titolario);
                            parametriPDF.Add(reg);
                            parametriPDF.Add(_classifica);
                            parametriPDF.Add(_pratica);
                            parametriPDF.Add(_codiceReg);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.ReportGiornaleRiscontri
                case ReportDisponibili.ReportGiornaleRiscontri:
                    try
                    {
                        ds = _docsPaWS.DO_GetDSReportGiornaleRiscontri(idRegCC, id_amm);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "registroCC":
                                        idRegCC = p.Valore;
                                        reg.Descrizione = "@param1";
                                        reg.Valore = p.Descrizione;
                                        break;
                                    case "codReg":
                                        _codiceReg.Descrizione = "@param2";
                                        _codiceReg.Valore = p.Descrizione;
                                        break;
                                }
                            }
                            parametriPDF.Add(reg);
                            parametriPDF.Add(_codiceReg);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.ReportDocSpeditiInterop
                case ReportDisponibili.ReportDocSpeditiInterop:
                    try
                    {
                        ds = _docsPaWS.DO_GetDSReportDocSpeditiInterop(id_amm, idRegCC, dataSpedDa, dataSpedA, confermaProt);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "totDocSpediti":
                                        _docSpeditiInterop.Descrizione = "@param1";
                                        _docSpeditiInterop.Valore = p.Valore;
                                        break;
                                    case "confermeAttese":
                                        _confermeRicevute.Descrizione = "@param2";
                                        _confermeRicevute.Valore = p.Valore;
                                        break;
                                    case "confermeMancanti":
                                        _confermeMancanti.Descrizione = "@param3";
                                        _confermeMancanti.Valore = p.Valore;
                                        break;
                                }
                            }
                            parametriPDF.Add(_docSpeditiInterop);
                            parametriPDF.Add(_confermeRicevute);
                            parametriPDF.Add(_confermeMancanti);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.CDC_reportControlloPreventivo
                case ReportDisponibili.CDC_reportControlloPreventivo:
                    try
                    {
                        _docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                        ds = _docsPaWS.DO_CDCGetDSReportControlloPreventivo(DocsPAWA.UserManager.getInfoUtente(), CDCDataDa, CDCDataA, CDCUffici, CDCMagistrato, CDCRevisore);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "CDCDataDa":
                                        _CDCDataDa.Descrizione = "@paramDataDa";
                                        _CDCDataDa.Valore = p.Valore;
                                        break;
                                    case "CDCDataA":
                                        _CDCDataA.Descrizione = "@paramDataA";
                                        _CDCDataA.Valore = p.Valore;
                                        break;
                                    case "CDCMagistrato":
                                        _CDCMagistrato.Descrizione = "@paramMagistrato";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCMagistrato.Valore = "Magistrato : " + p.Descrizione;
                                        else
                                            _CDCMagistrato.Valore = string.Empty;
                                        break;
                                    case "CDCRevisore":
                                        _CDCRevisore.Descrizione = "@paramRevisore";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCRevisore.Valore = "Revisore : " + p.Descrizione;
                                        else
                                            _CDCRevisore.Valore = string.Empty;
                                        break;
                                    case "CDCUffici":
                                        _CDCUffici.Descrizione = "@paramUfficio";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCUffici.Valore = "Ufficio : " + p.Descrizione;
                                        else
                                            _CDCUffici.Valore = string.Empty;
                                        break;
                                    case "TOTDEFERIMENTI":
                                        _TOTDEFERIMENTI.Descrizione = "@totdeferimenti";
                                        if (!string.IsNullOrEmpty(p.Valore))
                                            _TOTDEFERIMENTI.Valore = p.Valore;
                                        break;
                                    case "TOTDECRETIESAMINATI":
                                        _TOTDECRETIESAMINATI.Descrizione = "@totdecretiesaminati";
                                        if (!string.IsNullOrEmpty(p.Valore))
                                            _TOTDECRETIESAMINATI.Valore = p.Valore;
                                        else _TOTDECRETIESAMINATI.Valore = "0";
                                        break;
                                }
                            }
                            parametriPDF.Add(_CDCUffici);
                            parametriPDF.Add(_CDCMagistrato);
                            parametriPDF.Add(_CDCRevisore);
                            parametriPDF.Add(_CDCDataDa);
                            parametriPDF.Add(_CDCDataA);
                            parametriPDF.Add(_TOTDEFERIMENTI);
                            parametriPDF.Add(_TOTDECRETIESAMINATI);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;

                #endregion

                #region ReportDisponibili.CDC_reportSuccessivoSCCLA
                //Giordano Iacozzilli: 15/06/2012
                //Modifica Per aggiungere i nuovi report discriminati dal tipo di controllo(Suuc/Prev).
                case ReportDisponibili.CDC_reportSuccessivoSCCLA:
                    try
                    {
                        _docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                        ds = _docsPaWS.DO_CDCGetDSReportControlloSuccessivo(DocsPAWA.UserManager.getInfoUtente(), CDCDataDa, CDCDataA, CDCUffici, CDCMagistrato, CDCRevisore);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "CDCDataDa":
                                        _CDCDataDa.Descrizione = "@paramDataDa";
                                        _CDCDataDa.Valore = p.Valore;
                                        break;
                                    case "CDCDataA":
                                        _CDCDataA.Descrizione = "@paramDataA";
                                        _CDCDataA.Valore = p.Valore;
                                        break;
                                    case "CDCMagistrato":
                                        _CDCMagistrato.Descrizione = "@paramMagistrato";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCMagistrato.Valore = "Magistrato : " + p.Descrizione;
                                        else
                                            _CDCMagistrato.Valore = string.Empty;
                                        break;
                                    case "CDCRevisore":
                                        _CDCRevisore.Descrizione = "@paramRevisore";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCRevisore.Valore = "Revisore : " + p.Descrizione;
                                        else
                                            _CDCRevisore.Valore = string.Empty;
                                        break;
                                    case "CDCUffici":
                                        _CDCUffici.Descrizione = "@paramUfficio";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCUffici.Valore = "Ufficio : " + p.Descrizione;
                                        else
                                            _CDCUffici.Valore = string.Empty;
                                        break;
                                }
                            }
                            parametriPDF.Add(_CDCUffici);
                            parametriPDF.Add(_CDCMagistrato);
                            parametriPDF.Add(_CDCRevisore);
                            parametriPDF.Add(_CDCDataDa);
                            parametriPDF.Add(_CDCDataA);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;

                #endregion

                #region ReportDisponibili.CDC_reportControlloPreventivoSRC
                case ReportDisponibili.CDC_reportControlloPreventivoSRC:
                    try
                    {
                        _docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                        ds = _docsPaWS.DO_CDCGetDSReportControlloPreventivoSRC(DocsPAWA.UserManager.getInfoUtente(), CDCDataDa, CDCDataA, CDCUffici, CDCMagistrato, CDCRevisore);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "CDCDataDa":
                                        _CDCDataDa.Descrizione = "@paramDataDa";
                                        _CDCDataDa.Valore = p.Valore;
                                        break;
                                    case "CDCDataA":
                                        _CDCDataA.Descrizione = "@paramDataA";
                                        _CDCDataA.Valore = p.Valore;
                                        break;
                                    case "CDCMagistrato":
                                        _CDCMagistrato.Descrizione = "@paramMagistrato";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCMagistrato.Valore = "Magistrato : " + p.Descrizione;
                                        else
                                            _CDCMagistrato.Valore = string.Empty;
                                        break;
                                    case "CDCRevisore":
                                        _CDCRevisore.Descrizione = "@paramRevisore";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCRevisore.Valore = "Revisore : " + p.Descrizione;
                                        else
                                            _CDCRevisore.Valore = string.Empty;
                                        break;
                                    case "CDCUffici":
                                        _CDCUffici.Descrizione = "@paramUfficio";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCUffici.Valore = "Ufficio : " + p.Descrizione;
                                        else
                                            _CDCUffici.Valore = string.Empty;
                                        break;
                                    case "TOTDEFERIMENTI":
                                        _TOTDEFERIMENTI.Descrizione = "@totdeferimenti";
                                        if (!string.IsNullOrEmpty(p.Valore))
                                            _TOTDEFERIMENTI.Valore = p.Valore;
                                        break;
                                    case "DESCREGISTRO":
                                        _DESCREGISTRO.Descrizione = "@paramRegistro";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _DESCREGISTRO.Valore = "Ufficio : " + p.Valore;
                                        break;

                                }
                            }
                            parametriPDF.Add(_CDCUffici);
                            parametriPDF.Add(_CDCMagistrato);
                            parametriPDF.Add(_CDCRevisore);
                            parametriPDF.Add(_CDCDataDa);
                            parametriPDF.Add(_CDCDataA);
                            parametriPDF.Add(_TOTDEFERIMENTI);
                            parametriPDF.Add(_DESCREGISTRO);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;

                #endregion

                #region ReportDisponibili.CDC_reportPensioniCivili
                case ReportDisponibili.CDC_reportPensioniCivili:
                    try
                    {
                        _docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                        ds = _docsPaWS.DO_CDCGetDSReportPensioniCivili(DocsPAWA.UserManager.getInfoUtente(), CDCDataDa, CDCDataA, CDCUffici, CDCMagistrato, CDCRevisore);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "CDCDataDa":
                                        _CDCDataDa.Descrizione = "@paramDataDa";
                                        _CDCDataDa.Valore = p.Valore;
                                        break;
                                    case "CDCDataA":
                                        _CDCDataA.Descrizione = "@paramDataA";
                                        _CDCDataA.Valore = p.Valore;
                                        break;
                                    case "CDCMagistrato":
                                        _CDCMagistrato.Descrizione = "@paramMagistrato";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCMagistrato.Valore = "Magistrato : " + p.Descrizione;
                                        else
                                            _CDCMagistrato.Valore = string.Empty;
                                        break;
                                    case "CDCRevisore":
                                        _CDCRevisore.Descrizione = "@paramRevisore";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCRevisore.Valore = "Revisore : " + p.Descrizione;
                                        else
                                            _CDCRevisore.Valore = string.Empty;
                                        break;
                                    case "CDCUffici":
                                        _CDCUffici.Descrizione = "@paramUfficio";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCUffici.Valore = "Ufficio : " + p.Descrizione;
                                        else
                                            _CDCUffici.Valore = string.Empty;
                                        break;
                                }
                            }
                            parametriPDF.Add(_CDCUffici);
                            parametriPDF.Add(_CDCMagistrato);
                            parametriPDF.Add(_CDCRevisore);
                            parametriPDF.Add(_CDCDataDa);
                            parametriPDF.Add(_CDCDataA);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion

                #region ReportDisponibili.CDC_reportPensioniMilitari
                case ReportDisponibili.CDC_reportPensioniMilitari:
                    try
                    {
                        _docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                        ds = _docsPaWS.DO_CDCGetDSReportPensioniMilitari(DocsPAWA.UserManager.getInfoUtente(), CDCDataDa, CDCDataA, CDCUffici, CDCMagistrato, CDCRevisore);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (Parametro p in filtri)
                            {
                                switch (p.Nome)
                                {
                                    case "CDCDataDa":
                                        _CDCDataDa.Descrizione = "@paramDataDa";
                                        _CDCDataDa.Valore = p.Valore;
                                        break;
                                    case "CDCDataA":
                                        _CDCDataA.Descrizione = "@paramDataA";
                                        _CDCDataA.Valore = p.Valore;
                                        break;
                                    case "CDCMagistrato":
                                        _CDCMagistrato.Descrizione = "@paramMagistrato";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCMagistrato.Valore = "Magistrato : " + p.Descrizione;
                                        else
                                            _CDCMagistrato.Valore = string.Empty;
                                        break;
                                    case "CDCRevisore":
                                        _CDCRevisore.Descrizione = "@paramRevisore";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCRevisore.Valore = "Revisore : " + p.Descrizione;
                                        else
                                            _CDCRevisore.Valore = string.Empty;
                                        break;
                                    case "CDCUffici":
                                        _CDCUffici.Descrizione = "@paramUfficio";
                                        if (!string.IsNullOrEmpty(p.Valore) && !string.IsNullOrEmpty(p.Descrizione))
                                            _CDCUffici.Valore = "Ufficio : " + p.Descrizione;
                                        else
                                            _CDCUffici.Valore = string.Empty;
                                        break;
                                }
                            }
                            parametriPDF.Add(_CDCUffici);
                            parametriPDF.Add(_CDCMagistrato);
                            parametriPDF.Add(_CDCRevisore);
                            parametriPDF.Add(_CDCDataDa);
                            parametriPDF.Add(_CDCDataA);
                        }
                        else
                        {
                            /*DataSet vuoto*/
                            return null;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
                #endregion
            }
            #endregion

            #region Creazione del Report
            fileRep = PdfReport.do_MakePdfReport(tipoReport, templateFilePath, ds, parametriPDF);
            #endregion

            return fileRep;
        }
        #endregion

        //Veronica per piani di rientro


        #region DO_GetIdAmmByCodAmm
        /// <summary>
        /// DO_GetIdAmmByCodAmm
        /// </summary>
        /// <param name="codAmm">codice amministrazione</param>
        /// <returns>system_id dell'amministrazione</returns>
        public static int DO_GetIdAmmByCodAmm(string codAmm)
        {
            return _docsPaWS.DO_GetIdAmmByCodAmm(codAmm);
        }
        #endregion

        #region Do_GetVarDescAmmByIdAmm
        /// <summary>
        /// Do_GetVarDescAmmByIdAmm
        /// </summary>
        /// <param name="idAmm">id amministrazione</param>
        /// <returns>descrizione dell'amministrazione</returns>

        public static string Do_GetVarDescAmmByIdAmm(int idAmm)
        {
            return _docsPaWS.Do_GetVarDescAmmByIdAmm(idAmm);
        }

        public static DocsPAWA.DocsPaWR.PR_Amministrazione Do_GetAmmByIdAmm(int idAmm)
        {
            return _docsPaWS.Do_GetAmmByIdAmm(idAmm);
        }

        #endregion

        #region DO_GetDescRegistro
        /// <summary>
        /// DO_GetDescRegistro
        /// </summary>
        /// <param name="idReg">system_id del registro</param>
        /// <returns>descrizione del registro</returns>
        public static string DO_GetDescRegistro(string idReg)
        {
            return _docsPaWS.DO_GetDescRegistro(idReg);
        }

        #endregion

        #region DO_GETMesi
        public static void DO_GETMesi(DropDownList list)
        {
            list.Items.Clear();
            int mese = DateTime.Now.Month;
            string[] mesi = { "", "Gennaio", "Febbraio", "Marzo", "Aprile", "Maggio", "Giugno", "Luglio", "Agosto", "Settembre", "Ottobre", "Novembre", "Dicembre" };

            //			list.Items.Add(new ListItem("Tutti i mesi","0"));
            list.Items.Add(new ListItem("Tutti i mesi", ""));

            if (((ProspettiRiepilogativi_LF)list.Page).ddl_anno.SelectedValue != DateTime.Now.Year.ToString())
            {
                mese = 12; // se l'anno selezionato � diverso dal corrente descrivo nella combo tutti mesi
            }
            for (int i = 1; i <= mese; i++)
            {
                list.Items.Add(new ListItem(mesi[i].ToString(), i.ToString()));
            }

            //list.SelectedIndex = 0;
        }
        #endregion

        #region DO_GetTitolatio
        public static void DO_GetTitolario(ref DropDownList list, string codiceAmm)
        {
            try
            {
                int id_amm = DO_GetIdAmmByCodAmm(codiceAmm);
                foreach (DocsPAWA.DocsPaWR.PR_Titolario tit in _docsPaWS.DO_GetTitolari(id_amm))
                {
                    list.Items.Add(new ListItem(tit.Descrizione, tit.SystemId));
                }

                //   return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public static string DO_GetNumDocSpediti(string dataSpedDa, string dataSpedA, string idReg, string confermaProt, int idAmm)
        {
            string result = string.Empty;
            try
            {
                _docsPaWS.Timeout = System.Threading.Timeout.Infinite;
                result = _docsPaWS.DO_GetNumDocSpediti(dataSpedDa, dataSpedA, idReg, confermaProt, idAmm);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        public static DropDownList DO_GetRegistriCCByAmministrazione(DropDownList list, string cod_amm)
        {
            try
            {
                int id_amm = DO_GetIdAmmByCodAmm(cod_amm);
                ArrayList registri = new ArrayList(_docsPaWS.DO_GetRegistri(id_amm));
                list.Items.Clear();
                list.Items.Add(new ListItem("Tutti", ""));
                foreach (DocsPAWA.DocsPaWR.PR_Registro reg in registri)
                {
                    list.Items.Add(new ListItem(reg.Descrizione, reg.System_id));
                }
                return list;
            }

            catch (Exception ex)
            {
                //errore nel recupero dei dati
                throw ex;
            }

        }


        #region DO_ReadXML
        public static ArrayList DO_ReadXML()
        {
            ArrayList result = new ArrayList(_docsPaWS.DO_ReadXML());
            return result;
        }
        #endregion

        #region DO_AddSubReport
        public static DocsPAWA.DocsPaWR.Report DO_AddSubReport(DocsPAWA.DocsPaWR.Report report, DocsPAWA.DocsPaWR.Report subReport)
        {
            return _docsPaWS.DO_AddSubReport(report, subReport);
        }
        #endregion

        #region DO_NewReport
        public static DocsPAWA.DocsPaWR.Report DO_NewReport(string descrizione, string valore)
        {
            return _docsPaWS.DO_NewReport(descrizione, valore);
        }
        #endregion

        #region DO_NewRegistro
        public static DocsPAWA.DocsPaWR.PR_Registro DO_NewRegistro(string system_id, string codice, string descrizione)
        {
            return _docsPaWS.DO_NewRegistro(system_id, codice, descrizione);
        }
        #endregion

        #region DO_NewAmministrazione
        public static DocsPAWA.DocsPaWR.PR_Amministrazione DO_NewAmministrazione(string system_id, string codice, string descrizione, string libreria)
        {
            return _docsPaWS.DO_NewAmministrazione(system_id, codice, descrizione, libreria);
        }
        #endregion

        #region stampaExcel

        public static DocsPAWA.DocsPaWR.FileDocumento DO_StampaExcel(ArrayList filtri, string timeStamp, DocsPAWA.DocsPaWR.InfoUtente infoUtente, string searchType)
        {
            DocsPAWA.DocsPaWR.FileDocumento fileRep = new DocsPAWA.DocsPaWR.FileDocumento();

            DataSet ds = new DataSet();
            _docsPaWS.Timeout = System.Threading.Timeout.Infinite;
            //ProspettiRiepilogativi.Frontend.Parametro[] temp = (ProspettiRiepilogativi.Frontend.Parametro[])(filtri.ToArray(typeof(ProspettiRiepilogativi.Frontend.Parametro)));
            // DocsPAWA.DocsPaWR.Parametro[] temp = new DocsPAWA.DocsPaWR.Parametro[filtri.Count];
            /* for(int i=0; i<filtri.Count; i++){
                 ProspettiRiepilogativi.Frontend.Parametro tempPar = (ProspettiRiepilogativi.Frontend.Parametro)filtri[i];
                 temp[i] = new DocsPAWA.DocsPaWR.Parametro()
                 {
                     Descrizione = tempPar.Descrizione,
                     Nome = tempPar.Nome,
                     Valore = tempPar.Valore

                 };
             }*/


            string tipo_ricerca = "";
            string tipo_scelta = "";
            string valore_scelta = "";
            bool sottoposti = false;
            string dataC = "";
            string dataCdal = "";
            string dataCal = "";
            string dataCh = "";
            string dataChdal = "";
            string dataChal = "";
            string idTitolario = "";
            string nome_scelta = "";
            string data_creazione_stampa = "";
            string data_creazione_stampa_finale = "";
            string data_chiusura_stampa = "";
            string data_chiusura_stampa_finale = "";

            foreach (Parametro p in filtri)
            {
                //FILTRO TITOLARIO
                if (p.Nome.Equals("titolario_fasc"))
                {
                    idTitolario = p.Valore;
                }
                //FILTRO TIPO DI RICERCA
                if (p.Nome.Equals("tipo_ricerca"))
                {
                    tipo_ricerca = p.Valore;
                }
                //FILTRO UO O RF
                if (p.Nome.Equals("tipo_scelta"))
                {
                    tipo_scelta = p.Descrizione;
                    valore_scelta = p.Valore;
                }
                //NOME UO O RF
                if (p.Nome.Equals("nome_scelta"))
                {
                    nome_scelta = p.Valore;
                }
                //FILTRO SOTTOPOSTI
                if (p.Nome.Equals("sottoposti"))
                {
                    if (p.Valore.Equals("True"))
                    {
                        sottoposti = true;
                    }
                }
                //FILTRO DATA CREAZIONE GIORNO SINGOLO
                if (p.Nome.Equals("dataC"))
                {
                    dataC = p.Valore;
                    if (dataC.Equals("APERTURA_TODAY") || dataC.Equals("APERTURA_MC") || dataC.Equals("APERTURA_SC"))
                    {
                        data_creazione_stampa = p.Descrizione;
                    }
                    else
                    {
                        data_creazione_stampa = dataC;
                    }
                }
                //FILTRO DATA CREAZIONE INIZIALE
                if (p.Nome.Equals("dataCdal"))
                {
                    dataCdal = p.Valore;
                    if (dataCdal.Equals("APERTURA_TODAY") || dataCdal.Equals("APERTURA_MC") || dataCdal.Equals("APERTURA_SC"))
                    {
                        data_creazione_stampa = p.Descrizione;
                    }
                    else
                    {
                        data_creazione_stampa = dataCdal;
                    }
                }
                //FILTRO DATA CREAZIONE FINALE
                if (p.Nome.Equals("dataCal"))
                {
                    dataCal = p.Valore;
                    if (dataCal.Equals("APERTURA_TODAY") || dataCal.Equals("APERTURA_MC") || dataCal.Equals("APERTURA_SC"))
                    {
                        data_creazione_stampa_finale = p.Descrizione;
                    }
                    else
                    {
                        data_creazione_stampa_finale = dataCal;
                    }
                }
                //FILTRO DATA CHIUSURA GIORNO SINGOLO
                if (p.Nome.Equals("dataCh"))
                {
                    dataCh = p.Valore;
                    if (dataCh.Equals("APERTURA_TODAY") || dataCh.Equals("APERTURA_MC") || dataCh.Equals("APERTURA_SC"))
                    {
                        data_chiusura_stampa = p.Descrizione;
                    }
                    else
                    {
                        data_chiusura_stampa = dataCh;
                    }
                }
                //FILTRO DATA CHIUSURA INIZIALE
                if (p.Nome.Equals("dataChdal"))
                {
                    dataChdal = p.Valore;
                    if (dataChdal.Equals("APERTURA_TODAY") || dataChdal.Equals("APERTURA_MC") || dataChdal.Equals("APERTURA_SC"))
                    {
                        data_chiusura_stampa = p.Descrizione;
                    }
                    else
                    {
                        data_chiusura_stampa = dataChdal;
                    }
                }
                //FILTRO DATA CHIUSURA FINALE
                if (p.Nome.Equals("dataChal"))
                {
                    dataChal = p.Valore;
                    if (dataChal.Equals("APERTURA_TODAY") || dataChal.Equals("APERTURA_MC") || dataChal.Equals("APERTURA_SC"))
                    {
                        data_chiusura_stampa_finale = p.Descrizione;
                    }
                    else
                    {
                        data_chiusura_stampa_finale = dataChal;
                    }
                }
            }

            ds = _docsPaWS.DO_StampaReportFascExcel(timeStamp, infoUtente, searchType, tipo_ricerca, tipo_scelta, valore_scelta, sottoposti, dataC, dataCdal, dataCal, dataCh, dataChdal, dataChal, idTitolario);

            // fileRep = PdfReport.do_MakePdfReport(tipoReport, templateFilePath, ds, parametriPDF);
            //fileRep = creaReportExcelFascicoli(searchType ,ds);
            #region esporta in excel
            ExcelReportGenerator generator = new ExcelReportGenerator();


            string TITOLO = "nome_ricerca";
            string RISULTATO = "nome_risultato";
            string SFONDO_GRIGIO = "greyBackground";
            string DESCRIZIONERUOLI = "descrizione ruoli";
            string SFONDO_GRIGIO_BORDO = "sfondo bordo";

            // Creazione stili
            generator.AddStringStyle(SFONDO_GRIGIO, "Arial", 10, "#000000", "#C0C0C0", false);
            generator.AddStringStyle(RISULTATO, "Arial", 10, "#000000", false);
            generator.AddStringStyleBorder(DESCRIZIONERUOLI, "Arial", 10, "#000000", "", false, true);
            generator.AddStringStyleBorderItalic(TITOLO, "Arial", 10, "#000000", "", true, true);
            generator.AddStringStyleBorder(SFONDO_GRIGIO_BORDO, "Arial", 10, "#000000", "#C0C0C0", false, true);
            // Creazione sheet
            if (tipo_ricerca.Equals("reportNumFasc"))
            {
                generator.CreateSheet("Conteggio Fascicoli");
            }
            else
            {
                generator.CreateSheet("Conteggio Documenti");
            }

            generator.AddRow();
            generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 3);
            generator.AddRow();

            generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "Ruoli appartenenti alla UO", 3);
            generator.AddCell(XmlExcelHelper.CellType.String, "");
            generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "Sottoposti", 0);

            generator.AddRow();
            generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 3);
            generator.AddRow();

            if (tipo_scelta.Equals("uo"))
            {
                generator.AddCell(XmlExcelHelper.CellType.String, DESCRIZIONERUOLI, nome_scelta, 3);
                generator.AddCell(XmlExcelHelper.CellType.String, "");
                if (sottoposti == false)
                {
                    generator.AddCell(XmlExcelHelper.CellType.String, SFONDO_GRIGIO, "NO", 0);
                }
                else
                {
                    generator.AddCell(XmlExcelHelper.CellType.String, DESCRIZIONERUOLI, "SI", 0);
                }
            }
            else
            {
                generator.AddCell(XmlExcelHelper.CellType.String, SFONDO_GRIGIO, "", 3);
                generator.AddCell(XmlExcelHelper.CellType.String, "");
                generator.AddCell(XmlExcelHelper.CellType.String, SFONDO_GRIGIO, "NO", 0);
            }

            generator.AddRow();
            generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 3);
            generator.AddRow();

            generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "Ruoli appartenenti all'RF", 3);

            generator.AddRow();
            generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 3);

            generator.AddRow();

            if (tipo_scelta.Equals("uo"))
            {
                generator.AddCell(XmlExcelHelper.CellType.String, SFONDO_GRIGIO, "", 3);
            }
            else
            {
                generator.AddCell(XmlExcelHelper.CellType.String, DESCRIZIONERUOLI, nome_scelta, 3);
            }

            generator.AddRow();
            generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 3);
            generator.AddRow();
            generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 3);
            generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "DAL", 0);
            generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 0);
            generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "AL", 0);

            generator.AddRow();
            generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "Data creazione", 3);
            if (string.IsNullOrEmpty(dataC) && string.IsNullOrEmpty(dataCal) && string.IsNullOrEmpty(dataCdal))
            {
                generator.AddCell(XmlExcelHelper.CellType.String, SFONDO_GRIGIO_BORDO, "", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, SFONDO_GRIGIO_BORDO, "", 0);
            }
            else
            {
                if (!string.IsNullOrEmpty(dataC))
                {
                    generator.AddCell(XmlExcelHelper.CellType.String, DESCRIZIONERUOLI, data_creazione_stampa, 0);
                    generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 0);
                    generator.AddCell(XmlExcelHelper.CellType.String, DESCRIZIONERUOLI, data_creazione_stampa, 0);
                }
                else
                {
                    generator.AddCell(XmlExcelHelper.CellType.String, DESCRIZIONERUOLI, data_creazione_stampa, 0);
                    generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 0);
                    generator.AddCell(XmlExcelHelper.CellType.String, DESCRIZIONERUOLI, data_creazione_stampa_finale, 0);
                }
            }

            //DATA CHIUSURA
            generator.AddRow();
            generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "Data chiusura", 3);
            if (string.IsNullOrEmpty(dataCh) && string.IsNullOrEmpty(dataChal) && string.IsNullOrEmpty(dataChdal))
            {
                generator.AddCell(XmlExcelHelper.CellType.String, SFONDO_GRIGIO_BORDO, "", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, SFONDO_GRIGIO_BORDO, "", 0);
            }
            else
            {
                if (!string.IsNullOrEmpty(dataCh))
                {
                    generator.AddCell(XmlExcelHelper.CellType.String, DESCRIZIONERUOLI, data_chiusura_stampa, 0);
                    generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 0);
                    generator.AddCell(XmlExcelHelper.CellType.String, DESCRIZIONERUOLI, data_chiusura_stampa, 0);
                }
                else
                {
                    generator.AddCell(XmlExcelHelper.CellType.String, DESCRIZIONERUOLI, data_chiusura_stampa, 0);
                    generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 0);
                    generator.AddCell(XmlExcelHelper.CellType.String, DESCRIZIONERUOLI, data_chiusura_stampa_finale, 0);
                }
            }

            if (tipo_ricerca.Equals("reportNumFasc"))
            {
                generator.AddRow();
                generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 3);

                generator.AddRow();
                generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "Descrizione Ruolo", 3);
                generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "N� fascicoli creati", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "N� fascicoli chiusi", 0);

                string tempNum = "";
                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string system_id_ric = row["SYSTEM_ID"].ToString();
                        string desc_ruolo = row["VAR_DESC_CORR"].ToString();
                        string stato_fasc = row["CHA_STATO"].ToString();
                        string count_fasc = row["COUNT(*)"].ToString();

                        if (!system_id_ric.Equals(tempNum))
                        {
                            generator.AddRow();
                            generator.AddCell(XmlExcelHelper.CellType.String, DESCRIZIONERUOLI, desc_ruolo, 3);
                            tempNum = system_id_ric;
                        }

                        if (stato_fasc.Equals("A"))
                        {
                            generator.AddCell(XmlExcelHelper.CellType.Number, DESCRIZIONERUOLI, count_fasc, 0);
                        }
                        else
                        {
                            generator.AddCell(XmlExcelHelper.CellType.Number, DESCRIZIONERUOLI, count_fasc, 0);
                        }
                    }
                }

                generator.AddRow();
                generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 3);
                generator.AddRow();

            }
            else
            {
                //EXPORT CONTEGGIO DOCUMENTI IN FASCICOLO

                generator.AddRow();
                generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 3);

                generator.AddRow();
                generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "Descrizione Ruolo", 3);
                generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "Codice fascicolo", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "Descrizione fascicolo", 1);
                generator.AddCell(XmlExcelHelper.CellType.String, TITOLO, "N� documenti", 0);

                string tempNum = "";
                string tema = "";

                if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string system_id_ric = row["SYSTEM_ID"].ToString();
                        string desc_ruolo = row["VAR_DESC_CORR"].ToString();
                        string codice_fasc = row["VAR_CODICE"].ToString();
                        string count_fasc_doc = row["COUNT(*)"].ToString();
                        string fasc_desc = row["DESCRIPTION"].ToString();

                        generator.AddRow();

                        if (!system_id_ric.Equals(tempNum))
                        {
                            generator.AddCell(XmlExcelHelper.CellType.String, SFONDO_GRIGIO, desc_ruolo, 3);
                            tempNum = system_id_ric;
                            tema = SFONDO_GRIGIO;
                        }
                        else
                        {
                            generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 3);
                            tema = DESCRIZIONERUOLI;
                        }

                        generator.AddCell(XmlExcelHelper.CellType.String, tema, codice_fasc, 0);
                        generator.AddCell(XmlExcelHelper.CellType.String, tema, fasc_desc, 1);
                        generator.AddCell(XmlExcelHelper.CellType.Number, tema, count_fasc_doc, 0);

                    }
                }

                generator.AddRow();
                generator.AddCell(XmlExcelHelper.CellType.String, RISULTATO, "", 3);
                generator.AddRow();





            }

            using (System.IO.MemoryStream stream = new MemoryStream())
            {
                generator.SaveDocument(stream);

                stream.Position = 0;

                fileRep.name = string.Format("StampaReport_{0}.xml", DateTime.Now.ToString());
                fileRep.path = string.Empty;
                fileRep.fullName = fileRep.name;
                fileRep.contentType = "application/vnd.ms-excel";
                fileRep.content = new byte[stream.Length];

                stream.Read(fileRep.content, 0, fileRep.content.Length);
            }

            #endregion

            return fileRep;



        }
        #endregion

        #region esportazioneLog

        public static DocsPAWA.DocsPaWR.FileDocumento DO_StampaLog(ArrayList filtri, DocsPAWA.DocsPaWR.InfoUtente infoUtente)
        {
            DocsPAWA.DocsPaWR.FileDocumento fileRep = new DocsPAWA.DocsPaWR.FileDocumento();

            DataSet ds = new DataSet();
            _docsPaWS.Timeout = System.Threading.Timeout.Infinite;


            string data_inizio_iop = "";
            string data_fine_iop = "";
            string periodo_ipa = "";
            bool sottoposti = false;
            string ampiezza_ipa = "";
            string numero_ampiezza_ipa = "";
            string iagg = "";
            string ruolo = "";
            string uo = "";
            string rf = "";
            string aoo = "";
            string evento = "";
            string evento_secondario = "";
            string idAmministrazione = "";
            string descrizioneFinale = "";
            string descrizione_evento_secondario = "";
            string tipo_ruolo = "";

            foreach (Parametro p in filtri)
            {

                if (p.Nome.Equals("data_inizio_iop"))
                {
                    data_inizio_iop = p.Valore;
                }
                if (p.Nome.Equals("data_fine_iop"))
                {
                    data_fine_iop = p.Valore;
                }
                if (p.Nome.Equals("periodo_ipa"))
                {
                    periodo_ipa = p.Valore;
                }
                if (p.Nome.Equals("ampiezza_ipa"))
                {
                    ampiezza_ipa = p.Valore;
                }
                if (p.Nome.Equals("numero_ampiezza_ipa"))
                {
                    numero_ampiezza_ipa = p.Valore;
                }
                if (p.Nome.Equals("sottoposti"))
                {
                    if (p.Valore.Equals("true"))
                    {
                        sottoposti = true;
                    }
                }
                if (p.Nome.Equals("iagg"))
                {
                    iagg = p.Valore;
                }
                if (p.Nome.Equals("ruolo"))
                {
                    ruolo = p.Valore;
                }
                if (p.Nome.Equals("uo"))
                {
                    uo = p.Valore;
                }
                if (p.Nome.Equals("rf"))
                {
                    rf = p.Valore;
                }
                if (p.Nome.Equals("aoo"))
                {
                    aoo = p.Valore;
                }
                if (p.Nome.Equals("tp"))
                {
                    tipo_ruolo = p.Valore;
                }
                if (p.Nome.Equals("evento"))
                {
                    evento = p.Valore;
                }
                if (p.Nome.Equals("evento_secondario"))
                {
                    evento_secondario = p.Valore;
                    descrizione_evento_secondario = p.Descrizione;
                }
                if (p.Nome.Equals("idAmministrazione"))
                {
                    idAmministrazione = p.Valore;
                }
                if (p.Nome.Equals("descrizioneFinale"))
                {
                    descrizioneFinale = p.Valore;
                }
            }

            ds = _docsPaWS.DO_StampaExportLogExcel(infoUtente, data_inizio_iop, data_fine_iop, periodo_ipa, sottoposti, ampiezza_ipa, numero_ampiezza_ipa, iagg, ruolo, uo, rf, aoo, evento, evento_secondario, idAmministrazione, descrizione_evento_secondario, tipo_ruolo);

            if (ds.Tables[0].Rows.Count > 0)
            {
                ExcelReportGenerator generator = new ExcelReportGenerator();

                generator.CreateSheet("Exp log");
                //SCRIVI EXCEL LOG
                string NORMALE = "normale";
                string BORDO_SOTTO = "bordo_sotto";
                string BORDO_DESTRA = "bordo_destra";

                // Creazione stili
                generator.AddStringStyle(NORMALE, "Arial", 10, "#000000", false);
                generator.AddStringStyleBorderRight(BORDO_DESTRA, "Arial", 10, "#000000", "", false, true);
                generator.AddStringStyleBorder(BORDO_SOTTO, "Arial", 10, "#000000", "", false, true);

                generator.AddRow();
                generator.AddCell(XmlExcelHelper.CellType.String, BORDO_SOTTO, descrizioneFinale, 0);

                generator.AddRow();
                generator.AddCell(XmlExcelHelper.CellType.String, NORMALE, "", 0);

                generator.AddRow();
                generator.AddCell(XmlExcelHelper.CellType.String, BORDO_SOTTO, "Data inizio", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, NORMALE, "", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, BORDO_SOTTO, data_inizio_iop, 0);

                generator.AddRow();
                generator.AddCell(XmlExcelHelper.CellType.String, BORDO_SOTTO, "Data fine", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, NORMALE, "", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, BORDO_SOTTO, data_fine_iop, 0);

                generator.AddRow();
                generator.AddCell(XmlExcelHelper.CellType.String, BORDO_SOTTO, "Evento", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, NORMALE, "", 0);

                if (string.IsNullOrEmpty(evento))
                {
                    generator.AddCell(XmlExcelHelper.CellType.String, BORDO_SOTTO, "Tutti gli eventi", 0);
                }
                else
                {
                    if (!string.IsNullOrEmpty(evento_secondario))
                    {
                        generator.AddCell(XmlExcelHelper.CellType.String, BORDO_SOTTO, descrizione_evento_secondario, 0);
                    }
                    else
                    {
                        generator.AddCell(XmlExcelHelper.CellType.String, BORDO_SOTTO, "Tutti gli eventi che riguardano " + evento, 0);
                    }
                }

                generator.AddRow();

                generator.AddRow();
                generator.AddCell(XmlExcelHelper.CellType.String, NORMALE, "", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, BORDO_SOTTO, "Progressivo intervallo", 0);
                generator.AddCell(XmlExcelHelper.CellType.String, BORDO_SOTTO, "Conteggio eventi", 0);

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string intervallo = row["INTERVALLO"].ToString();
                    string numero = row["COUNT(*)"].ToString();

                    if (!iagg.Equals("0") && !iagg.Equals("1"))
                    {
                        intervallo = intervallo.Substring(0, 10);
                    }

                    generator.AddRow();
                    generator.AddCell(XmlExcelHelper.CellType.String, NORMALE, "", 0);
                    generator.AddCell(XmlExcelHelper.CellType.String, BORDO_DESTRA, intervallo.ToString(), 0);
                    generator.AddCell(XmlExcelHelper.CellType.Number, NORMALE, numero.ToString(), 0);
                }

                using (System.IO.MemoryStream stream = new MemoryStream())
                {
                    generator.SaveDocument(stream);

                    stream.Position = 0;

                    fileRep.name = string.Format("EsportaLog_{0}.xml", DateTime.Now.ToString());
                    fileRep.path = string.Empty;
                    fileRep.fullName = fileRep.name;
                    fileRep.contentType = "application/vnd.ms-excel";
                    fileRep.content = new byte[stream.Length];

                    stream.Read(fileRep.content, 0, fileRep.content.Length);
                }
            }
            else
            {
                fileRep = null;
            }

            return fileRep;
        }

        #endregion

    }
}