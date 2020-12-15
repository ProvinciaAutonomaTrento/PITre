using System;
using System.Collections;
using log4net;

namespace BusinessLogic.Report 
{
	/// <summary>
	/// Summary description for RegistriStampa.
	/// </summary>
	public class ReportUtils 
	{
        private static ILog logger = LogManager.GetLogger(typeof(ReportUtils));

		#region Metodo Commentato
//		/// <summary>
//		/// </summary>
//		/// <returns></returns>
//		public static string getPathName() 
//		{
//			//return DocsPaWebService.Path + "\\report";
//			string basePathFiles = System.Configuration.ConfigurationManager.AppSettings["REPORTS_PATH"];
//			basePathFiles = basePathFiles.Replace("%DATA", DateTime.Now.ToString("yyyyMMdd"));
//
//			return basePathFiles;
//		}
		#endregion

		/// <summary>
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static byte[] toByteArray(string str) 
		{
			char[] charArray=str.ToCharArray();
			System.Collections.ArrayList byteArr=new System.Collections.ArrayList();
			//byte[] res=new byte[charArray.Length];
			for(int i=0;i<charArray.Length;i++) { 
				if((int)charArray[i]>255) {
					string utf="\\u"+((int)charArray[i])+"G";
					char[] utfChars=utf.ToCharArray();
					for(int j=0;j<utfChars.Length;j++) {
						byteArr.Add((byte)utfChars[j]);
					}
				}
				else {
					byteArr.Add((byte)charArray[i]);
				}
				//res[i]=(byte) charArray[i];
			}
			byte[] res=(byte[]) byteArr.ToArray(typeof(byte));
			return res;
		}

		/// <summary>
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static string stringFile(string filePath)
		{
            /*
			string res="";
			System.IO.FileStream fs=null;
			bool streamOpen=false;
			try {
				logger.Debug("stringFile");
				fs=new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				streamOpen=true;
				for(int i=0;i<fs.Length;i++) {
					res=res+(char)fs.ReadByte();
				}
				fs.Close();
			}
			catch(Exception e) 
			{
				if(streamOpen) 
				{
					fs.Close();	
				}
				
				logger.Debug("Errore nella gestione di Report (stringFile)",e);
				throw e;
			}
			return res;
             * */
            string res = string.Empty;
            try
            {
                res = System.IO.File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                logger.Debug("Errore nella gestione di Report (stringFile)", e);
                throw e;
            }
            return res;
		}

		/// <summary>
		/// </summary>
		/// <param name="str"></param>
		/// <param name="report"></param>
		internal static void addStringToReport(string str, ref ArrayList report)
		{
			char[] charArray=str.ToCharArray();
			for(int i=0;i<charArray.Length;i++) 
			{
				//report.Add((byte)charArray[i]);
				if((int)charArray[i]>255) 
				{
					string utf="\\u"+((int)charArray[i])+"G";
					char[] utfChars=utf.ToCharArray();
					for(int j=0;j<utfChars.Length;j++) 
					{
						report.Add((byte)utfChars[j]);
					}
				}
				else 
				{
					report.Add((byte)charArray[i]);
				}
			}
		}
	}
}
