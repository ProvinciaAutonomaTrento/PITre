<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChooseCorrespondent.aspx.cs"
    Inherits="NttDataWA.Popup.ChooseCorrespondent" MasterPageFile="~/MasterPages/Popup.Master" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <p>
            <asp:Label runat="server" Text="Trovato più di un corrispondente con lo stesso codice. Selezionare il desiderato"
                CssClass="NormalBold" ID="ChooseCorrespondentLabelTitle"></asp:Label></p>
    </div>
    <asp:UpdatePanel runat="server" ID="UpGridCorr" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <asp:GridView ID="GridCorr" runat="server" AllowSorting="True" AutoGenerateColumns="false"
                HorizontalAlign="Center" HeaderStyle-CssClass="tableHeaderPopup" CssClass="tabPopup tbl"
                Width="97%" OnRowDataBound="GridCorr_RowDataBound">
                <SelectedRowStyle CssClass="selectedrow" />
                <RowStyle CssClass="NormalRow" />
                <AlternatingRowStyle CssClass="AltRow" />
                <Columns>
                    <asp:TemplateField Visible="false">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="LblSelCorr" Text='<%# this.GetCorrID((NttDataWA.DocsPaWR.ElementoRubrica)Container.DataItem) %>'> </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-CssClass="grdList_description">
                        <ItemTemplate>
                            <asp:Label ID="gridName" runat="server" Text='<%# this.GetCorrName((NttDataWA.DocsPaWR.ElementoRubrica)Container.DataItem) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-CssClass="grdList_date">
                        <ItemTemplate>
                            <asp:Label ID="gridCodice" runat="server" Text='<%# this.GetCorrCodice((NttDataWA.DocsPaWR.ElementoRubrica)Container.DataItem) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-CssClass="grdList_date">
                        <ItemTemplate>
                            <asp:Label ID="gridTipo" runat="server" Text='<%# this.GetCorrTipo((NttDataWA.DocsPaWR.ElementoRubrica)Container.DataItem) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField Visible="false" ItemStyle-Width="10%" 
                        ItemStyle-HorizontalAlign="center" HeaderStyle-HorizontalAlign="center" ItemStyle-CssClass="tab_dx">
                        <ItemTemplate>
                            <asp:Label ID="gridSto" runat="server" Text='<%# this.GetStoricizzato((NttDataWA.DocsPaWR.ElementoRubrica)Container.DataItem) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:TextBox ID="grid_rowindex" runat="server" CssClass="hidden" OnTextChanged="SetSelectedRow"
                AutoPostBack="true" ClientIDMode="Static"></asp:TextBox>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <div style="float: left;">
        <cc1:CustomButton ID="ChooseCorrespondentBtnOk" runat="server" OnClick="btnOk_Click"
            CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover"  OnClientClick="disallowOp('Content2');" />
    </div>
    <div style="float: left;">
        <cc1:CustomButton ID="ChooseCorrespondentBtnClose" runat="server" OnClick="ObjectBtnChiudi_Click"
            CssClass="btnEnable" CssClassDisabled="btnDisable" OnMouseOver="btnHover" />
    </div>
</asp:Content>
