// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;

namespace DocsPaUtils.Logging;

public class LogList
{
    private static readonly IList<LogItem> _logger = new List<LogItem>();
    private static readonly object _lock = new object();

    private static int logSize = 1000;

    public static void SetSize(int size) {
        logSize = size;

        while (_logger.Count > logSize)
            _logger.RemoveAt(0);
    }

    public static void Log(LogItem logItem)
    {
        lock (_lock)
        {
            while (_logger.Count > logSize)
                _logger.RemoveAt(0);
            _logger.Add(logItem);
        }
    }

    public static void Clear()
    {
        lock (_lock)
        {
            _logger.Clear();
        }
    }
    public static IList<LogItem> GetLogs()
    {
        lock (_lock)
        {
            return _logger;
        }
    }

}
