using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InformaticaTrentinaPCL.Droid.Delegation;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Resources;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Droid.Assign.Assignable
{
    public class AssignablesFragment : Fragment
    {
	    RelativeLayout emptyListLayout;
	    TextView emptyListTextView;
        RecyclerView assignableRecyclerView;
	    RecipientRecyclerViewAdapter adapter;
		private const string LIST_TYPE = "LIST_TYPE";
		public static AssignablesFragment CreateInstance()
		{
			return new AssignablesFragment();
		}

		public AssignablesFragment(){}

	    public override void OnCreate(Bundle savedInstanceState)
	    {
		    base.OnCreate(savedInstanceState);
		    RetainInstance = true;
	    }

	    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			return inflater.Inflate(Resource.Layout.fragment_assignables, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
			base.OnViewCreated(view, savedInstanceState);

	        emptyListLayout = View.FindViewById<RelativeLayout>(Resource.Id.empty_list_layout);
	        emptyListTextView = View.FindViewById<TextView>(Resource.Id.empty_list_textview);
            assignableRecyclerView = View.FindViewById<RecyclerView>(Resource.Id.recyclerView_favoritesList);
			assignableRecyclerView.HasFixedSize = true;
			assignableRecyclerView.SetLayoutManager(new LinearLayoutManager(Context));
	        
	        ((SelectAssigneeActivity)Activity).notifyFragmentReady();
        }

	    public void showList(List<AbstractRecipient> assignables, 
		    			     RecipientRecyclerViewAdapter.IRecipientRecyclerViewItemClickListener listener,
		    				 ListType type)
	    {
		    if (adapter == null)
		    {
			    adapter = new RecipientRecyclerViewAdapter(listener, type == ListType.MODEL);
		    }
		    
		    assignableRecyclerView.SetAdapter(adapter);
		    adapter.OnDatasetChanged(assignables, type);
		    
		    switch (assignables.Count)
		    {
				    case 0:
					    emptyListLayout.Visibility = ViewStates.Visible;
					    assignableRecyclerView.Visibility = ViewStates.Gone;					    
					    emptyListTextView.Text = type.Equals(ListType.MODEL)? 
						    LocalizedString.EMPTY_LIST_MODEL.Get() : LocalizedString.EMPTY_LIST_FAVORITE.Get();							    			  
					    break;
					    
					default:
						emptyListLayout.Visibility = ViewStates.Gone;
						assignableRecyclerView.Visibility = ViewStates.Visible;
						break;
		    }
		    		    
	    }
	    
	    public void UpdateRecipient(AbstractRecipient recipient)
	    {
		    adapter.UpdateRecipient(recipient);
	    }	    
    }
}
