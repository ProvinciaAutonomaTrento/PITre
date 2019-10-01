<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="NoteImport.aspx.cs" Inherits="NttDataWA.Popup.NoteImport" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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
<div class="container">
    <asp:PlaceHolder ID="plcFilename" runat="server">
        <div class="row">
            <div class="colHalf"><asp:Literal ID="lblTemplate" runat="server" visible="false" /></div>
            <div class="colHalf2"><img src="../Images/Icons/small_xls.png" alt="" /> <asp:HyperLink ID="lnkTemplate" runat="server" NavigateUrl="../ImportDati/ImportNote.xls" Target="_blank" /></div>
        </div>
        <div class="row">
            <div class="colHalf"><asp:Literal ID="lblFilename" runat="server" /></div>
            <div class="col"><asp:FileUpload ID="uploadedFile" runat="server" /></div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plcReport" runat="server" Visible="false">
        <asp:GridView ID="grdRisExcel" runat="server" AutoGenerateColumns="false" Width ="98%"
            CssClass="tbl_rounded round_onlyextreme"
        >
            <RowStyle CssClass="NormalRow" />
            <AlternatingRowStyle CssClass="AltRow" />
            <PagerStyle CssClass="recordNavigator2" />
            <Columns>
                <asp:BoundField DataField="Message" ItemStyle-Width="39%" />
                <asp:TemplateField ItemStyle-Width="10%">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Literal ID="ltlResult" runat="server" Text="<%#this.GetResult((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ItemStyle-Width="50%">
                    <ItemTemplate>
                        <asp:Literal ID="ltlDetails" runat="server" Text="<%#this.GetDetails((NttDataWA.DocsPaWR.ImportResult) Container.DataItem)%>" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:PlaceHolder>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnConfirm" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnConfirm_Click" OnClientClick="disallowOp('Content2');" />
    <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2');" />
</asp:Content>
