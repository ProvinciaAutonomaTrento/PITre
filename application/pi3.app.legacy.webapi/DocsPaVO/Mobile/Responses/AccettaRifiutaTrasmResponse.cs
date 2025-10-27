// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class AccettaRifiutaTrasmResponse
    {
        public AccettaRifiutaTrasmResponseCode Code
        {
            get;
            set;
        }

        public string Errore
        {
            get;
            set;
        }

        public static AccettaRifiutaTrasmResponse ErrorResponse
        {
            get
            {
                AccettaRifiutaTrasmResponse resp = new AccettaRifiutaTrasmResponse();
                resp.Code = AccettaRifiutaTrasmResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }

    }

    public enum AccettaRifiutaTrasmResponseCode
    {
        OK,SYSTEM_ERROR,BL_ERROR
    }
}
