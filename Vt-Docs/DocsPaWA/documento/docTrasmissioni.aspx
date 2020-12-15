<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc1" TagName="TestataDocumento" Src="TestataDocumento.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register TagPrefix="uc2" TagName="AclDocumento" Src="AclDocumento.ascx" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="uc3" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %> 
<%@ Page language="c#" Codebehind="docTrasmissioni.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.p_Trasmissioni" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
	    <link id="idLinkCss" href="" type="text/css" rel="stylesheet" />
		<script language="javascript" src="../LIBRERIE/ProgressMsg.js"></script>
		<script language="javascript">
		function butt_trasm_onClick()
		{   
		    if(document.getElementById("abilitaModaleVis").value == "true")
			{
			    AvvisoVisibilita();
			}
		    var btnTrasmetti = document.getElementById('butt_Trasm');
            var btnTrasmettiDisabled = document.getElementById('btn_trasmettiDisabled');

            if (btnTrasmetti != null && btnTrasmettiDisabled != null)
            {
                btnTrasmetti.style.display='none';
                btnTrasmettiDisabled.style.display='';
            }
		}	
		function pageLoad()
		{
		    _id = "docTrasmissioni";
			InitOperationMessage(true);
			_textOperation = new Array(1);
			_textOperation["butt_Trasm"] = new progressMsg(_id,"Operazione in corso...","Trasmissione documento");
		}
        function WaitDataGridCallback(eventTarget,eventArgument)
		{
			ShowWaitCursor();
		}
        function ShowWaitCursor()
		{
			window.document.body.style.cursor="wait";
        }
        
        function AvvisoVisibilita()
        {
            var newLeft=(screen.availWidth-500);
	        var newTop=(screen.availHeight-500);	
		    var newUrl;
		
	        newUrl="../popup/estensioneVisibilita.aspx";
		    
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
            } catch (e) { }
        }
        
		</script>
	</HEAD>
	<BODY onload="javascript:pageLoad()">
		<form id="p_Trasmissioni" method="post" runat="server">
		<asp:HiddenField ID="abilitaModaleVis" runat="server" />
        <asp:HiddenField ID="estendiVisibilita" runat="server" />
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Trasmissioni" />
			<INPUT id="flag_template" style="Z-INDEX: 101; LEFT: 8px; WIDTH: 40px; POSITION: absolute; TOP: 8px; HEIGHT: 22px"
				type="hidden" size="1" runat="server">
			<uc3:datagridpagingwait id="DataGridPagingWait1" runat="server" />
			<uc2:AclDocumento id="aclDocumento" runat="server"></uc2:AclDocumento>
			
			<table id="tbl_contenitore" height="100%" cellSpacing="0" cellPadding="0" width="398px" align="center"
				border="0">
				<tr vAlign="top">
					<TD height="5"><uc1:testatadocumento id="TestataDocumento" runat="server"></uc1:testatadocumento></TD>
				</tr>
				<tr vAlign="top">
					<td align="left">
						<table class="contenitore" height="99%" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td class="testo_grigio_scuro" vAlign="bottom" align="center" width="95%" height="5"><asp:radiobuttonlist id="rbl_tipoTrasm" CssClass="testo_grigio_scuro" RepeatColumns="2" Runat="server"
										OnSelectedIndexChanged="rbl_tipoTrasm_SelectedIndexChanged" AutoPostBack="True" DESIGNTIMEDRAGDROP="19">
										<asp:ListItem Value="E" Selected="True">Effettuate&#160;&#160;</asp:ListItem>
										<asp:ListItem Value="R">Ricevute</asp:ListItem>
									</asp:radiobuttonlist></td>
							</tr>
							<tr vAlign="top" height="1">
								<td width="95%" bgColor="#800000"><IMG height="1" src="../images/proto/spaziatore.gif" width="8" border="0"></td>
							</tr>
							<tr>
								<td align="center" height="5"></td>
							</tr>
							<tr vAlign="top">
								<td style="HEIGHT: 11px" align="center"><asp:label id="lbl_message" runat="server" CssClass="testo_msg_grigio" Height="9px" Width="100%"></asp:label></td>
							</tr>
							<tr vAlign="top">
								<TD align="center">
								    <asp:datagrid id="grdEffettuate" SkinID="datagrid" runat="server" Width="100%" BorderColor="Gray" AutoGenerateColumns="False"
										BorderWidth="1px" PageSize="8" CellPadding="1" AllowPaging="True" AllowCustomPaging="True" OnItemCreated="DataGrid1_ItemCreated">
										<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
										<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
										<ItemStyle CssClass="bg_grigioN"></ItemStyle>
										<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
										<Columns>
											<asp:TemplateColumn HeaderText=" Data invio">
												<HeaderStyle Width="10%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataInvio") %>'>
													</asp:Label>
												</ItemTemplate>
												<EditItemTemplate>
													<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataInvio") %>'>
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
													<asp:Label id=lblIdTrasm runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdTrasmissione") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Dett.">
												<HeaderStyle Width="10%"></HeaderStyle>
												<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
												<ItemTemplate>
													<cc1:ImageButton id="imgDettaglio" runat="server" AlternateText="Dettaglio" ImageUrl="../images/proto/fulmine.gif" CommandName="Select" OnClientClick="showWait();"></cc1:ImageButton>
												</ItemTemplate>
											</asp:TemplateColumn>
										</Columns>
										<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio" Mode="NumericPages"></PagerStyle>
									</asp:datagrid>
									<asp:datagrid id="grdRicevute" runat="server" SkinID="datagrid" Width="100%" BorderColor="Gray" AutoGenerateColumns="False"
										BorderWidth="1px" PageSize="8" CellPadding="1" AllowPaging="True" AllowCustomPaging="True" OnItemCreated="Datagrid2_ItemCreated">
										<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
										<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
										<ItemStyle CssClass="bg_grigioN"></ItemStyle>
										<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
										<Columns>
											<asp:TemplateColumn HeaderText="Data invio">
												<HeaderStyle Width="10%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id="Label4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataInvio") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Mittente trasm. utente">
												<HeaderStyle Width="30%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id="Label5" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Utente") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>										
											<asp:TemplateColumn HeaderText="Mittente trasm. ruolo">
												<HeaderStyle Width="30%"></HeaderStyle>
												<ItemTemplate>
													<asp:Label id="Label7" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Ruolo") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Ragione">
												<ItemTemplate>
													<asp:Label id="Label8" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.RagioneTrasmissione") %>'>
													</asp:Label>
												</ItemTemplate>
                                                <HeaderStyle Width="15%" />
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Data Scad">
												<ItemTemplate>
													<asp:Label id="Label9" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.DataScadenza") %>'>
													</asp:Label>
												</ItemTemplate>
                                                <HeaderStyle Width="10%" />
											</asp:TemplateColumn>
											<asp:TemplateColumn Visible="False" HeaderText="Chiave">
												<ItemTemplate>
													<asp:Label id="lblIdTrasm" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.IdTrasmissione") %>'>
													</asp:Label>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Dett.">
												<HeaderStyle Width="5%"></HeaderStyle>
												<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
												<ItemTemplate>
													<cc1:ImageButton id="Imagebutton2" runat="server" BorderWidth="1px" BorderColor="DimGray" AlternateText="Dettaglio" OnClientClick="showWait();"
														ImageUrl="../images/proto/dettaglio.gif" CommandName="Select"></cc1:ImageButton>
												</ItemTemplate>
											</asp:TemplateColumn>
										</Columns>
										<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
											Mode="NumericPages"></PagerStyle>
									</asp:datagrid></TD>
							</tr>
							<asp:panel id="pnl_trasm_rapida" Runat="server" Visible="True">
								<TR vAlign="bottom">
									<TD vAlign="bottom">
										<TABLE class="info_grigio" id="tbl_trasm_rapida" cellSpacing="0" cellPadding="0" width="95%"
											align="center" border="0">
											<TR>
												<TD class="titolo_scheda" vAlign="middle">&nbsp;&nbsp;Trasmissione Rapida</TD>
											</TR>
											<TR>
												<TD height="25">&nbsp;
													<asp:DropDownList id="ddl_tmpl" tabIndex="420" runat="server" AutoPostBack="True" CssClass="testo_grigio"
														Width="344px"></asp:DropDownList>
													<cc1:imagebutton id="btn_salva_trasm" Runat="server" Width="18" Height="16" Visible="False" DisabledUrl="../images/proto/salva.gif"
														AlternateText="Salva trasmissione da template" ImageUrl="../images/proto/salva.gif"></cc1:imagebutton></TD>
											</TR>
										</TABLE>
									</TD>
								</TR>
							</asp:panel>
							<tr>
								<td>
								    <cc2:messagebox id="msg_Trasmetti" runat="server"></cc2:messagebox>
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
								<TD style="height: 44px"><cc1:imagebutton id="btn_NuovaTrasm" Runat="server" DisabledUrl="../images/bottoniera/btn_nuova_nonattivo.gif"
										AlternateText="Nuova trasmissione" Thema="btn_" SkinId="nuova_Attivo" Tipologia="DO_TRA_NUOVA"></cc1:imagebutton></TD>
								<TD style="height: 44px"><cc1:imagebutton id="btn_ModifTrasm" Runat="server" DisabledUrl="../images/bottoniera/btn_modifica_nonattivo.gif"
										AlternateText="Modifica trasmissione" Thema="btn_" SkinId="modifica_attivo"
										Tipologia="DO_TRA_MODIFICA"></cc1:imagebutton></TD>
								<TD style="height: 44px"><cc1:imagebutton id="butt_Trasm" Runat="server" DisabledUrl="../images/bottoniera/btn_trasmetti_nonattivo.gif"
										AlternateText="Trasmetti" Thema="btn_" SkinId="trasmetti_attivo" Tipologia="DO_TRA_TRASMETTI"
										ImageAlign="Top"></cc1:imagebutton>
									<cc1:imagebutton id="btn_trasmettiDisabled" ondblclick="return false;" Runat="server" Tipologia="DO_TRA_TRASMETTI"
										ImageUrl="../images/bottoniera/btn_trasmetti_nonattivo.gif" DisabledUrl="../images/bottoniera/btn_trasmetti_nonattivo.gif"
										Enabled="false"></cc1:imagebutton></TD>
								<TD style="height: 44px"><cc1:imagebutton id="btn_stampa" Runat="server" DisabledUrl="../images/bottoniera/btn_stampa_nonAttivo.gif"
										AlternateText="Stampa report trasmissioni" Thema="btn_" SkinId="stampa_Attivo"
										Tipologia="DO_TRA_STAMPA"></cc1:imagebutton></TD>
								
							</TR>
						</TABLE>
						<!--FINE BOTTONIERA --></td>
				</TR>
			</table>
		</form>
	</BODY>
</HTML>
