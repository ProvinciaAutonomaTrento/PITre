// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.Fatturazione
{
    public class CampoCustomFattura
    {
        public TipoCampoCustomFatturaType Tipo { get; set; }

        public string NomeCampo { get; set; }

        public string NumeroLinea { get; set; }

        public string Valore { get; set; }
    }

    public enum TipoCampoCustomFatturaType
    {
        ITEM,
        HEADER
    }
}
