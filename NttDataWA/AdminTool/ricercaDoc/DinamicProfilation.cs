using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;

namespace SAAdminTool.ricercaDoc
{
	/// <summary>
	/// Summary description for DinamicProfilation.
	/// </summary>
	public class DinamicProfilation
	{
		DinamicItem[] items = null;
		public DinamicProfilation()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public DinamicItem[] DinamicItems
		{
			get { return items; }
		}

		public DinamicItem GetItemByName(string name)
		{
			DinamicItem item = null;
			bool found = false;
			for (int i=0; !found && items!=null && i<items.Length; i++)
			{
				item = items[i];
				if (item.Name==name)
					found = true;
				else item = null;
			}
			return item;
		}

		public void ParseProfilationFilter(string filter)
		{
			try
			{
				//Step 1: Spezzare il filtro in tante righe
				//N.B. Si dà per scontata una struttura costante del filtro
				#region Step 1
				string aux = filter;
				ArrayList al = new ArrayList();
				//Tagliamo tutto ciò che sta prima dell'ultimo carattere #.
				//Supponiamo essercene 2. Dopo il secondo sono riportate le coppie
				//valore-id_oggetto dei parametri dinamici (su cui ci concentriamo)
				aux = aux.Substring(aux.LastIndexOf("#")+1).Trim();
				while (aux!=null && aux!="")
				{
					//Si individua una riga per ogni AND 
					int startId = aux.IndexOf("AND")+"AND".Length;
					startId = (startId!=-1) ? startId : 0;
					int endId = aux.IndexOf("AND",startId);
					endId = (endId!=-1) ? endId : aux.Length;
					string line = aux.Substring(startId,(endId-startId)).Trim();
					//al.Add(line.Replace(" ",""));
					aux = aux.Substring(endId);
				}
				#endregion Step 1

				//Step 2: Accoppiare i valori dei campi con id_oggetto
				//Analizzando le righe significative, si deve procedere ad individuare
				//opportunamente gli accoppiamenti e selezionare il valore di id_oggetto
				//e il valore del campo
				#region Step 2
				ArrayList al2 = new ArrayList();
				foreach (string s in al)
				{
					//Tutte le righe iniziano con un generico ID seguito da un punto (.)
					//che possiamo sfruttare per controllare il corretto accoppiamento
					//dei valori
					int ptIndex = s.IndexOf(".");
					string id = s.Substring(0,ptIndex);

					//Preleviamo il resto della stringa
					string val = s.Substring(ptIndex);
					if (val.IndexOf(".Valore_Oggetto_Db")!=-1)
					{
						//Se la stringa contiene ".Valore_Oggetto_Db", vuol dire che riporta 
						//il valore del campo
						DinamicItem item = new DinamicItem();
						int i = 0;
						int f = 0;
						if ((i=val.LastIndexOf("'%"))!=-1)
						{
							//potrebbe trattarsi di una like su un valore di testo...
							i = i + "'%".Length;
							f = val.LastIndexOf("%'");
							val = val.Substring(i,(f-i));
							item.Value = val;
						}
						if ((i=val.LastIndexOf("='"))!=-1)
						{
							//oppure del valore di un qualsiasi altro tipo di campo
							i = i + "='".Length;
							f = val.LastIndexOf("'");
							val = val.Substring(i,(f-i));
							item.Value = val;
						}
						al2.Add(item);
					}
					else if (val.IndexOf(".ID_OGGETTO=")!=-1)
					{
						//Se la stringa contiene ".ID_OGGETTO=", vuol dire che riporta 
						//il valore dell'id dell'oggetto corrispondente
						bool found = false;
						DinamicItem item = null;
						for (int j=0; !found && j<al2.Count; j++)
						{
							item = (DinamicItem)al2[j];
							if (item.Id==id)
								found = true;
						}
						if (item!=null)
						{
							int i = 0;
							//int f = 0;
							if ((i=val.LastIndexOf(".ID_OGGETTO="))!=-1)
							{
								i = i + ".ID_OGGETTO=".Length;
								val = val.Substring(i);
								item.IdOggetto = val;
							}
						}
					}
				}
				items = new DinamicItem[al2.Count];
				al2.CopyTo(items);
				#endregion Step 2


				//Step 3: Recuperare nome e tipo dell'oggetto
				//Con opportune query al database (vedi seguito) occorre individuare
				//il nome dell'oggetto e il tipo.
				//	select oc.system_id, oc.descrizione, ot.descrizione
				//	from dpa_oggetti_custom oc, dpa_tipo_oggetto ot
				//	where oc.system_id = #idOggetto
				//	and oc.id_tipo_oggetto = ot.system_id
				#region Step 3
				try
				{
					DocsPaWR.DocsPaWebService docspaws = ProxyManager.getWS();
					foreach (DinamicItem item in items)
					{
						DocsPaWR.OggettoCustom oggetto = docspaws.getOggettoById(item.IdOggetto);
						item.ItemType = (DinamicItem.ItemTypes)System.Enum.Parse(typeof(DinamicItem.ItemTypes),oggetto.TIPO.DESCRIZIONE_TIPO,true);
						item.Name = oggetto.DESCRIZIONE;
					}
				}
				catch
				{
				}
				#endregion Step 3
			}
			catch(Exception ex)
			{
				Console.WriteLine("Fallito il parsing dei filtri di profilazione dinamica: "+ex.Message);
			}
		}

		public class DinamicItem
		{
			public enum ItemTypes {CampoDiTesto, CasellaDiSelezione, SelezioneEsclusiva, MenuATendina, Contatore, Data}

			string id = null;
			string name = null;
			string val = null;
			string idOggetto = null;
			ItemTypes itemtype = ItemTypes.CampoDiTesto;

			public DinamicItem()
			{
				//
				// TODO: Add constructor logic here
				//
			}

			public string Id
			{
				get { return id; } set { id = value; }
			}

			public string Name
			{
				get { return name; } set { name = value; }
			}

			public string Value
			{
				get { return val; } set { val = value; }
			}

			public string IdOggetto
			{
				get { return idOggetto; } set { idOggetto = value; }
			}

			public ItemTypes ItemType
			{
				get { return itemtype; } set { itemtype = value; }
			}
		}
	}
}
