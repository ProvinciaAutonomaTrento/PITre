<%@ Page language="c#" Codebehind="fascTrasmissioni.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.fascicolo.fasc_Trasmissioni" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register TagPrefix="uc1" TagName="AclFascicolo" Src="AclFascicolo.ascx" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script language="javascript" src="../LIBRERIE/ProgressMsg.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript">
		    var w = window.screen.width;
		    var h = window.screen.height;
		    var new_w = (w - 100) / 2;
		    var new_h = (h - 400) / 2;

		function pageLoad()
		{
		    _id = "fascTrasmissioni";
			InitOperationMessage(true);
			_textOperation = new Array(1);
			_textOperation["butt_Trasm"] = new progressMsg(_id,"Operazione in corso...","Trasmissione fascicolo");
		}
		
		function AvvisoVisibilita()
        {
            var newLeft=(screen.availWidth-500);
	        var newTop=(screen.availHeight-500);	
		    var newUrl;
		
	        newUrl="../popup/estensioneVisibilita.aspx";
		    
            if(document.getElementById("abilitaModaleVis").value == "true")
            {
                retValue=window.showModalDialog(newUrl,"","dialogWidth:306px;dialogHeight:188px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:yes;help:no;");
                
                if(retValue == 'NO')
                {
                    document.getElementById("estendiVisibilita").value = "true";
                }
                else
                {
                    document.getElementById("estendiVisibilita").value = "false";
                }
            }
        }

        function openChooseTransDoc() {
            window.showModalDialog('../popup/searchInControlledProject.aspx', '', 'dialogWidth:700px;dialogHeight:600px;status:no;resizable:no;scroll:auto;center:yes;help:no;close:no;top:' + new_h + ';left:' + new_w);

        }

        function showWait() {

            try {
                if (top.principale.iFrame_dx.document.getElementById('please_wait') != undefined) {
                    var divWait = top.principale.iFrame_dx.document.getElementById('please_wait');
                    var height, width;
                    divWait.style.display = '';
                    height = top.principale.iFrame_dx.document.body.offsetHeight / 2 - 90 / 2;
                    width = top.principale.iFrame_dx.document.body.offsetWidth / 2 - 350 / 2;
                    divWait.style.top = height;
                    divWait.style.left = width;
                }
            } catch (e) {

            }

        }
		</script>
	</HEAD>
	<BODY MS_POSITIONING="GridLayout" onload="javascript:pageLoad()">
		<form id="fasc_Trasmissioni" method="post" runat="server">
		<asp:HiddenField ID="abilitaModaleVis" runat="server" />
        <asp:HiddenField ID="estendiVisibilita" runat="server" />
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Trasmissioni Fascicolo" />
			<INPUT id="flag_template" style="Z-INDEX: 101; LEFT: 8px; WIDTH: 40px; POSITION: absolute; TOP: 8px; HEIGHT: 22px" type="hidden" size="1" runat="server" NAME="flag_template">
			<uc1:AclFascicolo id="aclFascicolo" runat="server"></uc1:AclFascicolo>
		    <!--1-->
			<table id="tbl_contenitore" cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
				<tr vAlign="top">
					<td vAlign="top" width="100%" height="5%">
						<table class = contenitore height="17" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr vAlign="top" align="center" height="17">
								<td class="testo_grigio_scuro" vAlign="top" width="100%" height="5"><asp:radiobuttonlist id="rbl_tipoTrasm" AutoPostBack="True" Runat="server" RepeatColumns="2" cssClass="testo_grigio_scuro ">
										<asp:ListItem Value="E" Selected="True">Effettuate&nbsp;&nbsp;</asp:ListItem>
										<asp:ListItem Value="R">Ricevute</asp:ListItem>
									</asp:radiobuttonlist></td>
							</tr>
							<tr vAlign="top" height="1">
								<td width="95%" bgColor="#800000"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></td>
							</tr>
							<tr>
								<td height="5"></td>
							</tr>
							<tr>
								<td align="center"><asp:label id="lbl_message" runat="server" Visible="False" Width="343px" Height="20px" CssClass="testo_msg_grigio">Trasmissioni non trovate</asp:label></td>
							</tr>
							<tr vAlign="top">
							<td width="100%" >
						<TABLE height="350" cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
							
							<tr vAlign="top">
								<TD width="100%" align=center><asp:datagrid id="DataGrid1" SkinID="datagrid" runat="server" AllowPaging="True" AllowCustomPaging="true" CellPadding="1" PageSize="8" BorderWidth="1px" AutoGenerateColumns="False" BorderColor="Gray" OnItemCreated="DataGrid1_ItemCreated">
										<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
										<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
										<ItemStyle CssClass="bg_grigioN"></ItemStyle>
										<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
										<Columns>
											<asp:TemplateColumn HeaderText=" Data invio">
												<HeaderStyle Width="10%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Data") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Data") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Mittente trasm. utente">
												<HeaderStyle Width="40%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=Label2 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Utente") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id=TextBox2 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Utente") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Mittente trasm. ruolo">
												<HeaderStyle Width="40%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=Label3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ruolo") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id=TextBox3 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ruolo") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn Visible="False" HeaderText="Chiave">
												<ItemTemplate>
													<asp:Label id=Label6 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id=TextBox6 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Dettagli">
												<HeaderStyle Width="10%"></HeaderStyle>
												<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
												<ItemTemplate>
													<cc1:ImageButton id="ImageButton1" runat="server" BorderWidth="1px" BorderColor="DimGray" AlternateText="Dettaglio" ImageUrl="../images/proto/dettaglio.gif" CommandName="Select" OnClientClick="showWait();"></cc1:ImageButton>
												</ItemTemplate>
											</asp:TemplateColumn>
										</Columns>
										<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#c2c2c2" CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
									</asp:datagrid><asp:datagrid id="Datagrid2" runat="server" SkinID="datagrid" Width="410px" AllowPaging="True" AllowCustomPaging="true" CellPadding="1" PageSize="8" BorderWidth="1px" AutoGenerateColumns="False" BorderColor="Gray" OnItemCreated="Datagrid2_ItemCreated">
										<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
										<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
										<ItemStyle CssClass="bg_grigioN"></ItemStyle>
										<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
										<Columns>
											<asp:TemplateColumn HeaderText="Data invio">
												<HeaderStyle Width="10%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id="Label4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Data") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id="Textbox4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Data") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Mittente trasm. utente">
												<HeaderStyle Width="40%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id="Label5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Utente") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id="Textbox5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Utente") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Mittente trasm. ruolo">
												<HeaderStyle Width="40%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id="Label7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ruolo") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id="Textbox7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ruolo") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Ragione">
												<ItemTemplate>
													<asp:Label id="Label8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ragione") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id="Textbox8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ragione") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Data Scad">
												<ItemTemplate>
													<asp:Label id="Label9" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataScad") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id="Textbox9" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataScad") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn Visible="False" HeaderText="Chiave">
												<ItemTemplate>
													<asp:Label id="Label10" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id="Textbox10" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'>
													</asp:TextBox>
												</EditItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn>
												<HeaderStyle Width="10%"></HeaderStyle>
												<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
												<ItemTemplate>
													<cc1:ImageButton id="Imagebutton2" runat="server" BorderWidth="1px" BorderColor="DimGray" AlternateText="Dettaglio" ImageUrl="../images/proto/dettaglio.gif" CommandName="Select" OnClientClick="showWait();"></cc1:ImageButton>
												</ItemTemplate>
											</asp:TemplateColumn>
										</Columns>
										<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#c2c2c2" CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
									</asp:datagrid></TD>
							</tr>
						</table>
					</td>
				</tr>
				<asp:panel id="pnl_trasm_rapida" Runat="server" Visible="True">
				<TR vAlign="bottom">
					<TD vAlign="bottom">
					<TABLE class="info_grigio" id="tbl_trasm_rapida" style="WIDTH: 397px; HEIGHT: 49px" cellSpacing="0" cellPadding="0" width="397" align="center" border="0">
							<TR>
								<TD class="titolo_scheda" vAlign="middle">&nbsp;&nbsp;Trasmissione Rapida</TD>
							</TR>
							<TR>
								<TD height="25">&nbsp;
									<asp:dropdownlist id="ddl_tmpl" tabIndex="420" runat="server" AutoPostBack="True" Width="344px" CssClass="testo_grigio">
									</asp:dropdownlist><cc1:imagebutton id="btn_salva_trasm" Runat="server" Visible="False" Width="18" Height="16" DisabledUrl="../images/proto/salva.gif" AlternateText="Salva trasmissione da template" ImageUrl="../images/proto/salva.gif"></cc1:imagebutton></TD>
							</TR>
						</TABLE>
					</td>
				</tr>
				</asp:panel>
				
				<tr>
					<td >
					    <cc2:messagebox id="msg_TrasmettiFasc" runat="server"></cc2:messagebox>
					</td>
				</tr>
				<tr>
						<td height="5"></td>
				</tr>
			</table>
		</td>
	</tr>
	<TR>
		<td align="center" height="10%">
			<!-- BOTTONIERA -->
			<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" align="center" border="0">
				<TR>
					<TD vAlign="top" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
				</TR>
				<TR>
					<TD><cc1:imagebutton id="btn_NuovaTrasm" Runat="server" DisabledUrl="../images/bottoniera/btn_nuova_nonattivo.gif" AlternateText="Nuova trasmissione" Thema="btn_" SkinID="nuova_attivo" Tipologia="DO_TRA_NUOVA"></cc1:imagebutton>
					    <cc1:imagebutton id="btn_ModifTrasm" Runat="server" DisabledUrl="../images/bottoniera/btn_modifica_nonattivo.gif" AlternateText="Modifica trasmissione" Thema="btn_" SkinID="modifica_attivo" Tipologia="DO_TRA_MODIFICA"></cc1:imagebutton>
					    <cc1:imagebutton id="butt_Trasm" Runat="server" Thema="btn_" SkinID="trasmetti_attivo" AlternateText="Trasmetti" DisabledUrl="../images/bottoniera/btn_trasmetti_nonattivo.gif" Tipologia="DO_TRA_TRASMETTI" OnClientClick="AvvisoVisibilita();"></cc1:imagebutton>
                        <cc1:imagebutton id="btn_stampa" Runat="server" DisabledUrl="../images/bottoniera/btn_stampa_nonAttivo.gif"
										AlternateText="Stampa report trasmissioni" Thema="btn_" SkinId="stampa_Attivo"
										Tipologia="DO_TRA_STAMPA"></cc1:imagebutton>
                    </TD>
					<TD</TD>
					<TD></TD>
                    
				</TR>
			</TABLE>		
			<!--FINE BOTTONIERA -->		
		</td>
	</TR>
</table>
</form>
</BODY>
</HTML>
