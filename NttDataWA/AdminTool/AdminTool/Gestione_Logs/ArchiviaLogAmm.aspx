<%@ Register TagPrefix="uc3" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuLog" Src="../UserControl/MenuLogAmm.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="ArchiviaLogAmm.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Logs.ArchiviaLogAmm" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
		<SCRIPT language="JavaScript">
			
			var cambiapass;
			var hlp;
			
			function apriPopup() {
				hlp = window.open('../help.aspx?from=GLAR','','width=450,height=500,scrollbars=YES');
			}				
			function cambiaPwd() {
				cambiapass = window.open('../CambiaPwd.aspx','','width=410,height=300,scrollbars=NO');
			}
			function ClosePopUp()
			{	
				if(typeof(cambiapass) != 'undefined')
				{
					if(!cambiapass.closed)
						cambiapass.close();
				}
				if(typeof(hlp) != 'undefined')
				{
					if(!hlp.closed)
						hlp.close();
				}				
			}	
			
			function verifyNumber()
		    {
		        var result = true;
		        var numero = document.getElementById('txt_num_rec').value;
		        if(isNaN(numero))
		        {
		            result = false;
		            alert('Inserire un valore numerico per il numero dei record massimo');
		        }
		        else
		        {
		            result = confirm('Si vuole procedere con la modifica del numero massimo dei record per l\'archiviazione nello storico?');
		        }
		        if(result == false)
		            document.getElementById('permesso').value = 'ko';
		    }	
		</SCRIPT>
		<script language="javascript" id="btn_archivia_click" event="onclick()" for="btn_archivia">
			window.document.body.style.cursor='wait';
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout" onunload="ClosePopUp()">
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Archivia log Amministrazione" />
		<!-- Gestione del menu a tendina --><uc3:menutendina id="MenuTendina" runat="server"></uc3:menutendina>
			<table height="100%" cellSpacing="1" cellPadding="0" width="100%" border="0">
				<tr>
					<td>
						<!-- TESTATA CON MENU' --><uc1:testata id="Testata" runat="server"></uc1:testata></td>
				</tr>
				<tr>
					<!-- STRISCIA SOTTO IL MENU -->
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20"><asp:label id="lbl_position" runat="server"></asp:label></td>
				</tr>
				<tr>
					<!-- TITOLO PAGINA -->
					<td class="titolo" align="center" width="100%" bgColor="#e0e0e0" height="20">Archivia 
						log Amministrazione</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
						<table height="100" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr height="100%" width="100%">
								<td width="120" height="100%"><uc2:menulog id="MenuLogAmm" runat="server"></uc2:menulog>
									<DIV id="DivSel" style="OVERFLOW: auto; HEIGHT: 388px"></DIV>
								</td>
								<td vAlign="top" align="center" width="100%" height="100%"><br>
									<table cellSpacing="0" cellPadding="0" align="center" border="0">
										<tr>
											<td class="pulsanti" align="center" width="840">
												<table width="100%" border="0" cellpadding="5" cellspacing="0">
													<tr>
														<td align="left" width="80%" class="testo_grigio_scuro" >
														    <asp:HiddenField ID="permesso" runat="server" Value="ok" />
														    <asp:CheckBox id="chk_avviso" Runat="server"></asp:CheckBox>Avvisa gli utenti se il numero dei record da archiviare 
																risulta essere maggiore di&nbsp;&nbsp;
																<asp:TextBox id="txt_num_rec" CssClass="testo_grigio_scuro" Runat="server" Width="40px"
																	MaxLength="6"></asp:TextBox>
															<asp:button id="modificaNum" runat="server" CssClass="testo_btn" Text="Modifica" ToolTip="Modifica Impostazione Numero Massimo dei Record per lo Storico" OnClientClick="verifyNumber()"></asp:button>	
									                    </td>
														<td align="center" width="20%"><asp:button id="btn_archivia" runat="server" CssClass="testo_btn" Text="Avvia archiviazione"></asp:button>&nbsp;</td>
													</tr>
													<tr>
														<td align="left" width="80%"><asp:Label CssClass="testo_grigio_scuro" ID="lbl_archivio" Runat="server"></asp:Label>
															
                                                        </td>
														<td align="center" width="20%"><asp:Label CssClass="testo_piccolo" ID="lbl_avviso" Runat="server"></asp:Label></td>
													</tr>
												</table>
											</td>
										</tr>
										<tr>
											<td><asp:Label ID="lbl_lista_log" CssClass="testo_grigio_scuro" Runat="server"></asp:Label></td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
						<!-- FINE CORPO CENTRALE --></td>
					</TD></tr>
			</table>
		</form>
	</body>
</HTML>
