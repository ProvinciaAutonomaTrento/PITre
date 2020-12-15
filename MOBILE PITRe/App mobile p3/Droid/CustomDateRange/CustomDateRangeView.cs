using System;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.Constraints;
using Android.Util;
using Android.Views;
using Android.Widget;

using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Droid.CustomDateRange
{
    public class CustomDateRangeView : ConstraintLayout
    {
        const string KEY_INSTANCE_STATE = "KEY_INSTANCE_STATE";
        const string KEY_FROM_DATE = "KEY_FROM_DATE";
        const string KEY_END_DATE = "KEY_END_DATE";

        Context mContext;

        View fromDateClickArea;
        View endDateClickArea;
        TextView fromDay;
        TextView textViewSelectDateStr;
        TextView fromMonth;
        TextView fromYear;
        TextView fromHour;
        TextView fromMinute;
        TextView endDay;
        TextView endMonth;
        TextView endYear;
        TextView endHour;
        TextView endMinute;

        DateTime fromDate;
        DateTime endDate;

        bool withTime;
        bool startDateAfterNow = false;

        string customTitleText = "";

        IDateRangeSelectedListener mListener;
        FragmentManager mFragmentManager;

        public CustomDateRangeView(Context context) : base(context)
        {
            init(context, null);
        }

        public CustomDateRangeView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            init(context, attrs);
        }

        public CustomDateRangeView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            init(context, attrs);
        }

        void init(Context context, IAttributeSet attrs)
        {
            mContext = context;

            Inflate(mContext, Resource.Layout.custom_date_range_view, this);
            SaveEnabled = true;

            TextView fromLabel = FindViewById<TextView>(Resource.Id.textView_select_date_start_str);
            fromDay = FindViewById<TextView>(Resource.Id.editText_from_day);
            fromMonth = FindViewById<TextView>(Resource.Id.editText_from_month);
            fromYear = FindViewById<TextView>(Resource.Id.editText_from_year);
            fromHour = FindViewById<TextView>(Resource.Id.editText_from_hour);
            fromMinute = FindViewById<TextView>(Resource.Id.editText_from_minute);

            TextView endLabel = FindViewById<TextView>(Resource.Id.textView_select_date_end_str);
            endDay = FindViewById<TextView>(Resource.Id.editText_end_day);
            endMonth = FindViewById<TextView>(Resource.Id.editText_end_month);
            endYear = FindViewById<TextView>(Resource.Id.editText_end_year);
            endHour = FindViewById<TextView>(Resource.Id.editText_end_hour);
            endMinute = FindViewById<TextView>(Resource.Id.editText_end_minute);

            fromDateClickArea = FindViewById(Resource.Id.view_fromDateClickArea);
            fromDateClickArea.Click += DateSelect_OnClick;
            endDateClickArea = FindViewById(Resource.Id.view_endDateClickArea);
            endDateClickArea.Click += DateSelect_OnClick;
            textViewSelectDateStr = FindViewById<TextView>(Resource.Id.textView_select_date_str);
            TypedArray a = context.Theme.ObtainStyledAttributes(
                attrs,
                Resource.Styleable.CustomDataRangeView,
                0, 0);

            string fromText;
            string endText;
            bool visibilityTitle;
           

            try
            {
                fromText = a.GetString(Resource.Styleable.CustomDataRangeView_fromLabel);
                endText = a.GetString(Resource.Styleable.CustomDataRangeView_endLabel);
                visibilityTitle = a.GetBoolean(Resource.Styleable.CustomDataRangeView_visibilityTitle, true);
                withTime = a.GetBoolean(Resource.Styleable.CustomDataRangeView_withTime, false);
            }
            finally
            {
                a.Recycle();
            }

            fromLabel.Text = (string.IsNullOrEmpty(fromText))
                ? Context.GetString(Resource.String.activity_new_delegation_start)
                : fromText;

            endLabel.Text = (string.IsNullOrEmpty(endText))
                ? Context.GetString(Resource.String.activity_new_delegation_end)
                : endText;

            textViewSelectDateStr.Visibility = visibilityTitle ? ViewStates.Visible : ViewStates.Gone;
            fromHour.Visibility = (withTime) ? ViewStates.Visible : ViewStates.Gone;
            fromMinute.Visibility = (withTime) ? ViewStates.Visible : ViewStates.Gone;
            endHour.Visibility = (withTime) ? ViewStates.Visible : ViewStates.Gone;
            endMinute.Visibility = (withTime) ? ViewStates.Visible : ViewStates.Gone;

           

        }

        protected override IParcelable OnSaveInstanceState()
        {
            Bundle bundle = new Bundle();
            bundle.PutParcelable(KEY_INSTANCE_STATE, base.OnSaveInstanceState());

            if (fromDate.IsSet())
                bundle.PutLong(KEY_FROM_DATE, fromDate.Ticks);

            if (endDate.IsSet())
                bundle.PutLong(KEY_END_DATE, endDate.Ticks);

            return bundle;
        }

        protected override void OnRestoreInstanceState(IParcelable state)
        {
            if (state != null && state is Bundle)
            {
                Bundle bundle = (Bundle)state;

                if (bundle.ContainsKey(KEY_FROM_DATE))
                {
                    fromDate = new DateTime(bundle.GetLong(KEY_FROM_DATE));
                    setDateValues(fromDate, fromDay, fromMonth, fromYear, fromHour, fromMinute, DateType.START_DATE);
                }

                if (bundle.ContainsKey(KEY_END_DATE))
                {
                    endDate = new DateTime(bundle.GetLong(KEY_END_DATE));
                    setDateValues(endDate, endDay, endMonth, endYear, endHour, endMinute, DateType.END_DATE);
                }

                state = (IParcelable)bundle.GetParcelable(KEY_INSTANCE_STATE);
            }
            base.OnRestoreInstanceState(state);

        }

        /// <summary>
        /// Metodo utilizzato per inizializzare gli attributi di CustomDateRangeView, indispensabile per il corretto funzionamento.
        /// </summary>
        /// <param name="listener">Listener.</param>
        /// <param name="fragmentManager">Fragment manager.</param>
        public void initializeCustomDateRangeView(IDateRangeSelectedListener listener, FragmentManager fragmentManager)
        {
            mListener = listener;
            mFragmentManager = fragmentManager;
        }

        public void initializeCustomDateRangeView(IDateRangeSelectedListener listener, FragmentManager fragmentManager , bool startdate_after_now)
        {
            startDateAfterNow = startdate_after_now;
            initializeCustomDateRangeView(listener, fragmentManager);
        }

        public void initializeCustomDateRangeView(IDateRangeSelectedListener listener, FragmentManager fragmentManager , String Titele)
        {
            initializeCustomDateRangeView( listener,  fragmentManager);
            textViewSelectDateStr.Text = Titele;
        }

        void DateSelect_OnClick(object sender, EventArgs eventArgs)
        {
            if (mFragmentManager == null)
            {
                throw new System.Exception("FragmentManager Not Initialized, please call initializeCustomDateRangeView before using this custom view");
            }

            int senderViewId = ((View)sender).Id;

            DateTime dateTime = senderViewId == Resource.Id.view_fromDateClickArea ? fromDate : endDate;

            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime date, int senderId)
            {
                if (senderId == fromDateClickArea.Id)
                {
                    if (startDateAfterNow)
                    {
                        DateTime currentTime = DateTime.Now;
                        currentTime = currentTime.AddMinutes(-1);

                        if (date>= currentTime)
                        {
                            fromDate = date;
                            setDateValues(date, fromDay, fromMonth, fromYear, fromHour, fromMinute, DateType.START_DATE);
                        }
                    }
                    else
                        if (IsDateRangeValid(date, endDate))
                        {
                        fromDate = date;
                        setDateValues(date, fromDay, fromMonth, fromYear, fromHour, fromMinute, DateType.START_DATE);
                        }
                }
                else
                {
                    if (IsDateRangeValid(fromDate, date))
                    {
                        endDate = date;
                        setDateValues(date, endDay, endMonth, endYear, endHour, endMinute, DateType.END_DATE);
                    }
                }
            }, senderViewId, dateTime, withTime);

            frag.Show(mFragmentManager, DatePickerFragment.TAG);
        }

        /// <summary>
        /// Utility method per controllare le date selezionate: valida la nuova data selezionata se fromDate è precedente a endDate
        /// </summary>
        /// <returns><c>true</c>, if dates was checked, <c>false</c> otherwise.</returns>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        bool IsDateRangeValid(DateTime start, DateTime end)
        {
            return !start.IsSet() || !end.IsSet() || DateTime.Compare(start, end) <= 0 ;     
        }

        /// <summary>
        /// Metodo utilizzato per settare una data alle rispettive edittext
        /// </summary>
        void setDateValues(DateTime date, TextView day, TextView month, TextView year, TextView hour, TextView minute, DateType type)
        {
            day.Text = date.Day.ToString();
            month.Text = date.Month.ToString();
            year.Text = date.Year.ToString();
            hour.Text = date.Hour.ToString("00");
            minute.Text = date.Minute.ToString("00");

            mListener.OnDateSelected(type, date, this.Id);
        }

        /// <summary>
        /// Sets from date.
        /// </summary>
        /// <param name="date">Date.</param>
        public void setFromDate(DateTime date)
        {
            InitFromDate(date);
            mListener.OnDateSelected(DateType.START_DATE, date, this.Id);
        }

        /// <summary>
        /// Sets the end date.
        /// </summary>
        /// <param name="date">Date.</param>
        public void setEndDate(DateTime date)
        {
            InitEndDate(date);
            mListener.OnDateSelected(DateType.END_DATE, date, this.Id);
        }

        /// <summary>
        /// Init from date, without calling listener method.
        /// </summary>
        /// <param name="date">Date.</param>
        public void InitFromDate(DateTime date)
        {
            if (!date.IsSet())
            {
                return;
            }

            fromDate = date;

            fromDay.Text = date.Day.ToString();
            fromMonth.Text = date.Month.ToString();
            fromYear.Text = date.Year.ToString();
        }

        /// <summary>
        /// Init end date, without calling listener method.
        /// </summary>
        /// <param name="date">Date.</param>
        public void InitEndDate(DateTime date)
        {
            if (!date.IsSet())
            {
                return;
            }

            endDate = date;

            endDay.Text = date.Day.ToString();
            endMonth.Text = date.Month.ToString();
            endYear.Text = date.Year.ToString();
        }

        #region getters

        public DateTime getFromDate()
        {
            return fromDate;
        }

        public DateTime getEndDate()
        {
            return endDate;
        }

        #endregion


        #region inner class

        public enum DateType
        {
            START_DATE,
            END_DATE
        }

        public interface IDateRangeSelectedListener
        {
            void OnDateSelected(DateType dateType, DateTime date, int idView);
        }

        public class DatePickerFragment : DialogFragment,
                                  DatePickerDialog.IOnDateSetListener,
                                    TimePickerDialog.IOnTimeSetListener
        {
            public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();

            int senderId;
            DateTime date;
            bool withTime;

            DateTime selectedDate;

            // Initialize this value to prevent NullReferenceExceptions.
            Action<DateTime, int> _dateSelectedHandler = delegate { };

            /// <summary>
            /// Metodo da invocare per inizializzare il DatePickerFragment
            /// </summary>
            /// <returns>The instance.</returns>
            /// <param name="onDateSelected">Callback da invocare su OnDateSet</param>
            /// <param name="senderId">Id della view invocante, utilizzato nella callback OnDateSet</param>
            /// <param name="date">La data da settare nel DatePickerFragment; se è null, viene utilizzata la data corrente</param>
            public static DatePickerFragment NewInstance(Action<DateTime, int> onDateSelected, int senderId, DateTime date, bool withTime)
            {
                DatePickerFragment frag = new DatePickerFragment();
                frag._dateSelectedHandler = onDateSelected;
                frag.senderId = senderId;
                frag.withTime = withTime;
                frag.date = date;
                return frag;
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                if (!date.IsSet())
                    date = DateTime.Now;

                DatePickerDialog dialog = new DatePickerDialog(Activity,
                                                               this,
                                                               date.Year,
                                                               date.Month - 1,
                                                               date.Day);
                return dialog;
            }

            public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
            {
                selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
                if (withTime)
                {
                    new TimePickerDialog(Activity, this, date.Hour, date.Minute, true).Show();
                }
                else
                {
                    // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
                    _dateSelectedHandler(selectedDate, senderId);
                }
            }

            public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
            {
                selectedDate = selectedDate
                    .AddHours(hourOfDay)
                    .AddMinutes(minute);
                _dateSelectedHandler(selectedDate, senderId);
            }
        }

        #endregion
    }
}
