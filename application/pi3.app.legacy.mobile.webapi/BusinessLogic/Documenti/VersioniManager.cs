// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;

namespace BusinessLogic.Documenti
{
    public class VersioniManager
    {
        private static ILogger logger = Log.ForContext(typeof(VersioniManager));

        public static string getLatestVersionID(string docNumber, DocsPaVO.utente.InfoUtente objSicurezza)
        {
            logger.Debug("getLatestVersionID");
            string versionId = null;

            DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(objSicurezza);
            versionId = documentManager.GetLatestVersionId(docNumber);

            return versionId;
        }

        public static DocsPaVO.documento.FileRequest addVersion(DocsPaVO.documento.FileRequest fileRequest, DocsPaVO.utente.InfoUtente infoUtente, bool daInviare)
        {
            logger.Information("BEGIN");
            logger.Debug("addVersion");

            if (fileRequest.repositoryContext != null)
            {
                if (!daInviare)
                {
                    // Inserimento della versione nel repositorycontext (il documento ancora non è stato salvato)
                    int newVersion, newVersionLabel;
                    Int32.TryParse(fileRequest.version, out newVersion);
                    Int32.TryParse(fileRequest.versionLabel, out newVersionLabel);

                    fileRequest.subVersion = "!";
                    fileRequest.version = (newVersion + 1).ToString();
                    fileRequest.versionLabel = (newVersionLabel + 1).ToString();
                }
            }
            else
            {
                // Verifica stato di consolidamento del documento
                DocumentConsolidation.CanExecuteAction(infoUtente, fileRequest.docNumber, DocumentConsolidation.ConsolidationActionsDeniedEnum.AddVersions, true);
                if (!fileRequest.conSegnaturaPermanente && !LibroFirma.LibroFirmaManager.CanExecuteAction(fileRequest, infoUtente))
                    throw new Exception("Non è possibile creare la versione poichè il documento principale è in libro firma");

                DocsPaDocumentale.Documentale.DocumentManager documentManager = new DocsPaDocumentale.Documentale.DocumentManager(infoUtente);
                if (!documentManager.AddVersion(fileRequest, daInviare))
                    fileRequest = null;
            }
            logger.Information("END");
            return fileRequest;
        }


    }
}
