// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public interface IPi3DbContextMappedFunctions
    {
        public static string GetDescCorr(long idCorr)
            => throw new NotSupportedException();

        public static string VarDescribe(long idProfile, string codice)
            => throw new NotSupportedException();

         public static string GetPeopleName(long idPeople)
            => throw new NotSupportedException();

        public static string GetCodRuoloByIdCorr(long idRuoloInUo)
            => throw new NotSupportedException();

        public static string GetPeopleUserId(long idPeople)
            => throw new NotSupportedException();

        public static string CorrCatByTipo(long idProfile, string chaTipoProto, string tipoCorr)
            => throw new NotSupportedException();

        public static string GetSegnaturaRepertorio(long idProfile, long idAmm)
            => throw new NotSupportedException();

        public static string IsCorrispondenteInterno(long idCorrGlobale, long idRegistro)
            => throw new NotSupportedException();

        public static string GetChaImg(long idObject)
           => throw new NotSupportedException();

        public static string GetChaFirmato(long idObject)
           => throw new NotSupportedException();

        public static string GetNomeOriginale(long docNumber)
           => throw new NotSupportedException();

        public static string GetImpronta(long docNumber)
            => throw new NotSupportedException();

        public static string GetImprontaWithAllegati(long docNumber)
            => throw new NotSupportedException();

        public static string CorrCat(long docId,string tipoProto)
            => throw new NotSupportedException();

        public static string GetRegDescr(long sysReg)
            => throw new NotSupportedException();

        public static string GetDescTipoDoc(long idTipologia)
            => throw new NotSupportedException();

        public static string GetCodeProject(long idParent)
             => throw new NotSupportedException();
        
        public static string GetCodTit2(long idParent)  
            => throw new NotSupportedException();
        public static string GetCodTit(long idParent)  
            => throw new NotSupportedException();

        public static string IsOggettoModificato(long docNumber)
            => throw new NotSupportedException();

        public static string ClassCat(long docNumber)
            => throw new NotSupportedException();

        public static string GetDescTitolario(long idTit)
            => throw new NotSupportedException();

        public static string GetCodRegCorcat(long idRuolo)
            => throw new NotSupportedException();

        public static string GetValProfObjPrj(long prjId,long customObjectId)
            => throw new NotSupportedException();
        
        public static string HasChildren(long corrId,string tipoURP)
            => throw new NotSupportedException();

        public static string GetCodReg(long idReg)
            => throw new NotSupportedException();

        public static string MailENoteCorrEsterni(long myIdCorr)
            => throw new NotSupportedException();

        public static string GetChaConsentiClass(long pIdParent,string pChaTipoProj,long pIdFasc)
            => throw new NotSupportedException();

        public static string GetChaConsentiFasc(long pIdParent, string pChaTipoProj, string pChaTipoFasc, long pIdFasc)
            => throw new NotSupportedException();
        
        public static string GetInAdl(long sysId, string typeId, long idGruppo, long idPeople)
            => throw new NotSupportedException();

        public static int IsVersionVisible(long versionId, long idPeople, long idGroup)
            => throw new NotSupportedException();

        public static string GetTestoUltimaNota(string tipoOggettoAssociato, 
                long idOggettoAssociato, long idRuoloInUO, 
                long idUtenteCreatore, long idRuoloCreatore)
            => throw new NotSupportedException();

        public static int GetInConservazione(long idProfile, long idProject, string typeId, long idPeople, long idGruppo)
            => throw new NotSupportedException();

        public static string GetDescTipoFasc(long idAtto)
            => throw new NotSupportedException();

        public static DateTime GetDateInADL(long sysId, string typeId, long idGruppo, long idPeople)
            => throw new NotSupportedException();

        public static string GetMotivoADL(long idOggetto, string typeId, long idGruppo, long idPeople)
            => throw new NotSupportedException();

        public static string GetTipologiaFascicoloPianoCons(long idPianoConservazione)
            => throw new NotSupportedException();

        public static string EsisteNotaVisibile(string tipoOggettoAssociato,
                    long idOggettoAssociato, long idRuoloInUO,
                    long idUtenteCreatore, long idRuoloCreatore)
            => throw new NotSupportedException();

        public static string GetStatoConservazioneFasc(long idProject)
            => throw new NotSupportedException();

        public static string GetDiagrammiStato(long docNumberOrIdProject, string tipo)
            => throw new NotSupportedException();

        public static string GetValProfObjsPrjAsJson(long idProject)
            => throw new NotSupportedException();

        public static string GetInConservazioneNoSec(long idProfile, long idProject, string typeId)
            => throw new NotSupportedException();

        public static string GetContatoreFasc(long idProject, string tipoContatore)
            => throw new NotSupportedException();

        public static string GetEsitoPubblicazione(long idProfile)
            => throw new NotSupportedException();

        public static DateTime? GetDataArrivoDoc(long docNumber)
            => throw new NotSupportedException();

        public static string GetChaTipoFirma(long docNumber)
            => throw new NotSupportedException();

        public static string GetContatoreDoc(long docNumber, string tipoContatore)
            => throw new NotSupportedException();
        public static string GetContatoreDoc2(long docNumber, string tipoContatore, long objCustomId)
            => throw new NotSupportedException();

        public static string GetImprontaWithAttachSearch(long docNumber)
            => throw new NotSupportedException();

        public static string GetEsitoSpedizione(long idDocument)
            => throw new NotSupportedException();

        public static string GetCountRicevuteInterop(long idDocument, string tipoRicevuta)
           => throw new NotSupportedException();

        public static string GetStatoConservazione(long idProfile)
            => throw new NotSupportedException();

        public static string GetPolicyVersamentoCod(long idDoc)
            => throw new NotSupportedException();

        public static string GetPolicyVersamentoCounter(long idDoc)
            => throw new NotSupportedException();

        public static string GetPolicyVersamentoDataExec(long idDoc)
            => throw new NotSupportedException();

        public static string GetValProfObjsDocAsJson(long idProfile)
           => throw new NotSupportedException();

        public static int Contains(string column, string text) 
            => throw new NotSupportedException();

        public static string GetValCampoProfDoc(long docNumber, long customObjectId)
            => throw new NotSupportedException();

        public static string GetContatoreFascContatore(long systemId, char tipoCont)
            => throw new NotSupportedException();

        public static string GetValCampoProfDocOrder(long docNumber, long customObjectId)
            => throw new NotSupportedException();

        public static string GetContatoreDocOrdinamento(long docNumber,char tipoCont)
            => throw new NotSupportedException();

        public static string GetValProfObjPrjOrder(long projectId,long customObjId)
            => throw new NotSupportedException();

        public static string GetCodUo(long idUo)
            => throw new NotSupportedException();

        public static string GetCodiceRfByProfileId(long systemIdProfile)
            => throw new NotSupportedException();

        public static long GetIdAmm(long idPeople)
            => throw new NotSupportedException();

        public static long CountAllegatiByDocNumber(long Id)
            => throw new NotSupportedException();

        public static string IsDocCartaceo(long docnum)
            => throw new NotSupportedException();
        public static string AtLeastOneFirmato(long docnum)
            => throw new NotSupportedException();

        public static string AtLeastOneMarcato(long iddoc)
            => throw new NotSupportedException();

        public static long EsisteDestinatarioMaiTrasmesso(long iddoc)
            => throw new NotSupportedException();

        public static long EsisteDestinatarioMaiSpedito(long iddoc)
            => throw new NotSupportedException();

        public static int CompareDate(string date, DateTime dateFrom, DateTime? dateTo = null)
            => throw new NotSupportedException();

        public static long GetValCampoProfDocOrderToNumber(long docNumber, long customObjectId)
            => throw new NotSupportedException();

        public static DateTime GetValCampoProfDocToDate(long docNumber, long customObjectId)
            => throw new NotSupportedException();
    }
}
