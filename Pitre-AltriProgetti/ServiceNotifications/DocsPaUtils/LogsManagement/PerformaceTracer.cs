using System;
using INOUT = System.IO;
using log4net;

namespace DocsPaUtils.LogsManagement
{
	/// <summary>
	/// Summary description for PerformaceTracer.
	/// </summary>
	public class PerformaceTracer
	{
        private static ILog logger = LogManager.GetLogger(typeof(PerformaceTracer));
		private INOUT.FileStream fs = null;
		private INOUT.StreamWriter sw = null;
		private long startTimer = 0;
		
		public PerformaceTracer(string traceType)
		{
			try
			{
				string filename = System.Configuration.ConfigurationManager.AppSettings[traceType];
				if (filename != null && filename != "")
				{
					fs = new INOUT.FileStream(filename, INOUT.FileMode.Append ,INOUT.FileAccess.Write);		
					sw = new INOUT.StreamWriter(fs);
					startTimer = System.DateTime.Now.Ticks ;
				}
			}
			catch
			{
				startTimer = 0;
				logger.Error("Errore durante l'apertura del file di trace");
			}
		}

		public void WriteLogTracer(string stringFrom)
		{
			long delta = ((System.DateTime.Now.Ticks - startTimer) / (long)10000);
			if (startTimer != 0)
			{
				try
				{
					sw.WriteLine(System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss") + " " + stringFrom + " " + delta);				
					sw.AutoFlush = true ;
					//Chiusura del file di trace
					sw.Close();			
					fs.Close();
				}
				catch
				{
					logger.Error("Errore durante l'apertura del file di trace");
					sw.AutoFlush = true ;
					//Chiusura del file di trace
					sw.Close();			
					fs.Close();
				}
			}
		}

	}
}
