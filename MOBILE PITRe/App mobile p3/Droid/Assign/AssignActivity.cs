using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;
using InformaticaTrentinaPCL.Assegna;
using InformaticaTrentinaPCL.Assegna.MVPD;
using InformaticaTrentinaPCL.Assign;
using InformaticaTrentinaPCL.Droid.ActionCompleted;
using InformaticaTrentinaPCL.Droid.Assign.Role;
using InformaticaTrentinaPCL.Droid.CustomBottomSheet;
using InformaticaTrentinaPCL.Droid.DocumentDetail;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Interfaces;
using InformaticaTrentinaPCL.Search;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Droid.Assign
{

    [Activity(Label = "@string/app_name")]
    public class AssignActivity : BaseActivity, AssigneeView.IAssigneeViewListener, IAssegnaView, DocumentDetailView.IRagioneClickListener
    {
        public const string ASSIGNEE_KEY = "ASSIGNEE_KEY";

        public const string KEY_CONFIGURE_ACTIVITY_FOR = "KEY_CONFIGURE_ACTIVITY_FOR";
        public const string KEY_DOCUMENT = "KEY_DOCUMENT";

        ActionType actionType;
        AbstractDocumentListItem _abstractDocument = null;

        TextView headerTextView;
        DocumentDetailView documentDetailView;
        AssigneeView assigneeView;
        EditText notesEditText;
        Button confirmButton;
        
        CustomLoaderUtility loaderUtility;

        IAssegnaPresenter presenter;

        protected override int LayoutResource => Resource.Layout.activity_assign;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            getIntentParams();

            presenter = new AssegnaPresenter(this, AndroidNativeFactory.Instance(),_abstractDocument);
            
            loaderUtility = new CustomLoaderUtility();
            
            Toolbar.SetNavigationIcon(Resource.Drawable.ic_ico_back_grigia);

            Toolbar.NavigationClick += delegate
            {
                Finish();
            };

            headerTextView = FindViewById<TextView>(Resource.Id.textView_header);
            documentDetailView = FindViewById<DocumentDetailView>(Resource.Id.documentDetailView);
            assigneeView = FindViewById<AssigneeView>(Resource.Id.assigneeView_container);
            notesEditText = FindViewById<EditText>(Resource.Id.editText_notes);
            confirmButton = FindViewById<Button>(Resource.Id.button_confirm);

            SetSectionTitle(actionType.titleBar);

            headerTextView.Text = actionType.SetDescriptionForTypeDocument(actionType.description, _abstractDocument.tipoDocumento);
            documentDetailView.showDocumentDetails(_abstractDocument, this);
            assigneeView.Listener = this;

            notesEditText.TextChanged += delegate
            {
                presenter.UpdateNote(notesEditText.Text);
            };

            confirmButton.Text = actionType.confirmButton;
            confirmButton.Click += delegate
            {
                presenter.Trasmetti();
            };
        }
        
        

        protected override void OnStart()
        {
            base.OnStart();
            presenter.OnViewReady();
        }

        void getIntentParams()
        {
            int actionTypeId = Intent.GetIntExtra(KEY_CONFIGURE_ACTIVITY_FOR, -1);
            string stringDocument = Intent.GetStringExtra(KEY_DOCUMENT);

            if ( actionTypeId < 0 || string.IsNullOrEmpty(stringDocument) )
                throw new Exception("One or more params missing in AssignActivity invocation");

            actionType = ActionType.GetFromId(actionTypeId);

            _abstractDocument = AbstractDocumentListItemHelper.DeserializeAbstractDocumentListItem(stringDocument);
            
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                AbstractRecipient recipient = AbstractRecipientHelper.DeserializeAbstractRecipient(data.GetStringExtra(ASSIGNEE_KEY));
                assigneeView.ShowRemovableAssignee(recipient);
            }
        }

        #region interface IAssigneeViewListener

        public void SelectAssigneeClick()
        {
            Intent intent = new Intent(this, typeof(SelectAssigneeActivity));
            intent.PutExtra(KEY_DOCUMENT, JsonConvert.SerializeObject(_abstractDocument));
            StartActivityForResult(intent, (int)ActivityRequestCodes.SelectAssignee);
        }

        public void RemoveAssigneeClick()
        {
//            presenter.UpdateAssegnatario(null); //commentato perché richiamato dal metodo AssigneeUpdated
        }

        public void AssigneeUpdated(AbstractRecipient assignee)
        {
            presenter.UpdateAssegnatario(assignee);
        }

        #endregion

        #region IAssegnaView

        public void ShowError(string e, bool isLight)
        {
            ShowErrorHelper.Show(this, e, isLight);
        }

        public void OnUpdateLoader(bool isShow)
        {
            if (isShow)
            {
                loaderUtility.showLoader(this);
            }
            else
            {
                loaderUtility.hideLoader();
            }
        }

        public void EnableButton(bool enabled)
        {
            confirmButton.Enabled = enabled;
        }

        public void OnAssegnaOk(Dictionary<string, string> extra)
        {
            SetResult(Result.Ok, Intent);

            Bundle bundle = Tools.ConvertDictionaryInBundle(extra);
            Intent intent = new Intent(this, typeof(ActionCompletedActivity));
            intent.PutExtra(ActionCompletedActivity.KEY_ACTION_TYPE, ActionType.ASSIGN.id);
            intent.PutExtras(bundle);

            StartActivity(intent);

            Finish();
        }
      
        public void ShowListaRagioni(List<Ragione> ragioni)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            ArrayAdapter<Ragione> adapter =
                new ArrayAdapter<Ragione>(this, Resource.Layout.simple_list_item);
            adapter.AddAll(ragioni);
            builder.SetCustomTitle(LayoutInflater.Inflate(Resource.Layout.view_lista_ragioni_title_dialog, null, false));
            builder.SetAdapter(adapter,
                delegate(object sender, DialogClickEventArgs args)
                {
                    Ragione ragione = ragioni.ElementAt(args.Which);
                    presenter.UpdateRagione(ragione);
                    documentDetailView.SetRagione(ragione);
                });
            builder.Show();

        }

        #endregion

        public void onRagioneClicked()
        {
            presenter.GetListaRagioni();
        }

        public void SelectAssigneeRoleClick()
        {
            throw new NotImplementedException();
        }

        public void ShowSelectRagione(bool visible)
        {
            documentDetailView.ShowSelectRagione(visible);
        }

        public bool ShowUserDefaultRole()
        {
            return true;
        }
    }
}
