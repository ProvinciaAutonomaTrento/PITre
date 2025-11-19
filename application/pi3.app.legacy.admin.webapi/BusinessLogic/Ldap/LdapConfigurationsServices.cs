// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.Ldap;
using DocsPaVO.utente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Ldap
{
    public class LdapConfigurationsServices
    {
        public static LdapConfig GetLdapConfig(InfoUtente infoUtente, string idAmministrazione)
        {
            return DocsPaLdapServices.LdapConfigurations.GetLdapConfig(idAmministrazione);
        }

        public static void SaveLdapConfig(InfoUtente infoUtente, string idAmministrazione, LdapConfig ldapInfo)
        {
            DocsPaLdapServices.LdapConfigurations.SaveLdapConfig(idAmministrazione, ldapInfo);
        }


    }
}
