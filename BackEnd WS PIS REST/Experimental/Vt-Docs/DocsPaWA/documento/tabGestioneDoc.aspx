<%@ Page language="c#" Codebehind="tabGestioneDoc.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.tabGestioneDoc" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="uc1" TagName="NavigationContext" Src="../SiteNavigation/NavigationContext.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<script type="text/javascript" language="javascript">
		<!--
			ns=window.navigator.appName == "Netscape"
			ie=window.navigator.appName == "Microsoft Internet Explorer"
			function openDesc() 
			{
				try
				{
					if(ns) 
					{
						showbox= document.layers[1]
						showbox.visibility = "show";
						// showbox.top=63;
						var items = 1     ;
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
						// curEl.style.background = "#C08682"   
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

		//-->
		</script>
	</head>
	<body leftmargin="1" topmargin="0" ms_positioning="GridLayout">
    <form id="tab_gestioneDoc" method="post" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="Gestione Documento" />
    <input id="hd_currentTabName" type="hidden" name="hd_currentTabName" runat="server">
    <table height="100%" cellspacing="0" cellpadding="0" width="415" border="0">
        <asp:Panel runat="server" Visible="False" ID="pnl_cont">
            <tr>
                <td valign="top" align="center">
                    <table class="info" cellspacing="0" cellpadding="0" width="99%" border="0">
                        <tr>
                            <asp:Panel ID="pnl_regStato" runat="server">
                                <td class="testo_grigio_scuro" valign="middle" width="10%">
                                    <img height="1" src="../images/proto/spaziatore.gif" width="8" border="0">Registro
                                </td>
                                <td valign="middle" width="55%">
                                    <img height="1" src="../images/proto/spaziatore.gif" width="1" border="0">
                                    <asp:DropDownList ID="ddl_registri" runat="server" AutoPostBack="True" CssClass="testo_grigio"
                                        Width="134px">
                                    </asp:DropDownList>
                                    <asp:Label ID="lbl_registri" runat="server" CssClass="testo_grigio"></asp:Label><img
                                        height="1" src="../images/proto/spaziatore.gif" width="1" border="0">
                                    <asp:Image ID="icoReg" onmouseover="openDesc()" onmouseout="closeDesc()" runat="server"
                                        ImageUrl="../images/proto/ico_registro.gif" BorderStyle="Solid" BorderWidth="0px"
                                        BorderColor="Gray" ImageAlign="AbsMiddle"></asp:Image>
                                </td>
                                <td align="left" width="12%">
                                    <table height="22" cellspacing="0" cellpadding="0" border="0">
                                        <tr>
                                            <td class="testo_grigio_scuro">
                                                Stato<img height="1" src="../images/proto/spaziatore.gif" width="1" border="0">
                                            </td>
                                            <td valign="middle" align="center">
                                                <asp:Image ID="img_statoReg" runat="server" ImageUrl="../images/stato_giallo2.gif"
                                                    BorderWidth="1" ImageAlign="Bottom" Height="18px" Width="52px"></asp:Image><img height="1"
                                                        src="../images/proto/spaziatore.gif" width="2" border="0">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </asp:Panel>
                        </tr>
                    </table>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnl_random" Visible="False">
            <tr>
                <td height="2">
                </td>
            </tr>
        </asp:Panel>
        <tr height="100%">
            <td>
                <table id="tbl_tab" height="100%" cellspacing="0" cellpadding="0" width="415" border="0">
                    <tr valign="bottom" height="17">
                        <td style="height: 18px" width="67" height="18">
                            <cc1:ImageButton ID="btn_profilo" runat="server" Width="67px" BorderWidth="0px" SkinID="profilo_nonattivo"
                                Height="19px" DisabledUrl="../images/tab_disattivo.gif" Tipologia="DO_CERCA">
                            </cc1:ImageButton>
                        </td>
                        <td style="height: 18px" width="1" bgcolor="#ffffff" height="18">
                        </td>
                        <td style="height: 18px" width="67" height="18">
                            <cc1:ImageButton ID="btn_protocollo" runat="server" Width="67px" BorderWidth="0px"
                                SkinID="protocollo_nonattivo" BorderStyle="None" Height="19px" DisabledUrl="../images/tab_disattivo.gif"
                                Tipologia="DO_CERCA"></cc1:ImageButton>
                        </td>
                        <td style="height: 18px" width="1" bgcolor="#ffffff" height="18">
                        </td>
                        <td style="height: 18px" width="67" height="18">
                            <cc1:ImageButton ID="btn_classifica" runat="server" Width="67px" BorderWidth="0px"
                                SkinID="classifica_nonattivo" Height="19px" DisabledUrl="../images/tab_disattivo.gif"
                                Tipologia="DO_CLASSIFICAZIONE"></cc1:ImageButton>
                        </td>
                        <td style="height: 18px" width="1" bgcolor="#ffffff" height="18">
                        </td>
                        <td style="height: 18px" width="67" height="18">
                            <cc1:ImageButton ID="btn_allegati" runat="server" Width="67px" BorderWidth="0px"
                                SkinID="allegati_nonattivo" Height="19px" DisabledUrl="../images/tab_disattivo.gif"
                                Tipologia="DO_ALLEGATI"></cc1:ImageButton>
                        </td>
                        <td style="height: 18px" width="1" bgcolor="#ffffff" height="18">
                        </td>
                        <td style="height: 18px" width="67" height="18">
                            <cc1:ImageButton ID="btn_versioni" runat="server" Width="67px" BorderWidth="0px"
                                SkinID="versioni_nonattivo" Height="19px" DisabledUrl="../images/tab_disattivo.gif"
                                Tipologia="DO_VERSIONI"></cc1:ImageButton>
                        </td>
                        <td style="height: 18px" width="1" bgcolor="#ffffff" height="18">
                        </td>
                        <td style="height: 18px" width="67" height="18">
                            <cc1:ImageButton ID="btn_trasmissioni" runat="server" Width="67px" BorderWidth="0px"
                                SkinID="trasmissioni_nonattivo" Height="19px" DisabledUrl="../images/tab_disattivo.gif"
                                Tipologia="DO_TRASMISSIONI"></cc1:ImageButton>
                        </td>
                        <td style="height: 18px" width="1" bgcolor="#ffffff" height="18">
                        </td>
                    </tr>
                    <tr valign="top" align="center">
                        <td valign="top" align="center" width="415" colspan="12">
                            <cc1:IFrameWebControl ID="IframeTabs" runat="server" Align="center" Frameborder="0"
                                Scrolling="auto" Marginwidth="0" Marginheight="0" iHeight="100%" iWidth="415">
                            </cc1:IFrameWebControl>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
