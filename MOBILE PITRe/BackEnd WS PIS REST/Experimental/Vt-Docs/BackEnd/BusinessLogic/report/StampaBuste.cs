using System;
using System.Configuration;
using System.Collections;
using System.Data;
using System.IO;
using log4net;

namespace BusinessLogic.Report
{
	/// <summary>
	/// Summary description for StampaBuste.
	/// </summary>
	public class StampaBuste
	{
        private static ILog logger = LogManager.GetLogger(typeof(StampaBuste));

			public StampaBuste()
			{
				//
				// TODO: Add constructor logic here
				//
			}
		
			/// <summary>
			/// 
			/// </summary>
			/// <param name="infoUtente"></param>
			/// <param name="objRuolo"></param>
			/// <param name="registro"></param>
			/// <param name="filters"></param>
			/// <returns></returns>
			public static DocsPaVO.documento.FileDocumento StampaBusteWithFilters(
				DocsPaVO.utente.InfoUtente infoUtente, 
				DocsPaVO.utente.Ruolo objRuolo, 
				DocsPaVO.utente.Registro registro,
				DocsPaVO.filtri.FiltroRicerca[][] filters)
			{			
				logger.Debug("StampaBusteWithFilters");
				DocsPaVO.documento.FileDocumento fileDoc = new DocsPaVO.documento.FileDocumento();
				try
				{
					// Recupero documenti da stampare
					DataTable tableProfile = (getDestProtUscitaWithFilters(filters, registro)).Tables[0];
					if(tableProfile.Rows.Count>0)//controllo se ritorsa un dataset pieno
					{
						StampaPDF.StampaBustePdf.StampaBustePdf stampaFile = new StampaPDF.StampaBustePdf.StampaBustePdf();
				
						System.IO.FileStream fs=stampaFile.GeneraBustaPdf(tableProfile,infoUtente.userId);

						fileDoc = GeneraFileDoc(fs);
						return fileDoc;
					}
					else
					{
						return null;
					}
				}
				catch(Exception ex)
				{
					throw ex;
					return null;
				}
			}

			private static DataSet getDestProtUscitaWithFilters(DocsPaVO.filtri.FiltroRicerca[][] filters, DocsPaVO.utente.Registro registro)
			{	
				DataSet ds = new DataSet();
				DocsPaDB.Query_DocsPAWS.Report report = new DocsPaDB.Query_DocsPAWS.Report();
				report.getDestProtUscitaWithFilters(out ds, filters, registro);
				return ds;
			}

			private static DocsPaVO.documento.FileDocumento GeneraFileDoc(System.IO.FileStream fs)
			{
				
				DocsPaVO.documento.FileDocumento doc = new DocsPaVO.documento.FileDocumento();
				try
				{
					doc.name = "StampaBusta.pdf";
					doc.path = AppDomain.CurrentDomain.BaseDirectory + "Report/busta";
					doc.fullName = "StampaBusta";
					doc.contentType = "application/pdf";
				
					Byte[] byteArray = new byte[fs.Length];
					fs.Read(byteArray,0,(int)fs.Length);

					doc.content = byteArray;
					doc.length =(int) fs.Length;
					fs.Close();
				}
				catch(Exception ex)
				{
					if(fs!=null)
					{
						fs.Close();
					}
					throw ex;
				}

				return doc;
			
			}

		}
	
}
