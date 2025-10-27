// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using DocsPaVO.utente.Repertori.RequestAndResponse;
using System.Collections;
using DocsPaVO.Conservazione;
using DocsPaVO.amministrazione;
using System.Data;
using DocsPaVO.Interoperabilita.Semplificata;
using System.ServiceModel;
using DocsPaVO.fascicolazione;
using System.Xml.Serialization;
using SoapCore.ServiceModel;
using Microsoft.AspNetCore.Mvc;
using Pi3.Core.Services.Configuration;
using ProspettiRiepilogativi;

namespace DocsPaWS
{
    [ServiceContract(Namespace = "http://localhost")]
    public interface IDocsPaWS
    {
        [OperationContract]
        int DO_GetIdAmmByCodAmm(string codAmm);

        [OperationContract]
        void deleteConfigurazioneCache(string idAmministrazione);

        [OperationContract]
        DocsPaVO.Caching.CacheConfig getConfigurazioneCache(string idAmministrazione);

        [OperationContract]
        bool AmmCheckSessionID(string sessionID);

        [OperationContract]
        Policy GetPolicyById(string idPolicy);

        [OperationContract]
        DocsPaVO.utente.Registro[] GetRfByIdAmm(int idAmministrazione, string tipo);

        [OperationContract]
        string GetIdRuoloRespConservazione(string idAmm, string idAOO = null);

        [OperationContract]
        string GetIdUtenteRespConservazione(string idAmm, string idAOO);

        [OperationContract]
        bool GetStatoAttivazione(string idAmm);

        [OperationContract]
        //ArrayList GetListaPolicyPARER(string idAmm, string tipo);
        IList<DocsPaVO.Conservazione.PARER.PolicyPARER> GetListaPolicyPARER(string idAmm, string tipo);

        [OperationContract]
        bool SetStatoAttivazione(string idAmm, string stato);

        [OperationContract]
        bool IsLogAttivatoAlert(string idAmm, string codice);

        [OperationContract]
        DocsPaVO.amministrazione.AlertConservazione GetAlertValues(string idAmm);

        [OperationContract]
        bool SaveAlertValues(string idAmm, DocsPaVO.amministrazione.AlertConservazione param);

        [OperationContract]
        bool SetMailStruttura(string idAmm, DocsPaVO.Conservazione.PARER.Mailbox mail);

        [OperationContract]
        DocsPaVO.Conservazione.PARER.Mailbox GetMailStruttura(string idAmm);

        [OperationContract]
        DocsPaVO.Conservazione.PARER.Report.ReportMonitoraggioPolicyResponse ReportMonitoraggioPolicy(DocsPaVO.Conservazione.PARER.Report.ReportMonitoraggioPolicyRequest request);

        [OperationContract]
        DocsPaVO.FormatiDocumento.SupportedFileType[] GetSupportedFileTypesPreservation(int idAmministrazione);

        [OperationContract]
        bool InsertNewPolicyPARER(DocsPaVO.Conservazione.PARER.PolicyPARER policy, InfoUtenteAmministratore utente);

        [OperationContract]
        bool UpdatePolicyPARER(DocsPaVO.Conservazione.PARER.PolicyPARER policy, InfoUtenteAmministratore utente);

        [OperationContract]
        string GetCountDocumentiFromPolicy(DocsPaVO.Conservazione.PARER.PolicyPARER policy, string idAmm);

        [OperationContract]
        DocsPaVO.DiagrammaStato.DiagrammaStato getDiagrammaById(string idDiagramma);

        [OperationContract]
        DocsPaVO.Conservazione.PARER.PolicyPARER GetPolicyPARERById(string idPolicy);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamicaLite.TemplateLite[] GetTypeDocumentsWithDiagramByIdAmm(int idAmministrazione, string type);

        [OperationContract]
        //ArrayList getTitolariUtilizzabili(string idAmministrazione);
        IList<OrgTitolario> getTitolariUtilizzabili(string idAmministrazione);

        [OperationContract]
        DocsPaVO.fascicolazione.Fascicolo[] GetFascicoloDaCodiceNoSecurityConservazione(string codiceFasc, string idAmm, string idTitolario, bool soloGenerali, bool isRicFasc, string idRegistro);

        [OperationContract]
        bool InserisciPolicyConservazione(DocsPaVO.Conservazione.Policy policy);

        [OperationContract]
        bool SvuotaCachePolicy(string idAmm, string tipo);

        [OperationContract]
        bool ModifyPolicyConservazione(DocsPaVO.Conservazione.Policy policy);

        [OperationContract]
        RegisteredRegistriRepertorioResponse GetRegisteredRegistries(RegisteredRegistriRepertorioRequest request);

        [OperationContract]
        RegistroRepertorioSettingsResponse GetRegisterSettings(RegistroRepertorioSettingsRequest request);

        [OperationContract]
        Utente[] getUserInRoleByIdCorrGlobali(string idCorrGlobale);

        [OperationContract]
        bool SavePeriodPolicy(DocsPaVO.Conservazione.Policy policy);

        [OperationContract]
        Policy[] GetListaPolicy(int idAmm, string tipo);

        [OperationContract]
        bool DeletePolicy(string idPolicy);

        [OperationContract]
        DocsPaVO.Conservazione.PARER.EsecuzionePolicy GetInfoEsecuzionePolicy(string idPolicy, string tipo);

        [OperationContract]
        bool DeletePolicyPARER(string idPolicy, InfoUtenteAmministratore utente);

        [OperationContract]
        bool UpdateStatoPolicy(ArrayList lista, InfoUtenteAmministratore utente);

        [OperationContract]
        bool SaveRuoloRespConservazione(string idGroup, string idUser, string idAmm);

        [OperationContract]
        string GetStampaRegistroValues(string idAmm);

        [OperationContract]
        bool SaveStampaRegistroValues(string idAmm, string disabled, string printFreq, string printHour);

        [OperationContract]
        void WriteLog(InfoUtenteAmministratore infoUtente, string WebMethodName, string ID_Oggetto, string Var_desc_Oggetto, bool esitoOperazione);

        [OperationContract]
        string GetStampaRegistroOraStampa(string idAmm);

        [OperationContract]
        string getIdAmmByCod(string codiceAmministrazione);

        [OperationContract]
        DocsPaVO.LibroFirma.ProcessoFirma[] GetProcessiDiFirmaByIdAmm(string idAmministrazione);

        [OperationContract]
        bool IsConsolidationEnabled();

        [OperationContract]
        DocsPaVO.FormatiDocumento.SupportedFileType[] GetSupportedFileTypes(int idAmministrazione);

        [OperationContract]
        DocsPaVO.amministrazione.OrgTipoFunzione AmmGetTipoFunzioneByCod(string codice, bool fillFunzioniElementari);

        [OperationContract]
        DocsPaVO.amministrazione.OrgFunzioneAnagrafica AmmGetFunzioneAnagrafica(string codice);

        [OperationContract]
        DocsPaVO.documento.FileDocumento AmmGetReportFunzioni(string tipoReport, string formato, string idFunzione, string idAmm);

        [OperationContract]
        DocsPaVO.amministrazione.OrgTipoFunzione[] AmmGetTipiFunzione(bool fillFunzioniElementari, string idAmm);

        [OperationContract]
        DocsPaVO.amministrazione.OrgFunzione[] AmmGetFunzioni(string idTipoFunzoine);

        [OperationContract]
        DocsPaVO.amministrazione.OrgFunzioneAnagrafica[] AmmGetFunzioniAnagrafica();

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmInsertTipoFunzione(ref DocsPaVO.amministrazione.OrgTipoFunzione tipoFunzione);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmUpdateTipoFunzione(ref DocsPaVO.amministrazione.OrgTipoFunzione tipoFunzione);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmDeleteTipoFunzione(DocsPaVO.amministrazione.OrgTipoFunzione tipoFunzione);

        [OperationContract]
        //ArrayList getListaTemi();
        IList<string> getListaTemi();

        [OperationContract]
        string getCssAmministrazione(string idAmm);

        [OperationContract]
        string getSegnAmm(string idAmm);

        [OperationContract]
        string getpath(string type);

        [OperationContract]
        bool setTemaAmministrazione(string idAmm, int valore);

        [OperationContract]
        bool setColoreSegnatura(string idAmm, int valore);

        [OperationContract]
        void clearHashTableChiaviConfig(string idAmm);

        [OperationContract]
        bool EnableJavaAppletOption();

        [OperationContract]
        string isEnableProtocolloTitolario();

        [OperationContract]
        bool isEnableRiferimentiMittente();

        [OperationContract]
        //DocsPaVO.Modelli.ModelProcessorInfo[] GetModelProcessors(DocsPaVO.utente.InfoUtente infoUtente);
        public DocsPaVO.Modelli.ModelProcessorInfo[] GetModelProcessors(InfoUtenteAmministratore infoUtente);

        [OperationContract]
        bool testConnessione(OrgRegistro.MailRegistro mailRegistro, string tipoConnessione, out string messaggioDiErrore);

        [OperationContract]
        string VerificaDocErrati(string idAmm);

        [OperationContract]
        DocsPaVO.amministrazione.VisualizzaStatoDoc GetInfoDocument(string docnumber, string numProto, string anno, string idAmm, string codiceRegistro);

        [OperationContract]
        DocsPaVO.LibroFirma.IstanzaProcessoDiFirma GetIstanzaProcessoDiFirmaByIdIstanzaProcesso(string idIstanzaProcesso, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.Import.Pregressi.EsitoImportPregressi importPregresso(InfoUtenteAmministratore infoUtente, byte[] dati, bool isAdministration);

        [OperationContract]
        bool asyncImportPregresso(InfoUtenteAmministratore infoUtente, DocsPaVO.Import.Pregressi.EsitoImportPregressi esitoPregressi, string descrizione);

        [OperationContract]
        DocsPaVO.documento.FileDocumento ExportReportExcel(List<DocsPaVO.Import.Pregressi.ItemReportPregressi> repInErr);

        [OperationContract]
        DocsPaVO.Import.Pregressi.ReportPregressi GetReportPregressi(string sysId, bool getItems);

        [OperationContract]
        DocsPaVO.documento.FileDocumento ExportPregressiExcel(DocsPaVO.Import.Pregressi.ReportPregressi report, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.utente.Registro GetRegistroBySistemId(string idRegistro);

        [OperationContract]
        DocsPaVO.Import.Pregressi.ReportPregressi[] GetReports(bool getItems, InfoUtenteAmministratore infoUtente, bool daAmministrazione);

        [OperationContract]
        bool DeleteReportById(string idReport);

        [OperationContract]
        Disservizio getInfoDisservizio();

        [OperationContract]
        bool updateDisservizio(Disservizio disservizio);

        [OperationContract]
        bool creaDisservizio(Disservizio disservizio);

        [OperationContract]
        bool setStatoNotificaDisservizio(string systemId, string stato);

        [OperationContract]
        bool cambiaStatoDisservizio(string stato, string system_id);

        [OperationContract]
        bool deleteDisservizio(string system_id);

        [OperationContract]
        DataSet getListeDistribuzioneAmm(string idAmm);

        [OperationContract]
        string getCodiceLista(string idLista);

        [OperationContract]
        DataSet getCorrispondentiLista(string codiceLista);

        [OperationContract]
        void deleteListaDistribuzione(string codiceLista);

        [OperationContract]
        string getNomeLista(string codiceLista, string idAmm);

        [OperationContract]
        bool isUniqueNomeLista(string nomeLista, string idAmm);


        [OperationContract]
        void modificaLista(object dsCorrLista, string idLista, string nomeLista, string codiceLista);

        [OperationContract]
        bool isUniqueCod(string codLista, string idAmm);

        [OperationContract]
        void salvaLista(object dsCorrLista, string nomeLista, string codiceLista, string idUtente, string idAmm);

        [OperationContract]
        string ExportAmministrazioni();

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione LoginAmministratoreProfilato(DocsPaVO.utente.UserLogin userLogin, bool forceLogin, out DocsPaVO.amministrazione.InfoUtenteAmministratore datiAmministratore);

        [OperationContract]
        DocsPaVO.amministrazione.InfoAmministrazione AmmGetInfoAmmCorrente(string idAmm);

        [OperationContract]
        DocsPaVO.amministrazione.InfoAmministrazione GetInfoAmmAppartenenzaUtente(string userid, string pwd);

        [OperationContract]
        //ArrayList AmmGetListMenuUtente(string idAmm, string idCorrGlob);
        IList<Menu> AmmGetListMenuUtente(string idAmm, string idCorrGlob);

        [OperationContract]
        string GetXMLLog(string codAmm);

        [OperationContract]
        bool SetXMLLog(string stream, string codAmm, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        string VerificaArchivioLogPath();

        [OperationContract]
        string ContaLog(string codAmm, string type);

        [OperationContract]
        bool StampaPDFLog(string codAmm, string type);

        [OperationContract]
        string[] GetFilesLog(string codAmm);

        [OperationContract]
        Assertion[] GetListAssertion(string idAmm);

        [OperationContract]
        int InsertAssertionEvent(Assertion newAssertion);

        [OperationContract]
        int UpdateAssertionEvent(Assertion assertion);

        [OperationContract]
        DocsPaVO.documento.FileDocumento ExportLog(string codAmm, string type, string exportType, string title, string user, string data_a, string data_da, string oggetto, string azione, string esito, int tabelle);

        [OperationContract]
        string findRecords(string idAmm);

        [OperationContract]
        string GetXMLLogFiltrato(string dataDa, string dataA, string user, string oggetto, string azione, string codAmm, string esito, string type, int table);

        [OperationContract]
        string GetXMLLogAmm(string codAmm);

        [OperationContract]
        //ArrayList AmmListaMezzoSpedizione(string idAmm, bool vediTutti);
        IList<MezzoSpedizione> AmmListaMezzoSpedizione(string idAmm, bool vediTutti);

        [OperationContract]
        DocsPaVO.amministrazione.MezzoSpedizione AmmGetMezzoSpedizione(string idMezzoSpedizione);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmInsertMezzoSpedizione(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.MezzoSpedizione m_sped, string idAmm);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmUpdateMezzoSpedizione(ref DocsPaVO.amministrazione.MezzoSpedizione m_sped);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmDeleteMezzoSpedizione(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.MezzoSpedizione m_sped, string idAmm);

        [OperationContract]
        //ArrayList getModelliByDdlAmmPaging(string idAmm, int nPagina, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca, out int numTotPag);
        IList<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione> getModelliByDdlAmmPaging(string idAmm, int nPagina, DocsPaVO.filtri.FiltroRicerca[] filtriRicerca, out int numTotPag);

        [OperationContract]
        string getModelloSystemId();

        [OperationContract]
        string getSeRegistroObbl(string idAmm, string nome);

        [OperationContract]
        string salvaModello(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione modelloTrasmissione, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        void CancellaModello(string idAmm, string idModello);

        [OperationContract]
        bool existRf(string idAmministrazione);

        [OperationContract]
        string getNews(string idAmm);

        [OperationContract]
        bool setNews(string idAmm, string news, bool enable_news);

        [OperationContract]
        bool importaOggettario(byte[] dati, string nomeFile, string codiceAmm, bool update, ref int oggInseriti, ref int oggAggiornati, ref int oggErrati);

        [OperationContract]
        //ArrayList getLogImportOggettario();
        IList<string> getLogImportOggettario();

        [OperationContract]
        EsitoOperazione CopyVisibility(InfoUtenteAmministratore infoUtente, DocsPaVO.Security.CopyVisibility copyVisibility);

        [OperationContract]
        //ArrayList GetExtApplications();
        IList<ExtApplication> GetExtApplications();

        [OperationContract]
        DocsPaVO.utente.Utente getUtenteById(string idPeople);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo ExtAppDeleteUte(string idApplicazione, string idUtente);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo ExtAppAddUte(string idApplicazione, string idUtente);

        [OperationContract]
        DocsPaVO.utente.Corrispondente AddressbookGetCorrispondenteBySystemId(string system_id);
        
        [OperationContract]
        DocsPaVO.utente.Corrispondente AddressbookGetCorrispondenteBySystemIdDisabled(string system_id);
       
        [OperationContract]
        DocsPaVO.utente.Ruolo getRuoloByIdGruppo(string idGruppo);

        [OperationContract]
        List<DocsPaVO.Qualifica.Qualifica> GetQualifiche(int id_amm);

        [OperationContract]
        List<DocsPaVO.Qualifica.PeopleGroupsQualifiche> GetPeopleGroupsQualifiche(String idAmm, String idUo, String idGruppo, String idPeople);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo InsertPeopleGroupsQual(DocsPaVO.Qualifica.PeopleGroupsQualifiche pgq);

        [OperationContract]
        int GetCountProcessiDiFirmaByUtenteTitolare(string idUtenteCoinvolto, string idRuoloCoinvolto);

        [OperationContract]
        int GetCountIstanzeProcessiDiFirmaByUtenteCoinvolto(string idUtenteCoinvolto, string idRuoloCoinvolto);

        [OperationContract]
        bool InvalidaPassiCorrelatiTitolare(string idRuolo, string idPeople, string tipoTick, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        //ArrayList AmmGetListRegistriAssRuolo(string idAmm, string idRuolo);
        IList<OrgRegistro> AmmGetListRegistriAssRuolo(string idAmm, string idRuolo);

        [OperationContract]
        bool importOrganigramma(InfoUtenteAmministratore infoUtente, byte[] dati);

        [OperationContract]
        DocsPaVO.RubricaComune.ElementoRubricaUO GetElementoRubricaUO(InfoUtenteAmministratore infoUtente, string idUo);

        [OperationContract]
        DocsPaVO.RubricaComune.ElementoRubricaRF GetElementoRubricaRF(InfoUtenteAmministratore infoUtente, String idRf);

        [OperationContract]
        //System.Collections.ArrayList AmmGetRegistri(string codiceAmministrazione, string chaRF);
        IList<OrgRegistro> AmmGetRegistri(string codiceAmministrazione, string chaRF);

        [OperationContract]
        DocsPaVO.amministrazione.CasellaRegistro[] AmmGetMailRegistro(string idRegistro);

        [OperationContract]
        //ArrayList getLogImportOrganigramma();
        IList<string> getLogImportOrganigramma();

        [OperationContract]
        //ArrayList AmmGetListUO(string idParent, string livello, string idAmm);
        IList<OrgUO> AmmGetListUO(string idParent, string livello, string idAmm);

        [OperationContract]
        //ArrayList AmmListaIDParentRicerca(string IDPartenza, string tipo);
        IList<int> AmmListaIDParentRicerca(string IDPartenza, string tipo);

        [OperationContract]
        //ArrayList AmmGetListRuoliUO(string idUO);
        IList<OrgRuolo> AmmGetListRuoliUO(string idUO);

        [OperationContract]
        //ArrayList AmmRicercaInOrg(string tipo, string codice, string descrizione, string idAmm, bool searchHistoricized, bool searchByCodeExact);
        IList<OrgRisultatoRicerca> AmmRicercaInOrg(string tipo, string codice, string descrizione, string idAmm, bool searchHistoricized, bool searchByCodeExact);

        [OperationContract]
        //ArrayList AmmGetListTipiRuolo(string idAmm);
        IList<OrgTipoRuolo> AmmGetListTipiRuolo(string idAmm);

        [OperationContract]
        DocsPaVO.amministrazione.OrgDettagliGlobali AmmGetDatiStampaBuste(string idCorrGlob);

        [OperationContract]
        //ArrayList AmmGetListFunzioni(string idAmm, string idRuolo);
        IList<OrgTipoFunzione> AmmGetListFunzioni(string idAmm, string idRuolo);

        [OperationContract]
        //ArrayList AmmGetListRegistriRF(string idAmm, string idRuolo, string chaRF);
        IList<OrgRegistro> AmmGetListRegistriRF(string idAmm, string idRuolo, string chaRF);

        [OperationContract]
        DataSet isCorrInListaDistr(string idCorr);

        [OperationContract]
        DocsPaVO.LibroFirma.ProcessoFirma[] GetProcessiDiFirmaByTitolarePaging(string idRuoloTitolare, string idUtenteTitolare, int numPage, int pageSize, out int numTotPage, out int nRec);

        [OperationContract]
        string GetDescDiagrammiByIdProcesso(string idProcesso);

        [OperationContract]
        SaveChangesToRoleResponse SaveChangesToRole(SaveChangesToRoleRequest request);

        [OperationContract]
        bool AmmStoricizzaRuoloPassiCorrelati(string idRuoloOld, string idRuoloNew);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmSpostaRuolo(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRuolo ruolo);

        [OperationContract]
        IList<string> GetListaParentUo(string idUo, string livelloUo);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmVerificaUtenteLoggato(string userId, string idAmm);

        [OperationContract]
        int GetCountProcessiDiFirmaByTitolare(string idRuoloTitolare, string idUtenteTitolare);

        [OperationContract]
        int GetCountIstanzaProcessiDiFirmaByTitolare(string idRuoloTitolare, string idUtenteTitolare);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmEliminaUtenteInRuolo(InfoUtenteAmministratore infoUtente, string idPeople, string idGruppo, string idAmm);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmVerificaTrasmRuolo(string idCorrGlobRuolo);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmRifiutaTrasmConWF(string idCorrGlobRuolo, string idGruppo);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmInsUtenteInRuolo(InfoUtenteAmministratore infoUtente, string idPeople, string idGruppo, string idAmm, string type);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmInsTrasmUtente(string idPeople, string idCorrGlobRuolo);

        [OperationContract]
        //ArrayList AmmGetNodiTitolario_InRuolo(string idGruppo);
        IList<OrgNodoTitolario> AmmGetNodiTitolario_InRuolo(string idGruppo);

        [OperationContract]
        string filtroRicercaTitAmm(string codice, string descrizione, string codAmm, string idRegistro);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmExtendToChildNodes(InfoUtenteAmministratore infoUtente, string idNodoTitolario, DocsPaVO.amministrazione.OrgRuoloTitolario ruoloTitolario, string idAmm, string idRegistro, bool check);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione[] AmmUpdateRuoliTitolario(
            InfoUtenteAmministratore infoUtente,
            string idTitolario,
            DocsPaVO.amministrazione.OrgRuoloTitolario[] ruoliTitolario,
            DocsPaVO.amministrazione.OrgRuoloTitolario[] ruoliTitolarioDisattiva,
            string idAmm,
            bool allTitolario,
            string idRegistro);

        [OperationContract]
        DocsPaVO.amministrazione.PasswordConfigurations AdminGetPasswordConfigurations(InfoUtenteAmministratore infoUtente, int idAmministrazione);

        bool AdminSavePasswordConfigurations(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.PasswordConfigurations configurations);

        [OperationContract]
        void AdminExpireAllPassword(InfoUtenteAmministratore infoUtente, int idAmministrazione);

        [OperationContract]
        void salvaAssociazioneModelli(string idTipoDoc, string idDiagramma, ArrayList modelliSelezionati, string idStato);

        [OperationContract]
        //ArrayList getModelliByAmmConRicerca(string idAmm, string codiceRicerca, string tipoRicerca);
        IList<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione> getModelliByAmmConRicerca(string idAmm, string codiceRicerca, string tipoRicerca);

        [OperationContract]
        //ArrayList getIdModelliTrasmAssociati(string idTipoDoc, string idDiagramma, string idStato);
        IList<DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm> getIdModelliTrasmAssociati(string idTipoDoc, string idDiagramma, string idStato);

        [OperationContract]
        //ArrayList getModelliAssDiagrammi(string idTipo, string idDiagramma, string stato, string idAmm, bool selezionati, string tipo);
        IList<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione> getModelliAssDiagrammi(string idTipo, string idDiagramma, string stato, string idAmm, bool selezionati, string tipo);


        [OperationContract]
        //ArrayList getModelliByAmmLite(string idAmm);
        IList<DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione> getModelliByAmmLite(string idAmm);

        [OperationContract]
        //ArrayList getRuoliByAmm(string idAmm, string codiceRicerca, string tipoRicerca);
        IList<DocsPaVO.utente.Ruolo> getRuoliByAmm(string idAmm, string codiceRicerca, string tipoRicerca);

        [OperationContract]
        //ArrayList getTemplatesArchivioDeposito(DocsPaVO.utente.InfoUtente infoUtente, string idAmm, bool seRepertorio);
        IList<DocsPaVO.ProfilazioneDinamica.Templates> getTemplatesArchivioDeposito(InfoUtenteAmministratore infoUtente, string idAmm, bool seRepertorio);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Templates getTemplateById(string idTemplate);

        [OperationContract]
        bool UpdateAssociazioneTemplateContestoProcedurale(string idTipoAtto, string idContestoProcedurale, InfoUtenteAmministratore infoUente);

        [OperationContract]
        bool InsertContestoProcedurale(DocsPaVO.FlussoAutomatico.ContestoProcedurale contesto, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        List<DocsPaVO.FlussoAutomatico.ContestoProcedurale> GetListContestoProcedurale(InfoUtenteAmministratore infoUtente);

        [OperationContract]
        //ArrayList getRuoliFromOggettoCustomDoc(string idTemplate, string idOggettoCustom);
        IList<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> getRuoliFromOggettoCustomDoc(string idTemplate, string idOggettoCustom);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.OggettoCustom getOggettoById(string idOggetto);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Contatore[] GetValuesContatoriDoc(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom);

        [OperationContract]
        void DeleteValueContatoreDoc(DocsPaVO.ProfilazioneDinamica.Contatore contatore);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.SelectiveHistoryResponse GetCustomHistoryList(DocsPaVO.ProfilazioneDinamica.SelectiveHistoryRequest request);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.SelectiveHistoryResponse ActiveSelectiveHistory(DocsPaVO.ProfilazioneDinamica.SelectiveHistoryRequest request);

        [OperationContract]
        bool isInUseCampoComuneDoc(string idTemplate, string idCampoComune);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Templates eliminaOggettoCustomTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, int oggettoCustom);

        [OperationContract]
        int countDocTipoDoc(string tipo_atto, string codiceAmm);

        [OperationContract]
        void updatePrivatoTipoDoc(int systemId_template, string privato);

        [OperationContract]
        void updateMesiConsTipoDoc(int systemId_template, string mesiCons);

        [OperationContract]
        void updateInvioConsTipoDoc(int systemId_template, string invioCons);

        [OperationContract]
        void updateConsolidamentoOggCustom(int systemId, string consolida, string systemId_template);

        [OperationContract]
        void updateConservazioneOggCustom(int systemId, string conserva, string systemId_template);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Templates aggiungiOggettoCustomTemplate(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DocsPaVO.ProfilazioneDinamica.Templates template);

        [OperationContract]
        bool isValueInUse(string idOggetto, string idTemplate, string valoreOggettoDB);

        [OperationContract]
        //ArrayList getRuoliTipoDoc(string idTipoDoc);
        IList<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> getRuoliTipoDoc(string idTipoDoc);

        [OperationContract]
        //ArrayList GetTipologiaAttoProfDin(string idAmministrazione);
        IList<DocsPaVO.documento.TipologiaAtto> GetTipologiaAttoProfDin(string idAmministrazione);

        [OperationContract]
        bool salvaTemplate(InfoUtenteAmministratore infoUtente, DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione);

        [OperationContract]
        bool aggiornaTemplate(DocsPaVO.ProfilazioneDinamica.Templates template);

        [OperationContract]
        void messaInEserizioTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione);

        [OperationContract]
        bool UpdateIsTypeInstance(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione);

        [OperationContract]
        bool disabilitaTemplate(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione, string codiceAmministrazione);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Templates spostaOggetto(DocsPaVO.ProfilazioneDinamica.Templates template, int oggettoSelezionato, string spostamento);

        [OperationContract]
        void updateScadenzeTipoDoc(int systemId_template, string scadenza, string preScadenza);

        [OperationContract]
        bool associaTipoDocDiagramma(string idTipoDoc, string idDiagramma);

        [OperationContract]
        int getDiagrammaAssociato(string idTipoDoc);

        [OperationContract]
        bool isDocumentiInStatoFinale(string idDiagramma, string idTemplate);

        [OperationContract]
        void disassociaTipoDocDiagramma(string idTipoDoc);

        [OperationContract]
        void salvaDirittiCampiTipologiaDoc(ArrayList listaDirittiCampiSelezionati);

        [OperationContract]
        //ArrayList getDirittiCampiTipologiaDoc(string idRuolo, string idTemplate);
        IList<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> getDirittiCampiTipologiaDoc(string idRuolo, string idTemplate);

        [OperationContract]
        void estendiDirittiCampiARuoliDoc(ArrayList listaDirittiCampiSelezionati, ArrayList listaRuoli);

        [OperationContract]
        byte[] GetFileFromPath(string path);

        [OperationContract]
        void salvaAssociazioneModelliFasc(string idTipoFasc, string idDiagramma, ArrayList modelliSelezionati, string idStato);

        [OperationContract]
        //ArrayList getIdModelliTrasmAssociatiFasc(string idTipoFasc, string idDiagramma, string idStato);
        IList<DocsPaVO.Modelli_Trasmissioni.AssDocDiagTrasm> getIdModelliTrasmAssociatiFasc(string idTipoFasc, string idDiagramma, string idStato);

        [OperationContract]
        //ArrayList getTemplatesFasc(string idAmministrazione);
        IList<DocsPaVO.ProfilazioneDinamica.Templates> getTemplatesFasc(string idAmministrazione);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascById(string idTemplate);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Templates impostaCampiComuniFasc(DocsPaVO.ProfilazioneDinamica.Templates modello, ArrayList campiComuni);

        [OperationContract]
        //ArrayList getRuoliFromOggettoCustomFasc(string idTemplate, string idOggettoCustom);
        IList<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> getRuoliFromOggettoCustomFasc(string idTemplate, string idOggettoCustom);

        [OperationContract]
        EsitoOperazione SaveStrutturaTemplateRelation(int idtipofascicolo, int idtitolario, int idtemplate);

        [OperationContract]
        bool isInUseCampoComuneFasc(string idTemplate, string idCampoComune);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Templates eliminaOggettoCustomTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template, int oggettoCustom);

        [OperationContract]
        int countFascTipoFasc(string tipo_fasc, string codiceAmm);

        [OperationContract]
        void updatePrivatoTipoFasc(int systemId_template, string privato);

        [OperationContract]
        void updateMesiConsTipoFasc(int systemId_template, string mesiCons);

        [OperationContract]
        bool isValueInUseFasc(string idOggetto, string idTemplate, string valoreOggettoDB);

        [OperationContract]
        IList<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> getRuoliTipoFasc(string idTipoFasc);

        [OperationContract]
        bool salvaTemplateFasc(InfoUtenteAmministratore infoUtente, DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione);

        [OperationContract]
        bool aggiornaTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template);

        [OperationContract]
        void messaInEserizioTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione);

        [OperationContract]
        bool disabilitaTemplateFasc(DocsPaVO.ProfilazioneDinamica.Templates template, string idAmministrazione, string codiceAmministrazione);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Templates getTemplateFascCampiComuniById(InfoUtenteAmministratore infoUtente, string idTemplate);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Templates spostaOggettoFasc(DocsPaVO.ProfilazioneDinamica.Templates template, int oggettoSelezionato, string spostamento);

        [OperationContract]
        void updateScadenzeTipoFasc(int systemId_template, string scadenza, string preScadenza);

        [OperationContract]
        void salvaDirittiCampiTipologiaFasc(ArrayList listaDirittiCampiSelezionati);

        [OperationContract]
        void salvaAssociazioneFascRuoli(ArrayList assFascRuoli);

        [OperationContract]
        //ArrayList getDirittiCampiTipologiaFasc(string idRuolo, string idTemplate);
        IList<DocsPaVO.ProfilazioneDinamica.AssDocFascRuoli> getDirittiCampiTipologiaFasc(string idRuolo, string idTemplate);

        [OperationContract]
        void estendiDirittiCampiARuoliFasc(ArrayList listaDirittiCampiSelezionati, ArrayList listaRuoli);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo InsertQual(DocsPaVO.Qualifica.Qualifica qual);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo UpdateQual(String idQualifica, String descrizione);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo DeleteQual(String idQualifica, int idAmministrazione);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmUpdateMessageNotificaRagioneTrasmissione(string codiceRagione, string idAmministrazione, string msgNotificaDocumenti, string msgNotificaFascioli, bool allRagioniDoc, bool allRagioniFasc);

        [OperationContract]
        DocsPaVO.amministrazione.OrgRagioneTrasmissione AmmGetRagioneTrasmissione(string idRagione);

        [OperationContract]
        DocsPaVO.amministrazione.OrgRagioneTrasmissione[] AmmGetRagioniTrasmissione(string idAmministrazione);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmInsertRagioneTrasmissione(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.OrgRagioneTrasmissione ragione, string idAmm);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmUpdateRagioneTrasmissione(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.OrgRagioneTrasmissione ragione, string idAmm);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmDeleteRagioneTrasmissione(ref DocsPaVO.amministrazione.OrgRagioneTrasmissione ragione);

        [OperationContract]
        bool DeleteModelloStampaRicevuta(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRegistro registro);

        [OperationContract]
        bool ContainsModelloStampaRicevuta(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRegistro objRegistro);

        [OperationContract]
        bool SaveModelloStampaRicevuta(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRegistro registro, byte[] modelContent, bool modelPdf);

        [OperationContract]
        bool AmmInvalidaProcessiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.utente.Corrispondente AddressbookGetCorrispondenteByCodRubricaIE(string codice, DocsPaVO.addressbook.TipoUtente tipoIE, InfoUtenteAmministratore u);

        [OperationContract]
        bool AmmSbloccoCasella(string email, string idRegistro);

        [OperationContract]
        DocsPaVO.utente.Ruolo getRuoloById(string idCorrGlobali);

        [OperationContract]
        GetUsersInRoleMinimalInfoResponse GetUsersInRoleMinimalInfo(GetUsersInRoleMinimalInfoRequest request);

        [OperationContract]
        string GetXMLUOSmistamento(string idRegistro);

        [OperationContract]
        IList<DocsPaVO.amministrazione.OrgRuolo> AmmGetListaRuoliAOO(string idRegistro);

        [OperationContract]
        //ArrayList AmmGetListRuoli(string idAmm, string ricercaPer, string testoDaRicercare, string idRegistro, string IDesclusi);
        IList<OrgRuolo> AmmGetListRuoli(string idAmm, string ricercaPer, string testoDaRicercare, string idRegistro, string IDesclusi);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmAssociazioneRFRuolo(string idRf, string idCorrGlobRuolo);

        [OperationContract]
        //ArrayList AmmGetListRuoliUORic(string idUO, bool ricorsivo);
        IList<OrgRuolo> AmmGetListRuoliUORic(string idUO, bool ricorsivo);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmDeleteAssociazioneRFRuolo(string idRf, string idCorrGlobRuolo);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmDelRightMailRegistro(string idRegistro, string idRuoloInAoo, string indirizzoEmail);

        [OperationContract]
        DocsPaVO.amministrazione.OrgRegistro AmmGetRegistro(string idRegistro);

        [OperationContract]
        bool IsEnabledInteropInterna();

        [OperationContract]
        bool AmmExistsPassiFirmaByIdRegistroAndEmailRegistro(string idRegistro, string emailRegistro);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmUpdateRegistro(ref DocsPaVO.amministrazione.OrgRegistro registro);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmDeleteRegistro(ref DocsPaVO.amministrazione.OrgRegistro registro);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmCanDeleteRegistro(DocsPaVO.amministrazione.OrgRegistro registro);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmInsRightMailRegistro(System.Collections.Generic.List<RightRuoloMailRegistro> rightRuoloMailRegistro);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmDeleteMailRegistro(string idRegistro, string casella);

        [OperationContract]
        bool IsEnabledRF(string idAmm);

        [OperationContract]
        //ArrayList AmmGetTipiRuolo(string codiceAmministrazione);
        IList<OrgTipoRuolo> AmmGetTipiRuolo(string codiceAmministrazione);

        [OperationContract]
        DocsPaVO.amministrazione.OrgTipoRuolo AmmGetTipoRuolo(string idTipoRuolo);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmInsertTipoRuolo(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.OrgTipoRuolo tipoRuolo, string idAmm);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmUpdateTipoRuolo(ref DocsPaVO.amministrazione.OrgTipoRuolo tipoRuolo);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmDeleteTipoRuolo(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.OrgTipoRuolo tipoRuolo, string idAmm);

        [OperationContract]
        string AmmGetIDAmm(string codAmm);

        [OperationContract]
        //ArrayList AmmGetListTipoRuoloUtenti(string codTipoRuolo, string idAmm);
        IList<OrgRuolo> AmmGetListTipoRuoloUtenti(string codTipoRuolo, string idAmm);

        [OperationContract]
        //ArrayList AmmGetSistemiEsterni(string idAmm);
        IList<SistemaEsterno> AmmGetSistemiEsterni(string idAmm);

        [OperationContract]
        string ctrlInserimentoSistemaEsterno(string idAmm, string codUtente, string codRuolo);

        [OperationContract]
        DocsPaVO.utente.UnitaOrganizzativa GetHubSistemiEsterni(string codice, string idAmm);

        [OperationContract]
        bool setVisibilityHubSysExt(string idHub);

        [OperationContract]
        DocsPaVO.utente.TipoRuolo AmmGetTipoRuoloByCode(string idAmm, string codice);

        [OperationContract]
        bool AmmInsSysExtAfterAssoc(InfoUtenteAmministratore infut, string idAmm, string codUtente, string codRuolo, string descrizione);

        [OperationContract]
        bool AmmModDescTknTimeForExtSys(string desc, string tknTime, string idSysExt);

        [OperationContract]
        bool DeleteExternalSystem(DocsPaVO.amministrazione.SistemaEsterno ExtSys, InfoUtenteAmministratore infout);

        [OperationContract]
        bool AmmModAllowedPISforExtSys(string methods, string idSysExt);

        [OperationContract]
        //ArrayList AmmGetPISMethods();
        IList<MetodoPIS> AmmGetPISMethods();

        [OperationContract]
        DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinalePerTipologia(
            InfoUtenteAmministratore infoUtente,
            string idOggetto, string id_Aoo_RF, string annoDa, string annoA, string numeroDa, string numeroA, bool sbloccati, string IdAmministrazione);

        [OperationContract]
        DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinalePerContatore(
            InfoUtenteAmministratore infoUtente,
            string idTemplates, string idOggetto, string id_Aoo_RF, string anno, string numero, bool sbloccati, string IdAmministrazione);

        [OperationContract]
        DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinalePerTipologiaDocumento(
            InfoUtenteAmministratore infoUtente,
            string idTemplate, string anno, bool sbloccati, string IdAmministrazione);

        [OperationContract]
        DocsPaVO.amministrazione.DocumentoStatoFinale[] GetDocumentiStatoFinale(
            InfoUtenteAmministratore infoUtente,
            string idDocumento, string anno, string idRegistro, bool sbloccati, string IdTipologia, string IdAmministrazione, bool Protocollati);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Templates getTemplateCampiComuniById(InfoUtenteAmministratore infoUtente, string idTemplate);

        [OperationContract]
        //ArrayList DocumentoGetVisibilita(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, bool cercaRimossi);
        IList<DocsPaVO.documento.DirittoOggetto> DocumentoGetVisibilita(InfoUtenteAmministratore infoUtente, string idProfile, bool cercaRimossi);

        [OperationContract]
        bool ModificaDocumentoStatoFinale(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.ModificaAclDocumentoStatoFinale[] infoModificheList);

        [OperationContract]
        DocsPaVO.documento.SchedaDocumento DocumentoGetDettaglioDocumentoNoSecurity(InfoUtenteAmministratore infoutente, string idProfile, string docNumber);

        [OperationContract]
        DataTable GetStruttureSottofascicoli(string idamministrazione);

        [OperationContract]
        EsitoOperazione DeleteStrutturaSottofascicoli(int idtemplate, int idtipofascicolo, int idtitolario, string idAmm);

        [OperationContract]
        DataTable GetStruttureSottofascicoliById(string idamm, int idfascicolo, int idtitolario, int idtemplate);

        [OperationContract]
        EsitoOperazione SaveStrutturaSottofascicoli(int id, DataTable data, string name, string idAmm);

        [OperationContract]
        bool IsNodoStrutturaInFascicoliEmpty(string strutturaid, string nodename, string idamm, out int numfascicoli);

        [OperationContract]
        DocsPaVO.amministrazione.OrgTitolario getTitolarioById(string idTitolario);

        [OperationContract]
        void setEtichetteTitolario(DocsPaVO.amministrazione.OrgTitolario titolario);

        [OperationContract]
        bool importIndice(byte[] dati, DocsPaVO.amministrazione.OrgTitolario titolario, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        bool importTitolario(byte[] dati, DocsPaVO.amministrazione.OrgTitolario titolario, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.fascicolazione.VoceIndiceSistematico existVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice);

        [OperationContract]
        void addNuovaVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice);

        [OperationContract]
        bool isAssociataVoce(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice, bool suSingoloNodo);

        [OperationContract]
        void removeVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice);

        [OperationContract]
        void associaVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice);

        [OperationContract]
        void dissociaVoceIndice(DocsPaVO.fascicolazione.VoceIndiceSistematico voceIndice);

        [OperationContract]
        //ArrayList getIndiceByIdAmm(string idAmm);
        IList<VoceIndiceSistematico> getIndiceByIdAmm(string idAmm);

        [OperationContract]
        //ArrayList getIndiceByIdProject(string idProject);
        IList<VoceIndiceSistematico> getIndiceByIdProject(string idProject);

        [OperationContract]
        //ArrayList getLogImportIndice();
        IList<string> getLogImportIndice();

        [OperationContract]
        //ArrayList getLogImportTitolario();
        IList<string> getLogImportTitolario();

        [OperationContract]
        bool isEnableIndiceSistematico();

        [OperationContract]
        bool existTitolarioInDef(string codiceAmm);

        [OperationContract]
        string isEnableContatoreTitolario();

        [OperationContract]
        int getContatoreProtTitolario(DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario);

        [OperationContract]
        bool SaveTitolario(InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.OrgTitolario titolario);

        [OperationContract]
        bool attivaTitolario(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgTitolario titolario);

        [OperationContract]
        bool deleteTitolario(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgTitolario titolario);

        [OperationContract]
        bool copiaTitolario(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgTitolario titolario, string idRegistro);

        [OperationContract]
        DocsPaVO.documento.FileDocumento ExportTitolarioInExcel(DocsPaVO.amministrazione.OrgTitolario titolario, string idRegistro);

        [OperationContract]
        //ArrayList getTitolari(string idAmministrazione);
        IList<OrgTitolario> getTitolari(string idAmministrazione);

        [OperationContract]
        List<DispositivoStampaEtichetta> AmministrazioneGetDispositivoStampaEtichetta();

        [OperationContract]
        bool EnableHTML5SocketOption();

        [OperationContract]
        bool AdminIsSupportedPasswordConfig();

        [OperationContract]
        DocsPaVO.Ldap.LdapConfig GetLdapConfig(InfoUtenteAmministratore infoUtente, string idAmministrazione);

        [OperationContract]
        //ArrayList AmmGetListUtentiInAmmRic(string idAmm, string ricercaPer, string testoDaRicercare);
        IList<OrgUtente> AmmGetListUtentiInAmmRic(string idAmm, string ricercaPer, string testoDaRicercare);

        [OperationContract]
        //ArrayList amministrazioneGetAmministrazioniByUser(string userId, bool controllo, out string returnMsg);
        IList<Amministrazione> amministrazioneGetAmministrazioniByUser(string userId, bool controllo, out string returnMsg);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmInsUtente(InfoUtenteAmministratore infoUtente, string idAmm, DocsPaVO.amministrazione.OrgUtente utente);

        [OperationContract]
        Utente GetUtenteAutomatico(string idAmministrazione);

        [OperationContract]
        string GetPasswordUtenteMultiAmm(string userId);

        [OperationContract]
        bool SetPasswordUtenteMultiAmm(string userId, string password);

        [OperationContract]
        bool SetUtenteVerticale(string userId, string idAmm, bool isVerticale);

        [OperationContract]
        bool ModificaPasswordUtenteMultiAmm(string userId, string idAmm);

        [OperationContract]
        bool IsUtenteVerticale(string idPeople);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmModUtente(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgUtente utente);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmVerificaEliminazioneUtente(DocsPaVO.amministrazione.OrgUtente utente);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmEliminaUtente(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgUtente utente);

        [OperationContract]
        string GetFormatoDominio(string idAmm);

        [OperationContract]
        bool AmmCheckRegAssUtente(string idCorrGlob);

        [OperationContract]
        bool AmmCheckMenuAssUtente(string idCorrGlob);

        [OperationContract]
        //ArrayList AmmGetListRuoliUtente(string idPeople);
        IList<OrgRuolo> AmmGetListRuoliUtente(string idPeople);

        [OperationContract]
        bool AmmCheckRespAOO(string idPeople, string idGruppo);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmEliminaRegistriUtenteAdmin(string idCorrGlob);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmEliminaMenuUtenteAdmin(string idCorrGlob);

        [OperationContract]
        bool importaUtenti(InfoUtenteAmministratore infoUtente, byte[] dati, string nomeFile, string codiceAmm, bool update, ref int utInseriti, ref int utAggiornati);

        [OperationContract]
        DocsPaVO.Ldap.LdapSyncronizationResponse GetLdapSyncResponse(InfoUtenteAmministratore infoUtente, int idHistoryItem);

        [OperationContract]
        void SaveLdapConfig(InfoUtenteAmministratore infoUtente, string idAmministrazione, DocsPaVO.Ldap.LdapConfig ldapInfo);

        [OperationContract]
        DocsPaVO.Ldap.LdapSyncronizationResponse SyncronizeLdapUsers(DocsPaVO.Ldap.LdapSyncronizationRequest request);

        [OperationContract]
        DocsPaVO.Ldap.LdapSyncronizationHistoryItem[] GetLdapSyncHistory(InfoUtenteAmministratore infoUtente, string idAmministrazione);

        [OperationContract]
        void DeleteLdapSyncHistoryItem(InfoUtenteAmministratore infoUtente, int idHistoryItem);

        [OperationContract]
        //ArrayList getLogImportUtenti();
        IList<string> getLogImportUtenti();

        [OperationContract]
        string ElencoUtentiConnessi(string codiceAmm);

        [OperationContract]
        int NumUtentiConnessi(string codiceAmm);

        [OperationContract]
        bool DisconnettiUtente(string userId, string codiceAmm);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmInsMenuUtenteAdmin(DocsPaVO.amministrazione.Menu[] listaMenu, string idCorrGlob, string idAmm);

        [OperationContract]
        //ArrayList AmmGetListRegistriUtente(string idAmm, string idCorrGlob);
        IList<OrgRuolo> AmmGetListRegistriUtente(string idAmm, string idCorrGlob);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmInsRegistriUtenteAdmin(DocsPaVO.amministrazione.OrgRegistro[] listaRegistri, string idCorrGlob);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmImpostaRuoloPreferito(InfoUtenteAmministratore infoUtente, string idPeople, string idGruppo);

        [OperationContract]
        string GetLabelTipoDocumento(string typeId);

        [OperationContract]
        InteroperabilitySettings LoadSimplifiedInteroperabilitySettings(String registryId);

        [OperationContract]
        bool SaveSimplifiedInteroperabilitySettings(InteroperabilitySettings interoperabilitySettings);

        [OperationContract]
        bool AmmVerificaGestioneChiavi(string idPeople);

        [OperationContract]
        bool SaveElementoRubricaRF(RaggruppamentoFunzionale rf, String idRegistro);

        [OperationContract]
        String GetRoleDescriptionByIdGroup(String idGroup);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AdminChangePassword(DocsPaVO.utente.UserLogin user, string oldPassword);

        [OperationContract]
        string getApplicationName();

        [OperationContract]
        IList<Amministrazione> amministrazioneGetAmministrazioni(out string returnMsg);

        [OperationContract]
        DocsPaVO.amministrazione.InfoAmministrazione[] AmmGetListAmministrazioni();

        [OperationContract]
        //DocsPaVO.documento.EtichettaInfo[] getEtichetteDocumenti(DocsPaVO.utente.InfoUtente infoUtente, string idAmm);
        DocsPaVO.documento.EtichettaInfo[] getEtichetteDocumenti(InfoUtenteAmministratore infoUtente, string idAmm);

        [OperationContract]
        //ArrayList getListaChiaviConfig(string idAmm);
        IList<ChiaveConfigurazione> getListaChiaviConfig(string idAmm);

        [OperationContract]
        OrgRagioneTrasmissione[] AmmGetInfoRagioniTrasmissione(string idAmministrazione);

        [OperationContract]
        List<DocsPaVO.documento.FascicolazioneTipiDocumento> GetFascicolazioneTipiDocumento(string idAmm, InfoUtenteAmministratore infoutente);

        [OperationContract]
        bool IsEnabledSupportedFileTypes();

        [OperationContract]
        DocsPaVO.RubricaComune.ConfigurazioniRubricaComune GetConfigurazioniRubricaComune(InfoUtenteAmministratore infoUtente);

        [OperationContract]
        IList<OrgUtente> AmmGetListUtentiInAmm(string idAmm);

        [OperationContract]
        int NumUtentiAttivi(string codiceAmm);

        [OperationContract]
        bool getValueInteropNoMail();

        [OperationContract]
        IList<DocsPaVO.ProfilazioneDinamica.Templates> getTipoFasc(string idAmministrazione);

        [OperationContract]
        DataSet GetAmmRightMailRegistro(string idRegistro, string idRuoloInUO);

        [OperationContract]
        string RegistriInAmm(string codAmm);

        [OperationContract]
        IList<OrgNodoTitolario> AmmGetNodiTitolario(string codiceAmministrazione, string idNodoTitolarioParent, string idRegistro);

        [OperationContract]
        IList<object> getResponsabiliCons(string idAmm);

        [OperationContract]
        DocsPaVO.Conservazione.PARER.PolicyFascicoliPARER[] GetListaPolicyFascicoliPARER(string idAmm);

        [OperationContract]
        string findFontColor(string idAmm);

        [OperationContract]
        DocsPaVO.CheckInOut.CheckOutStatus[] GetCheckOutAdminDocuments(InfoUtenteAmministratore adminInfoUtente, string idAdministration);

        [OperationContract]
        bool CanForceUndoCheckOutAdminDocument(InfoUtenteAmministratore adminInfoUtente);

        [OperationContract()]
        IList<DocsPaVO.ProfilazioneDinamica.Templates> getTemplates(string idAmministrazione);
        //[return: XmlArray()]
        //[return: XmlArrayItem(typeof(DocsPaVO.Deleghe.InfoDelega))]
        //[XmlInclude(typeof(DocsPaVO.ProfilazioneDinamica.Templates))]
        //[SoapInclude(typeof(DocsPaVO.ProfilazioneDinamica.Templates))]
        //ArrayList getTemplates(string idAmministrazione);

        [OperationContract]
        IList<DocsPaVO.DiagrammaStato.DiagrammaStato> getDiagrammi(string idAmm);

        [OperationContract]
        IList<DocsPaVO.trasmissione.RagioneTrasmissione> TrasmissioneGetRagioniSysExt(DocsPaVO.trasmissione.Diritti diritti);

        [OperationContract]
        IList<DocsPaVO.trasmissione.RagioneTrasmissione> TrasmissioneGetRagioni(DocsPaVO.trasmissione.Diritti diritti, bool flgDaRicercaTrasm);

        [OperationContract]
        IList<DocsPaVO.Deleghe.InfoDelega> DelegaGetLista(InfoUtenteAmministratore infoUtente, string tipoDelega, string statoDelega, string idAmm, out int numDeleghe);

        [OperationContract]
        DocsPaVO.amministrazione.MailProvider[] AmmGetProviderList();

        [OperationContract]
        bool IsEnabledMultiMail(string idAmm);

        [OperationContract]
        bool setEtichetteDocumenti(InfoUtenteAmministratore infoUtente, string idAmm, DocsPaVO.documento.EtichettaInfo[] etichette);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmUpdateAmm(ref DocsPaVO.amministrazione.InfoAmministrazione info, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente, bool modFascicolatura, bool modSegnatura, bool modTimbroPdf, bool modProtTit);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmInsertRegistro(ref DocsPaVO.amministrazione.OrgRegistro registro, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.amministrazione.OrgTipoFunzione AmmGetTipoFunzione(string idTipoFunzione, bool fillFunzioniElementari);

        [OperationContract]
        List<string> GetIdRuoliProcessiUltimoUtente(string idPeople);

        [OperationContract]
        GetSettingsMinimalInfoResponse GetSettingsMinimalInfo(GetSettingsMinimalInfoRequest request);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmInsNuovaUO(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgUO nuovaUO);

        [OperationContract]
        string GetMailPrincipaleRegistro(string idRegistro);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo ForceUndoCheckOutAdminDocument(InfoUtenteAmministratore adminInfoUtente, DocsPaVO.CheckInOut.CheckOutStatus checkOutAdminStatus);

        [OperationContract]
        bool LogoutAmministratore(DocsPaVO.amministrazione.InfoUtenteAmministratore adminUser);

        [OperationContract]
        void resetHashTable();

        [OperationContract]
        DocsPaVO.utente.Amministrazione AmmModificaUoTIBCO(string oldCodiceUO, DocsPaVO.amministrazione.OrgUO theUO, out bool result);

        [OperationContract]
        bool IsPianoConservazioneAcquisito(string idTitolario, string idRegistro, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmInsNuovoRuolo(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRuolo newRuolo, bool computeAtipicita);

        [OperationContract]
        List<DocsPaVO.PianoConservazione> GetPianoConservazioneByIdClassificazione(string idClassificazione, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmInsertMailRegistro(string idRegistro, CasellaRegistro[] caselle, bool insertInteropInt);

        [OperationContract]
        bool AmmExistsPassiFirmaByRuoloTitolareAndRegistro(DocsPaVO.amministrazione.RightRuoloMailRegistro[] rightRuoloMailReg, string idRuoloInUO, string idGruppo);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmInsRegistri(DocsPaVO.amministrazione.OrgRegistro[] listaRegistri, string idUO, string idCorrGlobRuolo);

        [OperationContract]
        int getDiagrammaAssociatoFasc(string idTipoFasc);

        [OperationContract]
        IList<DocsPaVO.amministrazione.OrgRuoloTitolario> AmmGetRuoliInTitolario(string idNodoTitolario, string idRegistro, string codiceRicerca, string tipoRicerca);

        [OperationContract]
        bool IsElementInteroperableWithSimplifiedInteroperability(String objectId, bool rf);

        [OperationContract]
        bool isFiltroAooEnabled();

        [OperationContract]
        void rubricaCheckChildrenExistence(ref DocsPaVO.rubrica.ElementoRubrica[] ers, InfoUtenteAmministratore u);

        [OperationContract]
        IList<DocsPaVO.utente.Amministrazione> GetDatiAmministrazioneByUserAdministrator(string userId, bool controllo, out string returnMsg);

        [OperationContract]
        InfoUtenteAmministratore GetDatiAmministratore(string userId, string idAmm);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmInsTipoFunzioni(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgTipoFunzione[] listaFunzioni);

        [OperationContract]
        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione getModelloByID(string idAmm, string idModello);

        [OperationContract]
        IList<DocsPaVO.rubrica.ElementoRubrica> rubricaGetElementiRubrica(DocsPaVO.rubrica.ParametriRicercaRubrica qc, InfoUtenteAmministratore u, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica);

        [OperationContract]
        DocsPaVO.rubrica.ElementoRubrica rubricaGetElementoRubricaSimpleBySystemId(string systemId, InfoUtenteAmministratore u);

        [OperationContract]
        bool isCorrispondenteValid(string userId);

        [OperationContract]
        bool isEnableConversionePdfLatoServer();

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmModUO(DocsPaVO.amministrazione.OrgUO theUO, bool StoricizzUO);

        [OperationContract]
        void rubricaCheckChildrenExistenceEx(ref DocsPaVO.rubrica.ElementoRubrica[] ers, bool checkUo, bool checkRuoli, bool checkUtenti, InfoUtenteAmministratore u);

        [OperationContract]
        IList<DocsPaVO.rubrica.ElementoRubrica> rubricaGetRootItems(DocsPaVO.addressbook.TipoUtente tipoIE, InfoUtenteAmministratore u, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica);

        [OperationContract]
        void salvaAssociazioneDocRuoli(ArrayList assDocRuoli);

        [OperationContract]
        bool isUniqueNameDiagramma(string nomeDiagramma);

        [OperationContract]
        void salvaDiagramma(DocsPaVO.DiagrammaStato.DiagrammaStato dg, string idAmm);

        [OperationContract]
        DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione UtentiConNotificaTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione objModTrasm, ArrayList utentiDaInserire, ArrayList utentiDaCancellare, string operazione);

        [OperationContract]
        IList<DocsPaVO.utente.Ruolo> GetListaRuoliUtente(string idPeople);

        [OperationContract]
        DocsPaVO.documento.FileDocumento AmmExportTabellaConfineIntegrazioni(string idAmministrazione);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmInsertAmm(ref DocsPaVO.amministrazione.InfoAmministrazione info, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente);

        [OperationContract]
        bool SetFascicolazioneTipiDocumento(List<DocsPaVO.documento.FascicolazioneTipiDocumento> listTipiDoc, string idAmm, InfoUtenteAmministratore infoutente);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmDeleteAmm(ref DocsPaVO.amministrazione.InfoAmministrazione info, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.PianoConservazione GetPianoConservazioneByIdTipoAtto(string idTipoAtto, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente);

        [OperationContract]
        List<DocsPaVO.PianoConservazione> GetPianoConservazioneByCodiceClassificazione(string codiceClassificazione, DocsPaVO.utente.Registro registro, string idAmministrazione, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.PianoConservazione GetPianoConservazioneByIdTipoFasc(string idTipoFasc, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente);

        [OperationContract]
        bool InsertPianoConservazioneTipoAtto(string idPianoConservazione, string idTipoAtto, string idAmministrazione, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente);

        [OperationContract]
        bool UpdatePianoConservazioneTipoAtto(string idTipoAtto, string idPianoConservazione, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.utente.Corrispondente AddressbookGetCorrispondenteByIdPeople(string idPeople, DocsPaVO.addressbook.TipoUtente tipoIE, DocsPaVO.amministrazione.InfoUtenteAmministratore u);

        [OperationContract]
        void salvaModelli(byte[] dati, string nomeProfilo, string codiceAmministrazione, string nomeFile, string estensione, DocsPaVO.ProfilazioneDinamica.Templates template);

        [OperationContract]
        DocsPaVO.amministrazione.OrgUtente AmmGetDatiUtente(string idCorrGlob);

        [OperationContract]
        bool AmmImportTabellaConfineIntegrazioni(byte[] dati, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente);

        [OperationContract]
        bool InsertPianoConservazioneTipoFasc(string idPianoConservazione, string idTipoFasc, string idAmministrazione, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente);

        [OperationContract]
        IList<Corrispondente> AddressbookGetListaCorrispondenti_Aut(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente);

        [OperationContract]
        //IList<Corrispondente> AddressbookGetListaCorrispondenti(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente);
        IList<Ruolo> AddressbookGetListaCorrispondenti(DocsPaVO.addressbook.QueryCorrispondente queryCorrispondente);

        [OperationContract]
        void inviaNotificaMail(DocsPaVO.amministrazione.OrgUO theUO, DocsPaVO.utente.Amministrazione amm, string descrizioneAOO, string tipoOperazione, string oldCodiceUO);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmSpostaUO(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgUO uoDaSpostare, DocsPaVO.amministrazione.OrgUO uoPadre);

        [OperationContract]
        IList<DocsPaVO.amministrazione.OrgUtente> AmmGetListUtentiRuolo(string idRuolo);

        [OperationContract]
        IList<DocsPaVO.amministrazione.OrgUtente> AmmGetListUtenti(string idAmm, string ricercaPer, string testoDaRicercare, string IDesclusi);

        [OperationContract]
        string GetCodLiv(string codliv, string livello, string codAmm, string idTitolario, string idRegistro);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmInsertTitolario(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente, ref DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario, string idAmm);

        [OperationContract]
        DocsPaVO.PianoConservazione GetPianoConservazioneById(string idPianoConservazione, DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.utente.RoleHistoryResponse GetRoleHistory(DocsPaVO.utente.RoleHistoryRequest request);

        [OperationContract]
        string AmmGetVisibNodoTit_InRuolo(string idNodoTitolario, string idGruppo);

        [OperationContract]
        DocsPaVO.LibroFirma.IstanzaProcessoDiFirma[] GetIstanzeProcessiDiFirmaByTitolarePaging(string idRuoloTitolare, string idUtenteTitolare, int numPage, int pageSize, out int numTotPage, out int nRec);

        [OperationContract]
        OrgRuolo GetRole(string idCorrGlobRuolo);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmEliminaUO(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente, string idCorrGlob);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmOnlyDisabledRole(DocsPaVO.amministrazione.InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRuolo ruolo);

        [OperationContract]
        DocsPaVO.utente.Amministrazione AmmEliminaUoTIBCO(DocsPaVO.amministrazione.OrgUO theUO, out bool result);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmEliminaRuolo(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgRuolo ruolo);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmVerificaUtenteRespStampaRep(string userId, string roleId, string idAmm);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmEliminaADLUtente(string idPeople, string idCorrGlobGruppo);

        [OperationContract]
        void estendiDirittiRuoloACampiDoc(ArrayList listaDirittiRuoli, ArrayList listaCampi);

        [OperationContract]
        DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma[] getRuoliStatiDiagramma(int idDiagramma);

        [OperationContract]
        bool ModifyRuoloStatiDiagramma(DocsPaVO.DiagrammaStato.Visibility.AssRuoloStatiDiagramma[] listAssRoleStatiDia);


        [OperationContract]
        string GetLivelloTipoRuolo(string idCorrGlobRuolo);


        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmEstendeVisibRuolo(InfoUtenteAmministratore infoUtente, string idRegistro, string idCorrGlobRuolo, string idGruppo, string idCorrGlobUO, string idAmm, string livelloRuolo, bool escludiAtipicita);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Templates aggiungiOggettoCustomTemplateFasc(DocsPaVO.ProfilazioneDinamica.OggettoCustom oggettoCustom, DocsPaVO.ProfilazioneDinamica.Templates template);




        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo AmmUpdateMailRegistro(string idRegistro, CasellaRegistro[] caselle);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmAbilitaUtente(InfoUtenteAmministratore infoUtente, string idPeople, string idAmm);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmDisabilitaUtente(InfoUtenteAmministratore infoUtente, string idPeople, string idAmm);

        [OperationContract]
        DocsPaVO.Report.PrintReportResponse GetReportRegistry(String contextName);

        [OperationContract]
        DocsPaVO.Report.PrintReportResponse GenerateReport(DocsPaVO.Report.PrintReportRequest request);

        [OperationContract]
        DocsPaVO.rubrica.ElementoRubrica rubricaGetElementoRubrica(string cod, InfoUtenteAmministratore u, DocsPaVO.rubrica.SmistamentoRubrica smistamentoRubrica, string condRegistri);

        [OperationContract]
        bool ImportPianoConservazione(byte[] dati, string idTitolario, string idAmm, string idRegistro, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.documento.FileDocumento ExportPianoConservazione(DocsPaVO.amministrazione.OrgTitolario titolario, string idRegistro);

        [OperationContract]
        //ArrayList GetLogImportPianoConservazione();
        IList<string> GetLogImportPianoConservazione();

        [OperationContract]
        IList<DocsPaVO.trasmissione.RagioneTrasmissione> TrasmissioneGetRagioniATutti(DocsPaVO.trasmissione.Diritti diritti);

        [OperationContract]
        //ArrayList GetLogImportTabellaConfineIntegrazioni();
        IList<string> GetLogImportTabellaConfineIntegrazioni();

        [OperationContract]
        bool CheckConnection(out string exceptionMessage);

        [OperationContract]
        IList<PR_Registro> DO_GetRegistriAoo(int idAmm);

        [OperationContract]
        void estendiDirittiRuoloACampiFasc(ArrayList listaDirittiRuoli, ArrayList listaCampi);

        [OperationContract]
        SaveRegistroRepertorioSettingsResponse SaveRegisterSettings(SaveRegistroRepertorioSettingsRequest request);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmUpdateTitolario(InfoUtenteAmministratore infoUtente, DocsPaVO.amministrazione.OrgNodoTitolario nodoTitolario);

        [OperationContract]
        DocsPaVO.documento.FileDocumento GetReportProcessiFirma(string idRuolo, string idUtente, string formato);

        [OperationContract]
        bool DelegaVerificaUnicaAssegnataAmm(DocsPaVO.Deleghe.InfoDelega delega);

        [OperationContract]
        bool DelegaCreaNuova(InfoUtenteAmministratore infoUtente, DocsPaVO.Deleghe.InfoDelega delega);

        [OperationContract]
        bool DelegaRevoca(InfoUtenteAmministratore infoUtente, DocsPaVO.Deleghe.InfoDelega[] listaDeleghe, out string msg);

        [OperationContract]
        bool DelegaModificaAmm(DocsPaVO.Deleghe.InfoDelega delegaOld, DocsPaVO.Deleghe.InfoDelega delegaNew, string tipoDelega, string dataScadenzaOld, string dataDecorrenzaOld);

        [OperationContract]
        ArrayList UtenteGetRegistriWithRf(string idCorrGlobali, string all, string idAooColl);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo SaveSupportedFileType(ref DocsPaVO.FormatiDocumento.SupportedFileType fileType);

        [OperationContract]
        void eliminaModelli(string nomeProfilo, string codiceAmministrazione, string nomeFile, string estensione, DocsPaVO.ProfilazioneDinamica.Templates template);

        [OperationContract]
        DocsPaVO.ProfilazioneDinamica.Templates impostaCampiComuniDoc(DocsPaVO.ProfilazioneDinamica.Templates modello, ArrayList campiComuni);

        [OperationContract]
        bool SalvaCessioneDirittiSuModelliTrasm(DocsPaVO.Modelli_Trasmissioni.ModelloTrasmissione objTrasm);

        [OperationContract]
        bool ExistsTrasmissioniPendentiConWorkflowUtente(string idRuoloInUO, string idPeople);

        [OperationContract]
        DocsPaVO.documento.FileDocumento GetReportTrasmissioniPendentiUtente(string idPeople, string idCorrGlobaliRuolo, string formato);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmSostituzioneUtente(string idPeopleNewUT, string idCorrGlobRuolo);

        [OperationContract]
        bool AmmSostituisciUtentePassiCorrelati(string idRuolo, string idOldPeople, string idNewPeople);

        [OperationContract]
        DocsPaVO.amministrazione.EsitoOperazione AmmAccettaTrasmConWFRuolo(string idCorrGlobRuolo);

        [OperationContract]
        bool StoricizzaPianoConservazione(string idTitolario, string idRegistro, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        DocsPaVO.documento.FileDocumento ExportDettaglioPolicyPARER(string[] idPolicy, string formato, string tipo);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo RemoveSupportedFileType(ref DocsPaVO.FormatiDocumento.SupportedFileType fileType);

        [OperationContract]
        DocsPaVO.Validations.ValidationResultInfo updateChiaveConfig(InfoUtenteAmministratore infoUtente, ChiaveConfigurazione chiave);

        [OperationContract]
        void updateDiagramma(DocsPaVO.DiagrammaStato.DiagrammaStato dg);

        [OperationContract]
        DocsPaVO.fascicolazione.Fascicolo FascicolazioneGetFascicoloByIdNoSecurity(string idFascicolo);

        [OperationContract]
        DocsPaVO.Conservazione.PARER.PolicyFascicoliPARER GetPolicyFascicoliPARERById(string idPolicy);

        [OperationContract]
        bool DeletePolicyFascicoliPARER(string idPolicy, InfoUtenteAmministratore utente);

        [OperationContract]
        bool UpdateStatoPolicyFascicoli(DocsPaVO.Conservazione.PARER.PolicyFascicoliPARER[] listaPolicy, InfoUtenteAmministratore infoUtente);

        [OperationContract]
        bool ammCheckCodiceUODuplicato(string id, string codice, string idAmm);

        [OperationContract]
        bool SaveRuoloRespConservazioneAOO(string idGruppo, string idUtente, string idAmm, string idAOO);

    }



}
