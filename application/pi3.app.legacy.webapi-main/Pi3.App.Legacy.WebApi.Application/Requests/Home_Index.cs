// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using DocsPaVO.amministrazione;
using DocsPaVO.Notification;
using DocsPaVO.Smistamento;
using DocsPaVO.utente;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Requests
{
    public record getInfoDisservizioResult(Disservizio output);

    public record getInfoDisservizio() : IRequest<getInfoDisservizioResult>;
    public record GetDocumentByNotificationsFiltersResult(List<string> output);

    public record GetDocumentByNotificationsFilters(string[] idObject, DocsPaVO.Notification.NotificationsFilters filter) : IRequest<GetDocumentByNotificationsFiltersResult>;
    public record GetIdTransmPendingByNotificationsFiltersResult(List<string> output);

    public record GetIdTransmPendingByNotificationsFilters(string[] idSpecializedObject, string idPeople) : IRequest<GetIdTransmPendingByNotificationsFiltersResult>;
    public record GetNoticeDaysNotificationResult(string output);

    public record GetNoticeDaysNotification(string idAmm) : IRequest<GetNoticeDaysNotificationResult>;
    public record RemoveNotificationsResult(bool output);

    public record RemoveNotifications(DocsPaVO.Notification.Notification[] notifications, DocsPaVO.utente.InfoUtente infoUtente) : IRequest<RemoveNotificationsResult>;
    public record GetNumberNotifyOtherRolesResult(int output);

    public record GetNumberNotifyOtherRoles(string idPeople, string idGroup) : IRequest<GetNumberNotifyOtherRolesResult>;

    public record ReadNotificationsResult(DocsPaVO.Notification.Notification[] Output);
    public record ReadNotifications(string idPeople, string idGroup) : IRequest<ReadNotificationsResult>;

    public record ChangeStateReadNotificationResult(bool output);

    public record ChangeStateReadNotification(DocsPaVO.Notification.Notification notify, bool isNotEnabledSetDataVistaGrd) : IRequest<ChangeStateReadNotificationResult>;
    public record CheckNotificationResult(bool output);

    public record CheckNotification(DocsPaVO.Notification.Notification notification, InfoUtente infoUser) : IRequest<CheckNotificationResult>;
    
    public record DelegaCheckAttivaResult(int output);

    public record DelegaCheckAttiva(DocsPaVO.utente.InfoUtente infoUtente) : IRequest<DelegaCheckAttivaResult>;

    public record UpdateNoteNotificationResult(bool output);
    public record UpdateNoteNotification(string idNotify, string note) : IRequest<UpdateNoteNotificationResult>;

    public record getBannerResult(string output);
    public record getBanner(string idAmm) : IRequest<getBannerResult>;

    public record getNewsResult(string output);
    public record getNews(string idAmm) : IRequest<getNewsResult>;

    public record GetLastDocumentsViewResult(List<DocsPaVO.documento.DocumentoVisualizzato> output);
    public record GetLastDocumentsView(InfoUtente infoUtente) : IRequest<GetLastDocumentsViewResult>;

    public record GetListDocumentiTrasmessiNotifyResult(DatiTrasmissioneDocumento[] output);
    public record GetListDocumentiTrasmessiNotify(List<Notification> notifications, DocsPaVO.Smistamento.MittenteSmistamento mittente) : IRequest<GetListDocumentiTrasmessiNotifyResult>;
}
