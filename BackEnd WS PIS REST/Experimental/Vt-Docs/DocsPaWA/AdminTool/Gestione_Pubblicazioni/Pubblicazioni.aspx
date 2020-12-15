<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pubblicazioni.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Pubblicazioni.Pubblicazioni" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc3" TagName="ChnDetail" Src="ChannelDetail.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
	<HEAD id="HEAD1" runat = "server">
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

		    function Init() {

		    }
		    function apriPopup() {
		        hlp = window.open('../help.aspx?from=HP', '', 'width=450,height=500,scrollbars=YES');
		    }
		    function cambiaPwd() {
		        cambiapass = window.open('../CambiaPwd.aspx', '', 'width=410,height=300,scrollbars=NO');
		    }
		    function ClosePopUp() {
		        if (typeof (cambiapass) != 'undefined') {
		            if (!cambiapass.closed)
		                cambiapass.close();
		        }
		        if (typeof (hlp) != 'undefined') {
		            if (!hlp.closed)
		                hlp.close();
		        }
		    }

		    function confirmGridItemDelete() {
		        return confirm("Si desidera eliminare l'elemento selezionato?");
		    }

		</SCRIPT>
		<script language="javascript" id="btn_test_Click" event="onclick()" for="btn_test">
			window.document.body.style.cursor='wait';
		</script>

		<script language="javascript" id="btn_elimina_click" event="onclick()" for="btn_elimina">
			window.document.body.style.cursor='wait';
		</script>
		<script language="javascript" id="btn_salva_click" event="onclick()" for="btn_salva">
			window.document.body.style.cursor='wait';
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" 
		rightMargin="0" onunload="Init();ClosePopUp()">
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Pubblicazioni" />
			<input id="hd_idAmm" type="hidden" name="hd_idAmm" runat="server"> 
			
            <!-- Gestione del menu a tendina -->
            <uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
			
            <table style="height: 100%; width: 100%" cellSpacing="1" cellPadding="0" width="100%" border="0">
				<tr>
					<td>
						<!-- TESTATA CON MENU' -->
                        <uc1:testata id="Testata" runat="server"></uc1:testata>
                    </td>
				</tr>
				<tr>
					<!-- STRISCIA SOTTO IL MENU -->
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20">
                        <asp:label id="lbl_position" runat="server"></asp:label>
                    </td>
				</tr>
				<tr>
					<!-- STRISCIA DEL TITOLO DELLA PAGINA -->
					<td class="titolo" align="center" bgColor="#e0e0e0" height="20">Pubblicazioni
                    </td>
				</tr>
                <tr>
                    <td class="titolo" align="center" height="20">
                    </td>
                </tr>	
				<tr>
                    <td>
                        <table style="height: 100%; width: 90%" cellSpacing="1" cellPadding="0" border="0" align="center">
                            <tr>
        			            <td class="pulsanti" align="right">
                                    <asp:button ID="btnNewChannel" runat="server" CssClass="testo_btn_p" OnClick="btnNewChannel_Click" Text="Nuovo canale"></asp:button>
                                    <asp:button ID="btnRefreshChannels" runat="server" CssClass="testo_btn_p" OnClick="btnRefreshChannels_Click" Text="Aggiorna canali"></asp:button>
                                </td>
                            </tr>   
                            <tr>
                                <td>
                                    <asp:Panel ID="pnlChannels" runat="server" Height="300px" ScrollBars="Auto">
                                        <asp:datagrid id="grdChannels" runat="server" 
                                            AllowPaging="true" PageSize="3"
                                            AutoGenerateColumns="False" BorderColor="Gray" 
                                            CellPadding="1" BorderWidth="1px" Width="100%"
                                            OnPageIndexChanged="grdChannels_PageIndexChanged"
                                            OnItemCommand="grdChannels_ItemCommand"
                                            OnItemCreated="OnDataGridItemCreated">
							                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							                <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							                <ItemStyle CssClass="bg_grigioN" Height="20px"></ItemStyle>
							                <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>
                                            <PagerStyle HorizontalAlign="Center" Mode="NumericPages" CssClass="testo" />
                                            <Columns>
                                                <asp:TemplateColumn HeaderText="Id" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblId" runat="server" Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).Id%>"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>

                                                <asp:TemplateColumn HeaderText="Nome Sottoscrittore  / Url Servizio" 
                                                        HeaderStyle-Width="55%" 
                                                        HeaderStyle-HorizontalAlign="Center"
                                                        ItemStyle-HorizontalAlign="Center" 
                                                        ItemStyle-VerticalAlign="Middle">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSubscriber" runat="server" Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).ChannelName%>" Font-Bold="true"></asp:Label>
                                                        <br />
                                                        --------------------
                                                        <br />
                                                        <asp:Label ID="lblSubscriberUrl" runat="server" Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).SubscriberServiceUrl%>"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>

                                                <asp:TemplateColumn HeaderText="Stato servizio" 
                                                    HeaderStyle-Width="22%" 
                                                    HeaderStyle-HorizontalAlign="Center"
                                                    ItemStyle-VerticalAlign="Middle" 
                                                    ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnStart" runat="server" CausesValidation="false" CommandName="Start" Text="Avvia" CssClass="testo_btn_p" 
                                                            Visible="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).State == Publisher.Proxy.ChannelStateEnum.Stopped%>" />
                                                        <asp:Button ID="btnStop" runat="server" CausesValidation="false" CommandName="Stop" Text="Ferma" CssClass="testo_btn_p"
                                                            Visible="<%#(((Publisher.Proxy.ChannelRefInfo) Container.DataItem).State == Publisher.Proxy.ChannelStateEnum.Started || ((Publisher.Proxy.ChannelRefInfo) Container.DataItem).State == Publisher.Proxy.ChannelStateEnum.UnexpectedStopped)%>"/>
                                                        <br />
                                                        --------------------
                                                        <br />
                                                        <asp:Label ID="lblState" runat="server" Text="<%#this.GetServiceState((Publisher.Proxy.ChannelRefInfo) Container.DataItem)%>"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
    
                                                <asp:TemplateColumn HeaderStyle-Width="3%" 
                                                    HeaderStyle-HorizontalAlign="Center"
                                                    ItemStyle-VerticalAlign="Middle" 
                                                    ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Image ID="imgServiceState" runat="server" Width="90%" Height="90%" ImageUrl="<%#this.GetServiceStateImageName((Publisher.Proxy.ChannelRefInfo) Container.DataItem)%>" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>


                                                <asp:TemplateColumn HeaderText="Pubbl. effettuate tot/par" 
                                                    HeaderStyle-HorizontalAlign="Center"
                                                    HeaderStyle-Width="5%" 
                                                    ItemStyle-VerticalAlign="Middle"
                                                    ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotalExecutionCount" runat="server" Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).TotalExecutionCount%>"></asp:Label>
                                                        /
                                                        <asp:Label ID="lblExecutionCount" runat="server" Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).ExecutionCount%>"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>

                                                <asp:TemplateColumn HeaderText="Oggetti pubbl. tot/par" 
                                                    HeaderStyle-HorizontalAlign="Center"
                                                    HeaderStyle-Width="5%" 
                                                    ItemStyle-VerticalAlign="Middle"
                                                    ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotalPublishedObjects" runat="server" Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).TotalPublishedObjects%>"></asp:Label>
                                                        /
                                                        <asp:Label ID="lblPublishedObjects" runat="server" Text="<%#((Publisher.Proxy.ChannelRefInfo) Container.DataItem).PublishedObjects%>"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>

                                                <asp:TemplateColumn Visible="true" ItemStyle-Width="15%" 
                                                    HeaderStyle-HorizontalAlign="Center"
                                                    ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnEditConfigurations" runat="server" CommandName="EditConfigurations" CssClass="testo_btn_p" Width="90%" Text="Configurazioni" />
                                                        <br />
                                                        <br />
                                                        <asp:Button ID="btnSelectChannel" runat="server" CommandName="Select" 
                                                            CssClass="testo_btn_p" Text="<%#this.GetTextEventsButton((Publisher.Proxy.ChannelRefInfo) Container.DataItem)%>" Width="90%" />
                                                        <br />
                                                        <br />
                                                        <asp:Button ID="btnDelete" runat="server" OnClientClick="return confirmGridItemDelete()" CommandName="Delete" Text="Rimuovi" CssClass="testo_btn_p" Width="90%" />
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>                                                                                                            

                                            </Columns>
			                            </asp:datagrid>          
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr>
                                <td>

                                            <table id="tblChannelEvents" runat="server" style="height: 100%; width: 100%" cellSpacing="1" cellPadding="0" border="0" align="center">
                                                <tr>
        			                                <td class="pulsanti" align="right">
                                                        <asp:button ID="btnNewEvent" runat="server" CssClass="testo_btn_p" OnClick="btnNewEvent_Click" Text="Nuovo evento"></asp:button>
                                                        <asp:button ID="btnCloseEvents" runat="server" CausesValidation="false" CssClass="testo_btn_p" OnClick="btnCloseEvents_Click" Text="Chiudi"></asp:button>
                                                    </td>
                                                </tr>  
                                                <tr>
        			                                <td>
                                                        <asp:datagrid id="grdEvents" runat="server" 
                                                            AllowPaging="True" 
                                                            PagerStyle-Mode="NumericPages"
                                                            PagerStyle-HorizontalAlign="Center"
                                                            PageSize="8"
                                                            AutoGenerateColumns="False" BorderColor="Gray" 
                                                            CellPadding="1" BorderWidth="1px" Width="100%"
                                                            OnItemCommand="grdEvents_ItemCommand"
                                                            OnPageIndexChanged="grdEvents_PageIndexChanged"
                                                            OnItemCreated="OnDataGridItemCreated">
							                                <PagerStyle HorizontalAlign="Center" Mode="NumericPages" CssClass="testo" />
							                                <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
							                                <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
							                                <ItemStyle CssClass="bg_grigioN"></ItemStyle>
							                                <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>

                                                            <Columns>
                                                                <asp:TemplateColumn HeaderText="Id" Visible="false">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblId" runat="server" Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).Id%>"></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>

                                                                <asp:TemplateColumn HeaderText="IdInstance" Visible="false">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblIdInstance" runat="server" Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).IdChannel%>"></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateColumn>

                                                                <asp:TemplateColumn HeaderText="Tipo oggetto" HeaderStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblObjectType" runat="server" 
                                                                        Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).ObjectType%>"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:DropDownList ID="cboObjectTypes" runat="server" CssClass="testo" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="cboObjectTypes_SelectedIndexChanged" />
                                                                        <asp:RequiredFieldValidator ID="requiredCboObjectTypes" runat="server" ControlToValidate="cboObjectTypes" ErrorMessage="*" Font-Bold="true" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                    </EditItemTemplate>
                                                                    <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
                                                                    <ItemStyle VerticalAlign="Top" />
                                                                </asp:TemplateColumn>

                                                                <asp:TemplateColumn HeaderText="Nome profilo" HeaderStyle-Width="33%">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblObjectTemplateName" runat="server" Text="<%#((Publisher.Proxy.EventInfo) Container.DataItem).ObjectTemplateName%>"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:DropDownList ID="cboObjectTemplates" runat="server" CssClass="testo" Width="100%" ></asp:DropDownList>
                                                                    </EditItemTemplate>
                                                                    <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center"  />
                                                                    <ItemStyle VerticalAlign="Top" />
                                                                </asp:TemplateColumn>

                                                                <asp:TemplateColumn HeaderText="Evento" HeaderStyle-Width="40%">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblEventName" runat="server" 
                                                                        Text="<%#this.GetEventDescription((Publisher.Proxy.EventInfo) Container.DataItem)%>"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:DropDownList ID="cboLogEvents" runat="server" CssClass="testo" Width="100%" DataTextField="Descrizione" DataValueField="Codice"></asp:DropDownList>
                                                                        <asp:RequiredFieldValidator ID="requiredCboLogEvents" runat="server" ControlToValidate="cboLogEvents" ErrorMessage="*" Font-Bold="true" ForeColor="Red"></asp:RequiredFieldValidator>
                                                                    </EditItemTemplate>
                                                                    <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
                                                                    <ItemStyle VerticalAlign="Top" />
                                                                </asp:TemplateColumn>

                                                                <asp:TemplateColumn HeaderText="Pubblica file" HeaderStyle-Width="5%" Visible="true">
                                                                    <ItemTemplate>
                                                                        <asp:Image id="imgFileTypeUsedConservazione" runat="server" ImageUrl="../Images/spunta_1.gif"
                                                                        Visible="<%#((Publisher.Proxy.EventInfo) Container.DataItem).LoadFileIfDocumentType%>"
                                                                        ToolTip="Sarà pubblicato il file associato alla versione corrente del documento"/>
                                                                    </ItemTemplate>
                                                                    <EditItemTemplate>
                                                                        <asp:CheckBox ID="chkLoadFile" runat="server"
                                                                        Checked="<%#((Publisher.Proxy.EventInfo) Container.DataItem).LoadFileIfDocumentType%>"></asp:CheckBox>
                                                                    </EditItemTemplate>
                                                                    <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" />
                                                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" VerticalAlign="Top" />
                                                                </asp:TemplateColumn>

                                                                <asp:TemplateColumn HeaderStyle-Width="13%" >
                                                                    <ItemTemplate>
                                                                        <asp:Button ID="btnEdit" runat="server" CommandName="Edit" CausesValidation="false" CssClass="testo_btn_p" Width="40%" Text="Modifica" />
                                                                        <asp:Button ID="btnCancel" runat="server" CommandName="Cancel" CausesValidation="false" CssClass="testo_btn_p" Width="40%" Text="Annulla" />
                                                                        <asp:Button ID="btnUpdate" runat="server" CommandName="Update" CssClass="testo_btn_p" Width="40%" Text="Salva" />
                                                                        <asp:Button ID="btnDelete" runat="server" OnClientClick="return confirmGridItemDelete()" CausesValidation="false" CommandName="Delete" CssClass="testo_btn_p" Width="40%" Text="Rimuovi" />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center"/>
                                                                    <ItemStyle Font-Bold="False" Font-Italic="False" Font-Overline="False" 
                                                                        Font-Strikeout="False" Font-Underline="False" HorizontalAlign="Center" VerticalAlign="Top"/>
                                                                </asp:TemplateColumn>

                                                            </Columns>
                                                        </asp:datagrid>
                                                    </td>
                                                </tr>
                                            </table>


                                            <table id="tblChannelConfigurations" runat="server" style="height: 100%; width: 100%" cellSpacing="1" cellPadding="0" border="0" align="center">
                                                <tr>
        			                                <td class="pulsanti" align="right">
                                                        <asp:button ID="btnSaveChannel" runat="server" CssClass="testo_btn_p" OnClick="btnSaveChannel_Click" Text="Salva"></asp:button>
                                                        <asp:button ID="btnCloseChannelConfigurations" runat="server" CausesValidation="false" CssClass="testo_btn_p" OnClick="btnCloseChannelConfigurations_Click" Text="Chiudi"></asp:button>
                                                    </td>
                                                </tr>   
                                                <tr>
                                                    <td>
                                                        <uc3:ChnDetail id="channelDetail" runat="server"></uc3:ChnDetail>
                                                    </td>
                                                </tr>
                                            </table>

                                </td>
                            </tr>
                        </table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>