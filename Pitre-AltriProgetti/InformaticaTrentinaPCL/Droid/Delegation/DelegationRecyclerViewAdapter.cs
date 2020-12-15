using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

using InformaticaTrentinaPCL.Delega.MVP;
using InformaticaTrentinaPCL.Droid.Core;
using InformaticaTrentinaPCL.Droid.Utils;
using InformaticaTrentinaPCL.Home;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Droid.Delegation
{
    public class DelegationRecyclerViewAdapter : RecyclerView.Adapter
    {
        IDelegationRecyclerViewItemClickListener listener;
        List<DelegaDocumentModel> delegationList
            ;

        private readonly int ITEMS_BEFORE_END_TO_START_INFINITE_SCROLL = 3;

        public override int ItemCount => delegationList == null ? 0 : 
                                         delegationList.Count == 0 ? 1 : 
                                         delegationList.Count;
        
        public DelegationRecyclerViewAdapter(IDelegationRecyclerViewItemClickListener listener)
        {
            this.listener = listener;
        }

        public override int GetItemViewType(int position)
        {
            if (delegationList == null)
            {
                return (int) CellType.FIRST_LOAD;
            }

            if (delegationList.Count == 0)
            {
                return (int) CellType.EMPTY;
            }

            return (int) CellType.MANDATE;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder.ItemViewType == (int) CellType.MANDATE)
            {
                var viewHolder = (DelegationViewHolder) holder;
                viewHolder.position = position;

                var item = delegationList[position];

                viewHolder.infoDelega.Text = item.delegato;
                viewHolder.dataDecorrenza.Text = item.dataDecorrenzaDelega;
                viewHolder.dataScadenzaValue.Text = item.dataScadenzaDelega;
                viewHolder.dataScadenzaValue.Visibility = EndDateHelper.CheckVisibilityEndDate(item) 
                                                                        ? ViewStates.Invisible : ViewStates.Visible;
                viewHolder.delegante.Text = item.delegante;
                viewHolder.role.Text = item.ruoloDelegato;

                if (listener != null && position == delegationList.Count - ITEMS_BEFORE_END_TO_START_INFINITE_SCROLL)
                {
                    listener.OnLastDelegationInListShown();
                }
            }
        }

        public DelegaDocumentModel GetDocumentModelAt(int position)
        {
            if(position < 0 || position >= delegationList.Count){
                return null;
            }
            return delegationList.ElementAt(position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case (int) CellType.EMPTY:
                {
                    Context context = parent.Context;
                    View v = LayoutInflater.From(context).Inflate(Resource.Layout.view_empty_list, parent, false);
                    return new EmptyViewViewHolder(v);
                }

                case (int) CellType.MANDATE:
                {
                    LinearLayout layout = new LinearLayout(parent.Context);
                    layout.Orientation = Orientation.Horizontal;

                    return new DelegationViewHolder(parent, layout, listener);
                }
            }

            return null;
        }

        /// <summary>
        /// Metodo utilizzato per aggiungere items alla recyclerview.
        /// </summary>
        /// <param name="newDelegationList">New delegation list.</param>
        public void addDelegationList(List<DelegaDocumentModel> newDelegationList)
        {
            if (delegationList == null)
            {
                delegationList = new List<DelegaDocumentModel>();
            }
            else
            {
                delegationList.Clear();
            }
            delegationList.AddRange(newDelegationList);
            NotifyDataSetChanged();
        }

        #region Inner classes

        protected class DelegationViewHolder : RecyclerView.ViewHolder
        {
            public int position;
            IDelegationRecyclerViewItemClickListener listener;
            Context context;
            ViewGroup parent;

            public TextView infoDelega;
            public TextView dataDecorrenza;
            public TextView dataScadenzaLabel;
            public TextView dataScadenzaValue;
            public TextView delegante;
            public TextView role;

            View leftView;
            View rightView;
            public DelegationViewHolder(ViewGroup parent, View view, IDelegationRecyclerViewItemClickListener listener) : base(view)
            {
                this.context = ItemView.Context;
                this.parent = parent;
                this.ItemView = view;
                this.listener = listener;

                leftView = GenerateLeftView();
                rightView = GenerateRightView();

                ((LinearLayout)ItemView).AddView(leftView);
                ((LinearLayout)ItemView).AddView(rightView);

            }

            protected View GenerateLeftView()
            {

                var vi = LayoutInflater.From(context);
                var v = vi.Inflate(Resource.Layout.item_delegation, parent, false);

                infoDelega = v.FindViewById<TextView>(Resource.Id.textView_infoDelega);
                dataDecorrenza= v.FindViewById<TextView>(Resource.Id.textView_dataDecorrenza);
                dataScadenzaValue = v.FindViewById<TextView>(Resource.Id.textView_dataScadenza);
                dataScadenzaLabel = v.FindViewById<TextView>(Resource.Id.textView_dataScadenza_str);            
                delegante = v.FindViewById<TextView>(Resource.Id.textView_delegante);
                role = v.FindViewById<TextView>(Resource.Id.textView_role);

                v.Click += (sender, e) =>
                {

                    int[] outLocation = { 0, 0, 0, 0 };
                    rightView.GetLocationOnScreen(outLocation);

                    if (outLocation[0] >= context.Resources.DisplayMetrics.WidthPixels)
                        listener.OnItemClick(position);
                };

                return v;
            }

            protected View GenerateRightView()
            {

                Color redColor = UIUtility.GetColor(context, Resource.Color.colorRed);
                Color whiteColor = UIUtility.GetColor(context, Resource.Color.colorWhite);
                int slideButtonWidth = (int)context.Resources.GetDimension(Resource.Dimension.delegation_list_item_slide_button_width);

                FrameLayout.LayoutParams frameParams = new FrameLayout.LayoutParams(slideButtonWidth, ViewGroup.LayoutParams.MatchParent);

                FrameLayout.LayoutParams tvParams = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                tvParams.Gravity = GravityFlags.Center;

                FrameLayout backgroundFrame = new FrameLayout(context);
                backgroundFrame.LayoutParameters = frameParams;
                backgroundFrame.SetBackgroundColor(redColor);

                TextView tvRevoke = new TextView(context);
                tvRevoke.SetText(Resource.String.item_delegation_revoke_delegation);
                tvRevoke.SetTextColor(whiteColor);
                tvRevoke.Gravity = GravityFlags.Center;
                tvRevoke.LayoutParameters = tvParams;

                backgroundFrame.Click += (sender, e) =>
                {
                    listener.OnRevocationClick(position);
                };

                backgroundFrame.AddView(tvRevoke);

                return backgroundFrame;
            }
        }
        
        public enum CellType
        {
            MANDATE,
            EMPTY,
            FIRST_LOAD
        }

        public interface IDelegationRecyclerViewItemClickListener
        {
            void OnItemClick(int position);
            void OnRevocationClick(int position);
            void OnLastDelegationInListShown();
        }
        #endregion
    }
}
