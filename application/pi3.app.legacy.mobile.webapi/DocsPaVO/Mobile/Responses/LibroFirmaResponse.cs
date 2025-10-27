// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class LibroFirmaResponse
    {
        public List<LibroFirmaElement> Elements
        {
            get;
            set;
        }

        public LibroFirmaResponseCode Code
        {
            get;
            set;
        }

        public int TotalRecordCount
        {
            get;
            set;
        }

        public bool ModalitaFirmaParallela
        {
            get;
            set;
        }

        public static LibroFirmaResponse ErrorResponse
        {
            get
            {
                LibroFirmaResponse resp = new LibroFirmaResponse();
                resp.Code = LibroFirmaResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum LibroFirmaResponseCode
    {
        OK, SYSTEM_ERROR
    }
}
