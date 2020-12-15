using System;
using System.Collections;
using System.Web;

namespace DocsPAWA.SitoAccessibile
{
	/// <summary>
	/// Interfaccia necessaria per consentire al motore del sito accessibile
	/// di rilasciare le risorse specifiche relative al contesto corrente
	/// </summary>
	public interface IResourceReleaser
	{
		/// <summary>
		/// Metodo necessario per il rilascio delle risorse
		/// </summary>
		void ReleaseResources();
	}

	/// <summary>
	/// Classe necessaria per il rilascio delle risorse
	/// </summary>
	public sealed class ResourceReleaser
	{
		private const string SESSION_KEY="ResourceReleaser";

		/// <summary>
		/// Registrazione di una classe che, implementando
		/// l'interfaccia "IResourceReleaser", definisce
		/// la logica relativa al rilascio delle risorse
		/// </summary>
		/// <param name="releaser"></param>
		public static void Register(IResourceReleaser releaser)
		{
			if (!IsRegistered(releaser.GetType()))
			{
				ArrayList list=GetListReleaser();

				if (list==null)
				{
					list=new ArrayList();
					HttpContext.Current.Session.Add(SESSION_KEY,list);
				}

				list.Add(releaser);
			}
		}

		/// <summary>
		/// Verifica se la classe utilizzata per il rilascio delle risorse
		/// sia già stata registrata in sessione
		/// </summary>
		/// <param name="releaser"></param>
		public static bool IsRegistered(Type releaserType)
		{
			bool retValue=false;

			ArrayList list=GetListReleaser();
			
			if (list!=null)
			{
				foreach (IResourceReleaser item in list)
				{
					retValue=(item.GetType().Equals(releaserType));

					if (retValue)
						break;
				}
			}

			return retValue;
		}

		/// <summary>
		/// Rilascio di una risorsa registrata
		/// </summary>
		/// <param name="releaserType"></param>
		public static void Release(Type releaserType)
		{
			ArrayList list=GetListReleaser();

			if (list!=null)
			{
				IResourceReleaser itemToRemove=null;

				foreach (IResourceReleaser item in list)
				{
					if (item.GetType().Equals(releaserType))
					{
						item.ReleaseResources();
						itemToRemove=item;
						break;
					}
				}

				list.Remove(itemToRemove);
			}
		}

		/// <summary>
		/// Rilascio di tutte le risorse registrate
		/// </summary>
		public static void ReleaseAll()
		{
			ArrayList list=GetListReleaser();

			ArrayList itemsToRemove=new ArrayList();

			if (list!=null)
			{
				foreach (IResourceReleaser releaser in list)
				{
					releaser.ReleaseResources();
					itemsToRemove.Add(releaser);
				}

				foreach (IResourceReleaser releaser in itemsToRemove)
					list.Remove(releaser);
			}
		}

		/// <summary>
		/// Reperimento lista gestori delle risorse
		/// </summary>
		/// <returns></returns>
		private static ArrayList GetListReleaser()
		{
			return HttpContext.Current.Session[SESSION_KEY] as ArrayList;
		}
		
		private ResourceReleaser() {}
	}
}