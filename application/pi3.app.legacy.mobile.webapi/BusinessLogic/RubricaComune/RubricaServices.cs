// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;
using DocsPaVO.RubricaComune;
using DocsPaVO.utente;
using Serilog;

namespace BusinessLogic.RubricaComune
{
    public class RubricaServices
    {
        private static ILogger logger = Log.ForContext(typeof(RubricaServices));

        public static DocsPaVO.utente.Corrispondente UpdateCorrispondente(DocsPaVO.utente.InfoUtente infoUtente, string codiceRubrica)
        {
#if false   // chiama web service rubrica comune
            RC.Proxy.Elementi.ElementoRubrica elemento = GetElementoInRubricaComune(infoUtente, codiceRubrica);

            if (elemento != null)
                return UpdateCorrispondente(infoUtente, elemento);
            else
                return null;

#endif
            return null;
        }

        public static DocsPaVO.utente.Corrispondente UpdateCorrispondente(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.utente.Corrispondente corrispondente)
        {
            logger.Debug("UpdateCorrispondente start");
            logger.Debug("UpdateCorrispondente - GetElementoInRubricaComune -start");
#if false // chiamata a web service rubrica comune
            RC.Proxy.Elementi.ElementoRubrica elemento = GetElementoInRubricaComune(infoUtente, corrispondente.codiceRubrica);


            if (elemento != null)
            {
                logger.Debug("UpdateCorrispondente: trovato elemento");
                bool isCorrModified = false;
                Corrispondente corr = UpdateCorrispondente(infoUtente, corrispondente, elemento, out isCorrModified);

                //Emanuela 20-01-2014: aggiunto l'inserimento in DPA_MAIL_CORR_ESTERNI per corretto aggiornamento del corrispondente appartenente ad una lista
                if (corr != null && (!string.IsNullOrEmpty(corr.systemId)) && isCorrModified)
                {
                    if (elemento.Emails.Length > 0)
                    {
                        List<MailCorrispondente> listCaselle = new List<MailCorrispondente>();
                        foreach (RC.Proxy.Elementi.EmailInfo mail in elemento.Emails)
                        {
                            listCaselle.Add(new MailCorrispondente
                            {
                                Email = mail.Email,
                                Note = mail.Note,
                                Principale = (mail.Preferita) ? "1" : "0"
                            });
                        }
                        Utenti.addressBookManager.InsertMailCorrispondente(listCaselle, corr.systemId);
                    }
                }

                return corr;
            }
            else
            {
                logger.Debug("UpdateCorrispondente:  NON trovato elemento");
                return null;
            }

#endif
            return null;
        }


#if false // chiama web service rubrica comune
        private static RC.Proxy.Elementi.ElementoRubrica GetElementoInRubricaComuneByEmail(DocsPaVO.utente.InfoUtente infoUtente, string email)
        {
            // Reperimento delle configurazioni per la gestione della rubrica comune da docspa
            ConfigurazioniRubricaComune config = Configurazioni.GetConfigurazioni(infoUtente);

            if (config.GestioneAbilitata)
            {
                RC.ElementiRubricaServices services = new global::RubricaComune.ElementiRubricaServices(config.ServiceRoot, config.SuperUserId, config.SuperUserPwd);

                try
                {
                    // Impostazione criterio di ricerca per email
                    RC.Proxy.Elementi.CriterioRicerca criterioRicerca = new global::RubricaComune.Proxy.Elementi.CriterioRicerca();
                    criterioRicerca.Nome = "email";
                    criterioRicerca.TipoRicerca = global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParolaIntera;
                    criterioRicerca.Valore = email;

                    RC.Proxy.Elementi.OpzioniRicerca opzioniRicerca = new global::RubricaComune.Proxy.Elementi.OpzioniRicerca();

                    opzioniRicerca.CriteriRicerca = new global::RubricaComune.Proxy.Elementi.CriteriRicerca();
                    opzioniRicerca.CriteriRicerca.Criteri = new global::RubricaComune.Proxy.Elementi.CriterioRicerca[1] { criterioRicerca };

                    RC.Proxy.Elementi.ElementoRubrica[] elementi = services.Search(ref opzioniRicerca);

                    if (elementi.Length > 0)
                        return elementi[0];
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    logger.Error(SEARCH_ERROR, ex);

                    return null;
                }
            }
            else
                return null;
        }

#endif

        /// <summary>
        /// Creazione di un elemento contenente gli attributi
        /// di un corrispondente docspa da inviare a rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idUO">
        /// Id dell'uo docspa da inviare a rubrica comune
        /// </param>
        /// <returns></returns>
        public static ElementoRubricaUO GetElementoRubricaUO(InfoUtente infoUtente, string idUO)
        {
            ElementoRubricaUO elemento = null;

            if (!string.IsNullOrEmpty(idUO))
            {
                // Reperimento dell'uo da inviare a rubrica comune
                OrgUO uo = GetUO(idUO);

                if (uo != null && !string.IsNullOrEmpty(uo.Codice))
                {
                    // Reperimento dati dell'amministrazione
                    InfoAmministrazione amministrazione = GetInfoAmministrazione(uo.IDAmministrazione);

                    // Creazione oggetto elemento rubrica da inviare
                    elemento = new ElementoRubricaUO();

                    // Impostazione dell'uo da inviare a rubrica comune
                    elemento.UO = uo;
                    // Reperimento codice uo per rubrica comune
                    elemento.CodiceRubrica = GetCodiceRubricaUO(amministrazione, uo.Codice);
                    //elemento.CodiceRubrica = GetCodiceRubrica(amministrazione, uo.Descrizione);

                    // Reperimento descrizione uo per rubrica comune
                    elemento.DescrizioneRubrica = GetDescrizioneRubrica(amministrazione, uo.Descrizione);

                    // Codice dell'amministrazione
                    elemento.Amministrazione = DocsPaDB.Utils.Personalization.getInstance(uo.IDAmministrazione).getCodiceAmministrazione();

                    // Codice AOO
                    elemento.AOO = uo.CodiceRegistroInterop;
                }
            }

            return elemento;
        }

        /// <summary>
        /// Reperimento dell'uo per rubrica comune
        /// </summary>
        /// <param name="idUO"></param>
        /// <returns></returns>
        private static OrgUO GetUO(string idUO)
        {
            // Reperimento dell'uo da inviare a rubrica comune
            OrgUO uo = Amministrazione.OrganigrammaManager.AmmGetDatiUOCorrente(idUO);

            if (uo != null)
                uo.DettagliUo = BusinessLogic.Amministrazione.OrganigrammaManager.AmmGetDatiStampaBuste(idUO);

            return uo;
        }

        /// <summary>
        /// Reperimento oggetto che rappresenta i dati dell'amministrazione
        /// </summary>
        /// <param name="idAmm"></param>
        /// <returns></returns>
        private static InfoAmministrazione GetInfoAmministrazione(string idAmm)
        {
            return Amministrazione.AmministraManager.AmmGetInfoAmmCorrente(idAmm);
        }

        /// <summary>
        /// Reperimento del codice univoco dell'uo per rubrica comune,
        /// come composizione del codice dell'amministrazione e del codice uo
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="uo"></param>
        /// <returns></returns>
        private static string GetCodiceRubricaUO(InfoAmministrazione amministrazione, String codiceCorr)
        {
            return string.Format("{0}-{1}", amministrazione.Codice, codiceCorr);
        }

        /// <summary>
        /// Reperimento della descrizione dell'uo per rurbica comune,
        /// come composizione del codice dell'amministrazione e della descrizione dell'uo o dell'RF
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="uo"></param>
        /// <returns></returns>
        private static string GetDescrizioneRubrica(InfoAmministrazione amministrazione, String descrizione)
        {
            return string.Format("{0} - {1}", amministrazione.Codice, descrizione);
        }

        /// <summary>
        /// Creazione di un elemento contenente gli attributi
        /// di un RF da inviare a rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="idRF">
        /// Id dell'RF da inviare a rubrica comune
        /// </param>
        /// <returns>Elemento inserito in Rubrica Comune</returns>
        public static ElementoRubricaRF GetElementoRubricaRF(InfoUtente infoUtente, string idRF)
        {
            ElementoRubricaRF elemento = null;

            if (!String.IsNullOrEmpty(idRF))
            {
                // Reperimento dell'RF da inviare a rubrica comune
                RaggruppamentoFunzionale rf = BusinessLogic.Utenti.RegistriManager.GetRaggruppamentoFunzionaleRC(idRF);

                if (rf != null)
                {
                    // Reperimento dati dell'amministrazione
                    InfoAmministrazione amministrazione = GetInfoAmministrazione(rf.idAmministrazione);

                    // Creazione oggetto elemento rubrica da inviare
                    elemento = new ElementoRubricaRF();

                    // Impostazione dell'RF da inviare a rubrica comune
                    elemento.RF = rf;

                    // Reperimento codice RF per rubrica comune
                    elemento.CodiceRubrica = GetCodiceRubricaRF(amministrazione, rf.Codice);

                    // Reperimento descrizione uo per rubrica comune
                    elemento.DescrizioneRubrica = GetDescrizioneRubrica(amministrazione, rf.descrizione);

                    // Dati per l'interoperabilità, se presente l'elemento rubrica è configurato per l'interoperabilità
                    //if (!string.IsNullOrEmpty(uo.CodiceRegistroInterop))
                    //    elemento.EMailRegistro = GetEMailRegistroInterop(uo.IDAmministrazione, uo.CodiceRegistroInterop);

                    // Codice dell'amministrazione
                    elemento.Amministrazione = amministrazione.Codice;

                    // Codice AOO
                    elemento.AOO = rf.codiceAOO;

                }
            }

            return elemento;
        }

        /// <summary>
        /// Reperimento del codice univoco dell'rf per rubrica comune,
        /// come composizione del codice dell'amministrazione e del codice uo
        /// </summary>
        /// <param name="amministrazione"></param>
        /// <param name="uo"></param>
        /// <returns></returns>
        private static string GetCodiceRubricaRF(InfoAmministrazione amministrazione, String codiceCorr)
        {
            return string.Format("{0}-{1}", amministrazione.Codice, codiceCorr);
        }

        /// <summary>
        /// Reperimento dei dati di un elemento da rubrica comune
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codiceRubrica"></param>
        /// <param name="filtraInterni">Flag utilizzato per indicare se il metodo deve filtrare i corrispondenti interni all'amministrazioune del chiamante</param>
        /// <returns></returns>
        public static DocsPaVO.rubrica.ElementoRubrica GetElementoRubricaComune(DocsPaVO.utente.InfoUtente infoUtente, string codiceRubrica, bool filtraInterni)
        {
#if false // chiamata a servizio esterno
            RC.Proxy.Elementi.ElementoRubrica elemento = GetElementoInRubricaComune(infoUtente, codiceRubrica);

            if (elemento != null && (!filtraInterni || !InternoInAmministrazione(elemento, infoUtente)))
                return MapElementoRubrica(elemento);
            else
                return null;

#endif        
            return null;
        }

#if false // chiamata a servizio esterno
        /// <summary>
        /// Reperimento di un elemento in rubrica comune e rubrica esterna
        /// </summary>
        /// <param name="infoUtente"></param>
        /// <param name="codice"></param>
        /// <returns></returns>
        private static RC.Proxy.Elementi.ElementoRubrica GetElementoInRubricaComune(DocsPaVO.utente.InfoUtente infoUtente, string codice)
        {
            RC.Proxy.Elementi.ElementoRubrica retValue = null;

            // Reperimento delle configurazioni per la gestione della rubrica comune da docspa
            ConfigurazioniRubricaComune config = Configurazioni.GetConfigurazioni(infoUtente);
            logger.Debug("GetElementoInRubricaComune START");
            if (config.GestioneAbilitata)
            {

                RC.ElementiRubricaServices services = new global::RubricaComune.ElementiRubricaServices(config.ServiceRoot, config.SuperUserId, config.SuperUserPwd);

                try
                {
                    logger.Debug("cerco in rC questo codice" + codice);

                    bool rubricheEsterneAttive = false;
                    if (DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_ENABLE_RUBRICHE_ESTERNE") != null &&
                        DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(infoUtente.idAmministrazione, "BE_ENABLE_RUBRICHE_ESTERNE").ToString().Equals("1"))
                        rubricheEsterneAttive = true;

                    //2020: filtro su ricerca rubriche esterne
                    string rubricheEsterne = DocsPaUtils.Configuration.InitConfigurationKeys.GetValue("0", "BE_RUBRICHE_ESTERNE");
                    if (rubricheEsterneAttive && !string.IsNullOrEmpty(rubricheEsterne) && rubricheEsterne != "0")
                    {
                        List<RC.Proxy.Elementi.CriterioRicerca> criteriRicerca = new List<RC.Proxy.Elementi.CriterioRicerca>();

                        RC.Proxy.Elementi.CriterioRicerca criterioRicerca = new global::RubricaComune.Proxy.Elementi.CriterioRicerca();
                        criterioRicerca.Nome = "Codice";
                        criterioRicerca.TipoRicerca = global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParolaIntera;
                        criterioRicerca.Valore = codice.Replace("'", "''");
                        criteriRicerca.Add(criterioRicerca);

                        criterioRicerca = new global::RubricaComune.Proxy.Elementi.CriterioRicerca();
                        criterioRicerca.Nome = "rubrica_esterna";
                        criterioRicerca.TipoRicerca = global::RubricaComune.Proxy.Elementi.TipiRicercaParolaEnum.ParteDellaParola;
                        criterioRicerca.Valore = rubricheEsterne.ToUpper();
                        criteriRicerca.Add(criterioRicerca);

                        RC.Proxy.Elementi.OpzioniRicerca opzioniRicerca = new global::RubricaComune.Proxy.Elementi.OpzioniRicerca();
                        opzioniRicerca.CriteriRicerca = new global::RubricaComune.Proxy.Elementi.CriteriRicerca();
                        opzioniRicerca.CriteriRicerca.Criteri = criteriRicerca.ToArray();

                        RC.Proxy.Elementi.ElementoRubrica[] elementi = services.Search(ref opzioniRicerca);

                        if (elementi.Length > 0)
                            retValue = elementi[0];
                    }
                    else
                    {
                        retValue = services.SearchSingle(codice);
                    }

                    return retValue;
                }
                catch (Exception ex)
                {
                    logger.Error(SEARCH_ERROR, ex);

                    return null;
                }
            }
            else
                return retValue;
        } 
#endif

        public static DocsPaVO.rubrica.ElementoRubrica[] GetElementiRubricaComune(InfoUtente infoUtente, DocsPaVO.RubricaComune.FiltriRubricaComune filtri)
        {
            List<DocsPaVO.rubrica.ElementoRubrica> list = new List<DocsPaVO.rubrica.ElementoRubrica>();

            // Ricerca degli elementi in rubrica comune
#if false // chiamata a servizio esterno
            foreach (RC.Proxy.Elementi.ElementoRubrica item in RicercaInRubricaComune(infoUtente, filtri))
            {
                if (!InternoInAOO(item, infoUtente))
                {
                    // Per ciascun elemento in rubrica comune viene creato un corrispondente
                    // oggetto "ElementoRubrica" per l'utilizzo trasparente nella rubrica docspa

                    list.Add(MapElementoRubrica(item));
                }
            }

#endif
            return list.ToArray();
        }

#if false // utilizza tipi dati Ws
        private static bool InternoInAOO(RC.Proxy.Elementi.ElementoRubrica elementoRubrica, InfoUtente infoUtente)
        {
            bool retValue = false;

            using (DocsPaDB.Query_DocsPAWS.Amministrazione amministrazioneDb = new DocsPaDB.Query_DocsPAWS.Amministrazione())
            {
                if (!string.IsNullOrEmpty(elementoRubrica.AOO))
                {
                    //Estraggo tutti i registri visibili all'utente e vado a verificare se il codice AOO di uno di questi registri
                    //coincide con il codice AOO dell'elementoRubrica.
                    DocsPaDB.Query_DocsPAWS.Utenti reg = new DocsPaDB.Query_DocsPAWS.Utenti();
                    List<Registro> registri = reg.GetListaRegistriRfRuolo(infoUtente.idCorrGlobali, "0", "").Cast<Registro>().ToList();
                    retValue = (from registro in registri where registro.codRegistro.Equals(elementoRubrica.AOO) select registro).FirstOrDefault() != null;
                }
            }

            return retValue;
        }

#endif
    }
}
