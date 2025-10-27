// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using Pi3.App.Indexer.Application.Queries.Request;
using Pi3.App.Indexer.Infrastructure.Services;
using Pi3.Core.AggregateModels;
using Pi3.Core.AggregateModels.DocumentBlobAggregate;
using Pi3.Core.SeedWork;
using Pi3.Core.Services.Principal;
using Pi3.Core.Services.File.TextExtractors;
using Pi3.Infrastructure.Legacy.EF;
using Pi3.Infrastructure.Legacy.EF.Oracle;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pi3.App.Indexer;

await Runner.Run(args);