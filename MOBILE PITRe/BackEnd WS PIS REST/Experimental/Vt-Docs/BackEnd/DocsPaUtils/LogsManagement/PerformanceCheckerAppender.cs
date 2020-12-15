using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;
using System.Threading;
using System.Runtime.Remoting.Contexts;
using System.Xml;
using System.IO;
using System.Configuration;
using System.Reflection;

namespace DocsPaUtils.LogsManagement
{
    public class PerformanceCheckerAppender : AppenderSkeleton
    {
        private Dictionary<string, StackTrace> _traces = new Dictionary<string, StackTrace>();
        private static System.Text.UnicodeEncoding _utf16 = new System.Text.UnicodeEncoding();
        private string _environmentHandler;
        private ILogEnvironmentHandler _handlerInstance;

        public string EnvironmentHandler
        {
            get { return _environmentHandler; }
            set { this._environmentHandler = value; }
        }


        override public void ActivateOptions()
        {
            if (_environmentHandler == null)
            {
                throw new ArgumentNullException("environmentHandler");
            }
            string[] split = _environmentHandler.Split(new char[] { ',' });
            if (split.Length < 2) throw new Exception("Value has to be in format");

            Assembly ass = GetAssembly(split[1].Trim());
            if (ass == null)
            {
                throw new Exception("Assembly not found");
            }
            Type type = ass.GetType(split[0]);
            if (type == null) throw new Exception("Type not found");
            _handlerInstance = (ILogEnvironmentHandler)Activator.CreateInstance(type);
        }

        private Assembly GetAssembly(string assemblyName)
        {
            Assembly[] assemblyList = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly temp in assemblyList)
            {
                AssemblyName tempName = temp.GetName();
                if (tempName.Name.Equals(assemblyName)) return temp;
            }
            return null;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            CheckEvent(loggingEvent);
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            foreach (LoggingEvent loggingEvent in loggingEvents) CheckEvent(loggingEvent);
        }


        private void CheckEvent(LoggingEvent loggingEvent)
        {
            if (loggingEvent.Level.Name.Equals("INFO"))
            {
                string id = loggingEvent.ThreadName;
                if (loggingEvent.MessageObject.ToString().ToUpper().Equals("BEGIN"))
                {
                    if (!_traces.ContainsKey(id))
                    {
                        StackTrace st = new StackTrace(loggingEvent.TimeStamp, loggingEvent.LocationInformation.ClassName, loggingEvent.LocationInformation.MethodName);
                        _traces.Add(id, st);
                    }
                    else
                    {
                        StackTrace temp = _traces[id];
                        if (temp.LevelExceeded)
                        {
                            _traces[id] = null;
                            _traces.Remove(id);
                        }
                        else
                        {
                            temp.Add(loggingEvent.TimeStamp, loggingEvent.LocationInformation.ClassName, loggingEvent.LocationInformation.MethodName);
                        }
                    }

                }
                if (loggingEvent.MessageObject.ToString().ToUpper().Equals("END"))
                {
                    if (_traces.ContainsKey(id))
                    {
                        _traces[id].SetEnd(loggingEvent.TimeStamp, loggingEvent.LocationInformation.ClassName, loggingEvent.LocationInformation.MethodName);
                        if (_traces[id].Completed)
                        {                            
                            Write(_traces[id].ToXml);
                            _traces[id] = null;
                            _traces.Remove(id);
                        }
                    }
                }
            }
        }

        public String ServerName { get; set; }

        private void Write(string text)
        {
            try
            {
                string debugLogPath = _handlerInstance.FilePath.Replace("%SERVERNAME", this.ServerName);
                string time = System.DateTime.Now.ToString();
                XmlDocument xmlDocument = new XmlDocument();
                debugLogPath = debugLogPath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                debugLogPath = Path.Combine(debugLogPath, DateTime.Now.Hour.ToString("D2"));
                if (!Directory.Exists(debugLogPath))
                    Directory.CreateDirectory(debugLogPath);
                debugLogPath = Path.Combine(debugLogPath, _handlerInstance.FileName);
                if (!File.Exists(debugLogPath))
                {
                    // Crea un nuovo file XML
                    XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-16", "yes");
                    xmlDocument.InnerXml = xmlDeclaration.OuterXml + "<performance>\r\n</performance>";
                    xmlDocument.Save(debugLogPath);
                }

                // Aggiungi messaggi al file XML
                FileStream file = new FileStream(debugLogPath, System.IO.FileMode.Open, System.IO.FileAccess.Write);
                if (file != null)
                {
                    file.Seek(-_utf16.GetByteCount("\r\n</performance>"), System.IO.SeekOrigin.End);
                    WriteString(file, text + "\r\n</performance>");
                    file.Close();
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("Errore nella scrittura del file di debug (segue descrizione errore):");
                System.Diagnostics.Debug.WriteLine(exception.ToString());
            }
        }

        private static void WriteString(FileStream f, string s)
        {
            byte[] bfr = _utf16.GetBytes(s);
            f.Write(bfr, 0, bfr.Length);
        }



        internal class StackTrace
        {
            private StackElement _root;
            private StackContext _context;

            public StackTrace(DateTime beginDate, string className, string method)
            {
                _context = new StackContext();
                _context.Level = 1;
                _root = new StackElement(beginDate, className, method, _context, null);
            }

            public void Add(DateTime beginDate, string className, string method)
            {
                _root.Current.Add(beginDate, className, method);
            }

            public void SetEnd(DateTime endDate, string className, string method)
            {
                StackElement temp = _root.Find(className, method);
                if (temp != null) temp.SetEnd(endDate);
            }

            public bool Completed
            {
                get
                {
                    return _root.Completed;
                }
            }

            public bool LevelExceeded
            {
                get
                {
                    return _context.Level > 10;
                }
            }

            public string ToXml
            {
                get
                {
                    return _root.ToXml;
                }
            }

        }

        internal class StackElement
        {
            private List<StackElement> _children;
            private DateTime _beginDate;
            private DateTime _endDate;
            private bool _completed = false;
            private string _className;
            private string _method;
            private StackContext _context;
            private StackElement _father;

            public StackElement(DateTime beginDate, string className, string method, StackContext context, StackElement father)
            {
                _children = new List<StackElement>();
                _beginDate = beginDate;
                _className = className;
                _method = method;
                _context = context;
                _father = father;
            }

            public void SetEnd(DateTime endDate)
            {
                _endDate = endDate;
                _completed = true;
            }

            private StackElement Father
            {
                get
                {
                    return _father;
                }
            }

            public StackElement Current
            {
                get
                {
                    if (_completed) return null;
                    foreach (StackElement child in _children)
                    {
                        if (child.Current != null) return child.Current;
                    }
                    //se i figli sono completi, allora l'elemento corrente è lui
                    return this;
                }
            }

            public StackElement Find(string className, string method)
            {
                //cerca prima nei figli: c'è il caso in cui nella stessa classe un metodo M 
                //chiama un metodo con lo stesso nome M (ma con diversi parametri).
                //log4net purtroppo non permette 
                foreach (StackElement child in _children)
                {
                    StackElement temp = child.Find(className, method);
                    if (temp != null) return temp;
                }
                if (_className.Equals(className) && _method.Equals(method) && !Completed) return this;
                return null;
            }

            public void Add(DateTime beginDate, string className, string method)
            {
                int level = 0;
                StackElement temp = Father;
                while (temp != null)
                {
                    level++;
                    temp = temp.Father;
                }
                if (level > _context.Level) _context.Level = level;
                StackElement element = new StackElement(beginDate, className, method, _context, this);
                _children.Add(element);
            }

            public string ToXml
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<stackElement class=\"" + _className + "\" method=\"" + _method + "\" begin=\"" + FormatDateTime(_beginDate) + "\"");
                    if (_endDate.CompareTo(DateTime.MinValue) > 0)
                    {
                        sb.Append(" end=\"" + FormatDateTime(_endDate) + "\" elapsed=\"" + Elapsed(_beginDate, _endDate) + "\"");
                    }
                    if (_children.Count == 0)
                    {
                        sb.Append("/>");

                    }
                    else
                    {
                        sb.Append(">");
                        _children.ForEach(e => sb.Append(e.ToXml));
                        sb.Append("</stackElement>");
                    }
                    return sb.ToString();
                }
            }

            public bool Completed
            {
                get
                {
                    return _completed;
                }
            }

            private string FormatDateTime(DateTime dateTime)
            {
                return dateTime.ToString("dd/MM/yyyy HH:mm:ss.fff");
            }

            private string Elapsed(DateTime begin, DateTime end)
            {
                TimeSpan temp = end.Subtract(begin);
                return temp.Hours + ":" + temp.Minutes + ":" + temp.Seconds + "." + temp.Milliseconds;
            }

        }

        internal class StackContext
        {
            public int Level
            {
                get;
                set;
            }
        }

    }
}
