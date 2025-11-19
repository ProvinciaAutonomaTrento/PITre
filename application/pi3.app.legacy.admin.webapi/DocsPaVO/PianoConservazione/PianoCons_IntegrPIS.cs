// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsPaVO
{
    public class PianoCons_IntegrPIS
    {
        public string SystemId { get; set; }
        public string IdIntegrazione { get; set; }
        public string IdAmministrazione { get; set; }
        public string IdRegistro { get; set; }
        public string CodRegistro { get; set; }
        public string DescRegistro { get; set; }
        public string IdPeopleIntegrazione { get; set; }
        public string UserIdIntegrazione { get;set; }
        public string UtenteIntegrazione { get; set; }
        public string IdGruppoIntegrazione { get; set; }
        public string CodRuoloIntegrazione { get; set; }
        public string DescRuoloIntegrazione { get; set; }
        public string CodiceClassifica { get; set; }
        public string CodeApplication { get; set; }
        public string DescIntegrazione { get; set; }
        public string IdPianoConservazione { get; set; }
        public string DescPianoConservazione { get; set; }
        public string TempoConservazione { get; set; }
        public string TempoConservazioneInMesi { get; set; }
        public string IdTipoDoc { get; set; }
        public string DescTipoDoc { get; set; }
        public string IdTipoFasc { get; set; }
        public string DescTipoFasc { get; set; }
        public DateTime DataInserimento { get; set; }
    }
}
