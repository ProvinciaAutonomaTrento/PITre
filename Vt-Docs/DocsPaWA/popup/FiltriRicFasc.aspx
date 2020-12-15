<%@ Import Namespace="Microsoft.Web.UI.WebControls" %>
<%@ Register Src="../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register TagPrefix="mytree" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register Src="../UserControls/Calendar.ascx" TagName="Calendario" TagPrefix="uc3" %>

<%@ Page Language="c#" CodeBehind="FiltriRicFasc.aspx.cs" AutoEventWireup="false"
    Inherits="DocsPAWA.fascicoli.FiltriRicFasc" EnableViewStateMac="False" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

    <script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>

    <script language="javascript" src="../LIBRERIE/rubrica.js"></script>

    <link id="idLinkCss" href="" type="text/css" rel="stylesheet">

    <script language="JavaScript">

		ns=window.navigator.appName == "Netscape"
		ie=window.navigator.appName == "Microsoft Internet Explorer"

        var w = window.screen.width;
		var h = window.screen.height;
		var new_w = (w-100)/2;
		var new_h = (h-400)/2;
			
		function apriPopupAnteprima() {
			//window.open('../documento/AnteprimaProfDinRicerche.aspx','','top = '+ new_h +' left = '+new_w+' width=500,height=420,scrollbars=YES');
			window.showModalDialog('../documento/AnteprimaProfDinModal.aspx?Chiamante=AnteprimaProfDinRicercheFasc.aspx','','dialogWidth:510px;dialogHeight:400px;status:no;resizable:no;scroll:no;center:yes;help:no;close:no;top:'+ new_h +';left:'+new_w);
		}		
		
		function btn_titolario_onClick(queryString)
		{		
			var retValue=true;
			//if (document.fascicoli_sx.txt_codClass.value=="")
			//	{
			//		retValue=confermaCaricamentoTitolario();
			//	}

			if (retValue)
				{
					ApriTitolario(queryString,"gestFasc");
				}
		
			return retValue;
		}
	
		function createFascicolo()
		{
			var l_createFascicolo='<%=this.l_createFascicolo%>';
			if (l_createFascicolo)
			{
				__doPostBack('','');
				var dx_frame=window.parent.frames[1];
			}
		}

		function openDesc() 
		{
			try
			{
				if(ns) 
				{
					showbox= document.layers[1]
					showbox.visibility = "show";
					var items = 1;
					for (i=1; i<=items; i++) 
					{
						elopen=document.layers[i]
						if (i != (1)) 
						{ 
							elopen.visibility = "hide" 
						}
					}
				}    
				if(ie) 
				{
					curEl = event.toElement
					showBox = document.all.descreg;
					showBox.style.visibility = "visible";
				}
			}
			catch(e)
			{
				return false;
			}
		}

		function closeDesc() 
		{
			try
			{			
				var items = 1 
				for (i=0; i<items; i++) 
				{
					if(ie)
					{
						document.all.descreg.style.visibility = "hidden"
					}
					if(ns)
					{ 
						document.layers[i].visibility = "hide"
					}          
				}
			}
			catch(e)
			{
				return false;
			}
		}

		function ApriRubricaUfficioRef(wnd, target) 
		{
			rtnValue=window.showModalDialog("../popup/rubricaDT.aspx?wnd="+wnd +"&target="+target + "&clear=1","Rubrica", "dialogWidth:600px;dialogHeight:590px;status:no;resizable:no;scroll:no;dialogLeft:170px;dialogTop:80px;center:no;help:no;"); 
			window.document.FiltriRicFasc.submit();
			return true;	
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

    </script>

    <script language="javascript" id="clientEventHandlersJS">
		<!--
			function window_onload() {}
		//-->
    </script>

    <%Response.Expires = -1;%>
    <base target="_self">
</head>
<body language="javascript" leftmargin="0" topmargin="0" ms_positioning="GridLayout">
    <form id="FiltriRicFasc" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Ricerca fascicoli" />
    <input id="hd_systemIdLF" type="hidden" size="1" name="hd_systemIdUffRef" runat="server">
    <input id="hd_systemIdUffRef" type="hidden" size="1" name="hd_systemIdUffRef" runat="server">
    <table id="tbl_contenitore" height="100%" cellspacing="0" cellpadding="0" width="95%"
        align="center" border="0">
        <tr>
            <td valign="top" height="5">
            </td>
        </tr>
        <tr>
            <td valign="top">
                <table class="contenitore" cellspacing="0" cellpadding="0" width="100%" border="0">
                    <tr>
                        <td class="menu_1_rosso" style="height: 27px" valign="middle" align="center" colspan="2"
                            height="17">
                            <asp:Button ID="Button2" runat="server" Text="Button" Height="0px" Width="0px"></asp:Button>Filtri
                            di ricerca
                        </td>
                    </tr>
                    <!-- inizio modifica 21/12/2004 -->
                    <tr align="center">
                        <td align="center" colspan="2">
                            <table class="info_grigio" cellspacing="0" cellpadding="0" width="95%" align="center"
                                border="0">
                                <tr>
                                    <td colspan="2" height="10">
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <!-- tabella Titolari-->
                                        <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                            <tr align="left">
                                                <td style="width: 20%" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:Label ID="lbl_Titolari" runat="server" CssClass="titolo_scheda" Visible="True">Titolari</asp:Label>
                                                </td>
                                                <td style="width: 80%" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:DropDownList ID="ddl_titolari" runat="server" AutoPostBack="false" CssClass="testo_grigio"
                                                        Width="65%" EnableViewState="True">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <!-- tabella Data APERTURA-->
                                        <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                            <tr align="left">
                                                <td style="width: 20%" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:Label ID="ldl_apertA" runat="server" CssClass="titolo_scheda" Visible="True">Aperto&nbsp;il</asp:Label>
                                                </td>
                                                <td width="80%" height="25">
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
                                <asp:Panel ID="pnl_chiuso" runat="server" Visible="false">
                                    <tr>
                                        <td colspan="2">
                                            <!-- tabella Data CHIUSURA-->
                                            <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                                <tr align="left">
                                                    <td style="width: 20%" height="25">
                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                        <asp:Label ID="lbl_dtaC" runat="server" CssClass="titolo_scheda" Visible="True">Chiuso&nbsp;il</asp:Label>
                                                    </td>
                                                    <td width="80%" height="25">
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
                                    <td colspan="2">
                                        <!-- tabella Data CREAZIONE-->
                                        <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                            <tr align="left">
                                                <td style="width: 20%" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:Label ID="lbl_dtaCreaz" runat="server" CssClass="titolo_scheda" Visible="True">Creato&nbsp;il</asp:Label>
                                                </td>
                                                <td width="80%" height="25">
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
                                    <td colspan="2">
                                        <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                            <tr align="left">
                                                <td style="width: 20%" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:Label ID="lbl_NumFasc" runat="server" CssClass="titolo_scheda" Visible="True">Numero</asp:Label>
                                                </td>
                                                <td width="25%" colspan="2" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:TextBox ID="txtNumFasc" runat="server" Width="75%" CssClass="testo_grigio" BackColor="White"></asp:TextBox>
                                                </td>
                                                <td align="left" width="10%" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:Label ID="lblAnnoFasc" runat="server" CssClass="titolo_scheda" Visible="True">Anno</asp:Label>
                                                </td>
                                                <td width="50%" colspan="2" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:TextBox ID="txtAnnoFasc" runat="server" Width="60%" CssClass="testo_grigio"
                                                        BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                            <tr align="left">
                                                <td style="width: 20%" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:Label ID="lbl_Stato" runat="server" CssClass="titolo_scheda" Visible="True">Stato</asp:Label>
                                                </td>
                                                <td width="25%" colspan="2" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:DropDownList ID="ddlStato" runat="server" Width="75%" CssClass="testo_grigio">
                                                        <asp:ListItem></asp:ListItem>
                                                        <asp:ListItem Value="A">Aperto</asp:ListItem>
                                                        <asp:ListItem Value="C">Chiuso</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <td align="left" width="10%" height="25">
                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                        <asp:Label ID="lbl_tipo" runat="server" CssClass="titolo_scheda" Visible="True">Tipo</asp:Label>
                                                    </td>
                                                    <td width="50%" colspan="2" height="25">
                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                        <asp:DropDownList ID="ddlTipo" runat="server" Width="60%" CssClass="testo_grigio"
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
                                    <td colspan="2">
                                        <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                            <tr align="left">
                                                <td style="width: 20%" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:Label ID="lbl_descr" runat="server" CssClass="titolo_scheda" Visible="True">Descrizione</asp:Label>
                                                </td>
                                                <td width="80%" colspan="2" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:TextBox ID="txtDescr" runat="server" Width="78%" CssClass="testo_grigio" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                            <tr align="left">
                                                <td style="width: 20%" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:Label ID="lblNote" runat="server" CssClass="titolo_scheda" Visible="True">Note</asp:Label>
                                                </td>
                                                <td width="80%" colspan="2" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:TextBox ID="txtNote" runat="server" Width="78%" CssClass="testo_grigio" BackColor="White"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <table cellspacing="0" cellpadding="0" width="100%" align="left" border="0">
                                            <tr align="left">
                                                <td style="width: 20%" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:Label ID="Label2" runat="server" CssClass="titolo_scheda" Visible="True">Data Collocaz.</asp:Label>
                                                </td>
                                                <td height="25">
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
                                    <td colspan="2">
                                        <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                            <tr align="left">
                                                <td style="width: 20%" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:Label ID="Label3" runat="server" CssClass="titolo_scheda" Visible="True">Collocaz. fisica</asp:Label>
                                                </td>
                                                <td width="65%" height="25">
                                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                    <asp:TextBox ID="txt_varCodRubrica_LF" runat="server" Width="30%" CssClass="testo_grigio"
                                                        AutoPostBack="True" BackColor="White"></asp:TextBox>&nbsp;
                                                    <asp:TextBox ID="txt_descr_LF" runat="server" Width="227px" CssClass="testo_grigio"
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
                                <asp:Panel ID="pnl_uffRef" Visible="False" runat="server">
                                    <tr>
                                        <td colspan="2">
                                            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                                <tr align="left">
                                                    <td style="width: 20%" height="25">
                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                        <asp:Label ID="lbl_uffRef" runat="server" Visible="True" CssClass="titolo_scheda">Ufficio Referente</asp:Label>
                                                    </td>
                                                    <td width="65%" height="25">
                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                        <asp:TextBox ID="txt_cod_UffRef" runat="server" Width="30%" CssClass="testo_grigio"
                                                            AutoPostBack="True" BackColor="White"></asp:TextBox>&nbsp;
                                                        <asp:TextBox ID="txt_desc_uffRef" runat="server" Width="227px" CssClass="testo_grigio"
                                                            BackColor="White" ReadOnly="True"></asp:TextBox>
                                                    </td>
                                                    <td valign="middle" align="left" height="25">
                                                        <asp:Image ID="btn_rubricaRef" runat="server" ImageUrl="../images/proto/rubrica.gif">
                                                        </asp:Image>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </asp:Panel>
                                <asp:Panel ID="pnl_profilazione" runat="server" Visible="false">
                                    <tr>
                                        <td colspan="2">
                                            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                                <tr align="left">
                                                    <td style="width: 20%" height="25">
                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                        <asp:Label ID="lbl_tipoFascicolo" runat="server" CssClass="titolo_scheda">Tipo fascicolo</asp:Label>&nbsp;
                                                    </td>
                                                    <td style="width: 80%" height="25">
                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                        <asp:DropDownList ID="ddl_tipoFasc" runat="server" AutoPostBack="true" CssClass="testo_grigio"
                                                            Width="343px">
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
                                <asp:Panel ID="pnl_sottofascicolo" runat="server" Visible="true">
                                    <tr>
                                        <td colspan="2">
                                            <table cellspacing="0" cellpadding="0" width="100%" border="0">
                                                <tr align="left">
                                                    <td style="width: 20%" height="25">
                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                        <asp:Label ID="Label1" runat="server" CssClass="titolo_scheda" Visible="True">SottoFascicolo</asp:Label>
                                                    </td>
                                                    <td width="80%" colspan="2" height="25">
                                                        <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                                        <asp:TextBox ID="txt_sottofascicolo" runat="server" Width="78%" CssClass="testo_grigio"
                                                            BackColor="White"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </asp:Panel>
                                <tr>
                                    <td colspan="2" height="10">
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <asp:Panel ID="pnl_mostraSottonodi" runat="server">
                    <tr>
                         <td colspan="2" style="width: 100%" height="25" valign="middle">&nbsp;&nbsp;
                                <asp:Label ID="lbl_mostraTuttiFascicoli" runat="server" CssClass="testo_grigio_scuro"></asp:Label>
                                <asp:RadioButtonList ID="rbl_MostraTutti" runat="server" CssClass="testo_grigio" AutoPostBack="True" RepeatDirection="Horizontal">
                                    <asp:ListItem Value="S">SI&nbsp;&nbsp;</asp:ListItem>
                                    <asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                    </asp:Panel>
                   <asp:Panel ID="pnl_filtroExcel" runat="server" Visible="false">
                        <tr>
                            <td style="width: 22%" height="25" valign="middle">
                                <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                <asp:Label ID="lbl_Export" runat="server" CssClass="titolo_scheda">Filtra risultati con</asp:Label>&nbsp;
                            </td>
                            <td style="width: 78%" height="25" valign="middle">
                                <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                <asp:FileUpload class="testo_grigio" ID="uploadedFile" size="40" name="uploadedFile" CssClass="PULSANTE" runat="server"></asp:FileUpload>&nbsp;
                                <asp:Button ID="UploadBtn" Text="Carica" CssClass="PULSANTE" runat="server"></asp:Button>&nbsp;<a href="../Import/Fascicoli/TemplateFiltroFascicoli.xls" target="_blank"><span style="color:#666666;font-size:11px;font-weight:bold;">Scarica</span></a>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                <asp:Label ID="lbl_fileExcel" runat="server" CssClass="titolo_scheda">Nessun file excel caricato.</asp:Label>
                                <asp:ImageButton ID="btn_elimina_excel" runat="server" ImageUrl="../images/proto/b_elimina.gif"
                                    ToolTip="Elimina" Visible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 22%" height="25">
                                <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                <asp:Label ID="lbl_attributo" runat="server" CssClass="titolo_scheda">Attributo</asp:Label>&nbsp;
                            </td>
                            <td style="width: 78%" height="25">
                                <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                <asp:DropDownList ID="ddl_attributo" runat="server" AutoPostBack="true" CssClass="testo_grigio"
                                    Width="343px">
                                    <asp:ListItem Value="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="NUMERO_FASCICOLO">Numero di fascicolo</asp:ListItem>
                                    <asp:ListItem Value="DATA_APERTURA">Data di apertura</asp:ListItem>
                                    <asp:ListItem Value="DESCRIZIONE_FASCICOLO">Descrizione</asp:ListItem>
                                    <asp:ListItem Value="CODICE_NODO">Codice di classifica</asp:ListItem>
                                    <asp:ListItem Value="TIPOLOGIA_FASCICOLO">Tipologia</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <asp:Panel ID="pnl_tipoFascExcel" runat="server" Visible = "false">
                            <tr>
                                <td style="width: 22%" height="25">
                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                    <asp:Label ID="Label4" runat="server" CssClass="titolo_scheda">Tipo fascicolo</asp:Label>&nbsp;
                                </td>
                                <td style="width: 78%" height="25">
                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                    <asp:DropDownList ID="ddl_tipoFascExcel" runat="server" AutoPostBack="true" CssClass="testo_grigio"
                                        Width="343px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnl_attrTipoFascExcel" Visible="false">
                            <tr>
                                <td style="width: 22%" height="25">
                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                    <asp:Label ID="Label5" runat="server" CssClass="titolo_scheda">Attributo tipo fasc.</asp:Label>&nbsp;
                                </td>
                                <td style="width: 78%" height="25">
                                    <img height="1" src="../images/proto/spaziatore.gif" width="4" border="0">
                                    <asp:DropDownList ID="ddl_attrTipoFascExcel" runat="server" AutoPostBack="true" CssClass="testo_grigio"
                                        Width="343px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </asp:Panel>
                    </asp:Panel>
                    <asp:Panel ID="pnl_spazioFinale" runat="server" Visible="false">
                        <tr>
                            <td colspan="2" height="10">
                            </td>
                        </tr>
                    </asp:Panel>
                </table>
            </td>
        </tr>
        <tr>
            <td valign="middle" width="95%" height="10%">
                <!-- BOTTONIERA -->
                <table id="tbl_bottoniera" cellspacing="0" cellpadding="0" width="100%" align="center"
                    border="0">
                    <tr>
                        <td align="center">
                            <asp:Button ID="btn_ricFascicoli" Text="CERCA" Height="19px" Width="55px" CssClass="PULSANTE"
                                runat="server"></asp:Button><img height="1" src="../images/proto/spaziatore.gif"
                                    width="4" border="0">
                            <asp:Button ID="btn_chiudi" runat="server" Text="CHIUDI" Height="19px" Width="55px"
                                CssClass="PULSANTE"></asp:Button>
                        </td>
                    </tr>
                </table>
                <!--FINE	BOTTONIERA -->
            </td>
        </tr>
    </table>

    <script language="javascript">
				esecuzioneScriptUtente();
    </script>

    </form>
</body>
</html>
