/***************************************************************************
 * Copyright Andy Brummer 2004-2005
 * 
 * This code is provided "as is", with absolutely no warranty expressed
 * or implied. Any use is at your own risk.
 *
 * This code may be used in compiled form in any way you desire. This
 * file may be redistributed unmodified by any means provided it is
 * not sold for profit without the authors written consent, and
 * providing that this notice and the authors name is included. If
 * the source code in  this file is used in any commercial application
 * then a simple email would be nice.
 * 
 **************************************************************************/

using System;
using System.Xml;
using System.Xml.XPath;
using System.Reflection;
using System.Collections;
using System.Timers;
using System.Diagnostics;
using System.Collections.Generic;
using DocsPaVO.utente;

namespace DocsPaConservazione.Scheduler
{


    public class Scheduler
    {
        public class verificaAutomaticaJob
        {
            public string ammId;
            public long ticks;
            public string IntervalType;
        }

        private void jobVerifica(object sender, ScheduledEventArgs e)
        {
            // Esecuzione di ogni singolo task schedulato
            string AmmId = sender as String;
            //timer.EventStorage.ToString();
            
            string[] istanze = new DocsPaConsManager().getIstanzeNonVerificate(AmmId);

            foreach (string s in istanze)
            {
                new DocsPaConsManager().VerificaAutomatica(s);
            }

        }

        static ScheduleTimer timer = new ScheduleTimer();
        public static List<String> timerJobs ;
        public Scheduler(List<String> Jobs)
        {
            timerJobs = Jobs;

        }

        public void refreshJobs()
        {
            timer.Stop();      //fermo il timer

            timer.ClearJobs(); //ripulisco
            List<Amministrazione> amministrazioniLst = new List<Amministrazione>((DocsPaVO.utente.Amministrazione[])BusinessLogic.Amministrazione.AmministraManager.GetAmministrazioni().ToArray(typeof(Amministrazione)));
            foreach (Amministrazione a in amministrazioniLst)
            {
                string configString= DocsPaUtils.Configuration.InitConfigurationKeys.GetValue(a.systemId, "BE_CONSERVAZIONE_AUTOTEST_JOB");
                if (!String.IsNullOrEmpty(configString) && !configString.Equals("0"))
                {
                    long tik=0;
                    string interval = "Daily";
                    string[] configVals = configString.Split('|');
                    //Azzeramento dell'event storage, per non appesantire il timer. 
                    timer.EventStorage = new NullEventStorage();
                    
                    interval = configVals[0];
                    if (configVals.Length > 1)
                        long.TryParse(configVals[1], out tik);
                    else
                        tik = 1000000000;
                    // aggiunta dei job con blockwrapper per quelli definiti in minuti e ore.
                    if (interval.ToLower() == "byminute" || interval.ToLower() == "hourly")
                    {
                        BlockWrapper bw = new BlockWrapper(new SimpleInterval(new DateTime(2012, 1, 1), TimeSpan.FromTicks(tik)), "Daily", "00:00", "23:59");
                        timer.AddJob(bw, new ScheduledEventHandler(jobVerifica), a.systemId);
                    }
                    else
                    {
                        //riaggiungo i job (perchè magari nel frattempo sono cambiati
                        timer.AddJob(
                            new ScheduledTime(
                                (EventTimeBase)Enum.Parse(typeof(EventTimeBase), interval, true)
                                , TimeSpan.FromTicks(tik)),
                            new ScheduledEventHandler(jobVerifica),
                            a.systemId);
                    }              
                
                }
            }
            timer.Start();  //rifaccio partire il timer
        }
    }

    /// <summary>
    /// Null event strorage disables error recovery by returning now for the last time an event fired.
    /// </summary>
    /// 
    public class NullEventStorage : IEventStorage
    {
        public NullEventStorage()
        {
        }

        public void RecordLastTime(DateTime Time)
        {
        }

        public DateTime ReadLastTime()
        {
            return DateTime.Now;
        }
    }

    /// <summary>
    /// Local event strorage keeps the last time in memory so that skipped events are not recovered.
    /// </summary>
    public class LocalEventStorage : IEventStorage
    {
        public LocalEventStorage()
        {
            _LastTime = DateTime.MaxValue;
        }

        public void RecordLastTime(DateTime Time)
        {
            _LastTime = Time;
        }

        public DateTime ReadLastTime()
        {
            if (_LastTime == DateTime.MaxValue)
                _LastTime = DateTime.Now;
            return _LastTime;
        }

        DateTime _LastTime;
    }

    /// <summary>
    /// FileEventStorage saves the last time in an XmlDocument so that recovery will include periods that the 
    /// process is shutdown.
    /// </summary>
    public class FileEventStorage : IEventStorage
    {
        public FileEventStorage(string FileName, string XPath)
        {
            _FileName = FileName;
            _XPath = XPath;
        }

        public void RecordLastTime(DateTime Time)
        {
            _Doc.SelectSingleNode(_XPath).Value = Time.ToString();
            _Doc.Save(_FileName);
        }

        public DateTime ReadLastTime()
        {
            _Doc.Load(_FileName);
            string Value = _Doc.SelectSingleNode(_XPath).Value;
            if (Value == null || Value == string.Empty)
                return DateTime.Now;
            return DateTime.Parse(Value);
        }

        string _FileName;
        string _XPath;
        XmlDocument _Doc = new XmlDocument();
    }
    /// <summary>
    /// IScheduledItem represents a scheduled event.  You can query it for the number of events that occur
    /// in a time interval and for the remaining interval before the next event.
    /// </summary>
    public interface IScheduledItem
    {
        /// <summary>
        /// Returns the times of the events that occur in the given time interval.  The interval is closed
        /// at the start and open at the end so that intervals can be stacked without overlapping.
        /// </summary>
        /// <param name="Begin">The beginning of the interval</param>
        /// <param name="End">The end of the interval</param>
        /// <returns>All events >= Begin and &lt; End </returns>
        void AddEventsInInterval(DateTime Begin, DateTime End, ArrayList List);

        /// <summary>
        /// Returns the next run time of the scheduled item.  Optionally excludes the starting time.
        /// </summary>
        /// <param name="time">The starting time of the interval</param>
        /// <param name="IncludeStartTime">if true then the starting time is included in the query false, it is excluded.</param>
        /// <returns>The next execution time either on or after the starting time.</returns>
        DateTime NextRunTime(DateTime time, bool IncludeStartTime);
    }

    /// <summary>
    /// IParameterSetter represents a serialized parameter list.  This is used to provide a partial specialized
    /// method call.  This is useful for remote invocation of method calls.  For example if you have a method with
    /// 3 parameters.  The first 2 might represent static data such as a report and a storage location.  The third
    /// might be the time that the report is invoked, which is only known when the method is invoked.  Using this,
    /// you just pass the method and the first 2 parameters to a timer object, which supplies the 3rd parameter.
    /// Without these objects, you would have to generate a custom object type for each method you wished to 
    /// execute in this manner and store the static parameters as instance variables.  
    /// </summary>
    public interface IParameterSetter
    {
        /// <summary>
        /// This resets the setter to the beginning.  It is used for setters that rely on positional state
        /// information.  It is called prior to setting any method values.
        /// </summary>
        void reset();
        /// <summary>
        /// This method is used to both query support for setting a parameter and actually set the value.
        /// True is returned if the parameter passed in is updated.
        /// </summary>
        /// <param name="pi">The reflection information about this parameter.</param>
        /// <param name="ParameterLoc">The location of the prameter in the parameter list.</param>
        /// <param name="parameter">The parameter object</param>
        /// <returns>true if the parameter is matched and false otherwise</returns>
        bool GetParameterValue(ParameterInfo pi, int ParameterLoc, ref object parameter);
    }

    /// <summary>
    /// This setter object takes a simple object array full of parameter data.  It applys the objects in order
    /// to the method parameter list.
    /// </summary>
    public class OrderParameterSetter : IParameterSetter
    {
        public OrderParameterSetter(params object[] _Params)
        {
            _ParamList = _Params;
        }
        public void reset()
        {
            _counter = 0;
        }
        public bool GetParameterValue(ParameterInfo pi, int ParameterLoc, ref object parameter)
        {
            if (_counter >= _ParamList.Length)
                return false;
            parameter = _ParamList[_counter++];
            return true;
        }

        object[] _ParamList;
        int _counter;
    }

    /// <summary>
    /// This setter object stores the parameter data in a Hashtable and uses the hashtable keys to match 
    /// the parameter names of the method to the parameter data.  This allows methods to be called like 
    /// stored procedures, with the parameters being passed in independent of order.
    /// </summary>
    public class NamedParameterSetter : IParameterSetter
    {
        public NamedParameterSetter(Hashtable Params)
        {
            _Params = Params;
        }
        public void reset()
        {
        }
        public bool GetParameterValue(ParameterInfo pi, int ParameterLoc, ref object parameter)
        {
            string ParamName = pi.Name;
            if (!_Params.ContainsKey(ParamName))
                return false;
            parameter = _Params[ParamName];
            return true;
        }
        Hashtable _Params;
    }


    /// <summary>
    /// ParameterSetterList maintains a collection of IParameterSetter objects and applies them in order to each
    /// parameter of the method.  Each time a match occurs the next parameter is tried starting with the first 
    /// setter object until it is matched.
    /// </summary>
    public class ParameterSetterList
    {
        public void Add(IParameterSetter setter)
        {
            _List.Add(setter);
        }

        public IParameterSetter[] ToArray()
        {
            return (IParameterSetter[])_List.ToArray(typeof(IParameterSetter));
        }

        public void reset()
        {
            foreach (IParameterSetter Setter in _List)
                Setter.reset();
        }

        public object[] GetParameters(MethodInfo Method)
        {
            ParameterInfo[] Params = Method.GetParameters();
            object[] Values = new object[Params.Length];
            //TODO: Update to iterate backwards
            for (int i = 0; i < Params.Length; ++i)
                SetValue(Params[i], i, ref Values[i]);

            return Values;
        }

        public object[] GetParameters(MethodInfo Method, IParameterSetter LastSetter)
        {
            ParameterInfo[] Params = Method.GetParameters();
            object[] Values = new object[Params.Length];
            //TODO: Update to iterate backwards
            for (int i = 0; i < Params.Length; ++i)
            {
                if (!SetValue(Params[i], i, ref Values[i]))
                    LastSetter.GetParameterValue(Params[i], i, ref Values[i]);
            }
            return Values;
        }

        bool SetValue(ParameterInfo Info, int i, ref object Value)
        {
            foreach (IParameterSetter Setter in _List)
            {
                if (Setter.GetParameterValue(Info, i, ref Value))
                    return true;
            }
            return false;
        }

        ArrayList _List = new ArrayList();
    }

    /// <summary>
    /// IMethodCall represents a partially specified parameter data list and a method.  This allows methods to be 
    /// dynamically late invoked for things like timers and other event driven frameworks.
    /// </summary>
    public interface IMethodCall
    {
        ParameterSetterList ParamList { get; }
        object Execute();
        object Execute(IParameterSetter Params);
        void EventHandler(object obj, EventArgs e);
        IAsyncResult BeginExecute(AsyncCallback callback, object obj);
        IAsyncResult BeginExecute(IParameterSetter Params, AsyncCallback callback, object obj);
    }

    delegate object Exec();
    delegate object Exec2(IParameterSetter Params);

    /// <summary>
    /// Method call captures the data required to do a defered method call.
    /// </summary>
    public class DelegateMethodCall : MethodCallBase, IMethodCall
    {
        public DelegateMethodCall(Delegate f)
        {
            _f = f;
        }

        public DelegateMethodCall(Delegate f, params object[] Params)
        {
            if (f.Method.GetParameters().Length < Params.Length)
                throw new ArgumentException("Too many parameters specified for delegate", "f");

            _f = f;
            ParamList.Add(new OrderParameterSetter(Params));
        }

        public DelegateMethodCall(Delegate f, IParameterSetter Params)
        {
            _f = f;
            ParamList.Add(Params);
        }

        Delegate _f;

        public Delegate f
        {
            get { return _f; }
            set { _f = value; }
        }

        public MethodInfo Method
        {
            get { return _f.Method; }
        }

        public object Execute()
        {
            return f.DynamicInvoke(GetParameterList(Method));
        }

        public object Execute(IParameterSetter Params)
        {
            return f.DynamicInvoke(GetParameterList(Method, Params));
        }

        public void EventHandler(object obj, EventArgs e)
        {
            Execute();
        }

        Exec _exec;
        public IAsyncResult BeginExecute(AsyncCallback callback, object obj)
        {
            _exec = new Exec(Execute);
            return _exec.BeginInvoke(callback, obj);
        }

        public IAsyncResult BeginExecute(IParameterSetter Params, AsyncCallback callback, object obj)
        {
            Exec2 exec = new Exec2(Execute);
            return exec.BeginInvoke(Params, callback, obj);
        }
    }

    public class DynamicMethodCall : MethodCallBase, IMethodCall
    {
        public DynamicMethodCall(MethodInfo method)
        {
            _obj = null;
            _method = method;
        }

        public DynamicMethodCall(object obj, MethodInfo method)
        {
            _obj = obj;
            _method = method;
        }

        public DynamicMethodCall(object obj, MethodInfo method, IParameterSetter setter)
        {
            _obj = obj;
            _method = method;
            ParamList.Add(setter);
        }

        object _obj;
        MethodInfo _method;

        public MethodInfo Method
        {
            get { return _method; }
            set { _method = value; }
        }

        public object Execute()
        {
            return _method.Invoke(_obj, GetParameterList(Method));
        }

        public object Execute(IParameterSetter Params)
        {
            return _method.Invoke(_obj, GetParameterList(Method, Params));
        }

        public void EventHandler(object obj, EventArgs e)
        {
            Execute();
        }

        Exec _exec;
        public IAsyncResult BeginExecute(AsyncCallback callback, object obj)
        {
            _exec = new Exec(Execute);
            return _exec.BeginInvoke(callback, null);
        }

        public IAsyncResult BeginExecute(IParameterSetter Params, AsyncCallback callback, object obj)
        {
            Exec2 exec = new Exec2(Execute);
            return exec.BeginInvoke(Params, callback, null);
        }
    }

    /// <summary>
    /// This is a base class that handles the Parameter list management for the 2 dynamic method call methods.
    /// </summary>
    public class MethodCallBase
    {
        ParameterSetterList _ParamList = new ParameterSetterList();

        public ParameterSetterList ParamList
        {
            get { return _ParamList; }
        }

        protected object[] GetParameterList(MethodInfo Method)
        {
            ParamList.reset();
            object[] Params = ParamList.GetParameters(Method);
            return Params;
        }

        protected object[] GetParameterList(MethodInfo Method, IParameterSetter Params)
        {
            ParamList.reset();
            object[] objParams = ParamList.GetParameters(Method, Params);
            return objParams;
        }
    }
    public enum EventTimeBase
    {
        BySecond = 1,
        ByMinute = 2,
        Hourly = 3,
        Daily = 4,
        Weekly = 5,
        Monthly = 6,
    }

    /// <summary>
    /// This class represents a simple schedule.  It can represent a repeating event that occurs anywhere from every
    /// second to once a month.  It consists of an enumeration to mark the interval and an offset from that interval.
    /// For example new ScheduledTime(Hourly, new TimeSpan(0, 15, 0)) would represent an event that fired 15 minutes
    /// after the hour every hour.
    /// </summary>
    [Serializable]
    public class ScheduledTime : IScheduledItem
    {
        public ScheduledTime(EventTimeBase Base, TimeSpan Offset)
        {
            _Base = Base;
            _Offset = Offset;
        }

        /// <summary>
        /// intializes a simple scheduled time element from a pair of strings.  
        /// Here are the supported formats
        /// 
        /// BySecond - single integer representing the offset in ms
        /// ByMinute - A comma seperate list of integers representing the number of seconds and ms
        /// Hourly - A comma seperated list of integers representing the number of minutes, seconds and ms
        /// Daily - A time in hh:mm:ss AM/PM format
        /// Weekly - n, time where n represents an integer and time is a time in the Daily format
        /// Monthly - the same format as weekly.
        /// 
        /// </summary>
        /// <param name="StrBase">A string representing the base enumeration for the scheduled time</param>
        /// <param name="StrOffset">A string representing the offset for the time.</param>
        public ScheduledTime(string StrBase, string StrOffset)
        {
            //TODO:Create an IScheduled time factory method.
            _Base = (EventTimeBase)Enum.Parse(typeof(EventTimeBase), StrBase, true);
            Init(StrOffset);
        }

        public int ArrayAccess(string[] Arr, int i)
        {
            if (i >= Arr.Length)
                return 0;
            return int.Parse(Arr[i]);
        }

        public void AddEventsInInterval(DateTime Begin, DateTime End, ArrayList List)
        {
            DateTime Next = NextRunTime(Begin, true);
            while (Next < End)
            {
                List.Add(Next);
                Next = IncInterval(Next);
            }
        }

        public DateTime NextRunTime(DateTime time, bool AllowExact)
        {
            DateTime NextRun = LastSyncForTime(time) + _Offset;
            if (NextRun == time && AllowExact)
                return time;
            if (NextRun > time)
                return NextRun;
            return IncInterval(NextRun);
        }


        private DateTime LastSyncForTime(DateTime time)
        {
            switch (_Base)
            {
                case EventTimeBase.BySecond:
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
                case EventTimeBase.ByMinute:
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0);
                case EventTimeBase.Hourly:
                    return new DateTime(time.Year, time.Month, time.Day, time.Hour, 0, 0);
                case EventTimeBase.Daily:
                    return new DateTime(time.Year, time.Month, time.Day);
                case EventTimeBase.Weekly:
                    return (new DateTime(time.Year, time.Month, time.Day)).AddDays(-(int)time.DayOfWeek);
                case EventTimeBase.Monthly:
                    return new DateTime(time.Year, time.Month, 1);
            }
            throw new Exception("Invalid base specified for timer.");
        }

        private DateTime IncInterval(DateTime Last)
        {
            switch (_Base)
            {
                case EventTimeBase.BySecond:
                    return Last.AddSeconds(1);
                case EventTimeBase.ByMinute:
                    return Last.AddMinutes(1);
                case EventTimeBase.Hourly:
                    return Last.AddHours(1);
                case EventTimeBase.Daily:
                    return Last.AddDays(1);
                case EventTimeBase.Weekly:
                    return Last.AddDays(7);
                case EventTimeBase.Monthly:
                    return Last.AddMonths(1);
            }
            throw new Exception("Invalid base specified for timer.");
        }

        private void Init(string StrOffset)
        {
            switch (_Base)
            {
                case EventTimeBase.BySecond:
                    _Offset = new TimeSpan(0, 0, 0, 0, int.Parse(StrOffset));
                    break;
                case EventTimeBase.ByMinute:
                    string[] ArrMinute = StrOffset.Split(',');
                    _Offset = new TimeSpan(0, 0, 0, ArrayAccess(ArrMinute, 0), ArrayAccess(ArrMinute, 1));
                    break;
                case EventTimeBase.Hourly:
                    string[] ArrHour = StrOffset.Split(',');
                    _Offset = new TimeSpan(0, 0, ArrayAccess(ArrHour, 0), ArrayAccess(ArrHour, 1), ArrayAccess(ArrHour, 2));
                    break;
                case EventTimeBase.Daily:
                    DateTime Daytime = DateTime.Parse(StrOffset);
                    _Offset = new TimeSpan(0, Daytime.Hour, Daytime.Minute, Daytime.Second, Daytime.Millisecond);
                    break;
                case EventTimeBase.Weekly:
                    string[] ArrWeek = StrOffset.Split(',');
                    if (ArrWeek.Length != 2)
                        throw new Exception("Weekly offset must be in the format n, time where n is the day of the week starting with 0 for sunday");
                    DateTime WeekTime = DateTime.Parse(ArrWeek[1]);
                    _Offset = new TimeSpan(int.Parse(ArrWeek[0]), WeekTime.Hour, WeekTime.Minute, WeekTime.Second, WeekTime.Millisecond);
                    break;
                case EventTimeBase.Monthly:
                    string[] ArrMonth = StrOffset.Split(',');
                    if (ArrMonth.Length != 2)
                        throw new Exception("Monthly offset must be in the format n, time where n is the day of the month starting with 1 for the first day of the month.");
                    DateTime MonthTime = DateTime.Parse(ArrMonth[1]);
                    _Offset = new TimeSpan(int.Parse(ArrMonth[0]) - 1, MonthTime.Hour, MonthTime.Minute, MonthTime.Second, MonthTime.Millisecond);
                    break;
                default:
                    throw new Exception("Invalid base specified for timer.");
            }
        }

        private EventTimeBase _Base;
        private TimeSpan _Offset;
    }
    /// <summary>
    /// ScheduleTimer represents a timer that fires on a more human friendly schedule.  For example it is easy to 
    /// set it to fire every day at 6:00PM.  It is useful for batch jobs or alarms that might be difficult to 
    /// schedule with the native .net timers.
    /// It is similar to the .net timer that it is based on with the start and stop methods functioning similarly.
    /// The main difference is the event uses a different delegate and arguement since the .net timer argument 
    /// class is not creatable.
    /// </summary>
    public class ScheduleTimerBase : IDisposable
    {
        public ScheduleTimerBase()
        {
            _Timer = new Timer();
            _Timer.AutoReset = false;
            _Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _Jobs = new TimerJobList();
            _LastTime = DateTime.MaxValue;
        }

        /// <summary>
        /// Adds a job to the timer.  This method passes in a delegate and the parameters similar to the Invoke method of windows forms.
        /// </summary>
        /// <param name="Schedule">The schedule that this delegate is to be run on.</param>
        /// <param name="f">The delegate to run</param>
        /// <param name="Params">The method parameters to pass if you leave any DateTime parameters unbound, then they will be set with the scheduled run time of the 
        /// method.  Any unbound object parameters will get this Job object passed in.</param>
        public void AddJob(IScheduledItem Schedule, Delegate f, params object[] Params)
        {
            _Jobs.Add(new TimerJob(Schedule, new DelegateMethodCall(f, Params)));
        }

        /// <summary>
        /// Adds a job to the timer to operate asyncronously.
        /// </summary>
        /// <param name="Schedule">The schedule that this delegate is to be run on.</param>
        /// <param name="f">The delegate to run</param>
        /// <param name="Params">The method parameters to pass if you leave any DateTime parameters unbound, then they will be set with the scheduled run time of the 
        /// method.  Any unbound object parameters will get this Job object passed in.</param>
        public void AddAsyncJob(IScheduledItem Schedule, Delegate f, params object[] Params)
        {
            TimerJob Event = new TimerJob(Schedule, new DelegateMethodCall(f, Params));
            Event.SyncronizedEvent = false;
            _Jobs.Add(Event);
        }

        /// <summary>
        /// Adds a job to the timer.  
        /// </summary>
        /// <param name="Event"></param>
        public void AddJob(TimerJob Event)
        {
            _Jobs.Add(Event);
        }

        /// <summary>
        /// Clears out all scheduled jobs.
        /// </summary>
        public void ClearJobs()
        {
            _Jobs.Clear();
        }

        /// <summary>
        /// Begins executing all assigned jobs at the scheduled times
        /// </summary>
        public void Start()
        {
            _StopFlag = false;
            QueueNextTime(EventStorage.ReadLastTime());
        }

        /// <summary>
        /// Halts executing all jobs.  When the timer is restarted all jobs that would have run while the timer was stopped are re-tried.
        /// </summary>
        public void Stop()
        {
            _StopFlag = true;
            _Timer.Stop();
        }

        /// <summary>
        /// EventStorage determines the method used to store the last event fire time.  It defaults to keeping it in memory.
        /// </summary>
        public IEventStorage EventStorage = new LocalEventStorage();
        public event ExceptionEventHandler Error;

        #region Private Methods and Fields
        /// <summary>
        /// This is here to enhance accuracy.  Even if nothing is scheduled the timer sleeps for a maximum of 1 minute.
        /// </summary>
        private static TimeSpan MAX_INTERVAL = new TimeSpan(0, 1, 0);

        private DateTime _LastTime;
        private Timer _Timer;
        private TimerJobList _Jobs;
        private volatile bool _StopFlag;

        private double NextInterval(DateTime thisTime)
        {
            TimeSpan interval = _Jobs.NextRunTime(thisTime) - thisTime;

            return interval.TotalMilliseconds;

            ////if (interval > MAX_INTERVAL)
            ////    interval = MAX_INTERVAL;
            //////Handles the case of 0 wait time, the interval property requires a duration > 0.
            ////return (interval.TotalMilliseconds == 0) ? 1 : interval.TotalMilliseconds;
        }

        private void QueueNextTime(DateTime thisTime)
        {
            _Timer.Interval = NextInterval(thisTime);
            System.Diagnostics.Debug.WriteLine(_Timer.Interval);
            _LastTime = thisTime;
            EventStorage.RecordLastTime(thisTime);
            _Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (_Jobs == null)
                    return;

                _Timer.Stop();

                foreach (TimerJob Event in _Jobs.Jobs)
                {
                    try { Event.Execute(this, _LastTime, e.SignalTime, Error); }
                    catch (Exception ex) { OnError(DateTime.Now, Event, ex); }
                }
            }
            catch (Exception ex)
            {
                OnError(DateTime.Now, null, ex);
            }
            finally
            {
                if (_StopFlag == false)
                    QueueNextTime(e.SignalTime);
            }
        }

        private void OnError(DateTime eventTime, TimerJob job, Exception e)
        {
            if (Error == null)
                return;

            try { Error(this, new ExceptionEventArgs(eventTime, e)); }
            catch (Exception) { }
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_Timer != null)
                _Timer.Dispose();
        }

        #endregion
    }

    public class ScheduleTimer : ScheduleTimerBase
    {
        /// <summary>
        /// Add event is used in conjunction with the Elaspsed event handler.  Set the Elapsed handler, add your schedule and call start.
        /// </summary>
        /// <param name="Schedule">The schedule to fire the event at.  Adding additional schedules will cause the event to fire whenever either schedule calls for it.</param>
        public void AddEvent(IScheduledItem Schedule)
        {
            if (Elapsed == null)
                throw new ArgumentNullException("Elapsed", "member variable is null.");

            AddJob(new TimerJob(Schedule, new DelegateMethodCall(Elapsed)));
        }

        /// <summary>
        /// The event to fire when you only need to fire one event.
        /// </summary>
        public event ScheduledEventHandler Elapsed;
    }

    /// <summary>
    /// ExceptionEventArgs allows exceptions to be captured and sent to the OnError event of the timer.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        public ExceptionEventArgs(DateTime eventTime, Exception e)
        {
            EventTime = eventTime;
            Error = e;
        }
        public DateTime EventTime;
        public Exception Error;
    }

    /// <summary>
    /// ExceptionEventHandler is the method type used by the OnError event for the timer.
    /// </summary>
    public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs Args);

    public class ScheduledEventArgs : EventArgs
    {
        public ScheduledEventArgs(DateTime eventTime)
        {
            EventTime = eventTime;
        }
        public DateTime EventTime;
    }

    public delegate void ScheduledEventHandler(object sender, ScheduledEventArgs e);

    /// <summary>
    /// The IResultFilter interface represents filters that either sort the events for an interval or
    /// remove duplicate events either selecting the first or the last event.
    /// </summary>
    public interface IResultFilter
    {
        void FilterResultsInInterval(DateTime Start, DateTime End, ArrayList List);
    }

    /// <summary>
    /// IEventStorage is used to provide persistance of schedule during service shutdowns.
    /// </summary>
    public interface IEventStorage
    {
        void RecordLastTime(DateTime Time);
        DateTime ReadLastTime();
    }

    /// <summary>
    /// Timer job groups a schedule, syncronization data, a result filter, method information and an enabled state so that multiple jobs
    /// can be managed by the same timer.  Each one operating independently of the others with different syncronization and recovery settings.
    /// </summary>
    public class TimerJob
    {
        public TimerJob(IScheduledItem schedule, IMethodCall method)
        {
            Schedule = schedule;
            Method = method;
            _ExecuteHandler = new ExecuteHandler(ExecuteInternal);
        }
        public IScheduledItem Schedule;
        public bool SyncronizedEvent = true;
        public IResultFilter Filter;
        public IMethodCall Method;
        //		public IJobLog Log;
        public bool Enabled = true;

        public DateTime NextRunTime(DateTime time, bool IncludeStartTime)
        {
            if (!Enabled)
                return DateTime.MaxValue;
            return Schedule.NextRunTime(time, IncludeStartTime);
        }

        public void Execute(object sender, DateTime Begin, DateTime End, ExceptionEventHandler Error)
        {
            if (!Enabled)
                return;

            ArrayList EventList = new ArrayList();
            Schedule.AddEventsInInterval(Begin, End, EventList);

            if (Filter != null)
                Filter.FilterResultsInInterval(Begin, End, EventList);

            foreach (DateTime EventTime in EventList)
            {
                if (SyncronizedEvent)
                    _ExecuteHandler(sender, EventTime, Error);
                else
                    _ExecuteHandler.BeginInvoke(sender, EventTime, Error, null, null);
            }
        }

        private void ExecuteInternal(object sender, DateTime EventTime, ExceptionEventHandler Error)
        {
            try
            {
                TimerParameterSetter Setter = new TimerParameterSetter(EventTime, sender);
                Method.Execute(Setter);
            }
            catch (Exception ex)
            {
                if (Error != null)
                    try { Error(this, new ExceptionEventArgs(EventTime, ex)); }
                    catch { }
            }
        }

        private delegate void ExecuteHandler(object sender, DateTime EventTime, ExceptionEventHandler Error);

        private ExecuteHandler _ExecuteHandler;
    }

    /// <summary>
    /// Timer job manages a group of timer jobs.
    /// </summary>
    public class TimerJobList
    {
        public TimerJobList()
        {
            _List = new ArrayList();
        }

        public void Add(TimerJob Event)
        {
            _List.Add(Event);
        }

        public void Clear()
        {
            _List.Clear();
        }

        /// <summary>
        /// Gets the next time any of the jobs in the list will run.  Allows matching the exact start time.  If no matches are found the return
        /// is DateTime.MaxValue;
        /// </summary>
        /// <param name="time">The starting time for the interval being queried.  This time is included in the interval</param>
        /// <returns>The first absolute date one of the jobs will execute on.  If none of the jobs needs to run DateTime.MaxValue is returned.</returns>
        public DateTime NextRunTime(DateTime time)
        {
            DateTime next = DateTime.MaxValue;
            //Get minimum datetime from the list.
            foreach (TimerJob Job in _List)
            {
                DateTime Proposed = Job.NextRunTime(time, true);
                next = (Proposed < next) ? Proposed : next;
            }
            return next;
        }

        public TimerJob[] Jobs
        {
            get { return (TimerJob[])_List.ToArray(typeof(TimerJob)); }
        }

        private ArrayList _List;
    }

    /// <summary>
    /// The timer job allows delegates to be specified with unbound parameters.  This ParameterSetter assigns all unbound datetime parameters
    /// with the specified time and all unbound object parameters with the calling object.
    /// </summary>
    public class TimerParameterSetter : IParameterSetter
    {
        /// <summary>
        /// Initalize the ParameterSetter with the time to pass to unbound time parameters and object to pass to unbound object parameters.
        /// </summary>
        /// <param name="time">The time to pass to the unbound DateTime parameters</param>
        /// <param name="sender">The object to pass to the unbound object parameters</param>
        public TimerParameterSetter(DateTime time, object sender)
        {
            _time = time;
            _sender = sender;
        }
        public void reset()
        {
        }
        public bool GetParameterValue(ParameterInfo pi, int ParameterLoc, ref object parameter)
        {
            switch (pi.ParameterType.Name.ToLower())
            {
                case "datetime":
                    parameter = _time;
                    return true;
                case "object":
                    parameter = _sender;
                    return true;
                case "scheduledeventargs":
                    parameter = new ScheduledEventArgs(_time);
                    return true;
                case "eventargs":
                    parameter = new ScheduledEventArgs(_time);
                    return true;
            }
            return false;
        }
        DateTime _time;
        object _sender;
    }
    public class BlockWrapper : IScheduledItem
    {
        public BlockWrapper(IScheduledItem item, string StrBase, string BeginOffset, string EndOffset)
        {
            _Item = item;
            _Begin = new ScheduledTime(StrBase, BeginOffset);
            _End = new ScheduledTime(StrBase, EndOffset);
        }
        public void AddEventsInInterval(DateTime Begin, DateTime End, ArrayList List)
        {
            DateTime Next = NextRunTime(Begin, true);
            while (Next < End)
            {
                List.Add(Next);
                Next = NextRunTime(Next, false);
            }
        }

        public DateTime NextRunTime(DateTime time, bool AllowExact)
        {
            return NextRunTime(time, 100, AllowExact);
        }

        DateTime NextRunTime(DateTime time, int count, bool AllowExact)
        {
            if (count == 0)
                throw new Exception("Invalid block wrapper combination.");

            DateTime
                temp = _Item.NextRunTime(time, AllowExact),
                begin = _Begin.NextRunTime(time, true),
                end = _End.NextRunTime(time, true);
            System.Diagnostics.Debug.WriteLine(string.Format("{0} {1} {2} {3}", time, begin, end, temp));
            bool A = temp > end, B = temp < begin, C = end < begin;
            System.Diagnostics.Debug.WriteLine(string.Format("{0} {1} {2}", A, B, C));
            if (C)
            {
                if (A && B)
                    return NextRunTime(begin, --count, false);
                else
                    return temp;
            }
            else
            {
                if (!A && !B)
                    return temp;
                if (!A)
                    return NextRunTime(begin, --count, false);
                else
                    return NextRunTime(end, --count, false);
            }
        }
        private IScheduledItem _Item;
        private ScheduledTime _Begin, _End;
    }

    /// <summary>
    /// There have been quite a few requests to allow scheduling of multiple delegates and method parameter data
    /// from the same timer.  This class allows you to match the event with the time that it fired.  I want to keep
    /// the same simple implementation of the EventQueue and interval classes since they can be reused elsewhere.
    /// The timer should be responsible for matching this data up.
    /// </summary>
    public class EventInstance : IComparable
    {
        public EventInstance(DateTime time, IScheduledItem scheduleItem, object data)
        {
            Time = time;
            ScheduleItem = scheduleItem;
            Data = data;
        }
        public DateTime Time;
        public IScheduledItem ScheduleItem;
        public object Data;

        public int CompareTo(object obj)
        {
            if (obj is EventInstance)
                return Time.CompareTo(((EventInstance)obj).Time);
            if (obj is DateTime)
                return Time.CompareTo((DateTime)obj);
            return 0;
        }
    }

    public class EventQueue : IScheduledItem
    {
        public EventQueue()
        {
            _List = new ArrayList();
        }
        /// <summary>
        /// Adds a ScheduledTime to the queue.
        /// </summary>
        /// <param name="time">The scheduled time to add</param>
        public void Add(IScheduledItem time)
        {
            _List.Add(time);
        }

        /// <summary>
        /// Clears the list of scheduled times.
        /// </summary>
        public void Clear()
        {
            _List.Clear();
        }

        /// <summary>
        /// Adds the running time for all events in the list.
        /// </summary>
        /// <param name="Begin">The beginning time of the interval</param>
        /// <param name="End">The end time of the interval</param>
        /// <param name="List">The list to add times to.</param>
        public void AddEventsInInterval(DateTime Begin, DateTime End, ArrayList List)
        {
            foreach (IScheduledItem st in _List)
                st.AddEventsInInterval(Begin, End, List);
            List.Sort();
        }

        /// <summary>
        /// Returns the first time after the starting time for all events in the list.
        /// </summary>
        /// <param name="time">The starting time.</param>
        /// <param name="AllowExact">If this is true then it allows the return time to match the time parameter, false forces the return time to be greater then the time parameter</param>
        /// <returns>Either the next event after the input time or greater or equal to depending on the AllowExact parameter.</returns>
        public DateTime NextRunTime(DateTime time, bool AllowExact)
        {
            DateTime next = DateTime.MaxValue;
            //Get minimum datetime from the list.
            foreach (IScheduledItem st in _List)
            {
                DateTime Proposed = st.NextRunTime(time, AllowExact);
                next = (Proposed < next) ? Proposed : next;
            }
            return next;
        }
        private ArrayList _List;
    }
    /// <summary>
    /// The simple interval represents the simple scheduling that .net supports natively.  It consists of a start
    /// absolute time and an interval that is counted off from the start time.
    /// </summary>
    [Serializable]
    public class SimpleInterval : IScheduledItem
    {
        public SimpleInterval(DateTime StartTime, TimeSpan Interval)
        {
            _Interval = Interval;
            _StartTime = StartTime;
            _EndTime = DateTime.MaxValue;
        }
        public SimpleInterval(DateTime StartTime, TimeSpan Interval, int count)
        {
            _Interval = Interval;
            _StartTime = StartTime;
            _EndTime = StartTime + TimeSpan.FromTicks(Interval.Ticks * count);
        }
        public SimpleInterval(DateTime StartTime, TimeSpan Interval, DateTime EndTime)
        {
            _Interval = Interval;
            _StartTime = StartTime;
            _EndTime = EndTime;
        }
        public void AddEventsInInterval(DateTime Begin, DateTime End, ArrayList List)
        {
            if (End <= _StartTime)
                return;
            DateTime Next = NextRunTime(Begin, true);
            while (Next < End)
            {
                List.Add(Next);
                Next = NextRunTime(Next, false);
            }
        }

        public DateTime NextRunTime(DateTime time, bool AllowExact)
        {
            DateTime returnTime = NextRunTimeInt(time, AllowExact);
            Debug.WriteLine(time);
            Debug.WriteLine(returnTime);
            Debug.WriteLine(_EndTime);
            return (returnTime >= _EndTime) ? DateTime.MaxValue : returnTime;
        }

        private DateTime NextRunTimeInt(DateTime time, bool AllowExact)
        {
            TimeSpan Span = time - _StartTime;
            if (Span < TimeSpan.Zero)
                return _StartTime;
            if (ExactMatch(time))
                return AllowExact ? time : time + _Interval;
            uint msRemaining = (uint)(_Interval.TotalMilliseconds - ((uint)Span.TotalMilliseconds % (uint)_Interval.TotalMilliseconds));
            return time.AddMilliseconds(msRemaining);
        }

        private bool ExactMatch(DateTime time)
        {
            TimeSpan Span = time - _StartTime;
            if (Span < TimeSpan.Zero)
                return false;
            return (Span.TotalMilliseconds % _Interval.TotalMilliseconds) == 0;
        }

        private TimeSpan _Interval;
        private DateTime _StartTime;
        private DateTime _EndTime;
    }
    /// <summary>Single event represents an event which only fires once.</summary>
    public class SingleEvent : IScheduledItem
    {
        public SingleEvent(DateTime eventTime)
        {
            _EventTime = eventTime;
        }
        #region IScheduledItem Members

        public void AddEventsInInterval(DateTime Begin, DateTime End, System.Collections.ArrayList List)
        {
            if (Begin <= _EventTime && End > _EventTime)
                List.Add(_EventTime);
        }

        public DateTime NextRunTime(DateTime time, bool IncludeStartTime)
        {
            if (IncludeStartTime)
                return (_EventTime >= time) ? _EventTime : DateTime.MaxValue;
            else
                return (_EventTime > time) ? _EventTime : DateTime.MaxValue;
        }
        private DateTime _EventTime;

        #endregion
    }
}

