<%@ Page language="c#" Codebehind="gestioneAllegato.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.gestioneAllegato" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<base target="_self" />
        <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script type="text/javascript">
		    function CloseMask(retValue)
		    {
		        window.returnValue = retValue;
		        window.close();
		    }
		</script> 
<SCRIPT LANGUAGE="JavaScript"> 
<!--
//
// Use the following variable to specify the maximum 
// number of times the user can submit the form
//
var maxSubmits = 1
 
function validate(frm)
{
    var totalSubmits = eval(GetCookie('TotalSubmissions'))

    if (totalSubmits == null)
        totalSubmits = 0

    if (totalSubmits >= maxSubmits)
    {
        return false
    }
    else
    {
        totalSubmits = totalSubmits + 1
        BakeIt(totalSubmits, "TotalSubmissions")
        return true
    }
}
 
function ResetCounter()
{
    BakeIt(0, "TotalSubmissions")
}
 
function BakeIt(cookieData, cookieName) 
{
    // Use this variable to set the number of days after which the cookie will expire
    var days = 999;

    // Calculate the expiration date
    var expires = new Date ();
    expires.setTime(expires.getTime() + days * (24 * 60 * 60 * 1000)); 

    // Set the cookie
    SetCookie(cookieName, cookieData, expires);
}
 
function SetCookie(cookieName, cookieData, expireDate) 
{
    document.cookie = cookieName + "=" + escape(cookieData) + "; expires=" + expireDate.toGMTString();
}    
 
function GetCookie(name) 
{
    var arg = name + "=";
    var alen = arg.length;
    var clen = document.cookie.length;
    var i = 0;
    while (i < clen) {
        var j = i + alen;
        if (document.cookie.substring(i, j) == arg)
            return GetCookieVal (j);
        i = document.cookie.indexOf(" ", i) + 1;
        if (i == 0) break; 
    }
    return null;
}
 
function GetCookieVal (offset) 
{
    var endstr = document.cookie.indexOf (";", offset);
    if (endstr == -1)
        endstr = document.cookie.length;
    return unescape(document.cookie.substring(offset, endstr));
}
//-->
</SCRIPT>
 
   </HEAD>
	<body leftMargin="2" topMargin="2" onload="ResetCounter()">
		<form id="gestioneAllegato" method="post" runat="server" onsubmit="return validate(gestioneAllegato);">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Gestione allegato" />
		    <TABLE align="center" id="Table1" class="info" border="0" style="WIDTH: 404px; height: 100%">
			    <TR>
				    <td class="item_editbox">
					    <P class="boxform_item">
					        <asp:Label id="LabelTitolo" runat="server" CssClass="menu_grigio_popup" Width="157px"></asp:Label>
                        </P>
				    </td>
			    </TR>
			    <tr>
				    <td align = "center">
				        <asp:Label id="LabelMessage" runat="server" CssClass="testo_grigio">
				        Il documento risulta bloccato. 
				        Per effettuare l'operazione richiesta è necessario prima rilasciare il documento.
				        </asp:Label>    
				    </td>
			    </tr>
			    <TR>
				    <TD><IMG height="1" src="../images/proto/spaziatore.gif" width="6" border="0">
					    <asp:label id="LabelCodice" runat="server" CssClass="titolo_scheda" Width="93px">Codice&nbsp;</asp:label>
					    <asp:textbox id="TextCodice" runat="server" CssClass="testo_grigio" Width="265px" ReadOnly="True" Height="22px"></asp:textbox>&nbsp;
				    </TD>
			    </TR>
			    <tr>
				    <td></td>
			    </tr>
			    <TR>
				    <TD><IMG height="1" src="../images/proto/spaziatore.gif" width="6" border="0">
					    <asp:label id="LabelDescrizione" runat="server" CssClass="titolo_scheda" Width="93px">Descrizione&nbsp;*&nbsp;</asp:label>
					    <asp:textbox id="TextDescrizione" runat="server" CssClass="testo_grigio" Width="265px"></asp:textbox>&nbsp;
				    </TD>
			    </TR>
                 <tr>
		<td colspan="2" align="right" class="testo_grigio">
			caratteri disponibili:&nbsp;<input type="text" id="clTesto" runat="server" name="clTesto"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
	</tr>
			    <tr>
				    <td></td>
			    </tr>
			    <TR>
				    <TD><IMG height="1" src="../images/proto/spaziatore.gif" width="6" border="0">
					    <asp:label id="LabelNumPag" runat="server" CssClass="titolo_scheda" Width="93px">Numero pagine&nbsp;</asp:label>
					    <asp:textbox id="TextNumPag" runat="server" CssClass="testo_grigio" Width="265px"></asp:textbox>&nbsp;
				    </TD>
			    </TR>
			    <tr>
				    <td></td>
			    </tr>
			    <TR>
				    <TD align="center" height="30">
					    <asp:button id="btn_ok" runat="server" CssClass="pulsante"  Width="39px" Height="19px" Text="OK"></asp:button>&nbsp;
					    <asp:button class="pulsante" id="btn_chiudi" Width="58px" Height="20px" runat="server" Text="Chiudi" />
				    </TD>
			    </TR>
		    </TABLE>
		</form>
	</body>
</HTML>
