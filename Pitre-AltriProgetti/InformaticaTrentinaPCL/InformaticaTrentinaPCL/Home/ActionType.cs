using System;
using System.Collections.Generic;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Resource;
using InformaticaTrentinaPCL.Utils;
using Newtonsoft.Json;

namespace InformaticaTrentinaPCL.Home
{
    public class ActionType
    {
        static readonly ActionType[] actionTypes = {
            new ActionType(
                0,
                LocalizedString.ACTIONTYPE_ACCEPT_LABELMENU.Get(),
                LocalizedString.ACTIONTYPE_ACCEPT_TITLEBAR.Get(),
                LocalizedString.ACTIONTYPE_ACCEPT_DESCRIPTION.Get(),
                LocalizedString.ACTIONTYPE_ACCEPT_CONFIRMBUTTON.Get(),
                LocalizedString.ACTIONTYPE_ACCEPT_DONETEXT.Get(),
                LocalizedString.ACTIONTYPE_ACCEPT_DONEBUTTON.Get()
            ),
            new ActionType(
                1,
                LocalizedString.ACTIONTYPE_ASSIGN_LABELMENU.Get(),
                LocalizedString.ACTIONTYPE_ASSIGN_TITLEBAR.Get(),
                LocalizedString.ACTIONTYPE_ASSIGN_DESCRIPTION.Get(),
                LocalizedString.ACTIONTYPE_ASSIGN_CONFIRMBUTTON.Get(),
                LocalizedString.ACTIONTYPE_ASSIGN_DONETEXT.Get(),
                LocalizedString.ACTIONTYPE_ASSIGN_DONEBUTTON.Get()
            ),
            new ActionType(
                2,
                LocalizedString.ACTIONTYPE_ACCEPT_AND_ADL_LABELMENU.Get(),
                LocalizedString.ACTIONTYPE_ACCEPT_AND_ADL_TITLEBAR.Get(),
                LocalizedString.ACTIONTYPE_ACCEPT_AND_ADL_DESCRIPTION.Get(),
                LocalizedString.ACTIONTYPE_ACCEPT_AND_ADL_CONFIRMBUTTON.Get(),
                LocalizedString.ACTIONTYPE_ACCEPT_AND_ADL_DONETEXT.Get(),
                LocalizedString.ACTIONTYPE_ACCEPT_AND_ADL_DONEBUTTON.Get()
            ),
            new ActionType(
                3,
                LocalizedString.ACTIONTYPE_REFUSE_LABELMENU.Get(),
                LocalizedString.ACTIONTYPE_REFUSE_TITLEBAR.Get(),
                LocalizedString.ACTIONTYPE_REFUSE_DESCRIPTION.Get(),
                LocalizedString.ACTIONTYPE_REFUSE_CONFIRMBUTTON.Get(),
                LocalizedString.ACTIONTYPE_REFUSE_DONETEXT.Get(),
                LocalizedString.ACTIONTYPE_REFUSE_DONEBUTTON.Get()
            ),
            new ActionType(
                4,
                LocalizedString.ACTIONTYPE_SIGN_LABELMENU.Get(),
                LocalizedString.ACTIONTYPE_SIGN_TITLEBAR.Get(),
                LocalizedString.ACTIONTYPE_SIGN_DESCRIPTION.Get(),
                LocalizedString.ACTIONTYPE_SIGN_CONFIRMBUTTON.Get(),
                LocalizedString.ACTIONTYPE_SIGN_DONETEXT.Get(),
                LocalizedString.ACTIONTYPE_SIGN_DONEBUTTON.Get()
            ),
            new ActionType(
                5,
                LocalizedString.ACTIONTYPE_OPEN_LABELMENU.Get()
            ),
            new ActionType(
                6,
                LocalizedString.ACTIONTYPE_SHARE_LABELMENU.Get()
            ),
            new ActionType(
                7,
                LocalizedString.ACTIONTYPE_MANDATE_LABELMENU.Get(),
                LocalizedString.ACTIONTYPE_MANDATE_TITLEBAR.Get(),
                LocalizedString.ACTIONTYPE_MANDATE_DESCRIPTION.Get(),
                LocalizedString.ACTIONTYPE_MANDATE_CONFIRMBUTTON.Get(),
                LocalizedString.ACTIONTYPE_MANDATE_DONETEXT.Get(),
                LocalizedString.ACTIONTYPE_MANDATE_DONEBUTTON.Get()
            ),
            new ActionType(
                8,
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                LocalizedString.TITLE_DESCRIPTION_ACTION_COMPLETED.Get(),
                string.Empty
            ),
            new ActionType(
                9,
                string.Empty,
                LocalizedString.ACTIONTYPE_SIGN_WITH_OTP_TITLEBAR.Get(),
                string.Empty,
                LocalizedString.ACTIONTYPE_SIGN_CONFIRMBUTTON.Get(),
                string.Empty,
                string.Empty
            )

            //TODO ADD MORE IF REQUIRED
        };




        //-------------------------------------------------------------
        // /!\ WARNING /!\
        // Position in the array MUST be the same of the ActionType reference!
        //-------------------------------------------------------------
        public static readonly ActionType ACCEPT = actionTypes[0];
        public static readonly ActionType ASSIGN = actionTypes[1];
        public static readonly ActionType ACCEPT_AND_ADL = actionTypes[2];
        public static readonly ActionType REFUSE = actionTypes[3];
        public static readonly ActionType SIGN = actionTypes[4];
        public static readonly ActionType OPEN = actionTypes[5];
        public static readonly ActionType SHARE = actionTypes[6];
        public static readonly ActionType MANDATE = actionTypes[7];
        public static readonly ActionType SIGN_THANK_YOU_PAGE = actionTypes[8];
        public static readonly ActionType VIEW_OTP = actionTypes[9];

        //TODO ADD MORE IF ADDED INTO THE ARRAY

        public static ActionType GetFromId(int id)
        {
            if(id < 0 || id >= actionTypes.Length)
            {
                return null;
            }
            return actionTypes[id];
        }

        public readonly int id;

        /// <summary>
        /// Gets the label menu.
        /// </summary>
        /// <value>Used to define the label in the action dialog</value>
        public string labelMenu { get; }

        /// <summary>
        /// Gets the title bar.
        /// </summary>
        /// <value>Used to define the label the title in the view</value>
        public string titleBar { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>Used to define the description of the view</value>
        public string description { get; }

        /// <summary>
        /// Gets the confirm button.
        /// </summary>
        /// <value>Used to define the text in the confirm button</value>
        public string confirmButton { get; }

        /// <summary>
        /// Gets the text shown in action completed view.
        /// </summary>
        /// <value>Used to define the text of the complete view</value>
        public string doneText { get; }

        /// <summary>
        /// Gets the done button.
        /// </summary>
        /// <value>Used to define the text in the button of the confirm view</value>
        public string doneButton { get; }

        //TODO Add more if required (i.e. custom error messages)


        /// <summary>
        /// Gets the description for type document.
        /// </summary>
        /// <returns>The description for type document.</returns>
        /// <param name="description">Description.</param>
        /// <param name="typeDocument">Type document.</param>
        public string SetDescriptionForTypeDocument(string description, TypeDocument typeDocument)
        {
            if (typeDocument == TypeDocument.DOCUMENTO)
            {
                return string.Format(description, LocalizedString.DOCUMENTO);
            }

            return string.Format(description, LocalizedString.FASCICOLO);
        }

        /// <summary>
        /// Gets the description for type document.
        /// </summary>
        /// <returns>The description for type document.</returns>
        /// <param name="description">Description.</param>
        /// <param name="typeDocument">Type document.</param>
        public string SetDescriptionForTypeDocumentString(string description, string typeDocument)
        {
            if (typeDocument == LocalizedString.DOCUMENTO.Get())
            {
                return string.Format(description, LocalizedString.DOCUMENTO);
            }

            return string.Format(description, LocalizedString.FASCICOLO);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:InformaticaTrentinaPCL.Home.ActionType"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="labelMenu">Label menu.</param>
        /// <param name="titleBar">Title bar.</param>
        /// <param name="description">Description.</param>
        /// <param name="confirmButton">Confirm button.</param>
        /// <param name="doneText">Done text.</param>
        /// <param name="doneButton">Done button.</param>
        private ActionType(
                           int id,
                           string labelMenu,
                           string titleBar = "",
                           string description = "",
                           string confirmButton = "",
                           string doneText = "",
                           string doneButton = "")
        {
            this.id = id;
            this.labelMenu = labelMenu;
            this.titleBar = titleBar;
            this.description = description;
            this.confirmButton = confirmButton;
            this.doneText = doneText;
            this.doneButton = doneButton;
        }

    }
}
