// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ServiceNotifications
{
    public class Notification
    {
        private static ILogger logger = Log.ForContext(typeof(Notification));

        public static List<DocsPaVO.Notification.Notification> ReadNotificationsMobile(string idPeople, string idGroup, int requestedPage, int pageSize, out int totalRecordCount)
        {
            totalRecordCount = 0;
            List<DocsPaVO.Notification.Notification> listNotifications = new List<DocsPaVO.Notification.Notification>();
            try
            {
                DocsPaDB.Query_DocsPAWS.NotificationDB notificationDB = new DocsPaDB.Query_DocsPAWS.NotificationDB();
                listNotifications = notificationDB.ReadNotificationsMobile(idPeople, idGroup, requestedPage, pageSize, out totalRecordCount);
            }
            catch (Exception e)
            {
                logger.Debug(e, "Errore in BusinessLogic.ServiceNotifications.Notification  - metodo: ReadNotifications");
            }
            return listNotifications;
        }

    }
}
