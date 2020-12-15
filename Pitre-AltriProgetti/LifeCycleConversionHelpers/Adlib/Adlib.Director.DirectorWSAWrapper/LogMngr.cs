////////////////////////////////////////////////////////////////////////////
/*
Copyright (C) 1997-2010 Adlib Software
All rights reserved.

DISCLAIMER OF WARRANTIES:
 
Permission is granted to copy this Sample Code for internal use only, 
provided that this permission notice and warranty disclaimer appears in all copies.
 
SAMPLE CODE IS LICENSED TO YOU AS-IS.
 
ADLIB SOFTWARE AND ITS SUPPLIERS AND LICENSORS DISCLAIM ALL WARRANTIES, 
EITHER EXPRESS OR IMPLIED, IN SUCH SAMPLE CODE, INCLUDING THE WARRANTY OF 
NON-INFRINGEMENT AND THE IMPLIED WARRANTIES OF MERCHANTABILITY OR FITNESS FOR A 
PARTICULAR PURPOSE. IN NO EVENT WILL ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS 
BE LIABLE FOR ANY DAMAGES ARISING OUT OF THE USE OF OR INABILITY TO USE THE SAMPLE 
APPLICATION OR SAMPLE CODE, DISTRIBUTION OF THE SAMPLE APPLICATION OR SAMPLE CODE, 
OR COMBINATION OF THE SAMPLE APPLICATION OR SAMPLE CODE WITH ANY OTHER CODE. 
IN NO EVENT SHALL ADLIB SOFTWARE OR ITS LICENSORS AND SUPPLIERS BE LIABLE FOR ANY 
LOST REVENUE, LOST PROFITS OR DATA, OR FOR DIRECT, INDIRECT, SPECIAL, CONSEQUENTIAL, 
INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER CAUSED AND REGARDLESS OF THE THEORY OF LIABILITY, 
EVEN IF ADLIB SOFTWARE OR ITS LICENSORS OR SUPPLIERS HAVE BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
*/
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace Adlib.Director.DirectorWSAWrapper.Logging
{
    public class ElsLogger : ILogger
    {
        private static readonly object locker = new object();

        private static Adlib.Diagnostics.Logging.Engine.LogWriter _AdlibLogger;

        private static string _ApplicationName = "Adlib.Generic.Connector";
        private static string _XmlSettings;

        public static string XmlSettings
        {
            get { return _XmlSettings; }
            set { lock (locker)_XmlSettings = value; }
        }

        public static string ApplicationName
        {
            get { return _ApplicationName; }
            set { lock (locker)_ApplicationName = value; }
        }

        public static void InitLogger(string xmlSettings)
        {
            lock (locker)
            {
                if (xmlSettings != null) _XmlSettings = xmlSettings.ToString();
                _AdlibLogger = null;
            }
        }
        public static void InitLogger(string xmlSettings, string applicationName)
        {
            lock (locker)
            {
                if (xmlSettings != null) _XmlSettings = xmlSettings.ToString();
                _ApplicationName = applicationName;
                _AdlibLogger = null;
            }
        }

        public static Adlib.Diagnostics.Logging.Engine.LogWriter AdlibLogger
        {
            get
            {
                lock (locker)
                {
                    if (_AdlibLogger == null)
                    {
                        if (!String.IsNullOrEmpty(_XmlSettings))
                            _AdlibLogger = new Adlib.Diagnostics.Logging.Engine.LogWriter(Adlib.Diagnostics.Logging.Engine.ConfigurationDataType.XmlString, _XmlSettings, _ApplicationName);
                        else
                            _AdlibLogger = new Adlib.Diagnostics.Logging.Engine.LogWriter(_ApplicationName);

                        _AdlibLogger.Debug("Logger Initialized");

                    }
                }

                return _AdlibLogger;
            }
            set { AdlibLogger = value; }
        }
        public void Info(string message)
        {
            AdlibLogger.Info(message);
        }
        public void Debug(string message)
        {
            AdlibLogger.Debug(message);
        }
        public void Error(string message)
        {
            AdlibLogger.Error(message);
        }
        public void ErrorException(Exception exception)
        {
            AdlibLogger.ErrorException(exception);
        }
        public void Warn(string message)
        {
            AdlibLogger.Warn(message);
        }
        public void ErrorException(string message, Exception exception)
        {
            AdlibLogger.ErrorException(message, exception);
        }

    }

    public class ConsoleLogger : ILogger
    {
        private string DefaultTimeFormat = "yyyy-MM-dd HH:mm:ss:fff"; 
        private int numberOfMessages = 0;
        private void Clean()
        {
            if (numberOfMessages > 300)
            {
                Console.Clear();
                numberOfMessages = 0;
            }
            numberOfMessages++;
        }

        public void Info(string message)
        {
            Clean();
            Console.Out.WriteLine(DateTime.Now.ToString(DefaultTimeFormat) + "\tInfo:\t" + message);

        }
        public void Debug(string message)
        {
            Clean();
            Console.Out.WriteLine(DateTime.Now.ToString(DefaultTimeFormat) + "\tDebug:\t" + message);
        }
        public void Error(string message)
        {
            Clean();
            Console.Out.WriteLine(DateTime.Now.ToString(DefaultTimeFormat) + "\tError:\t" + message);
        }
        public void ErrorException(Exception exception)
        {
            Clean();
            Console.Out.WriteLine(DateTime.Now.ToString(DefaultTimeFormat) + "\tException:\t" + exception.Message);
        }
        public void Warn(string message)
        {
            Clean();
            Console.Out.WriteLine(DateTime.Now.ToString(DefaultTimeFormat) + "\tWarn:\t" + message);
        }
        public void ErrorException(string message, Exception exception)
        {
            Clean();
            Console.Out.WriteLine(DateTime.Now.ToString(DefaultTimeFormat) + "\tErrorException:\t" + message + ". Error: " + exception.Message);
        }
    }

    public class LogMngr : ILogger
    {
        private List<ILogger> m_loggers = new List<ILogger>();
        private object logLocker = new object();
        public LogMngr() { }

        public void AddLogger(ILogger logger)
        {
            bool bFound = false;
            if (logger != null)
            {
                if (m_loggers != null && m_loggers.Count > 0)
                {
                    foreach (ILogger lg in m_loggers)
                    {
                        if (lg != null && lg == logger)
                        {
                            bFound = true;
                            break;
                        }
                    }
                }
                if (bFound == false)
                {
                    m_loggers.Add(logger);
                }
            }
        }

        public List<ILogger> GetLoggers()
        {
            return m_loggers;
        }
        public string GetLogFilePath()
        {
            string path = string.Empty;
            if (m_loggers != null && m_loggers.Count > 0)
            {
                foreach (ILogger lg in m_loggers)
                {
                    if (lg != null && lg is ElsLogger)
                    {
                        string[] pathList = ElsLogger.AdlibLogger.FileTargetPaths;
                        if (pathList != null && pathList[0] != null && pathList[0].Length > 0)
                        {
                            path = pathList[0];
                            break;
                        }
                    }
                }
            }
            return path;
        }
        public void Info(string message)
        {
            foreach (ILogger logger in m_loggers)
            {
                if (logger != null)
                {
                    try
                    {
                        lock (logLocker)
                        {
                            logger.Info(message);
                        }
                    }
                    catch 
                    {

                    }
                }
            }
        }
        public void Debug(string message)
        {
            foreach (ILogger logger in m_loggers)
            {
                if (logger != null)
                {
                    try
                    {
                        lock (logLocker)
                        {
                            logger.Debug(message);
                        }
                    }
                    catch 
                    {

                    }
                }
            }
        }
        public void Error(string message)
        {
            foreach (ILogger logger in m_loggers)
            {
                if (logger != null)
                {
                    try
                    {
                        lock (logLocker)
                        {
                            logger.Error(message);
                        }
                    }
                    catch 
                    {

                    }
                }
            }
        }
        public void Warn(string message)
        {
            foreach (ILogger logger in m_loggers)
            {
                if (logger != null)
                {
                    try
                    {
                        lock (logLocker)
                        {
                            logger.Warn(message);
                        }
                    }
                    catch 
                    {

                    }
                }
            }
        }
        public void ErrorException(Exception exception)
        {
            foreach (ILogger logger in m_loggers)
            {
                if (logger != null)
                {
                    try
                    {
                        lock (logLocker)
                        {
                            logger.ErrorException(exception);
                        }
                    }
                    catch 
                    {

                    }
                }
            }
        }
        public void ErrorException(string message, Exception exception)
        {
            foreach (ILogger logger in m_loggers)
            {
                if (logger != null)
                {
                    try
                    {
                        lock (logLocker)
                        {
                            logger.ErrorException(message, exception);
                        }
                    }
                    catch 
                    {

                    }
                }
            }
        }
    }
}
