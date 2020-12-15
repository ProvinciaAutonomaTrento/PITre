using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Subscriber.Rules
{
    /// <summary>
    /// Classe astratta che definisce la logica di gestione comune a tutte le regole per la pubblicazione di contenuti
    /// </summary>
    public abstract class BaseRule : IRule
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(BaseRule));

        /// <summary>
        /// Inizializzazione regola di pubblicazione
        /// </summary>
        /// <param name="request">Dati della richiesta di pubblicazione</param>
        /// <param name="ruleInfo">Dati della regola</param>
        public virtual void InitializeRule(Subscriber.Listener.ListenerRequest request, Subscriber.RuleInfo ruleInfo)
        {
            this.ListenerRequest = request;
            
            this.Response = new RuleResponse
            {
                Rule = ruleInfo
            };
        }

        /// <summary>
        /// Nome della regola di pubblicazione
        /// </summary>
        public abstract string RuleName
        {
            get;
        }

        /// <summary>
        /// Reperimento delle eventuali sottoregole di pubblicazione
        /// </summary>
        /// <returns>
        /// Lista delle sottoregole di pubblicazione definite per la regola
        /// </returns>
        public abstract string[] GetSubRules();

        /// <summary>
        /// Esecuzione della logica di pubblicazione dei contenuti
        /// <remarks>
        /// Imposta automaticamente l'attributo Response con l'esito della pubblicazione effettuata
        /// </remarks>
        /// </summary>
        /// <returns></returns>
        public virtual void Execute()
        {
            _logger.Info("BEGIN");

            try
            {   
                this.InternalExecute();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                // Eccezione non gestita
                this.Response.Rule.Error = new ErrorInfo
                {
                    Id = Subscriber.ErrorCodes.UNHANDLED_ERROR,
                    Message = ex.Message,
                    Stack = ex.ToString()
                };
            }
            finally
            {
                // La regola è considerata computed se almeno una sottoregola è, a sua volta, computed
                this.Response.Rule.Computed = this.Response.Rule.Computed || (this.Response.Rule.SubRules.Count(e => e.Computed) > 0);
                this.Response.Rule.ComputeDate = DateTime.Now;

                _logger.Info("END");
            }
        }

        /// <summary>
        /// Esecuzione della regola di pubblicazione
        /// <remarks>
        /// Nota per gli utilizzatori: implementare in questo metodo la logica di calcolo della pubblicazione
        /// e, in caso di errore, sollevare un'eccezione
        /// </remarks>
        /// </summary>
        protected abstract void InternalExecute();

        /// <summary>
        /// Determina se una regola o sottoregola risulta abilitata o meno 
        /// </summary>
        /// <param name="rule">Dati della regola</param>
        /// <remarks>
        /// Nota per gli utilizzatori: è possibile sovrascrivere il metodo per gestire
        /// politiche di abilitazione più complesse rispetto al semplice flag di abilitazione della regola
        /// </remarks>
        /// <returns></returns>
        protected virtual bool IsRuleEnabled(BaseRuleInfo rule)
        {
            return rule.Enabled;
        }

        /// <summary>
        /// Reperimento dei dati storicizzati relativi all'ultima pubblicazione effettuata da una regola
        /// </summary>
        /// <param name="rule">
        /// Dati della regola di pubblicazione
        /// </param>
        protected virtual RuleHistoryInfo GetLastExecutedRule(BaseRuleInfo rule)
        {
            // Reperimento, dall'history, dei dati relativi all'ultima esecuzione della regola per l'oggetto da pubblicare
            return DataAccess.RuleHistoryDataAdapter.GetLastRuleHistory(rule.Id, this.ListenerRequest.EventInfo.PublishedObject.IdObject);
        }

        /// <summary>
        /// Restituizione del valore di una proprietà di tipo stringa dell'oggetto da pubblicare
        /// </summary>
        /// <param name="propertyName">
        /// Nome della proprietà dell'oggetto
        /// </param>
        /// <returns></returns>
        protected string GetPropertyValueAsString(string propertyName)
        {
            return this.GetPropertyValueAsString(this.FindProperty(propertyName));
        }

        /// <summary>
        /// Restituizione del valore di una proprietà di tipo stringa dell'oggetto da pubblicare
        /// </summary>
        /// <param name="p">Proprietà dell'oggetto</param>
        /// <returns></returns>
        protected string GetPropertyValueAsString(Property p)
        {
            if (p != null && p.Value != null)
                return p.Value.ToString().Trim();
            else
                return string.Empty;
        }

        /// <summary>
        /// Restituizione del valore di una proprietà di tipo data dell'oggetto da pubblicare
        /// </summary>
        /// <param name="propertyName">Nome della proprietà dell'oggetto</param>
        /// <returns></returns>
        protected DateTime GetPropertyValueAsDate(string propertyName)
        {
            Property p = this.FindProperty(propertyName);

            if (p == null)
                throw new SubscriberException(ErrorCodes.MISSING_FIELD, string.Format(ErrorDescriptions.MISSING_FIELD, propertyName));

            return this.GetPropertyValueAsDate(p);
        }

        /// <summary>
        /// Restituizione del valore di una proprietà di tipo data dell'oggetto da pubblicare
        /// </summary>
        /// <param name="p">Proprietà dell'oggetto</param>
        /// <returns></returns>
        protected DateTime GetPropertyValueAsDate(Property p)
        {
            DateTime date = DateTime.MinValue;

            if (p != null && p.Value != null)
               // FREZZA DateTime.TryParse(p.Value.ToString(), out date);
        //LULUCIANI
                DateTime.TryParseExact(p.Value.ToString(),"dd/MM/yyyy HH:mm:ss", null ,System.Globalization.DateTimeStyles.None, out date);

            return date;
        }

        /// <summary>
        /// Ricerca di una proprietà dell'oggetto da pubblicare
        /// </summary>
        /// <param name="propertyName">Nome della proprietà</param>
        /// <returns>Dati della proprietà</returns>
        protected virtual Property FindProperty(string propertyName)
        {
            return this.ListenerRequest.EventInfo.PublishedObject.Properties.Where(e => e.Name.ToLowerInvariant().Trim() == propertyName.ToLowerInvariant().Trim()).FirstOrDefault();
        }

        /// <summary>
        /// Ricerca di una proprietà dell'oggetto da pubblicare nella storicizzazione dell'ultima pubblicazione effettuata
        /// </summary>
        /// <param name="propertyName">Nome della proprietà dell'oggetto</param>
        /// <param name="rule">Dati della regola</param>
        /// <returns></returns>
        protected virtual Property FindStoredProperty(string propertyName, BaseRuleInfo rule)
        {
            // Reperimento, dall'history, dei dati relativi all'ultima esecuzione della regola per l'oggetto da pubblicare
            RuleHistoryInfo lastExecutedRule = DataAccess.RuleHistoryDataAdapter.GetLastRuleHistory(rule.Id, this.ListenerRequest.EventInfo.PublishedObject.IdObject);
            
            return this.FindStoredProperty(propertyName, lastExecutedRule);
        }

        /// <summary>
        /// Ricerca di una proprietà dell'oggetto da pubblicare nella storicizzazione di una pubblicazione effettuata
        /// </summary>
        /// <param name="propertyName">Nome della proprietà dell'oggetto</param>
        /// <param name="lastExecutedRule">Dati storicizzati della regola</param>
        /// <returns></returns>
        protected virtual Property FindStoredProperty(string propertyName, RuleHistoryInfo lastExecutedRule)
        {
            Property p = null;

            if (lastExecutedRule != null && lastExecutedRule.ObjectSnapshot != null)
            {
                p = lastExecutedRule.ObjectSnapshot.Properties.Where(e => e.Name.ToLowerInvariant().Trim() == propertyName.ToLowerInvariant().Trim()).FirstOrDefault();
            }

            return p;
        }

        /// <summary>
        /// Verifica se una proprietà dell'oggetto da pubblicare è 
        /// stata modificata rispetto a quella dell'eventuale ultimo oggetto pubblicato
        /// </summary>
        /// <param name="actualProperty">Proprietà attuale</param>
        /// <param name="oldProperty">Proprietà storicizzata</param>        
        /// <returns>
        /// Esito  del confronto: se true, il dato dell'oggetto risulta modifiato rispetto ad una suo stato precedente
        /// </returns>
        protected virtual bool IsDirtyProperty(Property actualProperty, Property oldProperty)
        {
            //FieldStateTypesEnum state = this.GetFieldState(actualProperty, oldProperty);

            //return (state != FieldStateTypesEnum.Unchanged);

            bool isDirty = false;

            if (actualProperty != null && oldProperty == null)
            {
                // L'oggetto storicizzato non presenta la proprietà richiesta,
                // mentro l'oggetto attuale si, pertanto la proprietà deve essere considerata modificata
                isDirty = (!actualProperty.IsEmpty);
            }
            else if (actualProperty == null && oldProperty != null)
            {
                // L'oggetto storicizzato presenta la proprietà richiesta,
                // mentro l'oggetto attuale no, pertanto la proprietà deve non essere considerata modificata
                isDirty = (!oldProperty.IsEmpty);
            }
            else
            {
                isDirty = !this.AreEquals(actualProperty.Value.ToString(), oldProperty.Value.ToString());
                ////isDirty = string.Compare(actualProperty.Value.ToString(), oldProperty.Value.ToString(), true) != 0;

                //// Confronto delle rispettive proprietà
                //int ret = ((IComparable)actualProperty.Value).CompareTo(oldProperty.Value);

                //isDirty = (ret != 0);
            }

            return isDirty;
        }

        /// <summary>
        /// Creazione oggetto per l'invio della mail di notifica della regola
        /// </summary>
        /// <param name="rule">Dati della regola di pubblicazione</param>
        protected virtual Subscriber.Dispatcher.CalendarMail.MailRequest CreateMailRequest(BaseRuleInfo rule)
        {
            // Inserimento dati mittente
            Subscriber.Dispatcher.CalendarMail.MailSender sender = new Subscriber.Dispatcher.CalendarMail.MailSender
            {
                Host = this.ListenerRequest.ChannelInfo.SmtpHost,
                Port = this.ListenerRequest.ChannelInfo.SmtpPort,
                SSL = this.ListenerRequest.ChannelInfo.SmtpSsl,
                UserName = this.ListenerRequest.ChannelInfo.SmtpUserName,
                Password = this.ListenerRequest.ChannelInfo.SmtpPassword,
                SenderEMail = this.ListenerRequest.ChannelInfo.SmtpMail,
                SenderName = this.ListenerRequest.EventInfo.Author.RoleName
            };

            return new Subscriber.Dispatcher.CalendarMail.MailRequest
            {
                Sender = sender
            };
        }

        /// <summary>
        /// Azione di invio degli appuntamenti tramite mail
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="mailRequest"></param>
        protected virtual void DispatchMail(BaseRuleInfo rule, Dispatcher.CalendarMail.MailRequest mailRequest)
        {
            try
            {
                using (Subscriber.Dispatcher.CalendarMail.MailDispatcher dispatcher = new Subscriber.Dispatcher.CalendarMail.MailDispatcher())
                {
                    dispatcher.Dispatch(mailRequest);
                }
            }
            catch (Exception ex)
            {
                rule.Error = new ErrorInfo
                {
                    Id = ErrorCodes.SEND_MAIL_ERROR,
                    Message = string.Format(ErrorDescriptions.SEND_MAIL_ERROR, ex.Message),
                    Stack = ex.StackTrace
                };
            }
        }

        /// <summary>
        /// Dati della richiesta forniti dal Publisher al Subscriber
        /// </summary>
        protected Listener.ListenerRequest ListenerRequest
        {
            get;
            set;
        }

        /// <summary>
        /// Esito del calcolo della regola
        /// </summary>
        public Subscriber.Rules.RuleResponse Response
        {
            get;
            protected set;
        }

        /// <summary>
        /// Deallocazione risorse
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Possibili stati di un campo dell'oggetto da pubblicare rispetto ad una sua versione storicizzata
        /// </summary>
        protected enum FieldStateTypesEnum
        {
            Unchanged,
            Removed,
            Inserted,
            Changed,            
        }

        /// <summary>
        /// Reperimento dello stato di modifica del campo "Descrizione" dell'oggetto
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected virtual FieldStateTypesEnum GetDescriptionFieldState(BaseRuleInfo rule)
        {
            Property pDescription = new Property
            {
                Name= "Description",
                Type = PropertyTypesEnum.String,
                Value = this.ListenerRequest.EventInfo.PublishedObject.Description
            };

            RuleHistoryInfo lastExecutedRule = this.GetLastExecutedRule(rule);

            Property pOldDescription = new Property
            {
                Name = "Description",
                Type = PropertyTypesEnum.String
            };

            if (lastExecutedRule != null && lastExecutedRule.ObjectSnapshot != null)
                pOldDescription.Value = lastExecutedRule.ObjectSnapshot.Description;

            return this.GetFieldState(pDescription, pOldDescription);
        }

        /// <summary>
        /// Reperimento dello stato di modifica di un campo
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected virtual FieldStateTypesEnum GetFieldState(string fieldName, BaseRuleInfo rule)
        {
            Property actualProperty = this.FindProperty(fieldName);
            Property oldProperty = this.FindStoredProperty(fieldName, rule);

            return this.GetFieldState(actualProperty, oldProperty);
        }

        /// <summary>
        /// Reperimento dello stato di modifica di un campo
        /// </summary>
        /// <param name="actualProperty"></param>
        /// <param name="oldProperty"></param>
        /// <returns></returns>
        protected virtual FieldStateTypesEnum GetFieldState(Property actualProperty, Property oldProperty)
        {
            FieldStateTypesEnum state = FieldStateTypesEnum.Unchanged;

            if (actualProperty == null && oldProperty != null)
            {
                state = FieldStateTypesEnum.Removed;
            }
            else if (actualProperty != null && oldProperty == null)
            {
                state = FieldStateTypesEnum.Inserted;
            }
            else if (actualProperty != null && oldProperty != null &&
                    actualProperty.Value != null && oldProperty.Value != null)
            {
                if (actualProperty.Type == PropertyTypesEnum.Date && 
                    oldProperty.Type == PropertyTypesEnum.Date)
                {
                    DateTime d1;
                    DateTime.TryParse(actualProperty.Value.ToString(), out d1);

                    DateTime d2;
                    DateTime.TryParse(oldProperty.Value.ToString(), out d2);

                    if (DateTime.Compare(d1, d2) != 0)
                        state = FieldStateTypesEnum.Changed;
                }
                else
                {
                    // Il legale è stato modificato, è necessario inviare la cancellazione di tutti gli appuntamenti
                    // in precedenza inviato al vecchio legale
                    if (string.Compare(actualProperty.Value.ToString(), oldProperty.Value.ToString(), true) != 0)
                    {
                        state = FieldStateTypesEnum.Changed;
                    }
                }
            }

            return state;
        }

        /// <summary>
        /// Verifica se due stringe sono uguali
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        protected bool AreEquals(string value1, string value2)
        {
            return string.Compare(this.NormalizeString(value1), this.NormalizeString(value2), true) == 0;
        }

        /// <summary>
        /// Normalizzazione di una stringa
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual string NormalizeString(string value)
        {
            if (value == null)
                return string.Empty;

            return value.Trim().Replace(" ", string.Empty).ToLowerInvariant();
        }
    }
}
