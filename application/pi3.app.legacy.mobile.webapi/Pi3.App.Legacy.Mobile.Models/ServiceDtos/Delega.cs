// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.Models.ServiceDtos;
public class Delega
{
    public string? Id { get; set; }

    public string? IdRuoloDelegante { get; set; }

    public string? Delegante { get; set; }

    public string? CodiceDelegante { get; set; }

    public DateTime? DataDecorrenza { get; set; }

    public DateTime? DataScadenza { get; set; }

    public string? InEsercizio { get; set; }

    public StatoDelega Stato { get; set; }

    public string? DelegaStato { get; set; }

    public string? Delegato { get; set; }

    public string? IdDelegato { get; set; }

    public string? RuoloDelegato { get; set; }

    public string? RuoloDelegante { get; set; }

    public string? id_utente_delegante { get; set; }

    public string? IdRuoloDelegato { get; set; }


    public enum TipoDelega
    {
        ASSEGNATA, RICEVUTA, ESERCIZIO, TUTTI
    }

    public enum StatoDelega
    {
        ATTIVA, IMPOSTATA, SCADUTA, TUTTI
    }
}
