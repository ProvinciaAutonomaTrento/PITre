<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestPass.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_Password.GestPass" %>

<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider"
    TagPrefix="uct" %>
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

        function ValidateNumericKey()
		    {
			    var inputKey=event.keyCode;
			    var returnCode=true;
			    if(inputKey > 47 && inputKey < 58)
				    return;
			    else
			    {
				    returnCode=false; 
				    event.keyCode=0;
			    }
			
			    event.returnValue = returnCode;
			}

	    function OnClickExpireAllPassword() {
			return window.confirm("Attenzione:\nl'operazione renderà invalide le password degli utenti dell'amministrazione." +
			                           "\nAl prossimo login a DocsPa, ogni utente dovrà inserire una nuova password diversa dalla precedente.\nContinuare?");

            }

        function SavePasswordConfigCompleted() {
            alert("Sono state modificate le regole per la gestione delle password.\nPer farle applicare immediatamente a tutti gli utenti occorre invalidare le password con l'apposito comando.");
            }
			
    </script>
</head>
<body bottommargin="0" leftmargin="0" topmargin="0" bgcolor="#f6f4f4" height="100%">
    <form id="form1" runat="server">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName="AMMINISTRAZIONE > Gestione Password" />
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
                <asp:Label ID="lbl_position" TabIndex="25" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <!-- TITOLO PAGINA -->
            <td class="titolo" align="center" bgcolor="#e0e0e0" height="20">
                Gestione Password
            </td>
        </tr>
        <tr>
            <td valign="top" align="center" bgcolor="#f6f4f4" height="100%">
                <table cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <td align="center" style="width: 930px; height: 10px">
                            <asp:Label ID="lbl_avviso" TabIndex="23" runat="server" CssClass="testo_rosso"></asp:Label>
                        </td>
                    </tr>
                    <tr id="tblRowPasswordMng" runat="server">
                        <td align="center" style="width: 930px;">
                            
                            <table cellspacing="1" cellpadding="0" width="100%" border="0" class="contenitore">
                                <tr style="height: 15px">
                                    <td class="titolo_pnl" align="left" valign="middle">
                                        <asp:Label ID="lbl_titolo_pnl" runat="server">Dettagli</asp:Label>
                                    </td>
                                </tr>
                                <tr valign="middle">
                                    <td colspan="2" align="center" style="height: 21px; width: auto;" valign="middle">
                                        <asp:Label ID="lblMinLength" runat="server" CssClass="testo_grigio_scuro_grande"
                                            Text="Lunghezza min.:"></asp:Label>
                                        <asp:TextBox ID="txtPasswordMinLength" runat="server" CssClass="testo_grigio_scuro_grande"
                                            Width="30px" onkeypress="ValidateNumericKey();"></asp:TextBox>
                                        &nbsp;
                                        <asp:Label ID="lblSpecialChars" runat="server" CssClass="testo_grigio_scuro_grande"
                                            Text="Caratteri speciali:"></asp:Label>
                                        <asp:TextBox ID="txtPasswordSpecialChars" runat="server" CssClass="testo_grigio_scuro_grande"
                                            Width="210px"></asp:TextBox>
                                        &nbsp;
                                        <asp:Label ID="lblPasswordDescription" runat="server" CssClass="testo_grigio_scuro_grande"
                                            Text="Validità (gg):"></asp:Label>
                                        <asp:TextBox ID="txtPasswordValidityDays" runat="server" CssClass="testo_grigio_scuro_grande"
                                            Width="30px" onkeypress="ValidateNumericKey();"></asp:TextBox>&nbsp;
                                        <asp:Button ID="btnExpireAll" runat="server" Text="Invalida tutte" OnClick="btnExpireAll_Click"
                                            CssClass="testo_btn" Width="90px" />
                                    </td>
                                </tr>
                                
                            </table>
                            <table>
                                <tr>
                                    <td align="right" >
                                        <asp:Button ID="btnSave" runat="server" Text="Salva config. password" CssClass="testo_btn"
                                            OnClick="btnSave_Click" Width="95%" Font-Bold="true"/>&nbsp;
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
