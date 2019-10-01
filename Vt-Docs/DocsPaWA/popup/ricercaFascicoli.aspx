<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>

<%@ Page Language="c#" CodeBehind="ricercaFascicoli.aspx.cs" AutoEventWireup="false"
    Inherits="DocsPAWA.popup.ricercaFascicoli" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

    <script language="javascript" src="../LIBRERIE/rubrica.js"></script>

    <link href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
    <%Response.Expires = -1;%>
    <base target="_self">

    <script language="javascript" id="ricFascicoli_Click" event="onclick()" for="ricFascicoli">
					window.document.body.style.cursor='wait';
					SetPanelsVisibility()
    </script>

    <script language="JavaScript">
		    var w = window.screen.width;
		    var h = window.screen.height;
		    var new_w = (w-100)/2;
		    var new_h = (h-400)/2;
			
		    function apriPopupAnteprima() {
			    //window.open('../documento/AnteprimaProfDinRicerche.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=420,scrollbars=YES');
			    window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinRicercheFasc.aspx','','dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
		    }		
		    
		
			function SetPanelsVisibility()
			{
				if(document.getElementById("LOADING")!=null)
				{
					document.getElementById("LOADING").style.visibility = "Visible";
				}
				
				if(document.getElementById("DivDataGrid")!=null)
				{
					document.getElementById("DivDataGrid").style.visibility = "hidden";						
				}
				if(document.getElementById("pnlButtonOk")!=null)
				{
					document.getElementById("pnlButtonOk").style.visibility = "hidden";
					
				}
				
				var lbl_count=document.getElementById("lbl_countRecord");
				if(lbl_count!=null)
					lbl_count.style.visibility = "hidden";							
			}
			
		function _ApriRubrica(target)
		{
			var r = new Rubrica();
			
			switch (target) {

				case "filtri_uffref":
					r.CallType = r.CALLTYPE_FILTRIRICFASC_UFFREF;
					break;					
					
				case "filtri_locfis":
					r.CallType = r.CALLTYPE_FILTRIRICFASC_LOCFIS;
					break;												
			}
			var res = r.Apri(); 		
		}	
		
		function ApriRicercaSottoFascicoli(idfascicolo,desc)
		{
    		    var newLeft=(screen.availWidth-615);
			    var newTop=(screen.availHeight-710);	
    	        // apertura della ModalDialog
			    rtnValue = window.showModalDialog('../popup/RicercaSottoFascicoli.aspx?idfascicolo=' + idfascicolo + '&desc=' + desc,"","dialogWidth:615px;dialogHeight:440px;status:no;resizable:no;scroll:no;dialogLeft:"+newLeft+";dialogTop:"+newTop+";center:no;help:no;"); 
			    if (rtnValue == "Y")
			    {
			       window.returnValue = 'Y';
				    window.close();
			    }

        }

        function find_focus() {
            document.ricercaFascicoli.ricFascicoli.focus();
        }

		
		
    </script>

    <script language="C#" runat="server">		
        void checkOPT(object sender, EventArgs e)
        {
            foreach (DataGridItem dgItem in this.DgListaFasc.Items)
            {
                RadioButton optFasc = dgItem.Cells[0].FindControl("OptFasc") as RadioButton;
                if (optFasc != null)
                    optFasc.Checked = optFasc.Equals(sender);
            }
        }
			
			
    </script>

</head>
<body bottommargin="3" leftmargin="3" topmargin="3" rightmargin="3" onload="find_focus()">
    <form id="ricercaFascicoli" method="post" runat="server" defaultbutton="ricFascicoli">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Ricerca Fascicoli" />
    <input id="hd_systemIdLF" type="hidden" size="1" name="hd_systemIdUffRef" runat="server">
    <input id="hd_systemIdUffRef" type="hidden" size="1" name="hd_systemIdUffRef" runat="server">
    <table class="contenitore" height="100%" width="100%" align="center" border="0">
        <tr valign="middle">
            <td class="menu_1_rosso" align="center">
                Ricerca Fascicoli&nbsp;<asp:Label ID="lbl_codClass" runat="server" Visible="False"></asp:Label>
            </td>
        </tr>
        <tr valign="top">
            <td valign="top">
                <table class="info_grigio" cellspacing="3" cellpadding="0" width="95%" align="center"
                    border="0">
                    <tr>
                        <td>
                            <!-- tabella Data APERTURA-->
                            <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                <tr>
                                    <td>
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="lbl_titolari" runat="server" CssClass="titolo_scheda">Titolario</asp:Label>
                                    </td>
                                    <td>
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:DropDownList ID="ddl_titolari" runat="server" CssClass="testo_grigio" AutoPostBack="false"
                                            Width="250px">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr align="left">
                                    <!-- <td colSpan="2">
												<table cellSpacing="0" cellPadding="0" width="100%" align="left" border="1">
													<tr> -->
                                    <asp:Panel ID="pnl_registri" runat="server">
                                        <td style="width: 17%" height="20">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:Label ID="lbl_registro" runat="server" Width="53px" CssClass="titolo_scheda">Registro</asp:Label>
                                        </td>
                                        <td width="83%" height="20">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:DropDownList ID="ddl_registri" runat="server" Width="115px" CssClass="testo_grigio"
                                                AutoPostBack="True">
                                            </asp:DropDownList>
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:CheckBox ID="checkADL" runat="server"
                                                Checked="false" Text="Ricerca solo in ADL" CssClass="titolo_scheda" />
                                        </td>
                                    </asp:Panel>
                                    <!-- </tr>
												</table>
											</td> -->
                                </tr>
                                <tr align="left">
                                    <td style="width: 17%" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="ldl_apertA" runat="server" CssClass="titolo_scheda" Visible="True">Aperto&nbsp;il</asp:Label>
                                    </td>
                                    <td width="83%" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:DropDownList ID="ddl_dataA" runat="server" Width="25%" CssClass="testo_grigio"
                                            AutoPostBack="True">
                                            <asp:ListItem Value="0">Valore singolo</asp:ListItem>
                                            <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                        </asp:DropDownList>
                                        &nbsp;
                                        <asp:Label ID="lbl_initdataA" runat="server" CssClass="testo_grigio" Visible="False">da<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></asp:Label>
                                        <uc3:Calendario ID="txt_initDataA" runat="server" Visible="true" />
                                        <asp:Label ID="lbl_finedataA" runat="server" CssClass="testo_grigio" Visible="False">a<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></asp:Label>
                                        <uc3:Calendario ID="txt_fineDataA" runat="server" Visible="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <asp:Panel ID="pnl_dataChiusura" runat="server" Visible="false">
                        <tr>
                            <td colspan="2">
                                <!-- tabella Data CHIUSURA-->
                                <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                    <tr align="left">
                                        <td style="width: 17%" height="20">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:Label ID="lbl_dtaC" runat="server" CssClass="titolo_scheda" Visible="True">Chiuso&nbsp;il</asp:Label>
                                        </td>
                                        <td width="83%" height="20">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:DropDownList ID="ddl_dataC" runat="server" Width="25%" CssClass="testo_grigio"
                                                AutoPostBack="true">
                                                <asp:ListItem Value="0">Valore singolo</asp:ListItem>
                                                <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;
                                            <asp:Label ID="lbl_initdataC" runat="server" CssClass="testo_grigio" Visible="False">da<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></asp:Label>
                                            <uc3:Calendario ID="txt_initDataC" runat="server" Visible="true" />
                                            <asp:Label ID="lbl_finedataC" runat="server" CssClass="testo_grigio" Visible="False">a<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></asp:Label>
                                            <uc3:Calendario ID="txt_fineDataC" runat="server" Visible="false" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:Panel>
                    <tr>
                        <td>
                            <!-- tabella Data CREAZIONE-->
                            <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                <tr align="left">
                                    <td style="width: 17%" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="lbl_dtaCreaz" runat="server" CssClass="titolo_scheda" Visible="True">Creato&nbsp;il</asp:Label>
                                    </td>
                                    <td width="83%" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:DropDownList ID="ddl_creaz" runat="server" Width="25%" CssClass="testo_grigio"
                                            AutoPostBack="true">
                                            <asp:ListItem Value="0">Valore singolo</asp:ListItem>
                                            <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                        </asp:DropDownList>
                                        &nbsp;
                                        <asp:Label ID="lbl_initCreaz" runat="server" CssClass="testo_grigio" Visible="False">da<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></asp:Label>
                                        <uc3:Calendario ID="txt_initDataCrea" runat="server" Visible="true" />
                                        <asp:Label ID="lbl_finCreaz" runat="server" CssClass="testo_grigio" Visible="False">a<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></asp:Label>
                                        <uc3:Calendario ID="txt_fineDataCrea" runat="server" Visible="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                <tr align="left">
                                    <td style="width: 17%" align="left" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="lbl_NumFasc" runat="server" CssClass="titolo_scheda" Visible="True">Numero</asp:Label>
                                    </td>
                                    <td width="188" colspan="2" height="20" style="width: 188px">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:TextBox ID="txtNumFasc" runat="server" Width="117px" CssClass="testo_grigio"
                                            BackColor="White"></asp:TextBox>
                                    </td>
                                    <td align="left" width="8%" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="lblAnnoFasc" runat="server" CssClass="titolo_scheda" Visible="True">Anno</asp:Label>
                                    </td>
                                    <td width="20%" colspan="2" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:TextBox ID="txtAnnoFasc" runat="server" Width="80px" CssClass="testo_grigio"
                                            BackColor="White"></asp:TextBox>
                                    </td>
                                    <td align="center" width="8%" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="lbl_tipo" runat="server" CssClass="titolo_scheda" Visible="True">Tipo</asp:Label>
                                    </td>
                                    <td width="20%" colspan="2" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:DropDownList ID="ddlTipo" runat="server" Width="100px" CssClass="testo_grigio"
                                            AutoPostBack="True">
                                            <asp:ListItem></asp:ListItem>
                                            <asp:ListItem Value="G">Generale</asp:ListItem>
                                            <asp:ListItem Value="P">Procedimentale</asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                <tr align="left">
                                    <td style="width: 17%" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="lbl_descr" runat="server" CssClass="titolo_scheda" Visible="True">Descrizione</asp:Label>
                                    </td>
                                    <td width="83%" colspan="2" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:TextBox ID="txtDescr" runat="server" Width="385px" CssClass="testo_grigio" BackColor="White"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                <tr align="left">
                                    <td style="width: 17%" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="lbl_note" runat="server" CssClass="titolo_scheda" Visible="True">Note</asp:Label>
                                    </td>
                                    <td width="83%" colspan="2" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:TextBox ID="txt_note" runat="server" Width="385px" CssClass="testo_grigio" BackColor="White"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                <tr align="left">
                                    <td style="width: 17%" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="Label3" runat="server" CssClass="titolo_scheda" Visible="True">Collocaz. fisica</asp:Label>
                                    </td>
                                    <td width="73%" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:TextBox ID="txt_varCodRubrica_LF" runat="server" Width="29%" CssClass="testo_grigio"
                                            AutoPostBack="True" BackColor="White"></asp:TextBox>&nbsp;
                                        <asp:TextBox ID="txt_descr_LF" runat="server" Width="260px" CssClass="testo_grigio"
                                            BackColor="White" ReadOnly="True"></asp:TextBox>
                                    </td>
                                    <td valign="middle" align="left">
                                        <asp:Image ID="btn_Rubrica" runat="server" ImageUrl="../images/proto/rubrica.gif">
                                        </asp:Image>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                <tr align="left">
                                    <td style="width: 17%" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="Label2" runat="server" CssClass="titolo_scheda" Visible="True">Data Collocaz.</asp:Label>
                                    </td>
                                    <td width="83%" height="20">
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:DropDownList ID="ddl_data_LF" runat="server" Width="25%" CssClass="testo_grigio"
                                            AutoPostBack="True">
                                            <asp:ListItem Value="0">Valore singolo</asp:ListItem>
                                            <asp:ListItem Value="1">Intervallo</asp:ListItem>
                                        </asp:DropDownList>
                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                        <asp:Label ID="lbl_dta_LF_DA" runat="server" CssClass="testo_grigio" Visible="False">da<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></asp:Label>
                                        <uc3:Calendario ID="txt_dta_LF_DA" runat="server" Visible="true" />
                                        <asp:Label ID="lbl_dta_LF_A" runat="server" CssClass="testo_grigio" Visible="False">a<IMG height="1" src="../images/proto/spaziatore.gif" width="4" border="0"></asp:Label>
                                        <uc3:Calendario ID="txt_dta_LF_A" runat="server" Visible="false" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="pnl_uffRef" runat="server" Visible="False">
                                <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                    <tr align="left">
                                        <td width="17%" height="20">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:Label ID="lbl_uffRef" runat="server" Visible="True" CssClass="titolo_scheda">Uff. Referente</asp:Label>
                                        </td>
                                        <td width="68%" height="20">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:TextBox ID="txt_cod_UffRef" runat="server" Width="30%" CssClass="testo_grigio"
                                                AutoPostBack="True" BackColor="White"></asp:TextBox>&nbsp;
                                            <asp:TextBox ID="txt_desc_uffRef" runat="server" Width="227px" CssClass="testo_grigio"
                                                BackColor="White" ReadOnly="True"></asp:TextBox>
                                        </td>
                                        <td valign="middle" align="left" height="20">
                                            <asp:Image ID="btn_rubricaRef" runat="server" ImageUrl="../images/proto/rubrica.gif">
                                            </asp:Image>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <asp:Panel ID="pnl_sottofacicoli" runat="server">
                        <tr>
                            <td>
                                <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                    <tr align="left">
                                        <td style="width: 17%" height="20">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:Label ID="Label1" runat="server" CssClass="titolo_scheda" Visible="True">Sottofascicolo</asp:Label>
                                        </td>
                                        <td width="83%" colspan="2" height="20">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:TextBox ID="txt_sottofascicolo" runat="server" Width="385px" CssClass="testo_grigio"
                                                BackColor="White"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:Panel>
                    <tr>
                        <td>
                            <asp:Panel ID="pnl_mostraSottonodi" runat="server">
                                <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                    <tr align="left">
                                        <td class="testo_grigio" valign="middle" align="left" width="60%">
                                            <asp:Label ID="lbl_mostraTuttiFascicoli" runat="server" CssClass="testo_grigio_scuro"></asp:Label>
                                        </td>
                                        <td valign="middle" align="left" width="40%">
                                            <asp:RadioButtonList ID="rbl_MostraTutti" runat="server" CssClass="testo_grigio"
                                                AutoPostBack="True" Height="20" RepeatDirection="Horizontal">
                                                <asp:ListItem Value="S">SI&nbsp;&nbsp;</asp:ListItem>
                                                <asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <asp:Panel ID="pnl_profilazione" runat="server" Visible="false">
                        <tr>
                            <td>
                                <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                    <tr align="left">
                                        <td style="width: 17%" height="20">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:Label ID="lbl_tipoFascicolo" runat="server" CssClass="titolo_scheda">Tipo fasc.</asp:Label>
                                        </td>
                                        <td width="83%" colspan="2" height="20">
                                            <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                            <asp:DropDownList ID="ddl_tipoFasc" runat="server" AutoPostBack="true" CssClass="testo_grigio"
                                                Width="254px">
                                            </asp:DropDownList>
                                            &nbsp;
                                            <asp:ImageButton ID="img_dettagliProfilazione" ImageUrl="../images/proto/ico_oggettario.gif"
                                                AlternateText="Ricerca per campi profilati" runat="server" Visible="false" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:Panel>
                    <tr>
                        <td align="center">
                            <asp:Button ID="ricFascicoli" runat="server" Width="55px" CssClass="PULSANTE" Height="19px"
                                Text="CERCA"></asp:Button>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr valign="top" align="center">
            <td class="countRecord" valign="middle" align="center" width="100%" height="20">
                <asp:Label ID="lbl_countRecord" runat="server" CssClass="titolo_rosso" Visible="False">Fascicoli totali:</asp:Label>
            </td>
        </tr>
        <tr valign="top" align="center">
            <td valign="top">
                <div class="testo_grigio_scuro" id="LOADING" style="font-size: 12px; left: 250px;
                    visibility: hidden; position: absolute; top: 420px" align="center">
                    Ricerca in corso ...
                </div>
                <div id="DivDataGrid" style="overflow: auto; width: 580px; height: 280px">
                    <asp:DataGrid ID="DgListaFasc" SkinID="datagrid" runat="server" Width="97%" AllowPaging="True"
                        AllowCustomPaging="True" HorizontalAlign="Center" BorderColor="Gray" BorderWidth="1px"
                        CellPadding="1" AutoGenerateColumns="False" BorderStyle="Inset" OnItemCommand="DgListaFasc_ItemCommand">
                        <SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
                        <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
                        <ItemStyle HorizontalAlign="Center" Height="20px" CssClass="bg_grigioN" VerticalAlign="Middle">
                        </ItemStyle>
                        <HeaderStyle Wrap="False" Height="20px" ForeColor="White" CssClass="menu_1_bianco_dg">
                        </HeaderStyle>
                        <Columns>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="5%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <asp:RadioButton ID="OptFasc" runat="server" AutoPostBack="True" Visible="True" Text=""
                                        OnCheckedChanged="checkOPT" TextAlign="Right"></asp:RadioButton>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn>
                                <HeaderStyle Width="5%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                <ItemTemplate>
                                    <asp:ImageButton ID="btn_Sottofascicoli" runat="server" CssClass="testo_grigio" CommandName="Sottofascicoli"
                                        ImageUrl="../images/folder_piena.gif"></asp:ImageButton>
                                </ItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn SortExpression="Codice" HeaderText="Codice">
                                <HeaderStyle Width="20%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>'
                                        ID="Label6">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>'
                                        ID="Textbox4">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Registro">
                                <HeaderStyle Width="10%"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:Label ID="lbl_descRegistro" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Registro") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txt_descRegistro" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Registro") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn SortExpression="Descrizione" HeaderText="Descrizione">
                                <HeaderStyle Width="45%"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Left" Width="150px"></ItemStyle>
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DocsPAWA.Utils.TruncateString(DataBinder.Eval(Container, "DataItem.Descrizione")) %>'
                                        ID="Label7">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>'
                                        ID="Textbox5">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn SortExpression="Tipo" HeaderText="Tipo">
                                <HeaderStyle Width="5%"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipo") %>'
                                        ID="Label4">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Tipo") %>'
                                        ID="Textbox2">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn Visible="false" HeaderText="Stato">
                                <HeaderStyle Width="5%"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:Label ID="stato" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Stato") %>'>
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Apertura">
                                <HeaderStyle Width="10%"></HeaderStyle>
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Apertura") %>'
                                        ID="Label8">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Apertura") %>'
                                        ID="Textbox6">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn Visible="false" HeaderText="Chiusura">
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiusura") %>'
                                        ID="Label9">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiusura") %>'
                                        ID="Textbox7">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn Visible="False" HeaderText="Chiave">
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'
                                        ID="Label10">
                                    </asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Chiave") %>'
                                        ID="Textbox8">
                                    </asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                        <PagerStyle HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
                            Mode="NumericPages"></PagerStyle>
                    </asp:DataGrid></div>
            </td>
        </tr>
        <tr>
            <td align="center">
                <table>
                    <tr>
                        <td align="center">
                            <asp:Panel ID="pnlButtonOk" runat="server" Visible="False">
                                <asp:Button ID="btn_ok" runat="server" CssClass="PULSANTE" Text="OK"></asp:Button>
                            </asp:Panel>
                        </td>
                        <td>
                            <asp:Button ID="btn_chiudi" runat="server" CssClass="PULSANTE" Text="CHIUDI"></asp:Button>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
