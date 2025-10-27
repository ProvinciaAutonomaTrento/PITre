// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.CheckInOut;
using DocsPaVO.DiagrammaStato;
using DocsPaVO.documento;
using DocsPaVO.ProfilazioneDinamicaLite;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record VerificaACLResult(int output, string errorMessage); 
    public record VerificaACL(string tipoObj, string idObj, InfoUtente infoUtente) : IRequest<VerificaACLResult>;
    public record GetCheckOutDocumentStatusResult(CheckOutStatus output); 
    public record GetCheckOutDocumentStatus(string idDocument, string documentNumber, InfoUtente infoUtente) : IRequest<GetCheckOutDocumentStatusResult>;
    public record GetVersionsMainDocumentResult(DocsPaVO.documento.Documento[] output); 
    public record GetVersionsMainDocument(InfoUtente infoUser, string docNumber) : IRequest<GetVersionsMainDocumentResult>;
    public record DocumentoGetAllegatiResult(DocsPaVO.documento.Allegato[] output); 
    public record DocumentoGetAllegati(string docNumber, string filterAllegatiPec, string simplifiedInteroperabilityId) : IRequest<DocumentoGetAllegatiResult>;
    public record PutElectronicSignatureResult(bool output, string message); 
    public record PutElectronicSignature(DocsPaVO.documento.FileRequest approvingFile, InfoUtente infoUtente, bool isAdvancementProcess) : IRequest<PutElectronicSignatureResult>;
    public record Albo_GetStatiSelezioneFilesResult(String[] output); 
    public record Albo_GetStatiSelezioneFiles(string idDiagramma) : IRequest<Albo_GetStatiSelezioneFilesResult>;
    public record getStatoDocResult(Stato output); 
    public record getStatoDoc(string docNumber) : IRequest<getStatoDocResult>;
    public record DocumentoRimuoviAllegatoResult(bool output); public record DocumentoRimuoviAllegato(DocsPaVO.documento.Allegato allegato, InfoUtente infoUtente, DocsPaVO.documento.SchedaDocumento schedaDocumento) : IRequest<DocumentoRimuoviAllegatoResult>;
    public record DocumentoGetDettaglioDocumentoResult(DocsPaVO.documento.SchedaDocumento output); 
    public record DocumentoGetDettaglioDocumento(InfoUtente infoutente, string idProfile, string docNumber) : IRequest<DocumentoGetDettaglioDocumentoResult>;
    public record isFiltroAooEnabledResult(bool output); public record isFiltroAooEnabled() : IRequest<isFiltroAooEnabledResult>;
    public record UtenteGetRegistriResult(Registro[] output); public record UtenteGetRegistri(string idCorrGlobali) : IRequest<UtenteGetRegistriResult>;
    public record getAccessRightDocBySystemIDResult(string output); public record getAccessRightDocBySystemID(string idprofile, InfoUtente infoUtente) : IRequest<getAccessRightDocBySystemIDResult>;
    public record DocumentoScambiaAllegatoResult(bool output); public record DocumentoScambiaAllegato(InfoUtente infoUtente, Allegato allegato, Documento documento) : IRequest<DocumentoScambiaAllegatoResult>;
    public record DocumentoModificaAllegatoResult(bool output); public record DocumentoModificaAllegato(InfoUtente infoUtente, Allegato allegato, string idDocumentoPrincipale) : IRequest<DocumentoModificaAllegatoResult>;
}
