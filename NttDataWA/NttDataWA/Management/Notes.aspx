<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true" CodeBehind="Notes.aspx.cs" Inherits="NttDataWA.Management.Notes" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup2 Id="NoteDataentryNew" runat="server" Url="../Popup/NoteDataentry.aspx?isnew=true" Width="500" Height="400"
        PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui)  {__doPostBack('UpPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="NoteDataentryModify" runat="server" Url="../Popup/NoteDataentry.aspx" Width="500" Height="400"
        PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui)  {__doPostBack('UpPnlButtons', '');}" />
    <uc:ajaxpopup2 Id="NoteImport" runat="server" Url="../Popup/NoteImport.aspx" Width="700" Height="500"
        PermitClose="false" PermitScroll="false" CloseFunction="function (event, ui)  {__doPostBack('UpPnlButtons', '');}" />

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
                                    <p><asp:Label ID="pageTitle" runat="server"></asp:Label></p>
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
                        </div>
                        <div id="containerDocumentTabOrangeDx">
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
                        <div class="box_inside" style="margin-top: 10px; padding: 0 10px;">
                            <div class="row">
                                <div class="colHalf"><asp:Literal ID="lblRf" runat="server" /></div>
                                <div class="colHalf2">
                                    <div style="text-align: left;">
                                        <asp:DropDownList ID="ddlFiltroRf" runat="server" CssClass="chzn-select-deselect" Width="100%" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="colHalf"><asp:Literal ID="lblDesc" runat="server" /></div>
                                <div class="colHalf2">
                                    <cc1:CustomTextArea ID="txt_desc" runat="server" CssClass="txt_input_full" CssClassReadOnly="txt_input_full_disabled" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <asp:UpdatePanel ID="UpPnlGrid" runat="server">
                            <ContentTemplate>
                                <p align="left"><asp:Literal ID="lbl_messaggio" runat="server" /></p>
                                <asp:GridView ID="dgNote" runat="server" CssClass="tbl_rounded_custom round_onlyextreme"
                                    Width="99%" AutoGenerateColumns="false" AllowPaging="False" AllowSorting="false" BorderWidth="0"
                                    OnRowDataBound="dgNote_RowDataBound"
                                    >
                                    <RowStyle CssClass="NormalRow" />
                                    <AlternatingRowStyle CssClass="AltRow" />
                                    <PagerStyle CssClass="recordNavigator2" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sel" ItemStyle-Width="5%">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cbSel" runat="server" AutoPostBack="true" OnCheckedChanged="cbSel_CheckedChanged" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="idnota" ControlStyle-CssClass="hidden" ReadOnly="true" />
                                        <asp:BoundField DataField="codRegRf" HeaderText="Codice" ItemStyle-Width="45%" ReadOnly="true" />
                                        <asp:BoundField DataField="descNota" HeaderText="Descrizione" ItemStyle-Width="45%" ReadOnly="true" />
                                    </Columns>
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnSearch" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnSearch_Click" OnClientClick="disallowOp('');" />
            <cc1:CustomButton ID="BtnNew" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="return ajaxModalPopupNoteDataentryNew();" />
            <cc1:CustomButton ID="BtnModify" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="return ajaxModalPopupNoteDataentryModify();" />
            <cc1:CustomButton ID="BtnDelete" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnDelete_Click" />
            <cc1:CustomButton ID="BtnImport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="return ajaxModalPopupNoteImport();" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
