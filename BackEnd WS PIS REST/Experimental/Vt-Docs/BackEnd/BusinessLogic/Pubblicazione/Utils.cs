using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Pubblicazione
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    internal sealed class Utils
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documento"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.InfoUtente ImpersonateAuthor(DocsPaVO.Pubblicazione.DocumentoDaPubblicare documento)
        {
            // Reperimento oggetto utente richiedente
            DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.UserManager.getUtente(documento.UtenteCreatore);

            if (utente == null)
                throw new ApplicationException(string.Format("Utente {0} non trovato", documento.UtenteCreatore));

            DocsPaVO.utente.Ruolo ruolo = BusinessLogic.Utenti.UserManager.getRuolo(documento.RuoloCreatore);

            if (ruolo == null)
                throw new ApplicationException(string.Format("Ruolo {0} non trovato", documento.RuoloCreatore));

            DocsPaVO.utente.InfoUtente infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);

            // Reperimento token superutente
            infoUtente.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

            return infoUtente;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="loginInfo"></param>
        ///// <returns></returns>
        //public static DocsPaVO.utente.InfoUtente Login(DocsPaVO.Pubblicazione.LoginInfo loginInfo)
        //{
        //    DocsPaVO.utente.InfoUtente infoUtente = null;

        //    DocsPaVO.utente.UserLogin login = new DocsPaVO.utente.UserLogin { UserName = loginInfo.UserName, Password = loginInfo.Password };

        //    DocsPaVO.utente.UserLogin.LoginResult result;
        //    string ipAddress;

        //    DocsPaVO.utente.Utente utente = BusinessLogic.Utenti.Login.loginMethod(login, out result, true, string.Empty, out ipAddress);

        //    if (result == DocsPaVO.utente.UserLogin.LoginResult.OK)
        //    {
        //        DocsPaVO.utente.Ruolo[] ruoli = (DocsPaVO.utente.Ruolo[]) utente.ruoli.ToArray(typeof(DocsPaVO.utente.Ruolo));
                
        //        DocsPaVO.utente.Ruolo ruolo = null;
        //        // Reperimento ruolo richiesto
        //        for (int i = 0; i < ruoli.Length; i++)
        //        {
        //            if (ruoli[i].codiceRubrica.ToUpper() == loginInfo.CodiceRuolo.ToUpper())
        //            {
        //                ruolo = ruoli[i];
        //                break;
        //            }
                    
        //        }
        //            if (ruolo == null)
        //                throw new ApplicationException(string.Format("L'utente non risulta inserito nel ruolo '{0}'", loginInfo.CodiceRuolo));

        //            infoUtente = new DocsPaVO.utente.InfoUtente(utente, ruolo);
                
        //    }
            
        //    return infoUtente;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="infoUtente"></param>
        //public static void Logoff(DocsPaVO.utente.InfoUtente infoUtente)
        //{
        //    BusinessLogic.Utenti.Login.logoff(infoUtente.userId, infoUtente.idAmministrazione, infoUtente.dst);
        //}
    }
}
