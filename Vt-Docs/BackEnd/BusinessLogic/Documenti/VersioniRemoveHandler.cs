using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.documento;
using DocsPaVO.utente;
using DocsPaVO.PrjDocImport;

namespace BusinessLogic.Documenti
{
    public class VersioniRemoveHandler
    {
        private SchedaDocumento _sd;
        private InfoUtente _infoUtente;
        private RemoveVersionType _removeType;
        private int _numVersionRemoved = 0;
        private string _error;

        public VersioniRemoveHandler(SchedaDocumento sd, InfoUtente infoUtente, RemoveVersionType removeType)
        {
            this._sd = sd;
            this._infoUtente = infoUtente;
            this._removeType = removeType;
        }

        private int Index
        {
            get
            {
                return (_removeType == RemoveVersionType.ALL_BUT_LAST_TWO) ? 2 : 1;
            }
        }

        private int NumVersionRemoved
        {
            get
            {
                return _numVersionRemoved;
            }
        }

        private SchedaDocumento DocumentToCheck
        {
            get
            {
                if (_sd.documentoPrincipale == null)
                {
                    return _sd;
                }
                else
                {
                    return DocManager.getDettaglio(_infoUtente, _sd.documentoPrincipale.idProfile, null);
                }
            }
        }

        private bool CheckDocumento()
        {
            SchedaDocumento sd = DocumentToCheck;
            //controllo se il documento è consolidato
            if (sd.ConsolidationState != null && sd.ConsolidationState.State != DocumentConsolidationStateEnum.None)
            {
                _error = "Il documento è nello stato consolidato";
                return false;
            }
            //controllo se è in attesa di accettazione
            bool attesaAcc = "20".Equals(sd.accessRights);
            if (attesaAcc)
            {
                _error = "Il documento è in attesa di accettazione";
                return false;
            }
            //controllo se è bloccato
            if (sd.checkOutStatus != null)
            {
                _error = string.Format("Il documento risulta bloccato dall'utente {0}", sd.checkOutStatus.UserName);
                return false;
            }
            //controllo se è predisposto
            bool isPredisposto = sd.predisponiProtocollazione;
            isPredisposto = sd.protocollo != null && string.IsNullOrEmpty(sd.protocollo.segnatura);
            //controllo se è grigio
            bool isGrey = "G".Equals(sd.tipoProto);
            if (!isGrey && !isPredisposto)
            {
                _error = "Il documento non è grigio o predisposto alla protocollazione";
                return false;
            }
            //controllo sui diritti del documento
            bool hasRights = "0".Equals(sd.accessRights) || "63".Equals(sd.accessRights) || "255".Equals(sd.accessRights);
            if (!hasRights)
            {
                _error = "Non si possiedono i diritti per la rimozione delle versioni";
                return false;
            }
            return true;
        }

        public void Execute()
        {
            if (!CheckDocumento()) return;
            List<FileRequest> versions = GetVersions(_sd);
            _numVersionRemoved = 0;
            if (versions.Count > Index)
            {
                for (int i = Index; i < versions.Count; i++)
                {
                    bool resTemp = VersioniManager.removeVersion(versions[i], _infoUtente);
                    if (!resTemp)
                    {
                        _error = "Errore nella rimozione della versione " + versions[i].version + " del documento";
                        return;
                    }
                    else
                    {
                        _numVersionRemoved++;
                    }
                }
            }
            foreach (Allegato all in _sd.allegati)
            {
                SchedaDocumento allSd = DocManager.getDettaglio(_infoUtente, null, all.docNumber);
                List<FileRequest> versionsAll = GetVersions(allSd);
                if (versionsAll.Count > Index)
                {
                    for (int i = Index; i < versionsAll.Count; i++)
                    {
                        bool resTemp = VersioniManager.removeVersion(versionsAll[i], _infoUtente);
                        if (!resTemp)
                        {
                            _error = "Errore nella rimozione della versione " + versionsAll[i].version + " dell'allegato " + allSd.docNumber;
                            return;
                        }
                        else
                        {
                            _numVersionRemoved++;
                        }
                    }
                }
            }
            if (_numVersionRemoved == 0)
            {
                _error = "Il documento non ha versioni da eliminare";
            }
        }

        public ImportResult RemoveResult
        {
            get
            {
                ImportResult res = new ImportResult();
                res.DocNumber = _sd.docNumber;
                res.IdProfile = _sd.systemId;
                if (string.IsNullOrEmpty(_error))
                {
                    res.Outcome = ImportResult.OutcomeEnumeration.OK;
                    res.Message = "Sono state rimosse " + NumVersionRemoved + " versioni del documento e dei suoi allegati";
                }
                else
                {
                    res.Outcome = ImportResult.OutcomeEnumeration.KO;
                    res.Message = _error;
                }
                return res;
            }
        }

        private static int CompareDocsByVersionDesc(FileRequest r1, FileRequest r2)
        {
            return r2.version.CompareTo(r1.version);
        }

        private List<FileRequest> GetVersions(SchedaDocumento sd)
        {
            List<FileRequest> versioni = new List<FileRequest>();
            foreach (object temp in sd.documenti)
            {
                versioni.Add((FileRequest)temp);
            }
            versioni.Sort(CompareDocsByVersionDesc);
            return versioni;
        }
    }
}
