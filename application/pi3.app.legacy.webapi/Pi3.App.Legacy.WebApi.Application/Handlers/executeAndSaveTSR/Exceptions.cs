// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.executeAndSaveTSR
{
    public class InstanceTypeNotFoundPi3Exception : NotFoundPi3Exception
    {
        public InstanceTypeNotFoundPi3Exception(string typeName)
            : base(ErrorDescription.InstanceTypeNotFound, null, Resources.ResourceManager, typeName)
        {
            this.TypeName = typeName;
        }

        public string TypeName { get; init; }

    }
    public class TimestampNullPi3Exception : Pi3Exception
    {
        public TimestampNullPi3Exception()
            : base(ErrorDescription.TimestampNull, null, Resources.ResourceManager)
        {
            
        }

    }
    public class FileRequestNullPi3Exception : Pi3Exception
    {
        public FileRequestNullPi3Exception()
            : base(ErrorDescription.FileRequestNull, null, Resources.ResourceManager)
        {
            
        }

    }
}
