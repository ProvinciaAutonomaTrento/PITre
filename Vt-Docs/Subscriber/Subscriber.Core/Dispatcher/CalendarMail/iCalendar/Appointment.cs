using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Subscriber.Dispatcher.CalendarMail.iCalendar
{
    /// <summary>
    /// Classe per la gestione degli appuntamenti in formato iCalendar
    /// </summary>
    public sealed class Appointment
    {
        /// <summary>
        /// 
        /// </summary>
        protected static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(Appointment));

        /// <summary>
        /// 
        /// </summary>
        private const string SEND_APPOINTMENT = "SendAppointment.ics";

        /// <summary>
        /// 
        /// </summary>
        private const string SEND_APPOINTMENT_DISPLAY_ALERT = "SendAppointmentDisplayAlert.ics";

        /// <summary>
        /// 
        /// </summary>
        private const string CALENDAR_DATE_FORMAT = "yyyyMMddTHHmmssZ";    

        /// <summary>
        /// 
        /// </summary>
        private Appointment()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string Make(AppointmentInfo info)
        {
            string templateName = GetTemplateName(info);

            string content = GetSendAppointmentTemplate(templateName);
            
            content = ReplaceData(content, info);

            if (info.Alert != null)
            {
                content = ReplaceData(content, info.Alert);
            }

            return content;
        }

        /// <summary>
        /// Sostituzione dei valori nei corrispondenti campi del template ics
        /// </summary>
        /// <param name="content"></param>
        /// <param name="objectData"></param>
        private static string ReplaceData(string content, object objectData)
        {
            foreach (PropertyInfo p in objectData.GetType().GetProperties())
            {
                // Reperimento attributi associati alla proprietà
                AppointmentParamAttribute[] attrs = (AppointmentParamAttribute[])
                        p.GetCustomAttributes(typeof(AppointmentParamAttribute), true);

                if (attrs.Length > 0)
                {
                    AppointmentParamAttribute attribute = attrs[0];

                    string value = string.Empty;

                    if (p.PropertyType == typeof(DateTime))
                    {
                        DateTime date = (DateTime)p.GetValue(objectData, null);

                        if (date > DateTime.MinValue && date < DateTime.MaxValue)
                            value = date.ToUniversalTime().ToString(CALENDAR_DATE_FORMAT);
                        else
                            value = string.Empty;
                    }
                    else
                    {
                        if (p.GetValue(objectData, null) != null)
                            value = p.GetValue(objectData, null).ToString();
                        else
                            value = string.Empty;
                    }

                    content = content.Replace(attribute.Name, value);
                }
            }

            return content;
        }

        /// <summary>
        /// Reperimento template ics da utilizzare in base al tipo dell'appuntamenot
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static string GetTemplateName(AppointmentInfo info)
        {
            string templateName = string.Empty;

            if (info.Alert != null)
                templateName = typeof(Appointment).Namespace + ".Templates." + SEND_APPOINTMENT_DISPLAY_ALERT;
            else
                templateName = typeof(Appointment).Namespace + ".Templates." + SEND_APPOINTMENT;

            return templateName;
        }

        /// <summary>
        /// Reperimento template ics per l'invio di appuntamenti
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private static string GetSendAppointmentTemplate(string templateName)
        {
            string content = string.Empty;

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(templateName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }

            return content;
        }
    }
}