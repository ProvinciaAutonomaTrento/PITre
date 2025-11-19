// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs;

public class Trasmissione
{
    public string? IdTrasm { get; set; }

    public string? IdTrasmUtente { get; set; }

    public string? NoteGenerali { get; set; }

    public string? Mittente { get; set; }

    public DateTime? Data { get; set; }

    public string? Ragione { get; set; }

    public string? NoteIndividuali { get; set; }

    public bool HasWorkflow { get; set; }

    public bool Accettata { get; set; }

    public bool Rifiutata { get; set; }
}
