<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ConservazioneWA.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR" />
    <meta content="C#" name="CODE_LANGUAGE" />
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
    <title>Conservazione > Login</title>
    <link href="CSS/docspa_30.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" language="javascript">
        function body_onLoad() {
            var maxWidth = 570;
            var maxHeight = 360;

            window.resizeTo(maxWidth, maxHeight);
            var newLeft = (screen.availWidth - maxWidth) / 2;
            var newTop = (screen.availHeight - maxHeight) / 2;
            window.moveTo(newLeft, newTop);
            Form1.txt_userId.focus();
        }
        function openPage() {
            window.open('HomePage.aspx');
            window.close();
        }

        function forceLogin(ipaddress, ruolo) {
            var message;
            message = 'L\'utente ha gia\' una connessione in corso';
            if (ipaddress != "")
                message = message + " con indirizzo IP " + ipaddress;
            message += '\n';
            message += 'Chiudere la connessione esistente e crearne una nuova?';

            //var result = window.confirm('L\'utente ha gia\' una connessione in corso. \nChiudere la connessione esistente e crearne una nuova?');
            var result = window.confirm(message);

            if (result) {
                //		var popup = window.open('Login.aspx?forceLogin=True&userId='+Form1.hd_userId.value+'&pwd='+Form1.hd_pwd.value);
                //		
                //		if(popup != self) {
                //			window.opener = null;
                //			self.close();
                //		}
                Form1.hd_forceLogin.value = "true";
                Form1.submit();
            }
        }

        //    function openPage()
        //    {
        //        var pageHeight=window.screen.availHeight;
        //				var pageWidth=window.screen.availWidth;
        //				
        //				window.showModalDialog('HomePage.aspx',
        //								'',
        //								'dialogWidth:' + pageWidth + 'px;dialogHeight:' + pageHeight + 'px;status:no;resizable:yes;scroll:yes;dialogLeft:0;dialogTop:0;center:no;help:no;close:no');
        //								window.close();
        //    }
    </script>
    <style type="text/css">
        .style1
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 91px;
        }
        .style2
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            width: 62px;
            height: 10px;
        }
        .style3
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
            height: 20px;
            width: 62px;
        }
        .style4
        {
            height: 15px;
            width: 62px;
        }
    </style>
</head>
<!--<body style="background-color: #ffffff" onload="body_onLoad()">-->
<body style="background-color: #ffffff">
    <form id="Form1" name="loginF" method="post" runat="server">
    <input type="hidden" runat="server" id="hd_forceLogin" name="hd_forceLogin" />
    <input type="hidden" runat="server" id="hd_pwd" name="hd_pwd" />
    <input type="hidden" runat="server" id="hd_userId" name="hd_userId" />
    <table height="300" cellspacing="0" cellpadding="0" width="550" align="center" bgcolor="#ffffff"
        border="0">
        <tr valign="top">
            <td valign="top">
                <table class="login_header" cellspacing="0" cellpadding="0" align="center" border="0">
                    <tr class="login_header_row" valign="top" align="left">
                        <td width="50%">
                            <asp:Image runat="server" SkinID="logo" border="0" Width="141" Height="58" ID="img_logologin">
                            </asp:Image><asp:Image Width="121" Height="58" SkinID="logoente1" ID="img_logologinente1"
                                runat="server" ImageAlign="top" Visible="true"></asp:Image>
                        </td>
                        <td align="right" width="50%">
                            <asp:Image Width="262" Height="58" SkinID="logoente2" ID="img_logologinente2" runat="server"
                                ImageAlign="top" Visible="true"></asp:Image>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" width="100%" colspan="2" style="height: 20px"
                            class="testo_intestazione">
                            <asp:Label ID="Label1" runat="server" Text="Gestione istanze di conservazione"></asp:Label>
                        </td>
                    </tr>
                    <tr height="20">
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                    <asp:Panel ID="pnl_ddlAmm" Visible="False" runat="server">
                        <tr style="height: 20px">
                            <td class="testo_grigio_scuro" valign="middle" align="right">
                                Amministrazioni Disponibili&nbsp;&nbsp;&nbsp;
                            </td>
                            <td valign="top" align="left">
                                <asp:DropDownList ID="ddl_Amministrazioni" runat="server" CssClass="testo_grigio">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <!-- MEV CS 1.4 Esibizione Combo Profilo Accesso -->
                    <asp:Panel ID="pnl_profili" Visible="False" runat="server">
                        <tr style="height: 20px">
                            <td class="testo_grigio_scuro" valign="middle" align="right">
                                Profili Disponibili&nbsp;&nbsp;&nbsp;
                            </td>
                            <td valign="top" align="left">
                                <asp:DropDownList ID="ddl_profili" runat="server" CssClass="testo_grigio">
                                <asp:ListItem  Value="0" Text= "Conservazione"></asp:ListItem>
                                <asp:ListItem  Value="1" Text= "Esibizione"></asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <!-- End MEV CS 1.4 Esibizione Combo Profilo Accesso -->
                    <tr style="height: 10px">
                        <td align="right">
                            <asp:Label ID="lb_User" runat="server" CssClass="testo_grigio_scuro" Text="UserID"
                                Width="200"></asp:Label>&nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txt_userId" runat="server" CssClass="testo_grigio"></asp:TextBox>
                        </td>
                    </tr>
                    <tr style="height: 5px">
                        <td colspan="2">
                        </td>
                    </tr>
                    <tr style="height: 10px">
                        <td align="right">
                            <asp:Label ID="lb_pass" runat="server" CssClass="testo_grigio_scuro" Width="200"
                                Text="Password"></asp:Label>&nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txt_pass" runat="server" CssClass="testo_grigio" TextMode="Password"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" colspan="2">
                            <table>
                                <tr>
                                    <td align="left" class="style1">
                                        <asp:Label ID="lb_ruolo" runat="server" Text="Scelta Ruolo" Visible="False"></asp:Label>
                                    </td>
                                    <td align="left" class="testo_grigio_scuro">
                                        <asp:DropDownList ID="ddl_ruolo" runat="server" Visible="false" CssClass="testo_grigio">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="style4">
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:ImageButton ID="btn_accedi" runat="server" ImageUrl="~/Img/Butt_Accedi.jpg"
                                OnClick="btn_accedi_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lbl_error" runat="server" Text="Label" CssClass="testo_red" Visible="false"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="style2" colspan="2">
                            <asp:HiddenField ID="hd_ddlRuolicaricato" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="style2" colspan="2">
                            <asp:HiddenField ID="hf_loginResult" runat="server" />
                        </td>
                    </tr>
                    <!-- MEV cs 1.4 Esibizione -->
                    <tr>
                        <td class="style2" colspan="2">
                            <asp:HiddenField ID="hf_profiloUtente" runat="server" />
                        </td>
                    </tr>
                    <!-- End Mev cs 1.4 esibizione -->
                </table>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="testo_intestazione" height="10px">
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
