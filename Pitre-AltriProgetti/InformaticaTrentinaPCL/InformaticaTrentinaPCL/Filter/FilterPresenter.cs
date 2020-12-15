using System;
using InformaticaTrentinaPCL.AnalyticsCore;
using InformaticaTrentinaPCL.Filter.MVP;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Network;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Filter
{
    public class FilterPresenter : IFilterPresenter
    {
        protected IFilterView view;
        protected SessionData sessionData;
        protected IAnalyticsManager analyticsManager;
        protected FilterModel filterModel;
        protected SectionType sectionType;


        public FilterPresenter(IFilterView view, INativeFactory nativeFactory
                               , FilterModel filterModel, SectionType sectionType)
        {
            this.view = view;
            this.sessionData = nativeFactory.GetSessionData();
            this.analyticsManager = nativeFactory.GetAnalyticsManager();
            this.sectionType = sectionType;
            InitFilter(filterModel);
        }

        private void InitFilter(FilterModel filterModel)
        {
            if (filterModel == null)
            {
                this.filterModel = new FilterModel();
                this.filterModel.documentType = TypeDocument.ALL;
            }
            else
            {
                this.filterModel = filterModel;
            }
        }

        #region Implementation

        public void OnConfirmButton()
        {
            //when dates are not specified, then show an error,
            //when dates are specified and the diff is greater than NetworkConstants.MAX_SEARCH_MONTH_LIMIT then show an error
            //if only one is not specified then the filterModel is responsible for show the predefined date
            string error = "";
            switch (filterModel.documentType)
            {
                case TypeDocument.FASCICOLO:
                    error = checkFilterDate();
                    if (error != null) 
                    {
                        showError(error);
                        return;
                    }
                    break;

                case TypeDocument.DOCUMENTO:
                    error = checkFilterDate();
                    if (error != null)
                    {
                        error = checkProtocolFilterDate();
                        if (error != null)
                        {
                            showError(error);
                            return;
                        }
                    }
                    break;

                case TypeDocument.ALL:
                    error = checkFilterDate();
                    if (error != null)
                    {
                        showError(error);
                        return;
                    }

                    if (sectionType != SectionType.SIGN) {
                        error = checkProtocolFilterDate();
                        if (error != null)
                        {
                            showError(error);
                            return;
                        }
                    }
                    break;

            }

            view.OnNewFilter(this.filterModel);
        }

        private void showError(string error) 
        {
            view.OnFilterError(error);
        }

        string checkFilterDate()
        {
            var from = filterModel.fromDate;
            var to = filterModel.endDate;

            if (!from.IsSet() && !to.IsSet())
            {
                return LocalizedString.FILTER_DATE_ERROR.Get();
            }
            else if (from.IsSet() && to.IsSet() && to.AddMonths(-NetworkConstants.MAX_SEARCH_MONTH_LIMIT) > from)
            {
                return string.Format(LocalizedString.FILTER_DATE_RANGE_ERROR.Get(), NetworkConstants.MAX_SEARCH_MONTH_LIMIT);
            }
            return null;
        }

        string checkProtocolFilterDate()
        {
            var from = filterModel.fromDateProtocol;
            var to = filterModel.endDateProtocol;

            if (!from.IsSet() && !to.IsSet())
            {
               return LocalizedString.FILTER_PROTOCOL_DATE_ERROR.Get();

            }
            else if (from.IsSet() && to.IsSet() && to.AddMonths(-NetworkConstants.MAX_SEARCH_MONTH_LIMIT) > from)
            {
                return string.Format(LocalizedString.FILTER_PROTOCOL_DATE_RANGE_ERROR.Get(), NetworkConstants.MAX_SEARCH_MONTH_LIMIT);
            }
            return null;
        }

        public void OnViewReady()
        {
            view.UpdateFilterView(filterModel);
            checkFields();
        }

        public void UpdateEndDate(DateTime endDate)
        {
            this.filterModel.endDate = endDate;
            checkFields();
        }

        public void UpdateStartDate(DateTime startDate)
        {
            this.filterModel.fromDate = startDate;
            checkFields();
        }

        /// <summary>
        /// Updates the start date proto.
        /// </summary>
        /// <param name="startDateProtocol">Start date proto.</param>
        public void UpdateStartDateProtocol(DateTime startDateProtocol)
        {
            this.filterModel.fromDateProtocol = startDateProtocol;
            checkFields();
        }

        /// <summary>
        /// Updates the end date protol.
        /// </summary>
        /// <param name="startEndProtocol">Start end protocol.</param>
        public void UpdateEndDateProtocol(DateTime startEndProtocol)
        {
            this.filterModel.endDateProtocol = startEndProtocol;
            checkFields();
        }

        /// <summary>
        /// Updates the identifier document.
        /// </summary>
        /// <param name="idDocument">Identifier document.</param>
        public void UpdateIdDocument(string idDocument)
        {
            this.filterModel.idDocument = idDocument;
            checkFields();
        }

        public void UpdateOggetto(string oggetto)
        {
            this.filterModel.oggetto = oggetto;
            checkFields();
        }

        /// <summary>
        /// Updates the number protocol.
        /// </summary>
        /// <param name="numProtocol">Number protocol.</param>
        public void UpdateNumberProtocol(string numProtocol)
        {
            this.filterModel.NumProto = numProtocol;
            checkFields();
        }

        /// <summary>
        /// Updates the year protocol.
        /// </summary>
        /// <param name="yearProtocol">Year protocol.</param>
        public void UpdateYearProtocol(string yearProtocol)
        {
            this.filterModel.yearProto = yearProtocol;
            checkFields();
        }

        public void UpdateType(TypeDocument type)
        {
            this.filterModel.documentType = type;
            checkFields();
        }

        private void checkFields()
        {

            bool dateSet = filterModel.fromDate.IsSet() && filterModel.endDate.IsSet();
            bool dateUnset = !filterModel.fromDate.IsSet() && !filterModel.endDate.IsSet();
            bool dateValid = dateSet || dateUnset;

            bool enabled = filterModel.documentType != TypeDocument.ALL || (filterModel.documentType == TypeDocument.ALL && (!string.IsNullOrEmpty(filterModel.idDocument) || !string.IsNullOrEmpty(filterModel.NumProto) || !string.IsNullOrEmpty(filterModel.yearProto) ||
                                                                                                                             !string.IsNullOrEmpty(filterModel.oggetto) || filterModel.fromDate.IsSet() || filterModel.endDate.IsSet() || filterModel.fromDateProtocol.IsSet() || filterModel.endDateProtocol.IsSet()));
            view.EnableButton(enabled);
        }
        #endregion
    }
}
