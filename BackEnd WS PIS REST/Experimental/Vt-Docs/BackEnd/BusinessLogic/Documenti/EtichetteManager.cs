using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using log4net;


namespace BusinessLogic.Documenti
{

    public sealed class EtichetteManager
    {
        private static ILog logger = LogManager.GetLogger(typeof(EtichetteManager));

        private static EtichetteManager instance = null;
        private static Dictionary<string, DocsPaVO.documento.EtichettaInfo[]> _intance = null;

        /// Costruttore privato poichè la classe implementa il design pattern singleton (Fabio)

        private EtichetteManager()
        {
        }

        public static DocsPaVO.documento.EtichettaInfo[] GetInstance(DocsPaVO.utente.InfoUtente infoUtente, string idAmm)
        {
            if (instance == null)
            {
                instance = new EtichetteManager();
                instance.initializeInstance(infoUtente, idAmm);
            }
            if (instance != null && !_intance.ContainsKey(idAmm))
            {
                lock (_intance)
                {
                    instance.initializeInstance(infoUtente, idAmm);
                }
            }
            DocsPaVO.documento.EtichettaInfo[] etichette = _intance[idAmm];
            return etichette;
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

        /*  public static void Release(DocsPaVO.utente.InfoUtente infoUtente)
          {
              if (_intance != null && _intance.ContainsKey(infoUtente.idAmministrazione))
              {
                  lock (_intance)
                  {
                      _intance.Remove(infoUtente.idAmministrazione);
                  }
              }
          }*/

        //Inizializza il singleton
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
                lock (_intance)
                {
                    // Caricamento etichette relative all'amministrazione richiesta
                    //_intance.Add(infoUtente.idAmministrazione, GetData(infoUtente));
                    GetData(infoUtente, idAmm);
                }
            }
        }


    }
}
