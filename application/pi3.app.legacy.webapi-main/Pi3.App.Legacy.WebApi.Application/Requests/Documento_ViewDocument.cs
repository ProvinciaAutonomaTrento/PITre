// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.documento;
using DocsPaVO.FormatiDocumento;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record DocumentoGetAnteprimaFilePdfResult(FileDocumentoAnteprima output, string msgErr);

    public record DocumentoGetAnteprimaFilePdf(FileRequest fileRequest, int firstPg,int lastPg, SchedaDocumento sch,	labelPdf position, InfoUtente  infoUtente)	: IRequest<DocumentoGetAnteprimaFilePdfResult>;																																						
    
    public record DocumentoGetFileConSegnaturaResult(FileDocumento output);

    public record DocumentoGetFileConSegnatura(FileRequest fileRequest, SchedaDocumento sch, InfoUtente infoUtente, labelPdf position, bool Forced, bool convertToPdf = false) : IRequest<DocumentoGetFileConSegnaturaResult>;
    public record IsEnabledSupportedFileTypesResult(bool output);

    public record IsEnabledSupportedFileTypes() : IRequest<IsEnabledSupportedFileTypesResult>;
    public record GetSupportedFileTypesResult(SupportedFileType[] output);

    public record GetSupportedFileTypes(int idAmministrazione) : IRequest<GetSupportedFileTypesResult>;
    public record IsTitolareElementoInLFResult(bool output);

    public record IsTitolareElementoInLF(string docNumber, InfoUtente infoUtente) : IRequest<IsTitolareElementoInLFResult>;
    public record GetTypeSignatureToBeEnteredResult(string output);

    public record GetTypeSignatureToBeEntered(FileRequest fileReq, InfoUtente infoUtente) : IRequest<GetTypeSignatureToBeEnteredResult>;
    public record DocumentoGetInfoFileResult(FileDocumento output);

    public record DocumentoGetInfoFile(FileRequest fileRequest, InfoUtente infoUtente) : IRequest<DocumentoGetInfoFileResult>;
    public record getFileInformationResult(FileInformation output);

    public record getFileInformation(FileRequest fileRequest, InfoUtente infoUtente) : IRequest<getFileInformationResult>;

}
