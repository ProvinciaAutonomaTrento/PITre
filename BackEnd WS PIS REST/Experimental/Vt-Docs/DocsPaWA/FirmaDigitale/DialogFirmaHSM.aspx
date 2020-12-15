<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DialogFirmaHSM.aspx.cs"
    Inherits="DocsPAWA.FirmaDigitale.DialogFirmaHSM" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>
        <%=GetMaskTitle()%>
    </title>
    <link href="../CSS/docspa_30.css" type="text/css" rel="stylesheet" />
    <script src="../LIBRERIE/jquery-1.8.3.js" type="text/javascript" />
    <script type="text/javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
    <base target="_self" />
    <style type="text/css">
        #divPopUp
        {
            border: 2px black solid;
            background-color: blue;
            width: 200px;
            height: 200px;
        }
        .titoloscheda_hsm1
        {
            font-size: small;
            font-weight: bold;
            color: #ff0000;
            font-family: Verdana;
        }
    </style>
    <script type="text/javascript">
        // Javascript Functions
        function CloseWindow() {
            window.returnValue = true;
            window.close();
        }

        function showModalWait() {
            $find("mdlWait").show();
            var btn_firma = document.getElementById('btnFirma');
            btn_firma.disabled = true;
            __doPostBack(btn_firma.name, '');

            return true;
        }

        function hideModalWait() {
            $find("mdlWait").hide();
            return true;
        }
    </script>
</head>
<body>
    <form id="frmDialogFirmaHSM" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <table class="info_grigio" width="500px" cellpadding="1" align="center" cellspacing="10">
            <tr>
                <td class="titolo_scheda" align="center" colspan="10">
                    <asp:Label ID="lblTitle" runat="server" Text="Firma Remota HSM"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="titolo_scheda" align="center">
                    <table cellspacing="5" cellpadding="2">
                        <tr>
                            <td>
                                <asp:Label ID="lblUserName" runat="server" Text="UserName"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtUseerName" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblPassword" runat="server" Text="Password"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" Columns="21"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="titolo_scheda" align="center">
                    <table cellspacing="5" cellpadding="2">
                        <tr>
                            <td>
                                <asp:Label ID="lblDominio" runat="server" Text="Dominio"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtDominio" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblOTP" runat="server" Text="OTP"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtOTP" runat="server" TextMode="Password" Columns="21"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="titolo_scheda">
                    <!--<asp:Panel ID="pnlTipoFirma" runat="server">-->
                    <asp:UpdatePanel ID="upTipoFirma" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="titolo_scheda">
                                <asp:RadioButton ID="rbPades" GroupName="grpTipoFirma" CssClass="testo_grigio" runat="server"
                                    Text="PDF (PADES)" OnCheckedChanged="HsmrbPades_Change" AutoPostBack="true" />&nbsp;&nbsp;
                                <asp:RadioButton ID="rbCades" GroupName="grpTipoFirma" CssClass="testo_grigio" runat="server"
                                    Text="P7M (CADES)" Checked="true" OnCheckedChanged="HsmrbP7M_Change" AutoPostBack="true" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <!--</asp:Panel>-->
                </td>
            </tr>
            <tr>
                <td class="titolo_scheda">
                    <asp:UpdatePanel ID="upChConverti" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="titolo_scheda">
                                <asp:CheckBox ID="chkConverti" runat="server" Text="Converti in pdf" Checked="false"
                                    Visible="false" CssClass="testo_grigio" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="titolo_scheda">
                    <!--<asp:Panel ID="pnlFirma_CoFirma" runat="server">-->
                    <asp:UpdatePanel ID="uprbFirmaCoF" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="titolo_scheda">
                                <asp:RadioButton ID="rbFirma" GroupName="grpFirma_CoFirma" CssClass="testo_grigio"
                                    runat="server" Text="Firma" Checked="true" />&nbsp;&nbsp;
                                <asp:RadioButton ID="rbCofirma" GroupName="grpFirma_CoFirma" CssClass="testo_grigio"
                                    runat="server" Text="Cofirma" />
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <!--</asp:Panel>-->
                </td>
            </tr>
            <tr>
                <td class="titolo_scheda" align="center" colspan="10">
                    <%--<asp:UpdatePanel ID="upbtn" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="titolo_scheda">--%>
                    <asp:Button ID="btnFirma" runat="server" Text="Firma" CssClass="pulsante69" OnClick="btnFirma_OnClick"
                        OnClientClick="return showModalWait();"></asp:Button>
                    &nbsp;
                    <asp:Button ID="btnChiudi" runat="server" Text="Chiudi" OnClientClick="CloseWindow()"
                        CssClass="pulsante69"></asp:Button>
                    <%-- </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>--%>
                </td>
            </tr>
            <tr>
                <td class="titoloscheda_hsm1" align="center" colspan="10">
                    <asp:UpdatePanel ID="upUserMess" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="titoloscheda_hsm1">
                                <asp:Label ID="lblUserMess" runat="server" Visible="false"></asp:Label>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
        <!-- PopUp Wait-->
        <cc2:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
            BackgroundCssClass="modalBackground" BehaviorID="mdlWait" />
        <div id="Wait" runat="server" style="display: none; font-weight: normal; font-size: 13px;
            font-family: Arial; text-align: center;">
            <asp:UpdatePanel ID="pnlUP" runat="server">
                <ContentTemplate>
                    <div class="modalPopup">
                        <asp:Label ID="lblInfo" runat="server">Attendere prego...</asp:Label>
                        <br />
                        <img id="imgLoading" src="../images/loading.gif" style="border-width: 0px;" alt="Attendere prego" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    </form>
</body>
</html>
