// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Mobile.Responses
{
    public class GetPreviewResponse
    {
        public FileInfo File
        {
            get;
            set;
        }

        public GetPreviewResponseCode Code
        {
            get;
            set;
        }

        public static GetPreviewResponse ErrorResponse
        {
            get
            {
                GetPreviewResponse resp = new GetPreviewResponse();
                resp.Code = GetPreviewResponseCode.SYSTEM_ERROR;
                return resp;
            }
        }
    }

    public enum GetPreviewResponseCode
    {
        OK,SYSTEM_ERROR,NO_PREVIEW_FOR_EXTENSION,PAGE_NOT_FOUND
    }
}
