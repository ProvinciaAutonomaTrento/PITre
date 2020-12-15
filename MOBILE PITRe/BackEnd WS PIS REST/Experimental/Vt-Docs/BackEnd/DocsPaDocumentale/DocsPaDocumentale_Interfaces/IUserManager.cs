using System;

namespace DocsPaDocumentale.Interfaces
{
	/// <summary>
	/// Interfaccia per la gestione degli utenti tramite un documentale
	/// </summary>
	public interface IUserManager
	{
		/// <summary>
		/// Effettua il login di un utente
		/// </summary>
		/// <param name="utente">Oggetto Utente connesso</param>
		/// <returns>True = OK; False = Si è verificato un errore</returns>
		bool LoginUser(DocsPaVO.utente.UserLogin userLogin, out DocsPaVO.utente.Utente utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult);

        /// <summary>
        /// Effettua il login di un utente amministratore
        /// </summary>
        /// <param name="forceLogin"></param>
        /// <param name="utente"></param>
        /// <param name="loginResult"></param>
        /// <returns></returns>
        bool LoginAdminUser(DocsPaVO.utente.UserLogin userLogin, bool forceLogin, out DocsPaVO.amministrazione.InfoUtenteAmministratore utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult);

        /// <summary>
        /// Modifica password utente
        /// </summary>
        /// <param name="user">
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// </param>
        ///// <returns></returns>
        DocsPaVO.Validations.ValidationResultInfo ChangeUserPwd(DocsPaVO.utente.UserLogin user, string oldPassword);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dst"></param>
        /// <returns></returns>
        bool LogoutUser(string dst);

        /// <summary>
        /// Reperimento del token di autenticazione per il superuser del documentale 
        /// </summary>
        /// <returns></returns>
        string GetSuperUserAuthenticationToken();

        bool Checkconnection();
    }
}