// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections;
using System.Collections.Generic;

namespace DocsPaUtils.Logging;
public class Logger
{
    private LogItem _logItem = null;
    public static bool LoggingEnabled = true;
    public static TimeSpan logRunning = TimeSpan.FromSeconds(10);

    public Logger(string type, string name, string content)
    {
        _logItem = new LogItem
        {
            Type = type,
            Name = name,
            Content = content,
            Inizio = DateTime.Now
        };
    }

    public static void SetSize(int size) => LogList.SetSize(size);

    public static void SetLongRunning(TimeSpan time) => logRunning = time;

    public void Start()
    {
        if (!LoggingEnabled)
            return;

        _logItem.Inizio = DateTime.Now;

        LogList.Log(_logItem);
    }

    public void End(bool Errore = false)
    {
        if (!LoggingEnabled)
            return;

        _logItem.Errore = Errore;
        _logItem.Fine = DateTime.Now;
        _logItem.Durata = _logItem.Fine - _logItem.Inizio;

        _logItem.LongRunning = _logItem.Durata > logRunning;
    }

    public static IList<LogItem> GetLogs()
    {
        return LogList.GetLogs();
    }

    public static void ClearLogs()
    {
        LogList.Clear();
    }

}
