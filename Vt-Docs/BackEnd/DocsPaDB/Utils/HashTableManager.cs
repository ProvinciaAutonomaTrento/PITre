using System;
using System.Collections;

namespace DocsPaDB.Utils
{
	/// <summary>
	/// </summary>
	public class HashTableManager
	{
		/// <summary>
		/// </summary>
		/// <param name="ht"></param>
		/// <param name="val"></param>
		/// <returns></returns>
		public static object GetKeyFromValue(Hashtable ht, string val)
		{
		   object res = null;
		   IEnumerator keys = ht.Keys.GetEnumerator();
			
			while(keys.MoveNext())
			{
				if(((string)ht[keys.Current]).Equals(val))
				{
				  res = keys.Current;
				}
			}

			return res;
		}
	}
}
