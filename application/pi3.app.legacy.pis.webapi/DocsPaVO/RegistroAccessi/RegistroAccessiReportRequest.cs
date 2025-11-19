// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.filtri;

namespace DocsPaVO.RegistroAccessi
{
    [Serializable()]
    public class RegistroAccessiReportRequest
    {
        public RequestType requestType { get; set; }

        public List<FiltroRicerca> filters { get; set; }

    }

    [Serializable()]
    public enum RequestType
    {
        EXPORT,
        PUBLISH
    }
}
