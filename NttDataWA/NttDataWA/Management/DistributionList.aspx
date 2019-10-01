<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DistributionList.aspx.cs"
    Inherits="NttDataWA.Management.DistributionList" MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .tbl_rounded td
        {
            vertical-align: top;
        }
        .tbl_rounded tr.nopointer td
        {
            cursor: default;
        }
        .tbl_rounded tr
        {
            cursor: pointer;
        }
        .tbl_rounded th
        {
            white-space: nowrap;
        }
        .recordNavigator2, .recordNavigator2 table, .recordNavigator2 tr, .recordNavigator2 td
        {
            background-color: #EEEEEE;
        }
        .recordNavigator2 td
        {
            border: 0;
        }
    </style>
    <script type="text/javascript">
        function acePopulated(sender, e) {
            var behavior = $find('AutoCompleteExIngressoBIS');
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


            var searchText = $get('<%=TxtDescriptionSender.ClientID %>').value;
            searchText = searchText.replace('null', '');
            var testo = value;
            var indiceFineCodice = testo.lastIndexOf(')');
            document.getElementById("<%=this.TxtDescriptionSender.ClientID%>").focus();
            document.getElementById("<%=this.TxtDescriptionSender.ClientID%>").value = "";
            var indiceDescrizione = testo.lastIndexOf('(');
            var descrizione = testo.substr(0, indiceDescrizione - 1);
            var codice = testo.substring(indiceDescrizione + 1, indiceFineCodice);
            document.getElementById("<%=this.TxtCodeSender.ClientID%>").value = codice;
            document.getElementById("<%=TxtDescriptionSender.ClientID%>").value = descrizione;
        }
    </script>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup2 Id="AddressBook" runat="server" Url="../Popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  { $('#btnAddressBookPostback').click(); }" />
    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p>
                                        <asp:Literal ID="DistributionListTitle" runat="server"></asp:Literal></p>
                                </div>
                                <div id="containerStandardTopDx">
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional"
            ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerStandardTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <div id="containerDocumentTabOrangeSx">
                            <asp:UpdatePanel runat="server" ID="UpnlTabHeader" UpdateMode="Conditional" ClientIDMode="Static">
                                <ContentTemplate>
                                    <ul>
                                        <li id="distributionListLi" runat="server" class="prjIAmProfile">
                                            <asp:LinkButton ID="DistributionListLink" runat="server"></asp:LinkButton></li>
                                    </ul>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div id="containerDocumentTabOrangeDx">
                            <asp:UpdatePanel ID="UpdPnDisp" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col" style="margin-left: 60px; margin-top: 5px;">
                                            <asp:Label ID="lblDisp" runat="server" class="weight"></asp:Label>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div id="containerStandardTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard2" runat="server" clientidmode="Static">
                <div id="content">
                    <div id="contentSx">
                        <div class="box_inside" style="margin-top: 10px">
                            <asp:HiddenField ID="deleteList" runat="server" ClientIDMode="Static" />
                            <asp:UpdatePanel ID="upPnlGridList" runat="server" class="row" UpdateMode="Conditional"
                                ClientIDMode="Static">
                                <ContentTemplate>
                                    <asp:GridView ID="grdList" runat="server" Width="98%" AutoGenerateColumns="False"
                                        AllowPaging="True" ClientIDMode="Static" CssClass="tbl_rounded_custom round_onlyextreme"
                                        PageSize="10" OnRowCommand="grdList_RowCommand" OnPageIndexChanging="grdList_PageIndexChanging"
                                        OnPreRender="grdList_PreRender">
                                        <RowStyle CssClass="NormalRow" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <Columns>
                                            <asp:TemplateField ItemStyle-CssClass="grdList_date" HeaderStyle-Wrap="false" ItemStyle-HorizontalAlign="Left"
                                                HeaderStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <%# DataBinder.Eval(Container, "DataItem.VAR_DESC_CORR").ToString()%>
                                                    <asp:HiddenField ID="listId" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem.SYSTEM_ID") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-CssClass="grdList_del"
                                                ItemStyle-Width="10px">
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton ID="ImgDetail" runat="server" CommandName="Select" ImageUrl="../Images/Icons/search_response_documents.png"
                                                        OnMouseOutImage="../Images/Icons/search_response_documents.png" CausesValidation="false" OnMouseOverImage="../Images/Icons/search_response_documents.png"
                                                        ToolTip='<%# this.GetViewLabel()%>' AlternateText='<%# this.GetViewLabel()%>'
                                                        CssClass="clickableLeft" />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-CssClass="grdList_del"
                                                ItemStyle-Width="10px" ItemStyle-HorizontalAlign="center">
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton ID="ImgDelete" runat="server" CommandName="Erase" ImageAlign="Middle"
                                                        ImageUrl="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                        OnMouseOutImage="../Images/Icons/delete.png" CssClass="clickableLeft" ToolTip='<%# this.GetRemoveLabel()%>'
                                                        AlternateText='<%# this.GetRemoveLabel()%>' />
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" />
                                    <asp:HiddenField ID="HiddenField1" runat="server" ClientIDMode="Static" />
                                    <asp:HiddenField ID="grid_rowindex" runat="server" ClientIDMode="Static" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="grdList" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div id="contentDx">
                        <div id="contentDxTopProject">
                            <asp:UpdatePanel runat="server" ID="UpPnlRadio" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="box_inside">
                                        <div class="row" style="margin-left: 30px;">
                                            <div class="col">
                                                <asp:RadioButton ID="rbOnlyMe" runat="server" CssClass="rblHorizontal" GroupName="Type"
                                                    Enabled="false" />
                                            </div>
                                        </div>
                                        <div class="row" style="margin-left: 30px;">
                                            <div class="col">
                                                <asp:RadioButton ID="rbRole" runat="server" CssClass="rblHorizontal" GroupName="Type"
                                                    Enabled="false" />
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row" style="height: 10px">
                            </div>
                            <asp:UpdatePanel runat="server" ID="UpdPanelDetail" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="colHalf" style="width: 30%;">
                                            <asp:Literal ID="LitCodLista" runat="server" />
                                            <cc1:CustomTextArea ID="TxtCodList" runat="server" CssClass="txt_addressBookLeft"
                                                Style="width: 50%" CssClassReadOnly="txt_addressBookLeft_disabled" ReadOnly="true">
                                            </cc1:CustomTextArea>
                                        </div>
                                        <div class="colHalf" style="width: 70%; text-align: left">
                                            <asp:Literal ID="LitDescrLista" runat="server" />
                                            <cc1:CustomTextArea ID="TxtDescList" runat="server" Style="width: 80%" CssClass="txt_addressBookLeft"
                                                CssClassReadOnly="txt_addressBookLeft_disabled" ReadOnly="true">
                                            </cc1:CustomTextArea>
                                        </div>
                                    </div>
                                    <div class="row" style="height: 10px">
                                    </div>
                                    <asp:HiddenField ID="idCorrispondente" runat="server" />
                                    <div class="row" style="width: 100%">
                                        <div class="colHalf" style="width: 30%;">
                                            <asp:Literal ID="LitCorr" runat="server" />
                                            <cc1:CustomTextArea ID="TxtCodeSender" runat="server" CssClass="txt_addressBookLeft"
                                                Style="width: 50%; margin-left: 5px" AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled"
                                                AutoCompleteType="Disabled" OnTextChanged="TxtCodeSender_TextChanged" ReadOnly="true">
                                            </cc1:CustomTextArea>
                                        </div>
                                        <div class="colHalf" style="width: 60%;">
                                            <cc1:CustomTextArea ID="TxtDescriptionSender" runat="server" CssClass="txt_addressBookRight"
                                                CssClassReadOnly="txt_addressBookRight_disabled" AutoCompleteType="Disabled"
                                                ReadOnly="true" Width="384px"></cc1:CustomTextArea>
                                          <uc1:AutoCompleteExtender runat="server" ID="RapidSender" TargetControlID="TxtDescriptionSender"
                                                CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                UseContextKey="true" OnClientItemSelected="aceSelected" BehaviorID="AutoCompleteExIngressoBIS"
                                                OnClientPopulated="acePopulated" Enabled="false">
                                            </uc1:AutoCompleteExtender>
                                        </div>
                                        <div class="colHalf" style="width: 10%; text-align: left">
                                            <cc1:CustomImageButton runat="server" ID="DocumentImgSenderAddressBook" ImageUrl="../Images/Icons/address_book.png"
                                                OnMouseOutImage="../Images/Icons/address_book.png" OnMouseOverImage="../Images/Icons/address_book_hover.png"
                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/address_book_disabled.png"
                                                OnClick="DocumentImgSenderAddressBook_Click" Enabled="false" />
                                            <cc1:CustomImageButton ID="DocumentImgAddNewCorrispondent" runat="server" ImageUrl="../Images/Icons/add_version.png"
                                                OnMouseOutImage="../Images/Icons/add_version.png" OnMouseOverImage="../Images/Icons/add_version_hover.png"
                                                ImageUrlDisabled="../Images/Icons/add_version_disabled.png" CssClass="clickableLeft"
                                                ClientIDMode="Static" OnClick="DocumentImgAddNewCorrispondent_Click" Enabled="false" />
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                            <div class="row" style="height: 10px">
                            </div>
                        </div>
                        <div class="row" style="border-top: 1px solid #cccccc; padding-top: 10px;">
                            <asp:HiddenField ID="deleteCorr" runat="server" ClientIDMode="Static" />
                            <asp:UpdatePanel runat="server" ID="UpnlGrid" UpdateMode="Conditional" class="UpnlGrid">
                                <ContentTemplate>
                                    <asp:GridView ID="gridViewResult" runat="server" Width="97%" AllowPaging="true" PageSize="15"
                                        AutoGenerateColumns="false" CssClass="tbl_rounded_custom round_onlyextreme" HorizontalAlign="Left"
                                        ShowHeader="true" ShowHeaderWhenEmpty="true" OnRowCommand="gridViewResult_RowCommand"
                                        OnPageIndexChanging="gridViewResult_PageIndexChanging" EnableViewState="true">
                                        <RowStyle CssClass="NormalRow" />
                                        <PagerStyle CssClass="recordNavigator2" />
                                        <AlternatingRowStyle CssClass="AltRow" />
                                        <Columns>
                                            <asp:TemplateField HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <%# DataBinder.Eval(Container, "DataItem.VAR_DESC_CORR").ToString()%>
                                                    <asp:HiddenField ID="ID_DPA_CORR" runat="server" Value='<%# DataBinder.Eval(Container, "DataItem.ID_DPA_CORR") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderStyle-Width="10px" ItemStyle-Width="10px" ItemStyle-CssClass="grdList_del"
                                                ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <cc1:CustomImageButton ID="ImgDelete" runat="server" CommandName="Erase" ImageAlign="Middle"
                                                        ImageUrl="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                        OnMouseOutImage="../Images/Icons/delete.png" CssClass="clickableLeft" ToolTip='<%# this.GetRemoveLabel()%>'
                                                        AlternateText='<%# this.GetRemoveLabel()%>' />
                                                </ItemTemplate>
                                                <ItemStyle />
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                    <asp:PlaceHolder ID="plcNavigator" runat="server" />
                                    <asp:HiddenField ID="grid_pageindex" runat="server" ClientIDMode="Static" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="gridViewResult" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel runat="server" ID="UpDocumentButtons" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="ListBtnNew" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="ListBtnNew_Click" />
            <cc1:CustomButton ID="ListBtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="ListBtnSave_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
