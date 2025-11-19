// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.addressbook;
using DocsPaVO.amministrazione;
using DocsPaVO.areaConservazione;
using DocsPaVO.Deleghe;
using DocsPaVO.documento;
using DocsPaVO.ExportData;
using DocsPaVO.filtri;
using DocsPaVO.InstanceAccess;
using DocsPaVO.LibroFirma;
using DocsPaVO.Modelli_Trasmissioni;
using DocsPaVO.Note;
using DocsPaVO.ProfilazioneDinamica;
using DocsPaVO.ProspettiRiepilogativi;
using DocsPaVO.ricerche;
using DocsPaVO.trasmissione;
using DocsPaVO.utente;
using DocsPaVO.utente.Repertori;
using DocsPaVO.utente.Repertori.RequestAndResponse;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record LogoffOtherSessionsResult(bool output);

    public record LogoffOtherSessions(string userId, string idAmm, string sessionId) : IRequest<LogoffOtherSessionsResult>;

    public record getModelliUtenteResult(object[] output);

    public record getModelliUtente(Utente utente, InfoUtente infoUt, FiltroRicerca[] filtriRicerca) : IRequest<getModelliUtenteResult>;
    public record CancellaModelloResult();

    public record CancellaModello(string idAmm, string idModello) : IRequest<CancellaModelloResult>;
    public record getCodiceListaResult(string output);

    public record getCodiceLista(string idLista) : IRequest<getCodiceListaResult>;
    public record TrasmissioneGetRagioniATuttiResult(RagioneTrasmissione[] output);

    public record TrasmissioneGetRagioniATutti(Diritti diritti) : IRequest<TrasmissioneGetRagioniATuttiResult>;
    public record getCorrispondentiListaResult(System.Data.DataSet output);

    public record getCorrispondentiLista(string codiceLista) : IRequest<getCorrispondentiListaResult>;
    public record deleteListaDistribuzioneResult();

    public record deleteListaDistribuzione(string codiceLista) : IRequest<deleteListaDistribuzioneResult>;

    public record DO_GetSediResult(ArrayList output);
    public record DO_GetSedi() : IRequest<DO_GetSediResult>;

    public record getListePerRuoloUtResult(System.Data.DataSet output);

    public record getListePerRuoloUt(InfoUtente infoUtente) : IRequest<getListePerRuoloUtResult>;

    public record FattElAttiveGetFornitoriResult(DocsPaVO.ExternalServices.FornitoreFattAttiva[] output);
    public record FattElAttiveGetFornitori(DocsPaVO.utente.InfoUtente infoUtente) : IRequest<FattElAttiveGetFornitoriResult>;
    public record UtentiConNotificaTrasmResult(ModelloTrasmissione output);

    public record UtentiConNotificaTrasm(ModelloTrasmissione objModTrasm, object[] utentiDaInserire, object[] utentiDaCancellare, string operazione) : IRequest<UtentiConNotificaTrasmResult>;
    public record SalvaCessioneDirittiSuModelliTrasmResult(bool output);

    public record SalvaCessioneDirittiSuModelliTrasm(ModelloTrasmissione objTrasm) : IRequest<SalvaCessioneDirittiSuModelliTrasmResult>;
    public record AmmGetRagioneTrasmissioneResult(OrgRagioneTrasmissione output);

    public record AmmGetRagioneTrasmissione(string idRagione) : IRequest<AmmGetRagioneTrasmissioneResult>;

    public record GetInstanceAccessResult(InstanceAccess[] output);

    public record GetInstanceAccess(string idPeople, string idGroup, InfoUtente infoUtente) : IRequest<GetInstanceAccessResult>;
    public record GetInstanceAccessByIdResult(InstanceAccess output);

    public record GetInstanceAccessById(string idInstanceAccess, InfoUtente infoUtente) : IRequest<GetInstanceAccessByIdResult>;
    public record InsertInstanceAccessResult(InstanceAccess output);

    public record InsertInstanceAccess(InstanceAccess instanceAccess, InfoUtente infoUtente) : IRequest<InsertInstanceAccessResult>;

    public record InsertInstanceAccessDocumentsResult(bool output);

    public record InsertInstanceAccessDocuments(InstanceAccessDocument[] listInstanceAccessDocuments, InfoUtente infoUtente) : IRequest<InsertInstanceAccessDocumentsResult>;
    public record RemoveInstanceAccessDocumentsResult(bool output);

    public record RemoveInstanceAccessDocuments(InstanceAccessDocument[] listInstanceAccessDocuments, InfoUtente infoUtente) : IRequest<RemoveInstanceAccessDocumentsResult>;
    public record RemoveInstanceAccessAttachmentsResult(bool output);

    public record RemoveInstanceAccessAttachments(InstanceAccessAttachments[] listInstanceAccessAttachments, InfoUtente infoUtente) : IRequest<RemoveInstanceAccessAttachmentsResult>;
    public record UpdateInstanceAccessResult(InstanceAccess output);

    public record UpdateInstanceAccess(InstanceAccess instanceAccess, InfoUtente infoUtente) : IRequest<UpdateInstanceAccessResult>;
    public record UpdateInstanceAccessDocumentsResult(bool output);

    public record UpdateInstanceAccessDocuments(InstanceAccessDocument[] listInstanceAccessDocuments, InfoUtente infoUtente) : IRequest<UpdateInstanceAccessDocumentsResult>;
    public record AsyncCreateDownloadResult();

    public record AsyncCreateDownload(InstanceAccess instanceAccess, InfoUtente infoUtente, Ruolo ruolo) : IRequest<AsyncCreateDownloadResult>;
    public record GetStateDownloadInstanceAccessResult(string output);

    public record GetStateDownloadInstanceAccess(string idInstanceAccess, InfoUtente infoUtente) : IRequest<GetStateDownloadInstanceAccessResult>;
    public record ConservazioneGetInfoByFiltroResult(InfoConservazione[] output);

    public record ConservazioneGetInfoByFiltro(string filtro) : IRequest<ConservazioneGetInfoByFiltroResult>;
    public record CreateDeclarationDocumentResult(InstanceAccess output);

    public record CreateDeclarationDocument(InstanceAccess instance, InfoUtente infoUser, Ruolo role) : IRequest<CreateDeclarationDocumentResult>;
    public record UpdateInstanceAccessDocumentEnableResult(bool output);

    public record UpdateInstanceAccessDocumentEnable(InstanceAccessDocument[] listInstanceAccessDocument, InfoUtente infoUtente) : IRequest<UpdateInstanceAccessDocumentEnableResult>;
    public record UpdateInstanceAccessAttachmentsEnableResult(bool output);

    public record UpdateInstanceAccessAttachmentsEnable(InstanceAccessAttachments[] listInstanceAccessAttachments, InfoUtente infoUtente) : IRequest<UpdateInstanceAccessAttachmentsEnableResult>;
    public record GetTemplateInstanceAccessResult(Templates output);

    public record GetTemplateInstanceAccess(InfoUtente infoUtente) : IRequest<GetTemplateInstanceAccessResult>;
    public record ForwardsInstanceAccessResult(SchedaDocumento output, int totalFileSizeInstance);

    public record ForwardsInstanceAccess(DocsPaVO.InstanceAccess.InstanceAccess instance, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Ruolo ruolo) : IRequest<ForwardsInstanceAccessResult>;
    public record GetEventNotificationResult(AnagraficaEventi[] output);

    public record GetEventNotification(InfoUtente infoUtente) : IRequest<GetEventNotificationResult>;

    public record GetEventTypesResult(AnagraficaEventi[] output);

    public record GetEventTypes(string eventType, InfoUtente infoUtente) : IRequest<GetEventTypesResult>;
    public record GetProcessiFirmaByFilterResult(ProcessoFirma[] output);

    public record GetProcessiFirmaByFilter(FiltroProcessoFirma[] filtro, InfoUtente infoUtente) : IRequest<GetProcessiFirmaByFilterResult>;
    public record GetListaNoteResult(NotaElenco[] output, int numNote);

    public record GetListaNote(InfoUtente infoUtente, string idRF, string descNota) : IRequest<GetListaNoteResult>;

    public record GetElencoNote(string contextKey, string prefixText) : IRequest<GetElencoNoteResult>;

    public record GetElencoNoteResult(string[] output);


    public record isUniqueCodResult(bool output);

    public record isUniqueCod(string codLista, string idAmm) : IRequest<isUniqueCodResult>;
    public record StampaOrgInPdfResult(FileDocumento output);

    public record StampaOrgInPdf(XmlDocument xmlDoc) : IRequest<StampaOrgInPdfResult>;
    public record AmmGetDatiUOCorrenteResult(OrgUO output);

    public record AmmGetDatiUOCorrente(string idUO) : IRequest<AmmGetDatiUOCorrenteResult>;
    public record AmmGetListUOResult(OrgUO[] output);

    public record AmmGetListUO(string idParent, string livello, string idAmm) : IRequest<AmmGetListUOResult>;
    public record AmmGetListRuoliUOResult(OrgRuolo[] output);

    public record AmmGetListRuoliUO(string idUO) : IRequest<AmmGetListRuoliUOResult>;
    public record AmmGetListUtentiRuoloResult(OrgUtente[] output);

    public record AmmGetListUtentiRuolo(string idRuolo) : IRequest<AmmGetListUtentiRuoloResult>;
    public record AmmGetListTipiRuoloResult(OrgTipoRuolo[] output);

    public record AmmGetListTipiRuolo(string idAmm) : IRequest<AmmGetListTipiRuoloResult>;
    public record AmmRicercaInOrgResult(OrgRisultatoRicerca[] output);

    public record AmmRicercaInOrg(string tipo, string codice, string descrizione, string idAmm, bool searchHistoricized, bool searchByCodeExact) : IRequest<AmmRicercaInOrgResult>;
    public record AmmListaIDParentRicercaResult(object[] output);

    public record AmmListaIDParentRicerca(string IDPartenza, string tipo) : IRequest<AmmListaIDParentRicercaResult>;
    public record GetRoleResult(OrgRuolo output);

    public record GetRole(string idCorrGlobRuolo) : IRequest<GetRoleResult>;
    public record AmmGetListaRuoliAOOResult(OrgRuolo[] output);

    public record AmmGetListaRuoliAOO(string idRegistro) : IRequest<AmmGetListaRuoliAOOResult>;
    public record ReportTitolarioResult(FileDocumento output);

    public record ReportTitolario(InfoUtente infoUtente, DocsPaVO.utente.Registro registro) : IRequest<ReportTitolarioResult>;
    public record isUniqueNomeListaResult(bool output);

    public record isUniqueNomeLista(string nomeLista, string idAmm) : IRequest<isUniqueNomeListaResult>;
    public record stampaReportAvanzatiXLSResult(FileDocumento output);

    public record stampaReportAvanzatiXLS(ExportExcelClass objStampaReport) : IRequest<stampaReportAvanzatiXLSResult>;

    public record AmmGetTipiRuoloResult(OrgTipoRuolo[] output);

    public record AmmGetTipiRuolo(string codiceAmministrazione) : IRequest<AmmGetTipiRuoloResult>;
    public record getTemplatePerRicercaResult(Templates output);

    public record getTemplatePerRicerca(string idAmministrazione, string tipoAtto) : IRequest<getTemplatePerRicercaResult>;
    public record GeneratePrintRepertorioResult(GeneratePrintRepertorioResponse output);

    public record GeneratePrintRepertorio(GeneratePrintRepertorioRequest request) : IRequest<GeneratePrintRepertorioResult>;
    public record ChangeRepertorioStateResult(ChangeRepertorioStateResponse output);

    public record ChangeRepertorioState(ChangeRepertorioStateRequest request) : IRequest<ChangeRepertorioStateResult>;
    public record GetRegistriesWithAooOrRfResult(RegistroRepertorio[] output);

    public record GetRegistriesWithAooOrRf(string idRoleResp, string idRolePrinter) : IRequest<GetRegistriesWithAooOrRfResult>;
    public record GetRepertoriPrintRangesResult(GetRepertoriPrintRangesResponse output);

    public record GetRepertoriPrintRanges(GetRepertoriPrintRangesRequest request, bool repairBrokenPrint) : IRequest<GetRepertoriPrintRangesResult>;
    public record RemoveReportMailboxResult(bool output);

    public record RemoveReportMailbox(string idCheckMailbox) : IRequest<RemoveReportMailboxResult>;
    public record modificaListaCorrResult();

    public record modificaListaCorr(System.Data.DataSet dsCorrLista, string idLista) : IRequest<modificaListaCorrResult>;

    public record RegistriCambiaStatoResult(DocsPaVO.utente.Registro output);

    public record RegistriCambiaStato(InfoUtente infoutente, DocsPaVO.utente.Registro registro) : IRequest<RegistriCambiaStatoResult>;
    public record RegistriStampaResult(StampaRegistroResult output);

    public record RegistriStampa(InfoUtente infoUtente, Ruolo ruolo, DocsPaVO.utente.Registro registro) : IRequest<RegistriStampaResult>;
    public record DocumentoGetDocInCestinoResult(InfoDocumento[] output);

    public record DocumentoGetDocInCestino(InfoUtente infoUtente) : IRequest<DocumentoGetDocInCestinoResult>;
    public record DocumentoGetDocInCestinoFiltroResult(InfoDocumento[] output);

    public record DocumentoGetDocInCestinoFiltro(InfoUtente infoUtente, FiltroRicerca[][] filtriRicerca) : IRequest<DocumentoGetDocInCestinoFiltroResult>;
    public record InfoCheckMailboxResult(DocsPaVO.Interoperabilita.InfoCheckMailbox[] output);

    public record InfoCheckMailbox(string[] emails) : IRequest<InfoCheckMailboxResult>;
    public record CheckScaricoOtherRoleResult(bool output);

    public record CheckScaricoOtherRole(string idReg, string email) : IRequest<CheckScaricoOtherRoleResult>;
    public record CheckMailBoxResult(bool output);

    public record CheckMailBox(string serverName, DocsPaVO.utente.Registro reg, Utente ut, Ruolo ruolo, string idCheckMailbox) : IRequest<CheckMailBoxResult>;

    public record StartCheckMailBoxResult(string output);
    public record StartCheckMailBox(string serverName, DocsPaVO.utente.Registro reg, Utente ut, Ruolo ruolo) : IRequest<StartCheckMailBoxResult>;

    public record InsertProcessoDiFirmaResult(ProcessoFirma output, ResultProcessoFirma resultCreazioneProcesso);

    public record InsertProcessoDiFirma(ProcessoFirma processoDiFirma, InfoUtente infoUtente) : IRequest<InsertProcessoDiFirmaResult>;
    public record InserisciPassoDiFirmaResult(PassoFirma output);

    public record InserisciPassoDiFirma(PassoFirma passo, InfoUtente infoUtente) : IRequest<InserisciPassoDiFirmaResult>;

    public record InsertPassoDiFirmaResult(PassoFirma output);
    public record InsertPassoDiFirma(PassoFirma passo, InfoUtente infoUtente) : IRequest<InsertPassoDiFirmaResult>;

    public record modificaListaGruppoResult();

    public record modificaListaGruppo(System.Data.DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista, string idGruppo) : IRequest<modificaListaGruppoResult>;
    public record getRuoloOrUserListaResult(string output);

    public record getRuoloOrUserLista(string idLista) : IRequest<getRuoloOrUserListaResult>;

    public record RimuoviProcessoDiFirmaResult(bool output);

    public record RimuoviProcessoDiFirma(ProcessoFirma processo, InfoUtente infoUtente) : IRequest<RimuoviProcessoDiFirmaResult>;

    public record RimuoviPassoDiFirmaResult(bool output);

    public record RimuoviPassoDiFirma(PassoFirma passo, InfoUtente infoUtente) : IRequest<RimuoviPassoDiFirmaResult>;
    public record AggiornaPassoDiFirmaResult(bool output);

    public record AggiornaPassoDiFirma(PassoFirma passo, int oldNumeroSequenza, InfoUtente infoUtente) : IRequest<AggiornaPassoDiFirmaResult>;
    public record AggiornaProcessoDiFirmaResult(ProcessoFirma output, ResultProcessoFirma resultCreazioneProcesso);

    public record AggiornaProcessoDiFirma(ProcessoFirma processoDiFirma, InfoUtente infoUtente) : IRequest<AggiornaProcessoDiFirmaResult>;
    public record IsEventoAutomaticoResult(bool output);

    public record IsEventoAutomatico(string codiceEvento) : IRequest<IsEventoAutomaticoResult>;
    public record GetTypeRoleResult(TipoRuolo[] output);

    public record GetTypeRole(InfoUtente infoUtente) : IRequest<GetTypeRoleResult>;
    public record GetAnagraficaEventoByCodiceResult(AnagraficaEventi output);

    public record GetAnagraficaEventoByCodice(string codiceEvento) : IRequest<GetAnagraficaEventoByCodiceResult>;
    public record GetIstanzeProcessiInErroreResult(IstanzaProcessoDiFirma[] output);

    public record GetIstanzeProcessiInErrore(string[] idIstanzeProcessi, InfoUtente infoUtente) : IRequest<GetIstanzeProcessiInErroreResult>;
    public record RitentaIstanzeProcessiInErroreResult(bool output);

    public record RitentaIstanzeProcessiInErrore(IstanzaProcessoDiFirma[] istanzeProcessi, InfoUtente infoUtente) : IRequest<RitentaIstanzeProcessiInErroreResult>;
    public record EliminaDocResult(bool output);

    public record EliminaDoc(InfoUtente infoUtente, InfoDocumento infoDoc) : IRequest<EliminaDocResult>;
    public record modificaListaUserResult();

    public record modificaListaUser(System.Data.DataSet dsCorrLista, string idLista, string nomeLista, string codiceLista, string idUtente) : IRequest<modificaListaUserResult>;
    public record DO_GetAmministrazioniResult(object[] output);

    public record DO_GetAmministrazioni() : IRequest<DO_GetAmministrazioniResult>;
    public record DO_GetRegistriResult(object[] output);

    public record DO_GetRegistri(int idAmm) : IRequest<DO_GetRegistriResult>;
    public record DO_GetDSReportAnnualeByRegResult(System.Data.DataSet output);

    public record DO_GetDSReportAnnualeByReg(int id_amm, int idReg, int anno, int mese, string sede, bool simpleSP, int titolario) : IRequest<DO_GetDSReportAnnualeByRegResult>;
    public record ReportCorrispondentiResult(FileDocumento output);

    public record ReportCorrispondenti(QueryCorrispondente queryCorrispondente, InfoUtente infoUtente) : IRequest<ReportCorrispondentiResult>;
    public record DO_GetDSReportDocClassResult(System.Data.DataSet output);

    public record DO_GetDSReportDocClass(int idReg, int anno, int idamm, string sede, int titolario) : IRequest<DO_GetDSReportDocClassResult>;

    public record DO_GetDSReportDocClassCompactResult(System.Data.DataSet output);

    public record DO_GetDSReportDocClassCompact(int idReg, int anno, int idamm, string sede, int titolario) : IRequest<DO_GetDSReportDocClassCompactResult>;
    public record DO_GetCountReportDocClassCompactResult(string output);

    public record DO_GetCountReportDocClassCompact(int idReg, int anno, int idamm, string sede, int titolario) : IRequest<DO_GetCountReportDocClassCompactResult>;
    public record DO_GetCountDistinctReportDocClassCompactResult(string output);

    public record DO_GetCountDistinctReportDocClassCompact(int idReg, int anno, int idAmm, string sede, int titolario) : IRequest<DO_GetCountDistinctReportDocClassCompactResult>;

    public record ExportRubricaResult(FileDocumento output);

    public record ExportRubrica(InfoUtente infoUtente, bool store, string registri) : IRequest<ExportRubricaResult>;
    public record DO_GetDSReportDocTrasmToAOOResult(System.Data.DataSet output);

    public record DO_GetDSReportDocTrasmToAOO(int idReg, int anno, int idAmm) : IRequest<DO_GetDSReportDocTrasmToAOOResult>;
    public record getNodiFromProtoTitResult(OrgNodoTitolario[] output);

    public record getNodiFromProtoTit(DocsPaVO.utente.Registro registro, string idAmministrazione, string numProtoPratica, string idTitolario) : IRequest<getNodiFromProtoTitResult>;
    public record DO_GetCountReportDocTrasmToAOOResult(string output);

    public record DO_GetCountReportDocTrasmToAOO(int idReg, int anno, int idAmm) : IRequest<DO_GetCountReportDocTrasmToAOOResult>;
    public record DO_GetDSReportFascicoliPerVTResult(System.Data.DataSet output);

    public record DO_GetDSReportFascicoliPerVT(int idAmm, int idReg, int anno, int mese, int titolario) : IRequest<DO_GetDSReportFascicoliPerVTResult>;
    public record DO_GetDSReportFascicoliPerVTCompactResult(System.Data.DataSet output);

    public record DO_GetDSReportFascicoliPerVTCompact(int idAmm, int idReg, int anno, int mese, int titolario) : IRequest<DO_GetDSReportFascicoliPerVTCompactResult>;
    public record DO_GetDSReportAnnualeByFascResult(System.Data.DataSet output);

    public record DO_GetDSReportAnnualeByFasc(int idAmm, int idReg, int anno, int mese, int titolario, bool simpleSP) : IRequest<DO_GetDSReportAnnualeByFascResult>;
    public record DO_GetDSTempiMediLavorazioneResult(System.Data.DataSet output);

    public record DO_GetDSTempiMediLavorazione(int idAmm, int idReg, int anno, int mese, int titolario) : IRequest<DO_GetDSTempiMediLavorazioneResult>;
    public record DO_GetDSTempiMediLavorazioneCompactResult(System.Data.DataSet output);

    public record DO_GetDSTempiMediLavorazioneCompact(int idAmm, int idReg, int anno, int mese, int titolario) : IRequest<DO_GetDSTempiMediLavorazioneCompactResult>;
    public record DO_GetDSReportDocXSedeResult(System.Data.DataSet output);

    public record DO_GetDSReportDocXSede(int idAmm, int idReg, int anno, int idPeople, string timeStamp) : IRequest<DO_GetDSReportDocXSedeResult>;
    public record DO_GetDSReportDocXUoResult(System.Data.DataSet output);

    public record DO_GetDSReportDocXUo(int idAmm, int idReg, int anno) : IRequest<DO_GetDSReportDocXUoResult>;
    public record DO_GetDSReportContatoriDocResult(System.Data.DataSet output);

    public record DO_GetDSReportContatoriDoc(int idAmm, int anno) : IRequest<DO_GetDSReportContatoriDocResult>;
    public record DO_GetDSReportContatoriFascResult(System.Data.DataSet output);

    public record DO_GetDSReportContatoriFasc(int idAmm, int anno) : IRequest<DO_GetDSReportContatoriFascResult>;
    public record DO_GetDSReportProtocolloArmaResult(System.Data.DataSet output);

    public record DO_GetDSReportProtocolloArma(string idRegistro, int idTitolario, int idAmm) : IRequest<DO_GetDSReportProtocolloArmaResult>;
    public record DO_GetDSReportDettaglioPraticaResult(System.Data.DataSet output);

    public record DO_GetDSReportDettaglioPratica(string idRegistro, int idTitolario, int numPratica, int idAmm) : IRequest<DO_GetDSReportDettaglioPraticaResult>;
    public record DO_GetDSReportGiornaleRiscontriResult(System.Data.DataSet output);

    public record DO_GetDSReportGiornaleRiscontri(string idRegistro, int idAmm) : IRequest<DO_GetDSReportGiornaleRiscontriResult>;
    public record DO_GetDSReportDocSpeditiInteropResult(System.Data.DataSet output);

    public record DO_GetDSReportDocSpeditiInterop(int idAmm, string idRegistro, string dataSpedDa, string dataSpedA, string confermaProt) : IRequest<DO_GetDSReportDocSpeditiInteropResult>;
    public record DO_CDCGetDSReportControlloPreventivoResult(System.Data.DataSet output);

    public record DO_CDCGetDSReportControlloPreventivo(InfoUtente infoUtente, string CDCDataDa, string CDCDataA, string CDCCodiceUffici, string CDCCodiceMagistrato, string CDCCodiceRevisore) : IRequest<DO_CDCGetDSReportControlloPreventivoResult>;
    public record DO_CDCGetDSReportControlloPreventivoSRCResult(System.Data.DataSet output);

    public record DO_CDCGetDSReportControlloPreventivoSRC(InfoUtente infoUtente, string CDCDataDa, string CDCDataA, string CDCCodiceUffici, string CDCCodiceMagistrato, string CDCCodiceRevisore) : IRequest<DO_CDCGetDSReportControlloPreventivoSRCResult>;
    public record DO_CDCGetDSReportPensioniCiviliResult(System.Data.DataSet output);

    public record DO_CDCGetDSReportPensioniCivili(InfoUtente infoUtente, string CDCDataDa, string CDCDataA, string CDCCodiceUffici, string CDCCodiceMagistrato, string CDCCodiceRevisore) : IRequest<DO_CDCGetDSReportPensioniCiviliResult>;
    public record DO_CDCGetDSReportPensioniMilitariResult(System.Data.DataSet output);

    public record DO_CDCGetDSReportPensioniMilitari(InfoUtente infoUtente, string CDCDataDa, string CDCDataA, string CDCCodiceUffici, string CDCCodiceMagistrato, string CDCCodiceRevisore) : IRequest<DO_CDCGetDSReportPensioniMilitariResult>;
    public record DO_GetIdAmmByCodAmmResult(int output);

    public record DO_GetIdAmmByCodAmm(string codAmm) : IRequest<DO_GetIdAmmByCodAmmResult>;
    public record Do_GetVarDescAmmByIdAmmResult(string output);

    public record Do_GetVarDescAmmByIdAmm(int idAmm) : IRequest<Do_GetVarDescAmmByIdAmmResult>;

    public record Do_GetAmmByIdAmmResult(PR_Amministrazione output);

    public record Do_GetAmmByIdAmm(int idAmm) : IRequest<Do_GetAmmByIdAmmResult>;
    public record DO_GetTitolariResult(object[] output);

    public record DO_GetTitolari(int idAmm) : IRequest<DO_GetTitolariResult>;
    public record DO_GetNumDocSpeditiResult(string output);

    public record DO_GetNumDocSpediti(string dataSpedDa, string dataSpedA, string idReg, string confermaProt, int idAmm) : IRequest<DO_GetNumDocSpeditiResult>;

    public record DO_ReadXMLResult(object[] output);

    public record DO_ReadXML() : IRequest<DO_ReadXMLResult>;
    public record DO_StampaReportFascExcelResult(System.Data.DataSet output);

    public record DO_StampaReportFascExcel(string timeStamp, InfoUtente infoUtente, string searchType, string tipo_ricerca, string tipo_scelta, string valore_scelta, bool sottoposti, string dataC, string dataCdal, string dataCal, string dataCh, string dataChdal, string dataChal, string idTitolario) : IRequest<DO_StampaReportFascExcelResult>;
    public record DelegaRevocaResult(bool output, string msg);

    public record DelegaRevoca(InfoUtente infoUtente, InfoDelega[] listaDeleghe) : IRequest<DelegaRevocaResult>;
    public record DelegaSearchResult(InfoDelega[] output, SearchPagingContext pagingContext);

    public record DelegaSearch(InfoUtente utente, SearchDelegaInfo searchInfo, SearchPagingContext pagingContext) : IRequest<DelegaSearchResult>;

    public record DelegaEsercitaResult(Utente output, UserLogin.LoginResult loginResult);

    public record DelegaEsercita(InfoUtente infoUtente, UserLogin login, string webSessionId, string id_delega, string idRuoloDelegante) : IRequest<DelegaEsercitaResult>;
    public record GetUtenteAutomaticoResult(Utente output);

    public record GetUtenteAutomatico(string idAmministrazione) : IRequest<GetUtenteAutomaticoResult>;

    public record DocumentoRiattivaDocResult(bool output);

    public record DocumentoRiattivaDoc(InfoUtente infoUtente, InfoDocumento infoDocumento) : IRequest<DocumentoRiattivaDocResult>;
    public record SvuotaCestinoResult(bool output, bool docInCestino);

    public record SvuotaCestino(InfoUtente infoUtente, InfoDocumento[] ListaDoc) : IRequest<SvuotaCestinoResult>;

    public record GetProcessiDiFirmaResult(ProcessoFirma[] output);

    public record GetProcessiDiFirma(InfoUtente infoUtente) : IRequest<GetProcessiDiFirmaResult>;

    public record salvaListaGruppoResult();

    public record salvaListaGruppo(System.Data.DataSet dsCorrLista, string nomeLista, string codiceLista, string idUtente, string idAmm, string gruppo, InfoUtente infoUtente) : IRequest<salvaListaGruppoResult>;

    public record ExportVisibilitaProcessiFirmaResult(FileDocumento output);

    public record ExportVisibilitaProcessiFirma(InfoUtente infoUtente, string tipologiaExport, string title, List<CampoSelezionato> objects, List<ProcessoFirma> listaProcessiFirma) : IRequest<ExportVisibilitaProcessiFirmaResult>;

    public record DelegaDismettiResult(bool output);
    public record DelegaDismetti(InfoUtente infoUtente, string userIdDelegante) : IRequest<DelegaDismettiResult>;

    public record ExportDistributionListResult(FileDocumento output);

    public record ExportDistributionList(InfoUtente infoUtente, string title, string tipologia, string selectedListId) : IRequest<ExportDistributionListResult>;

    public record GetElementoInRubricaComuneNoEsternaResult(DocsPaVO.utente.Corrispondente output);
    public record GetElementoInRubricaComuneNoEsterna(string codice, InfoUtente u) : IRequest<GetElementoInRubricaComuneNoEsternaResult>;
}
