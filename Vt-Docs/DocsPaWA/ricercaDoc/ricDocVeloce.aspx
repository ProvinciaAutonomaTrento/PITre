<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page Language="c#" Codebehind="ricDocVeloce.aspx.cs" AutoEventWireup="false"
    Inherits="DocsPAWA.ricercaDoc.ricDocVeloce" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title>ricDocVeloce</title>
    <meta content="True" name="vs_showGrid">
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">

    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

    <script language="javascript" id="enterKeySimulator_click" event="onclick()" for="enterKeySimulator">
			window.document.body.style.cursor='wait';			
			WndWait();
    </script>

    <script language="javascript" id="butt_ricerca_click" event="onclick()" for="butt_ricerca">
			
	        var fullTextAlertMessage=document.getElementById('fullTextAlertMessage');
		    if (fullTextAlertMessage.value!=null && fullTextAlertMessage.value!='')
			    alert(fullTextAlertMessage.value);
				
		    window.document.body.style.cursor='wait';			
		    WndWait();
			
    </script>

    <script type="text/javascript">
		
		
			// Impostazione del focus su un controllo
			function SetControlFocus(controlID)
			{	
				try
				{
					var control=document.getElementById(controlID);
					
					if (control!=null)
					{
						control.focus();
					}
				}
				catch (e)
				{
				
				}
			}
			
			function OnChangeSavedFilter()
			{
				var ctl=document.getElementById("ddl_Ric_Salvate");

				if (ctl!=null && ctl.value>0)
					top.principale.iFrame_dx.location='../waitingpage.htm';	
			}
    </script>

</head>
<body leftmargin="0" MS_POSITIONING="GridLayout">
    <form id="ricDocVeloce" method="post" runat="server">
        <input id="fullTextAlertMessage" type="hidden" runat="server">
        <table id="tbl_contenitore" height="100%" cellspacing="0" cellpadding="0" width="413"
            align="center" border="0">
            <tbody>
                <tr valign="top">
                    <td style="width: 422px" align="left">
                        <table class="contenitore" height="100%" cellspacing="0" cellpadding="0" width="100%"
                            border="0">
                </tr>
                <tbody>
                    <tr valign="top">
                        <td class="titolo_scheda">
                            &nbsp;
                            <asp:Panel ID="pnlSearchStorage" runat="server" Width="409px">
                                <table class="info_grigio" id="Table1" style="height: 48px" cellspacing="0" cellpadding="0"
                                    width="95%" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="19">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                            <asp:Label ID="lblSearch" runat="server" Text="Ricerche Salvate"></asp:Label> 
                                            </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <font size="1">
                                                <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></font>
                                            <asp:DropDownList ID="ddl_Ric_Salvate" runat="server" Width="344px" AutoPostBack="True"
                                                CssClass="testo_grigio">
                                            </asp:DropDownList></td>
                                        <td align="left">
                                            <asp:ImageButton ID="btn_Canc_Ric" Width="19px" ImageUrl="../images/proto/cancella.gif"
                                                Height="17px" runat="server" AlternateText="Rimuove la ricerca selezionata"></asp:ImageButton><font
                                                    size="1"></font></td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:ImageButton ID="enterKeySimulator" runat="server" ImageUrl="..\images\spacer.gif">
                            </asp:ImageButton><img height="1" src="../images/proto/spaziatore.gif" width="8"
                                border="0">
                            <br>
                            <asp:Panel ID="pnlSearchOptions" runat="server" Width="280px">
                                &nbsp;&nbsp;
                                <asp:Label ID="lblRicercaPer" runat="server" Width="72px" Height="4px">Ricerca per:</asp:Label>&nbsp;&nbsp;
                                <asp:RadioButton ID="optSearchOggetto" runat="server" AutoPostBack="True" GroupName="TIPO_RICERCA"
                                    Text="oggetto"></asp:RadioButton>&nbsp;&nbsp;
                                <asp:RadioButton ID="optSearchFullText" runat="server" AutoPostBack="True" GroupName="TIPO_RICERCA"
                                    Text="testo contenuto"></asp:RadioButton></asp:Panel>
                            <br>
                            <br>
                            <asp:Panel ID="pnlSearchOggetto" runat="server" Width="409px" DESIGNTIMEDRAGDROP="223">
                                <table class="info_grigio" id="tblSearchOggetto" cellspacing="0" cellpadding="0"
                                    width="95%" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" width="1" height="1">
                                            &nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="10">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                            <asp:Label ID="lblOggetto" runat="server">Oggetto</asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td valign="middle" height="35">
                                            &nbsp;
                                            <asp:TextBox ID="txt_oggetto" runat="server" CssClass="testo_grigio" Width="370px" Height="32px"
													TextMode="MultiLine"></asp:TextBox></td>
                                    </tr>
                                       <tr>
		<td colspan="2" align="right" class="testo_grigio">
			caratteri disponibili:&nbsp;<input type="text" id="clTesto" runat="server" name="clTesto"  size="4" class="testo_grigio" readonly="readonly" />&nbsp;</td>
	</tr>

                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="10">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="titolo_scheda" style="height: 12px" valign="middle" height="12">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">
                                            <asp:Label ID="lblFiltri" runat="server">Filtri</asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td valign="middle" height="35">
                                            &nbsp;
                                            <asp:DropDownList ID="droplistLimitaRisultatiRicerca" runat="server" Width="368px"
                                                CssClass="testo_grigio" Visible="False">
                                                <asp:ListItem Value="DOC_DATA_ODIERNA" Selected="True">Documenti immessi in data odierna</asp:ListItem>
                                                <asp:ListItem Value="0">Ricerca tutti i documenti</asp:ListItem>
                                                <asp:ListItem Value="5">Ricerca gli ultimi 5 documenti</asp:ListItem>
                                                <asp:ListItem Value="10">Ricerca gli ultimi 10 documenti</asp:ListItem>
                                                <asp:ListItem Value="20">Ricerca gli ultimi 20 documenti</asp:ListItem>
                                                <asp:ListItem Value="30">Ricerca gli ultimi 30 documenti</asp:ListItem>
                                                <asp:ListItem Value="50">Ricerca gli ultimi 50 documenti</asp:ListItem>
                                                <asp:ListItem Value="100">Ricerca gli ultimi 100 documenti</asp:ListItem>
                                            </asp:DropDownList></td>
                                    </tr>
                                    <tr>
                                        <td height="20">
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlSearchFullText" runat="server" Width="409px" Height="56px" CssClass="info_grigio">
                                <table class="info_grigio" id="tblSearchFullText" cellspacing="0" cellpadding="0"
                                    width="95%" align="center" border="0">
                                    <tr>
                                        <td class="titolo_scheda" valign="middle" height="10">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">&nbsp;</td>
                                    </tr>
                                    <tr>
                                        <td class="titolo_scheda" style="height: 13px" valign="middle" height="13">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">&nbsp;
                                            <asp:Label ID="lblTestoContenuto" runat="server">Testo contenuto</asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td valign="middle" height="35">
                                            &nbsp;
                                            <asp:TextBox ID="txtTestoContenuto" runat="server" Width="368px" CssClass="testo_grigio"></asp:TextBox></td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </tbody>
        </table>
        <!--fine tabella ricerca-->
        <tr>
            <td style="width: 432px" height="10%">
                <!-- BOTTONIERA -->
                <table id="tbl_bottoniera" cellspacing="0" cellpadding="0" align="center" border="0">
                    <tr>
                        <td valign="top" height="5">
                            <img height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></td>
                    </tr>
                    <tr>
                        <td><asp:Button ID="butt_ricerca" runat="server" Text="Ricerca" CssClass="pulsante69" ToolTip="Ricerca documenti protocollati" /></td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                    </tr>
                    <!--TR>
								<TD width="100%" bgColor="#810d06"><IMG height="2" src="../images/proto/spaziatore.gif" width="5" border="0"></TD>
							</TR-->
                </table>
                <!--FINE BOTTONIERA -->
            </td>
        </tr>
        <cc2:MessageBox ID="mb_ConfirmDelete" Style="z-index: 101; left: 576px; position: absolute;
            top: 40px" runat="server"></cc2:MessageBox>
    </form>
</body>
</html>
