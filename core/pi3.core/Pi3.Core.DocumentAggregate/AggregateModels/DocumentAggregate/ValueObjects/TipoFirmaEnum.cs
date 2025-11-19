// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.DocumentAggregate.ValueObjects
{
    public enum TipoFirmaEnum
    {
        [Description("N")] 
        Nessuna,
        [Description("E")]
        Elettronica,
        [Description("P")]
        Pades,
        [Description("C")]
        Cades,
        [Description("T")]
        Tsd,
        [Description("X")]
        Xades,
        [Description("PE")]
        PadesElettronica,
        [Description("CE")]
        CadesElettronica,
        [Description("TE")]
        TsdElettronica,
        [Description("XE")]
        XadesElettronica
    }
}
