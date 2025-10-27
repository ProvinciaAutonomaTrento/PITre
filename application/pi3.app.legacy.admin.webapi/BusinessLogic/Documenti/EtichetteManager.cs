// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaDocumentale_ETDOCS.Documentale;
using DocsPaVO.InstanceAccess.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Documenti
{
    public sealed class EtichetteManager
    {
        private static Serilog.ILogger logger = Serilog.Log.ForContext(typeof(EtichetteManager));

        private static EtichetteManager instance = null;
        private static Dictionary<string, DocsPaVO.documento.EtichettaInfo[]> _intance = null;

        public static DocsPaVO.documento.EtichettaInfo[] GetInstance(DocsPaVO.utente.InfoUtente infoUtente, string idAmm)
        {
            if (instance == null)
            {
                instance = new EtichetteManager();
                instance.initializeInstance(infoUtente, idAmm);
            }
            if (instance != null && !_intance.ContainsKey(idAmm))
            {
                instance.initializeInstance(infoUtente, idAmm);
            }
            DocsPaVO.documento.EtichettaInfo[] etichette = _intance[idAmm];
            return etichette;
        }

        public void initializeInstance(DocsPaVO.utente.InfoUtente infoUtente, string idAmm)
        {
            if (_intance == null)
            {
                // Creazione oggetto dictionary contenente i dati delle etichette per tutte le amministrazioni
                _intance = new Dictionary<string, DocsPaVO.documento.EtichettaInfo[]>();
                GetData(infoUtente, idAmm);
            }

            if (!_intance.ContainsKey(idAmm))
            {
                GetData(infoUtente, idAmm);
            }
        }

        private void GetData(DocsPaVO.utente.InfoUtente infoUtente, string idAmm)
        {
            DocsPaVO.documento.EtichettaInfo[] data = null;

            // Caricamento etichette relative all'amministrazione richiesta
            using (DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti())
            {
                data = doc.getLettereDocumento(infoUtente, idAmm);
            }
            _intance.Add(idAmm, data);

            //_intance.Add<infoUtente.idAmministrazione, data>;
            //  return data;
        }

        public static bool SetInstance(DocsPaVO.utente.InfoUtente infoUtente, string idAmm, DocsPaVO.documento.EtichettaInfo[] etichette)
        {
            bool success = false;
            DocsPaDB.Query_DocsPAWS.Documenti doc = new DocsPaDB.Query_DocsPAWS.Documenti();
            success = doc.setLettereDocumento(infoUtente, idAmm, etichette);
            DocsPaVO.documento.EtichettaInfo[] data = null;
            _intance.Remove(idAmm);
            data = doc.getLettereDocumento(infoUtente, idAmm);
            _intance.Add(idAmm, etichette);
            return success;

        }

    }
}
