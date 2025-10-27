// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class TrasmDiagrEntity
    {
        public long SYSTEM_ID  {get;set;}
        public long ID_TRASM   {get;set;}
        public long? DOC_NUMBER {get;set;}
        public long ID_STATO   {get;set;}
        public long? ID_PROJECT { get; set;}
    }
}
