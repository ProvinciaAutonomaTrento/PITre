
using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.CustomDateRange;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Filter;
using InformaticaTrentinaPCL.Filter.MVP;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Droid.Filter
{
    [Activity(Label = "@string/app_name")]
    public class FilterActivity : BaseActivity, CustomDateRangeView.IDateRangeSelectedListener, IFilterView
    {
        public const string KEY_FILTER = "KEY_FILTER";
        public const string KEY_SECTION_TYPE_INDEX = "KEY_SECTION_TYPE_INDEX";

        LinearLayout linearLayoutChooseType;
        LinearLayout linearLayoutSelectionPeriodAndDate;
        RadioGroup typeRadioGroup;
        CustomDateRangeView dateRangeView;
        CustomDateRangeView customRangeDateDocument;
        CustomDateRangeView customRangeDateProtocol;
        Button confirmButton;
        TextView sectionProtocolTitle;
        EditText editTextProcolYear;
        TextView sectionDocumentTitle;
        EditText documentID;
        EditText protocolNumberOrObject;
        SectionType sectionType;

        IFilterPresenter presenter;

        protected override int LayoutResource => Resource.Layout.activity_filter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            linearLayoutChooseType = FindViewById<LinearLayout>(Resource.Id.linearLayout_choose_type);
            linearLayoutSelectionPeriodAndDate = FindViewById<LinearLayout>(Resource.Id.linearLayout_selection_period_and_date);
            typeRadioGroup = FindViewById<RadioGroup>(Resource.Id.radioGroup_type);
            dateRangeView = FindViewById<CustomDateRangeView>(Resource.Id.custom_date_range_view);
            dateRangeView.initializeCustomDateRangeView(this, FragmentManager);

            customRangeDateDocument = FindViewById<CustomDateRangeView>(Resource.Id.custom_range_date_document);
            //  customRangeDateDocument.initializeCustomDateRangeView(this, FragmentManager);
            customRangeDateDocument.initializeCustomDateRangeView(this, FragmentManager, Resources.GetText(Resource.String.activity_search_filter_data_documento));  //


            customRangeDateProtocol = FindViewById<CustomDateRangeView>(Resource.Id.custom_range_date_protocol);
            customRangeDateProtocol.initializeCustomDateRangeView(this, FragmentManager, Resources.GetText(Resource.String.activity_search_filter_data_fascicolo));


            FilterModel filter = null;

            if (Intent.GetStringExtra(KEY_FILTER)!=null)
            {
                filter = JsonConvert.DeserializeObject<FilterModel>(Intent.GetStringExtra(KEY_FILTER));
            }

            int sectionTypeIndex = Intent.GetIntExtra(KEY_SECTION_TYPE_INDEX, -1);
            sectionType = (SectionType)Enum.ToObject(typeof(SectionType), sectionTypeIndex);

            presenter = new FilterPresenter(this,AndroidNativeFactory.Instance(), filter, sectionType);
            
            typeRadioGroup.CheckedChange += (s, e) => {
                presenter.UpdateType(getTypeDocumentFromRadioButtonId(e.CheckedId));
            };

            FindViewById(Resource.Id.imageView_close).Click += delegate
            {
                Finish();
            };

            FindViewById(Resource.Id.button_today).Click += delegate
            {
                dateRangeView.setFromDate(DateTime.Now);
                dateRangeView.setEndDate(DateTime.Now);
            };

            FindViewById(Resource.Id.button_last7days).Click += delegate
            {
                DateTime now = DateTime.Now;

                dateRangeView.setFromDate(now.AddDays(-7));
                dateRangeView.setEndDate(now);
            };

            FindViewById(Resource.Id.button_last30days).Click += delegate
            {
                DateTime now = DateTime.Now;

                dateRangeView.setFromDate(now.AddDays(-30));
                dateRangeView.setEndDate(now);
            };

            confirmButton = FindViewById<Button>(Resource.Id.button_confirm);
            confirmButton.Click += delegate {
                presenter.OnConfirmButton();                
            };

            sectionDocumentTitle = FindViewById<TextView>(Resource.Id.textView_section_document_title);
            sectionDocumentTitle.Text = LocalizedString.FILTERS_SECTION_DOCUMENT_TITLE.Get();

            documentID = FindViewById<EditText>(Resource.Id.editText_document_id);
            documentID.Hint = LocalizedString.FILTERS_DOCUMENT_ID.Get();
            documentID.TextChanged += (s, e) =>
            {
                presenter.UpdateIdDocument(documentID.Text);
            };

            sectionProtocolTitle = FindViewById<TextView>(Resource.Id.textView_section_protocol_title);
            sectionProtocolTitle.Text = LocalizedString.FILTERS_SECTION_PROTOCOL_TITLE.Get();

            protocolNumberOrObject = FindViewById<EditText>(Resource.Id.editText_protocol_number);
            protocolNumberOrObject.Hint = LocalizedString.FILTERS_PROTOCOL_NUMBER.Get();
         
     
            editTextProcolYear = FindViewById<EditText>(Resource.Id.editText_year_protocol);
            editTextProcolYear.Hint = LocalizedString.FILTERS_SECTION_YEAR_PROTOCOL.Get();
            editTextProcolYear.TextChanged += (s, e) =>
            {
                presenter.UpdateYearProtocol(editTextProcolYear.Text);
            };

            ShowFiltersBySection(sectionType);
            GetNumberProtocolOrObjectText();
        }

        public void GetNumberProtocolOrObjectText()
        {
            if (sectionType == SectionType.SIGN)
            {
                protocolNumberOrObject.TextChanged += (s, e) =>
                {
                    presenter.UpdateOggetto(protocolNumberOrObject.Text);
                };
            }
            else
            {
                protocolNumberOrObject.TextChanged += (s, e) =>
               {
                   presenter.UpdateNumberProtocol(protocolNumberOrObject.Text);
               };
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            presenter.OnViewReady();
        }

        TypeDocument getTypeDocumentFromRadioButtonId(int radioButtonId)
        {
            return radioButtonId == Resource.Id.radioButton_documents ? TypeDocument.DOCUMENTO : radioButtonId == Resource.Id.radioButton_dossiers ? TypeDocument.FASCICOLO : TypeDocument.ALL;
        }

        int getResourceIdFromTypeDocument(TypeDocument type)
        {
            return type == TypeDocument.ALL ? Resource.Id.radioButton_all : type == TypeDocument.DOCUMENTO ? Resource.Id.radioButton_documents : Resource.Id.radioButton_dossiers;
        }

        #region CustomDateRangeView.IDateRangeSelectedListener
        public void OnDateSelected(CustomDateRangeView.DateType dateType, DateTime date, int idView)
        {
            switch (idView)
            {
                case Resource.Id.custom_range_date_document:
                    UpdateDateDocument(dateType, date);
                    break;
                case Resource.Id.custom_range_date_protocol:
                    UpdateDateProtocol(dateType, date);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Updates the date document.
        /// </summary>
        /// <param name="dateType">Date type.</param>
        /// <param name="date">Date.</param>
        public void UpdateDateDocument(CustomDateRangeView.DateType dateType, DateTime date)
        {
            if (dateType == CustomDateRangeView.DateType.START_DATE)
            {
                presenter.UpdateStartDate(date);
            }
            else
            {
                presenter.UpdateEndDate(date);
            }
        }

        /// <summary>
        /// Updates the date protocol.
        /// </summary>
        /// <param name="dateType">Date type.</param>
        /// <param name="date">Date.</param>
        public void UpdateDateProtocol(CustomDateRangeView.DateType dateType, DateTime date)
        {
            if (dateType == CustomDateRangeView.DateType.START_DATE)
            {
                presenter.UpdateStartDateProtocol(date);
            }
            else
            {
                presenter.UpdateEndDateProtocol(date);
            }
        }
        #endregion

        #region IFilterView
        public void EnableButton(bool enabled)
        {
            confirmButton.Enabled = enabled;
        }

        public void OnNewFilter(FilterModel filterModel)
        {
            Intent.PutExtra(KEY_FILTER, JsonConvert.SerializeObject(filterModel));
            SetResult(Result.Ok, Intent);
            Finish();
        }

        public void UpdateFilterView(FilterModel filterModel)
        {
            if (filterModel != null)
            {
                PopulateFields(filterModel);
            }
            else
            {
                typeRadioGroup.Check(Resource.Id.radioButton_all);
            }
            
        }

        public void PopulateFields(FilterModel filterModel)
        {
            FindViewById<RadioButton>(getResourceIdFromTypeDocument(filterModel.documentType)).Checked = true;
            dateRangeView.InitFromDate(filterModel.fromDate);
            dateRangeView.InitEndDate(filterModel.endDate);

            customRangeDateDocument.InitFromDate(filterModel.fromDate);
            customRangeDateDocument.InitEndDate(filterModel.endDate);

            customRangeDateProtocol.InitFromDate(filterModel.fromDateProtocol);
            customRangeDateProtocol.InitEndDate(filterModel.endDateProtocol);

            documentID.Text = !string.IsNullOrEmpty(filterModel.idDocument) ? filterModel.idDocument : string.Empty;
            editTextProcolYear.Text = !string.IsNullOrEmpty(filterModel.yearProto) ? filterModel.yearProto : string.Empty;

            if (sectionType == SectionType.SIGN)
            {
                protocolNumberOrObject.Text = !string.IsNullOrEmpty(filterModel.oggetto) ? filterModel.oggetto : string.Empty;
            }
            else
            {
                protocolNumberOrObject.Text = !string.IsNullOrEmpty(filterModel.NumProto) ? filterModel.NumProto : string.Empty;
            }
        }

        public void ShowFiltersBySection(SectionType sectionType)
        {
            linearLayoutChooseType.Visibility = ViewStates.Visible;
            linearLayoutSelectionPeriodAndDate.Visibility= ViewStates.Visible;

            switch(sectionType)
            {
                case SectionType.SIGN:
                    linearLayoutChooseType.Visibility = ViewStates.Gone;
                    linearLayoutSelectionPeriodAndDate.Visibility = ViewStates.Gone;
                    HideProtocolSection();
                    break;
                case SectionType.ADL:
                    linearLayoutSelectionPeriodAndDate.Visibility = ViewStates.Gone;
                    break;
                case SectionType.SEARCH:
                    linearLayoutSelectionPeriodAndDate.Visibility = ViewStates.Gone;
                    break;
                default:
                    break;
            }
        }

        private void HideProtocolSection()
        {
            ChangeProtocoloNumberInObject();
            customRangeDateProtocol.Visibility = ViewStates.Gone;
            sectionProtocolTitle.Visibility = ViewStates.Gone;
            editTextProcolYear.Visibility = ViewStates.Gone;
        }

        public void ChangeProtocoloNumberInObject()
        {
            protocolNumberOrObject.Hint = LocalizedString.OGGETTO.Get();          
        }

        public void OnFilterError(string error)
        {
            new AlertDialog.Builder(this)
               .SetTitle(LocalizedString.TITLE_ALERT.Get())
               .SetMessage(error)
               .SetCancelable(false)
               .SetPositiveButton(LocalizedString.OK_BUTTON.Get(), (senderAlert, args) => {
                   ((AlertDialog)senderAlert).Dismiss();
                })
               .Show();
        }
        #endregion
    }
}
