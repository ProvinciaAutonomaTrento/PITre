using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;

namespace Publisher.VtDocs
{
    /// <summary>
    /// Rappresenta l'istanza di pubblicazione di oggetti del sistema documentale
    /// </summary>
    public class InstancePublisher : IInstancePublisher
    {
        private static ILog logger = LogManager.GetLogger(typeof(InstancePublisher));

        #region Public Members

        /// <summary>
        /// Processo di pubblicazione dei contenuti del canale di pubbliciazione
        /// </summary>
        /// <param name="instance"></param>
        public void Publish(ChannelRefInfo instance)
        {
            DateTime actualData = DateTime.Now;
            logger.Debug("START");
            logger.Debug("IDADM: "+instance.Admin.Id);
            logger.Debug("NOMECANALE: " + instance.ChannelName);
            //logger.Debug("end ex date: " + instance.EndExecutionDate.ToString());
            //logger.Debug("conto esecuzioni: " + instance.ExecutionCount.ToString());
            logger.Debug("Id istanza: " + instance.Id.ToString());
            //logger.Debug("Ultima esecuzione: "+instance.LastExecutionDate.ToString());
            logger.Debug("LastLogId: " + instance.LastLogId.ToString());
            logger.Debug("MAchine Name: " + instance.MachineName);
            logger.Debug("Publisher Url: " + instance.PublisherServiceUrl);
            int lastLogId = instance.LastLogId;
            try
            {
                if (instance.Events != null && instance.Events.Length > 0)
                {
                    // Analisi dei singoli eventi
                    foreach (EventInfo ev in instance.Events)
                    {
                        //logger.Debug("presenti degli eventi legati all'istanza");
                        VtDocs.LogCriteria criteria = new VtDocs.LogCriteria
                        {
                            Admin = instance.Admin,
                            EventName = ev.EventName,
                            FromLogId = instance.LastLogId,
                            ObjectType = ev.ObjectType,
                            ObjectTemplateName = ev.ObjectTemplateName
                        };
                        logger.DebugFormat("Dati evento: Admin {0}, EventName={1}, fromLogId= {2}, objectType={3}, objTemplName= {4}",instance.Admin,ev.EventName,instance.LastLogId,ev.ObjectType,ev.ObjectTemplateName);
                        IDataMapper mapper = null;

                        // Ricerca delle attività effettuate per l'evento
                        foreach (VtDocs.LogInfo logInfo in VtDocs.LogDataAdapter.GetLogs(criteria))
                        {
                            //logger.Debug("Trovato un logInfo dalla store procedure");
                            logger.DebugFormat("{0}, Id: {1}, OD: {2}", logInfo.EventDescription, logInfo.Id, logInfo.ObjectDescription);
                            if (mapper == null)
                                mapper = new VtDocs.DataMapper();
                            //logger.Debug("Prima del mappamento");
                            // Mapping dell'oggetto documentale nel formato per la pubblicazione
                            Subscriber.Proxy.PublishedObject obj = mapper.Map(logInfo, ev);
                            logger.Debug("Post mappamento. Obj è null? "+ (obj==null).ToString());
                            if (obj != null)
                            {
                                //logger.Debug("obj non è null");
                                int countComputed = 0;
                                if (!string.IsNullOrEmpty(instance.ChannelName) && instance.ChannelName.ToUpper() == "ALBO_TELEMATICO")
                                {
                                    logger.Debug("Gestione albo telematico");
                                    logger.Debug(logInfo.ObjectDescription.ToUpper());
                                    if ((logInfo.ObjectDescription.ToUpper().Contains("DA PUBBLICARE") ||
                                        logInfo.ObjectDescription.ToUpper().Contains("DA ANNULLARE") ||
                                        logInfo.ObjectDescription.ToUpper().Contains("DA REVOCARE")) &&
                                        !logInfo.ObjectDescription.ToUpper().Contains("ERRORE"))
                                    {
                                        logger.Debug("E' in uno stato che genera notifica ad albo telematico. Chiamo il notifyEvent.");
                                        Subscriber.Proxy.ListenerResponse response = this.NotifyEvent(instance, logInfo, obj);
                                        countComputed = response.RuleResponseList.Count(e => e.Rule.Computed);
                                    }
                                }
                                else
                                {
                                    // Notifica evento al SubScriber
                                    logger.Debug("Entro nel notify event");
                                    Subscriber.Proxy.ListenerResponse response = this.NotifyEvent(instance, logInfo, obj);
                                    countComputed = response.RuleResponseList.Count(e => e.Rule.Computed);
                                }
                                // Reperimento del numero delle regole di pubblicazione processate con esito positivo
                                //int countComputed = response.RuleResponseList.Count(e => e.Rule.Computed);

                                if (countComputed > 0)
                                {
                                    logger.Debug("Pubblicazione effettuata");
                                    // Incremento del numero di oggetti pubblicati 
                                    instance.PublishedObjects++;
                                    instance.TotalPublishedObjects++;
                                }
                                else
                                {
                                    logger.Debug("Pubblicazione non effettuata");
                                }
                            }

                            // Viene memorizzato l'id e la data dell'ultimo log analizzato
                            //instance.LastLogId = logInfo.Id;
                            if (logInfo.Id > lastLogId)
                            {
                                lastLogId = logInfo.Id;
                            }
                            instance.StartLogDate = logInfo.Data;
                        }
                    }
                }
            }
            catch (PublisherException pubEx)
            {
                logger.Debug(pubEx.Message);
                SaveError(instance, pubEx);
                throw pubEx;
            }
            catch (Exception ex)
            {
                logger.Debug(ex.Message);
                PublisherException pubEx = new PublisherException(ErrorCodes.UNHANDLED_ERROR, ex.Message);
                SaveError(instance, pubEx);
                throw pubEx;
            }
            finally
            {
                // Viene incrementato il numero delle esecuzioni delle pubblicazioni
                instance.ExecutionCount++;
                instance.TotalExecutionCount++;

                // Impostazione della data dell'ultima esecuzione della regola
                instance.LastExecutionDate = actualData;

                // Aggiornamento del last log id avviene alla fine, non durante l'analisi degli eventi.
                instance.LastLogId = lastLogId;

                // Aggiornamento dati istanza di pubblicazione
                instance = DataAccess.PublisherDataAdapter.UpdateExecutionState(instance);
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="exception"></param>
        protected virtual void SaveError(ChannelRefInfo instance, PublisherException exception)
        {
            ErrorInfo error = new ErrorInfo
            {
                IdInstance = instance.Id,
                ErrorCode = exception.ErrorCode,
                ErrorDescription = exception.Message,
                ErrorStack = exception.ToString(),
                ErrorDate = DateTime.Now                
            };

            error = DataAccess.PublisherDataAdapter.SaveError(error);
        }

        /// <summary>
        /// Notifica evento al SubScriber
        /// </summary>
        /// <param name="channelRef"></param>
        /// <param name="logInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual Subscriber.Proxy.ListenerResponse NotifyEvent(ChannelRefInfo channelRef, VtDocs.LogInfo logInfo, Subscriber.Proxy.PublishedObject obj)
        {
            logger.Debug("Start. "+channelRef.ChannelName);
            Subscriber.Proxy.EventInfo eventInfo = new Subscriber.Proxy.EventInfo
            {
                EventName = logInfo.EventCode,
                EventDate = DateTime.Now,
                Author = new Subscriber.Proxy.EventAuthorInfo
                {
                    Name = logInfo.UserName,
                    RoleName = logInfo.RoleDescription
                },
                PublishedObject = obj
            };
            logger.DebugFormat("Eventname: {0}, Eventdate: {1}, Author user: {2}, Role {3}", eventInfo.EventName, eventInfo.EventDate.ToString(), eventInfo.Author.Name, eventInfo.Author.RoleName);
            Subscriber.Proxy.ListenerResponse response = null;

            Subscriber.Proxy.ChannelInfo channelInfo = null;

            using (Subscriber.Proxy.SubscriberWebService subscriber = new Subscriber.Proxy.SubscriberWebService())
            {
                subscriber.Url = channelRef.SubscriberServiceUrl;
                logger.DebugFormat("Sub Url: {0}. Prelievo canali",subscriber.Url);
                Subscriber.Proxy.ChannelInfo[] channels = subscriber.GetChannelList();

                if (channels != null)
                {
                    logger.Debug("Channel trovati. N " + channels.Length + "");
                    for (int i = 0; i < channels.Length; i++)
                    {
                        logger.Debug(channels[i].Name);
                    }
                    channelInfo = channels.Where(e => e.Name == channelRef.ChannelName).First();

                    if (channelInfo == null)
                    {
                        // Istanza di pubblicazione non trovata nel subscriber collegato
                        logger.Debug("Istanza di pubblicazione non trovata nel subscriber collegato");
                        throw new PublisherException(ErrorCodes.PUBLISH_CHANNEL_NOT_FOUND, ErrorDescriptions.PUBLISH_CHANNEL_NOT_FOUND);
                    }
                }
                logger.Debug("Chiamo il notify Event");
                response = subscriber.NotifyEvent(
                        new Subscriber.Proxy.ListenerRequest
                        {
                            ChannelInfo = channelInfo,
                            EventInfo = eventInfo
                        }
                    );
            }

            return response;
        }

        #endregion
    }
}
