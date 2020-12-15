<%@ Control Language="c#" AutoEventWireup="false" Codebehind="NavigationContext.ascx.cs" Inherits="SAAdminTool.SiteNavigation.NavigationContext" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<cc1:imagebutton id="btnBack" ImageAlign="Middle" Thema="back" SkinID="navigation"
	disabledurl="../images/backnavigation.gif" BorderColor="Black" BorderWidth="0px" Width="18px" Height=18px
	CssClass="menu_1_grigio" Runat="server" Visible="False"></cc1:imagebutton>&nbsp;
<asp:Label cssclass="testo_grigio" id="lblCallContextList" runat="server"></asp:Label>
