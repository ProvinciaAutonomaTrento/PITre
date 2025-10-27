// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Search.RabbitMq;

public class Request
{
    public RequestOperationTypesEnum operationType { get; set; }
    public string idTenant { get; set; }
    public string idDocument { get; set; }
}
