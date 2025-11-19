// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DocsPaVO.FascicolazioneCartacea
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public class SnapshotDocumentiFascicolazione
    {
        /// <summary>
        /// ID immagine
        /// </summary>
        public int IdSnapshot = 0;

        /// <summary>
        /// Nome dell'immagine
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// Data di creazione immagine
        /// </summary>
        public DateTime CreationDate = DateTime.MinValue;

        /// <summary>
        /// Utente che ha creato l'immagine
        /// </summary>
        public string UserId = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public SnapshotDocumentiFascicolazione()
        {
        }
    }
}
