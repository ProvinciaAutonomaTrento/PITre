// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record UploadFileOnSharedFolderResult(bool output);

    public record UploadFileOnSharedFolder(byte[] fileStream, string fileName, InfoUtente infoUtente) : IRequest<UploadFileOnSharedFolderResult>;
    public record FinalizeImportBigFileResult(bool output);

    public record FinalizeImportBigFile(DocsPaVO.utente.InfoUtente infoUtente, string uuid, string fileName, bool isAttachment) : IRequest<FinalizeImportBigFileResult>;
    public record DeleteReportByIdResult(bool output);

    public record DeleteReportById(string idReport) : IRequest<DeleteReportByIdResult>;

    public record getTipoDocObblResult(string output);
    public record getTipoDocObbl(string idAmministrazione) :IRequest<getTipoDocObblResult>;
    public record importPregressoResult(DocsPaVO.Import.Pregressi.EsitoImportPregressi output);

    public record importPregresso(DocsPaVO.utente.InfoUtente infoUtente, byte[] dati, bool isAdministration) : IRequest<importPregressoResult>;
    public record ExportReportExcelResult(DocsPaVO.documento.FileDocumento output);

    public record ExportReportExcel(List<DocsPaVO.Import.Pregressi.ItemReportPregressi> repInErr) : IRequest<ExportReportExcelResult>;
    public record asyncImportPregressoResult(bool output);

    public record asyncImportPregresso(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.Import.Pregressi.EsitoImportPregressi esitoPregressi, string descrizione) : IRequest<asyncImportPregressoResult>;

    public record ImportRDEDocumentResult(DocsPaVO.PrjDocImport.ImportResult output);

    public record ImportRDEDocument(DocsPaVO.PrjDocImport.DocumentRowData rowData, DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.utente.Ruolo role, string serverPath, bool isRapidClassificationRequired, bool isSmistamentoEnabled, bool isProfilationRequired, ProtoType protoType) : IRequest<ImportRDEDocumentResult>;
    public record ReadRDEDataFromExcelResult(DocsPaVO.PrjDocImport.DocumentRowDataContainer output, string error);

    public record ReadRDEDataFromExcel(byte[] content, string fileName, int versionNumber) : IRequest<ReadRDEDataFromExcelResult>;
    public record ReadProjectDataFromExcelResult(System.Collections.Generic.List<DocsPaVO.PrjDocImport.ProjectRowData> output);

    public record ReadProjectDataFromExcel(byte[] content, string fileName, DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.utente.Ruolo role) : IRequest<ReadProjectDataFromExcelResult>;
    public record ImportProjectResult(DocsPaVO.PrjDocImport.ImportResult output);

    public record ImportProject(DocsPaVO.PrjDocImport.ProjectRowData rowData, DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.utente.Ruolo role, bool isEnabledSmistamento, string serverPath) : IRequest<ImportProjectResult>;
}
