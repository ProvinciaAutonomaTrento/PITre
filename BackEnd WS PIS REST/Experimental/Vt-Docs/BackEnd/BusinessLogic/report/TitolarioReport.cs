using System;
using System.Configuration;
using log4net;

namespace BusinessLogic.Report
{
	/// <summary></summary>
	public class TitolarioReport
	{
        private static ILog logger = LogManager.GetLogger(typeof(TitolarioReport));
		/// <summary></summary>
		/// <param name="classificazioni"></param>
		/// <param name="infoUtente"></param>
		/// <returns></returns>
		public static DocsPaVO.documento.FileDocumento getTitolarioReport(string idAmministrazione, string idGruppo, string idPeople, System.Collections.ArrayList classificazioni)
		{
			DocsPaVO.documento.FileDocumento result=new DocsPaVO.documento.FileDocumento();
			//string path = ReportUtils.getPathName();
			//string path = DocsPaUtils.Functions.Functions.GetReportsPath();
			string path = AppDomain.CurrentDomain.BaseDirectory ;
			try
			{
				string headerString=ReportUtils.stringFile(path+"report\\titolario\\headerTitolario.txt");
				logger.Debug("Generazione report"); 
				System.DateTime data=System.DateTime.Now;
				System.Globalization.CultureInfo ci=new System.Globalization.CultureInfo("it-IT");
				string[] formats=data.GetDateTimeFormats(ci);
				string dataString=formats[5];
				headerString=headerString.Replace("XDATA",dataString);
				//string titolo=ConfigurationManager.AppSettings["intestazioneTitolario"];
				string titolo="Titolario"; // prima lo prendeva dal web.config, ma non esiste più
				if(titolo!=null)
				{
					headerString=headerString.Replace("XTITOLO",titolo);
				}
				else{
				   headerString=headerString.Replace("XTITOLO","");
				}
				System.Collections.ArrayList report=new System.Collections.ArrayList();
				ReportUtils.addStringToReport(headerString,ref report);
				logger.Debug("Inserimento voci di titolario");
				for(int i=0;i<classificazioni.Count;i++)
				{
					DocsPaVO.fascicolazione.Classificazione classificazione=(DocsPaVO.fascicolazione.Classificazione) classificazioni[i];
                    string headerTableString=ReportUtils.stringFile(path+"report\\titolario\\headerTableTitolario.txt");
					headerTableString=headerTableString.Replace("XPRIMO_CODICE",classificazione.codice);
					headerTableString=headerTableString.Replace("XPRIMA_DESCR",classificazione.descrizione);
					ReportUtils.addStringToReport(headerTableString,ref report);

					for(int j=0;j<classificazione.childs.Count;j++)
					{
						buildClassificazione(idAmministrazione, idGruppo, idPeople, (DocsPaVO.fascicolazione.Classificazione) classificazione.childs[j],0,ref report,path);
					}

					//inserimento fascicoli proc
					//addFascProc(idAmministrazione, idGruppo, idPeople, classificazione,0,ref report,path);

					string bottomTableString=ReportUtils.stringFile(path+"report\\titolario\\bottomTableTitolario.txt");
					ReportUtils.addStringToReport(bottomTableString,ref report);
				}

				logger.Debug("Inserimento bottom");
				string bottomString=ReportUtils.stringFile(path+"report\\titolario\\bottomTitolario.txt");
                ReportUtils.addStringToReport(bottomString,ref report);
				logger.Debug("Generazione fileDocumento");
				result.content=(byte[]) report.ToArray(typeof(byte));
				result.length=result.content.Length;
				result.contentType="application/rtf";
				result.name="report.rtf";
				logger.Debug("fileDocumento generato");
			}
			catch(Exception e)
			{
				logger.Debug("Errore nella gestione di Report (getTitolarioReport)",e);
				throw e;
			}
			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="classificazione"></param>
		/// <param name="infoUtente"></param>
		/// <param name="pos"></param>
		/// <param name="report"></param>
		/// <param name="path"></param>
		private static void buildClassificazione(string idAmministrazione, string idGruppo, string idPeople, DocsPaVO.fascicolazione.Classificazione classificazione, int pos,ref System.Collections.ArrayList report,string path)
		{
			string voceTitolario=ReportUtils.stringFile(path+"report\\titolario\\voceTitolario.txt");
		   int firstPos=100*classificazione.codice.Length;
		   int secondPos=pos+100*classificazione.codice.Length;

		   voceTitolario=voceTitolario.Replace("XFIRSTPOS",""+firstPos);
		   voceTitolario=voceTitolario.Replace("XPOS",""+secondPos);
		   string testo=classificazione.codice+" "+classificazione.descrizione;
		   voceTitolario=voceTitolario.Replace("XTESTO_VOCE",testo);
		   ReportUtils.addStringToReport(voceTitolario,ref report);

			for(int i=0;i<classificazione.childs.Count;i++){
			   buildClassificazione(idAmministrazione, idGruppo, idPeople,(DocsPaVO.fascicolazione.Classificazione) classificazione.childs[i],secondPos,ref report,path);
			}
			//parte relativa ai fascicoli proc.
			//addFascProc(idAmministrazione, idGruppo, idPeople, classificazione,firstPos,ref report,path);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="classificazione"></param>
		/// <param name="infoUtente"></param>
		/// <param name="pos"></param>
		/// <param name="report"></param>
		/// <param name="path"></param>
		private static void addFascProc(DocsPaVO.utente.InfoUtente infoUtente, DocsPaVO.fascicolazione.Classificazione classificazione,int pos,ref System.Collections.ArrayList report,string path)
		{
			logger.Debug("addFascProc");
			DocsPaVO.filtri.FiltroRicerca filtro=new DocsPaVO.filtri.FiltroRicerca();
			filtro.argomento=DocsPaVO.filtri.fascicolazione.listaArgomenti.TIPO_FASCICOLO.ToString();
			filtro.valore="P";
			DocsPaVO.filtri.FiltroRicerca[] filtri={filtro};
            System.Collections.ArrayList fascicoliProc = BusinessLogic.Fascicoli.FascicoloManager.getListaFascicoli(infoUtente, classificazione, filtri, false, false, false, null, "");
			logger.Debug("Fascicoli procedimentali trovati: "+fascicoliProc.Count);
			for(int k=0;k<fascicoliProc.Count;k++)
			{
				DocsPaVO.fascicolazione.Fascicolo fasc=(DocsPaVO.fascicolazione.Fascicolo) fascicoliProc[k];
				string voceFasc=ReportUtils.stringFile(path+"report\\titolario\\voceProc.txt");
				int firstPos=100*fasc.codice.Length;
				int secondPos=pos+100*fasc.codice.Length;
				voceFasc=voceFasc.Replace("XFIRSTPOS",""+firstPos);
				voceFasc=voceFasc.Replace("XPOS",""+secondPos);
				string testoFasc=fasc.codice+" "+fasc.descrizione;
				voceFasc=voceFasc.Replace("XTESTO_VOCE",testoFasc);
				ReportUtils.addStringToReport(voceFasc,ref report);
			}
		}
		
	}
}
