// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System.Runtime.Serialization;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents
{
    public class FollowRequest
    {
        /// <summary>
        /// Id oggetto che si intende monitorare/ non monitorare più
        /// </summary>
        public string IdObject
        {
            get;
            set;
        }

        /// <summary>
        /// Specifica il tipo di operazione:
        /// 0. Monitora il fascicolo
        /// 1. Non monitorare più il fascicolo
        /// 2. Monitora documento
        /// 3. Non monitorare più il documento
        /// </summary>
        public OperationFollow Operation
        {
            get;
            set;
        }

    }

    public enum OperationFollow
    {
        [EnumMember]
        AddFolder,
        [EnumMember]
        RemoveFolder,
        [EnumMember]
        AddDoc,
        [EnumMember]
        RemoveDoc
    }
}
