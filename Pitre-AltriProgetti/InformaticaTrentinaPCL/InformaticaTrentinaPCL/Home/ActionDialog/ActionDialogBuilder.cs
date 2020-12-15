using System;
using System.Collections.Generic;
using System.Threading;
using InformaticaTrentinaPCL.Home.ActionDialog;
using InformaticaTrentinaPCL.Home.MVP;
using InformaticaTrentinaPCL.Home.MVPD;
using InformaticaTrentinaPCL.Home.Network;
using InformaticaTrentinaPCL.Utils;

namespace InformaticaTrentinaPCL.Home
{
    public class ActionDialogBuilder
    {
        IDocumentListView view;
        SectionType sectionType;

        public ActionDialogBuilder(IDocumentListView view, SectionType sectionType)
        {
            this.view = view;
            this.sectionType = sectionType;
        }

        /// <summary>
        /// Utility method per generare la lista di voci da mostrare nella BottomDialog al tap di un documento/fascicolo
        /// </summary>
        /// <param name="abstractDocument">il documento tapped</param>
        /// <returns>La lista di DialogItem da mostrare nella BottomDialog </returns>
        /// <exception cref="Exception">Viene lanciata un'eccezione nel caso in cui il metodo venga chiamato per un SectionType non corretto (es. SIGN)</exception>
        public List<DialogItem> GetDialogActions(AbstractDocumentListItem abstractDocument, bool isTodoListRemovalManual, bool shareAllowed)
        {
            switch (sectionType)
            {
                case SectionType.ADL:
                    return GetADLDialogActions(abstractDocument, shareAllowed);

                case SectionType.TODO:
                    return GetToDoDialogActions(abstractDocument, isTodoListRemovalManual, shareAllowed);

                case SectionType.SEARCH:
                    return GetRicercaDialogActions(abstractDocument, shareAllowed);
            }

            throw new Exception("SECTION TYPE is not valid");
        }

        public List<DialogItem> GetDialogActionDocuments()
        {
            List<DialogItem> actions = new List<DialogItem>();

      actions.Add(new DialogItem(LocalizedString.BOTTOM_SIGN_ALL.Get(),
        delegate { view.DoSignAll(); }));
      actions.Add(new DialogItem(LocalizedString.BOTTOM_REJECT_ALL.Get(),
        delegate { view.DoRejectAll(); }));
      return actions;
    }

    List<DialogItem> GetADLDialogActions(AbstractDocumentListItem abstractDocument, bool shareAllowed)
    {
      List<DialogItem> actions = new List<DialogItem>();

            actions.Add(new DialogItem(LocalizedString.BOTTOM_DIALOG_REMOVE_FROM_ADL.Get(),
              delegate { view.DoRimuoviDaADL(abstractDocument); }));

            AddAssignActionIfAllowed(actions, abstractDocument);

            if (TypeDocument.DOCUMENTO.Equals(abstractDocument.tipoDocumento) && shareAllowed)
            {
                actions.Add(new DialogItem(LocalizedString.BOTTOM_DIALOG_SHARE.Get().Capitalize(),
                  delegate { view.DoCondividi(abstractDocument); }));
            }

            return actions;
        }

        List<DialogItem> GetToDoDialogActions(AbstractDocumentListItem abstractDocument, bool isTodoListRemovalManual, bool shareAllowed)
        {
            List<DialogItem> actions = new List<DialogItem>();

            bool hasWorkflow = true;

            if ((abstractDocument is ToDoDocumentModel))
            {
                hasWorkflow = ((ToDoDocumentModel)abstractDocument).hasWorkflow;
            }
            if (hasWorkflow)
            {
                actions.Add(new DialogItem(LocalizedString.BOTTOM_DIALOG_ACCEPT.Get().Capitalize(),
                  delegate { view.DoAccetta(abstractDocument); }));
                actions.Add(new DialogItem(LocalizedString.BOTTOM_DIALOG_ACCEPT_ADL.Get(),
                  delegate { view.DoAccettaEADL(abstractDocument); }));
                actions.Add(new DialogItem(LocalizedString.BOTTOM_DIALOG_REFUSE.Get().Capitalize(),
                  delegate { view.DoRifiuta(abstractDocument); }));
            }
            else
            {
                if (isTodoListRemovalManual)
                {
                    actions.Add(new DialogItem(LocalizedString.BOTTOM_DIALOG_VIEWED.Get().Capitalize(),
                      delegate { view.DoViewed(abstractDocument); }));
                    actions.Add(new DialogItem(LocalizedString.BOTTOM_DIALOG_VIEWED_ADL.Get(),
                        delegate { view.DoViewedADL(abstractDocument); }));
                }
            }
            AddAssignActionIfAllowed(actions, abstractDocument);

            if (TypeDocument.DOCUMENTO.Equals(abstractDocument.tipoDocumento) && shareAllowed)
            {
                actions.Add(new DialogItem(LocalizedString.BOTTOM_DIALOG_SHARE.Get().Capitalize(),
                  delegate { view.DoCondividi(abstractDocument); }));
            }

            return actions;
        }

        List<DialogItem> GetRicercaDialogActions(AbstractDocumentListItem abstractDocument, bool shareAllowed)
        {
            List<DialogItem> actions = new List<DialogItem>();

            actions.Add(new DialogItem(LocalizedString.BOTTOM_DIALOG_INSERT_IN_ADL.Get(),
              delegate { view.DoInserisciInADL(abstractDocument); }));

            AddAssignActionIfAllowed(actions, abstractDocument);

            if (TypeDocument.DOCUMENTO.Equals(abstractDocument.tipoDocumento) && shareAllowed)
            {
                actions.Add(new DialogItem(LocalizedString.BOTTOM_DIALOG_SHARE.Get().Capitalize(),
                  delegate { view.DoCondividi(abstractDocument); }));
            }

            return actions;
        }

        private void AddAssignActionIfAllowed(List<DialogItem> actions, AbstractDocumentListItem abstractDocument)
        {
            //only if there is a valid IdTrasmissione
            if (!string.IsNullOrEmpty(abstractDocument.GetIdTrasmissione()))
            {
                if (abstractDocument is ToDoDocumentModel)
                {
                    AddAssignActionIfTodoDocumentIsAllowed(actions, abstractDocument);
                }
                else
                {
                    AddAssignAction(actions, abstractDocument);
                }
            }
        }

        private void AddAssignActionIfTodoDocumentIsAllowed(List<DialogItem> actions, AbstractDocumentListItem abstractDocument)
        {
            // I must check that 'AccessRight' is >= 44, if I can't check or is true I will add the action
            var doc = abstractDocument as ToDoDocumentModel;
            if (!string.IsNullOrEmpty(doc.accessRights))
            {
                try
                {
                    /*double result = Convert.ToDouble(doc.accessRights);
                    if (result >= 44.0f)
                    {*/
                        AddAssignAction(actions, abstractDocument);
                    //}
                }
                catch (Exception)
                {
                    AddAssignAction(actions, abstractDocument);
                }
            }
            else
            {
                AddAssignAction(actions, abstractDocument);
            }
        }

        private void AddAssignAction(List<DialogItem> actions, AbstractDocumentListItem abstractDocumnet)
        {
            actions.Add(new DialogItem(LocalizedString.BOTTOM_DIALOG_ASSIGN.Get().Capitalize(),
              delegate { view.DoAssegna(abstractDocumnet); }));
        }

        public void Dispose()
        {
            view = null;
        }
    }
}