// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO.documento
{
    public enum ResultSigilloElettronico
    {
        OK,
        SYSTEM_ERROR,
        SERVICE_UNAVAILABLE,
        FILE_NON_ACQUISITO,
        DATI_DI_FIRMA_ERRATI,
        FORMATO_FILE_NON_VALIDO
    }
}
