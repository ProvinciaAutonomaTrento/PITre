<%@ Page language="c#" Codebehind="printLabelPdf.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.printLabel" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
	    <title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		
		<script language="javascript">
			function isNum_p()
			{
				isnum('tbxPosX','tbxPosY','docWidth','docHeight');		
			}
		</script>
		
		<script language="Javascript">
		function isnum(objX,objY,docWidth,docHeight) 
		{
			var objIdX = document.getElementById(objX);
			var objIdY = document.getElementById(objY);
			var docwidth = document.getElementById(docWidth);
			var docheight = document.getElementById(docHeight);
			if (objIdX.value != '' && objIdY.value != '')
			{
				if ( (!isNaN(objIdX.value))&& (!isNaN(objIdY.value)) )
				{
					if (parseInt(objIdX.value)<0 || parseInt(objIdX.value)>parseInt(docwidth.value))
					{
						alert('Sono ammessi valori di X compresi nelle dimensioni del documento');
						
						document.Form1.tbxPosX.select();
						document.Form1.tb_hidden.value = 'tbxPosX';
						
					}
					else
					{
						if (parseInt(objIdY.value)<0 || parseInt(objIdY.value)>parseInt(docheight.value))
						{
							alert('Sono ammessi valori di Y compresi nelle dimensioni del documento');
						
							document.Form1.tbxPosY.select();
							document.Form1.tb_hidden.value = 'tbxPosY';
							
						}
						else
						{
							//window.returnValue='refresh';window.close();
						}
					}
				}
				else
				{
					alert('Sono ammessi solo valori numerici');
				}
			}
			else
			{
				alert('Inserire i valori delle coordinate');
			}
		}		
		</script>
		
		<script language="javascript" >
		    // apre la modale che contiene il etichetta PDF
		    function ApriModaleVisDocPdf()
		    {
		       var newLeft=(screen.availWidth-605);
		       var newTop=(screen.availHeight-400);	
	           window.showModalDialog("modalVisualizzaDocPdf.aspx","","dialogWidth:600px;dialogHeight:600px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:no;help:no;"); 
		    }
		</script>
		
		<script language="javascript">
			function isNumProtocollo()
			{
				isnumProto('tbxPosX','tbxPosY','docWidth','docHeight');		
			}
		</script>
		
		<script language="Javascript">
		function isnumProto(objX,objY,docWidth,docHeight) 
		{
			var objIdX = document.getElementById(objX);
			var objIdY = document.getElementById(objY);
			var docwidth = document.getElementById(docWidth);
			var docheight = document.getElementById(docHeight);
			if (objIdX.value != '' && objIdY.value != '')
			{
				if ( (!isNaN(objIdX.value))&& (!isNaN(objIdY.value)) )
				{
					if (parseInt(objIdX.value)<0 || parseInt(objIdX.value)>parseInt(docwidth.value))
					{
						alert('Sono ammessi valori di X compresi nelle dimensioni del documento');
						
						document.Form1.tbxPosX.select();
						document.Form1.tb_hidden.value = 'tbxPosX';
						
					}
					else
					{
						if (parseInt(objIdY.value)<0 || parseInt(objIdY.value)>parseInt(docheight.value))
						{
							alert('Sono ammessi valori di Y compresi nelle dimensioni del documento');
						
							document.Form1.tbxPosY.select();
							document.Form1.tb_hidden.value = 'tbxPosY';
							
						}
						else
						{
						       ApriModaleVisDocPdf();
						}
					}
				}
				else
				{
					alert('Sono ammessi solo valori numerici');
				}
			}
			else
			{
				alert('Inserire i valori delle coordinate');
			}
		}

		function closeWind(e) {
		    //solo per modale invocata dal pulsante posiziona segnatura
		    if (window.dialogArguments.document != null) {
		        window.onunload = function (e) {
		            // Firefox || IE 
		            e = e || window.event;
		            var y = e.pageY || e.clientY;
		            if (y < 0)
		                window.dialogArguments.document.location = window.dialogArguments.document.location;
		        };
		    }
		}
        </script>
		
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Stampa etichetta" />
			<input type="hidden" id="tb_hidden" name="tb_hidden" runat="server"></input>
			    <TABLE class="info" id="Table1" style="Z-INDEX: 101; LEFT: 45px; WIDTH: 316px; POSITION: absolute; TOP: 8px; HEIGHT: 210px" cellSpacing="0" cellPadding="0" align="center" border="0">
				    <TR>
						<td class="titolo_scheda" style="HEIGHT: 20px" align="center" colSpan="4">Posizione 
							delle informazioni</td>
					</TR>
					<tr bgColor="#d9d9d9">
						<td class="titolo_scheda" style="HEIGHT: 20px" align="center" colSpan="4">Dimensioni 
							del documento (Pixel):
						</td>
					</tr>
					<tr bgColor="#d9d9d9">
						<td class="titolo_scheda" style="HEIGHT: 15px" align="center" colSpan="4">Larghezza:&nbsp;
							<asp:textbox id="docWidth" runat="server" ReadOnly="True" ForeColor="Black" BorderColor="#D9D9D9"
								BackColor="#D9D9D9" BorderStyle="None" Width="30px" CssClass="testo_grigio"></asp:textbox>Lunghezza:&nbsp;
							<asp:textbox id="docHeight" runat="server" ReadOnly="True" ForeColor="Black" BorderColor="#D9D9D9"
								BackColor="#D9D9D9" BorderStyle="None" Width="30px" CssClass="testo_grigio"></asp:textbox></td>
					</tr>
					<tr bgColor="#d9d9d9">
						<td style="HEIGHT: 7px" colSpan="4"></td>
					</tr>
					<TR>
						<TD align="center" bgColor="#d9d9d9" colSpan="4">
							<!--Tabella sensibile per la scelta della posizione della stampa del timbroA4-->
							<table id="tab_scelta" cellSpacing="0" cellPadding="0" align="center" background="../images/tabDoc/docPreview.jpg"
								border="1">
								<tr>
									<td>
										<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
											<tr>
												<td align="left" colSpan="2"><asp:button id="UpSx" runat="server" Width="60px" CssClass="testo_btn_p" Height="15px" Text="AA|B_B|01"></asp:button></td>
												<td align="right" colSpan="2"><asp:button id="UpDx" runat="server" Width="60px" CssClass="testo_btn_p" Height="15px" Text="AA|B_B|01"></asp:button></td>
											</tr>
											<TR>
												<td align="left" width="25" colSpan="1" height="53"></td>
												<td colSpan="2" rowSpan="2">&nbsp;&nbsp;&nbsp;</td>
												<td align="right" width="25" colSpan="1" height="53"></td>
											</TR>
											<tr>
												<td align="left" width="25" colSpan="1" height="53"></td>
												<td align="right" width="25" colSpan="1" height="53"></td>
											</tr>
											<tr>
												<td align="left" colSpan="2"><asp:button id="DownSx" runat="server" Width="60px" CssClass="testo_btn_p" Height="15px" Text="AA|B_B|01"></asp:button></td>
												<td align="right" colSpan="2"><asp:button id="DownDx" runat="server" Width="60px" CssClass="testo_btn_p" Height="15px" Text="AA|B_B|01"></asp:button></td>
											</tr>
										</table>
									</td>
								</tr>
							</table>
						</TD>
					</TR>
					</table>
					<table class="info" id="Table2" style="Z-INDEX: 101; LEFT: 45px; WIDTH: 316px; POSITION: absolute; TOP: 220px; HEIGHT: 50px" cellSpacing="0" cellPadding="0" align="center" border="0">
					    <tr>
					        <td class="titolo_scheda" style="HEIGHT: 20px" align="center">Posizione personalizzata</td>
			            </tr>
				        <TR bgColor="#d9d9d9">
				            <TD class="testo_msg_grigio" style="HEIGHT: 33px" align="center" colSpan="4">
                                X:
						        <asp:textbox id="tbxPosX" runat="server" Width="35" CssClass="testo_grigio" AutoPostBack="true"></asp:textbox>&nbsp;
						        Y:
						        <asp:textbox id="tbxPosY" runat="server" Width="35" CssClass="testo_grigio" AutoPostBack="true"></asp:textbox>&nbsp;&nbsp;&nbsp;
                                <%if (IsPermanentDisplaysSegnature && Request.QueryString["proto"] == null){ %>
                                    <asp:button id="btn_refresh_preview" runat="server" Width="110px" CssClass="pulsante_hand" Text="Aggiorna Preview"></asp:button>
                                <% }%>                           
                           </TD>						
				        </TR>				        
			        </TABLE>
			        <table class="info" id="Table5" style="Z-INDEX: 101; LEFT: 45px; WIDTH: 316px; POSITION: absolute; TOP: 280px; HEIGHT: 21px" cellSpacing="0" cellPadding="0" align="center" border="0">
			            <tr>
					        <td class="titolo_scheda" style="HEIGHT: 20px" align="center">Caratteristiche Timbro/Segnatura</td>
			            </tr>
			        </table>
			        <table class="info" id="Table4" style="Z-INDEX: 101; LEFT: 45px; WIDTH: 316px; POSITION: absolute; TOP: 300px; HEIGHT: 110px" cellSpacing="0" cellPadding="0" align="center" border="0">
				        <TR bgColor="#d9d9d9">
				            <TD class="pulsante_hand" style="HEIGHT: 50px;" align="center" colSpan="4">
				                <asp:Label ID="Label2" runat="server" Text="Carattere" Visible="True"></asp:Label>
				                &nbsp; <asp:DropDownList ID="CarattereList" runat="server" Height="18px" Width="140px" Visible="True" CssClass="testo_grigio" AutoPostBack="true">
				                </asp:DropDownList>
				                <br />
                                <br />
				                <asp:Label ID="Label1" runat="server" Text="Colore" Visible="True"></asp:Label>
				                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:DropDownList ID="ColoreList" AutoPostBack="true" runat="server" Height="18px" Width="140px" Visible="True" CssClass="testo_grigio">
				                </asp:DropDownList>
                            </TD>
				        </TR>
				        <TR bgColor="#d9d9d9">
				            <TD style="HEIGHT: 60px;" align="left" colSpan="4">
				                <asp:RadioButtonList ID="Tim_Segn_List" runat="server" CssClass="pulsante_hand" 
                                    Height="40px" Width="314px" AutoPostBack="True" 
                                    OnSelectedIndexChanged="Tim_Segn_List_SelectedIndexChanged" RepeatColumns="2">
                                    <asp:ListItem Value="verticale">Timbro verticale</asp:ListItem>
                                    <asp:ListItem Value="orizzontale">Timbro orizzontale</asp:ListItem>
                                    <asp:ListItem Value="false">Segnatura</asp:ListItem>
                                    <asp:ListItem Value="notimbro">Nessun Timbro/Segn.</asp:ListItem>
                                </asp:RadioButtonList>
                            </TD>
                            <%--<TD class="pulsante_hand" style="HEIGHT: 50px; width: 175px;" align="center" colSpan="4">
                                &nbsp; &nbsp; &nbsp; &nbsp;
                                <asp:Label ID="Label1" runat="server" Text="Rotazione" Visible="False"></asp:Label>
                                <asp:DropDownList ID="RotazioneList" runat="server" Height="18px" Width="80px" Visible="False" CssClass="testo_grigio">
                                    <asp:ListItem></asp:ListItem>
                                    <asp:ListItem>0</asp:ListItem>
                                    <asp:ListItem>90</asp:ListItem>
                                    <asp:ListItem>180</asp:ListItem>
                                    <asp:ListItem>270</asp:ListItem>
                                </asp:DropDownList><br />
                                </TD>--%>
				        </TR>				        
			        </TABLE>

                    <table class="info" id="Table6" style="Z-INDEX: 101; LEFT: 45px; WIDTH: 316px; POSITION: absolute; TOP: 410px; HEIGHT: 25px" cellSpacing="0" cellPadding="0" align="center" border="0">
                         <tr bgColor="#d9d9d9">  
                          <td>                     
                            <asp:CheckBox runat="server" ID="chkInfoFirma" AutoPostBack="true" OnCheckedChanged="chkInfoFirma_CheckedChanged"></asp:CheckBox> 
                            <asp:Label id="label3" CssClass="pulsante_hand" runat="server" Text="Visualizza informazioni firma digitale"></asp:Label>
                            </td>
                        </tr>
                                   					        
                         <tr bgColor="#d9d9d9">   
                         <td>
                             <%--<asp:RadioButton runat="server" id="chkPrima" text="Prima pagina"  OnCheckedChanged="chkPrima_OnCheckedChanged";/>
                             <asp:RadioButton runat="server" id="chkUltima" text="Ultima pagina" OnCheckedChanged="chkUltima_OnCheckedChanged"; />--%>
                            <asp:RadioButtonList ID="infoPosFirmaList" runat="server" CssClass="pulsante_hand" RepeatDirection="Horizontal" Enabled="false" AutoPostBack="true">
                                    <asp:ListItem Value="printOnFirstPage" Selected="True">Prima pagina</asp:ListItem>
                                    <asp:ListItem Value="printOnLastPage">Ultima pagina</asp:ListItem>                          
                            </asp:RadioButtonList>   
                            </td>                    
                        </tr>

                        <asp:Panel ID="pnlTipoFirma" runat="server" >
                        <tr bgColor="#d9d9d9" >   
                         <td>
                            <asp:RadioButtonList ID="rblTipoFirma" runat="server" CssClass="pulsante_hand" RepeatDirection="Horizontal" Enabled="false" AutoPostBack="true">
                                    <asp:ListItem Value="SIGN_EXT" Selected="True">Firma Completa</asp:ListItem>
                                    <asp:ListItem Value="SIGN_SHORT">Firma Sintetica</asp:ListItem>                          
                            </asp:RadioButtonList>   
                            </td>                    
                        </tr>
                        </asp:Panel>
			        </table>

			        <table id="Table3" style="Z-INDEX: 101; LEFT: 42px; WIDTH: 300px; POSITION: absolute; TOP: 490px; HEIGHT: 50px" cellSpacing="0" cellPadding="0" align="center" border="0">
                         <tr align="center">
			                <td>
                                <asp:button id="btn_save" runat="server" Width="188px" CssClass="pulsante_hand" Text="Salva Versione con segnatura" 
                                    ToolTip="Salva una nuova versione del documento con impressa la segnatura di protocollo come visualizzata nel file a destra" Visible="false"></asp:button>
			                </td>
			             </tr>
                         <tr align="center">
			                <td>
			                    <asp:button id="btn_ok" runat="server" Width="80px" CssClass="pulsante_hand" Text="Conferma"></asp:button>
			                    <asp:button id="btn_only_print" runat="server" Width="105px" CssClass="pulsante_hand" Text="Applica a stampa" Visible="false"></asp:button>
			                    <asp:button id="btn_chiudi" runat="server" Width="80px" CssClass="pulsante_hand" Text="Chiudi"></asp:button>
			                </td>
			             </tr>
			        </table>
		</form>
	</body>
</HTML>
