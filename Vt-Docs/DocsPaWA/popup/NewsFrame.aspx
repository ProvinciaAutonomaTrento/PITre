<%@ Page language="c#" Codebehind="NewsFrame.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.NewsFrame" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register Assembly="DocsPaWebCtrlLibrary" Namespace="DocsPaWebCtrlLibrary" TagPrefix="cc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head runat = "server">
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
  </head>
  <body MS_POSITIONING="GridLayout">
    <form id="Form1" method="post" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "News" />
        <table width="100%"><tr><td width="100%" height="90%">
            <cc1:IFrameWebControl ID="IF_News" runat="server" Align="center" iWidth="100%" iHeight="600px" /> 
<%--		<iframe id="frameUP" src="http://www.gazzetta.it" scrolling="auto" width="100%" height="90%"></iframe>
--%>		</td></tr>
		<tr><td align="center">
		    <INPUT class="pulsante" id="btn_chiudi" style="WIDTH: 58px; HEIGHT: 20px" onclick="window.close();" type="button" value="CHIUDI" name="btn_chiudi" />
		</td></tr></table>
     </form>
  </body>
</html>
