// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ProfilazioneDinamicaLite;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record GetDocumentListVersionsResult(DocsPaVO.documento.SchedaDocumento SchedaDocumento);
    public record GetDocumentListVersions(DocsPaVO.utente.InfoUtente infoutente, string idProfile, string docNumber) : IRequest<GetDocumentListVersionsResult>;
    public record IsVersionWithSegnatureResult(bool output); public record IsVersionWithSegnature(DocsPaVO.utente.InfoUtente infoUtente, string versionId) : IRequest<IsVersionWithSegnatureResult>;
    public record GetConvertiblePdfFileTypesResult(string[] output); 
    public record GetConvertiblePdfFileTypes() : IRequest<GetConvertiblePdfFileTypesResult>;
    public record CanConvertFileToPdfResult(bool output); 
    public record CanConvertFileToPdf(string fileName) : IRequest<CanConvertFileToPdfResult>;
    public record DocumentoGetFileConSegnaturaUsingLCResult(DocsPaVO.documento.FileDocumento output); 
    public record DocumentoGetFileConSegnaturaUsingLC(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.documento.SchedaDocumento sch, DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.labelPdf position, bool Forced) : IRequest<DocumentoGetFileConSegnaturaUsingLCResult>;
    public record DocumentoGetFileAsPdfFormatResult(DocsPaVO.documento.FileDocumento output, bool isConverted); 
    public record DocumentoGetFileAsPdfFormat(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<DocumentoGetFileAsPdfFormatResult>;
    public record DocumentoGetFileFirmatoResult(DocsPaVO.documento.FileDocumento output); 
    public record DocumentoGetFileFirmato(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<DocumentoGetFileFirmatoResult>;
    public record DocumentoGetFileDocResult(DocsPaVO.documento.FileDocumento output, string msgErr); 
    public record DocumentoGetFileDoc(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<DocumentoGetFileDocResult>;
    public record DocumentoGetFileDocAsEMLResult(DocsPaVO.documento.FileDocumento output, string msgErr); 
    public record DocumentoGetFileDocAsEML(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<DocumentoGetFileDocAsEMLResult>;

}
