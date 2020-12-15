<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestIndisponibilita.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_Indisponibilita.GestIndisponibilita" %>

<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
<%@ Register TagPrefix="cc2" Namespace="Utilities" Assembly="MessageBox" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
    <link href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <script type="text/javascript" src="../CSS/caricamenujs.js"></script>
    <script type="text/javascript">
        
        function CountLeft(field, count, max) {
            if (field.value.length > max)
                field.value = field.value.substring(0, max);
            else
                count.value = max - field.value.length;
        }

        
    </script>
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" bgcolor="#f6f4f4" height="100%">
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Gestione Password" />
    <!-- Gestione del menu a tendina -->
    <uc2:MenuTendina ID="MenuTendina" runat="server"></uc2:MenuTendina>
        <table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
            <tr>
                <td>
                    <!-- TESTATA CON MENU' -->
                    <uc1:Testata ID="Testata" runat="server"></uc1:Testata>
                </td>
            </tr>
            <tr>
                <td class="testo_grigio_scuro" bgcolor="#c0c0c0" height="20">
                    <asp:Label ID="lbl_position" TabIndex="25" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <!-- TITOLO PAGINA -->
                <td class="titolo" align="center" bgcolor="#e0e0e0" height="20">
                    Gestione indisponibilità del servizio
                </td>
            </tr>
            <tr>
                <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                    <!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
                    <table cellspacing="0" cellpadding="0" align="center" border="0">
                        <tr>
                            <td align="center" height="25">
                                <asp:label id="lbl_avviso" runat="server" cssclass="testo_rosso"></asp:label>
                            </td>
                        </tr>
                        <tr>
                            <td class="pulsanti" align="center">
                                <table width="900">
                                    <tr>
                                        <td align="left">
                                            <asp:label id="lbl_titolo" runat="server" cssclass="titolo">Gestione indisponibilità</asp:label>
                                        </td>
                                        <td align="center">
                                            <asp:Label id="lbl_stato1" runat="server" CssClass="testo_grigio_scuro_grande" Text="Stato Servizio:"/>
                                            <asp:Label id="lbl_stato"  runat="server" cssclass="testo_verde" />
                                        </td>
                                        <td align="right">
                                            <asp:button id="btn_Salva" runat="server" CssClass="testo_btn_p" Text="Salva" OnClick="btn_Salva_Click"></asp:button>
                                            <asp:button id="btn_Delete" runat="server" Width="120" CssClass="testo_btn_p" Visible="false" Text="Elimina disservizio" OnClick="btn_Delete_Click"></asp:button>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    <tr>
                        <td align="center" height="5"><br>
                            <table width="99.5%" border="0">
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lbl_notifica" runat="server" cssclass="testo_grigio_scuro_grande" Width="520px" Text="Testo per la notifica agli utenti connessi*" />
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lbl_cortesia" runat="server" CssClass="testo_grigio_scuro_grande" Width="350px" Text="Testo per la pagina di cortesia*" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:TextBox ID="txt_notifica" runat="server" onKeyDown="CountLeft(this.form.txt_notifica,this.form.inp_caratt,1024);" onKeyUp="CountLeft(this.form.txt_notifica,this.form.inp_caratt,1024);" CssClass="testo" TextMode="MultiLine" MaxLength="1024"  Width="350px" Height="120px"  />
                                        </td>
                                        <td align="right">
                                            <asp:TextBox ID="txt_cortesia" runat="server" onKeyDown="CountLeft(this.form.txt_cortesia,this.form.inp_caratt1,1024);" onKeyUp="CountLeft(this.form.txt_cortesia,this.form.inp_caratt1,1024);" CssClass="testo" TextMode="MultiLine" MaxLength="1024" Width="350px" Height="120px"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbl_caratt" cssclass="testo_grigio_scuro" Text="caratteri disponibili" runat="server"/>
                                            <input readonly="readonly" ID="inp_caratt" cssclass="testo_grigio_scuro" size="3" class="testo_grigio_scuro" type="text"  value="1024" runat="server" />
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="lbl_caratt1" cssclass="testo_grigio_scuro" Text="caratteri disponibili" runat="server"/>
                                            <input readonly="readonly" ID="inp_caratt1" cssclass="testo_grigio_scuro" size="3" class="testo_grigio_scuro" type="text"  value="1024" runat="server" />
                                        </td>
                                    </tr>
                                    <tr >
                                        <td colspan="2" bgcolor="#800000" height="2"/>
                                    </tr>
                                    <tr>
                                        <td align="center" height="10">
                                            <asp:label id="Label1" runat="server" cssclass="testo_rosso"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:CheckBox ID="chk_email" Text="Invio notifica email disservizio" runat="server" CssClass="testo_grigio_scuro" />
                                        </td>
                                        <td align="left">
                                            <asp:CheckBox ID="chk_ripresa" Text="Invio notifica email di ripresa del servizio" runat="server" CssClass="testo_grigio_scuro"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" height="5">
                                            <asp:label id="Label2" runat="server" cssclass="testo_rosso"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="left">
                                            <asp:Label ID="lbl_email" runat="server" cssclass="testo_grigio_scuro_grande" Width="520px" Text="Testo per la notifica email di disservizio" />
                                        </td>
                                        <td align="left">
                                            <asp:Label ID="lbl_ripresa" runat="server" CssClass="testo_grigio_scuro_grande" Width="350px" Text="Testo per la notifica email di ripresa del servizio" />
                                        </td>
                                    </tr> 
                                    <tr>
                                        <td align="left">
                                            <asp:TextBox ID="txt_email" runat="server" onKeyDown="CountLeft(this.form.txt_email,this.form.inp_caratt2,1024);" onKeyUp="CountLeft(this.form.txt_email,this.form.inp_caratt2,1024);" CssClass="testo" TextMode="MultiLine" MaxLength="1024"  Width="350px" Height="120px"/>
                                        </td>
                                        <td align="right">
                                            <asp:TextBox ID="txt_ripresa" runat="server" onKeyDown="CountLeft(this.form.txt_ripresa,this.form.inp_caratt3,1024);" onKeyUp="CountLeft(this.form.txt_ripresa,this.form.inp_caratt3,1024);" CssClass="testo" TextMode="MultiLine" MaxLength="1024" Width="350px" Height="120px"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:Label ID="lbl_caratt2" cssclass="testo_grigio_scuro" Text="caratteri disponibili" runat="server"/>
                                            <input readonly="readonly" ID="inp_caratt2" cssclass="testo_grigio_scuro" size="3" class="testo_grigio_scuro" type="text"  value="1024" runat="server" /> 
                                        </td>
                                        <td align="right">
                                            <asp:Label ID="lbl_caratt3" cssclass="testo_grigio_scuro" Text="caratteri disponibili" runat="server"/>
                                            <input readonly="readonly" ID="inp_caratt3" cssclass="testo_grigio_scuro" size="3" class="testo_grigio_scuro" type="text"  value="1024" runat="server" /> 
                                        </td>
                                    </tr>
                                    <tr >
                                        <td colspan="2" bgcolor="#800000" height="2"/>
                                    </tr>
                                    <tr align="left">
                                       <td>
                                            <asp:Button ID="btn_notifica" runat="server" CssClass="testo_btn_p" Width="120" Text="Notifica disservizio" OnClick="btn_notifica_Click"></asp:Button>&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btn_avvia" runat="server" CssClass="testo_btn_p" Width="120" Text="Avvia disservizio" OnClick="btn_avvia_Click"></asp:Button>&nbsp;&nbsp;&nbsp;
                                            <asp:Button ID="btn_ripristina" runat="server" Width="120" CssClass="testo_btn_p" Text="Ripristina servizio" Visible="false" OnClick="btn_ripristina_Click"></asp:Button>
                                      </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="msg_notifica" 
                                runat="server" ongetmessageboxresponse="msg_notifica_GetMessageBoxResponse"/>
                                            <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="msg_avvia" 
                                            runat="server" ongetmessageboxresponse="msg_avvia_GetMessageBoxResponse"/>
                                            <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="msg_elimina" 
                                            runat="server" ongetmessageboxresponse="msg_elimina_GetMessageBoxResponse"/>
                                            <cc2:MessageBox Width="1" Height="1" CssClass="testo_grigio" ID="msg_ripristina" 
                                            runat="server" ongetmessageboxresponse="msg_ripristina_GetMessageBoxResponse"/>
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
</html>
