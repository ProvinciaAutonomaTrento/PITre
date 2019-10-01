<%@ Register TagPrefix="cf1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestScarto.aspx.cs" Inherits="DocsPAWA.Scarto.GestScarto" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider" TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="HEAD1" runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <script>
		function Body_OnMouseOver()
		{
			try
			{
			
				if(top.superiore.document!=null)
					if(top.superiore.document.Script!=null)
					if(top.superiore.document.Script["closeIt"]!=null)
						top.superiore.document.Script.closeIt();
			}
			catch(e)
			{
				alert(e.message);
			}
			
		}
		function Body_OnLoad()
		{
			try
			{
			
					if(top.superiore.document!=null)
						if(top.superiore.document.Script!=null)
						if(top.superiore.document.Script["CheckTestataTastoSel"]!=null)
							top.superiore.document.Script.CheckTestataTastoSel();
			}
			catch(e)
			{
			alert(e.message);
			}		
		}
    </script>

</head>
<body ms_positioning="GridLayout" onmouseover="Body_OnMouseOver()" onload="Body_OnLoad()">
    <form id="gestScarto" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Gestione Scarto" />
    <table cellpadding="0" cellspacing="0" width="100%" height="100%" border="0">
        <tr valign="top" height="50%">
            <td width="316" valign="top" style="width: 300px">
                <cf1:iframewebcontrol id="iFrame_opzioni" runat="server" marginwidth="0" marginheight="2"
                    iwidth="415" iheight="100%" frameborder="0" scrolling="no" navigateto="OpzioniScarto.aspx"
                    width="415px"></cf1:iframewebcontrol>
            </td>
            <td width="1">
                <img border="0" src="../../images/spaziatore.gif" width="1">
            </td>
            <td width="1">
                <img border="0" src="../../images/spaziatore.gif" width="1">
            </td>
            <td width="*">
                <cf1:iframewebcontrol id="iFrame_dettagli" runat="server" marginwidth="10" marginheight="2"
                    iwidth="100%" iheight="100%" frameborder="0" scrolling="no" navigateto="whitepage.htm"></cf1:iframewebcontrol>
               
            </td>
        </tr>
    </table>
   </form>
</body>
</html>