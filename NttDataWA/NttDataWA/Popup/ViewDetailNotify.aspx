<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewDetailNotify.aspx.cs"
    Inherits="NttDataWA.Popup.ViewDetailNotify" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
<script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            // --- Using the default options:
            $("h2.expand").toggler({ initShow: "div.shown" });

            $('#contentNoteTask input, #contentNoteTask textarea').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });

            $('#contentNoteTask select').change(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });
        });

        function closeSearchProject() {
            $('#btnSearchProjectPostback').click();
        }

        function closeOpenTitolario() {
            $('#btnTitolarioPostback').click();
        }

        function acePopulatedCod(sender, e) {
            var behavior = $find('AutoCompleteDescriptionProject');
            var target = behavior.get_completionList();
            if (behavior._currentPrefix != null) {
                var prefix = behavior._currentPrefix.toLowerCase();
                var i;
                for (i = 0; i < target.childNodes.length; i++) {
                    var sValue = target.childNodes[i].innerHTML.toLowerCase();
                    if (sValue.indexOf(prefix) != -1) {
                        var fstr = target.childNodes[i].innerHTML.substring(0, sValue.indexOf(prefix));

                        var pstr = target.childNodes[i].innerHTML.substring(fstr.length, fstr.length + prefix.length);

                        var estr = target.childNodes[i].innerHTML.substring(fstr.length + prefix.length, target.childNodes[i].innerHTML.length);

                        target.childNodes[i].innerHTML = fstr + '<span class="selectedWord">' + pstr + '</span>' + estr;
                        try {
                            target.childNodes[i].attributes["_value"].value = fstr + pstr + estr;
                        }
                        catch (ex) {
                            target.childNodes[i].attributes["_value"] = fstr + pstr + estr;
                        }
                    }
                }
            }
        }

        function verifica() {
            var valReturn = false;
            if (document.getElementById("<%=this.TxtCodeProject.ClientID%>").value != '' && document.getElementById("<%=TxtDescriptionProject.ClientID%>").value != '') {
                valReturn = true;
            }
            else if (document.getElementById("<%=this.TxtCodeProject.ClientID%>").value == '') {
                valReturn = true;
            }

            return valReturn;
        }

        function aceSelectedDescr(sender, e) {

            var value = e.get_value();
            if (!value) {

                if (e._item.parentElement && e._item.parentElement.tagName == "LI") {

                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentElement && e._item.parentElement.parentElement.tagName == "LI") {
                    try {
                        value = e._item.parentElement.attributes["_value"].value;
                    }
                    catch (ex1) {
                        value = e._item.parentElement.attributes["_value"];
                    }
                    if (value == undefined || value == null)
                        value = e._item.parentElement.attributes["_value"];
                }
                else if (e._item.parentNode && e._item.parentNode.tagName == "LI") {
                    value = e._item.parentNode._value;
                }
                else if (e._item.parentNode && e._item.parentNode.parentNode.tagName == "LI") {
                    value = e._item.parentNode.parentNode._value;
                }
                else value = "";

            }

            var searchText = $get('<%=TxtDescriptionProject.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionProject.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionProject.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtCodeProject.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionProject.ClientID%>").value = descrizione;
        }


    </script>
    <style type="text/css">
        .tbl_rounded2
        {
            margin: 0 auto 15px auto;
            width: 97%;
            border-collapse: collapse;
        }
        
        .tbl_rounded2 td.header1
        {
            background: #01497B url(../../Images/Common/table_header_bg.png) repeat-x top left;
            color: #fff;
            font-weight: normal;
            border-top-left-radius: 10px;
            border-top-right-radius: 10px;
            -ms-border-top-left-radius: 10px; /* ie */
            -ms-border-top-right-radius: 10px; /* ie */
            -moz-border-top-left-radius: 10px; /* firefox */
            -moz-border-top-right-radius: 10px; /* firefox */
            -webkit-border-top-left-radius: 10px; /* safari, chrome */
            -webkit-border-top-right-radius: 10px; /* safari, chrome */
            text-align: left;
            font-weight: bold;
            border: 1px solid #d4d4d4;
            padding: 5px;
            vertical-align: top;
            height: auto;
        }
        
        .tbl_rounded2 td.header2
        {
            background: #FFD700 url(../../Images/Common/table_header_bg.png) repeat-x top left;
            font-weight: normal;
            border-top-left-radius: 10px;
            border-top-right-radius: 10px;
            -ms-border-top-left-radius: 10px; /* ie */
            -ms-border-top-right-radius: 10px; /* ie */
            -moz-border-top-left-radius: 10px; /* firefox */
            -moz-border-top-right-radius: 10px; /* firefox */
            -webkit-border-top-left-radius: 10px; /* safari, chrome */
            -webkit-border-top-right-radius: 10px; /* safari, chrome */
            text-align: left;
            font-weight: bold;
            border: 1px solid #d4d4d4;
            padding: 5px;
            vertical-align: top;
            height: auto;
        }
        
        .tbl_rounded2 td, tr
        {
            background: #fff;
            min-height: 1em;
            border: 1px solid #d4d4d4;
            text-align: left;
            height: 10px;
            line-height: 15px;
        }
        
        .tbl_rounded2 tr.header td, .tbl_rounded2 tr.header2 td
        {
            background-color: #e1e9f0;
        }
        
        .tbl_rounded2 tr.header3 td, .tbl_rounded2 tr.header3 td
        {
            background-color: #E8E8E8;
        }
        
        .tbl_rounded2 th.center, .tbl_rounded2 td.center
        {
            text-align: center;
        }
        .tbl_rounded2 td.right
        {
            text-align: right;
        }
        .tbl_rounded2 th.first, .tbl_rounded2 td.first
        {
            padding-left: 15px;
        }
        
        .tbl_rounded2 th.first2, .tbl_rounded2 td.first2
        {
            background: #01497B url(../../Images/Common/table_header_bg.png) repeat-x top left;
            color: #fff;
            text-align: left;
            font-weight: bold;
            height: 2px;
        }
        
        .tbl_rounded2 th.trasmDetailDate, .tbl_rounded2 td.trasmDetailDate
        {
            width: 10%;
        }
        .tbl_rounded2 th.trasmDetailType, .tbl_rounded2 td.trasmDetailType
        {
            width: 13%;
        }
        .tbl_rounded2 th.trasmDetailReason, .tbl_rounded2 td.trasmDetailReason
        {
            width: 12%;
        }
        .tbl_rounded2 th.trasmDetailNote, .tbl_rounded2 td.trasmDetailNote
        {
            width: 25%;
        }
        .expand img
        {
            float: right;
            border: 0px;
        }
        .expand
        {
            font-size:13px;
            font-style:italic;
            white-space:nowrap;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div id="containerDetail">
        <asp:UpdatePanel ID="UpdatePanelContainerInfoNotify" runat="server" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerInfoNotify" style="padding-bottom: 5px">
                    <asp:Table ID="tblInfoNotify" runat="server" CssClass="tbl_rounded2">
                        <asp:TableRow>
                            <asp:TableCell CssClass="first2" ID="TableCellLtlSignatureDoc" Text='<%$ localizeByText:ViewDetailNotifyDocFasc%>'
                                Width="20%" runat="server"></asp:TableCell>
                            <asp:TableCell ID="TableCellTxtSignatureDoc" ColumnSpan="3" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell CssClass="first2" ID="TableCellLtlObjectDoc" Text='<%$ localizeByText:ViewDetailNotifyDetailNotification%>'
                                runat="server"></asp:TableCell>
                            <asp:TableCell ID="TableCellTxtObjectDoc" ColumnSpan="3" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowDisplayed" runat="server">
                            <asp:TableCell CssClass="first2" ID="TableCell" runat="server" Text='<%$ localizeByText:ViewDetailNotifyDisplayed%>'></asp:TableCell>
                            <asp:TableCell ID="TableCellDispayed" runat="server" Width="30%"></asp:TableCell>
                            <asp:TableCell CssClass="first2" ID="TableCell3" runat="server" Text='<%$ localizeByText:ViewDetailNotifyReplied%>'
                                Width="20%"></asp:TableCell>
                            <asp:TableCell ID="TableCellReplied" runat="server" Width="30%"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowAccepted" runat="server">
                            <asp:TableCell CssClass="first2" runat="server" Text='<%$ localizeByText:ViewDetailNotifyAccepted%>'></asp:TableCell>
                            <asp:TableCell ID="TableCellAccepted" runat="server"></asp:TableCell>
                            <asp:TableCell CssClass="first2" runat="server" Text='<%$ localizeByText:ViewDetailNotifyRejectedThe%>'></asp:TableCell>
                            <asp:TableCell ID="TableCellRejected" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowNote" runat="server">
                            <asp:TableCell CssClass="first2" runat="server" Text='<%$ localizeByText:ViewDetailNotifyNoteAccRej%>'></asp:TableCell>
                            <asp:TableCell ID="TableCellNoteAccRej" ColumnSpan="3" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowRoleUserNotification" runat="server">
                            <asp:TableCell ID="TableCellRoleUserNotification" ColumnSpan="4" runat="server"></asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowTypeEvent" runat="server">
                            <asp:TableCell ID="TableCellTypeEvent" ColumnSpan="4" runat="server"></asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="UpdatePanelTransmission" runat="server" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <asp:PlaceHolder ID="PlaceHolderTransmission" runat="server">
                    <asp:UpdatePanel ID="UpdatePanelDetailTrasmission" runat="server" UpdateMode="Conditional"
                        ClientIDMode="Static">
                        <ContentTemplate>
                            <asp:PlaceHolder ID="containerDetailTrasmission" runat="server">
                                <asp:Table ID="tblDetails" runat="server" CssClass="tbl_rounded2" Visible="false">
                                    <asp:TableRow>
                                        <asp:TableCell ColumnSpan="7" CssClass="th first">
                                            <asp:Literal ID="TransmissionLitDetailsRecipient" runat="server" />
                                        </asp:TableCell></asp:TableRow>
                                    <asp:TableRow CssClass="header">
                                        <asp:TableCell CssClass="first" ColumnSpan="2" Text='<%$ localizeByText:ViewDetailNotifyReceiver%>'></asp:TableCell><asp:TableCell
                                            CssClass="center trasmDetailReason" Text='<%$ localizeByText:ViewDetailNotifyReason%>'></asp:TableCell><asp:TableCell
                                                CssClass="center trasmDetailType" ColumnSpan="2" Text='<%$ localizeByText:ViewDetailNotifyType%>'></asp:TableCell><asp:TableCell
                                                    CssClass="trasmDetailNote" Text='<%$ localizeByText:ViewDetailNotifyNote%>'></asp:TableCell><asp:TableCell
                                                        CssClass="center trasmDetailDate" Text='<%$ localizeByText:ViewDetailNotifyExpires%>'></asp:TableCell></asp:TableRow>
                                    <asp:TableRow CssClass="height">
                                        <asp:TableCell ID="trasmDetailsRecipient" runat="server" CssClass="first" ColumnSpan="2" />
                                        <asp:TableCell ID="trasmDetailsReason" runat="server" CssClass="center" />
                                        <asp:TableCell ID="trasmDetailsType" runat="server" CssClass="center" ColumnSpan="2" />
                                        <asp:TableCell ID="trasmDetailsNote" runat="server" />
                                        <asp:TableCell ID="trasmDetailsExpire" runat="server" CssClass="center" />
                                    </asp:TableRow>
                                    <asp:TableRow CssClass="header2">
                                        <asp:TableCell CssClass="first" Text='<%$ localizeByText:ViewDetailNotifyUser%>'>Utente</asp:TableCell><asp:TableCell
                                            CssClass="center trasmDetailDate">Vista il</asp:TableCell><asp:TableCell CssClass="center trasmDetailDate"
                                                Text='<%$ localizeByText:ViewDetailNotifyAcc%>'>Acc. il</asp:TableCell><asp:TableCell
                                                    CssClass="center trasmDetailDate" Text='<%$ localizeByText:ViewDetailNotifyRejected%>'></asp:TableCell><asp:TableCell
                                                        CssClass="center trasmDetailDate" Text='<%$ localizeByText:ViewDetailNotifyRemoved%>'></asp:TableCell><asp:TableCell
                                                            CssClass="center" ColumnSpan="2" Text='<%$ localizeByText:ViewDetailNotifyInfo%>'></asp:TableCell></asp:TableRow>
                                    <asp:TableRow ID="rowDetails" runat="server" CssClass="height">
                                        <asp:TableCell ID="trasmDetailsUser" runat="server" CssClass="first" />
                                        <asp:TableCell ID="trasmDetailsViewed" runat="server" CssClass="center" />
                                        <asp:TableCell ID="trasmDetailsAccepted" runat="server" CssClass="center" />
                                        <asp:TableCell ID="trasmDetailsRif" runat="server" CssClass="center" />
                                        <asp:TableCell ID="trasmDetailsRemoved" runat="server" CssClass="center" />
                                        <asp:TableCell ID="trasmDetailsInfo" runat="server" ColumnSpan="2" />
                                    </asp:TableRow>
                                </asp:Table>
                            </asp:PlaceHolder>
                            <asp:UpdatePanel ID="upPnlNoteAccRif" runat="server" UpdateMode="Conditional" Visible="false">
                                <ContentTemplate>
                                    <div class="row" style="padding-left: 15px">
                                        <span class="weight">
                                            <asp:Literal ID="TransmissionNoteAccRej" runat="server" /></span><br />
                                        <cc1:CustomTextArea ID="txt_noteAccRif" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                                            Width="98%" MaxLength="250"/>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:UpdatePanel ID="upPnlFascRequired" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Panel ID="pnlFascRequired" runat="server" Width="98%">
                                        <div class="row" style="padding-left: 15px">
                                        <asp:UpdatePanel runat="server" ID="UpPnlPrimaryClassification" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:PlaceHolder ID="pnl_fasc_Primaria" runat="server" Visible="false">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">
                                                                    <asp:Literal runat="server" ID="ltlPrimaryFascText"></asp:Literal></span>&nbsp;<asp:Label
                                                                        ID="lblPrimaryFascDescr" runat="server">*</asp:Label></p>
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal runat="server" ID="DocumentLitClassificationRapidTrasm"></asp:Literal></span></p>
                                            </div>
                                            <div class="col-right-no-margin">
                                                <asp:UpdatePanel ID="pPnlBUttonsProject" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <cc1:CustomImageButton ID="ImgOpenTitolario" ImageUrl="../Images/Icons/classification_scheme.png"
                                                            runat="server" OnMouseOverImage="../Images/Icons/classification_scheme_hover.png"
                                                            OnMouseOutImage="../Images/Icons/classification_scheme.png" alt="Titolario" title="Titolario"
                                                            OnClick="ImgOpenTitolario_Click" CssClass="clickableLeftN" OnClientClick="disallowOp('ContentPlaceHolderContent');"
                                                            ImageUrlDisabled="../Images/Icons/classification_scheme_disabled.png"/>
                                                        <cc1:CustomImageButton ID="ImgSearchProjects" ImageUrl="../Images/Icons/search_projects.png"
                                                            runat="server" OnMouseOverImage="../Images/Icons/search_projects_hover.png" OnMouseOutImage="../Images/Icons/search_projects.png"
                                                            ImageUrlDisabled="../Images/Icons/search_projects_disabled.png" alt="Titolario"
                                                            CssClass="clickableLeftN" OnClick="ImgSearchProjects_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
                                                        <cc1:CustomImageButton ID="ImgAddProjects" ImageUrl="../Images/Icons/add_sub_folder.png"
                                                            runat="server" OnMouseOverImage="../Images/Icons/add_sub_folder_hover.png" OnMouseOutImage="../Images/Icons/add_sub_folder.png"
                                                            ImageUrlDisabled="../Images/Icons/add_sub_folder_disabled.png" alt="Titolario"
                                                            CssClass="clickableLeftN" OnClick="ImgAddProjects_Click" OnClientClick="disallowOp('ContentPlaceHolderContent');" />
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <asp:UpdatePanel ID="UpPnlProject" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:PlaceHolder runat="server" ID="PnlProject">
                                                        <asp:HiddenField ID="IdProject" runat="server" />
                                                        <div class="colHalf">
                                                            <cc1:CustomTextArea ID="TxtCodeProject" runat="server" CssClass="txt_addressBookLeft" onchange="disallowOp('ContentPlaceHolderContent');"
                                                                AutoComplete="off"  AutoPostBack="true" OnTextChanged="TxtCodeProject_OnTextChanged"
                                                                CssClassReadOnly="txt_addressBookLeft_disabled">
                                                            </cc1:CustomTextArea>
                                                        </div>
                                                        <div class="colHalf2">
                                                            <div class="colHalf3">
                                                                <cc1:CustomTextArea ID="TxtDescriptionProject" runat="server" CssClass="txt_addressBookRight"
                                                                    autocomplete="off" CssClassReadOnly="txt_addressBookRight_disabled">
                                                                </cc1:CustomTextArea>
                                                            </div>
                                                        </div>
                                                         <uc1:AutoCompleteExtender runat="server" ID="RapidSenderDescriptionProject" TargetControlID="TxtDescriptionProject"
                                                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListDescriptionProject"
                                                            MinimumPrefixLength="6" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                            UseContextKey="true" OnClientItemSelected="aceSelectedDescr" BehaviorID="AutoCompleteDescriptionProject"
                                                            OnClientPopulated="acePopulatedCod">
                                                        </uc1:AutoCompleteExtender>
                                                    </asp:PlaceHolder>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                    </asp:Panel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <asp:PlaceHolder ID="plcTransmissions" runat="server"></asp:PlaceHolder>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:PlaceHolder>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnAccept" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnClientClick="disallowOp('ContentPlaceHolderContent');" OnMouseOver="btnHover"
                OnClick="BtnAccept_Click" />
            <cc1:CustomButton ID="BtnAcceptAdLU" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnClientClick="disallowOp('ContentPlaceHolderContent');" OnMouseOver="btnHover"
                OnClick="BtnAcceptAdLU_Click" />
            <cc1:CustomButton ID="BtnAcceptAdLR" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnClientClick="disallowOp('ContentPlaceHolderContent');" OnMouseOver="btnHover"
                OnClick="BtnAcceptAdLR_Click" />
            <cc1:CustomButton ID="BtnAcceptLF" runat="server" CssClass="btnEnable clickable" CssClassDisabled="btnDisable"
                OnClientClick="disallowOp('ContentPlaceHolderContent');" OnMouseOver="btnHover"
                OnClick="BtnAcceptLF_Click" />
            <cc1:CustomButton ID="BtnReject" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnClientClick="disallowOp('ContentPlaceHolderContent');" OnMouseOver="btnHover"
                OnClick="BtnReject_Click" />
            <cc1:CustomButton ID="BtnView" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnClientClick="disallowOp('ContentPlaceHolderContent');" OnMouseOver="btnHover"
                OnClick="BtnView_Click" />
            <cc1:CustomButton ID="BtnViewAdLU" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnClientClick="disallowOp('ContentPlaceHolderContent');" OnMouseOver="btnHover"
                OnClick="BtnViewAdLU_Click" />
            <cc1:CustomButton ID="BtnViewAdLR" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnClientClick="disallowOp('ContentPlaceHolderContent');" OnMouseOver="btnHover"
                OnClick="BtnViewAdLR_Click" />
            <cc1:CustomButton ID="BtnRemovePredisposed" runat="server" CssClass="btnEnable clickable"
                CssClassDisabled="btnDisable" OnClientClick="disallowOp('ContentPlaceHolderContent');"
                OnMouseOver="btnHover" OnClick="BtnRemovePredisposed_Click" />
            <cc1:CustomButton ID="BtnCancel" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnClientClick="disallowOp('ContentPlaceHolderContent');" OnMouseOver="btnHover"
                OnClick="BtnCancel_Click" />
            <cc1:CustomButton ID="BtnAcquireRights" runat="server" CssClass="btnEnable clickable"
                CssClassDisabled="btnDisable" OnClientClick="disallowOp('ContentPlaceHolderContent');"
                OnMouseOver="btnHover" OnClick="BtnAcquireRights_Click" />
            <asp:Button ID="btnSearchProjectPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnSearchProject_Click" />
            <asp:Button ID="btnTitolarioPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnTitolarioPostback_Click" />
            <asp:HiddenField ID="HiddenRemovePredisposed" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="final_state" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
