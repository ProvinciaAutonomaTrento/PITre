<%@ Page language="c#" Codebehind="login.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.login" %>
<%@ Register src="UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uc1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<%Response.Expires=-1500;%>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<META HTTP-EQUIV="Pragma" CONTENT="no-cache">
        <META HTTP-EQUIV="Expires" CONTENT="-1">	
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">

function forceLogin(ipaddress) 
{
	var message;
	message  = 'L\'utente ha gia\' una connessione in corso';
	if (ipaddress != "")
		message=message+" con indirizzo IP "+ipaddress;
	message += '\n';
	message += 'Chiudere la connessione esistente e crearne una nuova?';

	//var result = window.confirm('L\'utente ha gia\' una connessione in corso. \nChiudere la connessione esistente e crearne una nuova?');
	var result = window.confirm(message);

	if(result) {
		var popup = window.open('login.aspx?forceLogin=True', "_self");
		
		if(popup != self) {
			window.opener = null;
			self.close();
		}
	}
}

function body_onLoad()
{
	var maxWidth=570;
	var maxHeight=360;

	window.resizeTo(maxWidth,maxHeight);
	
	var newLeft=(screen.availWidth-maxWidth)/2;
	var newTop=(screen.availHeight-maxHeight)/2;
	window.moveTo(newLeft,newTop);

}

//Inserito per integrazione IAM-GFD
function nascondiSitoAccessibile() {
    document.getElementById('sitoAcc').style.display = 'none';
}

		</script>
	</HEAD>
	<body bgColor="#ffffff" onload="body_onLoad()" MS_POSITIONING="GridLayout">
		<form id="Form1" name="loginF" method="post" runat="server">
		    <uc1:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Login" />
            <asp:MultiView ID="loginMultiView" runat="server" ActiveViewIndex="0">
                <asp:View ID="defaultView" runat="server">
			        <table height="300" cellSpacing="0" cellPadding="0" width="550" align="center" bgColor="#ffffff"
				border="0">
				<tr vAlign="top">
					<td vAlign="top">
						<table height="100%" cellSpacing="0" cellPadding="0" width="524" align="center" border="0">
							<tr vAlign="top" align="left" bgColor="#810d06">
								<td width="50%"><asp:image runat="server" ImageUrl="images/logo.gif" border="0" width="141" height="58" ID="img_logologin"></asp:image><asp:image Width="121" height="58" ImageUrl="../images/testata/320/login_logoente2.gif" id="img_logologinente1" runat="server" ImageAlign="AbsMiddle" Visible="true"></asp:image></td>
								<td vAlign="middle" align="right" width="50%"><asp:image Width="262" height="58" ImageUrl="../images/testata/320/login_logoente.gif" id="img_logologinente2" runat="server" ImageAlign="AbsMiddle" Visible="true"></asp:image></td>
							</tr>
							<tr>
								<td align="right" width="100%" bgColor="#9e9e9e" colSpan="2" style="height: 20px"><A id="sitoAcc" class="sitoAccessibile" title="Versione accessibile di DocsPA - Legge 9 gennaio 2004, n.4" href="javascript: var s=window.open('SitoAccessibile/login.aspx','_blank'); self.close();">Versione 
										accessibile</A>&nbsp;&nbsp;&nbsp;
								</td>
							</tr>
							<TR height="20">
								<TD colSpan="2">&nbsp;</TD>
							</TR>
							<asp:panel id="pnl_ddlAmm" Visible="False" Runat="server">
								<TR>
									<TD class="testo_grigio_scuro" vAlign="middle" align="right">Amministrazioni 
										Disponibili&nbsp;&nbsp;&nbsp;</TD>
									<TD vAlign="top" align="left">
										<asp:DropDownList id="ddl_Amministrazioni" runat="server" CssClass="testo_grigio"></asp:DropDownList></TD>
								</TR>
							</asp:panel>
							<tr>
								<td align="center" colSpan="2"><asp:label id="lbl_Amm" Visible="False" Runat="server" CssClass="testo_grigio_scuro"></asp:label></td>
							</tr>
							<TR height="5">
								<TD colSpan="2"><INPUT id="sel_amministrazioni" type="hidden" name="sel_amministrazioni" runat="server">
                                    <IMG height="3" src="images/spaziatore.gif" width="3" border="0">
								</TD>
							</TR>
							<asp:panel id="pnl_login" Visible="true" Runat="server">
							    <TR style="height: 10px">
								    <TD align="right">
									    <asp:label id="Label2" runat="server" CssClass="testo_grigio_scuro" Width="200">UserID</asp:label>&nbsp;&nbsp;&nbsp;</TD>
								    <TD>
									    <asp:textbox id="userid" runat="server" CssClass="testo_grigio"></asp:textbox></TD>
							    </TR>
							    <TR style="height: 5px">
								    <TD colSpan="2"></TD>
							    </TR>
							    <TR style="height: 10px">
								    <TD align="right">
									    <asp:label id="lblPassword" runat="server" CssClass="testo_grigio_scuro" Width="200">Password</asp:label>&nbsp;&nbsp;&nbsp;</TD>
								    <TD>
									    <asp:textbox id="password" runat="server" CssClass="testo_grigio" TextMode="Password"></asp:textbox>
								    </TD>
							    </TR>
							    <TR style="height: 10px">
								    <TD align="right">
									    <asp:label id="lblPasswordConfirm" runat="server" CssClass="testo_grigio_scuro" Width="200">Conferma password</asp:label>&nbsp;&nbsp;&nbsp;</TD>
								    <TD>
									    <asp:textbox id="passwordConfirm" runat="server" CssClass="testo_grigio" TextMode="Password"></asp:textbox>
								    </TD>
							    </TR>								
							    <TR style="height: 10px">
								    <TD colSpan="2"></TD>
							    </TR>
							    <TR>
								    <TD>&nbsp;</TD>
								    <TD align="left">
									    <asp:imagebutton id="btn_login" runat="server" ImageUrl="images/Butt_Accedi.jpg"></asp:imagebutton></TD>
							    </TR>
								<TR style="height: 10px">
									<TD colSpan="2"></TD>
								</TR>
								<TR>
									<TD width="100%" bgColor="#9e9e9e" colSpan="2" height="5px"></TD>
								</TR>
								</asp:panel>
								<TR>
								    <TD align="center" colSpan="2" height="30px">
                                        <asp:Panel ID="pnl_error" Runat="server" Visible="true" Wrap="true">
										<asp:Label ID="lbl_error" Runat="server" CssClass="testo_red" Font-Size="Medium" 
                                                Visible="False"></asp:Label>
                                        <asp:HiddenField ID="hflLoginResult" runat="server" Value ="" />
									</asp:Panel></TD>
								</TR>
							
						</table>
					</td>
				</tr>
				<tr>
					<td height="100%"></td>
				</tr>
			</table>
                </asp:View>
                <asp:View ID="CasView" runat="server">
                    <asp:panel ID="pnlCasAuthentication" runat="server" ScrollBars="Auto" HorizontalAlign="Center">
                        <asp:Label ID="lblCasAuthenticationMessage" runat="server" Font-Size="Medium" CssClass="testo_grigio_scuro"></asp:Label>
                    </asp:panel>
                </asp:View>
            </asp:MultiView>
		</form>
	</body>
</HTML>
