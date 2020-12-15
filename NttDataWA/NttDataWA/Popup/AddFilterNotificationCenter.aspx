<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddFilterNotificationCenter.aspx.cs" Inherits="NttDataWA.Popup.AddFilterNotificationCenter" MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function changeBgImage(image, id) {
            var element = document.getElementById(id);
            element.style.backgroundImage = "url(" + image + ")";
        }

        function closeObjectPopup() {
            $('#btnObjectPostback').click();
        }

        function closeAddressBookPopup() {
            $('#btnAddressBookPostback').click();
        }

        function creatorePopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoCreatore');
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

        function creatoreSelected(sender, e) {
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

            var searchText = $get('<%=txtDescrizioneCreatore.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneCreatore.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneCreatore.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceCreatore.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneCreatore.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txtCodiceCreatore.ClientID%>', '');
        }

    </script>
    <style type="text/css">
        .rbl input
            {
                padding-left: 15px;
                padding-right: 1px;
            }
        .cbl input
            {
                padding-left: 15px;
                padding-right: 1px;
            }
        legend
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent" ClientIDMode="static"  runat="server">
  <div id="containerFilter" style=" padding-top:10px">
         <asp:UpdatePanel runat="server" ID="UpPnlContainerFilter" UpdateMode="Conditional" ClientIDMode="static">
            <ContentTemplate>
            <fieldset>            
                <div id="containerSenderTransmission" style=" padding-top:10px">
                    <div style=" float:left;">
                        <strong>
                            <asp:Label ID="lblSenderTransmission" runat="server"></asp:Label></strong>
                    </div>
                    <div class="col">
                        <asp:RadioButtonList ID="rblOwnerType" runat="server" CssClass="rblHorizontal" RepeatLayout="UnorderedList">
                            <asp:ListItem id="optUO" runat="server" Value="U" />
                            <asp:ListItem id="optRole" runat="server" Value="R" Selected="True" />
                            <asp:ListItem id="optUser" runat="server" Value="P" />
                        </asp:RadioButtonList>
                    </div>
                    <div class="row" style=" padding-top:10px">
                         <asp:UpdatePanel ID="upPnlAuthor" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:HiddenField ID="idCreatore" runat="server" />
                                <div class="colHalf" style=" padding-left:70px">
                                    <cc1:CustomTextArea ID="txtCodiceCreatore" runat="server" CssClass="txt_addressBookLeft"
                                        AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" onblur="disallowOp('ContentPlaceHolderContent');"
                                         OnTextChanged="TxtCode_OnTextChanged"
                                        AutoCompleteType="Disabled">
                                    </cc1:CustomTextArea>
                                </div>
                            <div style=" float:left; width:62%; padding-right:10px">
                                <div class="colHalf3">
                                    <cc1:CustomTextArea ID="txtDescrizioneCreatore" runat="server" CssClass="txt_projectRight"
                                        CssClassReadOnly="txt_ProjectRight_disabled">
                                    </cc1:CustomTextArea>
                                </div>
                               <uc1:AutoCompleteExtender runat="server" ID="RapidCreatore" TargetControlID="txtDescrizioneCreatore"
                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                    MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                    UseContextKey="true" OnClientItemSelected="creatoreSelected" BehaviorID="AutoCompleteExIngressoCreatore"
                                    OnClientPopulated="creatorePopulated" Enabled="false">
                                </uc1:AutoCompleteExtender>
                            </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div>
                            <cc1:CustomImageButton runat="server" ID="ImageButtonAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/address_book_disabled.png" OnClick="ImageButtonAddressBook_Click"/>
                        </div>
                    </div>
                </div>
                 <div class="row" style=" padding-top:10px">
                    <div style=" float:left; padding-right:5px;">
                        <strong>
                            <asp:Label ID="lblObject" runat="server"></asp:Label></strong>
                    </div>
                    <div style=" float:left; width:67%; padding-right:5px">
                        <asp:UpdatePanel ID="UpdPnlObject" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                            <ContentTemplate>
                                <asp:Panel ID="PnlCodeObject" runat="server" Visible="false">
                                    <cc1:CustomTextArea ID="TxtCodeObject" runat="server" CssClass="txt_addressBookLeft"
                                        CssClassReadOnly="txt_addressBookLeft_disabled" AutoPostBack="true">
                                    </cc1:CustomTextArea>
                                </asp:Panel>
                                <asp:Panel ID="PnlCodeObject3" runat="server">
                                    <cc1:CustomTextArea ID="TxtObject" Width="99%" runat="server" class="txt_input_full"
                                        CssClassReadOnly="txt_input_full_disabled" MaxLength="2000" ClientIDMode="Static"></cc1:CustomTextArea>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div>
                        <cc1:CustomImageButton runat="server" ID="ImageButtonObjectary" ImageUrl="../Images/Icons/obj_objects.png"
                            OnMouseOutImage="../Images/Icons/obj_objects.png" OnMouseOverImage="../Images/Icons/obj_objects_hover.png"
                            CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/obj_objects_disabled.png" OnClientClick="return parent.ajaxModalPopupObject();"
                                />
                    </div>
                </div>
                <div style=" padding-top:10px">
                    <div class="row" style=" padding-top:10px">
                        <div style=" float:left; padding-right:27px">
                            <strong>
                                <asp:Label ID="lblDateEvent" runat="server"></asp:Label></strong>
                        </div>
                        <div>
                            <div class="styled-select_full" style=" float:left; padding-right:25px">
                                <asp:DropDownList ID="ddl_dateEvent" runat="server" CssClass="chzn-select-deselect" Width="150px"
                                     AutoPostBack="true" OnSelectedIndexChanged="DdlDateEvent_SelectedIndexChanged">
                                    <asp:ListItem id="ddlItemValueSingle" Value="0"></asp:ListItem>
                                    <asp:ListItem id="ddlItemInterval" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <asp:PlaceHolder ID="PlaceHolderDateEventFrom" runat="server">
                                <div class="col2">
                                    <strong>
                                        <asp:Literal runat="server" ID="ltlDateEventFrom" Visible="false"></asp:Literal></strong>
                                </div>
                                <div class="col4">
                                <cc1:CustomTextArea ID="txt_dateEventFrom" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                    CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="PlaceHolderDateEventTo" runat="server" Visible="false">
                                <div class="col2">
                                    <strong>
                                        <asp:Literal runat="server" ID="ltlDateEventTo"></asp:Literal></strong>
                                </div>
                                <div class="col4">
                                    <cc1:CustomTextArea ID="txt_dateEventTo" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                </div>
                            </asp:PlaceHolder>
                            </div>
                        </div>
        
                        <div class="row" style=" padding-top:10px">
                            <div style=" float:left; padding-right:10px">
                                <strong>
                                    <asp:Label ID="lblExpirationDate" runat="server"></asp:Label></strong>
                            </div>
                            <div class="styled-select_full" style=" float:left; padding-right:27px">
                                <asp:DropDownList ID="ddl_expirationDate" runat="server" CssClass="chzn-select-deselect" Width="150px" 
                                    OnSelectedIndexChanged="DdlExpirationDate_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem id="ddlExpirationDateItemValueSingle" Value="0"></asp:ListItem>
                                    <asp:ListItem id="ddlExpirationDateItemInterval" Value="1"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <asp:PlaceHolder ID="PlaceHolderExpirationDateFrom" runat="server">  
                                <div class="col2">
                                        <strong>
                                            <asp:Literal runat="server" ID="ltlExpirationDateFrom" Visible="false"></asp:Literal></strong>
                                    </div>
                                    <div class="col4">
                                        <cc1:CustomTextArea ID="txt_expirationDateFrom" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                    </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="PlaceHolderExpirationDateTo" runat="server" Visible="false">
                                <div class="col2">
                                    <strong>
                                        <asp:Literal runat="server" ID="ltlExpirationDateTo"></asp:Literal></strong>
                                </div>
                                <div class="col4">
                                    <cc1:CustomTextArea ID="txt_expirationDateTo" runat="server" Width="80px" CssClass="txt_textdata datepicker"
                                        CssClassReadOnly="txt_textdata_disabled"></cc1:CustomTextArea>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                        <asp:PlaceHolder ID="PlaceHolderNotes" runat="server" Visible="false">
                        <div class="row" style=" padding-top:20px">
                            <div style=" float:left; padding-right:10px">
                                <strong>
                                    <asp:Label ID="lblNotes" runat="server"></asp:Label></strong>
                            </div>
                            <div class="col4">
                            <cc1:CustomTextArea ID="txt_notes" runat="server" Width="400px"  MaxLength="1000" CssClass="txt_addressBookLeft"
                                        CssClassReadOnly="txt_addressBookLeft_disabled" ></cc1:CustomTextArea>
                        </div>
                        </div>
                        </asp:PlaceHolder>
                     </div>
                    <div id="containerWaitingAcceptance" style=" padding-top:10px">
                        <strong>
                            <asp:Label ID="lblWaitingAcceptance" runat="server"></asp:Label></strong>
                            <asp:CheckBox ID="cbWaitingAcceptance" runat="server" />
                    </div> 

            </fieldset>
            <div style=" padding-top:20px">
                <fieldset>
                    <legend>
                        <strong>
                            <asp:Label ID="lblSectionDocument" runat="server" style=" font-size:1.2em"></asp:Label></strong>
                    </legend>
                    <div id="containerFilterDocumentType">
                        <div class="row">
                    
                                <asp:RadioButtonList ID="rblFilterDocumentType" runat="server" RepeatDirection="Horizontal" CssClass="rbl">
                                    <asp:ListItem id="rbDocumentTypeInbound" Value="A"></asp:ListItem>
                                    <asp:ListItem id="rbDocumentTypeOutbound" Value="P"></asp:ListItem>
                                    <asp:ListItem id="rbDocumentTypeInternal" Value="I"></asp:ListItem>
                                    <asp:ListItem id="rbDocumentTypeDocument" Value="G"></asp:ListItem>
                                    <asp:ListItem id="rbDocumentTypePrepared" Value="PRED"></asp:ListItem>
                                    <asp:ListItem id="rbDocumentTypeAll" Value="ALL" Selected="True"></asp:ListItem>
                                </asp:RadioButtonList>
                        </div>
                    </div>  
                    <div id="containerFilterDocument">
                            <asp:CheckBoxList ID="cblFilterDocument" runat="server" RepeatDirection="Horizontal" CssClass="cbl">
                                <asp:ListItem id="cbDocumentAcquired" Value="DOCUMENT_ACQUIRED"></asp:ListItem>
                                <asp:ListItem id="cbDocumentNotSigned" Value="DOCUMENT_NOT_SIGNED"></asp:ListItem>
                                <asp:ListItem id="cbDocumentSigned" Value="DOCUMENT_SIGNED"></asp:ListItem>
                            </asp:CheckBoxList>
                    </div> 
                    <div class="row" style="padding-top:20px">
                        <div style=" float:left; padding-right:10px;">
                            <strong>
                                <asp:Label ID="lblTypeFileAcquired" runat="server"></asp:Label></strong>
                        </div>
                        <div>
                            <asp:DropDownList ID="ddlTypeFileAcquired" runat="server" CssClass="chzn-select-deselect" Width="50%">
                                <asp:ListItem Text=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </fieldset>
            </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="AddFilterBtnConfirm" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddFilterBtnConfirm_Click" />
            <cc1:CustomButton ID="AddFilterBtnCancel" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="AddFilterBtnCancel_Click" />
            <asp:Button ID="btnObjectPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnObjectPostback_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" OnClientClick="disallowOp('ContentPlaceHolderContent')" />
            <script type="text/javascript">
                $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
                $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
            </script>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>