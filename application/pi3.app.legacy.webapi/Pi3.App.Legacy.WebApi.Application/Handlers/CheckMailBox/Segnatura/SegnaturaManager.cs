// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using DocsPaVO.Interoperabilita.Segnatura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.App.Legacy.WebApi.Application.Handlers.CheckMailBox.Segnatura
{
    public class SegnaturaManager
    {

        public static string GetIndirizzoTelematicoMittente(SegnaturaInformaticaType sexml)
        {
            try
            {
                Type t = sexml.Descrizione.Mittente.Item.GetType();
                switch (t.Name)
                {
                    case "PersonaFisicaType":
                        {
                            IndirizzoTelematicoType[] ind = (sexml.Descrizione.Mittente.Item as PersonaFisicaType).Contatti.IndirizzoTelematico;
                            return GerEmailFromIndTelematico(ind);
                        }
                    case "PersonaGiuridicaType":
                        {
                            IndirizzoTelematicoType[] ind = (sexml.Descrizione.Mittente.Item as PersonaGiuridicaType).ContattiPersonaGiuridica.IndirizzoTelematico;
                            return GerEmailFromIndTelematico(ind);
                        }
                    case "AmministrazioneType":
                        {
                            IndirizzoTelematicoType[] ind = null;
                            AmministrazioneType amministrazioneType = sexml.Descrizione.Mittente.Item as AmministrazioneType;
                            if (amministrazioneType.ContattiUO != null && amministrazioneType.ContattiUO.IndirizzoTelematico != null
                                && amministrazioneType.ContattiUO.IndirizzoTelematico.Count() > 0)
                                ind = amministrazioneType.ContattiUO.IndirizzoTelematico;
                            else
                                ind = amministrazioneType.ContattiAmministrazione.IndirizzoTelematico;
                            return GerEmailFromIndTelematico(ind);
                        }
                    case "AmministrazioneEsteraType":
                        {
                            IndirizzoTelematicoType[] ind = (sexml.Descrizione.Mittente.Item as AmministrazioneEsteraType).ContattiAmministrazione.IndirizzoTelematico;
                            return GerEmailFromIndTelematico(ind);
                        }
                    default: return "";
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static string GetIndirizzoTelematico(DestinatarioType destinatario)
        {
            try
            {
                Type t = (destinatario as DestinatarioType).Item.GetType();
                switch (t.Name)
                {
                    case "PersonaFisicaType":
                        {
                            IndirizzoTelematicoType[] ind = ((destinatario as DestinatarioType).Item as PersonaFisicaType).Contatti.IndirizzoTelematico;
                            return GerEmailFromIndTelematico(ind);
                        }
                    case "PersonaGiuridicaType":
                        {
                            IndirizzoTelematicoType[] ind = ((destinatario as DestinatarioType).Item as PersonaGiuridicaType).ContattiPersonaGiuridica.IndirizzoTelematico;
                            return GerEmailFromIndTelematico(ind);
                        }
                    case "AmministrazioneType":
                        {
                            IndirizzoTelematicoType[] ind = null;
                            AmministrazioneType amministrazioneType = (destinatario as DestinatarioType).Item as AmministrazioneType;
                            if (amministrazioneType.ContattiUO != null && amministrazioneType.ContattiUO.IndirizzoTelematico != null
                                && amministrazioneType.ContattiUO.IndirizzoTelematico.Count() > 0)
                                ind = amministrazioneType.ContattiUO.IndirizzoTelematico;
                            else
                                ind = amministrazioneType.ContattiAmministrazione.IndirizzoTelematico;
                            return GerEmailFromIndTelematico(ind);
                        }
                    case "AmministrazioneEsteraType":
                        {
                            IndirizzoTelematicoType[] ind = ((destinatario as DestinatarioType).Item as AmministrazioneEsteraType).ContattiAmministrazione.IndirizzoTelematico;
                            return GerEmailFromIndTelematico(ind);
                        }
                    default: return "";
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static string GetDescrizioneDestinatario(DestinatarioType destinatario)
        {

            Type t = (destinatario as DestinatarioType).Item.GetType();
            switch (t.Name)
            {
                case "PersonaFisicaType":
                    {
                        return (destinatario.Item as PersonaFisicaType).Cognome + " " + (destinatario.Item as PersonaFisicaType).Nome;
                    }
                case "PersonaGiuridicaType":
                    {
                        return (destinatario.Item as PersonaGiuridicaType).Denominazione;
                    }
                case "AmministrazioneType":
                    {
                        return (destinatario.Item as AmministrazioneType).DenominazioneAmministrazione;
                    }
                case "AmministrazioneEsteraType":
                    {
                        return (destinatario.Item as AmministrazioneEsteraType).DenominazioneAmministrazione;
                    }
                default: return "";
            }

        }


        private static string GerEmailFromIndTelematico(IndirizzoTelematicoType[] indirizzoTelematico)
        {
            if (indirizzoTelematico != null)
            {
                IndirizzoTelematicoType email = indirizzoTelematico.Where(i => i.tipo.ToString() == "smtp").FirstOrDefault();
                if (email != null)
                    return email.Value;
            }
            return "";
        }
    }
}
