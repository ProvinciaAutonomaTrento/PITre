using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.NotificationCenter;
using DocsPaDB.Query;
using log4net;

namespace ServiceNotifications.CoreNotification
{
    class EventProcessingEngine
    {

        private static ILog logger = LogManager.GetLogger(typeof(EventProcessingEngine));
        #region private field

        private List<Event> _listEvent;
        private DataLayerEvents _dataLayersEvents = new DataLayerEvents();
        private DataLayersRecipients _dataLayersRecipients = new DataLayersRecipients();
        private DataLayerNotification _dataLayerNotification = new DataLayerNotification();
        private DataLayerAssertions _dataLayerAssertions = new DataLayerAssertions();
        private DataLayerFollowDomainObject _dataLayerFollowDomainObject = new DataLayerFollowDomainObject();
        #endregion 

        #region public property

        #endregion

        #region public methods

        /// <summary>
        /// Popola la lista degli eventi da processare
        /// </summary>
        /// <returns></returns>
        public void ListToBeProcessed()
        {
            logger.Debug("Start");
            this._listEvent = _dataLayersEvents.SelectEventsToBeProcessed();
        }

        /// <summary>
        /// He commits the transaction started with the select the event queue
        /// </summary>
        /// <returns></returns>
        public void UnlockEventQueue()
        {
            logger.Debug("Start");
            this._dataLayersEvents.UnlockEventQueue(_listEvent);
        }

        /// <summary>
        /// Build the recipients of events
        /// </summary>
        public void BuildRecipientsOfEvents()
        {
            logger.Debug("Start");
            for (int i = 0; i < _listEvent.Count; i++)
            {
                //se l'evento è stato tracciato prima della transazione da amministrazione da OBB --> NN o da CONF --> NN per il tipo
                //evento in questione allora lo scarto.
                if(!_listEvent[i].CONFIGURATION_EVENT_TYPE.Equals(SupportStructures.ConfigurationEventType.NN))
                    _listEvent[i].ACTORS.RECIPIENTS = _dataLayersRecipients.BuildRecipients(_listEvent[i]);
            }
        }

        /// <summary>
        /// Creates the list of notifications associated with the event
        /// </summary>
        public void CreatesEventNotifications()
        {
            logger.Debug("Start");
            for (int i = 0; i < _listEvent.Count; i++)
            {
                //se l'evento è stato tracciato prima della transazione da amministrazione da OBB --> NN o da CONF --> NN per il tipo
                //evento in questione allora lo scarto.
                if(!_listEvent[i].CONFIGURATION_EVENT_TYPE.Equals(SupportStructures.ConfigurationEventType.NN))
                {
                    List<Notification> listNotifications = new List<Notification>();
                    Items items = _dataLayerNotification.CreateItems(_listEvent[i]);
                    items.ITEM_SPECIALIZED = _dataLayerNotification.CreateSpecializedItem(_listEvent[i]);

                    if (_listEvent[i].ACTORS != null && _listEvent[i].ACTORS.RECIPIENTS != null && _listEvent[i].ACTORS.RECIPIENTS.Count > 0)
                    {
                        foreach (Recipient recipient in _listEvent[i].ACTORS.RECIPIENTS)
                        {
                            Notification notification = new Notification();
                            notification.ITEMS = items;
                            notification.DTA_NOTIFY = Convert.ToDateTime(System.DateTime.Now, new System.Globalization.CultureInfo("it-IT"));
                            notification.RECIPIENT = recipient;
                            notification.TYPE_NOTIFY = recipient.OPERATIONAL_OR_INFORMATION;
                            listNotifications.Add(notification);
                        }
                        _listEvent[i].NOTIFICATIONS = listNotifications; 
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void WriteEventsNotifications()
        {
            logger.Debug("Start");
            for (int i = 0; i < _listEvent.Count; i++)
            {
                //writes on db notifications generated for the event
                //Notified the field is true if the write is successful, false otherwise
                if (!_listEvent[i].CONFIGURATION_EVENT_TYPE.Equals(SupportStructures.ConfigurationEventType.NN)) //se l'evento è stato tracciato prima della transazione da amministrazione da OBB --> NN o da CONF --> NN per il tipo
                                                                                                                 //evento in questione allora lo scarto.
                {
                    _listEvent[i].NOTIFIED = _dataLayerNotification.WriteEventNotifications(_listEvent[i]);
                }
                else
                {
                    _listEvent[i].NOTIFIED = true;
                }
            }
            
        }

        /// <summary>
        /// Filters the recipients identified through assertions defined
        /// </summary>
        public void FilterByAssertions()
        {
            logger.Debug("Start");
            try
            {
                //ciclo su ogni evento
                for (int i = 0; i < _listEvent.Count; i++)
                {
                    if (_listEvent[i].CONFIGURATION_EVENT_TYPE.Equals(SupportStructures.ConfigurationEventType.CONF))
                    {
                        //lista dei ruoli oggetto della notifica
                        List<string> listRole = ListRolesNotificationRecipients(_listEvent[i].ACTORS.RECIPIENTS);
                        //applico le asserzioni per ogni ruolo
                        foreach(string role in listRole)
                        {
                            //preparo la lista degli aggregatori del ruolo
                            List<Aggregator> listAggregatorRole = GetListAggregatorRole(role);
                            List<Aggregator> listAggregatorTypeEvent = GetListAggregatortypeEvent(_listEvent[i].ID_TYPE_EVENT, _listEvent[i].ID_ADM.ToString());
                            for (int y = 0; y < listAggregatorRole.Count; y++)
                            {
                                for (int x = 0; x < listAggregatorTypeEvent.Count; x++)
                                {
                                    if (listAggregatorRole[y].IDAUR.Equals(listAggregatorTypeEvent[x].IDAUR) &&
                                        listAggregatorRole[y].TYPEAUR.Equals(listAggregatorTypeEvent[x].TYPEAUR))
                                    {
                                        //è stata trovata un'asserzione con un aggregato valido per il ruolo processato
                                        listAggregatorRole[y].TYPENOTIFY = listAggregatorTypeEvent[x].TYPENOTIFY;
                                        break;
                                    }

                                }
                            }

                            int countAssertionOperative =(from ag in listAggregatorRole where (!string.IsNullOrEmpty(ag.TYPENOTIFY)) && ag.TYPENOTIFY.Equals("O") select ag).Count();
                            int countAssertionInformation =(from ag in listAggregatorRole where (!string.IsNullOrEmpty(ag.TYPENOTIFY)) && ag.TYPENOTIFY.Equals("I") select ag).Count();
                            if (countAssertionOperative > 0) //è stata definita da almeno un aggregato del ruolo un asserzione nella quale si dice che il
                                //che le notifiche per questo tipo evento devono giungere al ruolo come operative
                            {
                                for (int x = 0; x < _listEvent[i].ACTORS.RECIPIENTS.Count; x++)
                                {
                                    if (_listEvent[i].ACTORS.RECIPIENTS[x].ID_ROLE.ToString().Equals(role))
                                    {
                                        _listEvent[i].ACTORS.RECIPIENTS[x].OPERATIONAL_OR_INFORMATION = SupportStructures.NotificationType.OPERATIONAL;
                                    }
                                }
                            }
                            else if (countAssertionInformation > 0)//sono state definite per il ruolo solo asserzioni che dicono che le notifiche per questo tipo
                                //evento devono giungere al ruolo come informative
                            {
                                for (int x = 0; x < _listEvent[i].ACTORS.RECIPIENTS.Count; x++)
                                {
                                    if (_listEvent[i].ACTORS.RECIPIENTS[x].ID_ROLE.ToString().Equals(role))
                                    {
                                        _listEvent[i].ACTORS.RECIPIENTS[x].OPERATIONAL_OR_INFORMATION = SupportStructures.NotificationType.INFORMATION;
                                    }
                                }
                            }
                            else // non sono state definite asserzione dagli aggregati del ruolo per questo tipo evento
                            {
                                _listEvent[i].ACTORS.RECIPIENTS.RemoveAll(r => r.ID_ROLE.ToString().Equals(role)); 
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                // traccia l'eccezione nel file di log
            }
        }

        /// <summary>
        /// Apply the filter to the events that trigger notification to potential recipients
        /// if the latter follow the document.
        /// </summary>
        public void FilterByFollowDomainObject()
        {
            logger.Debug("Start");
            //loop of each event
            for (int i = 0; i < _listEvent.Count; i++)
            {
                // if the event should be notified only in the event of a domain object below, then
                // then among potential RECIPIENTS consider only those who have decided to follow the object Domain.
                if (_listEvent[i].FOLLOW_DOMAIN_OBJECT)
                {
                    List<RecipientFollowDomainObject> recipientsFollowobject = GetRecipientsFollowDomainObject(_listEvent[i].ID_OBJECT.ToString(), 
                        _listEvent[i].DOMAIN_OBJECT, _listEvent[i].ID_ADM.ToString());
                    // if it is FOLLOW_DOC_EXT_APP event or FOLLOW_FASC_EXT_APP then I have to consider 
                    // the potential recipients, only those registered by external application in case
                    // otherwise only those recorded by PITRE
                    if (_listEvent[i].TYPE_EVENT.Equals(SupportStructures.EventType.FOLLOW_DOC_EXT_APP) ||
                        _listEvent[i].TYPE_EVENT.Equals(SupportStructures.EventType.FOLLOW_FASC_EXT_APP))
                    {
                        recipientsFollowobject = (from r in recipientsFollowobject
                                                  where
                                                      r.APPLICATION.Equals(RecipientFollowDomainObject.EXTERNAL)
                                                  select r).ToList();
                    }
                    else
                    {
                        recipientsFollowobject = (from r in recipientsFollowobject
                                                  where
                                                      r.APPLICATION.Equals(RecipientFollowDomainObject.INTERNAL)
                                                  select r).ToList();
                    }

                    if (_listEvent[i].ACTORS != null && _listEvent[i].ACTORS.RECIPIENTS != null && 
                        _listEvent[i].ACTORS.RECIPIENTS.Count > 0)
                    { 
                        // if there are potential recipients of the notification that follow the object domain
                        // question then empty the list of recipients.
                        if (recipientsFollowobject == null || recipientsFollowobject.Count < 1)
                        {
                            _listEvent[i].ACTORS.RECIPIENTS.Clear();
                        }
                        else
                        {
                            // I create a list of support and people with recipients
                            List<Recipient> tmpListRecipients = new List<Recipient>();
                            tmpListRecipients.InsertRange(0, _listEvent[i].ACTORS.RECIPIENTS);
                            //for each recipient in the list of support
                            foreach (Recipient rec in tmpListRecipients)
                            {
                                // if the potential recipient is not among those rec (recipientsFollowobject)
                                // the following domain object
                                if ((from r in recipientsFollowobject
                                     where r.IDROLE == rec.ID_ROLE && r.IDUSER == rec.ID_USER
                                     select r).Count() < 1)
                                {
                                    //delete it from the list of recipients
                                    _listEvent[i].ACTORS.RECIPIENTS.RemoveAll(recipient => recipient.ID_ROLE == rec.ID_ROLE &&
                                            recipient.ID_USER == rec.ID_USER);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Assertions

        /// <summary>
        /// Returns a list of roles to which the notice
        /// </summary>
        private List<string> ListRolesNotificationRecipients(List<Recipient> listRecipient)
        {
            try
            {
                List<string> listRole = new List<string>();
                return (from recipient in listRecipient group recipient by recipient.ID_ROLE into groupUserInRole select groupUserInRole.Key.ToString()).ToList();
            }
            catch (Exception exc)
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Returns a list of all the aggregates for the role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private List<Aggregator> GetListAggregatorRole(string role)
        {
            return _dataLayerAssertions.ListAggregatorRole(role);
        }

        private List<Aggregator> GetListAggregatortypeEvent(string idtypeEvent, string idAmm)
        {
            return _dataLayerAssertions.ListAggregatorTypeEvent(idtypeEvent, idAmm);
        }

        #endregion

        #region Follow Domain object

        private List<RecipientFollowDomainObject> GetRecipientsFollowDomainObject(string idObject, string domainObject, string idAmm)
        {
            return _dataLayerFollowDomainObject.GetRecipientsFollowDomainObject(idObject, domainObject, idAmm);
        }

        #endregion
    }
}