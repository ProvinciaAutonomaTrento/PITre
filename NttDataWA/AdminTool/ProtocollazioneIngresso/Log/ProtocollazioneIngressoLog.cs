using System;
using log4net;

namespace ProtocollazioneIngresso.Log
{
	/// <summary>
	/// Classe per la gestione dei log relativamente alla protocollazione in ingresso
	/// </summary>
	public sealed class ProtocollazioneIngressoLog
	{
        private static ILog logger = LogManager.GetLogger(typeof(ProtocollazioneIngressoLog));
		private const string LOG_ID="ProtocollazioneIngressoSemplificata";

		private ProtocollazioneIngressoLog()
		{
		}

		public static void WriteLogEntry(string entryValue)
		{
			entryValue=LOG_ID + " - " + entryValue;
			logger.Debug(entryValue);
		}
	}
}
