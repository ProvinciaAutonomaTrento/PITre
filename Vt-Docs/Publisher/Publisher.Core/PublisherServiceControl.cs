using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.AccessControl;
using log4net;

namespace Publisher
{
    /// <summary>
    /// ServiceControlPanel del processo del Publisher
    /// </summary>
    public sealed class PublisherServiceControl
    {
        private static ILog logger = LogManager.GetLogger(typeof(PublisherServiceControl));
        /// <summary>
        /// Oggetto che gestisce in memoria lo stato di esecuzione di un canale di pubblicazione
        /// </summary>
        private class StartedChannelTimer
        {
            /// <summary>
            /// Oggetto timer
            /// </summary>
            public Schedule.ScheduleTimer Timer
            {
                get;
                set;
            }

            /// <summary>
            /// Data di avvio del timer
            /// </summary>
            public DateTime StartExecutionDate
            {
                get;
                set;
            }

            /// <summary>
            /// Nome del computer in cui il servizio è attivo
            /// </summary>
            public string MachineName
            {
                get;
                set;
            }

            /// <summary>
            /// Url del servizio di pubblicazione
            /// </summary>
            public string PublisherServiceUrl
            {
                get;
                set;
            }
        }

        #region Public Members

        /// <summary>
        /// Riavvia l'esecuzione di un canale di pubblicazione a partire dall'identificativo univoco
        /// </summary>
        /// <param name="channelId"></param>
        public static void ReStartChannel(int channelId)
        {
            logger.DebugFormat("Restart channel {0}", channelId);
            ReStartChannel(DataAccess.PublisherDataAdapter.GetChannel(channelId));
        }

        /// <summary>
        /// Riavvia l'esecuzione di un canale di pubblicazione
        /// </summary>
        /// <param name="channelRef">Dati del canale di pubblicazione</param>
        /// <remarks>
        /// Il canale sarà riavviato sullo stesso server
        /// </remarks>
        public static void ReStartChannel(ChannelRefInfo channelRef)
        {
            // Verifica se il canale di pubblicazione risulta avviato su un server remoto
            if (!IsChannelStartedOnRemoteMachine(channelRef))
            {
                try
                {
                    // Fermo del canale di pubblicazione
                    StopChannel(channelRef);
                }
                catch (PublisherException pubEx)
                {
                    logger.Error(pubEx.Message);
                    if (pubEx.ErrorCode != ErrorCodes.OPERATION_NOT_ALLOWED_PER_SERVICE_STOPPED)
                        throw pubEx;
                }

                // Il canale è avviato su un computer locale
                try
                {
                    // Avvio locale del canale
                    StartChannel(channelRef);
                }
                catch (PublisherException pubEx)
                {
                    logger.Error(pubEx.Message);
                   
                    if (pubEx.ErrorCode != ErrorCodes.OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED)
                        throw pubEx;
                }
            }
            else
            {
                // Il canale è avviato su un computer remoto, non fa nulla
                logger.Info("Canale su una macchina remota");
            }
        }

        ///// <summary>
        ///// Riavvia l'esecuzione di un canale di pubblicazione
        ///// </summary>
        ///// <param name="channelRef">Dati del canale di pubblicazione</param>
        ///// <remarks>
        ///// Il canale sarà riavviato sullo stesso server
        ///// </remarks>
        //public static void ReStartChannel(ChannelRefInfo channelRef)
        //{
        //    // Verifica se il canale di pubblicazione risulta avviato su un server remoto
        //    bool wasStartedOnRemoteMachine = IsChannelStartedOnRemoteMachine(channelRef);

        //    string publisherServiceUrl = channelRef.PublisherServiceUrl;

        //    try
        //    {
        //        // Fermo del canale di pubblicazione
        //        StopChannel(channelRef);
        //    }
        //    catch (PublisherException pubEx)
        //    {
        //        if (pubEx.ErrorCode != ErrorCodes.OPERATION_NOT_ALLOWED_PER_SERVICE_STOPPED)
        //            throw pubEx;
        //    }

        //    if (wasStartedOnRemoteMachine)
        //    {
        //        // Il canale è avviato su un computer remoto
        //        try
        //        {
        //            // Riavvio del canale di pubblicazione dal server remoto
        //            StartRemoteChannel(channelRef, publisherServiceUrl);
        //        }
        //        catch (PublisherException innerPubEx)
        //        {
        //            if (innerPubEx.ErrorCode == ErrorCodes.START_CHANNEL_ON_REMOTE_SERVER_ERROR)
        //            {
        //                // In caso di errore nell'avvio da remoto (es. server non disponibile), 
        //                // il canale sarà riavviato dal server corrente
        //                StartChannel(channelRef);
        //            }
        //            else
        //            {
        //                throw innerPubEx;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // Il canale è avviato su un computer locale
        //        try
        //        {
        //            // Avvio locale del canale
        //            StartChannel(channelRef);
        //        }
        //        catch (PublisherException pubEx)
        //        {
        //            if (pubEx.ErrorCode != ErrorCodes.OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED)
        //                throw pubEx;
        //        }
        //    }

        //}

        /// <summary>
        /// Avvia l'esecuzione di un canale di pubblicazione
        /// </summary>
        /// <param name="channelId"></param>
        public static void StartChannel(int channelId)
        {
            logger.DebugFormat("Start channel {0}", channelId);
            StartChannel(DataAccess.PublisherDataAdapter.GetChannel(channelId));
        }

        /// <summary>
        /// Avvia l'esecuzione di un canale di pubblicazione
        /// </summary>
        /// <param name="channelRef"></param>
        public static void StartChannel(ChannelRefInfo channelRef)
        {
            // Aggiornamento stato del canale
            RefreshChannelState(channelRef);

            if (channelRef.State == ChannelStateEnum.Stopped)
            {
                using (DocsPaDB.TransactionContext tx = new DocsPaDB.TransactionContext())
                {
                    // Impostazione dello stato di avvio del servizio
                    channelRef = DataAccess.PublisherDataAdapter.StartService(channelRef);

                    // Avvio timer
                    StartTimer(channelRef);

                    tx.Complete();
                }
            }
            else
            {
                // L'istanza risulta già avviata
                throw new PublisherException(ErrorCodes.OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED, ErrorDescriptions.OPERATION_NOT_ALLOWED_PER_SERVICE_STOPPED);
            }
        }

        /// <summary>
        /// Sospende l'esecuzione di un canale di pubblicazione a partire dall'identificativo univoco
        /// </summary>
        /// <param name="channelId"></param>
        public static void StopChannel(int channelId)
        {
            logger.DebugFormat("Stop channel {0}", channelId);
            StopChannel(DataAccess.PublisherDataAdapter.GetChannel(channelId));
        }

        /// <summary>
        /// Sospende l'esecuzione di un canale di pubblicazione
        /// </summary>
        /// <param name="channelRef"></param>
        public static void StopChannel(ChannelRefInfo channelRef)
        {
            //DocsPaUtils.LogsManagement.Debugger.Write("StopChannel - INIT");

            // Aggiornamento stato del canale
            RefreshChannelState(channelRef);

            if (channelRef.State == ChannelStateEnum.Started ||
                channelRef.State == ChannelStateEnum.UnexpectedStopped)
            {
                // Il canale di pubblicazione risulta avviato su un server diverso da quello attuale
                if (string.Compare(channelRef.MachineName, ApplicationContext.GetMachineName(), false) != 0)
                {
                    // Si sta tentando di fermare un canale di pubblicazione avviato da un altro computer.
                    // Viene richiamato il servizio di pubblicazione da cui è stato avviato per fermarne correttamente lo stato.
                    using (Publisher.Proxy.PublisherWebService realWs = CreatePublisherInstance(channelRef.PublisherServiceUrl))
                    {
                        try
                        {
                            // Fermo del canale nel server di pubblicazione in cui risulta avviato
                            realWs.StopChannelById(channelRef.Id);
                        }
                        catch (PublisherException pubEx)
                        {
                            throw pubEx;
                        }
                        catch (Exception ex)
                        {
                            //DocsPaUtils.LogsManagement.Debugger.Write(ex);

                            // Si è verificato un errore, il server remoto di pubblicazione non risulta raggiungibile,
                            // pertanto viene forzato lo stato di fermo del servizio sul database
                            using (DocsPaDB.TransactionContext tx = new DocsPaDB.TransactionContext())
                            {
                                channelRef = DataAccess.PublisherDataAdapter.StopService(channelRef);

                                tx.Complete();
                            }

                            //throw new PublisherException(ErrorCodes.STOP_CHANNEL_ON_REMOTE_SERVER_ERROR,
                            //                            string.Format(ErrorDescriptions.STOP_CHANNEL_ON_REMOTE_SERVER_ERROR, ex.ToString()));
                        }
                    }
                }
                else
                {
                    using (DocsPaDB.TransactionContext tx = new DocsPaDB.TransactionContext())
                    {
                        // Impostazione dello stato di fermo del servizio
                        channelRef = DataAccess.PublisherDataAdapter.StopService(channelRef);

                        StopTimer(channelRef);

                        tx.Complete();
                    }
                }
            }
            else
            {
                // L'istanza risulta già fermata
                throw new PublisherException(ErrorCodes.OPERATION_NOT_ALLOWED_PER_SERVICE_STOPPED, ErrorDescriptions.OPERATION_NOT_ALLOWED_PER_SERVICE_STARTED);
            }

            //DocsPaUtils.LogsManagement.Debugger.Write("StopChannel - END");
        }

        /// <summary>
        /// Reperimento stato del canale di pubblicazione
        /// </summary>
        /// <param name="channelRef"></param>
        /// <returns></returns>
        public static ChannelStateEnum GetChannelState(ChannelRefInfo channelRef)
        {
            try
            {
                // Aggiorna lo stato del canale
                RefreshChannelState(channelRef);

                return channelRef.State;
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, ex.Message);
            }
        }

        /// <summary>
        /// Riavvio dei servizi fermi in maniera inaspettata
        /// </summary>
        public static void RestartUnexpectedStoppedChannels()
        {
            logger.Debug("RestartUnexpectedStoppedChannels");
            try
            {
                foreach (ChannelRefInfo channel in DataAccess.PublisherDataAdapter.GetChannelList())
                {
                    // Aggiornamento stato del canale
                    RefreshChannelState(channel);
                    
                    if (channel.State == ChannelStateEnum.Stopped)
                        ReStartChannel(channel);
                    
                    if (channel.State == ChannelStateEnum.UnexpectedStopped)
                        ReStartChannel(channel);
                    
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, ex.Message);
            }
        }

        /// <summary>
        /// Aggiornamento stato del canale di pubblicazione
        /// </summary>
        /// <param name="channelRef"></param>
        public static void RefreshChannelState(ChannelRefInfo channelRef)
        {
            //DocsPaUtils.LogsManagement.Debugger.Write("RefreshChannelState - BEGIN");

            try
            {
                if (IsChannelStartedOnRemoteMachine(channelRef))
                {
                    //DocsPaUtils.LogsManagement.Debugger.Write("RefreshChannelState - remote");

                    // Il canale di pubblicazione risulta avviato su un server remoto

                    // Creazione istanza del servizio di pubblicazione remoto
                    using (Publisher.Proxy.PublisherWebService realWs = CreatePublisherInstance(channelRef.PublisherServiceUrl))
                    {
                        Publisher.Proxy.ChannelRefInfo remoteChannel = null;

                        try
                        {
                            // Reperimento dei metadati del canale di pubblicazione
                            remoteChannel = realWs.GetChannel(channelRef.Id);
                        }
                        catch (PublisherException pubEx)
                        {
                            //DocsPaUtils.LogsManagement.Debugger.Write(pubEx);

                            throw pubEx;
                        }
                        catch (Exception ex)
                        {
                            //DocsPaUtils.LogsManagement.Debugger.Write(ex);

                            // Si è verificato un errore nell'esecuzione del servizio di pubblicazione remoto
                            remoteChannel = null;

                            channelRef.State = ChannelStateEnum.UnexpectedStopped;
                        }

                        if (remoteChannel != null)
                        {
                            // Aggiornamento dello stato del canale di pubblicazione
                            if (remoteChannel.State == Proxy.ChannelStateEnum.Started)
                                channelRef.State = ChannelStateEnum.Started;
                            else if (remoteChannel.State == Proxy.ChannelStateEnum.Stopped)
                                channelRef.State = ChannelStateEnum.Stopped;
                            else if (remoteChannel.State == Proxy.ChannelStateEnum.UnexpectedStopped)
                                channelRef.State = ChannelStateEnum.UnexpectedStopped;

                            channelRef.StartExecutionDate = remoteChannel.StartExecutionDate;
                            channelRef.EndExecutionDate = remoteChannel.EndExecutionDate;
                            channelRef.MachineName = remoteChannel.MachineName;
                            channelRef.PublisherServiceUrl = remoteChannel.PublisherServiceUrl;
                        }
                    }
                }
                else
                {
                    StartedChannelTimer timer = GetTimer(channelRef.GetKey());

                    if (timer == null)
                    {
                        if (channelRef.State == ChannelStateEnum.Started &&
                            channelRef.MachineName == ApplicationContext.GetMachineName())
                        {
                            // Se il servizio risulta avviato nella base dati
                            // ma non risulta essere presente nella memoria del server,
                            // significa che è stato fermato in modo non previsto 
                            // (es. il server è giù). Pertanto lo stato del servizio viene
                            // impostato a "UnexpectedStopped".
                            channelRef.State = ChannelStateEnum.UnexpectedStopped;
                        }
                    }
                    else
                    {
                        // Il timer è presente: aggiornamento dello stato del canale
                        channelRef.State = ChannelStateEnum.Started;
                        channelRef.StartExecutionDate = timer.StartExecutionDate;
                        channelRef.EndExecutionDate = DateTime.MinValue;
                        channelRef.MachineName = timer.MachineName;
                        channelRef.PublisherServiceUrl = timer.PublisherServiceUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                //DocsPaUtils.LogsManagement.Debugger.Write(ex);
                logger.Error(ex);
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, ex.Message);
            }

            //DocsPaUtils.LogsManagement.Debugger.Write("RefreshChannelState - END");
        }

        #endregion

        #region Private Members

        /// <summary>
        /// 
        /// </summary>
        private static Dictionary<string, StartedChannelTimer> _dictionary = null;

        /// <summary>
        /// 
        /// </summary>
        static PublisherServiceControl()
        {
            _dictionary = new Dictionary<string, StartedChannelTimer>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Publish(object sender, Schedule.ScheduledEventArgs e)
        {
            logger.Debug("START Publish");
            string disabled = System.Configuration.ConfigurationManager.AppSettings["PUBLISHER_DISABLED"].ToString().ToLower();
            if (!string.IsNullOrEmpty(disabled))
            {
                bool disa = false;
                Boolean.TryParse(disabled, out disa);
                if (disa)
                {
                    logger.Debug("Publisher disabled [PUBLISHER_DISABLED] key is set to true, to enable it, remove key or set to false");
                    return;
                }
            }
            
            // Esecuzione di ogni singolo task schedulato
            ChannelRefInfo channel = (ChannelRefInfo)sender;

            // Reperimento dati dell'istanza
            channel = DataAccess.PublisherDataAdapter.GetChannel(channel.Id);

            // Aggiornamento stato istanza
            RefreshChannelState(channel);
            logger.DebugFormat("Canale {0}  stato {1}",channel.ChannelName,channel.State.ToString() );
            if (channel.State == ChannelStateEnum.Started)
            {
                Mutex mutex = null;

                try
                {
                    logger.Debug ("chiamo CreateMutex");
                    // Creazione o reperimento del mutex
                    mutex = CreateMutex(channel);
                    logger.DebugFormat  ("Mutex Chiamato");
                    if (mutex.WaitOne())
                    {
                        logger.Debug ("Pubblicazione Canale");
                        // Pubblicazione istanza
                        IInstancePublisher publisher = new VtDocs.InstancePublisher();
                        publisher.Publish(channel);
                        logger.Debug("Canale Pubblicato");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (mutex != null)
                    {
                        mutex.ReleaseMutex();
                        mutex.Close();
                        mutex = null;
                        logger.Debug("Mutex Rimosso");
                    }
                }
            }
        }



        private static Mutex CreateMutex(ChannelRefInfo instance)
        {
            bool createdNew; 

            string mutexName = string.Format("PublisherM-{0}-{1}", instance.Admin.Id.ToString(), instance.Id.ToString());
            logger.DebugFormat("GetOrCreateMutex Name {0}", mutexName);
            Mutex m = new Mutex(false, mutexName, out createdNew);

            if (createdNew)
            {

                // the mutex was created and this thread is the owner 
                // (we don't need to call WaitOne)
            }
            else
            {
                m.WaitOne();
                logger.DebugFormat("Mutex {0} Devo aspettare un processo che termini in waitone", mutexName);
                // the mutex already exists, we hold a reference to the mutex, but we
                // don't own it (we need to call WaitOne).
            }
            return m;
        }

        /// <summary>
        /// Reperimento o creazione del Mutext per l'istanza corrente
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private static Mutex GetOrCreateMutex(ChannelRefInfo instance)
        {
            string mutexName = string.Format("PublisherM-{0}-{1}", instance.Admin.Id.ToString(), instance.Id.ToString());
            logger.DebugFormat("GetOrCreateMutex Name {0}",mutexName);
            Mutex m = null;
            bool doesNotExist = false;
            bool unauthorized = false;

            // The value of this variable is set by the mutex
            // constructor. It is true if the named system mutex was
            // created, and false if the named mutex already existed.
            //
            bool mutexWasCreated = false;

            // Attempt to open the named mutex.
            try
            {
                // Open the mutex with (MutexRights.Synchronize |
                // MutexRights.Modify), to enter and release the
                // named mutex.
                //
                m = Mutex.OpenExisting(mutexName);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                logger.DebugFormat("Mutex does not exist.");
                doesNotExist = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.DebugFormat("Unauthorized access: {0}", ex.Message);
                unauthorized = true;
            }

            // There are three cases: (1) The mutex does not exist.
            // (2) The mutex exists, but the current user doesn't 
            // have access. (3) The mutex exists and the user has
            // access.
            //
            if (doesNotExist)
            {
                // The mutex does not exist, so create it.

                // Create an access control list (ACL) that denies the
                // current user the right to enter or release the 
                // mutex, but allows the right to read and change
                // security information for the mutex.
                //
                string user = Environment.UserDomainName + "\\" + Environment.UserName;
                MutexSecurity mSec = new MutexSecurity();

                MutexAccessRule rule = new MutexAccessRule(user,
                    MutexRights.Synchronize | MutexRights.Modify,
                    AccessControlType.Deny);
                mSec.AddAccessRule(rule);

                rule = new MutexAccessRule(user,
                    MutexRights.ReadPermissions | MutexRights.ChangePermissions,
                    AccessControlType.Allow);
                mSec.AddAccessRule(rule);

                // Create a Mutex object that represents the system
                // mutex named by the constant 'mutexName', with
                // initial ownership for this thread, and with the
                // specified security access. The Boolean value that 
                // indicates creation of the underlying system object
                // is placed in mutexWasCreated.
                //
                m = new Mutex(true, mutexName, out mutexWasCreated, mSec);

                // If the named system mutex was created, it can be
                // used by the current instance of this program, even 
                // though the current user is denied access. The current
                // program owns the mutex. Otherwise, exit the program.
                // 
                if (mutexWasCreated)
                {
                    logger.DebugFormat("Created the mutex.");
                }
                else
                {
                    logger.DebugFormat("Unable to create the mutex.");
                    throw new ApplicationException("Unable to create the mutex.");
                }

            }
            else if (unauthorized)
            {
                // Open the mutex to read and change the access control
                // security. The access control security defined above
                // allows the current user to do this.
                //
                try
                {
                    m = Mutex.OpenExisting(mutexName,
                        MutexRights.ReadPermissions | MutexRights.ChangePermissions);

                    // Get the current ACL. This requires 
                    // MutexRights.ReadPermissions.
                    MutexSecurity mSec = m.GetAccessControl();

                    string user = Environment.UserDomainName + "\\"
                        + Environment.UserName;

                    // First, the rule that denied the current user 
                    // the right to enter and release the mutex must
                    // be removed.
                    MutexAccessRule rule = new MutexAccessRule(user,
                         MutexRights.Synchronize | MutexRights.Modify,
                         AccessControlType.Deny);
                    mSec.RemoveAccessRule(rule);

                    // Now grant the user the correct rights.
                    // 
                    rule = new MutexAccessRule(user,
                        MutexRights.Synchronize | MutexRights.Modify,
                        AccessControlType.Allow);
                    mSec.AddAccessRule(rule);

                    // Update the ACL. This requires
                    // MutexRights.ChangePermissions.
                    m.SetAccessControl(mSec);

                    logger.DebugFormat("Updated mutex security.");

                    // Open the mutex with (MutexRights.Synchronize 
                    // | MutexRights.Modify), the rights required to
                    // enter and release the mutex.
                    //
                    m = Mutex.OpenExisting(mutexName);

                }
                catch (UnauthorizedAccessException ex)
                {
                    logger.DebugFormat("Unable to change permissions: {0}", ex.Message);
                    throw new ApplicationException(string.Format("Unable to change permissions: {0}", ex.Message));
                }
            }

            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnScheduleError(object sender, Schedule.ExceptionEventArgs e)
        {
            // Esecuzione di ogni singolo task schedulato
            ChannelRefInfo instance = (ChannelRefInfo)sender;

            PublisherException ex = null;

            if (e.Error.GetType() == typeof(PublisherException))
                ex = (PublisherException)e.Error;
            else
                ex = new PublisherException(ErrorCodes.UNHANDLED_ERROR, e.Error.Message);

            DataAccess.PublisherDataAdapter.SaveError
                (
                    new ErrorInfo
                    {
                        IdInstance = instance.Id,
                        ErrorCode = ex.ErrorCode,
                        ErrorDescription = ex.Message,
                        ErrorStack = ex.ToString(),
                        ErrorDate = e.EventTime
                    }
                );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        private static StartedChannelTimer GetTimer(string key)
        {
            if (_dictionary.ContainsKey(key))
                return _dictionary[key];
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timer"></param>
        private static void SetTimer(string key, StartedChannelTimer timer)
        {
            _dictionary[key] = timer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelRef"></param>
        private static void StartTimer(ChannelRefInfo channelRef)
        {
            // Creazione timer
            Schedule.ScheduleTimer timer = new Schedule.ScheduleTimer();
            timer.Error += new Schedule.ExceptionEventHandler(OnScheduleError);

            
            if (channelRef.ExecutionConfiguration.IntervalType != JobExecutionConfigurations.IntervalTypesEnum.Block)
            {
                long ticks;
                long.TryParse(channelRef.ExecutionConfiguration.ExecutionTicks, out ticks);

                Schedule.ScheduledTime interval = new Schedule.ScheduledTime(
                    (Schedule.EventTimeBase)Enum.Parse(typeof(Schedule.EventTimeBase),channelRef.ExecutionConfiguration.IntervalType.ToString(), true),
                    TimeSpan.FromTicks(ticks));
                timer.AddJob(interval, new Schedule.ScheduledEventHandler(Publish), channelRef);
            }
            else
            {
                string[] spans = channelRef.ExecutionConfiguration.ExecutionTicks.Split('|');
                string strIntervalBase = string.Empty;
                string strStartTime = string.Empty;
                string strStopTime = string.Empty;
                if (spans.Length > 0)
                    strIntervalBase = spans[0];
                if (spans.Length > 1)
                    strStartTime = spans[1];
                if (spans.Length > 2)
                    strStopTime = spans[2];

                long intervalBase;
                long.TryParse(strIntervalBase, out intervalBase);

                long startTime;
                long.TryParse(strStartTime, out startTime);

                long stopTime;
                long.TryParse(strStopTime, out stopTime);

                Schedule.BlockWrapper interval = new Schedule.BlockWrapper(
                    new Schedule.SimpleInterval(DateTime.Parse("01/01/2000"), TimeSpan.FromTicks(intervalBase)),
                    "Daily",
                    new DateTime(startTime).ToString(),
                    new DateTime(stopTime).ToString());
                timer.AddJob(interval, new Schedule.ScheduledEventHandler(Publish), channelRef);
            }
           

            // Avvio timer
            timer.Start();

            // Creazione entry
            StartedChannelTimer entry = new StartedChannelTimer
            {
                Timer = timer,
                StartExecutionDate = channelRef.StartExecutionDate,
                MachineName = channelRef.MachineName,
                PublisherServiceUrl = channelRef.PublisherServiceUrl
            };

            lock (_dictionary)
                _dictionary.Add(channelRef.GetKey(), entry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelRef"></param>
        private static void StopTimer(ChannelRefInfo channelRef)
        {
            if (_dictionary.ContainsKey(channelRef.GetKey()))
            {
                StartedChannelTimer timer = _dictionary[channelRef.GetKey()];

                if (timer != null)
                {
                    timer.Timer.ClearJobs();
                    timer.Timer.Stop();
                    timer.Timer.Dispose();

                    lock (_dictionary)
                        _dictionary.Remove(channelRef.GetKey());
                }
            }
        }

        /// <summary>
        /// Avvio del canale di pubblicazione su un server remoto
        /// </summary>
        /// <param name="channelRef"></param>
        /// <param name="publisherServiceUrl"></param>
        private static void StartRemoteChannel(ChannelRefInfo channelRef, string publisherServiceUrl)
        {
            try
            {
                using (Publisher.Proxy.PublisherWebService realWs = CreatePublisherInstance(publisherServiceUrl))
                {
                    realWs.StartChannelById(channelRef.Id);
                }
            }
            catch (PublisherException pubEx)
            {
                throw pubEx;
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.START_CHANNEL_ON_REMOTE_SERVER_ERROR,
                                            string.Format(ErrorDescriptions.START_CHANNEL_ON_REMOTE_SERVER_ERROR, ex.ToString()));
            }
        }

        /// <summary>
        /// Verifica se il canale di pubblicazione risulta avviato o meno su un server remoto
        /// </summary>
        /// <param name="channelRef"></param>
        /// <returns></returns>
        private static bool IsChannelStartedOnRemoteMachine(ChannelRefInfo channelRef)
        {
            bool onRemote = false;

            if (channelRef.State == ChannelStateEnum.Started)
            {
                // Il canale è sicuramente avviato;
                // verifica se risulta avviato sul computer corrente o su un computer remoto
                onRemote = (string.Compare(channelRef.MachineName, ApplicationContext.GetMachineName(), true) != 0);
            }

            return onRemote;
        }

        /// <summary>
        /// Creazione proxy del servizio di pubblicazione
        /// </summary>
        /// <param name="publisherServiceUrl"></param>
        /// <returns></returns>
        private static Publisher.Proxy.PublisherWebService CreatePublisherInstance(string publisherServiceUrl)
        {
            //DocsPaUtils.LogsManagement.Debugger.Write(publisherServiceUrl);

            Publisher.Proxy.PublisherWebService instance = new Proxy.PublisherWebService();
            instance.Url = publisherServiceUrl;
            instance.Timeout = 10000; // Timeout impostato a 10 secondi
            return instance;
        }

        #endregion
    }
}