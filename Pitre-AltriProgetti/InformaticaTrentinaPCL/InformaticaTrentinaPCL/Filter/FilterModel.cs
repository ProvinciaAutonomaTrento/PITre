using System;
using System.Text;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;


namespace InformaticaTrentinaPCL.Filter
{
    public class FilterModel
    {
        public TypeDocument documentType { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime endDate { get; set; }
        public string idDocument { get; set; }
        public string NumProto { get; set; }
        public string yearProto { get; set; }
        // optional can be also null
        public string oggetto { get; set; }
        public DateTime fromDateProtocol { get; set; }
        public DateTime endDateProtocol{ get; set; }

        public FilterModel()
        {
        }

        public string GetLabel()
        {
            StringBuilder labelBuilder = new StringBuilder();
            switch (documentType)
            {
                case TypeDocument.DOCUMENTO:
                    labelBuilder.Append(LocalizedString.DOCUMENTI.Get())
                                .Append(" ");
                    break;
                case TypeDocument.FASCICOLO:
                    labelBuilder.Append(LocalizedString.FASCICOLI.Get())
                                .Append(" ");
                    break;
            }

            GetLabelIdDocument(labelBuilder);
            GetDateDocument(labelBuilder);

            GetLabelNumProtocol(labelBuilder);
            GetLabelYearProtocol(labelBuilder);
            GetLabelDateProtocol(labelBuilder);
            GetLabelOggetto(labelBuilder);

            return labelBuilder.ToString();
        }

        public void GetLabelOggetto(StringBuilder labelBuilder)
        {
            if (!string.IsNullOrEmpty(oggetto))
            {
                labelBuilder.Append(" ");
                labelBuilder.Append(LocalizedString.OGGETTO.Get());
                labelBuilder.Append(": ");
                labelBuilder.Append(oggetto);
            }
        }

        /// <summary>
        /// Gets the label date protocol.
        /// </summary>
        public void GetLabelDateProtocol(StringBuilder labelBuilder)
        {
            appendDateString(fromDateProtocol, endDateProtocol, labelBuilder);
        }

        /// <summary>
        /// Gets the label identifier document.
        /// </summary>
        /// <param name="labelBuilder">Label builder.</param>
        public void GetLabelIdDocument(StringBuilder labelBuilder)
        {

            if(!string.IsNullOrEmpty(idDocument))
            {
                string labelDocument = string.Format(LocalizedString.FILTERS_DOCUMENT_ID_LABEL_RESULT.Get(), ": ", idDocument);
                labelBuilder.Append(labelDocument)
                               .Append(" ");
            }
        }

        /// <summary>
        /// Gets the date document.
        /// </summary>
        public void GetDateDocument(StringBuilder labelBuilder)
        {
            appendDateString(fromDate, endDate, labelBuilder);
        }

        void appendDateString(DateTime from, DateTime to, StringBuilder labelBuilder)
        {
            if (from.IsSet() || to.IsSet())
            {
                string daLabel = documentType == TypeDocument.ALL
                                ? LocalizedString.DA.Get()
                                : LocalizedString.DA.Get().ToLower();

                labelBuilder.Append(daLabel)
                    .Append(" ")
                    .Append(from.IsSet() ? from.ToReadableString() : to.AddMonths(-NetworkConstants.MAX_SEARCH_MONTH_LIMIT).ToReadableString())
                    .Append(" ");

                labelBuilder.Append(LocalizedString.A.Get())
                    .Append(" ")
                    .Append(to.IsSet() ? to.ToReadableString() : from.AddMonths(NetworkConstants.MAX_SEARCH_MONTH_LIMIT).ToReadableString())
                    .Append(" ");
            }
        }

        /// <summary>
        /// Gets the label number protocol.
        /// </summary>
        /// <param name="labelBuilder">Label builder.</param>
        public void GetLabelNumProtocol(StringBuilder labelBuilder)
        {
            if (!string.IsNullOrEmpty(NumProto))
            {
                string labelNumProto= string.Format(LocalizedString.FILTERS_PROTOCOL_NUMBER_LABEL_RESULT.Get(), ": ", NumProto);
                labelBuilder.Append(labelNumProto)
                               .Append(" ");
            }
        }

        /// <summary>
        /// Gets the label year protocol.
        /// </summary>
        /// <param name="labelBuilder">Label builder.</param>
        public void GetLabelYearProtocol(StringBuilder labelBuilder)
        {
            if (!string.IsNullOrEmpty(yearProto))
            {
                string labelYearProto = string.Format(LocalizedString.FILTERS_SECTION_YEAR_PROTOCOL_LABEL_RESULT.Get(), ": ", yearProto);
                labelBuilder.Append(labelYearProto)
                               .Append(" ");
            }
        }

        /// <summary>
        /// Ritorna il tipo da utilizzare nella request per ADL 
        /// </summary>
        /// <returns>The search type.</returns>
        public string GetADLRequestType()
        {
            switch (documentType)
            {
                case TypeDocument.FASCICOLO:
                    return NetworkConstants.TIPO_ADL_FASC;
                case TypeDocument.DOCUMENTO:
                    return NetworkConstants.TIPO_ADL_DOC;
                default:
                    return NetworkConstants.TIPO_ADL_ALL;
            }
        }
        /// <summary>
        /// Ritorna il tipo da utilizzare nella request per 'Ricerca' 
        /// </summary>
        /// <returns>The search type.</returns>
        public string GetSearchRequestType()
        {
            switch (documentType)
            {
                case TypeDocument.FASCICOLO:
                    return NetworkConstants.TIPO_RICERCA_FASC;
                case TypeDocument.DOCUMENTO:
                    return NetworkConstants.TIPO_RICERCA_DOC;
                default:
                    return NetworkConstants.TIPO_RICERCA_ALL;
            }
        }
    }
}
