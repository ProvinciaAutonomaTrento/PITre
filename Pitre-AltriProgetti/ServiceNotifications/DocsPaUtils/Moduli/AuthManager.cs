using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaUtils.Moduli
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ModuliAuthManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public static bool IsModuloCentroServizi(string modulo)
        {
            if (string.IsNullOrEmpty(modulo))
                return false;
            else
                return (modulo.Trim().ToUpperInvariant() == ListaModuli.CENTRO_SERVIZI);
        }

        /// <summary>
        /// Verifica se, per il modulo richiesto, è necessario che l'utente sia censito in almeno un ruolo
        /// </summary>
        /// <param name="modulo"></param>
        /// <returns></returns>
        public static bool RolesRequired(string modulo)
        {
            bool required = true;

            if (!string.IsNullOrEmpty(modulo))
            {
                if (DocsPaUtils.Moduli.ModuliAuthManager.IsModuloCentroServizi(modulo))
                {
                    // Per l'accesso al modulo centro servizi, non è necessario che l'utente sia censito nei ruoli
                    //required = false;

                    //modifica 06-12-2012: senza ruolo l'utente non può inviare le notifiche. Quindi il ruolo diventa necessario.
                    //Questi metodi risulteranno obsoleti nel caso di modifica permanente.
                    required = true;
                }
            }
            
            return required;
        }
    }
}
