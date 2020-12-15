using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Subscriber.Pat.Avvocatura.Rules
{
    /// <summary>
    /// Classe base astratta che tutte le regole dell'avvocatura devono estendere
    /// </summary>
    public abstract class AvvocaturaBaseRule : Subscriber.Rules.BaseRule
    {
        /// <summary>
        /// Nome della regola 
        /// </summary>
        public override string RuleName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Sottoregole gestite dalla regola
        /// </summary>
        /// <returns></returns>
        public override string[] GetSubRules()
        {
            return new string[0];
        }

        /// <summary>
        /// Reperimento nome del template dell'oggetto gestito dalla regola
        /// </summary>
        /// <returns></returns>
        protected virtual string GetTemplateName()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifica se i campi dipendenti ad una regola sono stati modificati o meno
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        /// <remarks>
        /// Rientrano tra i campi dipendenti da gestire in questa funzione:
        /// - Sospensione feriale dei termini
        /// - Competenza territoriale
        /// 
        /// Non rientrano tra i campi dipendenti da gestire in questa funzione:
        /// - Il legale incaricato
        /// - La tipologia causa
        /// 
        /// 
        /// </remarks>
        protected virtual bool IsDirtyDependentFields(BaseRuleInfo rule)
        {
            bool isDirty = false;

            // Verifica se è stato modificato il campo "Autorirà giudiziaria"
            FieldStateTypesEnum state = this.GetFieldState(CommonFields.AUTORITA_GIUDIZIARIA, rule);

            isDirty = (state == FieldStateTypesEnum.Changed);

            // Verifica se è stato modificato il campo "Competenza territoriale"
            if (!isDirty)
            {
                state = this.GetFieldState(CommonFields.COMPETENZA_TERRITORIALE, rule);

                isDirty = (state == FieldStateTypesEnum.Changed);
            }

            return isDirty;
        }

        /// <summary>
        /// Verifica se l'appuntamento attualmente calcolato risulta diverso dall'appuntamento storicizzato
        /// </summary>
        /// <param name="actualAppontment"></param>
        /// <param name="pastAppontment"></param>
        /// <returns></returns>
        protected virtual bool AppointmentNotScheduled(Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentInfo actualAppontment, Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentInfo pastAppontment)
        {
            bool isDirty = (pastAppontment.Status != actualAppontment.Status);

            if (!isDirty)
            {
                // Verifica se la data iniziale dell'appuntamento è stata modificata rispetto al precedente
                isDirty = (DateTime.Compare(pastAppontment.DtStart, actualAppontment.DtStart) != 0);
            }

            if (!isDirty)
            {
                // Verifica se la data finale dell'appuntamento è stata modificata rispetto al precedente
                isDirty = (DateTime.Compare(pastAppontment.DtEnd, actualAppontment.DtEnd) != 0);
            }

            if (!isDirty)
            {
                // Verifica se è stato modificato il subject dell'appuntamento rispetto al precedente
                isDirty = (string.Compare(pastAppontment.Summary, actualAppontment.Summary, true) != 0);
            }

            if (!isDirty)
            {
                // Verifica se è stata modificata la location dell'appuntamento rispetto al precedente
                isDirty = (string.Compare(pastAppontment.Location, actualAppontment.Location, true) != 0);
            }

            if (!isDirty)
            {
                // Verifica se è stato modificato il body dell'appuntamento rispetto al precedente
                isDirty = (string.Compare(pastAppontment.Description, actualAppontment.Description, true) != 0);
            }

            return isDirty;
        }

        /// <summary>
        /// Verifica se l'appuntamento attualmente calcolato risulta diverso dall'appuntamento storicizzato
        /// </summary>
        /// <param name="actualAppontment"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected virtual bool AppointmentNotScheduled(Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentInfo actualAppontment, BaseRuleInfo rule)
        {
            bool isDirty = false;

            Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentInfo pastAppontment = null;

            // Reperimento ultima esecuzione della regola
            RuleHistoryInfo lastExecutedRule = this.GetLastExecutedRule(rule);

            if (lastExecutedRule != null &&
                lastExecutedRule.MailMessageSnapshot != null &&
                lastExecutedRule.MailMessageSnapshot.Appointment != null)
            {
                pastAppontment = lastExecutedRule.MailMessageSnapshot.Appointment;
            }

            if (pastAppontment == null)
            {
                // L'appuntamento passato non esiste
                isDirty = true;
            }
            else
            {
                isDirty = this.AppointmentNotScheduled(actualAppontment, pastAppontment);
            }

            return isDirty;
        }

        /// <summary>
        /// Esecuzione della regola
        /// </summary>
        protected override void InternalExecute()
        {
            _logger.Info("BEGIN");

            if (this.ListenerRequest.EventInfo.PublishedObject.TemplateName != this.GetTemplateName())
            {
                this.Response.Rule.Error = new ErrorInfo
                {
                    Id = Subscriber.ErrorCodes.INVALID_TEMPLATE_PER_RULE,
                    Message = string.Format(Subscriber.ErrorDescriptions.INVALID_TEMPLATE_PER_RULE, this.ListenerRequest.EventInfo.PublishedObject.TemplateName)
                };
            }
            else
            {
                foreach (SubRuleInfo subRule in this.Response.Rule.SubRules)
                {
                    if (this.IsRuleEnabled(subRule))
                    {
                        // Solo se la regola è abilitata

                        if (this.GetSubRules().Contains(subRule.SubRuleName))
                        {
                            _logger.InfoFormat("BEGIN - EXECUTE SUBRULE: '{0}'", subRule.SubRuleName);

                            try
                            {
                                // Esecuzione della SubRule
                                // NB. Per default, viene eseguita la regola più semplice, 
                                // ovvero quella dipendente da un singolo campo data
                                this.InternalExecuteSubRule(subRule);
                            }
                            catch (SubscriberException pubEx)
                            {
                                // Eccezione non gestita di tipo "SubscriberException"
                                _logger.Error(pubEx.Message);

                                subRule.Computed = false;
                                subRule.ComputeDate = DateTime.Now;

                                subRule.Error = new ErrorInfo
                                {
                                    Id = pubEx.ErrorCode,
                                    Message = pubEx.Message,
                                    Stack = pubEx.ToString()
                                };

                                // Scrittura errore non gestito per la SubRule
                                this.WriteRuleHistory(subRule);

                            }
                            catch (Exception ex)
                            {
                                // Eccezione non gestita
                                _logger.Error(ex.Message);

                                subRule.Computed = false;
                                subRule.ComputeDate = DateTime.Now;

                                subRule.Error = new ErrorInfo
                                {
                                    Id = Subscriber.ErrorCodes.UNHANDLED_ERROR,
                                    Message = ex.Message,
                                    Stack = ex.ToString()
                                };

                                // Scrittura errore non gestito per la SubRule
                                this.WriteRuleHistory(subRule);
                            }

                            _logger.InfoFormat("END - EXECUTE SUBRULE: '{0}'", subRule.SubRuleName);
                        }
                        else
                        {
                            // SubRule non dichiarata nel tipo

                            subRule.Computed = false;
                            subRule.ComputeDate = DateTime.Now;

                            subRule.Error = new ErrorInfo
                            {
                                Id = ErrorCodes.SUBRULE_NAME_NOT_DECLARED,
                                Message = string.Format(ErrorDescriptions.SUBRULE_NAME_NOT_DECLARED,
                                                subRule.SubRuleName, subRule.RuleName, this.GetType().FullName)
                            };

                            // Scrittura errore non gestito per la SubRule
                            this.WriteRuleHistory(subRule);
                        }
                    }
                }
            }

            _logger.Info("END");
        }

        /// <summary>
        /// Esecuzione di una SubRule
        /// </summary>
        /// <param name="subRule"></param>
        protected virtual void InternalExecuteSubRule(SubRuleInfo subRule)
        {
            // L'implementazione predefinita esegue la regola più semplice, 
            // ovvero quella dipendente da un singolo campo data
            this.ExecuteDateRule(subRule, false);
        }

        /// <summary>
        /// Esecuzione di una regola che prevede la creazione di appuntamenti sulla base di due campi data
        /// </summary>
        /// <param name="subRule"></param>
        /// <param name="fieldNamePrimary">
        /// Campo data primario che ha la precedenza sul secondario.
        /// Se presente, gli appuntamenti legati al campo hanno la precedenza su quelli legati al secondo campo.
        /// Gli eventuali appuntamenti legati al campo secondario dovranno essere annullati.
        /// </param>
        /// <param name="fieldNameSecondary">
        /// Campo data secondario.
        /// Anche se presente, gli appuntamenti devono essere calcolati se e solo se il campo primario non è valorizzato.
        /// </param>
        protected virtual void ExecuteDateRuleAlternativeFields(SubRuleInfo subRule, string fieldNamePrimary, string fieldNameSecondary)
        {
            _logger.InfoFormat("BEGIN");

            Property pPrimaryProperty = this.FindProperty(fieldNamePrimary);

            if (pPrimaryProperty != null)
            {
                if (pPrimaryProperty.IsEmpty)
                {
                    // Campo primario non valorizzato

                    FieldStateTypesEnum primaryFieldState = this.GetFieldState(fieldNamePrimary, subRule);

                    if (primaryFieldState == FieldStateTypesEnum.Changed ||
                        primaryFieldState == FieldStateTypesEnum.Removed)
                    {
                        Property pSecondaryProperty = this.FindProperty(fieldNameSecondary);

                        if (!pSecondaryProperty.IsEmpty)
                        {
                            // Forza l'invio dell'appuntamento
                            this.ExecuteDateRule(subRule, true);
                        }
                    }
                    else
                    {
                        // Viene eseguita normalmente la regola "Data comunicazione deposito ordinanza"
                        this.ExecuteDateRule(subRule, false);
                    }
                }
                else
                {
                    // Il campo primario risulta valorizzato, 
                    // verifica se l'ultimo appuntamento per la regola è di tipo "CANCELLAZIONE",
                    // in caso non lo sia ancora invia l'appuntamento di cancellazione

                    RuleHistoryInfo lastExecutedRule = this.GetLastExecutedRule(subRule);

                    if (lastExecutedRule != null &&
                        lastExecutedRule.MailMessageSnapshot != null &&
                        lastExecutedRule.MailMessageSnapshot.Appointment != null &&
                        lastExecutedRule.MailMessageSnapshot.Appointment.Status != Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentStatusTypes.CANCELLED)
                    {
                        this.SendCancelAppointment(subRule);
                    }
                }
            }
            else
            {
                // Campo non presente tra i dati del profilo
                throw new SubscriberException(ErrorCodes.MISSING_FIELD_VALUE,
                                                string.Format(ErrorDescriptions.MISSING_FIELD_VALUE, fieldNamePrimary));
            }

            _logger.InfoFormat("END");
        }

        /// <summary>
        /// Invio di un'appuntamento di cancellazione
        /// </summary>
        /// <param name="rule"></param>
        protected void SendCancelAppointment(BaseRuleInfo rule)
        {
            _logger.InfoFormat("BEGIN");

            Subscriber.Dispatcher.CalendarMail.MailRequest mailRequest = null;

            try
            {
                // Reperimento ultima esecuzione della regola
                RuleHistoryInfo lastExecutedRule = this.GetLastExecutedRule(rule);

                if (lastExecutedRule != null && lastExecutedRule.MailMessageSnapshot != null)
                {
                    mailRequest = lastExecutedRule.MailMessageSnapshot;
                    mailRequest.Appointment.Sequence++;
                    mailRequest.Appointment.OrganizerName = this.ListenerRequest.EventInfo.Author.RoleName;
                    mailRequest.Appointment.OrganizerEMail = this.ListenerRequest.ChannelInfo.SmtpMail;
                    mailRequest.Appointment.Method = Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentMethodTypes.CANCEL;
                    mailRequest.Appointment.Status = Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentStatusTypes.CANCELLED;
                    mailRequest.Appointment.DtStamp = DateTime.Now;

                    // Invio messaggio
                    this.DispatchMail(rule, mailRequest);
                }
            }
            catch (Subscriber.SubscriberException ruleEx)
            {
                _logger.Error(ruleEx.Message);

                rule.Error = new ErrorInfo
                {
                    Id = ruleEx.ErrorCode,
                    Message = ruleEx.Message,
                    Stack = ruleEx.StackTrace
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                rule.Error = new ErrorInfo
                {
                    Id = Subscriber.ErrorCodes.UNHANDLED_ERROR,
                    Message = ex.Message,
                    Stack = ex.StackTrace
                };
            }
            finally
            {
                if (rule.Error == null)
                {
                    // Impostare queste informazioni se e solo se l'intero task è andato a buon fine
                    rule.Computed = true;
                }

                rule.ComputeDate = DateTime.Now;

                // Scrittura regola di pubblicazione nell'history
                this.WriteRuleHistory(rule, mailRequest);

                _logger.Info("END");
            }
        }

        /// <summary>
        /// Calcolo Rule applicata a partire da un campo di tipo data del profilo
        /// </summary>
        /// <remarks>
        /// Il nome del campo e le altre informazioni devono essere presenti nei campi opzioni personalizzabili della regola
        /// </remarks>
        /// <param name="rule"></param>
        /// <param name="forceSendAppointment">
        /// Se true, indica di forzare in ogni caso l'invio dell'appuntamento
        /// indipendentemente dal fatto che il profilo risulti modificato o meno rispetto alla precedente versione
        /// </param>
        protected virtual void ExecuteDateRule(BaseRuleInfo rule, bool forceSendAppointment)
        {
            _logger.Info("BEGIN");

            try
            {
                AvvocaturaBaseRuleOptions opts = new AvvocaturaBaseRuleOptions(rule);

                this.ExecuteDateRule(rule, opts, forceSendAppointment);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

                throw ex;
            }
            finally
            {
                _logger.Info("END");
            }
        }

        /// <summary>
        /// Calcolo Rule applicata a partire da un campo di tipo data del profilo
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="opts">
        /// Opzioni della regola dell'avvocatura
        /// </param>
        /// <param name="forceSendAppointment">
        /// Se true, indica di forzare in ogni caso l'invio dell'appuntamento
        /// indipendentemente dal fatto che il profilo risulti modificato o meno rispetto alla precedente versione
        /// </param>
        protected virtual void ExecuteDateRule(BaseRuleInfo rule, AvvocaturaBaseRuleOptions opts, bool forceSendAppointment)
        {
            _logger.Info("BEGIN");

            bool isChangedLegaleIncaricato = false;

            // Reperimento dello stato del campo "Legale incaricato"
            FieldStateTypesEnum legaleIncaricatoFieldState = this.GetFieldState(CommonFields.MAIL_LEGALE_INCARICATO, rule);

            // Indica se è necessario inviare la mail di cancellazione degli appuntamenti
            if (legaleIncaricatoFieldState == FieldStateTypesEnum.Removed ||
                legaleIncaricatoFieldState == FieldStateTypesEnum.Changed)
            {
                isChangedLegaleIncaricato = true;

                // La data del campo del profilo si riferisce ad un evento che ancora non si è verificato.
                // Pertanto è possibile inviare gli appuntamenti di cancellazione al legale attualmente incaricato.

                Property oldProperty = this.FindStoredProperty(opts.Campo, rule);

                if (!oldProperty.IsEmpty)
                {
                    // Invio cancellazione appuntamento se il legale incaricato è stato rimosso o modificato
                    // e solo se il precedente legale aveva ricevuto realmente l'appuntamento

                    // NB: Non è necessario inviare appuntamenti di cancellazione al legale incaricato quando la data 
                    // dell'apputnamento da cancellare si rifescisce ad un evento passato. 
                    // Ciò perché al nuovo legale non devono interessare gli appuntamenti passati del precedente legale.
                    this.SendCancelAppointment(rule);
                }
            }

            Subscriber.Dispatcher.CalendarMail.MailRequest mailRequest = null;

            // Indica se per la regola deve essere memorizzato un elemento nell'history
            bool makeHistory = false;

            try
            {
                if (rule.Enabled)
                {
                    Property dateProperty = this.FindProperty(opts.Campo);

                    if (dateProperty == null)
                    {
                        rule.Error = new ErrorInfo
                        {
                            Id = Subscriber.ErrorCodes.MISSING_FIELD,
                            Message = string.Format(Subscriber.ErrorDescriptions.MISSING_FIELD, opts.Campo)
                        };
                    }
                    else
                    {
                        // Reperimento, dall'history, dei dati relativi all'ultima esecuzione della regola per l'oggetto da pubblicare
                        //RuleHistoryInfo lastExecutedRule = DataAccess.RuleHistoryDataAdapter.GetLastRuleHistory(subRule.Id, this.ListenerRequest.EventInfo.PublishedObject.IdObject);
                        RuleHistoryInfo lastExecutedRule = this.GetLastExecutedRule(rule);

                        Property oldDateProperty = this.FindStoredProperty(opts.Campo, rule);

                        // Determina se deve essere inviato il messaggio di notifica
                        bool sendAppointment = (lastExecutedRule != null && !lastExecutedRule.Published) ||
                                                this.IsDirtyProperty(dateProperty, oldDateProperty) ||
                                                forceSendAppointment;

                        if (!sendAppointment)
                        {
                            // Se è stato inserito o modificato il legale incaricato rispetto alla precedente versione,
                            // forza l'invio dell'appuntamento al nuovo destinatario solo se il campo data risulta valorizzata
                            sendAppointment = !dateProperty.IsEmpty &&
                                    (legaleIncaricatoFieldState == FieldStateTypesEnum.Inserted ||
                                     legaleIncaricatoFieldState == FieldStateTypesEnum.Changed);

                            if (!sendAppointment)
                            {
                                // Verifica se sono stati modificati altri campi dipendenti
                                sendAppointment = !dateProperty.IsEmpty &&
                                                this.IsDirtyDependentFields(rule);
                            }
                        }

                        if (sendAppointment)
                        {
                            makeHistory = true;

                            // Se la data prima udienza è stata rimossa, deve essere inviato 
                            // automaticamente un appuntamento di cancellazione
                            bool sendDelete = (dateProperty.IsEmpty);

                            DateTime date = DateTime.MinValue;

                            if (!sendDelete)
                            {
                                // Verifica se il valore della proprietà monitorata dalla regola
                                // è stata modificata rispetto all'ultima esecuzione 
                                date = this.GetPropertyValueAsDate(dateProperty);

                                // Reperimento della prima data feriale disponibile per la scadenza
                                date = this.ComputeDate(date, rule);

                                if (!sendDelete)
                                {
                                    if (opts.Decorrenza != 0)
                                    {
                                        // Se l'appuntamento è calcolato sulla base di una decorrenza, 
                                        // devono essere azzerate le ore e i minuti in quanto
                                        // l'ora di inizio dell'evento deve essere quello configurato
                                        date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                                    }

                                    if (date.Hour == 0 && date.Minute == 0)
                                    {
                                        // Se la data del profilo non prevede ore e minuti
                                        // l'ora di inizio dell'evento sarà calcolata sulla base delle configurazioni
                                        // previste per la regola

                                        if (opts.OraInizio > 0)
                                        {
                                            // La regola prevede l'ora di inizio
                                            date = date.AddHours(opts.OraInizio);
                                        }
                                        else
                                        {
                                            // La regola non prevede l'ora di inizio, 
                                            // pertanto per default, l'appuntamento inizia alle 9 del mattino
                                            date = date.AddHours(9);
                                        }
                                    }
                                }
                            }

                            mailRequest = this.CreateMailRequest(rule);
                         

                            // Creazione appuntamento iCalendar

                            _logger.Error("Creazione appuntamento iCalendar");

                            Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentInfo appointment = null;

                            if (lastExecutedRule != null && lastExecutedRule.MailMessageSnapshot != null)
                            {
                                appointment = lastExecutedRule.MailMessageSnapshot.Appointment;
                                appointment.Sequence++;
                            }
                            else
                            {
                                appointment = new Dispatcher.CalendarMail.iCalendar.AppointmentInfo();
                                appointment.Sequence = 1;
                                appointment.UID = Subscriber.Dispatcher.CalendarMail.iCalendar.UidGenerator.Create();
                            }

                            appointment.OrganizerName = this.ListenerRequest.EventInfo.Author.RoleName;
                            appointment.OrganizerEMail = this.ListenerRequest.ChannelInfo.SmtpMail;
                            appointment.Method = (sendDelete ? Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentMethodTypes.CANCEL : Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentMethodTypes.REQUEST);
                            appointment.Status = (sendDelete ? Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentStatusTypes.CANCELLED : Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentStatusTypes.CONFIRMED);
                            appointment.DtStamp = DateTime.Now;

                            // NB: In caso di cancellazione dell'appuntamento, la data inizio e fine rimane invariata
                            if (!sendDelete)
                            {
                                appointment.DtStart = date;

                                if (opts.OreDurataEvento > 0)
                                    appointment.DtEnd = date.AddHours(opts.OreDurataEvento);
                                else
                                    appointment.DtEnd = date.AddHours(1); // Per default, l'appuntamento ha una durata di un'ora
                            }

                            // Creazione oggetto della mail
                            mailRequest.Subject = this.CreateMailSubject(rule, appointment.DtStart);
                            mailRequest.Body = string.Empty;
                            appointment.Summary = mailRequest.Subject;

                            appointment.Description = this.CreateMailBody(new Dispatcher.CalendarMail.CalendarMailBodyFormatter(), rule, appointment.DtStart);
                            _logger.Error("CreateMailBody");
                            appointment.Location = this.GetLocation();

                            if (opts.GiorniDecorrenzaAvviso != 0)
                            {

                                _logger.Error("opts.GiorniDecorrenzaAvviso" + opts.GiorniDecorrenzaAvviso);
                                // La regola prevede la notifica dell'avviso di scadenza (reminder)
                                appointment.Alert = new Dispatcher.CalendarMail.iCalendar.AlertInfo();
                                appointment.Alert.TriggerDays = opts.GiorniDecorrenzaAvviso;
                                appointment.Alert.Description = opts.DescrizioneDecorrenzaAvviso;
                            }

                            mailRequest.Appointment = appointment;

                            if (isChangedLegaleIncaricato || this.AppointmentNotScheduled(appointment, rule))
                            {
                                // Invio del messaggio solamente se
                                // - l'appuntamento risulta modificato nella data / ora oppure nei dati visualizzati
                                // - è stato modificato il legale incaricato 
                                this.DispatchMail(rule, mailRequest);
                            }
                            else
                            {
                                // Quest'eccezione non richiede la creazione dell'history
                                makeHistory = false;

                                //// L'appuntamento non risulta modificato rispetto all'ultimo inviato;
                                //// viene segnalata pertanto la "non pubblicazione" della regola.
                                //// NB: Il controllo evita la ripianificazione di appuntamenti quando il 
                                //// campo dipendente del profilo è stato modificato ma i calcoli della decorrenza
                                //// non determinano una modifica all'appuntamento
                                //rule.Error = new ErrorInfo
                                //{
                                //    Id = ErrorCodes.APPOINTMENT_DATE_VALUE_IS_NOT_CHANGED,
                                //    Message = ErrorDescriptions.APPOINTMENT_DATE_VALUE_IS_NOT_CHANGED
                                //};
                            }
                        }
                    }
                }
            }
            catch (Subscriber.SubscriberException ruleEx)
            {
                _logger.Error(ruleEx.Message);

                makeHistory = true;

                rule.Error = new ErrorInfo
                {
                    Id = ruleEx.ErrorCode,
                    Message = ruleEx.Message,
                    Stack = ruleEx.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);

                makeHistory = true;

                rule.Error = new ErrorInfo
                {
                    Id = Subscriber.ErrorCodes.UNHANDLED_ERROR,
                    Message = ex.Message,
                    Stack = ex.ToString()
                };
            }
            finally
            {
                if (rule.Enabled && makeHistory)
                {
                    if (rule.Error == null)
                    {
                        // Impostare queste informazioni se e solo se l'intero task è andato a buon fine
                        rule.Computed = true;
                    }

                    rule.ComputeDate = DateTime.Now;

                    // Scrittura elemento di pubblicazione nell'history
                    this.WriteRuleHistory(rule, mailRequest);
                }

                _logger.Info("END");
            }
        }

        /// <summary>
        /// Esecuzione della regola di calcolo di una scadenza relativa ad un'udienza fissata nel profilo
        /// </summary>
        /// <param name="rule"></param>
        /// <remarks>
        /// Solitamente, un campo DataUdienza è legato ad un altro campo TipologiaUdienza.
        /// </remarks>
        protected virtual void ExecuteUdienzaRule(BaseRuleInfo rule)
        {
            _logger.InfoFormat("BEGIN");

            AvvocaturaBaseRuleOptions opts = new AvvocaturaBaseRuleOptions(rule);

            // Reperimento del nome del campo Tipologia udienza nel profilo dalle opzioni della SubRule
            string campoTipologiaUdienza = rule.GetOptionByName("Campo tipologia udienza", true);

            // Reperimento dalle opzioni della descrizione della tipologia d'udienza
            string tipologiaUdienza = rule.GetOptionByName("Tipologia udienza", true);

            // Reperimento campo "Tipologia udienza" dal profilo
            // NB. E'  necessario per determinare le scadenze per l'udienza di comparizione
            Property pTipologiaUdienza = this.FindProperty(campoTipologiaUdienza);

            // Reperimento del campo "Tipologia udienza" dal profilo storicizzato
            Property pOldtipologiaUdienza = this.FindStoredProperty(campoTipologiaUdienza, rule);

            if (pTipologiaUdienza != null && !pTipologiaUdienza.IsEmpty)
            {
                if (this.AreEquals(pTipologiaUdienza.Value.ToString(), tipologiaUdienza))
                {
                    // La tipologia coincide, la regola viene processata
                    // se e solo se il campo data udienza è valorizzato

                    Property pUdienzaDateProperty = this.FindProperty(opts.Campo);

                    FieldStateTypesEnum stateTipologia = this.GetFieldState(pTipologiaUdienza, pOldtipologiaUdienza);

                    bool forceSendAppointment = (stateTipologia != FieldStateTypesEnum.Unchanged &&
                                                pUdienzaDateProperty != null && !pUdienzaDateProperty.IsEmpty);

                    this.ExecuteDateRule(rule, forceSendAppointment);
                }
                else
                {
                    // Determina se la tipologia dell'udienza nel profilo attuale è stata modificata 
                    // rispetto alla precedente.
                    // In caso affermativo viene inviato un appuntamento di cancellazione se 
                    // e solo se la precedente tipologia di udienza è quella configurata.
                    if (pOldtipologiaUdienza != null && !pOldtipologiaUdienza.IsEmpty)
                    {
                        if (this.AreEquals(pOldtipologiaUdienza.Value.ToString(), tipologiaUdienza))
                        {
                            RuleHistoryInfo lastExecutedRule = this.GetLastExecutedRule(rule);

                            if (lastExecutedRule != null && lastExecutedRule.Published)
                            {
                                if (this.AreEquals(lastExecutedRule.MailMessageSnapshot.Appointment.Method, Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentMethodTypes.REQUEST))
                                {
                                    // Invio dell'appuntamento di cancellazione se e solo se
                                    // il precedente appuntamento inviato non è di tipo Cancellazione.
                                    // In pratica, se l'appuntamento di cancellazione dell'evento è già
                                    // stato inviato, non serve inviarne un altro.
                                    this.SendCancelAppointment(rule);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // La tipologia dell'udienza nel profilo attuale non risulta impostata.
                // Determina se era presente nella versione storicizzata del profilo, 
                // in caso affermativo viene inviato un appuntamento di cancellazione
                if (pOldtipologiaUdienza != null && !pOldtipologiaUdienza.IsEmpty)
                {
                    if (this.AreEquals(pOldtipologiaUdienza.Value.ToString(), tipologiaUdienza))
                    {
                        RuleHistoryInfo lastExecutedRule = this.GetLastExecutedRule(rule);

                        if (lastExecutedRule != null && lastExecutedRule.Published)
                        {
                            if (this.AreEquals(lastExecutedRule.MailMessageSnapshot.Appointment.Method, Subscriber.Dispatcher.CalendarMail.iCalendar.AppointmentMethodTypes.REQUEST))
                            {
                                // Invio dell'appuntamento di cancellazione se e solo se
                                // il precedente appuntamento inviato non è di tipo Cancellazione.
                                // In pratica, se l'appuntamento di cancellazione dell'evento è già
                                // stato inviato, non serve inviarne un altro.
                                this.SendCancelAppointment(rule);
                            }
                        }
                    }
                }
            }

            _logger.InfoFormat("END");
        }

        /// <summary>
        /// Sovrascrittura metodo per l'invio della mail di notifica della regola
        /// </summary>
        /// <param name="rule"></param>
        protected override Subscriber.Dispatcher.CalendarMail.MailRequest CreateMailRequest(BaseRuleInfo rule)
        {
            Property pMailLegaleIncaricato = this.FindProperty(CommonFields.MAIL_LEGALE_INCARICATO);

            _logger.Error("CreateMailRequest a " + pMailLegaleIncaricato.Value.ToString());

            if (pMailLegaleIncaricato != null)
            {
                Subscriber.Dispatcher.CalendarMail.MailRequest request = base.CreateMailRequest(rule);
                request.To = new string[1] { pMailLegaleIncaricato.Value.ToString() };
                return request;
            }
            else
                throw new Subscriber.SubscriberException(ErrorCodes.LEGALE_INCARICATO_MISSING_EMAIL,
                                                    ErrorDescriptions.LEGALE_INCARICATO_MISSING_EMAIL);
        }

        /// <summary>
        /// Creazione dell'oggetto del messaggio di posta
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="computedAppointmentDate">
        /// Data dell'appuntamento calcolato
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// Per impostazione predefinita, restituisce la descrizione della regola
        /// </remarks>
        protected virtual string CreateMailSubject(BaseRuleInfo rule, DateTime computedAppointmentDate)
        {
            return rule.RuleDescription;
        }

        /// <summary>
        /// Creazione del body a partire dai dati di una richiesta
        /// </summary>
        /// <param name="formatter"></param>
        /// <param name="computedAppointmentDate">
        /// Data dell'appuntamento calcolato
        /// </param>
        /// <returns></returns>
        protected virtual string CreateMailBody(Subscriber.Dispatcher.CalendarMail.IMailBodyFormatter formatter, BaseRuleInfo rule, DateTime computedAppointmentDate)
        {
            PublishedObject obj = this.ListenerRequest.EventInfo.PublishedObject;

            formatter.AddData("DESCRIZIONE: ", obj.Description);
            formatter.AddData("TIPOLOGIA: ", obj.TemplateName);

            foreach (Property p in obj.Properties)
            {
                if (!p.Hidden)
                {
                    string value = string.Empty;

                    if (p.Value != null)
                        value = p.Value.ToString();

                    formatter.AddData(string.Format("{0}: ", p.Name.ToUpper()), value);
                }
            }

            return formatter.ToString();
        }

        /// <summary>
        /// Reperimento, dai dati della causa, luogo dell'appuntamento in agenda
        /// </summary>
        /// <remarks>
        /// Per impostazione predefinita, il luogo è calcolato sulla base dei seguenti due campi comuni a tutte le cause:
        /// - Autorità giudiziaria
        /// - Competenza territoriale
        /// </remarks>
        /// <returns></returns>
        protected virtual string GetLocation()
        {
            string location = string.Empty;

            Property p = this.FindProperty(CommonFields.AUTORITA_GIUDIZIARIA);

            if (p != null && !p.IsEmpty)
                location = p.Value.ToString();

            p = this.FindProperty(CommonFields.COMPETENZA_TERRITORIALE);

            if (p != null && !p.IsEmpty)
            {
                if (!string.IsNullOrEmpty(location))
                    location += " " + p.Value.ToString();
                else
                    location = p.Value.ToString();
            }

            return location;
        }

        /// <summary>
        /// Determina se il giorno richiesto è festivo o meno
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        protected virtual bool IsFestivity(DateTime date)
        {
            HolidayChecker.HolidaysChecker checker = new HolidayChecker.HolidaysChecker();

            return checker.IsHoliday(date, HolidayChecker.Easter.EasterAlgorithmEnum.OudinTondering);
        }

        /// <summary>
        /// Indica se, per la causa, è prevista la sospensione feriale dei termini
        /// </summary>
        /// <remarks>
        /// Se la causa prevede tale impostazione, tutte le scadenze dovranno prevedere la sospensione
        /// feriale estiva di 46 giorni dal 1 agosto al 15 settembre
        /// </remarks>
        /// <param name="rule">
        /// La regola attualmente processata fornita come parametro 
        /// permette alle classi derivate di applicare ulteriori verifiche
        /// sulla sospensione estiva
        /// </param>
        protected virtual bool HasSospensioneFerialeTermini(BaseRuleInfo rule)
        {
            return false;
        }

        /// <summary>
        /// Reperimento delle opzioni tipizzate per una regola dell'avvocatura
        /// </summary>
        /// <param name="rule"></param>
        /// <returns></returns>
        protected virtual AvvocaturaBaseRuleOptions GetAvvocaturaOptions(BaseRuleInfo rule)
        {
            return new AvvocaturaBaseRuleOptions(rule);
        }

        /// <summary>
        /// Calcolo della data dell'appuntamento in base ai termini previsti dalla regola
        /// </summary>
        /// <param name="date">Data di base da cui calcolare la scadenza. E' la data inserita nel profilo.</param>
        /// <param name="rule">Regola di pubblicazione</param>
        /// <returns></returns>
        protected virtual DateTime ComputeDate(DateTime date, BaseRuleInfo rule)
        {
            // Creazione oggetto per opzioni tipizzate dell'avvocatura
            // NB. Punto di estensione
            AvvocaturaBaseRuleOptions opts = this.GetAvvocaturaOptions(rule);

            // Mantiene la data iniziale
            DateTime initialDate = date;

            // Mantiene la prima data feriale disponibile a partire dalla data iniziale,
            // senza aver ancora aggiunto la decorrenza
            DateTime computedIntialDate = date;

            if (opts.Decorrenza != 0)
            {
                if (opts.TipoDecorrenza == AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Giorni)
                {
                    // Aggiunta / Rimozione degli eventuali giorni di decorrenza
                    // configurati nelle opzioni di ciascuna sottoregola
                    date = date.AddDays(opts.Decorrenza);
                }
                else if (opts.TipoDecorrenza == AvvocaturaBaseRuleOptions.TipoDecorrenzaEnum.Mesi)
                {
                    // Aggiunta / Rimozione degli eventuali mesi di decorrenza
                    // configurati nelle opzioni di ciascuna sottoregola
                    date = date.AddMonths(opts.Decorrenza);
                }

                if (opts.Decorrenza > 0)
                {
                    if (this.HasSospensioneFerialeTermini(rule))
                    {
                        // Calcolo dei termini per la sospensione feriale dei termini
                        // in quanto la causa prevede tale modalità
                        date = this.ComputeDateSospensioneEstiva(initialDate, date, true);
                    }

                    // DECORRENZA DEI TERMINI IN AVANTI

                    // Se il termine si calcola in avanti (es. +20gg dalla data)
                    // - se domenica o festivo, va al primo giorno feriale successivo
                    // - se di sabato, va al primo giorno feriale antecedente
                    if (date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        date = date.AddDays(1);

                        // Verifica se il lunedi successivo non è festivo
                        while (this.IsFestivity(date))
                            date = date.AddDays(1);
                    }
                    else if (date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        // E' necessario assicurarsi che la decorrenza minima dei termini sia rispettata
                        if (date.AddDays(-1).CompareTo(computedIntialDate) > 0)
                        {
                            date = date.AddDays(-1);

                            // Verifica se il venderdi antecedente non è festivo
                            while (this.IsFestivity(date))
                                date = date.AddDays(-1);
                        }
                        else
                        {
                            // La data calcolata con la decorrenza è minore o uguale alla data del profilo,
                            // pertanto viene incrementata di un giorno piuttosto che diminuita

                            date = date.AddDays(1);

                            // Verifica se il lunedi successivo non è festivo
                            while (this.IsFestivity(date))
                                date = date.AddDays(1);
                        }
                    }
                    else
                    {
                        // Incrementa la data se festivo infrasettimanale
                        while (this.IsFestivity(date))
                            date = date.AddDays(1);
                    }
                }
                else if (opts.Decorrenza < 0)
                {
                    // DECORRENZA DEI TERMINI ALL'INDIETRO

                    if (this.HasSospensioneFerialeTermini(rule))
                    {
                        // Calcolo dei termini per la sospensione feriale dei termini
                        // in quanto la causa prevede tale modalità
                        date = this.ComputeDateSospensioneEstiva(initialDate, date, false);
                    }

                    // Se il termine si calcola all'indietro (es. -20gg dalla data)
                    // - se domenica, festivo o sabato va al primo giorno feriale antecedente (es. -21)

                    while (this.IsFestivity(date))
                        date = date.AddDays(-1);
                }
            }

            return date;
        }

        /// <summary>
        /// Determina le modalità con cui la data fornita rientra o meno nei termini della sospensione feriale
        /// </summary>
        /// <param name="date"></param>
        /// <returns>
        /// 0 = Data in sospensione feriale
        /// 1 = Data successiva alla sospensione feriale
        /// -1 = Data inferiore alla sospensione feriale
        /// </returns>
        protected int IsDateInSospensioneFeriale(DateTime date)
        {
            int result = 0;

            if (date.Month == 8 || (date.Month == 9 && date.Day <= 15))
            {
                // Agosto o primi 15 giorni di settembre
                result = 0;
            }
            else if (date.Month > 9 || (date.Month == 9 && date.Day > 15))
            {
                // Successiva al 15 settembre
                result = 1;
            }
            else if (date.Month < 8)
            {
                // Inferiore ad agosto
                result = -1;
            }

            return result;
        }

        /// <summary>
        /// Calcolo della data dell'appuntamento in funzione della sospensione feriale dei termini
        /// </summary>
        /// <param name="initDate"></param>
        /// <param name="endDate"></param>
        /// <param name="positiveIncrement"></param>
        /// <returns></returns>
        protected virtual DateTime ComputeDateSospensioneEstiva(DateTime initDate, DateTime endDate, bool positiveIncrement)
        {
            int days = 0;

            const int DAYS_SOSPENSIONE = 30;

            DateTime computedDate = endDate;

            // Determina se l'anno delle due date è lo stesso
            bool areSameYears = (initDate.Year == endDate.Year);

            // "Posizione" delle date rispetto alla sospensione feriale dei termini
            int initDateResult = this.IsDateInSospensioneFeriale(initDate);
            int endDateResult = this.IsDateInSospensioneFeriale(endDate);

            // Numero da moltiplicare ai giorni della sospensione 
            int offset = 0;

            if (positiveIncrement)
            {
                offset = (endDate.Year - initDate.Year);

                if (offset == 0)
                {
                    if (initDateResult == -1 && endDateResult == 1 ||
                        initDateResult == 0 ||
                        endDateResult == 0)
                    {
                        // Se le date sono dello stesso anno, l'offset va incrementato 
                        // solo se la data iniziale è precedente e la finale è successiva alla sospensione feriale
                        // oppure se almeno una delle due rientra nei termini di sospensione
                        offset = 1;
                    }
                }
                else
                {
                    // Se la differenza è un anno o superiore, l'offset va decrementato di 1
                    // solo se la iniziale supera la sospensione feriale e 
                    // la data finale è precedente alla sospensione feriale
                    // (data a cavallo tra i due anni ma non comprese nella sospensione)
                    if (initDateResult == 1 && endDateResult == -1)
                        offset--;
                }
            }
            else
            {
                offset = (initDate.Year - endDate.Year);

                if (offset == 0)
                {
                    if (initDateResult == 1 && endDateResult == -1 ||
                        initDateResult == 0 ||
                        endDateResult == 0)
                    {
                        // Se le date sono dello stesso anno, l'offset va incrementato 
                        // solo se la data iniziale è successiva e la finale è precedente alla sospensione feriale
                        // oppure se almeno una delle due rientra nei termini di sospensione
                        offset = 1;
                    }
                }
                else
                {
                    // Se la differenza è un anno o inferiore

                    if (initDateResult == -1 && endDateResult == 1)
                    {
                        // l'offset va decrementato di 1 solo se la iniziale è precedente alla sospensione feriale e 
                        // la data finale è successiva alla sospensione feriale
                        // (data a cavallo tra i due anni ma non comprese nella sospensione)
                        offset--;
                    }
                    else if (initDateResult == 1 && endDateResult == -1 ||
                            initDateResult == 0 ||
                            endDateResult == 0)
                    {
                        // l'offset va incrementato di 1 solo se la data iniziale supera la sospensione feriale
                        // e la data finale è precedente
                        offset++;
                    }
                }
            }

            if (positiveIncrement)
                days = (offset * DAYS_SOSPENSIONE);
            else
                days = (offset * -DAYS_SOSPENSIONE);


            if (days != 0)
            {
                computedDate = computedDate.AddDays(days);

                if (this.IsDateInSospensioneFeriale(computedDate) == 0)
                {
                    // Se la data rientra ulteriormente nella sospensione feriale dei termini,
                    // applica nuovamente la regola
                    if (positiveIncrement)
                        computedDate = computedDate.AddDays(DAYS_SOSPENSIONE);
                    else
                        computedDate = computedDate.AddDays(-DAYS_SOSPENSIONE);
                }
            }

            return computedDate;
        }

        /// <summary>
        /// Scrittura esito dell'esecuzione della regola
        /// </summary>
        /// <param name="rule"></param>
        protected virtual void WriteRuleHistory(BaseRuleInfo rule)
        {
            this.WriteRuleHistory(rule, null);
        }

        /// <summary>
        /// Scrittura esito dell'esecuzione della regola
        /// </summary>
        /// <param name="rule">Regola da memorizzare</param>
        /// <param name="mailRequest">Dati della mail inviata</param>
        protected virtual void WriteRuleHistory(BaseRuleInfo rule, Subscriber.Dispatcher.CalendarMail.MailRequest mailRequest)
        {
            // Scrittura elemento di pubblicazione nell'history
            RuleHistoryInfo historyInfo = RuleHistoryInfo.CreateInstance(rule);
            historyInfo.Author = this.ListenerRequest.EventInfo.Author;
            historyInfo.ObjectSnapshot = this.ListenerRequest.EventInfo.PublishedObject;
            historyInfo.MailMessageSnapshot = mailRequest;
            historyInfo = DataAccess.RuleHistoryDataAdapter.SaveHistoryItem(historyInfo);
        }

        /// <summary>
        /// Verifica se il campo data della pratica si riferisce ad una data successiva a quella attuale
        /// </summary>
        /// <param name="dateFieldName">Campo data della pratica</param>
        /// <returns></returns>
        protected bool IsPastDate(string dateFieldName)
        {
            DateTime profileDate = this.GetPropertyValueAsDate(dateFieldName);

            return this.IsPastDate(profileDate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        protected bool IsPastDate(DateTime dateValue)
        {
            DateTime eventDate = new DateTime(dateValue.Year, dateValue.Month, dateValue.Day);
            DateTime actualDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            return (DateTime.Compare(eventDate, actualDate) < 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="mailRequest"></param>
        protected override void DispatchMail(BaseRuleInfo rule, Dispatcher.CalendarMail.MailRequest mailRequest)
        {
            if (this.IsPastDate(mailRequest.Appointment.DtStart))
            {
                // Se la data dell'appuntamento avviene nel passato, nono deve essere inviato alcun appuntamento
                rule.Error = new ErrorInfo
                {
                    Id = ErrorCodes.APPOINTMENT_DATE_VALUE_IS_PAST_DATE,
                    Message = string.Format(ErrorDescriptions.APPOINTMENT_DATE_VALUE_IS_PAST_DATE, mailRequest.Appointment.DtStart.ToString(Subscriber.Properties.Resources.DateTimeFormat))
                };
            }
            else
            {
                // Invio dell'appuntamento
                base.DispatchMail(rule, mailRequest);
            }
        }
    }
}