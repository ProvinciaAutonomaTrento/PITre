<%@ Page language="c#" Codebehind="progress.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.progress" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
    <title>progress</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
    <LINK href="CSS/docspa_30.css" type="text/css" rel="stylesheet">
    <script language=javascript>
		
		function setTitle(text)
		{	
			document.all["title"].innerText = text;
		}
		function setSubTitle(text)
		{
			document.all["subTitle"].innerText = text;
		}
		
		</script>
</HEAD>
    <body MS_POSITIONING="GridLayout">
	    <form id="Form1" method="post" runat="server">
	        <table id=container height=117 width=400 cellSpacing=0  cellPadding=0 border=0 bordercolor="#810d06" bgcolor="#d9d9d9">
                <tr>
                    <td vAlign=top align=right>
		                <SPAN style="CURSOR: hand" onclick="javascript:if (typeof(window.opener) == 'undefined') window.top.hideProgress(); else window.opener.hideProgress();" >
		                    <asp:ImageButton id=ImageButton1 runat="server" ImageUrl="images/cancella.gif"></asp:ImageButton>&nbsp;
                        </SPAN>
                    </td>
                </tr>
                <tr>
                    <td height=90 align=center>
                        <span id=title style="FONT-SIZE: 16pt; BORDER-BOTTOM: #810d06 1px solid; FONT-FAMILY: Verdana; font-weigth: bold">
                            &nbsp;Operazione in corso...attendere!
                        </span>
                        <br >
                        <span id=subTitle style="FONT-SIZE: 10pt; FONT-FAMILY: Verdana; font-weigth: bold">
                            &nbsp;Creazione del file, e modifica....
                        </span> 
                    </td>
                </tr>
                
            </table>
        </form>
    </body>
</HTML>

