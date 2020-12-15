using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.NotificationCenter;
using System.Data;

namespace DocsPaDB.Query.SpecializedItem
{
    /// <summary>
    /// Interface for the creation of specialized content of the notification.
    /// The content is related to the event type that generates the notification
    /// </summary>
    public interface ISpecializedItem
    {
        string CreateSpecializedItem(Event e);
        void CreateSpecializedItemObject(DataSet ds, ref string specializedItem);
    }
}
