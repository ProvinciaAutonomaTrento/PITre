// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Pi3.Core.AggregateModels.ContentElementAggregate;
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.AggregateModels.ContentElementAggregate
{
    public class ContentElementPermission : ValueObject
    {
        #region Public Members

        public ContentElementPermission()
        {
        }

        public ContentElementPermission(string? idMember, string? memberName, ContentElementMemberTypesEnum? memberType, ContentElementRightTypesEnum rightType, ContentElementPermissionTypesEnum permissionType)
        {
            IdMember = idMember;
            MemberName = memberName;
            MemberType = memberType;
            RightType = rightType;
            PermissionType = permissionType;
        }

        public string? IdMember
        {
            get;
            protected set;
        }

        public string? MemberName
        {
            get;
            protected set;
        }

        public ContentElementMemberTypesEnum? MemberType
        {
            get;
            protected set;
        }

        public ContentElementRightTypesEnum RightType
        {
            get;
            protected set;
        }

        public ContentElementPermissionTypesEnum PermissionType
        {
            get;
            protected set;
        }

        #endregion

        #region Private Members

        #endregion
    }
}
