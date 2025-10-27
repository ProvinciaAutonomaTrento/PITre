// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.ExportData;
using DocsPaVO.fascicolazione;
using DocsPaVO.filtri;
using DocsPaVO.Grid;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record ExportNotificationCenter(InfoUtente infoUtente, string tipologiaExport, string titolo, List<CampoSelezionato> objects, List<DocsPaVO.Notification.Notification> notifications) : IRequest<ExportNotificationCenterResult>;
    public record ExportNotificationCenterResult(DocsPaVO.documento.FileDocumento output);

}