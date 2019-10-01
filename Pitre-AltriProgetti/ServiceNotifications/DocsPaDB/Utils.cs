using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocsPaVO.NotificationCenter;

namespace DocsPaDB
{
    static class Utils
    {

        public static int ConvertField(string field)
        {
            int val = 0;
            if (!string.IsNullOrEmpty(field))
            {
                try
                {
                    val = Convert.ToInt32(field);
                }
                catch (Exception exc)
                {
                }
            }
            return val;
        }

        public static string GetTypeEvent(string eventTypeExtended)
        {
            if (eventTypeExtended.Contains(SupportStructures.EventType.ACCEPT_TRASM + "_"))
                return SupportStructures.EventType.ACCEPT_TRASM;
            else if (eventTypeExtended.Contains(SupportStructures.EventType.REJECT_TRASM + "_"))
                return SupportStructures.EventType.REJECT_TRASM;
            else if (eventTypeExtended.Contains(SupportStructures.EventType.CHECK_TRASM + "_"))
                return SupportStructures.EventType.CHECK_TRASM;
            else if (eventTypeExtended.Contains(SupportStructures.EventType.TRASM + "_"))
                return SupportStructures.EventType.TRASM;
            else if (eventTypeExtended.Contains(SupportStructures.EventType.INTERROTTO_PROCESSO + "_"))
                return SupportStructures.EventType.INTERROTTO_PROCESSO;
            else if (eventTypeExtended.Contains(SupportStructures.EventType.CONCLUSIONE_PROCESSO + "_"))
                return SupportStructures.EventType.CONCLUSIONE_PROCESSO;
            else if (eventTypeExtended.Contains(SupportStructures.EventType.PROCESSO_FIRMA + "_"))
                return SupportStructures.EventType.PROCESSO_FIRMA;
            else
                return eventTypeExtended;
        }  
    }
}
