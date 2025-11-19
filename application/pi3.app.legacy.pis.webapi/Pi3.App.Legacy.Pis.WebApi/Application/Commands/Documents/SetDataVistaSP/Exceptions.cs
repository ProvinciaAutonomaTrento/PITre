// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.Pis.WebApi.Application.Commands.Documents.SetDataVistaSP
{
    internal class Exceptions
    {
    }

    public class NotifyNotFoundException : NotFoundPi3Exception
    {

        public NotifyNotFoundException(long systemIdTs, long idGroup, long idUser)
        : base(ErrorDescriptions.NotifyNotFound, null, ErrorDescriptions.ResourceManager, systemIdTs, idGroup, idUser)
        {
            this.SystemIdTs = systemIdTs;
            this.IdGroup = idGroup;
            this.IdUser = idUser;
        }

        public NotifyNotFoundException(long systemIdTs, long idUser)
        : base(ErrorDescriptions.NotifyNotFound_2, null, ErrorDescriptions.ResourceManager, systemIdTs, idUser)
        {
            this.SystemIdTs = systemIdTs;
            this.IdGroup = 0;
            this.IdUser = idUser;
        }


        public long SystemIdTs { get; init; }
        public long IdGroup { get; init; }
        public long IdUser { get; init; }
    }

   
    public class TrasmUtenteNotFoundException : NotFoundPi3Exception
    {

        public TrasmUtenteNotFoundException(long systemIdTs, long idGroup, long idUser)
        : base(ErrorDescriptions.TrasmUtenteNotFound, null, ErrorDescriptions.ResourceManager, systemIdTs, idGroup, idUser)
        {
            this.SystemIdTs = systemIdTs;
            this.IdGroup = idGroup;
            this.IdUser = idUser;
        }

        public long SystemIdTs { get; init; }
        public long IdGroup { get; init; }
        public long IdUser { get; init; }
    }

    public class ToDoListItemNotFountException : NotFoundPi3Exception
    {

        public ToDoListItemNotFountException(long systemIdTs, long idUser)
        : base(ErrorDescriptions.ToDoListItemNotFound, null, ErrorDescriptions.ResourceManager, systemIdTs, idUser)
        {
            this.SystemIdTs = systemIdTs;
            this.IdGroup = 0;
            this.IdUser = idUser;
        }

        public long SystemIdTs { get; init; }
        public long IdGroup { get; init; }
        public long IdUser { get; init; }
    }
}

