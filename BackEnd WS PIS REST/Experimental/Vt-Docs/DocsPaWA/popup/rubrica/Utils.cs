using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;

namespace DocsPAWA.popup.RubricaDocsPA
{
	/// <exclude></exclude>
	public class Utils
	{

		public class GZipFilter : MemoryStream
		{
			private Stream sink;

			public GZipFilter(Stream sink) : base()
			{
				this.sink = sink;
			}

			public override void Close() 
			{
				GZipOutputStream gzip = new GZipOutputStream(sink);
				WriteTo(gzip);
				gzip.Close();
				base.Close();
			}
		}

		public static string ER_Array_Serialize (DocsPAWA.DocsPaWR.ElementoRubrica[] ers)
		{
			string res = "";
            string rubCom = "";
			if (ers != null && ers.Length > 0)
				foreach (DocsPAWA.DocsPaWR.ElementoRubrica er in ers)
                    {
                        if(er.isRubricaComune && er.rubricaComune != null)
                        {
                            if (!string.IsNullOrEmpty(er.rubricaComune.IdRubricaComune.ToString()))
                                rubCom = er.rubricaComune.IdRubricaComune.ToString();
                            else
                                rubCom = "";
                        }
                        else
                            rubCom = "";
                    res += String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}\n", 
						er.codice,
						Convert.ToBase64String (Encoding.UTF8.GetBytes(er.descrizione)),
						er.has_children, 
						er.interno,
                        er.tipo,
                        er.systemId,
                            rubCom,
                            er.codiceRegistro);
                    }

			return (res != "") ? res.Substring (0, res.Length - 1).Replace ("\r","") : "";
		}

		public static DocsPAWA.DocsPaWR.ElementoRubrica[] ER_Array_Deserialize (string ers)
		{
			if (ers == null || ers == "")
				return new DocsPAWA.DocsPaWR.ElementoRubrica[0];
			
			string[] rows = System.Text.RegularExpressions.Regex.Split (ers, "\n");

			DocsPaWR.ElementoRubrica[] a_ers = new DocsPAWA.DocsPaWR.ElementoRubrica[rows.Length];
			for  (int n = 0; n < rows.Length; n++) 
			{
				string row = rows[n];
				a_ers[n] = new DocsPAWA.DocsPaWR.ElementoRubrica();
				string[] a_row = System.Text.RegularExpressions.Regex.Split (row, ":");
				if (a_row.Length == 8) 
				{
					a_ers[n].codice = a_row[0].Replace ("\r","");
					a_ers[n].descrizione = Encoding.UTF8.GetString(Convert.FromBase64String (a_row[1].Replace ("\r","")));
					a_ers[n].has_children = Convert.ToBoolean (a_row[2].Replace ("\r",""));
					a_ers[n].interno = Convert.ToBoolean (a_row[3].Replace ("\r",""));
					a_ers[n].tipo = a_row[4].Replace ("\r","");
                    a_ers[n].systemId = a_row[5].Replace("\r", "");
                    if ((a_row[6].Replace("\r", "")).ToUpper() == "")
                        a_ers[n].isRubricaComune = false;
                    else
                    {
                        a_ers[n].isRubricaComune = true;
                        a_ers[n].rubricaComune = new DocsPaWR.InfoElementoRubricaComune();
                        a_ers[n].rubricaComune.IdRubricaComune = Convert.ToInt32(a_row[6].Replace("\r", ""));
                    }
                    a_ers[n].codiceRegistro = a_row[7].Replace("\r", "");
				}
				else
				{
					a_ers[n].codice = "";
					a_ers[n].descrizione = "&nbsp;";
					a_ers[n].has_children = false;
					a_ers[n].interno = false;
					a_ers[n].tipo = "";
                    a_ers[n].systemId = "";
                    a_ers[n].isRubricaComune = false;
                    a_ers[n].codiceRegistro = "";
				}
			}
			return a_ers;
		}
	}

}
