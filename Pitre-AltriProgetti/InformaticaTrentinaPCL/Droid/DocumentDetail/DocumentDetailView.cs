using System;
using Android.Content;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Assegna;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Droid.DocumentDetail
{
    public class DocumentDetailView : FrameLayout
    {
        Context mContext;
        TextView documentTitle;
        TextView sendDate;
        TextView sentBy;
        TextView sentFor;

	    TextView sentByLabel, sentForLabel;

		public DocumentDetailView(Context context) : base(context)
        {
			init(context);
		}
		public DocumentDetailView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
			init(context);
		}

		public DocumentDetailView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
			init(context);
		}

		private void init(Context ctx) 
        {
			mContext = ctx;

			Inflate(mContext, Resource.Layout.view_document_detail, this);

			documentTitle = FindViewById<TextView>(Resource.Id.textView_documentTitle);
            sendDate = FindViewById<TextView>(Resource.Id.textView_sendDate);
            sentBy = FindViewById<TextView>(Resource.Id.textView_sentBy);
            sentFor = FindViewById<TextView>(Resource.Id.textView_sentFor);
	        
	        sentByLabel = FindViewById<TextView>(Resource.Id.textView_sentByStr);
	        sentForLabel = FindViewById<TextView>(Resource.Id.textView_sentForStr);
            ShowSelectRagione(false);
            
		}

        public void showDocumentDetails(AbstractDocumentListItem abstractDocument, IRagioneClickListener listener = null) 
        {

            documentTitle.Text = abstractDocument.GetOggetto();
            sendDate.Text = abstractDocument.GetData();
            sentBy.Text = abstractDocument.GetMittente();
	        
	        if (string.IsNullOrEmpty(sentBy.Text))
	        {
		        sentByLabel.Visibility = ViewStates.Invisible;
		        sentBy.Visibility = ViewStates.Invisible;

		        sentByLabel.LayoutParameters.Height = (int)Resources.GetDimension(Resource.Dimension.dimen_1px);
		        sentBy.LayoutParameters.Height = (int)Resources.GetDimension(Resource.Dimension.dimen_1px);
	        }
	        else
	        {
		        sentByLabel.Visibility = ViewStates.Visible;
		        sentBy.Visibility = ViewStates.Visible;
		        
		        sentByLabel.LayoutParameters.Height = LayoutParams.WrapContent;
		        sentBy.LayoutParameters.Height = LayoutParams.WrapContent;
	        }
	        
	        if (listener == null)
	        {
		        sentFor.Text = abstractDocument.GetInfo();
		        if (string.IsNullOrEmpty(sentFor.Text))
		        {
			        sentForLabel.Visibility = ViewStates.Gone;
			        sentFor.Visibility = ViewStates.Gone;
		        }
		        else
		        {
			        sentForLabel.Visibility = ViewStates.Visible;
			        sentFor.Visibility = ViewStates.Visible;
		        }
		        
	        }
	        else
	        {
		        SpannableString ss = new SpannableString(LocalizedString.SELEZIONA_RAGIONE.Get());
		        ss.SetSpan(new TextAppearanceSpan(Context, Resource.Style.link_to_page), 0, ss.Length(), SpanTypes.ExclusiveExclusive );
		        sentFor.SetText(ss, TextView.BufferType.Spannable);
		        sentFor.Click += delegate(object sender, EventArgs args) 
		        { 
			        listener.onRagioneClicked();
		        };
	        }
        }

	    public void SetRagione(Ragione ragione)
	    {
		    SpannableString ss = new SpannableString(ragione.ToString());
		    ss.SetSpan(new TextAppearanceSpan(Context, Resource.Style.details_page_details), 0, ss.Length(), SpanTypes.ExclusiveExclusive );
		    sentFor.SetText(ss, TextView.BufferType.Spannable);
	    }

	    public string GetRagione()
	    {
		    return sentFor.Text;
	    }

        public void ShowSelectRagione(bool visibility)
        {
            sentForLabel.Visibility = visibility? ViewStates.Visible: ViewStates.Gone;
            sentFor.Visibility = visibility? ViewStates.Visible: ViewStates.Gone;
        }

	    public interface IRagioneClickListener
	    {
		    void onRagioneClicked();
	    }
	    
	    
    }
}
