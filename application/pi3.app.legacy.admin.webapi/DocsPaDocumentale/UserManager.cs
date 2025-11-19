// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaDocumentale.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaDocumentale.Documentale
{
    public class UserManager : IUserManager
    {

        static UserManager()
        {
            _type = typeof(DocsPaDocumentale_ETDOCS.Documentale.UserManager);
        }

        /// <summary>
        /// 
        /// </summary>
        public UserManager()
        {
            this._instance = (IUserManager)Activator.CreateInstance(_type);
        }

        /// <summary>
        /// Tipo documentale corrente
        /// </summary> 
        private static Type _type = null;

        /// <summary>
        /// Oggetto documentale corrente
        /// </summary>
        private IUserManager _instance = null;

        /// <summary>
        /// Reperimento istanza oggetto "IUserManager"
        /// relativamente al documentale correntemente configurato
        /// </summary>
        protected IUserManager Instance
        {
            get
            {
                return this._instance;
            }
        }

        public bool LoginUser(DocsPaVO.utente.UserLogin userLogin, out DocsPaVO.utente.Utente utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            return this.Instance.LoginUser(userLogin, out utente, out loginResult);
        }

        public bool LoginAdminUser(DocsPaVO.utente.UserLogin userLogin, bool forceLogin, out DocsPaVO.amministrazione.InfoUtenteAmministratore utente, out DocsPaVO.utente.UserLogin.LoginResult loginResult)
        {
            return this.Instance.LoginAdminUser(userLogin, forceLogin, out utente, out loginResult);
        }

        public DocsPaVO.Validations.ValidationResultInfo ChangeUserPwd(DocsPaVO.utente.UserLogin user, string oldPassword)
        {
            return this.Instance.ChangeUserPwd(user, oldPassword);
        }

        public bool LogoutUser(string dst)
        {
            return this.Instance.LogoutUser(dst);
        }

        public string GetSuperUserAuthenticationToken()
        {
            return this.Instance.GetSuperUserAuthenticationToken();
        }

        public bool Checkconnection()
        {
            return this.Instance.Checkconnection();
        }

    }
}
