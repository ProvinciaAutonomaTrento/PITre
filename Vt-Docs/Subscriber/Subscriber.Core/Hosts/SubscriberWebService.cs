using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Subscriber.Hosts
{
    /// <summary>
    /// Servizi Web per la gestione dei canali di pubblicazione del Subscriber
    /// </summary>
    [WebService(Namespace = "http://www.valueteam.com/VTDocs/Publishing")]
    public class SubscriberWebService : WebService
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(SubscriberWebService));

        /// <summary>
        /// Servizio per la notifica di un evento di pubblicazione verso un canale di pubblicazione.
        /// Analizza tutte le regole di pubblicazione registrate per il canale di pubblicazione 
        /// richiamandone il metodo Execute.
        /// Ciascuna regola stabilisce, in base ai propri vincoli di business,
        /// se è necessaria la pubblicazione dei contenuti verso una destinazione.
        /// L'esito di ciascun calcolo delle regole è restituito come valore di ritorno del servizio. 
        /// </summary>
        /// <param name="request">
        /// Dettagli dell'evento verificatosi nel sistema documentale
        /// </param>
        /// <returns>
        /// Esito della pubblicazione dell'oggetto.
        /// Riporta l'esito del calcolo effettuato da ciascuna regola di pubblicazione registrata per il canale.
        /// <remarks>
        /// La pubblicazione dovrà presumersi effettuata se almeno l'analisi 
        /// di una regola ha determinato la pubblicazione dell'oggetto verso una destinazione. 
        /// Gli eventuali errori ottenuti saranno restituiti nell'attributo Error.
        /// </remarks>
        /// </returns>
        [WebMethod()]
        public Subscriber.Listener.ListenerResponse NotifyEvent(Subscriber.Listener.ListenerRequest request)
        {
            _logger.Info("BEGIN");

            Subscriber.Listener.ListenerResponse response = new Subscriber.Listener.ListenerResponse();

            try
            {
                List<Subscriber.Rules.RuleResponse> ruleResponseList = new List<Subscriber.Rules.RuleResponse>();

                // Reperimento delle regole definite per l'istanza
                RuleInfo[] instanceRules = Subscriber.DataAccess.RuleDataAdapter.GetRules(request.ChannelInfo.Id);

                if (instanceRules != null)
                {
                    foreach (RuleInfo r in instanceRules)
                    {
                        Type t = Type.GetType(r.RuleClassFullName, false);

                        if (t == null)
                        {
                            // Regola non istanziata
                            _logger.ErrorFormat(string.Format(ErrorDescriptions.INVALID_RULE_TYPE, r.RuleClassFullName));

                            r.Error = new ErrorInfo
                            {
                                Id = ErrorCodes.INVALID_RULE_TYPE,
                                Message = string.Format(ErrorDescriptions.INVALID_RULE_TYPE, r.RuleClassFullName)
                            };

                            ruleResponseList.Add(new Rules.RuleResponse
                            {
                                Rule = r
                            });
                        }
                        else
                        {
                            // Esecuzione delle azioni che estendono la classe astratta "AvvocaturaBaseRule"
                            using (Subscriber.Rules.IRule rule = (Subscriber.Rules.IRule)Activator.CreateInstance(t))
                            {
                                _logger.InfoFormat("Creazione RULE '{0}'", rule.GetType().FullName);

                                // Inizializzazione ed esecuzione della regola
                                rule.InitializeRule(request, r);

                                _logger.InfoFormat("Inizializzazione RULE '{0}'", rule.GetType().FullName);

                                if (rule.Response.Rule.Enabled)
                                {
                                    _logger.InfoFormat("RULE '{0}' abilitata", rule.GetType().FullName);

                                    // Se la regola risulta abilitata, viene eseguita
                                    rule.Execute();

                                    _logger.InfoFormat("RULE '{0}' eseguita", rule.GetType().FullName);

                                    // Esito del calcolo della regola
                                    Subscriber.Rules.RuleResponse ruleResponse = rule.Response;

                                    ruleResponseList.Add(ruleResponse);
                                }
                                else
                                {
                                    _logger.InfoFormat("RULE '{0}' non abilitata", rule.GetType().FullName);
                                }
                            }
                        }
                    }
                }

                // Esito dell'esecuzione delle regole
                response.RuleResponseList = ruleResponseList.ToArray();
            }
            catch (Exception ex)
            {
                // Errore non gestito nel listener

                _logger.Error(ex.Message, ex);

                response.Error = new ErrorInfo
                {
                    Id = ErrorCodes.UNHANDLED_ERROR,
                    Message = ex.Message,
                    Stack = ex.ToString()
                };
            }
            finally
            {
                _logger.Info("END");
            }

            return response;
        }

        /// <summary>
        /// Reperimento dei canali di pubblicazione definiti per il Subscriber
        /// </summary>
        /// <returns>
        /// Lista dei canali di pubblicazione registrati
        /// </returns>
        [WebMethod()]
        public ChannelInfo[] GetChannelList()
        {
            return DataAccess.ChannelDataAdapter.GetList();
        }

        /// <summary>
        /// Reperimento di un canale di pubblicazione del Subscriber
        /// </summary>
        /// <param name="id">
        /// Identificativo univoco del canale di pubblicazione
        /// </param>
        /// <returns>
        /// Dati del canale di pubblicazione richiesto
        /// </returns>
        [WebMethod()]
        public ChannelInfo GetChannel(int id)
        {
            return DataAccess.ChannelDataAdapter.Get(id);
        }

        /// <summary>
        /// Aggiornamento dati del canale di pubblicazione
        /// </summary>
        /// <param name="channel">
        /// Dati del canale di pubblicazione da aggiornare
        /// </param>
        /// <returns>
        /// Dati del canale di pubblicazione aggiornato nei dati identificativi
        /// <remarks>
        /// Se il canale viene inserito, l'attributo Id riporterà l'identificativo
        /// univoco assegnato dal sistema
        /// </remarks>
        /// </returns>
        [WebMethod()]
        public ChannelInfo SaveChannel(ChannelInfo channel)
        {
            return DataAccess.ChannelDataAdapter.Save(channel);
        }

        /// <summary>
        /// Rimozione del canale di pubblicazione
        /// </summary>
        /// <param name="idChannel">
        /// Identificativo univoco del canale di pubblicazione da rimuovere
        /// </param>
        [WebMethod()]
        public void DeleteChannel(int idChannel)
        {
            DataAccess.ChannelDataAdapter.Delete(idChannel);
        }

        /// <summary>
        /// Reperimento della lista delle regole definite per un canale di pubblicazione
        /// </summary>
        /// <param name="idChannel">
        /// Identificativo univoco del canale di pubblicazione
        /// </param>
        /// <returns>
        /// Lista delle regole di pubblicazione associate al canale
        /// </returns>
        [WebMethod()]
        public RuleInfo[] GetRules(int idChannel)
        {
            return DataAccess.RuleDataAdapter.GetRules(idChannel);
        }

        /// <summary>
        /// Reperimento di una regola definita per un canale di pubblicazione
        /// </summary>
        /// <param name="id">
        /// Identificativo univoco della regola
        /// </param>
        /// <returns>
        /// Dati della regola di pubblicazione richiesta
        /// </returns>
        [WebMethod()]
        public RuleInfo GetRule(int id)
        {
            return DataAccess.RuleDataAdapter.GetRule(id);
        }

        /// <summary>
        /// Aggiornamento dati di una regola di pubblicazione
        /// </summary>
        /// <param name="rule">
        /// Dati della regola di pubblicazione da aggiornare
        /// </param>
        /// <returns>
        /// Dati della regola di pubblicazione aggiornata.
        /// <remarks>
        /// Se la regola viene inserita, l'attributo Id riporterà l'identificativo
        /// univoco assegnato dal sistema
        /// </remarks>
        /// </returns>
        [WebMethod()]
        public RuleInfo SaveRule(RuleInfo rule)
        {
            return DataAccess.RuleDataAdapter.SaveRule(rule);
        }

        /// <summary>
        /// Rimozione di una regola di pubblicazione
        /// </summary>
        /// <param name="id">
        /// Identificativo univoco della regola da rimuovere
        /// </param>
        [WebMethod()]
        public void DeleteRule(int id)
        {
            DataAccess.RuleDataAdapter.DeleteRule(id);
        }

        /// <summary>
        /// Reperimento delle sottoregole di pubblicazione associate ad una regola
        /// </summary>
        /// <param name="idRule">
        /// Identificativo univoco della regola di pubblicazione padre
        /// </param>
        /// <returns>
        /// Lista delle sottoregole di pubblicazione
        /// </returns>
        [WebMethod()]
        public SubRuleInfo[] GetSubRules(int idRule)
        {
            return DataAccess.RuleDataAdapter.GetSubRules(idRule);
        }

        /// <summary>
        /// Reperimento di una sottoregola di pubblicazione
        /// </summary>
        /// <param name="id">
        /// Identificativo univoco della sottoregola
        /// </param>
        /// <returns>
        /// Dati della sottoregola di pubblicazione
        /// </returns>
        [WebMethod()]
        public SubRuleInfo GetSubRule(int id)
        {
            return DataAccess.RuleDataAdapter.GetSubRule(id);
        }

        /// <summary>
        /// Aggiornamento metadati di una sottoregola di pubblicazione
        /// </summary>
        /// <param name="subRule">
        /// Dati della sottoregola di pubblicazione da aggiornare
        /// </param>
        /// <returns>
        /// Dati della sottoregola di pubblicazione aggiornata
        /// <remarks>
        /// Se la sottoregola viene inserita, l'attributo Id riporterà l'identificativo
        /// univoco assegnato dal sistema
        /// </remarks>
        /// </returns>
        [WebMethod()]
        public SubRuleInfo SaveSubRule(SubRuleInfo subRule)
        {
            return DataAccess.RuleDataAdapter.SaveSubRule(subRule);
        }

        /// <summary>
        /// Rimozione di una sottoregola di pubblicazione
        /// </summary>
        /// <param name="id">
        /// Identificativo univoco della sottoregola da rimuovere
        /// </param>
        [WebMethod()]
        public void DeleteSubRule(int id)
        {
            DataAccess.RuleDataAdapter.DeleteSubRule(id);
        }

        /// <summary>
        /// Reperimento dei dati delle pubblicazioni di oggetti effettuate da una regola nel corso del tempo.
        /// I calcoli effettuati sono storicizzati per consentire logiche di pubblicazioni
        /// di oggetti basate sul confronto con i dati attuali.
        /// Es. La pubblicazione di un documento di tipo delibera sarà effettuata solo se 
        /// il campo "Data di pubblicazione" risulta modificato rispetto alla pubblicazione precedente.
        /// </summary>
        /// <param name="requestInfo">
        /// Dati di filtro della richiesta.
        /// Dato obbligatorio da prevedere: Id della regola.
        /// E' consentita la paginazione dei risultati.
        /// </param>
        /// <returns>
        /// Dati delle pubblicazioni effettuate da una regola
        /// </returns>
        [WebMethod()]
        public GetRuleHistoryListResponse GetRuleHistoryList(GetRuleHistoryListRequest requestInfo)
        {
            return DataAccess.RuleHistoryDataAdapter.GetRuleHistoryList(requestInfo);
        }

        /// <summary>
        /// Reperimento dei dati di una pubblicazione storicizzata
        /// </summary>
        /// <param name="id">
        /// Identificativo univoco della pubblicazione storicizzata
        /// </param>
        /// <returns>
        /// Dati della pubblicazione storicizzata
        /// </returns>
        [WebMethod()]
        public RuleHistoryInfo GetHistoryItem(int id)
        {
            return DataAccess.RuleHistoryDataAdapter.GetHistoryItem(id);
        }
    }
}
