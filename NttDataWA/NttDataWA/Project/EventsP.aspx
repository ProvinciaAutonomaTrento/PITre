<%@ Page Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true"
    CodeBehind="EventsP.aspx.cs" Inherits="NttDataWA.Project.Events" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Src="~/UserControls/HeaderProject.ascx" TagPrefix="uc1" TagName="HeaderProject" %>
<%@ Register Src="~/UserControls/ProjectTabs.ascx" TagPrefix="uc2" TagName="ProjectTabs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .recordNavigator2, .recordNavigator2 table, .recordNavigator2 td
        {
            background: #eee;
            border: 0;
        }
        .recordNavigator2, .recordNavigator2 td
        {
            border: 0;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <uc:ajaxpopup Id="SignatureA4" runat="server" Url="../popup/Signature.aspx?printSignatureA4=true"
        PermitClose="false" PermitScroll="false" Width="1000" Height="1200" CloseFunction="function (event, ui) {__doPostBack('UpDocumentButtons', '');}" />
    <uc:ajaxpopup Id="Prints" runat="server" Url="../popup/visualReport_iframe.aspx"
        Width="400" Height="300" PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui) { __doPostBack('panelButtons', '');}" />
    <uc:ajaxpopup Id="AddressBook" runat="server" Url="../popup/AddressBook.aspx" PermitClose="false"
        PermitScroll="false" IsFullScreen="true" CloseFunction="function (event, ui)  {$('#btnAddressBookPostback').click();}" />
    <div id="containerTop">
        <asp:UpdatePanel ID="UpUserControlHeaderProject" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <uc1:HeaderProject runat="server" ID="HeaderProject" />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ClientIDMode="Static">
            <ContentTemplate>
                <div id="containerDocumentTab" class="containerProjectTab">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <asp:UpdatePanel runat="server" ID="UpContainerProjectTab" UpdateMode="Conditional">
                            <ContentTemplate>
                                <uc2:ProjectTabs runat="server" PageCaller="EVENTS" ID="ProjectTabs"></uc2:ProjectTabs>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <div id="containerDocumentTabOrangeDx">
                        </div>
                    </div>
                    <div id="containerProjectTabDxBorder">
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerProject" runat="server" clientidmode="Static">
                <div id="content">
                    <div id="contentSx">
                        <div class="box_inside">
                            <fieldset>
                                <div class="row">
                                    <div class="col">
                                        <span class="weight">
                                            <asp:Literal ID="litDescriptionText" runat="server" /></span>
                                    </div>
                                </div>
                                <div id="row_object" class="row">
                                    <asp:Literal ID="litDescription" runat="server" /></div>
                            </fieldset>
                            <div class="row">
                                <fieldset>
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">
                                                    <asp:Literal ID="EventsLblData" runat="server" /></span>
                                            </p>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col3">
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
                                            <asp:Label ID="EventsFrom" runat="server"></asp:Label>
                                        </div>
                                        <div class="col4">
                                            <cc1:CustomTextArea ID="TxtFrom" runat="server" CssClass="txt_textdata2 datepicker"
                                                CssClassReadOnly="txt_textdata_disabled2" ClientIDMode="Static"></cc1:CustomTextArea>
                                        </div>
                                        <div class="col2">
                                            <asp:Label ID="EventsTo" runat="server" Visible="False"></asp:Label>
                                        </div>
                                        <div class="col4">
                                            <cc1:CustomTextArea ID="TxtTo" runat="server" CssClass="txt_textdata2 datepicker"
                                                CssClassReadOnly="txt_textdata_disabled2" Visible="false" ClientIDMode="Static"></cc1:CustomTextArea>
                                        </div>
                                    </div>
                                    <asp:PlaceHolder ID="PnlFilterAuthorAction" runat="server" Visible="false">
                                        <%-- AUTHOR --%>
                                        <asp:UpdatePanel ID="UpPnlAuthor" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlAuthor"></asp:Literal>
                                                            </span>
                                                        </p>
                                                    </div>
                                                    <div class="col">
                                                        <p>
                                                            <asp:RadioButtonList ID="rblOwnerType" runat="server" CssClass="rblHorizontal" RepeatLayout="UnorderedList">
                                                                <asp:ListItem id="optRole" runat="server" Value="R" Selected="True" />
                                                                <asp:ListItem id="optUser" runat="server" Value="P" />
                                                            </asp:RadioButtonList>
                                                        </p>
                                                    </div>
                                                    <div class="col-right-no-margin1">
                                                        <cc1:CustomImageButton runat="server" ID="DocumentImgRecipientAddressBookAuthor"
                                                            ImageUrl="../Images/Icons/address_book.png" OnMouseOutImage="../Images/Icons/address_book.png"
                                                            OnMouseOverImage="../Images/Icons/address_book_hover.png" CssClass="clickable"
                                                            ImageUrlDisabled="../Images/Icons/address_book_disabled.png" OnClick="DocumentImgRecipientAddressBookAuthor_Click" />
                                                    </div>
                                                </div>
                                                <asp:HiddenField runat="server" ID="IdRecipient" />
                                                <asp:HiddenField runat="server" ID="RecipientTypeOfCorrespondent" />
                                                <div class="row">
                                                    <div class="colHalf">
                                                        <cc1:CustomTextArea ID="txt_codAuthor_E" runat="server" CssClass="txt_addressBookLeft"
                                                            AutoPostBack="true" CssClassReadOnly="txt_addressBookLeft_disabled" AutoCompleteType="Disabled"
                                                            OnTextChanged="txt_author_E_TextChanged">
                                                        </cc1:CustomTextArea>
                                                    </div>
                                                    <div class="colHalf2">
                                                        <div class="colHalf3">
                                                            <cc1:CustomTextArea ID="txt_descrAuthor_E" runat="server" CssClass="txt_addressBookRight"
                                                                CssClassReadOnly="txt_addressBookRight" AutoCompleteType="Disabled">
                                                            </cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    <uc1:AutoCompleteExtender runat="server" ID="RapidRecipient" TargetControlID="txt_descrAuthor_E"
                                                        CompletionListCssClass="autocomplete_completionListElement" CompletionListItemCssClass="single_item"
                                                        CompletionListHighlightedItemCssClass="single_item_hover" ServiceMethod="GetListaCorrispondentiVeloce"
                                                        MinimumPrefixLength="7" CompletionInterval="1000" EnableCaching="true" CompletionSetCount="20"
                                                        DelimiterCharacters=";" ServicePath="~/AjaxProxy.asmx" ShowOnlyCurrentWordInCompletionListItem="true"
                                                        UseContextKey="true" OnClientItemSelected="recipientSelected" BehaviorID="AutoCompleteExIngressoRecipient"
                                                        OnClientPopulated="recipientPopulated" Enabled="false">
                                                    </uc1:AutoCompleteExtender>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <%-- EVENT --%>
                                        <asp:UpdatePanel ID="UpPnlEvent" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
                                            <ContentTemplate>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">
                                                                <asp:Literal runat="server" ID="LtlEvent"></asp:Literal>
                                                            </span>
                                                        </p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <asp:DropDownList ID="ddlEvent" Width="98%" runat="server" CssClass="chzn-select-deselect">
                                                    </asp:DropDownList>
                                                </div>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </asp:PlaceHolder>
                                </fieldset>
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <asp:UpdatePanel ID="UpdPanelEvents" UpdateMode="Conditional" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="GridEvents" runat="server" ClientIDMode="Static" CssClass="tbl_rounded_custom round_onlyextreme"
                                    Width="99%" AutoGenerateColumns="False" AllowPaging="True" ShowHeaderWhenEmpty="true"
                                    OnPageIndexChanging="GridEvents_PageIndexChanging" BorderWidth="0" EnableViewState="true"
                                    PageSize="10">
                                    <RowStyle CssClass="NormalRow" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:Label ID="LblData" runat="server" Text='<%# Bind("dataAzione") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="LblOperatore" runat="server" Text='<%# Bind("userIdOperatore") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                            <ItemTemplate>
                                                <asp:Label ID="LblAzione" runat="server" Text='<%# Bind("descrOggetto") %>'></asp:Label>
                                            </ItemTemplate>
                                            <HeaderStyle Wrap="False" />
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
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
                OnMouseOver="btnHover" ID="BtnFilter" OnClick="BtnFilter_Click" />
            <cc1:CustomButton runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ID="BtnRemoveFilter" OnClick="BtnRemoveFilter_Click" />
            <asp:Button ID="btnAddressBookPostback" runat="server" CssClass="hidden" ClientIDMode="Static"
                OnClick="btnAddressBookPostback_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <script type="text/javascript">
        $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato trovato" });
        $(".chzn-select").chosen({ no_results_text: "Nessun risultato trovato" });
    </script>
</asp:Content>
