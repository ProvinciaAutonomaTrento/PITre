// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System.Collections;

namespace BusinessLogic.Documenti;

public class ProtocolloUscitaManager
{
    private static ILogger logger = Log.ForContext(typeof(ProtocolloUscitaManager));

    /// <summary>
    /// Metodo per la spedizione di un documento per interoperabilità semplificata
    /// </summary>
    /// <param name="schedaDoc">Documento da spedire</param>
    /// <param name="infoUtente">Informazioni sull'utente che sta effettuando la spedizione</param>
    /// <param name="receivers">Destinatari cui indirizzare la spedizione</param>
    /// <returns>Risultato della spedizione</returns>
    public static DocsPaVO.Interoperabilita.SendDocumentResponse SpedisciPerInteroperabilitaSemplificata(
        DocsPaVO.documento.SchedaDocumento schedaDoc,
        DocsPaVO.utente.InfoUtente infoUtente,
        IEnumerable<DocsPaVO.Spedizione.DestinatarioEsterno> receivers)
    {
        // Oggetto contenente l'esito della spedizione del documento ai destinatari.
        // Per ognuno di essi, nella tabella "DPA_STATO_INVIO",
        // deve essere impostata correttamente la data di spedizione; per gli 
        // altri viene impostata a null;
        DocsPaVO.Interoperabilita.SendDocumentResponse sendDocumentResponse = null;
        //try
        //{
        //    InsertStatoInvioISPreliminare(schedaDoc, receivers);
        //}
        //catch (Exception e1)
        //{
        //    logger.Error("Errore nell'inserimento preliminare dei destinatari nella DPA_STATO_INVIO", e1);
        //}

        try
        {
            DocsPaVO.utente.Corrispondente mittente = BusinessLogic.Utenti.UserManager.getCorrispondente(infoUtente.idCorrGlobali, false);

            DocsPaVO.utente.Ruolo ruoloMitt = (DocsPaVO.utente.Ruolo)mittente;
            mittente.descrizione = ruoloMitt.descrizione;

            // Invio del documento a tutti i destinatari
            sendDocumentResponse = BusinessLogic.Interoperabilita.InteroperabilitaInvioSegnatura.SendDocumentIS(schedaDoc, infoUtente, receivers);
        }
        catch (Exception e)
        {
            logger.Error("Errore nella richiesta di interoperabilità semplificata", e);

            throw new Exception("Si è verificato un errore durante l'invio della richiesta di interoperabilità: " + e.Message);
        }

        //try
        //{
        //    List<DocsPaVO.utente.Corrispondente> c = new List<DocsPaVO.utente.Corrispondente>();
        //    foreach (DocsPaVO.Spedizione.DestinatarioEsterno de in receivers)
        //        c.AddRange(de.DatiDestinatari);

        //    InsertStatoInvioDestinatari(schedaDoc, new ArrayList(c), sendDocumentResponse);

        //    BusinessLogic.interoperabilita.Semplificata.InteroperabilitaSemplificataManager.IS_statusMaskUpdater(schedaDoc.systemId);
        //}
        //catch (Exception e)
        //{
        //    logger.Error("Errore nella gestione della spedizione", e);

        //    throw new Exception("Errore nella gestione della spedizione:\\n\\n" + e.Message);
        //}

        return sendDocumentResponse;
    }

    /// <summary>
    /// Spedizione del documento per interoperabilità
    /// </summary>
    /// <param name="schedaDoc"></param>
    /// <param name="registroMittente">
    /// Indica il registro (o l'RF) mittente del documento per interoperabilità.
    /// <remarks>
    /// Se null, il mittente sarà la mail del registro su cui è protocollato il documento
    /// </remarks>
    /// </param>
    /// <param name="mailAddress"></param>
    /// <param name="destinatari"></param>
    /// <param name="infoUtente"></param>
    /// <param name="confermaRic"></param>
    /// <returns></returns>
    public static DocsPaVO.Interoperabilita.SendDocumentResponse spedisci(
        DocsPaVO.documento.SchedaDocumento schedaDoc,
        DocsPaVO.utente.Registro registroMittente,
        string mailAddress,
        ArrayList destinatari,
        DocsPaVO.utente.InfoUtente infoUtente,
        bool confermaRic)
    {
        // Oggetto contenente l'esito della spedizione del documento ai destinatari.
        // Per ognuno di essi, nella tabella "DPA_STATO_INVIO",
        // deve essere impostata correttamente la data di spedizione; per gli 
        // altri viene impostata a null;
        DocsPaVO.Interoperabilita.SendDocumentResponse sendDocumentResponse = null;

        //se c'è il batch_interoperabilità non bisogna mandare risultati a video

        try
        {
            DocsPaVO.utente.Corrispondente mittente = BusinessLogic.Utenti.UserManager.getCorrispondente(infoUtente.idCorrGlobali, false);

            DocsPaVO.utente.Ruolo ruoloMitt = (DocsPaVO.utente.Ruolo)mittente;
            mittente.descrizione = ruoloMitt.descrizione;

            // Invio mail a tutti i destinatari del documento
            sendDocumentResponse = BusinessLogic.Interoperabilita.InteroperabilitaInvioSegnatura.SendDocument(schedaDoc, registroMittente, mailAddress, destinatari, infoUtente, confermaRic);
        }
        catch (Exception e)
        {
            logger.Debug("Errore nell'invio mail", e);

            throw new Exception("Si è verificato un errore nell'invio dell'email:\\n\\n" + e.Message);
        }

        try
        {
            InsertStatoInvioDestinatari(schedaDoc, destinatari, sendDocumentResponse);

            //SOLO nel caso di interoperabilità interna abilitata
            if (DocsPaVO.Settings.AppSettings.Instance.INTEROP_INT_NO_MAIL != null &&
                        DocsPaVO.Settings.AppSettings.Instance.INTEROP_INT_NO_MAIL.ToString() != "0")
            {
                foreach (DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse sdml in sendDocumentResponse.SendDocumentMailResponseList)
                {
                    if (sdml.datiInteropAutomatica != null)
                    {
                        string erroreMessage = string.Empty;
                        Interoperabilita.InteroperabilitaInvioRicevuta.sendRicevutaRitorno(sdml.datiInteropAutomatica.schedaDoc, sdml.datiInteropAutomatica.registro, sdml.datiInteropAutomatica.ruolo, sdml.datiInteropAutomatica.infoUtente, out erroreMessage);
                        //
                        bool verificaRagioni;
                        string message = "";
                        //BUG INPS: doppia trasmis
                        //BusinessLogic.trasmissioni.TrasmProtoIntManager.TrasmissioneProtocolloAutomatico(sdml.datiInteropAutomatica.schedaDoc, sdml.datiInteropAutomatica.registro.systemId, schedaDoc, sdml.datiInteropAutomatica.ruolo, sdml.datiInteropAutomatica.infoUtente, infoUtente.urlWA, false, out verificaRagioni, out message);
                    }
                }
            }
        }
        catch (Exception e)
        {
            logger.Debug("Errore nella gestione della spedizione", e);

            throw new Exception("Errore nella gestione della spedizione:\\n\\n" + e.Message);
        }

        return sendDocumentResponse;
    }

    public static void InsertStatoInvioDestinatari(DocsPaVO.documento.SchedaDocumento schedaDocumento, ArrayList listDestinatari, DocsPaVO.Interoperabilita.SendDocumentResponse sendDocumentResponse)
    {
        DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();

        foreach (DocsPaVO.utente.Corrispondente corrispondente in listDestinatari)
        {
            if (schedaDocumento.protocollo.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
            {
                string idDocArrivoPar = doc.GetIdDocArrivoPar(schedaDocumento.systemId, corrispondente, "'D','C','F'");

                string mailCorrispondente = corrispondente.email;
                if (mailCorrispondente == null)
                    mailCorrispondente = string.Empty;

                // Reperimento esito invio della mail al destinatario corrente
                DocsPaVO.Interoperabilita.SendDocumentResponse.SendDocumentMailResponse mailResponse = sendDocumentResponse.FindDocumentResponse(corrispondente);
                //if(corrispondente.tipoCorrispondente!="O")
                doc.InsertStatoInvio(corrispondente, idDocArrivoPar, schedaDocumento.registro.systemId, schedaDocumento.systemId, (mailResponse == null || (mailResponse != null && !mailResponse.SendSucceded && !string.IsNullOrEmpty(mailResponse.SendErrorMessage))), mailResponse.MailAddress);

            }
        }


    }

}
