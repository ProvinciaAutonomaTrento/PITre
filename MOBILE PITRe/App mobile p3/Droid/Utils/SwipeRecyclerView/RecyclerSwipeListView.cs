using System;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace InformaticaTrentinaPCL.Droid.Utils.SwipeRecyclerView
{
    public class RecyclerSwipeListView : RecyclerView
    {

        CustomBool mIsHorizontal;
        View mPreItemView;
        View mCurrentItemView;
        float mFirstX;
        float mFirstY;

        int mRightViewWidth = 0;
        const int mDuration = 100;
        const int mDurationStep = 10;
        bool mIsShown;
        bool mSwipeEnable = true;

        public RecyclerSwipeListView(Context context) : base(context)
    	{
    		
    	}

        public RecyclerSwipeListView(Context context, IAttributeSet attrs) : base(context, attrs)
    	{
    		
    	}

        public RecyclerSwipeListView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (!mSwipeEnable)
                return base.OnInterceptTouchEvent(ev);

            float lastX = ev.GetX();
            float lastY = ev.GetY();
            switch (ev.Action)
            {
                case MotionEventActions.Down:
                    mIsHorizontal = null;

                    mFirstX = lastX;
                    mFirstY = lastY;

                    int motionPosition = pointToPosition((int)mFirstX, (int)mFirstY);

                    if (motionPosition >= 0)
                    {
                        View currentItemView = GetChildAt(motionPosition);
                        mPreItemView = mCurrentItemView;
                        mCurrentItemView = currentItemView;

                        if ((mRightViewWidth == 0) && (currentItemView.GetType() == typeof(LinearLayout))) 
                            mRightViewWidth = ((LinearLayout)currentItemView).GetChildAt(1).Width;
                        
                    }
                    break;

                case MotionEventActions.Move:
                    float dx = lastX - mFirstX;
                    float dy = lastY - mFirstY;

                    if (Math.Abs(dx) >= 5 && Math.Abs(dy) >= 5)
                        return true;

                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    if (mIsShown && (mPreItemView != mCurrentItemView || isHitCurItemLeft(lastX)))
                        hiddenRight(mPreItemView);
                    
                break;
            }

            return base.OnInterceptTouchEvent(ev);
		}


    	bool isHitCurItemLeft(float x)
    	{
            return x < Width - mRightViewWidth;
    	}

        /**
         * @param dx
         * @param dy
         * @return judge if can judge scroll direction
         */
        private bool judgeScrollDirection(float dx, float dy)
        {
        	bool canJudge = true;

        	if (Math.Abs(dx) > 30 && Math.Abs(dx) > 2 * Math.Abs(dy))
        	{
                mIsHorizontal = mIsHorizontal == null ? new CustomBool() : mIsHorizontal;                    
                mIsHorizontal.boolValue = true;
        	}
        	else if (Math.Abs(dy) > 30 && Math.Abs(dy) > 2 * Math.Abs(dx))
        	{
                mIsHorizontal = mIsHorizontal == null ? new CustomBool() : mIsHorizontal;
                mIsHorizontal.boolValue = false;
        	}
        	else
        	{
        		canJudge = false;
        	}

        	return canJudge;
        }

		/**
		 * return false, can't move any direction. return true, cant't move vertical, can move
		 * horizontal. return super.onTouchEvent(ev), can move both.
        */
		public override bool OnTouchEvent(MotionEvent ev)
        {
            if (!mSwipeEnable || !(mCurrentItemView is LinearLayout))
                return base.OnTouchEvent(ev);


            float lastX = ev.GetX();
			float lastY = ev.GetY();

            switch (ev.Action)
			{
                case MotionEventActions.Down:
					break;

                case MotionEventActions.Move:
					float dx = lastX - mFirstX;
					float dy = lastY - mFirstY;

					// confirm is scroll direction
					if (mIsHorizontal == null)
					{
						if (!judgeScrollDirection(dx, dy))
						{
							break;
						}
					}

                    if (mIsHorizontal.boolValue)
					{
						if (mIsShown && mPreItemView != mCurrentItemView)
						{
							hiddenRight(mPreItemView);
						}

						if (mIsShown && mPreItemView == mCurrentItemView)
						{
							dx = dx - mRightViewWidth;
						}

						// can't move beyond boundary
						if (dx < 0 && dx > -mRightViewWidth)
						{
                            mCurrentItemView.ScrollTo((int)(-dx), 0);
						}

						return true;
					}
					else
					{
						if (mIsShown)
						{
							hiddenRight(mPreItemView);
						}
					}

					break;

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
					
					clearPressedState();
					if (mIsShown)
					{
						hiddenRight(mPreItemView);
					}

                    if (mIsHorizontal != null && mIsHorizontal.boolValue)
					{
						if (mFirstX - lastX > mRightViewWidth / 2)
						{
							showRight(mCurrentItemView);
						}
						else
						{
							hiddenRight(mCurrentItemView);
						}

						return true;
					}
					break;
			}

            return base.OnTouchEvent(ev);
        }

        public void resetSwipe()
    	{
    		if (mCurrentItemView != null)
    		{
    			hiddenRight(mCurrentItemView);
    		}
    	}

    	void clearPressedState()
    	{
            if(mCurrentItemView != null)
                mCurrentItemView.Pressed = false;
            Pressed = false;
    		RefreshDrawableState();
    		// invalidate();
    	}

        void showRight(View view)
        {
            Message msg = new MoveHandler().ObtainMessage();
            msg.Obj = view;
            msg.Arg1 = view.ScrollX;
        	msg.Arg2 = mRightViewWidth;
        	msg.SendToTarget();

        	mIsShown = true;
        }

    	void hiddenRight(View view)
    	{
    		if (mCurrentItemView == null)
    		{
    			return;
    		}
                
            Message msg = new MoveHandler().ObtainMessage();//
    		msg.Obj = view;
            msg.Arg1 = view.ScrollX;
    		msg.Arg2 = 0;

    		msg.SendToTarget();

    		mIsShown = false;
    	}

    	public bool isSwipeEnable()
    	{
    		return mSwipeEnable;
    	}

    	public void setSwipeEnable(bool swipeEnable)
    	{
    		this.mSwipeEnable = swipeEnable;
    	}

        /**
         * show or hide right layout animation
         */
        class MoveHandler : Handler
	    {
            int stepX = 0;
            int fromX;
            int toX;
    		View view;

		    bool mIsInAnimation = false;

        	void animatioOver()
        	{
        		mIsInAnimation = false;
        		stepX = 0;
        	}

            public override void HandleMessage(Message msg)
            {
                base.HandleMessage(msg);
                if (stepX == 0)
                {
                    if (mIsInAnimation)
                        return;
                    
                    mIsInAnimation = true;
                    view = (View)msg.Obj;
                    fromX = msg.Arg1;
                    toX = msg.Arg2;
                    stepX = (int)((toX - fromX) * mDurationStep * 1.0 / mDuration);

                    if (stepX < 0 && stepX > -1)
                        stepX = -1;
                    else if (stepX > 0 && stepX < 1)
                        stepX = 1;
                    
                    if (Math.Abs(toX - fromX) < 10)
                    {
                        view.ScrollTo(toX, 0);
                        animatioOver();
                        return;
                    }
                }

                fromX += stepX;
                bool isLastStep = (stepX > 0 && fromX > toX) || (stepX < 0 && fromX < toX);

                if (isLastStep)
                    fromX = toX;

                view.ScrollTo(fromX, 0);

                //TODO?invalidate();

                if (!isLastStep)
                    this.SendEmptyMessageDelayed(0, mDurationStep);
                else
                    animatioOver();
                
			}
        }

        /**
         * Maps a point to a position in the list.
         *
         * @param x X in local coordinate
         * @param y Y in local coordinate
         * @return The position of the item which contains the specified point, or -1
         */
        Rect mTouchFrame;
        public int pointToPosition(int x, int y)
        {
        	Rect frame = mTouchFrame;


        	if (frame == null)
        	{
        		mTouchFrame = new Rect();
        		frame = mTouchFrame;
        	}

            int count = ChildCount;
        	for (int i = count - 1; i >= 0; i--)
        	{
                View child = GetChildAt(i);
                if (child.Visibility == ViewStates.Visible)
        		{
        			child.GetHitRect(frame);
                    if (frame.Contains(x, y))
        			{
        				return i;
        			}
        		}
        	}
        	return -1;
        }

        class CustomBool
        {
            public bool boolValue;
        }

    }

}
