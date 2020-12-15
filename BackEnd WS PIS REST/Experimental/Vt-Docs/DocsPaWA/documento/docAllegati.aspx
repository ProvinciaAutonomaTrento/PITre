<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc1" TagName="TestataDocumento" Src="TestataDocumento.ascx" %>
<%@ Page language="c#" Codebehind="docAllegati.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.documento.docAllegati" %>
<%@ Register TagPrefix="uc2" TagName="AclDocumento" Src="AclDocumento.ascx" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
        <link id="styleLink" runat="server" href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>		
		<script language="javascript">
		    
		    function showMaskRimuoviAllegato(docNumber,versionId)
            {
                var url = "../popup/rimuoviAllegato.aspx?docNumber=" + docNumber + "&versionId=" + versionId;
	            return window.showModalDialog(url,
	                        null, 'dialogWidth:360px;dialogHeight:150px;status:no;resizable:no;scroll:no;help:no;close:no'); 
            }

		    function showMaskAllegato(versionId)
		    {
                var url = "../popup/gestioneAllegato.aspx";
                if (versionId != "") 
                    url = url + "?versionId=" + versionId;
	            
	            versionId = window.showModalDialog(url, null, 'dialogWidth:408px;dialogHeight:180px;status:no;resizable:no;scroll:no;help:no;close:no');
	            document.getElementById("txtSavedVersionId").value = versionId;
	            return (versionId != null);
		    }
		    
            function onClickNuovoAllegato()
		    {
		        var oldcursor = window.document.body.style.cursor;
		        window.document.body.style.cursor='wait';
		        
	            var ret = showMaskAllegato("");
	            
	            if (!ret)
	                window.document.body.style.cursor=oldcursor;
	                
                return ret;
		    }

		    function onClickModificaAllegato()
		    {
		        var oldcursor = window.document.body.style.cursor;
		        window.document.body.style.cursor='wait';
		    
		        var ret = showMaskAllegato("<%=GetSelectedVersionId()%>");
		        
                if (!ret)
	                window.document.body.style.cursor=oldcursor;
	                
                return ret;
		    }

		    function onClickScambiaAllegato() {

		        var True = true, False = false;
		        if ("<%=DoScambiaWithNewComment()%>"=="1") {
		            var oldcursor = window.document.body.style.cursor;
		            window.document.body.style.cursor = 'wait';

		            var ret = showMaskAllegato("<%=GetSelectedVersionId()%>");

		            if (!ret)
		                window.document.body.style.cursor = oldcursor;

		            return ret;
		        } 
		    }
		    
		    function onClickRimuoviAllegato()
		    {
                var oldcursor = window.document.body.style.cursor;
		        window.document.body.style.cursor='wait';
		        
		        var ret = showMaskRimuoviAllegato("<%=GetSelectedDocNumber()%>", "<%=GetSelectedVersionId()%>");
		        
                if (!ret)
	                window.document.body.style.cursor=oldcursor;
	                
                return ret;		        
		    }
		    
		</script>
	</HEAD>
	<body>
		<form id="docAllegati" method="post" runat="server">
		<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Allegati" />
		<uc2:AclDocumento id="aclDocumento" runat="server"></uc2:AclDocumento>
		    <input id = "txtSavedVersionId" runat = "server" type = "hidden">
			<table id="tbl_contenitore" height="100%" cellSpacing="0" cellPadding="0" width="398px" align="center"
				border="0">
				<tr vAlign="top">
					<TD height="5"><uc1:testatadocumento id="TestataDocumento" runat="server"></uc1:testatadocumento></TD>
				</tr>

                <tr>
                    <td style="height:2px;"></td>
                </tr>

                <!-- filtri allegati -->
                 <tr>
					<td style="border:1px solid gray;" height="5">
                        <div style="padding-left:3px;">
                            <asp:Label ID="lblFilter" runat="server" Text="Tipologia allegati" CssClass="titolo_scheda" />
                            <asp:RadioButtonList RepeatDirection="Horizontal" ID="rblFilter" runat="server" CssClass="testo_grigio" AutoPostBack="true"  TextAlign="Right">
                                <asp:ListItem Text="Tutti" Value="all"></asp:ListItem>
                                <asp:ListItem Text="PEC" Value="pec"></asp:ListItem>
                                <asp:ListItem Text="Sist. esterni" Value="esterni"></asp:ListItem>
                                <asp:ListItem Text="Utente" Value="user" Selected="True"></asp:ListItem>
                            </asp:RadioButtonList>

                        </div>
                    </td>
                 </tr>

                <tr>
                    <td style="height:2px;"></td>
                </tr>

				 <tr vAlign="top">
					<td align="left">
						<!-- TABELLA ALLEGATI -->
						<table class="contenitore" height="99%" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0">
								</td>
							</tr>
							<tr>
								<td align="center"><asp:label id="lbl_message" runat="server" CssClass="testo_msg_grigio" Height="20px" Width="343px"
										Visible="False">Allegati non trovati</asp:label></td>
							</tr>
							<tr>
								<td align="center" valign="top">
								    <asp:datagrid id="grdAllegati" SkinID="datagrid" runat="server" Width="100%" 
                                        AutoGenerateColumns="False" BorderWidth="1px"
										AllowPaging="True" CellPadding="1" BorderColor="Gray" 
										OnItemCreated="grdAllegati_ItemCreated" 
										OnItemCommand="grdAllegati_ItemCommand"
										OnSelectedIndexChanged = "grdAllegati_SelectedIndexChanged" >
										<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
										<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
										<ItemStyle CssClass="bg_grigioN"></ItemStyle>
										<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
										<Columns>
										    <asp:BoundColumn DataField = "docNumber" Visible = "false" />
											<asp:BoundColumn DataField = "versionId" Visible = "false" />
										    
                                            <asp:TemplateColumn HeaderText="Codice<br>&nbsp;-------<br>&nbsp;Data creaz.">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCodiceAllegato" runat="server"
                                                        Text="<%# this.GetCodiceAllegato((DocsPAWA.DocsPaWR.Allegato) Container.DataItem) %>"
                                                        ToolTip="<%# this.GetAutoreOrDelegato((DocsPAWA.DocsPaWR.Allegato) Container.DataItem) %>"></asp:Label>
                                                        <br />
                                                        &nbsp;-------
                                                        <br />
                                                    <asp:Label ID="lblDataInserimentoAllegato" runat="server"
                                                     Text="<%# this.GetDataInserimentoAllegatoAsString((DocsPAWA.DocsPaWR.Allegato) Container.DataItem) %>"
                                                     ToolTip="<%# this.GetAutoreOrDelegato((DocsPAWA.DocsPaWR.Allegato) Container.DataItem) %>"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
                                            <asp:BoundColumn DataField="descrizione" HeaderText="Descrizione">
                                                <HeaderStyle Width="58%" />
                                            </asp:BoundColumn>
                                            <asp:BoundColumn DataField="numeroPagine" HeaderText="Num pag." >
											    <HeaderStyle Width="10%" />
                                            </asp:BoundColumn>
                                            <asp:TemplateColumn>
                                                <ItemTemplate>
                                                    <asp:ImageButton runat="server" ImageUrl="~/images/Indent.ico"  ID="imgGoToDocument"
                                                        ToolTip = "Vai al dettaglio del documento sorgente dell'inoltro"
                                                        Visible='<%# GetIsVisibileSourceButton((DocsPAWA.DocsPaWR.Allegato) Container.DataItem) %>' 
                                                        CommandName="GoToSource"vvcx
                                                        CommandArgument='<%# ((DocsPAWA.DocsPaWR.Allegato) Container.DataItem).ForwardingSource %>'  />
                                                </ItemTemplate>
                                            </asp:TemplateColumn>
											<asp:TemplateColumn ItemStyle-HorizontalAlign = "Center" >
												<ItemTemplate>
                                                    <cc1:ImageButton id="btnNavigateDocumento" runat="server" BorderWidth="0px" 
                                                    AlternateText="Vai al documento" ImageUrl = '<%# GetNavigateIconPath((DocsPAWA.DocsPaWR.Allegato) Container.DataItem) %>' CommandName="NavigateDocument"></cc1:ImageButton>
												</ItemTemplate>
											    <HeaderStyle Width="5%" />
                                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
											</asp:TemplateColumn>		
											<asp:TemplateColumn>
												<HeaderStyle Width="5%"></HeaderStyle>
												<ItemTemplate>
													<cc1:ImageButton id="imgSelect" runat="server" BorderWidth="0px" AlternateText="Seleziona" ImageUrl="../images/proto/ico_riga.gif" CommandName="Select"></cc1:ImageButton>
												</ItemTemplate>
											</asp:TemplateColumn>
										</Columns>
										<PagerStyle VerticalAlign="Middle" HorizontalAlign="Center" BackColor="#c2c2c2" CssClass="menu_pager_grigio"
											Mode="NumericPages"></PagerStyle>
									</asp:datagrid>
								</td>
							</tr>
                            <tr>
                            <td style="height:100%;"></td>
                            </tr>
						</table>
					</td>
				</tr>
				<tr height="10%">
					<td>
						<!-- BOTTONIERA -->
						<TABLE id="tbl_bottoniera" cellSpacing="0" cellPadding="0" align="center" border="0">
							<TR>
								<TD vAlign="top" height="5"><IMG height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></TD>
							</TR>
							<TR>
								<TD><cc1:imagebutton id="btn_aggAlleg" Thema="btn_" SkinId="nuovo_Attivo" AlternateText="Nuovo allegato"
										Runat="server" DisabledUrl="../images/bottoniera/btn_nuovo_nonattivo.gif" Tipologia="DO_ALL_AGGIUNGI" OnClientClick = "return onClickNuovoAllegato();"></cc1:imagebutton></TD>
								<TD><cc1:imagebutton id="btn_modifAlleg" Thema="btn_" SkinId="modifica_attivo" AlternateText="Modifica allegato"
										Runat="server" DisabledUrl="../images/bottoniera/btn_modifica_nonattivo.gif" Tipologia="DO_ALL_MODIFICA" OnClientClick = "return onClickModificaAllegato();"></cc1:imagebutton></TD>
								<TD><cc1:imagebutton id="btn_aggiungiAreaLav" Thema="btn_" SkinId="area_attivo" AlternateText="Aggiungi ad Area di lavoro"
										Runat="server" DisabledUrl="../images/bottoniera/btn_area_nonattivo.gif" Tipologia="DO_ADD_ADL"></cc1:imagebutton></TD>
								<TD><cc1:imagebutton id="btn_sostituisciDocPrinc" Thema="btn_" SkinId="scambia_attivo" Enabled = "true"
										AlternateText="Scambia con documento principale" Runat="server" DisabledUrl="../images/bottoniera/btn_scambia_nonattivo.gif"
										Tipologia="DO_ALL_SOSTITUISCI" OnClientClick = "return onClickScambiaAllegato();" ></cc1:imagebutton></TD>
								<TD><cc1:imagebutton id="btn_rimuoviAlleg" Thema="btn_" SkinId="rimuovi_Attivo" AlternateText="Rimuovi allegato"
										Runat="server" DisabledUrl="../images/bottoniera/btn_rimuovi_nonattivo.gif" Tipologia="DO_ALL_RIMUOVI" OnClientClick="return onClickRimuoviAllegato();"></cc1:imagebutton></TD>
							</TR>
						</TABLE>
                    </td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
