<%@ Page Language="c#" CodeBehind="RagioneTrasm.aspx.cs" AutoEventWireup="false"
    Inherits="Amministrazione.Gestione_RagioniTrasm.RagioneTrasm" %>

<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">

    <script language="JavaScript" src="../CSS/caricamenujs.js"></script>

    <script language="JavaScript">
			
			var cambiapass;
			var hlp;
			
			function apriPopup() {
				hlp = window.open('../help.aspx?from=RT','','width=450,height=500,scrollbars=YES');
			}
			function DivHeight()
			{
				if (DivDGList.scrollHeight< 300) 
					DivDGList.style.height=DivDGList.scrollHeight;
				else
					DivDGList.style.height=300;
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
    </script>

    <script language="javascript">
		
			function ShowValidationMessage(message,warning)
			{
				if (message!=null && message!='')
				{
					if (warning)
					{
						if (window.confirm(message + "\n\nContinuare?"))
						{
							Form1.submit();
						}
					}
					else
					{
						alert(message);
					}
				}
			}
			
			function apriGestioneMsgNotifica(idRagione, idAmm)
			{
		
				var myUrl = "messaggioNotifica.aspx?idRagione="+idRagione+"&idAmm="+idAmm;				
				window.showModalDialog(myUrl,"","dialogWidth:785px;dialogHeight:660px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 								
			}
    </script>

</head>
<body bottommargin="0" leftmargin="0" topmargin="0" onload="javascript: DivHeight();"
    rightmargin="0" onunload="ClosePopUp()">
    <form id="Form1" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Ragioni di Trasmissione" />
    <!-- Gestione del menu a tendina -->
    <uc2:MenuTendina ID="MenuTendina" runat="server"></uc2:MenuTendina>
    <table height="100%" cellspacing="1" cellpadding="0" width="100%" border="0">
        <tr>
            <td>
                <!-- TESTATA CON MENU' -->
                <uc1:Testata ID="Testata" runat="server"></uc1:Testata>
            </td>
        </tr>
        <tr>
            <td class="testo_grigio_scuro" bgcolor="#c0c0c0" height="20">
                <asp:Label ID="lbl_position" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <!-- TITOLO PAGINA -->
            <td class="titolo" align="center" bgcolor="#e0e0e0" height="20">
                Ragioni di trasmissione
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <table cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <td align="center" height="25">
                        </td>
                    </tr>
                    <tr>
                        <td class="pulsanti" align="center" width="700">
                            <table width="100%">
                                <tr>
                                    <td align="left">
                                        <asp:Label ID="lbl_tit" CssClass="titolo" runat="server">Lista ragioni di trasmissione</asp:Label>
                                    </td>
                                    <td align="right">
                                        <asp:Button ID="btn_nuova" runat="server" CssClass="testo_btn" Text="Nuova ragione">
                                        </asp:Button>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td height="4">
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <div id="DivDGList" style="overflow: auto; height: 950px">
                                <asp:DataGrid ID="dg_Ragioni" runat="server" BorderWidth="1px" CellPadding="1" BorderColor="Gray"
                                    AutoGenerateColumns="False" Width="950px">
                                    <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                                    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                                    <ItemStyle CssClass="bg_grigioN"></ItemStyle>
                                    <HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg" BackColor="#810D06">
                                    </HeaderStyle>
                                    <Columns>
                                        <asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="Codice" HeaderText="Codice">
                                            <HeaderStyle HorizontalAlign="Center" Width="15%"></HeaderStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Visibile" HeaderText="Visibile">
                                            <HeaderStyle HorizontalAlign="Center" Width="5%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Diritto" HeaderText="Diritti">
                                            <HeaderStyle Width="15%"></HeaderStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Destinatario" HeaderText="Destinatario">
                                            <HeaderStyle Width="15px"></HeaderStyle>
                                        </asp:BoundColumn>
                                        <asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione">
                                            <HeaderStyle Width="40%"></HeaderStyle>
                                        </asp:BoundColumn>
                                        <asp:ButtonColumn Text="&lt;img src=../Images/lentePreview.gif border=0 alt='Dettaglio'&gt;"
                                            CommandName="Select">
                                            <HeaderStyle Width="5%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:ButtonColumn>
                                        <asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 alt='Elimina'&gt;"
                                            CommandName="Delete">
                                            <HeaderStyle Width="5%"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                        </asp:ButtonColumn>
                                        <asp:BoundColumn Visible="False" DataField="Sistema" HeaderText="Sistema"></asp:BoundColumn>
                                    </Columns>
                                </asp:DataGrid></div>
                        </td>
                    </tr>
                    <tr>
                        <td height="10">
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            <asp:Panel ID="pnl_info" runat="server" Visible="False">
                                <table class="contenitore" width="100%">
                                    <tr>
                                        <td class="titolo_pnl" colspan="6">
                                            <table cellspacing="0" cellpadding="0" width="100%">
                                                <tr>
                                                    <td class="titolo">
                                                        Dettagli ragione di trasmissione
                                                    </td>
                                                    <td align="right">
                                                        <asp:ImageButton ID="btn_chiudiPnlInfo" runat="server" ToolTip="Chiudi" ImageUrl="../Images/cancella.gif">
                                                        </asp:ImageButton>&nbsp;
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_grigio_scuro" style="height: 16px" align="right">
                                            Codice *
                                        </td>
                                        <td style="height: 16px">
                                            <asp:TextBox ID="txt_codice" CssClass="testo" runat="server" Width="130px" MaxLength="32"></asp:TextBox>
                                            <asp:Label ID="lbl_cod" runat="server" CssClass="testo"></asp:Label>
                                        </td>
                                        <td class="testo_grigio_scuro" style="height: 16px" align="right">
                                            Notifica trasm.ne
                                        </td>
                                        <td style="height: 16px">
                                            <asp:DropDownList ID="ddl_notifica" runat="server" CssClass="testo">
                                                <asp:ListItem Value="Nessuna" Selected="True">nessuna notifica</asp:ListItem>
                                                <asp:ListItem Value="Mail">tramite email (link a scheda documento/fascicolo e, se documento, link a immagine)</asp:ListItem>
                                                <asp:ListItem Value="MailAllegati">tramite email (allegati, link a scheda documento/fascicolo e, se documento, link a immagine)</asp:ListItem>
                                                <asp:ListItem Value="MailSoloAllegati">tramite email (solo allegati)</asp:ListItem>                                     
                                                <asp:ListItem Value="NonNotificareMai">non notificare mai</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:ImageButton ID="imgModMsgNotifica" runat="server" ImageUrl="../../images/proto/RispProtocollo/freccina_dx.gif"
                                                ToolTip="Gestione messaggi di notifica" />
                                        </td>
                                        <td class="testo_grigio_scuro" style="height: 16px" align="right">
                                            Visibilità
                                        </td>
                                        <td style="height: 16px">
                                            <asp:DropDownList ID="ddl_visibilita" runat="server" CssClass="testo">
                                                <asp:ListItem Value="False" Selected="True">No</asp:ListItem>
                                                <asp:ListItem Value="True">Si</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_grigio_scuro" style="height: 19px" align="right">
                                            Tipo
                                        </td>
                                        <td style="height: 19px">
                                            <asp:DropDownList ID="ddl_tipo" runat="server" CssClass="testo" Width="130px"
                                             OnSelectedIndexChanged="DdlTipo_SelectedIndexChanged" AutoPostBack="true">
                                                <asp:ListItem Value="SenzaWorkflow" Selected="True">Senza Workflow</asp:ListItem>
                                                <asp:ListItem Value="ConWorkflow">Con Workflow</asp:ListItem>
                                                <asp:ListItem Value="ConInterop">Interoperabilt&#224;</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="testo_grigio_scuro" style="height: 19px" align="right">
                                            Diritti
                                        </td>
                                        <td style="height: 19px">
                                            <asp:DropDownList ID="ddl_diritti" runat="server" CssClass="testo" Width="180px">
                                                <asp:ListItem Value="Nessuno" Selected="True">Nessuno</asp:ListItem>
                                                <asp:ListItem Value="LetturaScrittura">Lettura e scrittura</asp:ListItem>
                                                <asp:ListItem Value="Lettura">Solo Lettura</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <asp:Panel ID="pnlPrevedeRisposta" runat="server">
                                            <td class="testo_grigio_scuro" style="height: 19px" align="right">
                                                Prevede risposta
                                            </td>
                                            <td style="height: 19px">
                                                <asp:DropDownList ID="ddl_tipoRisposta" runat="server" CssClass="testo">
                                                    <asp:ListItem Value="False" Selected="True">No</asp:ListItem>
                                                    <asp:ListItem Value="True">Si</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>                                        
                                        </asp:Panel>
                                        <asp:Panel ID="pnlTipoAttivita" runat="server" Visible="false">
                                            <td class="testo_grigio_scuro" style="height: 19px" align="right">
                                                Tipo attività
                                            </td>
                                            <td style="height: 19px">
                                                <asp:DropDownList ID="DdlTipoAttivita" runat="server" CssClass="testo"
                                                 OnSelectedIndexChanged="DdlTipoAttivita_SelectedIndexChanged" AutoPostBack="true">
                                                    <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                                    <asp:ListItem Value="Si">Si</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>       
                                        </asp:Panel>
                                    </tr>
                                    <asp:Panel ID="pnlClassificazioneObbligatoria" runat="server" Visible="false">
                                        <tr>
                                            <td class="testo_grigio_scuro" style="height: 25px" align="right">
                                                Classificazione Obbligatoria
                                            </td>
                                            <td style="height: 19px">
                                                <asp:DropDownList ID="ddl_classificazioneObbligatoria" runat="server" CssClass="testo">
                                                    <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                                    <asp:ListItem Value="Si">Si</asp:ListItem>
                                                </asp:DropDownList>
                                            </td> 
                                        </tr>
                                    </asp:Panel>
                                    <tr>
                                        <td class="testo_grigio_scuro" style="height: 20px" align="right">
                                            Destinatario
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="ddl_destinatario" runat="server" CssClass="testo" Width="130px">
                                                <asp:ListItem Value="Tutti" Selected="True">Tutti</asp:ListItem>
                                                <asp:ListItem Value="SoloSuperiori">Solo superiori</asp:ListItem>
                                                <asp:ListItem Value="SoloSottposti">Solo sottoposti</asp:ListItem>
                                                <asp:ListItem Value="Parilivello">Pari livello</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <asp:Panel ID="pnlColonnaVuota" runat="server" Visible="false">
                                            <td></td>
                                            <td></td>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlEUnaRisposta" runat="server">
                                            <td class="testo_grigio_scuro" style="height: 20px" align="right">
                                                E' una risposta
                                            </td>
                                            <td style="height: 20px">
                                                <asp:DropDownList ID="ddl_risposta" runat="server" CssClass="testo">
                                                    <asp:ListItem Value="True" Selected="True">Si</asp:ListItem>
                                                    <asp:ListItem Value="False">No</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlTipologia" runat="server" Visible="false">
                                            <td class="testo_grigio_scuro" style="height: 20px" align="right">
                                                Tipologia
                                            </td>
                                            <td style="height: 20px">
                                                <asp:DropDownList ID="ddlTipologia" runat="server" CssClass="testo">
                                                    <asp:ListItem Value="" Selected="True"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td class="testo_grigio_scuro" style="height: 20px" align="right">
                                                Contributo obb.
                                            </td>
                                            <td style="height: 20px">
                                                <asp:DropDownList ID="ddl_contributo_obbligatorio" runat="server" CssClass="testo">
                                                    <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                                    <asp:ListItem Value="Si">Si</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>       
                                        </asp:Panel>
                                        <td class="testo_grigio_scuro" style="height: 20px" align="right">
                                            Eredita
                                        </td>
                                        <td style="height: 20px">
                                            <asp:DropDownList ID="ddl_eredita" runat="server" CssClass="testo">
                                                <asp:ListItem Value="False" Selected="True">No</asp:ListItem>
                                                <asp:ListItem Value="True">Si</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <asp:Panel ID="pnl_cessione" runat="server" Visible="false" Width="950px">
                                        <tr>
                                            <td class="testo_grigio_scuro" align="right">
                                                Cede diritti
                                            </td>
                                            <td class="testo" colspan="5">
                                                <asp:DropDownList ID="ddl_cedeDiritti" runat="server" CssClass="testo" AutoPostBack="true"
                                                    OnSelectedIndexChanged="ddl_cedeDiritti_SelectedIndexChanged">
                                                    <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                                    <asp:ListItem Value="Si">Si con scelta dell'utente</asp:ListItem>
                                                    <asp:ListItem Value="Sempre">Si sempre, l'utente non può scegliere</asp:ListItem>
                                                </asp:DropDownList>
                                                <%--<label class="testo_grigio_scuro">Mantieni diritti di lettura</label>--%> 
                                                <label class="testo_grigio_scuro">Mantieni diritti</label>
                                                <asp:DropDownList ID="ddl_mantieniLettura" runat="server" CssClass="testo">
                                                    <asp:ListItem Value="Nessuno" Selected="True">Nessuno</asp:ListItem>
                                                    <asp:ListItem Value="Lettura">Lettura</asp:ListItem>
                                                    <asp:ListItem Value="Scrittura">Scrittura</asp:ListItem>
                                                    <%--<asp:ListItem Value="False" Selected="True">No</asp:ListItem>--%>
                                                    <%--<asp:ListItem Value="True">Si</asp:ListItem>--%>
                                                </asp:DropDownList>
                                            </td>
                                            
                                        </tr>
                                    </asp:Panel>
                                    <tr valign="top">
                                        <td class="testo_grigio_scuro" align="right">
                                            Descrizione *
                                        </td>
                                        <td colspan="5">
                                            <asp:TextBox ID="txt_note" CssClass="testo" runat="server" Width="615" MaxLength="250"
                                                TextMode="MultiLine" Height="50"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left" colspan="6">
                                            <table cellspacing="0" cellpadding="0" width="100%">
                                                <tr>
                                                    <td align="left" width="70%">
                                                    </td>
                                                    <td align="right">
                                                        <asp:Button ID="btn_aggiungi" runat="server" CssClass="testo_btn" Text="Aggiungi">
                                                        </asp:Button>&nbsp;
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
