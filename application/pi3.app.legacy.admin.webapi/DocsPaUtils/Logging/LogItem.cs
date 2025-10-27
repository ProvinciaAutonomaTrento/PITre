// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;

namespace DocsPaUtils.Logging;

public class LogItem
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public DateTime? Inizio { get; set; }
    public DateTime? Fine { get; set; }
    public TimeSpan? Durata { get; set; }
    public bool Errore { get; set; }
    public bool LongRunning { get; set; }
}
