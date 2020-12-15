using System.Collections.Generic;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Content;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Droid.Assign;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Utils;


namespace InformaticaTrentinaPCL.Droid.Delegation
{
    public class RecipientRecyclerViewAdapter : RecyclerView.Adapter
    {
	    private bool isFavoriteDisabled;
	    private List<AbstractRecipient> recipients;
	    IRecipientRecyclerViewItemClickListener listener;
	    private TextView noResult;
	    private string emptyMessage = "";

	    public override int ItemCount => recipients == null ? 0 : 
		    recipients.Count == 0 ? 1 : 
		    recipients.Count;

	    public RecipientRecyclerViewAdapter(IRecipientRecyclerViewItemClickListener listener, bool isFavoriteDisabled = false)
	    {
		    this.listener = listener;
		    this.isFavoriteDisabled = isFavoriteDisabled;
	    }
	    
	    public override int GetItemViewType(int position)
	    {
		    if (recipients == null)
		    {
			    return (int) CellType.FIRST_LOAD;
		    }

		    if (recipients.Count == 0)
		    {
			    return (int) CellType.EMPTY;
		    }

		    return (int) CellType.ASSIGNEE;
	    }
	    
	    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
	    {
		    if (holder.ItemViewType == (int) CellType.ASSIGNEE)
		    {
			    var viewHolder = (RecipientViewHolder) holder;

			    var recipient = recipients[position];

			    viewHolder.disableCheckedChange = true;

			    viewHolder.recipient = recipient;

			    UpdateFavoriteIcon(viewHolder.favoriteCheckBox, recipient.isPreferred());
			    UpdateText(viewHolder.recipientTitle, recipient.getTitle(), recipient.getSubtitle());
			    AssigneeUiHelper.UpdateIcon(viewHolder.recipientImage, recipient.getRecipientType());
			    viewHolder.disableCheckedChange = false;
		    }
		    else
		    {
			    var viewHolder = (EmptyViewViewHolder) holder;
			    viewHolder.message.Text = emptyMessage;
		    }
		    
	    }

	    private void UpdateFavoriteIcon(CheckBox favoriteIcon, bool isFavorite)
	    {
		    if (isFavoriteDisabled)
		    {
			    favoriteIcon.Visibility = ViewStates.Gone;
		    }
		    else
		    {
			    favoriteIcon.Visibility = ViewStates.Visible;
			    favoriteIcon.Checked = isFavorite;			    
		    }
	    }

	    private void UpdateText(TextView text, string title, string subtitle)
	    {
		    UIUtility.setTitleSubtitle(text, title, subtitle);
		    UIUtility.SetTextColor(text, Resource.Color.textBlue);
	    }

	    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
	    {
		    /**
		    var vi = LayoutInflater.From(parent.Context);
		    var v = vi.Inflate(Resource.Layout.item_assignable, parent, false);

		    return new RecipientViewHolder(v, listener);
		    **/
		    
		    switch (viewType)
		    {
			    case (int) CellType.EMPTY:
			    {
				    Context context = parent.Context;
				    View v = LayoutInflater.From(context).Inflate(Resource.Layout.view_text_empty_list, parent, false);
				    return new EmptyViewViewHolder(v);
			    }

			    case (int) CellType.ASSIGNEE:
			    {
				/**    LinearLayout layout = new LinearLayout(parent.Context);
				    layout.Orientation = Orientation.Horizontal;**/
				    
				    var vi = LayoutInflater.From(parent.Context);
				    var v = vi.Inflate(Resource.Layout.item_assignable_for_adapter, parent, false);

				    return new RecipientViewHolder(v, listener);
			    }
		    }

		    return null;
	    }

	    public void UpdateRecipient(AbstractRecipient recipient)
	    {
		    var recipientToRollback = recipients.Find(x => x.getId() == recipient.getId());
		    if (recipientToRollback != null)
		    {
			    recipientToRollback.setPreferred(!recipientToRollback.isPreferred());
			    NotifyDataSetChanged();
		    }
	    }

	    public void OnDatasetChanged(List<AbstractRecipient> newDataset, ListType type = ListType.NULL)
	    {
		    if (type == ListType.NULL)
		    {
			    recipients = null;
		    }
		    else
		    {
			    emptyMessage =
				    type == ListType.FAVORITE ? LocalizedString.EMPTY_LIST_FAVORITE.Get() :
				    type == ListType.MODEL ? LocalizedString.EMPTY_LIST_MODEL.Get() :
				    type == ListType.SEARCH ? LocalizedString.EMPTY_LIST_RESULT.Get() :
				    //type == ListType.ROLE ? LocalizedString.EMPTY_LIST_ROLE.Get() : // Predisposto | da valutare
				     		"";
			    
			    if (recipients == null)
			    {
				    recipients = new List<AbstractRecipient>();
			    }
			    else
			    {
				    recipients.Clear();
			    }
			    recipients.AddRange(newDataset);
		    }
		    
		    NotifyDataSetChanged();
	    }

	    protected class RecipientViewHolder : RecyclerView.ViewHolder
	    {

		    public bool disableCheckedChange;
		    public AbstractRecipient recipient;

		    public readonly ImageView recipientImage;
		    public readonly TextView recipientTitle;
		    public readonly CheckBox favoriteCheckBox;

		    public RecipientViewHolder(View view, IRecipientRecyclerViewItemClickListener listener) : base(view)
		    {
			    ItemView = view;
			    recipientImage = ItemView.FindViewById<ImageView>(Resource.Id.imageView_assignableImage);
			    recipientTitle = ItemView.FindViewById<TextView>(Resource.Id.textView_assignableName);
			    favoriteCheckBox = ItemView.FindViewById<CheckBox>(Resource.Id.checkbox_favorite);

			    ItemView.Click += delegate
			    {
				    listener.OnClick(recipient);
			    };
			    
			    favoriteCheckBox.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs args)
			    {
				    if (!disableCheckedChange)
				    {
					    bool val = ((CheckBox) sender).Checked;
					    recipient.setPreferred(val);
					    listener.OnFavoriteClick(recipient, val);
				    }					    
			    };
			    
		    }
	    }
	    
	    public enum CellType
	    {
		    ASSIGNEE,
		    EMPTY,
		    FIRST_LOAD
	    }
	    
	    public interface IRecipientRecyclerViewItemClickListener
	    {
		    void OnClick(AbstractRecipient selectedRecipient);
		    void OnFavoriteClick(AbstractRecipient selectedRecipient, bool isChecked);
	    }

    }
}
