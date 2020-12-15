using System;
using Android.OS;
using InformaticaTrentinaPCL.Assign;
using InformaticaTrentinaPCL.Delega;
using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Droid.CustomDateRange;
using InformaticaTrentinaPCL.Droid.Utils;
using Newtonsoft.Json;
using static InformaticaTrentinaPCL.Droid.CustomDateRange.CustomDateRangeView;

namespace InformaticaTrentinaPCL.Droid.Delegation
{
    public class NewMandateNativePresenter : NewMandatePresenter, IAndroidPresenter, IDateRangeSelectedListener
    {
        const string KEY_START_DATE = "KEY_START_DATE";
        const string KEY_END_DATE = "KEY_END_DATE";
        const string KEY_ASSIGNABLE_MODEL = "KEY_ASSIGNABLE_MODEL";

        public NewMandateNativePresenter(INewMandateView view, AndroidNativeFactory nativeFactory) : base(view, nativeFactory) { }

        public Bundle GetStateToSave()
        {
            Bundle bundle = new Bundle();
            bundle.PutLong(KEY_START_DATE, startDate.ToBinary());
            bundle.PutLong(KEY_END_DATE, endDate.ToBinary());
            bundle.PutString(KEY_ASSIGNABLE_MODEL, JsonConvert.SerializeObject(assignableModel));

            return bundle;
        }

        public void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            if(savedInstanceState != null) {
                startDate = new DateTime(savedInstanceState.GetLong(KEY_START_DATE));
                endDate = new DateTime(savedInstanceState.GetLong(KEY_END_DATE));

                string stringAssignable = savedInstanceState.GetString(KEY_ASSIGNABLE_MODEL);

                if(!string.IsNullOrEmpty(stringAssignable))
                    assignableModel = JsonConvert.DeserializeObject<AssigneeModel>(stringAssignable);
            }

        }

        #region interface IDateRangeSelectedListener

        public void OnDateSelected(DateType dateType, DateTime date, int idView)
        {
            switch(dateType)
            {
                case DateType.START_DATE:
                    startDate = date;
                    break;

                case DateType.END_DATE:
                    endDate = date;
                    break;
            }
        }

        #endregion
    }
}
