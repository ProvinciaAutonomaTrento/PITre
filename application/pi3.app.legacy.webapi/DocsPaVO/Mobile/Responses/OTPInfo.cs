// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
namespace DocsPaVO.Mobile.Responses
{
    public class OTPInfo
    {
        private string idUtente;
        private string email;

        public OTPInfo(string idUtente, string email)
        {
            this.idUtente = idUtente;
            this.email = email;
        }

        public string Email { get { return this.email; }  }
        public string UserId { get { return this.idUtente; } }
    }
}