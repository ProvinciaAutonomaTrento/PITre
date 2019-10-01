using StampaRegistri.Oggetti;
using System;
using System.Web.Services.Protocols;
using System.Xml;

namespace StampaRegistri.Utils
{
	public class Trace
	{
		public static string GetDescrizioneMessaggio(int codMsg)
		{
			return ((Costanti.MessaggiDaVisualizzare)codMsg).ToString().Replace("______", "...").Replace("____", "'").Replace("___", ": ").Replace("__", ". ").Replace("_", " ");
		}

		private static string formattaMsg(string message, string tipoMsg)
		{
			return "[" + tipoMsg + "]-" + message;
		}

		public static void Traccia(Exception ex, Configuration conf)
		{
			Trace.Traccia(ex, conf, conf.Log_Device);
		}

		public static void Traccia(Exception ex, Configuration conf, Costanti.DispositiviDiLog device)
		{
			string text = ex.Message;
			string str = "";
			if (ex.GetType().Equals(typeof(SoapException)))
			{
				SoapException ex2 = (SoapException)ex;
				text = ((XmlElement)ex2.Detail).GetElementsByTagName("messaggio")[0].FirstChild.Value;
				str = ((XmlElement)ex2.Detail).GetElementsByTagName("debug")[0].FirstChild.Value;
			}
			if (conf.Log_LevelTrace >= Costanti.LivelliLog.Debug)
			{
				text = text + "\n\n" + str;
			}
			Trace.traccia(text, Costanti.TipoMessaggio.ERRORE, device, conf);
		}

		public static void Traccia(Costanti.Errori errore, Configuration conf)
		{
			Trace.Traccia(errore, conf, conf.Log_Device);
		}

		public static void Traccia(Costanti.Errori errore, Configuration conf, Costanti.DispositiviDiLog device)
		{
			Trace.traccia(Trace.GetDescrizioneMessaggio((int)errore), Costanti.TipoMessaggio.ERRORE, device, conf);
		}

		public static void Traccia(Costanti.Debug debug, Configuration conf)
		{
			Trace.traccia(Trace.GetDescrizioneMessaggio((int)debug), Costanti.TipoMessaggio.DEBUG, conf.Log_Device, conf);
		}

		public static void Traccia(Costanti.Debug debug, Configuration conf, Costanti.DispositiviDiLog device)
		{
			Trace.traccia(Trace.GetDescrizioneMessaggio((int)debug), Costanti.TipoMessaggio.DEBUG, device, conf);
		}

		public static void Traccia(Costanti.Informazioni idInfo, Configuration conf)
		{
			Trace.Traccia(idInfo, conf, conf.Log_Device);
		}

		public static void Traccia(Costanti.Informazioni idInfo, Configuration conf, Costanti.DispositiviDiLog device)
		{
			Trace.traccia(Trace.GetDescrizioneMessaggio((int)idInfo), Costanti.TipoMessaggio.INFORMAZIONE, device, conf);
		}

		public static void Traccia(string messaggio, Costanti.TipoMessaggio tipoMsg, Configuration conf)
		{
			Trace.traccia(messaggio, tipoMsg, conf.Log_Device, conf);
		}

		public static void Traccia(string messaggio, Costanti.TipoMessaggio tipoMsg, Configuration conf, Costanti.DispositiviDiLog device)
		{
			Trace.traccia(messaggio, tipoMsg, device, conf);
		}

		public static void Traccia(int idMessaggio, string messaggioAggiuntivo, Costanti.TipoMessaggio tipoMsg, Configuration conf)
		{
			Trace.traccia(Trace.GetDescrizioneMessaggio(idMessaggio) + messaggioAggiuntivo, tipoMsg, conf.Log_Device, conf);
		}

		public static void Traccia(int idMessaggio, string messaggioAggiuntivo, Costanti.TipoMessaggio tipoMsg, Configuration conf, Costanti.DispositiviDiLog device)
		{
			Trace.traccia(Trace.GetDescrizioneMessaggio(idMessaggio) + messaggioAggiuntivo, tipoMsg, device, conf);
		}

		private static void traccia(string msg, Costanti.TipoMessaggio tipoMsg, Costanti.DispositiviDiLog device, Configuration conf)
		{
			string msg2 = Trace.formattaMsg(msg, tipoMsg.ToString());
			Costanti.LivelliLog log_LevelTrace = conf.Log_LevelTrace;
			if (log_LevelTrace != Costanti.LivelliLog.NonTracciare && tipoMsg <= (Costanti.TipoMessaggio)conf.Log_LevelTrace)
			{
				Logger.log(msg2, device, conf);
			}
		}
	}
}
