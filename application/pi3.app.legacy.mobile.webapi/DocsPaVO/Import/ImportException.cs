// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;

namespace DocsPaVO.Import
{
    /// <summary>
    /// Questa classe rappresenta un'eccezione lanciata da uno dei processi di importazione
    /// </summary>
    public class ImportException : ApplicationException
    {
        /// <summary>
        /// Metodo per l'inizializzazione di una nuova classe di eccezione generica 
        /// nell'importazione
        /// </summary>
        /// <param name="message">Il messaggio dell'eccezione</param>
        public ImportException(string message) : base(message) { }

    }
}
