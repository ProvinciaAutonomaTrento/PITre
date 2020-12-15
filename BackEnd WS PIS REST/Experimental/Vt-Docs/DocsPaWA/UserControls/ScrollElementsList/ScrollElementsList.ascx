<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScrollElementsList.ascx.cs" Inherits="DocsPAWA.UserControls.ScrollElementsList.ScrollElementsList" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<cc1:imagebutton id="btn_ScrollPrev" ImageAlign="Middle" Thema="prev_" SkinID="ScrollElements" 
    disabledurl="" Runat="server" 
    onclick="btn_Scroll_Click" style="width: 18px; height: 18px"></cc1:imagebutton>
<asp:Label ID="lbl_ScrollElementsList" cssclass="testo_grigio" runat="server"></asp:Label>
<cc1:imagebutton id="btn_ScrollNext" ImageAlign="Middle" Thema="next_" SkinID="ScrollElements" 
    disabledurl="" Runat="server" 
    onclick="btn_Scroll_Click" style="width: 18px"></cc1:imagebutton>