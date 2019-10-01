using StampaRegistri.Oggetti;
using StampaRegistri.Utils;
using System;
using System.Diagnostics;
using System.IO;

namespace StampaRegistri
{
	public class Logger
	{
		public static void log(string msg, Costanti.DispositiviDiLog livello, Configuration conf)
		{
			switch (livello)
			{
			case Costanti.DispositiviDiLog.Nessuno:
				break;
			case Costanti.DispositiviDiLog.Console:
				if (!Logger.logInConsole(msg, conf))
				{
					Logger.log(msg, livello - 1, conf);
					return;
				}
				break;
			case Costanti.DispositiviDiLog.File:
				if (!Logger.logInFile(msg, conf))
				{
					Logger.log(msg, livello - 1, conf);
					return;
				}
				break;
			case Costanti.DispositiviDiLog.EventViewer:
				if (!Logger.logInEventLog(msg, conf))
				{
					Logger.log(msg, livello - 1, conf);
					return;
				}
				break;
			default:
				Logger.log(StampaRegistri.Utils.Trace.GetDescrizioneMessaggio(17), Costanti.DispositiviDiLog.Console, conf);
				conf.Log_Device = Costanti.DispositiviDiLog.Console;
				Logger.log("Attenzione: Tutti i messaggi verranno mostrati a video.", conf.Log_Device, conf);
				break;
			}
		}

		private static bool logInFile(string message, Configuration conf)
		{
			bool result;
			try
			{
				string str = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.ff");
				string log_File = conf.Log_File;
				StreamWriter streamWriter = new StreamWriter(log_File, true);
				streamWriter.WriteLine(str + " - " + message + "\n\n");
				streamWriter.Flush();
				streamWriter.Close();
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static bool logInEventLog(string msg, Configuration conf)
		{
			bool result;
			try
			{
				if (!EventLog.SourceExists(conf.App_Name))
				{
					EventLog.CreateEventSource(conf.App_Name, conf.Log_InEventViewer_LogName);
				}
				EventLog eventLog = new EventLog();
				eventLog.Source = conf.App_Name;
				eventLog.WriteEntry(msg, EventLogEntryType.Error, 5);
				eventLog.Close();
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		private static bool logInConsole(string msg, Configuration conf)
		{
			bool result;
			try
			{
				string str = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.ff");
				Console.WriteLine(str + " - " + msg);
				result = true;
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}
	}
}
