using System;
using System.Collections;
using System.Xml;

namespace DocsPaDbManagement.Functions
{
	/// <summary>
	/// Classe per la gestione delle mappature tableName - sequenceName per Oracle
	/// </summary>
	public class OracleSequenceMapper : DocsPaUtils.Interfaces.DbManagement.ISequenceMapper
	{
		/// <summary>
		/// Nome del file xml
		/// </summary>
		private const string ORACLE_SEQUENCE_MAPS_FILE="oracleSequenceMaps.xml";
		
		/// <summary>
		/// Attributi gestiti dal file xml
		/// </summary>
		private const string MAP_NODE="Map";
		private const string TABLE_NAME_ATTRIBUTE="tableName";
		private const string SEQUENCE_NAME_ATTRIBUTE="sequenceName";

		/// <summary>
		/// Hashtable contenente le mappature tableName - sequenceName
		/// </summary>
		private static Hashtable _sequenceMaps=Hashtable.Synchronized(new Hashtable());

		public OracleSequenceMapper()
		{
		}

		/// <summary>
		/// Reperimento SequenceName
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public string GetSequenceName(string tableName)
		{
			if (!this.IsInitialized())
				this.Initialize();

			string sequenceName=string.Empty;

			if (_sequenceMaps.ContainsKey(tableName))
				sequenceName=_sequenceMaps[tableName].ToString();
	
			return sequenceName;
		}

		/// <summary>
		/// Verifica se l'hashtable contenente le mappature è stata inizializzata
		/// </summary>
		/// <returns></returns>
		private bool IsInitialized()
		{
			return (_sequenceMaps.Count>0);
		}

		/// <summary>
		/// Reperimento percorso del file xml contenente
		/// le mappature tableName - SequenceName
		/// </summary>
		/// <returns></returns>
		private string GetFilePath()
		{
			return string.Concat(AppDomain.CurrentDomain.BaseDirectory,@"/xml/",ORACLE_SEQUENCE_MAPS_FILE);
		}

		/// <summary>
		/// Inizializzazione hashtable contenente le mappature tableName - SequenceName
		/// </summary>
		private void Initialize()
		{
			// Reperimento percorso del file xml contenente
			// le mappature tableName - SequenceName
			string filePath=this.GetFilePath();

			if (System.IO.File.Exists(filePath))
			{
				string tableName=string.Empty;
				string sequenceName=string.Empty;

				XmlTextReader reader=new XmlTextReader(filePath);

				try
				{
					while (reader.Read())
					{
						if (reader.IsStartElement() && reader.Name.Equals(MAP_NODE))
						{
							tableName=reader.GetAttribute(TABLE_NAME_ATTRIBUTE);
							sequenceName=reader.GetAttribute(SEQUENCE_NAME_ATTRIBUTE);

							if (tableName!=null &&	tableName!=string.Empty && !_sequenceMaps.ContainsKey(tableName))
								_sequenceMaps.Add(tableName,sequenceName);
						}
					}
				}
				catch
				{
				}
				finally
				{
					reader.Close();
					reader=null;
				}
			}
			else
			{
				throw new ApplicationException(string.Concat("File '",ORACLE_SEQUENCE_MAPS_FILE,"' non presente"));
			}
		}
	}
}
