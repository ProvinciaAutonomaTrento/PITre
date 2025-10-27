// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocsPaVO.areaLavoro
{
    public class ResultAddAreaLavoro
    {
        public string idObject;
        public TipoOggetto tipoOggetto;
        public Esito esito;
    }

    public enum Esito
    {
        OK,
        DOCUMENTO_IN_AREA_LAVORO,
        FASCICOLO_IN_AREA_LAVORO,
        KO
    }
}
