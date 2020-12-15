using System;
using System.Configuration;

namespace BusinessLogic.Application
{
	/// <summary>
	/// Summary description for ProtocollazioneInterna.
	/// </summary>
	public class ProtocollazioneInterna
	{
		/// <summary>
		/// 
		/// </summary>
		public static bool Enabled
		{
			get
			{
				bool result = false;
				
//				if(ConfigurationManager.AppSettings["api"] != null)
//				{
//					result = Boolean.Parse(ConfigurationManager.AppSettings["api"]);
//				}

				return result;
			}
		}
	}
}
