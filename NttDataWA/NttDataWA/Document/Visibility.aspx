<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true"
    CodeBehind="Visibility.aspx.cs" Inherits="NttDataWA.Document.Visibility" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Src="~/UserControls/DocumentButtons.ascx" TagPrefix="uc3" TagName="DocumentButtons" %>
<%@ Register Src="~/UserControls/DocumentTabs.ascx" TagPrefix="uc4" TagName="DocumentTabs" %>
<%@ Register Src="~/UserControls/HeaderDocument.ascx" TagPrefix="uc2" TagName="HeaderDocument" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .corr_grey
        {
            color: #ccc;
        }
        #divDetails
        {
            text-align: left;
            margin: 5px 0 0 0;
            padding: 5px 0 0 0;
            font-size: 0.9em;
            border-top: 1px solid #ccc;
        }
        #divDetails ul
        {
            list-style-type: disc;
            margin: 0;
            padding: 0;
        }
        #divDetails ul li
        {
            margin: 0 0 0 15px;
        }
        .tbl_rounded_custom tr.nopointer td
        {
            cursor: default;
        }
        .tbl_rounded_custom tr td
        {
            cursor: pointer;
        }
        .tbl_rounded_custom th a
        {
            color: #fff;
            text-decoration: none;
        }
        
        .recordNavigator, .recordNavigator table, .recordNavigator tr, .recordNavigator td
        {
            background-color: #EEEEEE;
        }
        .recordNavigator td
        {
            border: 0;
        }
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        function espandi(obj, row) {
            var div = document.getElementById(obj);
            var img = document.getElementById('img' + obj);
            if (div != null) {
                if (div.style.display == "none") {
                    div.style.display = "block";
                    img.alt = "Clicca per nascondere";
                }
                else {
                    div.style.display = "none";
                    img.alt = "Clicca per aprire";
                }
            }
        }


        function acePopulatedUser(sender, e) {
            var behavior = $find('AutoCompletUser');
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

        function acePopulatedRole(sender, e) {
            var behavior = $find('AutoCompletRole');
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

        function aceSelected(sender, e) {
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

            var searchText = $get('<%=TxtDescriptionUser.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionUser.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionUser.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtUser.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionUser.ClientID%>").value = descrizione;
        }

        function aceSelectedRole(sender, e) {
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

            var searchText = $get('<%=TxtDescriptionRole.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionRole.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionRole.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtRole.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionRole.ClientID%>").value = descrizione;
        }

        function SelectAllCheckBox(chk) {
            if (chk.checked)
                $('.tbl_rounded_custom td input[type="checkbox"]').attr('checked', 'checked');
            else
                $('.tbl_rounded_custom td input[type="checkbox"]').removeAttr('checked');
            SetItemCheck();
        }

        function SetItemCheck() {
            $('#hdnIds').val('');
            $('.tbl_rounded_custom td input[type="checkbox"]').each(function (index) {
                if (this.checked) {
                    if ($('#hdnIds').val().length > 0)
                        $('#hdnIds').val($('#hdnIds').val()+',');
                    $('#hdnIds').val($('#hdnIds').val()+$(this).parent().attr('rel'));
                }
            });
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup Id="VisibilityHistory" runat="server" Url="../popup/VisibilityHistory.aspx?tipoObj=D"
        Width="800" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('container', ''); }" />
    <uc:ajaxpopup Id="VisibilityRemove" runat="server" Url="../popup/VisibilityRemove.aspx"
        Width="600" Height="400" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('container', ''); }" />
    <uc:ajaxpopup Id="SignatureA4" runat="server" Url="../popup/Signature.aspx?printSignatureA4=true"
        PermitClose="false" PermitScroll="false" Width="1000" Height="1200" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <uc:ajaxpopup Id="MassiveVisibilityRemove" runat="server" Url="../popup/MassiveVisibilityRemove.aspx"
        Width="600" Height="400" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('container', ''); }" />
    <uc:ajaxpopup Id="MassiveVisibilityRestore" runat="server" Url="../popup/MassiveVisibilityRestore.aspx"
        Width="600" Height="400" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('container', ''); }" />
    <uc:ajaxpopup Id="VisibilityHistoryRole" runat="server" Url="../popup/VisibilityHistoryRole.aspx"
        Width="900" Height="500" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui) {__doPostBack('panelButtons', 'closeVisibilityHistoryRole'); }" />
    <uc:ajaxpopup Id="PrintLabel" runat="server" Url="../popup/PrintLabel.aspx"
        PermitClose="false" PermitScroll="false" Width="300" Height="2" CloseFunction="function (event, ui) { __doPostBack('panelButtons', '');}" />
    <div id="containerTop">
        <asp:UpdatePanel ID="UpUserControlHeaderDocument" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <uc2:HeaderDocument runat="server" ID="HeaderDocument" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="UpcontainerDocumentTabLeftBorder" runat="server" UpdateMode="Conditional"
            ClientIDMode="static">
            <ContentTemplate>
                <div id="containerDocumentTab" runat="server" clientidmode="Static">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <div id="containerDocumentTabOrangeSx">
                            <asp:UpdatePanel runat="server" ID="UpContainerDocumentTab" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <uc4:DocumentTabs runat="server" PageCaller="VISIBILITY" ID="DocumentTabs"></uc4:DocumentTabs>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="containerDocumentTabOrangeDx">
                            <uc3:DocumentButtons runat="server" ID="DocumentButtons" PageCaller="VISIBILITY" Visible="false" />
                            <%--Azioni massive --%>
                            <asp:UpdatePanel ID="UpnlAzioniMassive" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <div class="rowMassiveOperation">
                                        <div class="col">
                                            <asp:DropDownList runat="server" ID="DdlMassiveOperation" Width="400"
                                                AutoPostBack="true" CssClass="chzn-select-deselect" OnSelectedIndexChanged="DdlMassiveOperation_SelectedIndexChanged">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <asp:UpdatePanel ID="UpPnlInfoVisibility" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
                                        <ContentTemplate>
                                            <asp:Panel ID="PnlInfoVisibility" runat="server" Visible="false">
                                                <div id="containerProjectCxBottomDx">
                                                    <asp:ImageButton ID="ImgInfoVisibility" src= "../Images/Common/messager_warning.png" runat="server" 
                                                    style="margin-left:130px" Width="25px" Height="25px" CssClass="clickableLeftN"/>
                                                </div>
                                            </asp:Panel>
                                            </ContentTemplate>
                                    </asp:UpdatePanel>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <asp:UpdatePanel runat="server" ID="UpcontainerDocumentTabDxBorder" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerDocumentTabDxBorder" runat="server" clientidmode="Static">
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="container" runat="server" clientidmode="Static">
                <div id="content">
                    <div id="contentSx">
                        <div class="box_inside">
                            <fieldset>
                                <div class="row">
                                    <div class="col">
                                        <span class="weight">
                                            <asp:Literal ID="litSubject" runat="server" /></span>
                                    </div>
                                </div>
                                <div id="row_object" class="row">
                                    <asp:Literal ID="litObject" runat="server" /></div>
                            </fieldset>
                            <div class="row">
                                <fieldset>
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">
                                                    <asp:Literal ID="litUser" runat="server" /></span><span class="little"></span></p>
                                        </div>
                                        <div class="col-right-no-margin">
                                            <cc1:CustomImageButton ID="VisibilityImgAddressBookUser" ImageUrl="../Images/Icons/address_book.png"
                                                runat="server" OnMouseOverImage="../Images/Icons/address_book_hover.png" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                OnMouseOutImage="../Images/Icons/address_book.png" CssClass="clickable" OnClick="VisibilityImgAddressBookUser_Click" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <asp:UpdatePanel runat="server" ID="UpPnlUser" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:HiddenField ID="IdSender" runat="server" />
                                                <div class="colHalf">
                                                    <cc1:CustomTextArea ID="TxtUser" runat="server" CssClass="txt_addressBookLeft" OnTextChanged="TxtUser_OnTextChanged"
                                                        AutoPostBack="true"></cc1:CustomTextArea>
                                                </div>
                                                <div class="colHalf2">
                                                    <div class="colHalf3">
                                                        <cc1:CustomTextArea ID="TxtDescriptionUser" runat="server" CssClass="txt_addressBookRight"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                               <uc1:AutoCompleteExtender runat="server" ID="RapidUser" TargetControlID="TxtDescriptionUser"
                                                    CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                    CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                    MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                    DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                    UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompletUser"
                                                    OnClientPopulated="acePopulatedUser" Enabled="false">
                                                </uc1:AutoCompleteExtender>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </fieldset>
                                <div class="row">
                                    <fieldset>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal ID="litRole" runat="server" /></span><span class="little"></span></p>
                                            </div>
                                            <div class="col-right-no-margin">
                                                <cc1:CustomImageButton ID="VisibilityImgAddressBookRole" ImageUrl="../Images/Icons/address_book.png"
                                                    runat="server" OnMouseOverImage="../Images/Icons/address_book_hover.png" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                    OnMouseOutImage="../Images/Icons/address_book.png" CssClass="clickable" OnClick="VisibilityImgAddressBookRole_Click" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <asp:UpdatePanel runat="server" ID="UpPnlRole" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:HiddenField ID="IdRecipient" runat="server" />
                                                    <div class="colHalf">
                                                        <cc1:CustomTextArea ID="TxtRole" runat="server" CssClass="txt_addressBookLeft" OnTextChanged="TxtRole_TextChanged"
                                                            AutoPostBack="true"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="colHalf2">
                                                        <div class="colHalf3">
                                                            <cc1:CustomTextArea ID="TxtDescriptionRole" runat="server" CssClass="txt_addressBookRight"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                     <uc1:AutoCompleteExtender runat="server" ID="RapidRole" TargetControlID="TxtDescriptionRole"
                                                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                        MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                        UseContextKey="true" OnClientItemSelected="aceSelectedRole" BehaviorID="AutoCompletRole"
                                                        OnClientPopulated="acePopulatedRole" Enabled="false">
                                                    </uc1:AutoCompleteExtender>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </fieldset>
                                </div>
                                <div class="row">
                                    <fieldset>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal ID="litMotive" runat="server" /></span><span class="little"></span></p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <asp:UpdatePanel runat="server" ID="UpdatePanel2" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:HiddenField ID="HiddenField2" runat="server" />
                                                        <%--<div class="colHalf">--%>
                                                        <asp:DropDownList ID="DdlCause" runat="server" Width="300" CssClass="chzn-select-deselect">
                                                            <asp:ListItem id="optAll" Value="" Selected="True" />
                                                            <asp:ListItem id="optA" Value="A" />
                                                            <asp:ListItem id="optAC" Value="AC" />
                                                            <asp:ListItem id="optF" Value="F" />
                                                            <asp:ListItem id="optUP" Value="UP" />
                                                            <asp:ListItem id="optRP" Value="RP" />
                                                            <asp:ListItem id="optT" Value="T" />
                                                        </asp:DropDownList>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal ID="litTypeRight" runat="server" /></span><span class="little"></span></p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <asp:UpdatePanel runat="server" ID="UpdatePanel3" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <div class="colHalf">
                                                        <%--<div class="col-full">--%>
                                                        <asp:DropDownList ID="DdlType" runat="server" Width="100" CssClass="chzn-select-deselect">
                                                            <asp:ListItem id="optAll2" Selected="True" Value="" />
                                                            <asp:ListItem id="optR" Value="R" />
                                                            <asp:ListItem id="optW" Value="W" />
                                                        </asp:DropDownList>
                                                    </div>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">
                                                        <asp:Literal ID="litDateRight" runat="server" /></span>
                                                </p>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col">
                                                <asp:DropDownList ID="DdlDate" runat="server" Width="150" AutoPostBack="True" OnSelectedIndexChanged="DdlDate_SelectedIndexChanged"
                                                    CssClass="chzn-select-deselect">
                                                    <asp:ListItem id="opt0" Selected="True" Value="0" />
                                                    <asp:ListItem id="opt1" Value="1" />
                                                    <asp:ListItem id="opt2" Value="2" />
                                                    <asp:ListItem id="opt3" Value="3" />
                                                    <asp:ListItem id="opt4" Value="4" />
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col2">
                                                <asp:Label ID="VisibilityFrom" runat="server"></asp:Label>
                                            </div>
                                            <div class="col4">
                                                <cc1:CustomTextArea ID="TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                    CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                            </div>
                                            <div class="col2">
                                                <asp:Label ID="VisibilityTo" runat="server" Visible="False"></asp:Label>
                                            </div>
                                            <div class="col4">
                                                <cc1:CustomTextArea ID="TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                    CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                            </div>
                                        </div>
                                    </fieldset>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <asp:UpdatePanel ID="UpdPanelVisibility" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="GridDocuments" runat="server" ClientIDMode="Static" CssClass="tbl_rounded_custom round_onlyextreme"
                                    Width="99%" AutoGenerateColumns="false" AllowPaging="False" AllowSorting="true"
                                    OnSorting="GridDocuments_Sorting" OnRowCreated="GridDocuments_RowCreated"
                                    OnRowCommand="GridDocuments_RowCommand" OnSelectedIndexChanging="GridDocuments_SelectedIndexChanging" OnPageIndexChanging="GridDocuments_PageIndexChanging"
                                    DataKeyNames="idCorr" OnPreRender="GridDocuments_PreRender" BorderWidth="0">
                                    <RowStyle CssClass="NormalRow" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <input id="selectAll" onclick="SelectAllCheckBox(this);" type="checkbox" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkbox" runat="server" Width="18px" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Image ID="imgTipo" runat="server" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" SortExpression="Ruolo">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRole" runat="server" Text='<%# Bind("Ruolo") %>'></asp:Literal>
                                                <div id="divDetails" runat="server" visible="false">
                                                    <div>
                                                        <strong>
                                                            <asp:Literal ID="lblDetailsUser" runat="server" Visible="false" /></strong></div>
                                                    <asp:Literal ID="LblDetails" runat="server"></asp:Literal>
                                                    <asp:Literal ID="VisibilityLblDetails" runat="server" />
                                                    <asp:Literal ID="LblDetailsInfo" runat="server" />
                                                    <asp:HiddenField ID="hdnTipo" runat="server" Value='<%#Eval("Tipo") %>' />
                                                    <asp:HiddenField ID="hdnCodRubrica" runat="server" Value='<%#Eval("CodiceRubrica") %>' />
                                                    <asp:HiddenField ID="hdnDiSistema" runat="server" Value='<%#Eval("DiSistema") %>' />
                                                </div>
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Left" />
                                            <ControlStyle Width="120px" />
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <cc1:CustomImageButton ID="ImgRoleHistory" runat="server" CommandName="ShowHistoryRole" ImageUrl="../Images/Icons/obj_history.png"
                                                    OnMouseOutImage="../Images/Icons/obj_history.png" OnMouseOverImage="../Images/Icons/obj_history_hover.png"
                                                    ToolTip='<%$ localizeByText:VisibilityObjHistoryRole%>' CssClass="clickableLeft"
                                                     Visible='<%# this.ShowHistoryRole((NttDataWA.Document.Visibility.DocumentsVisibility) Container.DataItem) %>' />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField SortExpression="diritti">
                                            <ItemTemplate>
                                                <asp:Label ID="LblDiritto" runat="server" Text='<%# Bind("diritti") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="TipoDiritto" SortExpression="TipoDiritto" />
                                        <asp:BoundField DataField="DataInsSecurity" SortExpression="DataInsSecurity" />
                                        <asp:TemplateField HeaderStyle-Wrap="false" SortExpression="DataFine">
                                            <ItemTemplate>
                                                <asp:Label ID="LblEndDate" runat="server" Text='<%# Bind("DataFine") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="NoteSecurity" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <cc1:CustomImageButton ID="ImgDelete" runat="server" CommandName="Erase" ImageAlign="Middle"
                                                    ImageUrl="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                    OnMouseOutImage="../Images/Icons/delete.png" CssClass="clickableLeft" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                                            <HeaderTemplate>
                                                <cc1:CustomImageButton ID="ImgHistory" runat="server" CommandName="History" ImageUrl="../Images/Icons/obj_history.png"
                                                    OnMouseOutImage="../Images/Icons/obj_history.png" OnMouseOverImage="../Images/Icons/obj_history_hover.png"
                                                    OnClientClick="return ajaxModalPopupVisibilityHistory();" ToolTip='<%$ localizeByText:VisibilityObjHistory%>'
                                                    AlternateText='<%#  this.GetTitle()%>' CssClass="clickableLeft" />
                                            </HeaderTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField Visible="False">
                                            <ItemTemplate>
                                                <asp:Label ID="LblRemoved" runat="server" Text='<%# Bind("Rimosso") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Note" Visible="False" />
                                        <asp:BoundField HeaderText="idCorr" Visible="False" DataField="idCorr" />
                                        <asp:TemplateField Visible="False">
                                            <ItemTemplate>
                                                <cc1:CustomImageButton ID="ImgDetails" runat="server" CommandName="Details" ImageAlign="Middle"
                                                    ImageUrl="~/Images/Icons/docDetails.png" OnMouseOverImage="~/Images/Icons/docDetails_hover.png"
                                                    OnMouseOutImage="~/Images/Icons/docDetails.png" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="GridDocuments" EventName="PageIndexChanging" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <asp:HiddenField ID="rowIndex" runat="server" ClientIDMode="Static" />
                        <asp:Button ID="btnDetails" runat="server" ClientIDMode="Static" CssClass="hidden"
                            OnClick="GridDocuments_Details" />
                        <div class="row">
                            <asp:Label ID="VisibilityLblDelVis" runat="server" Visible="False"></asp:Label>
                            <br />
                            <asp:Label ID="lblResult" runat="server" ForeColor="Red" Visible="False"></asp:Label>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- end of container -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="panelButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ID="VisibilityBtnFilter" OnClick="VisibilityBtnFilter_Click" />
            <cc1:CustomButton ID="VisibilityBtnClear" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="VisibilityBtnClear_Click" />

            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
            <asp:HiddenField ID="hdnIds" runat="server" ClientIDMode="Static" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
