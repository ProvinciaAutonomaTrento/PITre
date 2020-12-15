using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DocsPaIntegration.Config;
using DocsPaIntegration.Attributes;
using DocsPaIntegration.Search;
using DocsPaIntegration.ObjectTypes;
using DocsPaIntegration;
using DocsPaIntegration.ObjectTypes.Attributes;
using DocsPaIntegrationArca.ArcaWS;


namespace DocsPaIntegrationArca
{
    [IntegrationAdapterAttribute("ArcaIntegrationAdapter", "Arca integration adapter", "L'adapter interagisce con un webservice (il cui URL è configurato da parametro) che si interfaccia con il sistema Arca di Inps", "1.0", false)]

    public class ArcaIntegrationAdapter : GeneralIntegrationAdapter
    {
        [IntegrationStringType("WS Url", false)]
        private string WSUrl
        {
            get;
            set;
        }

        [IntegrationStringType("Provenienza", true)]
        private string Provenineza
        {
            get;
            set;
        }

        [IntegrationStringType("Matricola", true)]
        private string Matricola
        {
            get;
            set;
        }

        [IntegrationStringType("Password", true)]
        private string Password
        {
            get;
            set;
        }

        [IntegrationStringType("Applicazione", true)]
        private string Applicazione
        {
            get;
            set;
        }

        [IntegrationStringType("Ruolo", true)]
        private string Ruolo
        {
            get;
            set;
        }


        public override bool HasIcon
        {
            get
            {
                return false;
            }
        }

        public override Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        protected override void InitParticular()
        {

        }


        public override DocsPaIntegration.Search.SearchOutput Search(DocsPaIntegration.Search.SearchInfo searchInfo)
        {
            try
            {
                //Client del webservice
                ArcaWS.ArcaIntraWSService wsArca = new ArcaIntraWSService();
                wsArca.Url = WSUrl;

                //Imposta il profilo per invocare il webservice di Arca
                ArcaWS.tProfilo profilo = new tProfilo();
                SetProfilo(ref profilo);

                //Istanzia l'oggetto per il risultato della ricerca
                ArcaWS.DatiRisposta datiRisposta = new DatiRisposta();

                //Ricerca per codice fiscale
                if (searchInfo.OtherParam.ContainsKey("CF"))
                {
                    //Istanzia l'oggetto per la ricerca per codice fiscale
                    ArcaWS.RichiestaPerCodiceFiscale ricercaCF = new RichiestaPerCodiceFiscale();
                    ricercaCF.Profilo = profilo;

                    //Istanzia l'oggetto per i dati codice fiscale
                    ArcaWS.tCodiceFiscale cfRichiesta = new tCodiceFiscale();

                    //Imposta i dati del codice fiscale
                    SetCodiceFiscale(ref cfRichiesta, searchInfo.OtherParam["CF"].ToString());
                    ricercaCF.CodiceFiscale = cfRichiesta;
                    datiRisposta = wsArca.ricercaPerCodiceFiscale(ricercaCF);
                }
                else //Ricerca per dati anagrafici
                {
                    //Istanzia l'oggetto per la ricerca per dati anagrafici
                    ArcaWS.RichiestaPerDatiAnagraficiParziali ricercaDA = new RichiestaPerDatiAnagraficiParziali();
                    ricercaDA.Profilo = profilo;

                    //Istanzia l'oggetto per i data anagrafici parziali
                    ArcaWS.tDatiPersonaliParziali2 dpRichiesta = new tDatiPersonaliParziali2();

                    //Imposta i dati anagrafici parziali
                    if (searchInfo.OtherParam.ContainsKey("COGNOME"))
                        dpRichiesta.Cognome = searchInfo.OtherParam["COGNOME"].ToString();

                    if (searchInfo.OtherParam.ContainsKey("NOME"))
                        dpRichiesta.Cognome = searchInfo.OtherParam["NOME"].ToString();

                    if (searchInfo.OtherParam.ContainsKey("DATA_NASCITA"))
                    {
                        ArcaWS.tData2 dataNascita = new tData2();
                        dataNascita.Anno = searchInfo.OtherParam["DATA_NASCITA"].ToString().Substring(4, 4);
                        dataNascita.Mese = searchInfo.OtherParam["DATA_NASCITA"].ToString().Substring(2, 2);
                        dataNascita.Giorno = searchInfo.OtherParam["DATA_NASCITA"].ToString().Substring(0, 2);
                        dpRichiesta.DataNascita = dataNascita;
                    }

                    if (searchInfo.OtherParam.ContainsKey("SESSO"))
                    {
                        dpRichiesta.Sesso = searchInfo.OtherParam["SESSO"].ToString();
                    }


                    if (searchInfo.OtherParam.ContainsKey("COMUNE_NASCITA") || searchInfo.OtherParam.ContainsKey("PROVINCIA_NASCITA"))
                    {
                        ArcaWS.tLuogo2 comuneNascita = new tLuogo2();
                        if (searchInfo.OtherParam.ContainsKey("COMUNE_NASCITA"))
                            comuneNascita.Nome = searchInfo.OtherParam["COMUNE_NASCITA"].ToString();

                        if (searchInfo.OtherParam.ContainsKey("PROVINCIA_NASCITA"))
                            comuneNascita.Provincia = searchInfo.OtherParam["PROVINCIA_NASCITA"].ToString();

                        dpRichiesta.ComuneNascita = comuneNascita;
                    }

                    ricercaDA.DatiPersonali = dpRichiesta;
                    datiRisposta = wsArca.ricercaPerDatiPersonaliParziali(ricercaDA);
                }
                                

                //Se il webservice di Arca è andato in errore lancia un'eccezione con il dettaglio dell'errore
                if (datiRisposta.Esito.ReturnCode != "WS-OK")
                {
                    throw new SearchException(SearchExceptionCode.SERVER_ERROR, datiRisposta.Esito.Descrizione);
                }
                else
                {
                    List<SearchOutputRow> rows = new List<SearchOutputRow>();
                    if (datiRisposta.Esito.TotaleSinonimiRestituiti == 1)
                    {
                        SearchOutputRow row = new SearchOutputRow();
                        ArcaWS.tCodiceFiscale cfRisposta = datiRisposta.Dettaglio.HighProfile.CodiceFiscale;
                        row.Codice = cfRisposta.Cognome + cfRisposta.Nome + cfRisposta.Anno + cfRisposta.Mese + cfRisposta.Giorno + cfRisposta.Comune + cfRisposta.CodControllo;
                        row.Descrizione = datiRisposta.Dettaglio.HighProfile.DatiPersonali.Cognome + " " + datiRisposta.Dettaglio.HighProfile.DatiPersonali.Nome;
                        rows.Add(row);
                    }
                    else
                    {
                        for (int i = 0; i <= datiRisposta.Esito.TotaleSinonimiRestituiti; i++)
                        {
                            SearchOutputRow row = new SearchOutputRow();
                            ArcaWS.tCodiceFiscale cfRisposta = datiRisposta.Sinonimi[i].LowProfile.CodiceFiscale;
                            row.Codice = cfRisposta.Cognome + cfRisposta.Nome + cfRisposta.Anno + cfRisposta.Mese + cfRisposta.Giorno + cfRisposta.Comune + cfRisposta.CodControllo;
                            row.Descrizione = datiRisposta.Sinonimi[i].LowProfile.DatiPersonali.Cognome + " " + datiRisposta.Sinonimi[i].LowProfile.DatiPersonali.Nome;
                            rows.Add(row);
                        }
                    }

                    SearchOutput res = new SearchOutput(rows, datiRisposta.Esito.TotaleSinonimiRestituiti);

                    return res;
                }
            }
            catch (Exception e)
            {
                throw new SearchException(SearchExceptionCode.SERVICE_UNAVAILABLE);
            }
        }

        /// <summary>
        /// Metodo utilizzato per una ricerca puntuale, per codice fiscale
        /// </summary>
        /// <param name="puntualSearchInfo"></param>
        /// <returns></returns>
        public override DocsPaIntegration.Search.SearchOutputRow PuntualSearch(DocsPaIntegration.Search.PuntualSearchInfo puntualSearchInfo)
        {
            SearchOutputRow row = new SearchOutputRow();
            return row;
        }


        /// <summary>
        /// Imposta i dati del profilo per poter richiamare i metodi di ricerca di Arca
        /// </summary>
        /// <param name="profilo">Profilo</param>
        private void SetProfilo(ref ArcaWS.tProfilo profilo)
        {
            try
            {
                profilo.Applicazione = this.Applicazione;
                profilo.Matricola = this.Matricola;
                profilo.Password = this.Password;
                profilo.Provenienza = this.Provenineza;
                profilo.Ruolo = this.Ruolo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Imposta i dati del codice fiscale, utilizzato per la ricerca puntuale
        /// </summary>
        /// <param name="?"></param>
        private void SetCodiceFiscale(ref tCodiceFiscale cf, string codiceFiscale)
        {

            cf.Cognome = codiceFiscale.Substring(0, 3);
            cf.Nome = codiceFiscale.Substring(3, 3);
            cf.Anno = Int32.Parse(codiceFiscale.Substring(6, 2));
            cf.Mese = codiceFiscale.Substring(8, 1);
            cf.Giorno = codiceFiscale.Substring(9, 2);
            cf.Comune = codiceFiscale.Substring(11, 4);
            cf.CodControllo = codiceFiscale.Substring(15, 1); 
        }

    }
}
