<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="ReportFormatiIstanzaConservazione.aspx.cs" Inherits="NttDataWA.Popup.ReportFormatiIstanzaConservazione" %>

<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ACT" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="NttDL" %>
<%@ Register Src="~/UserControls/Calendar.ascx" TagPrefix="uc6" TagName="Calendar" %>
<%@ Register Src="~/UserControls/CorrespondentCustom.ascx" TagPrefix="uc7" TagName="Correspondent" %>
<asp:Content ID="ContentPlaceHolderHeader" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <asp:UpdatePanel ID="UpPnlGridDocResult" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <div class="row">
                <div class="colHalf6">
                    <strong>
                        <asp:Literal ID="litNumDoc" runat="server"></asp:Literal></strong>
                </div>
                <div class="col">
                    <asp:Literal ID="litNumDocValue" runat="server"></asp:Literal>
                </div>
                <div class="row">
                    <div class="colHalf6">
                        <strong>
                            <asp:Literal ID="litNumFile" runat="server"></asp:Literal></strong>
                    </div>
                    <div class="col">
                        <asp:Literal ID="litNumFileValue" runat="server"></asp:Literal>
                    </div>
                </div>
                <div class="row">
                    <div class="colHalf">
                    </div>
                    <strong>
                        <asp:Literal ID="litFiltriBase" runat="server"></asp:Literal></strong>
                </div>
                <div class="colHalf5">
                    <asp:RadioButtonList ID="rbtConformDoc" runat="server">
                        <asp:ListItem Text="Tutti" Value="0" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="Solo Conformi" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Solo
            non Conformi" Value="2"></asp:ListItem>
                    </asp:RadioButtonList>
                </div>
            </div>
            <div class="row">
            </div>
            <div class="row">
            </div>
            <div class="row">
                <div class="colHalf">
                </div>
                <strong>
                    <asp:Literal ID="litFiltriAvanzati" runat="server"></asp:Literal></strong>
            </div>
            <div class="row">
                <div class="colHalf10">
                    <asp:CheckBox ID="chkAmmessi" runat="server" />
                    <asp:Literal ID="litAmmessi" runat="server"></asp:Literal>
                </div>
                <div class="colHalf10">
                    <asp:CheckBox ID="chkNonAmmessi" runat="server" />
                    <asp:Literal ID="litNonAmmessi" runat="server"></asp:Literal>
                </div>
            </div>
            <div class="row">
                <div class="colHalf10">
                    <asp:CheckBox ID="chkValido" runat="server" />
                    <asp:Literal ID="litValido" runat="server"></asp:Literal>
                </div>
                <div class="colHalf10">
                    <asp:CheckBox ID="chkNonValido" runat="server" />
                    <asp:Literal ID="litNonValido" runat="server"></asp:Literal>
                </div>
                <div class="colHalf10">
                    <asp:CheckBox ID="chkNonDaValidare" runat="server" />
                    <asp:Literal ID="litNonDaValidare" runat="server"></asp:Literal>
                </div>
            </div>
            <div class="row">
                <div class="colHalf10">
                    <asp:CheckBox ID="chkConv" runat="server" />
                    <asp:Literal ID="litConv" runat="server"></asp:Literal>
                </div>
                <div class="colHalf10">
                    <asp:CheckBox ID="chkNonConvDaMe" runat="server" />
                    <asp:Literal ID="litNonConv" runat="server"></asp:Literal>
                </div>
                <div class="colHalf10">
                    <asp:CheckBox ID="chkNonDaConv" runat="server" />
                    <asp:Literal ID="litNonDaConv" runat="server"></asp:Literal>
                </div>
                <div class="colHalf10">
                    <asp:CheckBox ID="chkNonConvertibili" runat="server" />
                    <asp:Literal ID="litNonConvertibili" runat="server"></asp:Literal>
                </div>
            </div>
            <div class="row">
                <div class="colHalf10">
                    <asp:CheckBox ID="chkError" runat="server" />
                    <asp:Literal ID="litError" runat="server"></asp:Literal>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel ID="upPnlButtons" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <NttDL:CustomButton ID="btnGeneraReport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnGeneraReport_Click" />
            <NttDL:CustomButton ID="btnChiudi" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="btnChiudi_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
