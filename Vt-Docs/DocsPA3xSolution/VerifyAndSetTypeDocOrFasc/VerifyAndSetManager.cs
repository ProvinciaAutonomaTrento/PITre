using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


namespace VerifyAndSetTypeDocOrFasc
{
    public class VerifyAndSetManager
    {
        private enum TipoVerifica { CDC, PCM }

        public static VerifyAndSetManager Instance
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["VerifyAndSetTypeDocOrFasc"]))
                {
                    string tipoVerifica = ConfigurationManager.AppSettings["VerifyAndSetTypeDocOrFasc"];

                    if (tipoVerifica.Equals(TipoVerifica.CDC.ToString()))
                        return new VerifyAndSetTypeDocOrFasc.VerifyAndSetCDC();
                    else if (tipoVerifica.Equals(TipoVerifica.PCM.ToString()))
                        return new VerifyAndSetTypeDocOrFasc.VerifyAndSetPCM();
                }

                return new VerifyAndSetManager();
            }
        }

        public virtual string verifyTipoDoc(DocsPaVO.utente.InfoUtente infoUtente, ref DocsPaVO.documento.SchedaDocumento schedaDocumento)
        {
            return string.Empty;
        }

        public virtual string verifyTipoFasc(DocsPaVO.utente.InfoUtente infoUtente, ref DocsPaVO.fascicolazione.Fascicolo fascicolo)
        {
            return string.Empty;
        }
    }
}
