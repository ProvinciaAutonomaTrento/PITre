<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="notificheDocumento.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.notificheDocumento" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat = "server">
	    <title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<%Response.Expires=-1;%>
		<base target="_self">
	</HEAD>
	<body bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Ricevute Documento" />
			<TABLE class="contenitore" id="tbl_container" width="605" align="center" border="0" height="430">
				<TR>
					<td align="center" height="20"><label id="lbl_text" class="menu_1_rosso">Ricevute Documento</label>
					</td>
				</TR>

				<TR>
					<TD align="center" style="HEIGHT: 14px">
                        <asp:label id="lbl_message" runat="server" Height="8px" Visible="False" Width="329px" CssClass="testo_grigio_scuro"></asp:label>
                    </TD>
                </TR>
                <!-- panel filter -->
                <TR>
					<TD style="border:1px solid black;">
                        <div style="padding:3px;">
                            <asp:CheckBox ID="cbxSelDes" CssClass="testo_grigio" runat="server" OnCheckedChanged="cbxSelDes_CheckedChanged" Text="Seleziona/Deseleziona tutti" AutoPostBack="true" />
                        </div>
                        <div style="float:left;padding:3px;">
                            <div>
                                <asp:Label ID="lblFilterType" runat="server" Text="Tipo di ricevuta" CssClass="titolo_scheda" />
                                <asp:CheckBoxList CssClass="testo_grigio" RepeatDirection="Horizontal" CellSpacing="3" RepeatColumns="3" ID="cboFilterType" runat="server">
                                    <asp:ListItem Text="accettazione" Value="accettazione" />
                                    <asp:ListItem Text="avvenuta consegna" Value="avvenuta-consegna" />
                                    <asp:ListItem Text="mancata accettazione" Value="non-accettazione" />
                                    <asp:ListItem Text="mancata consegna" Value="errore-consegna" />
                                    <asp:ListItem Text="conferma ricezione" Value="ricevuta" />
                                    <asp:ListItem Text="annullamento protocollazione" Value="annulla" />
                                    <asp:ListItem Text="eccezione" Value="eccezione" />
                                    <asp:ListItem Text="con errori " Value="errore" />
                                </asp:CheckBoxList>
                            </div>
                            <%--Panel checkbox filter mail --%>
                            <div style="display:none;" id="divMail" runat="server">
                                <asp:Label ID="lblFilterMailDest" runat="server" Text="Mail destinatario" CssClass="titolo_scheda" />
                                <br />
                                <asp:CheckBoxList ID="cbxMail" runat="server" CssClass="testo_grigio" RepeatDirection="Horizontal" CellSpacing="3" RepeatColumns="2"></asp:CheckBoxList>
                            </div>
                            <%--Panel filter Code Corr for IS --%>
                            <div style="display:none;" id="divCodiceCorr" runat="server">
                                <asp:Label ID="lblFilterCodiceIS" runat="server" Text="Codice corrispondente" CssClass="titolo_scheda" />
                                <br />
                                <asp:TextBox id="txtFilterCodiceIS" runat="server" CssClass="testo_grigio" Width="270px" MaxLength="50" ></asp:TextBox>
                            </div>
                            
                            <div  style=" margin-top:12px;">
                                <asp:button id="btnApplyFilter"  runat="server" Visible="True" CssClass="PULSANTE" Text="Applica Filtri"></asp:button>
                            </div>
                        </div>

                        <!-- Panel export excel -->
                        <div style="float:right;">
                            <asp:ImageButton AlternateText="Esporta Ricevute" ID="btnExport" ImageUrl="~/images/proto/export_1.gif" OnClick="btnExport_Click" runat="server" />
                        </div>
                    </TD>
                </TR>

				<TR vAlign="top" align="center">
					<TD align="center"><DIV id="DivDataGrid" style="OVERFLOW: auto; height:330px;"><asp:datagrid id="DataGrid1" runat="server" SkinID="datagrid" AutoGenerateColumns="False" BorderWidth="1px" AllowPaging="True"
								CellPadding="1" BorderColor="Gray" OnPageIndexChanged="Datagrid1_SelectedPageIndexChanged" PageSize="10">
								<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
								<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
								<ItemStyle CssClass="bg_grigioN"></ItemStyle>
								<HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
								<Columns>
									<asp:TemplateColumn HeaderText="Data notifica" Visible="False">
										<HeaderStyle Width="10px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
											<asp:Label id=Label1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>' Font-Bold="True" ForeColor="Red">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox id=TextBox1 runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.data") %>'>
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Tipo">
										<HeaderStyle Width="50px"></HeaderStyle>
										<ItemStyle HorizontalAlign="Center"></ItemStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>' ID="Label2" NAME="Label2">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.tipo") %>' ID="Textbox2" NAME="Textbox2">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Destinatario">
										<HeaderStyle Width="250px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrizione") %>' ID="Label3" NAME="Label3">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.descrizione") %>' ID="Textbox3" NAME="Textbox3">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="Dettagli">
										<HeaderStyle Width="250px"></HeaderStyle>
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dettaglio") %>' ID="Label4" NAME="Label4">
											</asp:Label>
										</ItemTemplate>
										<EditItemTemplate>
											<asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.dettaglio") %>' ID="Textbox4" NAME="Textbox4">
											</asp:TextBox>
										</EditItemTemplate>
									</asp:TemplateColumn>
									<asp:TemplateColumn Visible="False" HeaderText="chiave">
										<ItemTemplate>
											<asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.chiave") %>' ID="Label7" NAME="Label7">
											</asp:Label>
										</ItemTemplate>
									</asp:TemplateColumn>
								</Columns>
								<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
									Mode="NumericPages"></PagerStyle>
							</asp:datagrid></DIV>
					</TD>
				</TR>
				<TR height="35">
					<TD align="center" height="35">
                        <asp:button id="btn_chiudi" runat="server" Visible="True" CssClass="PULSANTE" Text="CHIUDI"></asp:button>
                    </TD>
				</TR>
			</TABLE>
                    <iframe runat="server" id="reportContent" style="width:0px; height:0px;" />
		</form>
	</body>
</HTML>
