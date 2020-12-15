<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="tabFascCampiProf.aspx.cs" Inherits="DocsPAWA.fascicolo.tabFascCampiProf"%>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc3" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html>
<head id="Head1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <meta http-equiv="Pragma" content="no-cache">
    <meta http-equiv="Expires" content="-1">

    <LINK href="../CSS/ProfilazioneDinamica.css" type="text/css" rel="stylesheet">
	<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
	<script language="JavaScript" src="../CSS/ETcalendar.js"></script>    
    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
</head>
<body>
    <form id="tabFascCP" runat="server">
    <asp:ScriptManager ID="ScriptManagerProfDinam" AsyncPostBackTimeout="360000" runat="server"></asp:ScriptManager>
    <div>
    <asp:Panel id="panel_profDinamica" runat="server" Visible="false" BorderWidth="1" 
            BorderColor="white" Height="100%" ScrollBars="Auto">
                <table width="99.5%" style="margin-left:0px; border: #800000 1px solid; BACKGROUND-COLOR: #fafafa;">
                     <%--<tr>
                        <td class="titolo_scheda">&nbsp;Tipologia fascicolo :&nbsp;&nbsp;</td>
                        <td style="padding-top:5px"><asp:DropDownList ID="ddl_tipologiaFasc" runat="server" CssClass="testo_grigio" AutoPostBack="True" Width="350px"></asp:DropDownList></td>
                    </tr>--%>
              
                    <tr>
                        <td colspan="2">
                            <asp:panel id="panel_Contenuto" runat="server" Width="100%" Height="530px" ScrollBars="Auto"></asp:panel></td>
                            <td style="vertical-align:top;">
                                <cc1:imagebutton id="btn_HistoryField" Runat="server" Width="18" AlternateText="Storia modifica campi profilati" DisabledUrl="../images/proto/storia.gif"
									Tipologia="DO_PROFIL_HISTORY" Height="17" ImageUrl="../images/proto/storia.gif"></cc1:imagebutton>
                            </td>
                    </tr>
                </table>
            </asp:Panel>

            
    </div>
    <input type="hidden" id="hd_Update" runat="server" />
    </form>
</body>
</html>
