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
using DocsPaUtils.Configuration;
using System.Configuration;
using DocsPaUtils.Interfaces.DbManagement;
namespace SAAdminTool.utils
{
    public class PerformanceCheckerAppender : AppenderSkeleton
    {
        private Dictionary<string, StackTrace> _traces = new Dictionary<string, StackTrace>();
        private static System.Text.UnicodeEncoding _utf16 = new System.Text.UnicodeEncoding();

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
                if (loggingEvent.MessageObject.ToString().Equals("BEGIN"))
                {
                    if (!_traces.ContainsKey(id))
                    {
                        StackTrace st = new StackTrace(loggingEvent.TimeStamp, loggingEvent.LocationInformation.ClassName, loggingEvent.LocationInformation.MethodName);
                        _traces.Add(id, st);
                    }
                    else
                    {
                        _traces[id].Add(loggingEvent.TimeStamp, loggingEvent.LocationInformation.ClassName, loggingEvent.LocationInformation.MethodName);
                    }

                }
                if (loggingEvent.MessageObject.ToString().Equals("END"))
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

        private static void Write(string text)
        {
            try
            {

                string debugLogPath = GetDebugPath();
                int debugLevel = GetDebugLevel();
                if (debugLevel > 0)
                {
                    string time = System.DateTime.Now.ToString();
                    XmlDocument xmlDocument = new XmlDocument();
                    debugLogPath = debugLogPath.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
                    debugLogPath = Path.Combine(debugLogPath, DateTime.Now.Hour.ToString("D2"));
                    if (!Directory.Exists(debugLogPath))
                        Directory.CreateDirectory(debugLogPath);
                    debugLogPath = Path.Combine(debugLogPath, "performance.xml");
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
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("Errore nella scrittura del file di debug (segue descrizione errore):");
                System.Diagnostics.Debug.WriteLine(exception.ToString());
            }
        }

        private static int GetDebugLevel()
        {
            int logLevel = 0;
            // Accesso ai dati e reperimento del path in cui saranno scritti i log per l'amministrazione             
            string kValue = InitConfigurationKeys.GetValue("0", "FE_PERFORMANCE_LOG_LEVEL");
            if (kValue == null)
                kValue = ConfigurationManager.AppSettings["PERFORMANCE_LOG_LEVEL"];
            Int32.TryParse(kValue, out logLevel);
            
            return logLevel;
        }

        /// <summary>
        /// Reperimento path per la scrittura dei file di log
        /// </summary>
        /// <returns></returns>
        private static string GetDebugPath()
        {
            string debugPath = string.Empty;
            // Accesso ai dati e reperimento del path in cui saranno scritti i log per l'amministrazione
            debugPath = InitConfigurationKeys.GetValue("0", "FE_PERFORMANCE_LOG_PATH");
            if (debugPath == null)
                debugPath = ConfigurationManager.AppSettings["PERFORMANCE_DEBUG_PATH"];
            
            return debugPath;
        }

        private static void WriteString(FileStream f, string s)
        {
            byte[] bfr = _utf16.GetBytes(s);
            f.Write(bfr, 0, bfr.Length);
        }


        internal class StackTrace
        {
            private StackElement _root;

            public StackTrace(DateTime beginDate, string className, string method)
            {
                _root = new StackElement(beginDate, className, method);
            }

            public void Add(DateTime beginDate, string className, string method)
            {
                _root.Current.Add(new StackElement(beginDate, className, method));
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

            public StackElement(DateTime beginDate, string className, string method)
            {
                _children = new List<StackElement>();
                _beginDate = beginDate;
                _className = className;
                _method = method;
            }

            public void SetEnd(DateTime endDate)
            {
                _endDate = endDate;
                _completed = true;
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

            public void Add(StackElement element)
            {
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

    }

}