// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Collections;

namespace BusinessLogic.Documenti;

public class TimestampManager
{
    private static ILogger logger = Log.ForContext(typeof(TimestampManager));

    public static ArrayList getTimestampsDoc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.documento.FileRequest fileRequest)
    {
        try
        {
            DocsPaDB.Query_DocsPAWS.TimestampDoc timestampDoc = new DocsPaDB.Query_DocsPAWS.TimestampDoc();
            return timestampDoc.getTimestampsDoc(infoUtente, fileRequest);
        }
        catch (Exception e)
        {
            logger.Debug("ERRORE : Restituzioni marche temporali del documento : " + e.Message);
            return null;
        }

    }
}
