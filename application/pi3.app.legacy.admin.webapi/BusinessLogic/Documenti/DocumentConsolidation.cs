// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Reflection;

namespace BusinessLogic.Documenti;

public class DocumentConsolidation
{
    /// <summary>
    /// Microfunzione associata al servizio di consolidamento documenti
    /// </summary>
    private const string DO_CONSOLIDAMENTO = "DO_CONSOLIDAMENTO";

    /// <summary>
    /// Microfunzione associata al servizio di consolidamento metadati documenti
    /// </summary>
    private const string DO_CONSOLIDAMENTO_METADATI = "DO_CONSOLIDAMENTO_METADATI";

    /// <summary>
    /// Attributo di utilità da associare agli enumerations per determinare facilmente lo stato di consolidamento
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DocumentConsolidationAttribute : Attribute
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="state"></param>
        public DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum state)
        {
            this.State = state;
        }

        /// <summary>
        /// Stato di consolidamento del documento
        /// </summary>
        public DocsPaVO.documento.DocumentConsolidationStateEnum State
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static DocsPaVO.documento.DocumentConsolidationStateEnum GetState(Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DocumentConsolidationAttribute[] attributes = (DocumentConsolidationAttribute[])
                    fi.GetCustomAttributes(typeof(DocumentConsolidationAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].State;
            else
                return DocsPaVO.documento.DocumentConsolidationStateEnum.None;
        }
    }

    private class CorrispondenteEqualityComparer : IEqualityComparer<DocsPaVO.utente.Corrispondente>
    {
        #region IEqualityComparer<Corrispondente> Members

        /// <summary>
        ///
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        bool IEqualityComparer<DocsPaVO.utente.Corrispondente>.Equals(DocsPaVO.utente.Corrispondente c1, DocsPaVO.utente.Corrispondente c2)
        {
            if (c1 != null && c2 != null)
            {
                if (string.Compare(c1.systemId, c2.systemId, true) != 0)
                    return false;

                if (string.Compare(c1.codiceRubrica, c2.codiceRubrica, true) != 0)
                    return false;

                //if (string.Compare(c1.descrizione, c2.descrizione, true) != 0)
                //    return false;
            }

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        int IEqualityComparer<DocsPaVO.utente.Corrispondente>.GetHashCode(DocsPaVO.utente.Corrispondente obj)
        {
            if (obj != null)
            {
                int ret;
                Int32.TryParse(obj.systemId, out ret);
                return ret;
            }
            else
                return 0;
        }

        #endregion
    }

    public enum ConsolidationActionsDeniedEnum
    {
        [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
        AddVersions,
        [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
        RemoveVersions,
        [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
        ModifyVersions,
        [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
        AddAttatchments,
        [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
        RemoveAttatchments,
        [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
        ModifyAttatchments,
        [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
        DeleteDocument,
        [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
        SignDocument,
        [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)]
        PrepareProtocol,            // Predisponi alla protocollazione
        [DocumentConsolidationAttribute(DocsPaVO.documento.DocumentConsolidationStateEnum.Step2)]
        CancelProtocol,             // Annullamento protocollo
    }

    public static bool IsConfigEnabled()
    {
        string value = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_CONSOLIDAMENTO");

        if (!string.IsNullOrEmpty(value) && value.Equals("1"))
        {
            return value == "1";
        }
        else
        {
            return false;
        }
    }

    public static bool CanChangeMetadata(DocsPaVO.utente.InfoUtente userInfo, DocsPaVO.documento.SchedaDocumento document, bool throwOnError)
    {
        bool canChange = false;

        DocsPaVO.documento.DocumentConsolidationStateInfo actualState = GetState(userInfo, document.systemId);

        if (actualState.State == DocsPaVO.documento.DocumentConsolidationStateEnum.None ||
            actualState.State == DocsPaVO.documento.DocumentConsolidationStateEnum.Step1)
        {
            // Il primo stato di consolidamento consente ancora di modificare tutti i metadati??
            canChange = true;
        }
        else if (actualState.State == DocsPaVO.documento.DocumentConsolidationStateEnum.Step2)
        {
            // Nome del campo oggetto del controllo
            string fieldName = string.Empty;

            // Reperimento dati del documento salvati
            DocsPaVO.documento.SchedaDocumento savedDocument = BusinessLogic.Documenti.DocManager.getDettaglio(userInfo, document.systemId, document.docNumber);

            // 1. Verifica oggetto
            fieldName = "dell'oggetto";
            canChange = AreEquals(document.oggetto, savedDocument.oggetto);

            if (canChange && document.protocollo != null && savedDocument.protocollo != null)
            {
                // 2. Verifica mittenti / destinatari
                fieldName = "dei mittenti o destinatari";
                canChange = AreEquals(document.protocollo, savedDocument.protocollo);
            }

            if (canChange)
            {
                // 3. Verifica data arrivo
                fieldName = "della data arrivo";

                string dataArrivoAsString = string.Empty;
                string savedDataArrivoAsString = string.Empty;

                if (document.documenti != null && document.documenti.Count > 0)
                {
                    DocsPaVO.documento.Documento firstDoc = document.documenti[0] as DocsPaVO.documento.Documento;
                    if (firstDoc != null)
                        dataArrivoAsString = firstDoc.dataArrivo;
                }

                if (savedDocument.documenti != null && savedDocument.documenti.Count > 0)
                {
                    DocsPaVO.documento.Documento firstDoc = savedDocument.documenti[0] as DocsPaVO.documento.Documento;
                    if (firstDoc != null)
                        savedDataArrivoAsString = firstDoc.dataArrivo;
                }

                DateTime dataArrivo;
                DateTime savedDataArrivo;
                DateTime.TryParse(dataArrivoAsString, out dataArrivo);
                DateTime.TryParse(savedDataArrivoAsString, out savedDataArrivo);

                canChange = (DateTime.Compare(dataArrivo, savedDataArrivo) == 0);
            }

            if (!canChange && throwOnError)
                throw new ApplicationException(string.Format("La modifica {0} non può essere effettuata in quanto il documento risulta in stato consolidato nei metadati", fieldName));
        }

        return canChange;
    }

    public static DocsPaVO.documento.DocumentConsolidationStateInfo GetState(DocsPaVO.utente.InfoUtente userInfo, string idDocument)
    {
        return new DocsPaDB.Query_DocsPAWS.DocumentConsolidation(userInfo).GetState(idDocument);
    }

    public static bool CanExecuteAction(DocsPaVO.utente.InfoUtente userInfo, string idDocument, ConsolidationActionsDeniedEnum action)
    {
        return CanExecuteAction(userInfo, idDocument, action, false);
    }

    public static bool CanExecuteAction(DocsPaVO.utente.InfoUtente userInfo, string idDocument, ConsolidationActionsDeniedEnum action, bool throwOnError)
    {
        if (IsConfigEnabled())
        {
            DocsPaVO.documento.DocumentConsolidationStateInfo actualState = GetState(userInfo, idDocument);

            if (actualState.State == DocsPaVO.documento.DocumentConsolidationStateEnum.None)
                return true;
            else
            {
                // Determina, dai metadati dell'enumeration, a quale stato di consolidamento si riferisce l'azione richiesta
                DocsPaVO.documento.DocumentConsolidationStateEnum actionApplyState = DocumentConsolidationAttribute.GetState(action);

                bool canExecute = (actualState.State < actionApplyState);

                if (!canExecute && throwOnError)
                    throw new ApplicationException("L'azione non può essere eseguita, il documento risulta in stato consolidato");

                return canExecute;
            }
        }
        else
        {
            return true;
        }
    }


    private static bool AreEquals(DocsPaVO.documento.Oggetto o1, DocsPaVO.documento.Oggetto o2)
    {
        if (o1 == null || o2 == null)
            return false;

        if (string.Compare(o1.systemId, o2.systemId, true) != 0)
            return false;

        if (string.Compare(o1.descrizione, o2.descrizione, true) != 0)
            return false;

        return true;
    }

    private static bool AreEquals(DocsPaVO.documento.Protocollo p1, DocsPaVO.documento.Protocollo p2)
    {
        if (p1.GetType() != p2.GetType())
            return false;

        // Uguaglianza tra i corrispondenti del protocollo
        List<DocsPaVO.utente.Corrispondente> actualList = null;
        List<DocsPaVO.utente.Corrispondente> savedList = null;

        if (p1.GetType() == typeof(DocsPaVO.documento.ProtocolloUscita))
        {
            // ------------------------------------
            // Controllo mittenti protocollo uscita
            DocsPaVO.documento.ProtocolloUscita pu1 = (DocsPaVO.documento.ProtocolloUscita)p1;
            DocsPaVO.documento.ProtocolloUscita pu2 = (DocsPaVO.documento.ProtocolloUscita)p2;

            if (pu1.mittente != null && pu2.mittente != null)
            {
                actualList = new List<DocsPaVO.utente.Corrispondente> { pu1.mittente };
                savedList = new List<DocsPaVO.utente.Corrispondente> { pu2.mittente };

                // Verifica uguaglianza tramite operazione di intersezione
                var intersection = actualList.Intersect(savedList, new CorrispondenteEqualityComparer());
                if (savedList.Count() != intersection.Count())
                    return false;
            }

            // ------------------------------------

            // ------------------------------------
            // Controllo destinatari protocollo uscita
            if (pu1.destinatari != null && pu2.destinatari != null)
            {
                actualList = new List<DocsPaVO.utente.Corrispondente>((DocsPaVO.utente.Corrispondente[])pu1.destinatari.ToArray(typeof(DocsPaVO.utente.Corrispondente)));
                savedList = new List<DocsPaVO.utente.Corrispondente>((DocsPaVO.utente.Corrispondente[])pu2.destinatari.ToArray(typeof(DocsPaVO.utente.Corrispondente)));

                // Verifica uguaglianza tramite operazione di intersezione
                var intersection = actualList.Intersect(savedList, new CorrispondenteEqualityComparer());
                if (savedList.Count() != intersection.Count())
                    return false;
            }
            // ------------------------------------

            // ------------------------------------
            // Controllo destinatari in CC protocollo uscita
            if (pu1.destinatariConoscenza != null && pu2.destinatariConoscenza != null)
            {
                actualList = new List<DocsPaVO.utente.Corrispondente>((DocsPaVO.utente.Corrispondente[])pu1.destinatariConoscenza.ToArray(typeof(DocsPaVO.utente.Corrispondente)));
                savedList = new List<DocsPaVO.utente.Corrispondente>((DocsPaVO.utente.Corrispondente[])pu2.destinatariConoscenza.ToArray(typeof(DocsPaVO.utente.Corrispondente)));

                // Verifica uguaglianza tramite operazione di intersezione
                var intersection = actualList.Intersect(savedList, new CorrispondenteEqualityComparer());
                if (savedList.Count() != intersection.Count())
                    return false;
            }
            // ------------------------------------
        }
        else if (p1.GetType() == typeof(DocsPaVO.documento.ProtocolloEntrata))
        {
            DocsPaVO.documento.ProtocolloEntrata pe1 = (DocsPaVO.documento.ProtocolloEntrata)p1;
            DocsPaVO.documento.ProtocolloEntrata pe2 = (DocsPaVO.documento.ProtocolloEntrata)p2;

            // ------------------------------------
            // Controllo mittente protocollo entrata
            if (pe1.mittente != null && pe2.mittente != null)
            {
                actualList = new List<DocsPaVO.utente.Corrispondente> { pe1.mittente };
                savedList = new List<DocsPaVO.utente.Corrispondente> { pe2.mittente };

                // Verifica uguaglianza tramite operazione di intersezione
                var intersection = actualList.Intersect(savedList, new CorrispondenteEqualityComparer());
                if (savedList.Count() != intersection.Count())
                    return false;
            }
            // ------------------------------------

            // ------------------------------------
            // Controllo mittente intermedio protocollo entrata
            if (pe1.mittenteIntermedio != null && pe2.mittenteIntermedio != null)
            {
                actualList = new List<DocsPaVO.utente.Corrispondente> { pe1.mittenteIntermedio };
                savedList = new List<DocsPaVO.utente.Corrispondente> { pe2.mittenteIntermedio };

                // Verifica uguaglianza tramite operazione di intersezione
                var intersection = actualList.Intersect(savedList, new CorrispondenteEqualityComparer());
                if (savedList.Count() != intersection.Count())
                    return false;
            }
            // ------------------------------------

            // ------------------------------------
            // Controllo mittenti protocollo entrata
            if (pe1.mittenti != null && pe2.mittenti != null)
            {
                actualList = new List<DocsPaVO.utente.Corrispondente>((DocsPaVO.utente.Corrispondente[])pe1.mittenti.ToArray(typeof(DocsPaVO.utente.Corrispondente)));
                savedList = new List<DocsPaVO.utente.Corrispondente>((DocsPaVO.utente.Corrispondente[])pe2.mittenti.ToArray(typeof(DocsPaVO.utente.Corrispondente)));

                // Verifica uguaglianza tramite operazione di intersezione
                var intersection = actualList.Intersect(savedList, new CorrispondenteEqualityComparer());
                if (savedList.Count() != intersection.Count())
                    return false;
            }
            // ------------------------------------
        }
        else if (p1.GetType() == typeof(DocsPaVO.documento.ProtocolloInterno))
        {
            DocsPaVO.documento.ProtocolloInterno pi1 = (DocsPaVO.documento.ProtocolloInterno)p1;
            DocsPaVO.documento.ProtocolloInterno pi2 = (DocsPaVO.documento.ProtocolloInterno)p2;

            if (pi1.mittente != null && pi2.mittente != null)
            {
                actualList = new List<DocsPaVO.utente.Corrispondente> { pi1.mittente };
                savedList = new List<DocsPaVO.utente.Corrispondente> { pi2.mittente };

                // Verifica uguaglianza tramite operazione di intersezione
                var intersection = actualList.Intersect(savedList, new CorrispondenteEqualityComparer());
                if (savedList.Count() != intersection.Count())
                    return false;
            }
            // ------------------------------------

            // ------------------------------------
            // Controllo destinatari protocollo interno
            if (pi1.destinatari != null && pi2.destinatari != null)
            {
                actualList = new List<DocsPaVO.utente.Corrispondente>((DocsPaVO.utente.Corrispondente[])pi1.destinatari.ToArray(typeof(DocsPaVO.utente.Corrispondente)));
                savedList = new List<DocsPaVO.utente.Corrispondente>((DocsPaVO.utente.Corrispondente[])pi2.destinatari.ToArray(typeof(DocsPaVO.utente.Corrispondente)));

                // Verifica uguaglianza tramite operazione di intersezione
                var intersection = actualList.Intersect(savedList, new CorrispondenteEqualityComparer());
                if (savedList.Count() != intersection.Count())
                    return false;
            }
            // ------------------------------------

            // ------------------------------------
            // Controllo destinatari in CC protocollo interno
            if (pi1.destinatariConoscenza != null && pi2.destinatariConoscenza != null)
            {
                actualList = new List<DocsPaVO.utente.Corrispondente>((DocsPaVO.utente.Corrispondente[])pi1.destinatariConoscenza.ToArray(typeof(DocsPaVO.utente.Corrispondente)));
                savedList = new List<DocsPaVO.utente.Corrispondente>((DocsPaVO.utente.Corrispondente[])pi2.destinatariConoscenza.ToArray(typeof(DocsPaVO.utente.Corrispondente)));

                // Verifica uguaglianza tramite operazione di intersezione
                var intersection = actualList.Intersect(savedList, new CorrispondenteEqualityComparer());
                if (savedList.Count() != intersection.Count())
                    return false;
            }
            // ------------------------------------
        }

        return true;
    }

    internal static DocsPaVO.documento.DocumentConsolidationStateInfo Consolidate(DocsPaVO.utente.InfoUtente userInfo, string idDocument, DocsPaVO.documento.DocumentConsolidationStateEnum toState, bool bypassFinalStateCheck)
    {
        if (!HasDocumentConsolidationRights(userInfo))
            throw new ApplicationException(string.Format("L'utente {0} non dispone dei diritti necessari per effettuare l'operazione di consolidamento", userInfo.userId));

        // Reperimento stato di consolidamento del documento richiesto
        DocsPaVO.documento.SchedaDocumento savedDocument = BusinessLogic.Documenti.DocManager.getDettaglio(userInfo, idDocument, idDocument);

        if (savedDocument.documentoPrincipale != null)
            throw new ApplicationException("Il documento risulta un allegato di un altro documento, pertanto non può essere consolidato singolarmente");

        if (savedDocument.checkOutStatus != null)
            throw new ApplicationException(string.Format("Il documento risulta bloccato dall'utente {0}, pertanto non può essere consolidato", savedDocument.checkOutStatus.UserName));

        if (BusinessLogic.LibroFirma.LibroFirmaManager.IsDocOrAllInLibroFirma(savedDocument.docNumber))
            throw new ApplicationException("Il documento o uno dei suoi allegati risulta coinvolto in un processo di firma, pertanto non può essere consolidato ");

        // Verifica sei il documento si trova nello stato finale
        if (!bypassFinalStateCheck)
        {
            DocsPaVO.DiagrammaStato.Stato workflowState = BusinessLogic.DiagrammiStato.DiagrammiStato.getStatoDoc(savedDocument.docNumber);
            if (workflowState != null && workflowState.STATO_FINALE)
                throw new ApplicationException("Il documento risulta in stato finale");
        }

        // Verifica dei diritti di lettura / scrittura sul documento
        int accessRights;
        Int32.TryParse(savedDocument.accessRights, out accessRights);
        if (accessRights > 0 && accessRights < 63)
            throw new ApplicationException(string.Format("L'utente {0} non dispone dei diritti di lettura / scrittura sul documento necessari per effettuare l'operazione di consolidamento", userInfo.userId));

        DocsPaVO.documento.DocumentConsolidationStateInfo state = savedDocument.ConsolidationState;

        // Controllo sullo stato di destinazione
        if (toState <= state.State)
        {
            throw new ApplicationException(string.Format("Il documento è attualmente in stato '{1}'",
                idDocument,
                DocsPaVO.documento.DocumentConsolidationStateDescriptionAttribute.GetDescription(state.State)));
        }

        // Un documento predisposto alla protocollazione non può essere consolidato in quanto rappresenta uno stato intermedio
        if (savedDocument.predisponiProtocollazione)
        {
            throw new ApplicationException("Il documento risulta predisposto alla protocollazione e non può essere consolidato");
        }

        using (DocsPaDB.TransactionContext transactionalContext = new DocsPaDB.TransactionContext())
        {
            // Operazione di consolidamento
            DocsPaDB.Query_DocsPAWS.DocumentConsolidation consolidationDb = new DocsPaDB.Query_DocsPAWS.DocumentConsolidation(userInfo);
            state = consolidationDb.SetState(idDocument, toState);

            // Operazione di consolidamento sui singoli allegati del documento
            foreach (DocsPaVO.documento.Allegato attatchment in AllegatiManager.getAllegati(idDocument, string.Empty))
            {
                DocsPaVO.documento.DocumentConsolidationStateInfo attState = null;
                attState = consolidationDb.SetState(attatchment.docNumber, toState);
            }

            transactionalContext.Complete();
        }

        return state;
    }

    /// <summary>
    /// Verifica se l'utente dispone dei diritti necessari per utilizzare le funzioni di consolidamento
    /// </summary>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    public static bool HasDocumentConsolidationRights(DocsPaVO.utente.InfoUtente userInfo)
    {
        if (IsConfigEnabled())
        {
            DocsPaVO.utente.Ruolo currentRole = BusinessLogic.Utenti.UserManager.getRuolo(userInfo.idCorrGlobali);

            if (currentRole != null)
            {
                DocsPaVO.utente.Funzione[] funzioni = (DocsPaVO.utente.Funzione[])
                    currentRole.funzioni.ToArray(typeof(DocsPaVO.utente.Funzione));

                DocsPaVO.utente.Funzione function = funzioni.Where(e => (e.codice == DO_CONSOLIDAMENTO || e.codice == DO_CONSOLIDAMENTO_METADATI)).FirstOrDefault();

                return (function != null);
            }
            else
                throw new ApplicationException(string.Format("Ruolo corrente con id {0} per l'utente {1} non trovato", userInfo.idCorrGlobali, userInfo.userId));
        }
        else
            return false;
    }

}
