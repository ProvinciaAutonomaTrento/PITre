// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interoperabilita;

public class InteroperabilitaManager
{
    public static bool isRicevutaPec(Interoperabilita.CMMsg mc)
    {
        bool retval = false;
        DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta ricevuta = getTipoRicevutaPec(mc);
        if (ricevuta != DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.unknown &&
            ricevuta != DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_NO_XRicevuta)
            retval = true;

        /*
        if (mc.isPECAcceptNotify() ||
                                       mc.isPECDeliveredNotify() ||
                                       mc.isDeliveryStatusNotification() ||
                                       mc.isFromNonPEC() ||
                                       mc.isPECAcceptNotify() ||
                                       mc.isPECAlertVirus() ||
                                       mc.isPECContainVirus() ||
                                      // mc.isPECDelivered() ||
                                       mc.isPECDeliveredNotifyShort() ||
                                       mc.isPECError() ||
                                       mc.isPECErrorDeliveredNotifyByVirus() ||
                                       mc.isPECErrorPreavvisoDeliveredNotify() ||
                                       mc.isPECNonAcceptNotify() ||
                                       mc.isPECPresaInCarico())
        {
            retval = true;
        }
        */
        return retval;
    }

    public static DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta getTipoRicevutaPec(Interoperabilita.CMMsg mc)
    {

        DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.unknown;

        if (mc.isPECAcceptNotify())                         //avvenuta accettazione
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Accept_Notify;
        else if (mc.isDeliveryStatusNotification())         //messaggi non ricevuti
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.Delivery_Status_Notification;
        else if (mc.isFromNonPEC())                         //messaggi non Pec
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.From_Non_PEC;
        else if (mc.isPECAlertVirus())                      //rilevazione virus
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Alert_Virus;
        else if (mc.isPECContainVirus())                    //non accettazione
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Contain_Virus;
        else if (mc.isPECDelivered())                       //Consengata
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Delivered;
        else if (mc.isPECDeliveredNotify())                 //avvenuta consegna
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Delivered_Notify;
        else if (mc.isPECDeliveredNotifyShort())            //avvenuta consegna
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Delivered_Notify_Short;
        else if (mc.isPECError())                           //errore generico
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Error;
        else if (mc.isPECErrorDeliveredNotifyByVirus())     //errore consegna
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Error_Delivered_Notify_By_Virus;
        else if (mc.isPECErrorPreavvisoDeliveredNotify())  //preavviso errore consegna
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Error_Preavviso_Delivered_Notify;
        else if (mc.isPECNonAcceptNotify())                //non accettazione
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Non_Accept_Notify;
        else if (mc.isPECPresaInCarico())                  //presa in carico
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Presa_In_Carico;
        else if (mc.isPECErrorDeliveredNotify())
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_Mancata_Consegna;
        else if (mc.isPEC())
            tipoRicevuta = DocsPaVO.Interoperabilita.MailAccountCheckResponse.MailProcessed.MailPecXRicevuta.PEC_NO_XRicevuta;


        return tipoRicevuta;

    }


}
