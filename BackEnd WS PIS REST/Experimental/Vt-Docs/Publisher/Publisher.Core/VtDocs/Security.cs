using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Publisher.VtDocs
{
    /// <summary>
    /// Classe per la gestione dell'accesso al sistema documentale
    /// </summary>
    public sealed class Security
    {
        /// <summary>
        /// Impersonificazione dell'utente del sistema documentale che ha generato l'evento
        /// </summary>
        /// <remarks>
        /// L'impersonificazione è necessaria in quanto, per reperire l'oggetto dal sistema documentale,
        /// è necessario utilizzare i diritti di visibilità dell'autore
        /// </remarks>
        /// <param name="logInfo"></param>
        /// <returns></returns>
        public static DocsPaVO.utente.InfoUtente ImpersonateUser(LogInfo logInfo)
        {
            try
            {
                if (_state == null)
                    _state = new Dictionary<string, DocsPaVO.utente.InfoUtente>();

                string key = string.Format("{0}_{1}_{2}", logInfo.IdAdmin, logInfo.IdUser, logInfo.IdRole);

                if (_state.ContainsKey(key))
                {
                    return (DocsPaVO.utente.InfoUtente)_state[key];
                }
                else
                {
                    // Reperimento oggetto utente richiedente
                    DocsPaVO.utente.Utente user = BusinessLogic.Utenti.UserManager.getUtente(logInfo.UserName, logInfo.IdAdmin.ToString());

                    if (user == null)
                        throw new PublisherException(ErrorCodes.USER_NOT_FOUND, string.Format(ErrorDescriptions.USER_NOT_FOUND, logInfo.UserName));

                    DocsPaVO.utente.Ruolo role = BusinessLogic.Utenti.UserManager.getRuoloByIdGruppo(logInfo.IdRole.ToString());

                    if (role == null)
                        throw new PublisherException(ErrorCodes.ROLE_NOT_FOUND, string.Format(ErrorDescriptions.ROLE_NOT_FOUND, logInfo.RoleCode));

                    DocsPaVO.utente.InfoUtente userInfo = new DocsPaVO.utente.InfoUtente(user, role);

                    // Reperimento token superutente
                    userInfo.dst = BusinessLogic.Utenti.UserManager.getSuperUserAuthenticationToken();

                    lock (_state)
                    {
                        _state[key] = userInfo;
                    }

                    return userInfo;
                }
            }
            catch (PublisherException pubEx)
            {
                throw pubEx;
            }
            catch (Exception ex)
            {
                throw new PublisherException(ErrorCodes.UNHANDLED_ERROR, ex.Message);
            }
        }

        /// <summary>
        /// Dictionary per effettuare il caching degli oggetti di autenticazione
        /// </summary>
        private static Dictionary<string, DocsPaVO.utente.InfoUtente> _state = null;
    }
}
