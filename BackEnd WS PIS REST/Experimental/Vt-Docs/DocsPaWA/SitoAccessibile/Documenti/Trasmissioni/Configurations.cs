using System;
using System.Configuration;

namespace DocsPAWA.SitoAccessibile.Documenti.Trasmissioni
{
	/// <summary>
	/// Gestione configurazioni relative alle trasmissioni
	/// </summary>
	public sealed class Configurations
	{
		private Configurations()
		{
		}

		/// <summary>
		/// Attivazione / disattivazione ricerca automatica todolist
		/// </summary>
		public static bool AutomaticSearchTodoList
		{
			get
			{
				bool retValue=false;

				try
				{
					string config=System.Configuration.ConfigurationManager.AppSettings["AUTO_TO_DO_LIST"];
					
					if (config!=null && config!=string.Empty)
						retValue=(Convert.ToInt32(config) > 0);
				}
				catch
				{
				}

				return retValue;
			}
		}
	}
}