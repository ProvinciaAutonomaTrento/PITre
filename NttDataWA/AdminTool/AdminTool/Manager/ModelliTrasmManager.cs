using System;
using System.Collections;
using System.Web.SessionState;
using System.Web.UI;

namespace SAAdminTool.AdminTool.Manager
{

	
	/// <summary>
	/// Summary description for ModelliTrasmManager.
	/// </summary>
	public class ModelliTrasmManager
	{

		private static SAAdminTool.DocsPaWR.DocsPaWebService docsPaWS = ProxyManager.getWS();

		public ModelliTrasmManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static SAAdminTool.DocsPaWR.RagioneTrasmissione[] getlistRagioniTrasm(string idAmm,bool all,string tipo_trasm,bool sysExt = false)
		{
			try
			{            
    			DocsPaWR.TrasmissioneDiritti diritti = new SAAdminTool.DocsPaWR.TrasmissioneDiritti();
				diritti.accessRights = "-1";  //INDICA CHE LE RAGIONI DA PRENDERE SONO TUTTE
				diritti.idAmministrazione = idAmm;
				DocsPaWR.RagioneTrasmissione[] result;
				if(all==true) //sel tutta la AOO -- solo trasmissioni con cha_tipo_dest = 'T','P'
				{
					result = docsPaWS.TrasmissioneGetRagioniATutti(diritti);
				}
                else if (!sysExt)
                {
                    result = docsPaWS.TrasmissioneGetRagioni(diritti, false);
                }
                else
                {
                    result = docsPaWS.TrasmissioneGetRagioniSysExt(diritti);
                }
				if(result == null)
				{
					throw new Exception();
				}

                //se non siamo in amministrazione e l'utente corrente non può visualizzare le ragioni con diritto di cessione diritti, le elimino dal result delle rag. trasm.
                if (!string.IsNullOrEmpty(tipo_trasm))
                {
                    DocsPaWR.Ruolo role = SAAdminTool.UserManager.getRuolo();
                    DocsPaWR.Funzione[] funz = role.funzioni;
                    bool cedi_diritti = false;
                    string cedi_diritti_str = string.Empty;
                    if (tipo_trasm.Equals("D"))
                        cedi_diritti_str = "ABILITA_CEDI_DIRITTI_DOC";
                    else if (tipo_trasm.Equals("F"))
                        cedi_diritti_str = "ABILITA_CEDI_DIRITTI_FASC";
                    foreach (DocsPaWR.Funzione f in funz)//verifico se attiva la microfunz ABILITA_CEDI_DIRITTI_DOC/FASC
                    {
                        if (f.codice.Equals(cedi_diritti_str))
                        {
                            cedi_diritti = true;
                            break;
                        }
                    }
                    if (!cedi_diritti)//se l'utente non ha attiva la microfunz abilita_cedi_diritti_doc/fasc elimino le rag di trasm con cessione diritti dal result 
                    {
                        ArrayList r = new ArrayList();
                        ArrayList resultNotCessione = new ArrayList();
                        r.InsertRange(0, result);

                        foreach (DocsPaWR.RagioneTrasmissione res in r)
                        {
                            if (!res.prevedeCessione.Equals("R") && (!res.prevedeCessione.Equals("W")))
                                resultNotCessione.Add(res);
                        }
                        result = new DocsPaWR.RagioneTrasmissione[resultNotCessione.Count];
                        resultNotCessione.CopyTo(result);
                    }
                }
				return result;
			} 
			catch
			{
				return null;
			} 
		
			
		}

	}
}
