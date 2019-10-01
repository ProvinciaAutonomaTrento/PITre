<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="OrganizationChartSearchResult.aspx.cs" Inherits="NttDataWA.Popup.OrganizationChartSearchResult" %>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <asp:UpdatePanel ID="UpPnlMain" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="row">
                <p><strong><asp:Literal ID="pageTitle" runat="server" /></strong></p>
				<asp:GridView id="dg_listaRicerca" runat="server" CssClass="tbl_rounded_custom round_onlyextreme"
					AutoGenerateColumns="False" Width="100%">
                    <RowStyle CssClass="NormalRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <PagerStyle CssClass="recordNavigator2" />
                    <Columns>
						<asp:BoundField DataField="IDCorrGlob" ReadOnly="True" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden"></asp:BoundField>
						<asp:BoundField DataField="Codice" ReadOnly="True" ItemStyle-Width="20%" HtmlEncode="false" />
						<asp:BoundField DataField="Descrizione" ReadOnly="True" ItemStyle-Width="40%" HtmlEncode="false" />
						<asp:BoundField DataField="IDParent" ReadOnly="True" HeaderStyle-CssClass="hidden" ItemStyle-CssClass="hidden"></asp:BoundField>
						<asp:BoundField DataField="DescParent" ReadOnly="True" ItemStyle-Width="40%" />
					</Columns>
				</asp:GridView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
