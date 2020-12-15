using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NttDataWA.DocsPaWR;
using RC = RubricaComune;
using RubricaComune.Proxy.Elementi;

namespace NttDataWA.CommonAddressBook
{
    public sealed class RubricaServices
    {
        /// <summary>
        /// 
        /// </summary>
        private RubricaServices()
        { }

        /// <summary>
        /// Reperimento istanza servizi elementi rubrica
        /// </summary>
        public static RC.ElementiRubricaServices GetElementiRubricaServiceInstance(DocsPaWR.InfoUtente infoUtente)
        {

            DocsPaWR.ConfigurazioniRubricaComune config = CommonAddressBook.Configurations.GetConfigurations(infoUtente);

            if (config.GestioneAbilitata)
                return new RC.ElementiRubricaServices(config.ServiceRoot, config.SuperUserId, config.SuperUserPwd);
            else
                throw new ApplicationException("Gestione rubrica comune non abilitata");
        }

        /// <summary>
        /// Reperimento di un elemento rubrica corrispondente ad un'unità organizzativa
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idUo"></param>
        /// <returns></returns>
        public static RC.Proxy.Elementi.ElementoRubrica GetElementoRubricaUO(DocsPaWR.InfoUtente infoUtente, string idUo)
        {
            RC.Proxy.Elementi.ElementoRubrica elementoRubrica = null;

            DocsPaWR.ElementoRubricaUO elementoUO = new DocsPaWR.DocsPaWebService().GetElementoRubricaUO(infoUtente, idUo);

            if (elementoUO != null)
            {
                // Ricerca dell'elemento in rubrica comune
                elementoRubrica = GetElementiRubricaServiceInstance(infoUtente).SearchSingle(elementoUO.CodiceRubrica, global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParolaIntera);
            }

            return elementoRubrica;
        }

        /// <summary>
        /// Reperimento di un elemento rubrica corrispondente ad un Raggruppamento Funzionale
        /// </summary>
        /// <param name="infoUtente">Informazioni sul richiedente</param>
        /// <param name="idRf">Id dell'RF</param>
        /// <returns>Informazioni sull'RF</returns>
        public static RC.Proxy.Elementi.ElementoRubrica GetElementoRubricaRF(DocsPaWR.InfoUtente infoUtente, string idRf)
        {
            RC.Proxy.Elementi.ElementoRubrica elementoRubrica = null;

            DocsPaWR.ElementoRubricaRF elementoRF = new DocsPaWR.DocsPaWebService().GetElementoRubricaRF(infoUtente, idRf);

            if (elementoRF != null)
            {
                // Ricerca dell'elemento in rubrica comune
                elementoRubrica = GetElementiRubricaServiceInstance(infoUtente).SearchSingle(elementoRF.CodiceRubrica, global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParolaIntera);
            }

            return elementoRubrica;
        }

        /// <summary>
        /// Rimozione di un elemento rubrica corrispondente ad un'unità organizzativa
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idUo"></param>
        public static void EliminaElementoRubricaUO(DocsPaWR.InfoUtente infoUtente, string idUo)
        {
            RC.Proxy.Elementi.ElementoRubrica elementoRubrica = GetElementoRubricaUO(infoUtente, idUo);

            if (elementoRubrica != null)
                GetElementiRubricaServiceInstance(infoUtente).Delete(elementoRubrica);
        }

        /// <summary>
        /// Creazione di un elemento contenente gli attributi
        /// di un corrispondente docspa da inviare a rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idElemento">
        /// Id dell'elemento docspa da inviare a rubrica comune
        /// </param>
        /// <param name="tipo">
        /// Tipo di elemento da salvare
        /// </param>
        /// <returns></returns>
        public static RC.Proxy.Elementi.ElementoRubrica NuovoElementoRubrica(DocsPaWR.InfoUtente infoUtente, string idElemento, Tipo tipo)
        {
            RC.Proxy.Elementi.ElementoRubrica elementoRubrica = null;

            DocsPaWR.ElementoRC elementoDaInviare = null;

            switch (tipo)
            {
                case Tipo.UO:
                    elementoDaInviare = new DocsPaWR.DocsPaWebService().GetElementoRubricaUO(infoUtente, idElemento);
                    break;
                case Tipo.RF:
                    elementoDaInviare = new DocsPaWR.DocsPaWebService().GetElementoRubricaRF(infoUtente, idElemento);
                    break;
            }

            if (elementoDaInviare != null)
            {
                // UO ancora non presente in rubrica comune
                elementoRubrica = new RC.Proxy.Elementi.ElementoRubrica();

                elementoRubrica.Codice = elementoDaInviare.CodiceRubrica;
                elementoRubrica.Descrizione = elementoDaInviare.DescrizioneRubrica;

                ExtractDetails(elementoRubrica, elementoDaInviare, tipo);
            }

            return elementoRubrica;
        }

        /// <summary>
        /// Metodo per la compilazione del dettaglio del corrispondente
        /// </summary>
        /// <param name="elementoRubrica">Elemento di cui impostare il dettaglio</param>
        /// <param name="elementoDaInviare">Elemento da cui estrarre il dettaglio</param>
        /// <param name="tipo">Tipo del corrispondente</param>
        private static void ExtractDetails(RC.Proxy.Elementi.ElementoRubrica elementoRubrica, DocsPaWR.ElementoRC elementoDaInviare, Tipo tipo)
        {
            switch (tipo)
            {
                case Tipo.UO:
                    SetUoDetails(elementoRubrica, (DocsPaWR.ElementoRubricaUO)elementoDaInviare);
                    break;
                case Tipo.RF:
                    SetRfDetails(elementoRubrica, (DocsPaWR.ElementoRubricaRF)elementoDaInviare);
                    break;

            }

            // Se presente la mail del registro, l'elemento rubrica è interoperante
            elementoRubrica.Email = elementoDaInviare.EMailRegistro;
            elementoRubrica.Amministrazione = elementoDaInviare.Amministrazione;
            elementoRubrica.AOO = elementoDaInviare.AOO;

            // Impostazione dell'URL (viene valorizzato solo se l'RF o il registro associato alla UO è interoperante
            switch (tipo)
            {
                case Tipo.UO:
                    elementoRubrica.Urls = UIManager.SimplifiedInteroperabilityManager.GetUrls(tipo, ((DocsPaWR.ElementoRubricaUO)elementoDaInviare).UO.IDCorrGlobale);
                    break;
                case Tipo.RF:
                    elementoRubrica.Urls = UIManager.SimplifiedInteroperabilityManager.GetUrls(tipo, ((DocsPaWR.ElementoRubricaRF)elementoDaInviare).RF.systemId);
                    break;
            }


            elementoRubrica.TipoCorrispondente = tipo;

        }

        private static void SetUoDetails(RubricaComune.Proxy.Elementi.ElementoRubrica elementoRubrica, DocsPaWR.ElementoRubricaUO elementoDaInviare)
        {
            if (elementoDaInviare.UO != null && elementoDaInviare.UO.DettagliUo != null)
            {
                // Impostazione dei dati aggiuntivi
                elementoRubrica.Indirizzo = elementoDaInviare.UO.DettagliUo.Indirizzo;
                elementoRubrica.Nazione = elementoDaInviare.UO.DettagliUo.Nazione;
                elementoRubrica.Telefono = elementoDaInviare.UO.DettagliUo.Telefono1;
                elementoRubrica.Fax = elementoDaInviare.UO.DettagliUo.Fax;
                elementoRubrica.Citta = elementoDaInviare.UO.DettagliUo.Citta;
                elementoRubrica.Cap = elementoDaInviare.UO.DettagliUo.Cap;
                elementoRubrica.Provincia = elementoDaInviare.UO.DettagliUo.Provincia;
                elementoRubrica.CodiceFiscale = elementoDaInviare.UO.DettagliUo.CodiceFiscale;
                elementoRubrica.PartitaIva = elementoDaInviare.UO.DettagliUo.PartitaIva;

            }
        }

        private static void SetRfDetails(RubricaComune.Proxy.Elementi.ElementoRubrica elementoRubrica, DocsPaWR.ElementoRubricaRF elementoDaInviare)
        {
            if (elementoDaInviare.RF != null)
            {
                // Impostazione dei dati aggiuntivi
                elementoRubrica.Indirizzo = elementoDaInviare.RF.indirizzo;
                elementoRubrica.Nazione = elementoDaInviare.RF.nazionalita;
                elementoRubrica.Telefono = elementoDaInviare.RF.telefono1;
                elementoRubrica.Fax = elementoDaInviare.RF.fax;
                elementoRubrica.Citta = elementoDaInviare.RF.citta;
                elementoRubrica.Cap = elementoDaInviare.RF.cap;
                elementoRubrica.Provincia = elementoDaInviare.RF.prov;
                elementoRubrica.CodiceFiscale = elementoDaInviare.RF.codfisc;
                elementoRubrica.PartitaIva = elementoDaInviare.RF.partitaiva;


            }
        }
    }

}