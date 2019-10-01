<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="VisibilityHistory.aspx.cs" Inherits="NttDataWA.Popup.VisibilityHistory" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .box_inside fieldset:first-child
        {
            position: relative;
            top: 3px;
        }
        
        #ver_editable
        {
            cursor: pointer;
        }
        #ver_editable img
        {
            border: 0;
        }
        #rblFilter_row
        {
            margin: 3px 0;
        }
        #rblFilter_row span
        {
            float: left;
            font-weight: bold;
            margin: 0 10px 0 0;
        }
        #rblFilter
        {
            list-style-type: none;
            display: inline;
        }
        #rblFilter li
        {
            float: left;
            padding: 0;
            margin: 0;
        }
        #row_object
        {
            max-height: 35px;
            overflow: auto;
        }
        
        .tbl
        {
            margin: 3px 0 0 5px;
        }
        .tbl th
        {
            background: url(<%=Page.ResolveClientUrl("~/Images/Common/table_header_bg.png")%>) repeat-x top left;
            color: #fff;
            font-weight: normal;
        }
        .tbl th, .tbl td
        {
            border: 1px solid #d4d4d4;
            padding: 5px;
        }
        .tbl td
        {
            background: #fff;
            cursor: pointer;
        }
        .tbl td.grdAllegati_code
        {
            width: 40px;
            text-align: center;
        }
        .tbl td.grdAllegati_date
        {
            width: 90px;
        }
        .tbl td.grdAllegati_pages
        {
            width: 40px;
            text-align: center;
        }
        .tbl td.grdAllegati_icon
        {
            width: 25px;
        }
        .tbl td.grdAllegati_description
        {
            width: 90%;
            text-align: left;
        }
        .tbl tr.selectedrow td
        {
            background-color: #FCD85C;
        }
        .tbl tr.AltRow td
        {
            background-color: #e1e9f0;
        }
        .tbl tr.NormalRow:hover td, .tbl tr.AltRow:hover td
        {
            background-color: #b2d7f8;
        }
        
        #GrdDocumentAttached
        {
            border: 0;
        }
        #GrdDocumentAttached td
        {
            border: 0;
            padding: 0;
        }
        #GrdDocumentAttached tr.selectedrow span
        {
            background: #276DA9;
            color: #fff;
        }
    </style>
</asp:Content>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div>
        <p>
            <span class="weight">
                <asp:Label ID="LblHistory" runat="server" Visible="False" />
            </span>
        </p>
    </div>
    <asp:UpdatePanel ID="panelHistory" runat="server" class="row" UpdateMode="Conditional"
        ClientIDMode="Static">
        <ContentTemplate>
            <asp:GridView ID="GridHistory" runat="server" Width="98%" AutoGenerateColumns="False"
                AllowPaging="True" ClientIDMode="Static" CssClass="tbl_rounded_custom round_onlyextreme" OnPageIndexChanging="GridHistory_PageIndexChanging">
                <RowStyle CssClass="NormalRow" />
                <AlternatingRowStyle CssClass="AltRow" />
                <Columns>
                    <asp:BoundField DataField="utente" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="ruolo" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                    <asp:BoundField DataField="data"  HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="codOperazione" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="descrizione" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                </Columns>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- end of container -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="HistoryBtnClose" runat="server" CssClass="btnEnable" Style="float: left"
        CssClassDisabled="btnDisable" OnMouseOver="btnHover" OnClick="HistoryBtnClose_Click" />
</asp:Content>
