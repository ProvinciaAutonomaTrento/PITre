<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewEditMandate.aspx.cs"
    Inherits="NttDataWA.Popup.NewEditMandate" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function proprietarioPopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoProprietario');
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

        function proprietarioSelected(sender, e) {
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

            var searchText = $get('<%=txtDescrizioneProprietario.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.txtDescrizioneProprietario.ClientID%>").focus();
            document.getElementById("<%=this.txtDescrizioneProprietario.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.txtCodiceProprietario.ClientID%>").value = codice;
            document.getElementById("<%=txtDescrizioneProprietario.ClientID%>").value = descrizione;

            __doPostBack('<%=this.txtCodiceProprietario.ClientID%>', '');
        }


    </script>
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <div class="container">
        <fieldset class="basic">
            <div class="row">
                <div class="col">
                    <p>
                        <span class="weight"><asp:Literal ID="litMandateNewRole" runat="server" /></span>
                    </p>
                </div>
            </div>
            <div class="row">
                <div class="col-full">
                    <asp:DropDownList ID="MandateDdlRole" runat="server" CssClass="chzn-select-deselect" Width="700" />
                </div>
            </div>
        </fieldset>
        <fieldset class="basic">
            <asp:UpdatePanel ID="upPnlProprietario" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="row">
                        <div class="col">
                            <p><span class="weight"><asp:Literal ID="litMandateNewUser" runat="server" /></span></p>
                        </div>
                        <div class="col-right-no-margin">
                            <cc1:CustomImageButton runat="server" ID="ImgProprietarioAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                OnClick="ImgProprietarioAddressBook_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <asp:HiddenField ID="idProprietario" runat="server" />
                        <asp:HiddenField ID="idPeople" runat="server" />
                        <div class="colHalf">
                            <cc1:CustomTextArea ID="txtCodiceProprietario" runat="server" CssClass="txt_addressBookLeft"
                                AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" OnTextChanged="TxtCode_OnTextChanged" onchange="disallowOp('Content2');"
                                AutoCompleteType="Disabled">
                            </cc1:CustomTextArea>
                        </div>
                        <div class="colHalf5">
                            <div class="colHalf3">
                                <cc1:CustomTextArea ID="txtDescrizioneProprietario" runat="server" CssClass="txt_projectRight"
                                    CssClassReadOnly="txt_ProjectRight_disabled">
                                </cc1:CustomTextArea>
                            </div>
                        </div>
                        <uc1:AutoCompleteExtender runat="server" ID="RapidProprietario" TargetControlID="txtDescrizioneProprietario"
                            CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                            CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce_trasmD"
                            MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                            DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                            UseContextKey="true" OnClientItemSelected="proprietarioSelected" BehaviorID="AutoCompleteExIngressoProprietario"
                            OnClientPopulated="proprietarioPopulated">
                        </uc1:AutoCompleteExtender>
                    </div>
                    <asp:PlaceHolder ID="plcRoleDelegate" runat="server" Visible="false">
                        <div class="row">
                            <div class="col">
                                <p>
                                    <span class="weight"><asp:Literal ID="litRoleDelegate" runat="server" /></span>
                                </p>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-full">
                                <asp:DropDownList ID="DdlMandateUserRole" runat="server" CssClass="chzn-select-deselect" Width="100%" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div class="row">
                <div class="col">
                    <p>
                        <span class="weight"><asp:Literal ID="litMandateNewDate" runat="server" /></span>
                    </p>
                </div>
            </div>
            <div class="row">
                <div class="colTitle">
                    <p><asp:Literal ID="litMandateNewEffectiveDate" runat="server" /></p>
                </div>
                <div class="col">
                    <cc1:CustomTextArea ID="txtDateFrom" runat="server" MaxLength="10" CssClass="txt_date datepicker"
                                CssClassReadOnly="txt_date_disabled" />
                </div>
                <div class="colTitle">
                    <p><asp:Literal ID="litMandateNewHour" runat="server" /></p>
                </div>
                <div class="col">
                    <asp:DropDownList ID="ddlHourFrom" runat="server" CssClass="chzn-select-deselect" Width="80">
                        <asp:ListItem Value="" Text="" Selected="True" />
                        <asp:ListItem Value="00" Text="00" />
                        <asp:ListItem Value="01" Text="01" />
                        <asp:ListItem Value="02" Text="02" />
                        <asp:ListItem Value="03" Text="03" />
                        <asp:ListItem Value="04" Text="04" />
                        <asp:ListItem Value="05" Text="05" />
                        <asp:ListItem Value="06" Text="06" />
                        <asp:ListItem Value="07" Text="07" />
                        <asp:ListItem Value="08" Text="08" />
                        <asp:ListItem Value="09" Text="09" />
                        <asp:ListItem Value="10" Text="10" />
                        <asp:ListItem Value="11" Text="11" />
                        <asp:ListItem Value="12" Text="12" />
                        <asp:ListItem Value="13" Text="13" />
                        <asp:ListItem Value="14" Text="14" />
                        <asp:ListItem Value="15" Text="15" />
                        <asp:ListItem Value="16" Text="16" />
                        <asp:ListItem Value="17" Text="17" />
                        <asp:ListItem Value="18" Text="18" />
                        <asp:ListItem Value="19" Text="19" />
                        <asp:ListItem Value="20" Text="20" />
                        <asp:ListItem Value="21" Text="21" />
                        <asp:ListItem Value="22" Text="22" />
                        <asp:ListItem Value="23" Text="23" />
                    </asp:DropDownList>
                </div>
                <div class="colTitle">
                    <p><asp:Literal ID="litMandateNewExpireDate" runat="server" /></p>
                </div>
                <div class="col">
                    <cc1:CustomTextArea ID="txtDateTo" runat="server" MaxLength="10" CssClass="txt_date datepicker"
                                CssClassReadOnly="txt_date_disabled" />
                </div>
                <div class="colTitle">
                    <p><asp:Literal ID="litMandateNewHour2" runat="server" /></p>
                </div>
                <div class="col">
                    <asp:DropDownList ID="ddlHourTo" runat="server" CssClass="chzn-select-deselect" Width="80">
                        <asp:ListItem Value="" Text="" Selected="True" />
                        <asp:ListItem Value="00" Text="00" />
                        <asp:ListItem Value="01" Text="01" />
                        <asp:ListItem Value="02" Text="02" />
                        <asp:ListItem Value="03" Text="03" />
                        <asp:ListItem Value="04" Text="04" />
                        <asp:ListItem Value="05" Text="05" />
                        <asp:ListItem Value="06" Text="06" />
                        <asp:ListItem Value="07" Text="07" />
                        <asp:ListItem Value="08" Text="08" />
                        <asp:ListItem Value="09" Text="09" />
                        <asp:ListItem Value="10" Text="10" />
                        <asp:ListItem Value="11" Text="11" />
                        <asp:ListItem Value="12" Text="12" />
                        <asp:ListItem Value="13" Text="13" />
                        <asp:ListItem Value="14" Text="14" />
                        <asp:ListItem Value="15" Text="15" />
                        <asp:ListItem Value="16" Text="16" />
                        <asp:ListItem Value="17" Text="17" />
                        <asp:ListItem Value="18" Text="18" />
                        <asp:ListItem Value="19" Text="19" />
                        <asp:ListItem Value="20" Text="20" />
                        <asp:ListItem Value="21" Text="21" />
                        <asp:ListItem Value="22" Text="22" />
                        <asp:ListItem Value="23" Text="23" />
                    </asp:DropDownList>
                </div>
            </div>
        </fieldset>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnSave_Click" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" />

            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <asp:HiddenField ID="proceed_permanent" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
