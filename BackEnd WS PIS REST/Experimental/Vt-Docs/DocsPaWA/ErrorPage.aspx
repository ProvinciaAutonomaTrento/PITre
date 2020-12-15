<%@ Page Language="c#" CodeBehind="ErrorPage.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.form" %>

<%@ Register Src="UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

   <!-- <script language="javascript" src="LIBRERIE/DocsPA_Func.js"></script>

   -->

    <script language="javascript">
        function Azione()
        {
			var wnd=window.document.URLUnencoded;
			var i=window.document.URLUnencoded.indexOf("popup");
			if(i!=-1)
				{
				
				var str=wnd.substring(i,i+5);
				if(str=="popup")
				{
					window.close();
				}
				else
				{
					window.history.back();
				}
			  }
			  else
			  {
			  
					window.history.back();
			   }
        }
        
      function waitComplete()
		{
			try
			{
				waitingManagerJS_waitingComplete();
			}
			catch(exc)
			{;}
		}
    </script>

    <link href="CSS/DocsPA.css" type="text/css" rel="stylesheet">
</head>
<body  ms_positioning="GridLayout">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Login" />
    <table cellpadding="0" cellspacing='0' width="100%" border="0">
        <tr valign="top">
            <td>
                <form id="form" method="post" runat="server">
                <table cellpadding="0" cellspacing='0' width="100%" align="center" border="0">
                    <tr valign="top" bgcolor="#810d06">
                        <td valign="top" align="middle" height="30">
                            <asp:Label ID="Label1" runat="server" Font-Size="Small" Font-Names="Verdana" Font-Bold="True"
                                ForeColor="White"> Avviso </asp:Label>
                        </td>
                    </tr>
                    <tr valign="top" bgcolor="#f2f2f2">
                        <td align="center" class="testo_grigio">
                            <asp:Label runat="server" ID="lbl_mgserrore" Height="20px" Width="100%" BorderWidth="3px"
                                BorderColor="#F2F2F2" BorderStyle="Solid" Font-Size="X-Small" Font-Names="verdana;arial"
                                ForeColor="#666666" Font-Bold="True">Non è stato possibile effettuare l'operazione richiesta. Riprovare più tardi.</asp:Label>
                        </td>
                    </tr>
                    <tr valign="top" bgcolor="#f2f2f2">
                        <td align="middle" class="testo_grigio" valign="center" height="30">
                            <!--	<INPUT class="pulsante" type="button" value="Back" onclick="Azione();">-->
                        </td>
                    </tr>
                    <tr>
                        <td width="110%" height="400" bgcolor="#f2f2f2">
                            &nbsp;
                        </td>
                    </tr>
                </table>
                </form>
            </td>
        </tr>
    </table>
</body>
</html>
