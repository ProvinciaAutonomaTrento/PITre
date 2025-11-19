// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DocsPaVO.Settings;

public class HeaderValue
{
    private static HeaderValue _instance;
    private static HeaderValue _addressBookAuthToken;
    private static readonly object _lock = new object();

    public AsyncLocal<string> ConnectionString { get; set; } = new AsyncLocal<string>();
    public AsyncLocal<string> ConnectionName { get; set; } = new AsyncLocal<string>();
    public AsyncLocal<string> Token { get; set; } = new AsyncLocal<string>();

    public static HeaderValue Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new HeaderValue();
                }
                return _instance;
            }
        }
    } public static HeaderValue AddressBookAuthToken
    {
        get
        {
            lock (_lock)
            {
                if (_addressBookAuthToken == null)
                {
                    _addressBookAuthToken = new HeaderValue();
                }
                return _addressBookAuthToken;
            }
        }
    }
}