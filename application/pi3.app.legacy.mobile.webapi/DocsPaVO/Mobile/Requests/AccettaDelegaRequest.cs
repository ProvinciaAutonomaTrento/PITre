// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class AccettaDelegaRequest
    {
        public DelegaInfo Delega
        {
            get; 
            set;
        }

        public UserInfo UserInfo
        {
            get; 
            set;
        }

        public string IpAddress
        {
            get; 
            set;
        }

        public string SessionId
        {
            get; 
            set; 
        }

    }
}
