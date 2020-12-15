<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Grafica.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Grafica.Grafica" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script language="JavaScript" src="../CSS/caricamenujs.js"></script>
    <META HTTP-EQUIV="Pragma" CONTENT="no-cache">
    <META HTTP-EQUIV="Expires" CONTENT="-1">	
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <script language="javascript" type="text/javascript" id="ControlloClient">
        esadec='0123456789ABCDEF'

        function controllaCampi(_red,_green,_blu)
        {
            var red = document.getElementById(_red);
            var blue = document.getElementById(_blu);
            var green = document.getElementById(_green);
            
            if ((red.value != '')&&(blue.value != '')&&(green.value != ''))
            {
                if (isNaN(red.value) && isNaN(blue.value) && isNaN(green.value))
                {
                     alert('Attenzione sono ammessi solo valori numerici');
                     return false;
                }
                else
                {
                    if(isNaN(red.value))
                    {
                        alert('Attenzione sono ammessi solo valori numerici per Rosso');
                        document.form1._red.select();
                        return false;
                    }
                    if(isNaN(green.value))
                    {
                      alert('Attenzione sono ammessi solo valori numerici per Verde');
                      document.form1._green.select();
                      return false;
                    }
                    if(isNaN(blue.value))
                    {
                      alert('Attenzione sono ammessi solo valori numerici per Blu');
                      document.form1._blu.select();
                      return false;
                    }
                }
             }
             else
             {
                 alert('Attenzione inserire i valori RGB');
                 return false;
             }
        }
 
        function controllaPulsCampi(_red,_green,_blu)
        {
            var red = document.getElementById(_red);
            var blue = document.getElementById(_blu);
            var green = document.getElementById(_green);
            
            if ((red.value != '')&&(blue.value != '')&&(green.value != ''))
            {
                if (isNaN(red.value) && isNaN(blue.value) && isNaN(green.value))
                {
                     alert('Attenzione sono ammessi solo valori numerici');
                     return false;
                }
                else
                {
                    if(isNaN(red.value))
                    {
                        alert('Attenzione sono ammessi solo valori numerici per Rosso');
                        document.form1._red.select();
                        return false;
                    }
                    if(isNaN(green.value))
                    {
                      alert('Attenzione sono ammessi solo valori numerici per Verde');
                      document.form1._green.select();
                      return false;
                    }
                    if(isNaN(blue.value))
                    {
                      alert('Attenzione sono ammessi solo valori numerici per Blu');
                      document.form1._blu.select();
                      return false;
                    }
                    
                }
             }
             else
             {
                 alert('Attenzione inserire i valori RGB');
                 return false;
             }
        }
 
   </script>
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" bgcolor="#f6f4f4" height="100%"  >
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Gestione Grafica" />
         <uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
        <table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
            <tr>
                <td>
                    <!-- TESTATA CON MENU' -->
                    <uc1:testata id="Testata" runat="server"></uc1:testata>
                </td>
            </tr>
            <tr>
                <td class="testo_grigio_scuro" bgcolor="#c0c0c0" height="20">
                    <asp:label id="lbl_position" runat="server"></asp:label>
                </td>
            </tr>
            <tr>
                <!-- TITOLO PAGINA -->
                <td class="titolo" style="height: 20px" align="center" bgcolor="#e0e0e0" height="34">Gestione Grafica</td>
            </tr>
            <tr>
                <td valign="top" align="center" bgcolor="#f6f4f4" height="100%" width="100%">
                    <!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
                    <table cellspacing="0" cellpadding="0" align="center" border="0">
                        <tr><td height="10"></td></tr>
                        <tr>
                            <td align="center" height="25" rowspan="2"><b><font  class="testo_grigio_scuro">Logo Ente</font></b><br /><font class="testo_piccolo">width="455" height="38"</font></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"><asp:Image runat="server" ID="logoEnte" ImageUrl="~/images/testata/320/sf1.bmp" Width="133" Height="38" /></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td align="center" height="25"><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"><INPUT type="file" runat="server" class="testo" id="uploadLogoEnte" size="40" name="uploadLogoEnte"></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"><asp:Button CssClass="testo_btn" 
                                    ID="ModificaEnte" runat="server" Text="Modifica" onclick="Mod_LogoEnte" /></td>
                        </tr>
                        <tr>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"><asp:Image runat="server" ID="sfondoTesto" ImageUrl="~/images/testata/320/sf2.jpg" Width="133" Height="10px" /></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td align="center" height="25"><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"><INPUT type="file" runat="server" class="testo" id="uploadSfondo2" size="40" name="uploadSfondo2"></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"><asp:Button 
                                    ID="ModificaSfondoTesto" runat="server" Text="Modifica Sfondo" 
                                    CssClass="testo_btn" onclick="Mod_Sfondo" /></td>
                        </tr>
                        <tr><td colspan="7" height="15"></td></tr>
                        <tr><td colspan="7" height="2"><img height="2" src="../../images/testata/btn_sfondo.gif" width="100%" /></td></tr>
                        <tr><td colspan="7" height="15"></td></tr>
                        <tr>
                            <td align="center" height="25" rowspan="2" class="testo_grigio_scuro"><b>Colore del testo e sfondo</b></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td runat="server" id="backgroundTesto" ><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0">
                            <asp:Label runat="server" ID="sfondo" name="sfondo" Text="Testo di Prova" 
                                    width="133px" height="20px"></asp:Label> </td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td align="center" height="25" valign="top"><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0">
                                <table>
                                    <tr>
                                        <td colspan="6">
                                            <asp:DropDownList ID="colori" runat="server" CssClass="testo" Width="130px">
                                                <asp:ListItem Value="None" Text="" />
                                                <asp:ListItem Value="0^0^0" Text="nero" />
                                                <asp:ListItem Value="0^0^255" Text="blu" />
                                                <asp:ListItem Value="0^255^0" Text="verde" />
                                                <asp:ListItem Value="255^0^0" Text="rosso" />
                                                <asp:ListItem Value="255^255^255" Text="bianco" />
                                                <asp:ListItem Value="255^255^0" Text="giallo" />
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="testo_piccolo">Rosso</td><td><asp:TextBox runat="server" ID="_red" Width="20" CssClass="testo_grigio_scuro" ></asp:TextBox></td>
                                        <td class="testo_piccolo">Verde</td><td><asp:TextBox runat="server" ID="_green" Width="20" CssClass="testo_grigio_scuro" ></asp:TextBox></td>
                                        <td class="testo_piccolo">Blu</td><td><asp:TextBox runat="server" ID="_blu" Width="20" CssClass="testo_grigio_scuro" ></asp:TextBox></td>
                                    </tr>
                                </table>
                            </td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"><asp:Button 
                                    ID="ModificaColoreTesto" runat="server" Text="Modifica" CssClass="testo_btn" 
                                    OnClientClick="return controllaCampi('_red','_green','_blu')" 
                                    onclick="Mod_Click" /></td>
                        </tr>
                        <tr>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"><asp:Image runat="server" ID="sfondoLogoEnte" ImageUrl="~/images/testata/320/sf1.jpg" Width="133" Height="10px"  /></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td align="center" height="25"><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"><INPUT type="file" runat="server" class="testo" id="uploadSfondo" size="40" name="uploadSfondo"></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"><asp:Button 
                                    ID="ModificaEnteSfondo" CssClass="testo_btn" runat="server" 
                                    Text="Modifica Sfondo" onclick="Mod_SfondoEnte" /></td>
                        </tr>
                        <tr><td colspan="7" height="15"></td></tr>
                        <tr><td colspan="7" height="2"><img height="2" src="../../images/testata/btn_sfondo.gif" width="100%" /></td></tr>
                        <tr><td colspan="7" height="15"></td></tr>
                        <tr>
                            <td align="center" height="25" rowspan="2" class="testo_grigio_scuro"><b>Colore Amministrazione</b></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <asp:TableCell runat="server" ID="Td1" SkinID="pulsantiera"><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></asp:TableCell>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td align="center" height="25" valign="top"><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0">
                                <asp:DropDownList ID="ddl_TemaAmministraz" runat="server" CssClass="testo" Width="130px">
                                </asp:DropDownList>
                            </td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"><asp:Button 
                                    ID="btn_modificaTema" runat="server" Text="Modifica" CssClass="testo_btn" 
                                    onclick="Mod_Tema" /></td>
                        </tr>
                        <tr><td colspan="7" height="15"></td></tr>
                        <tr><td colspan="7" height="2"><img height="2" src="../../images/testata/btn_sfondo.gif" width="100%" /></td></tr>
                        <tr><td colspan="7" height="15"></td></tr>
                        <tr>
                            <td align="center" height="25" rowspan="2" class="testo_grigio_scuro"><b>Colore Segnatura</b></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <asp:TableCell runat="server" ID="Td2" SkinID="pulsantiera"><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></asp:TableCell>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td align="center" height="25" valign="top"><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0">
                                <asp:DropDownList ID="ddl_segnatura" runat="server" CssClass="testo" Width="130px">
                                     <asp:ListItem Value="0" Text="Nero"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="Blu"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="Rosso"></asp:ListItem>
                                </asp:DropDownList>
                           </td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"></td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0"><asp:Button 
                                    ID="btn_modificaSegnatura" runat="server" Text="Modifica" CssClass="testo_btn" 
                                    onclick="Mod_Segn" /></td>
                        </tr>
                        <tr><td colspan="7" height="15"></td></tr>
                        <tr><td colspan="7" height="2"><img height="2" src="../../images/testata/btn_sfondo.gif" width="100%" /></td></tr>
                        <tr><td colspan="7" height="15"></td></tr>
                        <tr>
                            <td colspan="6">&nbsp;</td>
                            <td><IMG height="15" src="../../images/spaziatore.gif" width="5" border="0">
                            <asp:Button ID="btn_refresh" runat="server" Text="Aggiorna" CssClass="testo_btn" OnClick="Aggiorna" /></td>
                        </tr>
                 </table>
                </td>
            </tr>
        </table>

    </form>
</body>
</html>
