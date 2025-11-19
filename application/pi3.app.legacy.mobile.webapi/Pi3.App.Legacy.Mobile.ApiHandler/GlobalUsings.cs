// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using Microsoft.Extensions.Logging;

global using QUERY = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Queries;
global using RESPONSE = Pi3.App.Legacy.Mobile.ApiHandler.Models.QueryResponses;
global using COMMAND = Pi3.App.Legacy.Mobile.ApiHandler.Handlers.Commands;
global using RESULT = Pi3.App.Legacy.Mobile.ApiHandler.Models.CommandResults;
global using DTO = Pi3.App.Legacy.Mobile.ApiHandler.Models.DTOs;

global using EXCEPTIONS = Pi3.App.Legacy.Mobile.Shared.Exceptions;

global using MODELS = Pi3.App.Legacy.Mobile.Models;
global using SERVICE_DTO = Pi3.App.Legacy.Mobile.Models.ServiceDtos;
global using SERVICE_REQUESTS = Pi3.App.Legacy.Mobile.Models.ServiceRequests;