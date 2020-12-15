using System;

namespace InformaticaTrentinaPCL.Home.ActionDialog
{
    public class DialogItem
    {
        public string Title { get; set; }
        public Action OnClickHandler = delegate { };

        public DialogItem(string title, Action onClickHandler)
        {
            Title = title;
            OnClickHandler = onClickHandler;
        }
    }
}