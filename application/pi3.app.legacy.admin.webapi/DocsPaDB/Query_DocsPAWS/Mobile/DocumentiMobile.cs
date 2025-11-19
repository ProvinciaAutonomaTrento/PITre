// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaDbManagement.Functions;
using Serilog;
using System;
using System.Data;

namespace DocsPaDB.Query_DocsPAWS.Mobile;

public class DocumentiMobile : Documenti
{
    private static ILogger logger = Serilog.Log.ForContext(typeof(DocumentiMobile));

    public DocsPaVO.documento.SchedaDocumento GetDettaglioMobile(DocsPaVO.utente.InfoUtente infoUtente, string idProfile, string docNumber, bool impostaDataVista)
    {
        DocsPaVO.documento.SchedaDocumento schedaDocumento = null;

        try
        {
            DocsPaUtils.Query queryDef = DocsPaUtils.InitQuery.getInstance().getQuery("S_GET_SCHEDA_DOCUMENTO");

            queryDef.setParam("creationDate", Functions.ToChar("A.CREATION_DATE", false));
            queryDef.setParam("creationTime", Functions.ToChar("A.CREATION_TIME", true));
            queryDef.setParam("dtaProtoEme", Functions.ToChar("A.DTA_PROTO_EME", false));
            queryDef.setParam("dtaProto", Functions.ToChar("A.DTA_PROTO", true));
            queryDef.setParam("dtaProtoIn", Functions.ToChar("A.DTA_PROTO_IN", false));
            queryDef.setParam("dtaAnnulla", Functions.ToChar("A.DTA_ANNULLA", false));
            queryDef.setParam("idGroup", infoUtente.idGruppo);
            queryDef.setParam("idPeople", infoUtente.idPeople);

            string documentoPubblico = string.Empty;
            string idRuoloPubblico = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "ENABLE_FASCICOLO_PUBBLICO");
            if (string.IsNullOrEmpty(idRuoloPubblico))
            {
                idRuoloPubblico = "0";
            }
            else
            {
                if (dbType.Equals("SQL"))
                    documentoPubblico = getUserDB() + ".checkDocumentoPubblico(A.DOCNUMBER, " + idRuoloPubblico + ") AS CHA_PUBBLICO, ";
                else
                    documentoPubblico = "checkDocumentoPubblico(A.DOCNUMBER, " + idRuoloPubblico + ") AS CHA_PUBBLICO, ";
            }
            queryDef.setParam("documentoPubblico", documentoPubblico);
            queryDef.setParam("idRuoloPubblico", idRuoloPubblico);

            string keyParams = string.Empty;

            if (!string.IsNullOrEmpty(idProfile))
                queryDef.setParam("pk", String.Format("A.SYSTEM_ID = {0}", idProfile));
            else if (!string.IsNullOrEmpty(docNumber))
                queryDef.setParam("pk", String.Format("A.DOCNUMBER = {0}", docNumber));

            queryDef.setParam("dbUser", this.getUserDB());

            string commandText = queryDef.getSQL();
            logger.Debug(commandText);
            DataSet dataSet;
            if (this.ExecuteQuery(out dataSet, "PROFILE", commandText))
            {
                DataRow dataRow = dataSet.Tables["PROFILE"].Rows[0];

                schedaDocumento = this.GetSchedaDocumentoMobile(infoUtente, dataRow);

                dataSet.Dispose();
                dataSet = null;

                if (impostaDataVista)
                    this.SetDataVistaSP(infoUtente, schedaDocumento.systemId, "D");
            }

            //Profilazione dinamica
            if (schedaDocumento != null && schedaDocumento.tipologiaAtto != null && schedaDocumento.tipologiaAtto.systemId != null && schedaDocumento.tipologiaAtto.systemId != "")
            {
                Model model = new Model();
                DocsPaVO.ProfilazioneDinamica.Templates template = model.getTemplateDettagli(schedaDocumento.docNumber);
                schedaDocumento.template = template;
            }
        }
        catch (Exception e)
        {
            string errorMessage = string.Format("Errore nel reperimento del documento con idProfile {0} e con docNumber {1}: {2}", idProfile, docNumber, e.Message);
            logger.Error(errorMessage);
            throw new ApplicationException(errorMessage, e);
        }

        return schedaDocumento;
    }

    private DocsPaVO.documento.SchedaDocumento GetSchedaDocumentoMobile(DocsPaVO.utente.InfoUtente infoUtente, DataRow dataRow)
    {
        DocsPaVO.documento.SchedaDocumento schedaDoc = new DocsPaVO.documento.SchedaDocumento();

        if (!GetProfiloMobile(infoUtente, dataRow, ref schedaDoc))
        {
            schedaDoc = null;
        }
        if (schedaDoc != null && !string.IsNullOrEmpty((dataRow["ID_DOCUMENTO_PRINCIPALE"]).ToString()))
        {
            Documenti querydocs= new Documenti();
            schedaDoc.documentoPrincipale = querydocs.GetInfoDocumento(infoUtente.idGruppo, infoUtente.idPeople, dataRow["ID_DOCUMENTO_PRINCIPALE"].ToString(), false);
        }
        return schedaDoc;
    }


}
