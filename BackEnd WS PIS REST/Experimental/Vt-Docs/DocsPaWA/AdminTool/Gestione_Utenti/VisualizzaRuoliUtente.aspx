<%@ Page language="c#" Codebehind="VisualizzaRuoliUtente.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Utenti.VisualizzaRuoliUtente" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<script language="C#" runat="server">
			public bool getCheck(object abilita)
			{			
				string abil = abilita.ToString();
				if(abil == "1")
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		</script>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<base target="_self"/>
		<script language="javascript">
			function body_onLoad()
			{
				var newLeft=0;
				var newTop=0;
				window.moveTo(newLeft,newTop);								
			}
		</script>
		<script lang="javascript">
			function SelectChange(chkB)
			{			   			
			    for(i = 0; i < document.forms[0].elements.length; i++) 
				{
					elm = document.forms[0].elements[i]
					
					if(elm.name==chkB)
					{
					    elm.checked = true;
					}
					else
					{
					    elm.checked = false;
					}
				}
				//var ID = chkB.id; 
				//var count = document.getElementById("dg_ruoli").rows.length;
				//var i;
				//for (i = 2; i <= count; i++)
				//{
				//	var id = "dg_ruoli$ctl" + i + "_ruolo_pref";
				//  var elem= document.getElementById(id);
				//  elem.checked = false ;
				//}
				//document.getElementById(chkB).checked = true;								
			}
		</script>
	</HEAD>
	<body leftMargin="5" topMargin="5" onload="body_onLoad();">
		<form id="Form1" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Visualizzazione dei ruoli utente" />
			<input id="hd_idPeople" type="hidden" name="hd_idPeople" runat="server">
			<table cellSpacing="0" cellPadding="0" width="590" align="center" border="0">
				<tr>
					<td class="testo_grigio_scuro" align="right" height="10">|&nbsp;&nbsp;&nbsp;<A onclick="javascript: self.close();" href="#">Chiudi</A>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td align="left" height="48"><IMG height="48" src="../Images/logo.gif" width="218" border="0"></td>
				</tr>
				<tr>
					<td height="15"></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" height="20"><asp:label id="lbl_testa" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td height="10"></td>
				</tr>
				<tr>
					<td align="center">
						<TABLE cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
							<TR>
								<TD>
									<DIV style="OVERFLOW: auto; HEIGHT: 250px"><asp:datagrid id="dg_ruoli" runat="server" AutoGenerateColumns="False" CellPadding="1" BorderWidth="1px"
											BorderColor="Gray" Width="570">
											<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle CssClass="bg_grigioN"></ItemStyle>
											<HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
											<Columns>
												<asp:BoundColumn Visible="False" DataField="IDGRUPPO" ReadOnly="True" HeaderText="ID"></asp:BoundColumn>
												<asp:BoundColumn DataField="CODICE" ReadOnly="True" HeaderText="Codice">
													<ItemStyle Width="30%"></ItemStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="DESCRIZIONE" ReadOnly="True" HeaderText="Descrizione">
													<ItemStyle Width="70%"></ItemStyle>
												</asp:BoundColumn>
												<asp:TemplateColumn HeaderText="Pref">
													<HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
													<ItemTemplate>
														<asp:RadioButton id="RBpref" runat="server" GroupName="ruolo_pref" Checked='<%# getCheck(DataBinder.Eval(Container, "DataItem.PREFERITO")) %>'>
														</asp:RadioButton>
													</ItemTemplate>
												</asp:TemplateColumn>
											</Columns>
										</asp:datagrid></DIV>
								</TD>
							</TR>
						</TABLE>
					</td>
				</tr>
				<tr>
					<td align="center">
						<asp:Button ID="btn_salvaPref" Runat="server" Text="Salva ruolo preferito" CssClass="testo_btn"
							tabIndex="1"></asp:Button>
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<asp:Button ID="btn_chiudi" Runat="server" Text="Chiudi" CssClass="testo_btn" tabIndex="2"></asp:Button>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
