﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Requests
{
    public class DismettiDelegaRequest
    {
        public UserInfo UserInfo
        {
            get;
            set;
        }

        public string IdDelegante
        {
            get;
            set;
        }
    }
}
