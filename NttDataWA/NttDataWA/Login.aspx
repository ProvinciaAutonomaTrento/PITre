<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="NttDataWA.Login" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" id="Html" runat="server">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
    <meta http-equiv="cache-control" content="max-age=0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
    <meta http-equiv="pragma" content="no-cache" />
    <title>Login</title>
    <script language="javaScript" type="text/javascript" src="Scripts/Functions.js"></script>
    <script src="Scripts/jquery-1.8.1.min.js" type="text/javascript"></script>
    <script language="javaScript" type="text/javascript">
        if (!parent.frames.fra_sessionend) {
            location.href = "./login.htm";
        }

        function sessionend(userid) {
            var frs = parent.frames.fra_sessionend;
            if (frs.document.getElementById('user_id') != null)
                frs.document.getElementById('user_id').value = userid;
        }
    </script>
    <link runat="server" type="text/css" rel="stylesheet" id="CssLayout" />

    <script src="Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/Functions.js" type="text/javascript"></script>
</head>
<body>
    <form id="FrmLogin" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="3600"
            EnablePartialRendering="true">
            <Scripts>
                <asp:ScriptReference Path="~/Scripts/webkit.js" />
            </Scripts>
        </asp:ScriptManager>
        <script language="javascript" type="text/javascript">
            Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(beginRequest);
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(endRequest);
        </script>

        <!--Messaggio di avviso-->
        <div id="msgLoginDiv" class="msgLogin" runat="server">
            <b>AVVISO: </b><br />
            <asp:Label ID="msgLoginText" runat="server" Text=""></asp:Label><br />
        </div>
        <!--Fine Messaggio di avviso -->

        <div id="containerLogin">
            <asp:UpdatePanel ID="UpPnlLogin" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField runat="server" ID="HiForcerLogin" ClientIDMode="Static" />
                    <div id="UpPnlLoginLeft">
                        <asp:Image ImageUrl="Images/Common/logo_login.jpg" runat="server" ID="img_logo_login"
                            AlternateText="" />
                        <%--<img src="Images/Common/logo_login.jpg" alt="" />--%>
                    </div>
                    <div id="UpPnlLoginRight">
                        <asp:Panel ID="PnlMultiLanguage" runat="server">
                            <asp:Menu ID="MenuLanguages" runat="server" Orientation="Horizontal" IncludeStyleBlock="true"
                                OnMenuItemClick="MenuLanguages_Click" Visible="false">
                            </asp:Menu>
                        </asp:Panel>
                        <div id="loginAccedi">
                            <div class="loginAccediItem">
                                <div class="loginAccediItemCx">
                                    <p id="debug" runat="server" visible="false" />
                                    <asp:Label ID="LblError" runat="server" Visible="false" CssClass="lbl_login_error"></asp:Label>
                                </div>
                                <asp:UpdatePanel runat="server" ID="UpPnlDllAdmin" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnl_ddlAmm" Visible="False" runat="server">
                                            <p>
                                                <asp:DropDownList ID="ddl_Amministrazioni" runat="server" CssClass="chzn-select-deselect" Width="430">
                                                </asp:DropDownList>
                                            </p>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <div class="loginAccediItem">
                                <div class="loginAccediItemSx">
                                    <asp:Label runat="server" ID="LogInLblUserId" CssClass="lbl_login"></asp:Label>
                                </div>
                                <div class="loginAccediItemDx">
                                    <cc2:CustomTextArea ID="TxtUserId" runat="server" CssClass="txt_login" CssClassReadOnly="txt_login_readonly"></cc2:CustomTextArea>
                                </div>
                            </div>
                            <div class="loginAccediItem">
                                <div class="loginAccediItemSx">
                                    <asp:Label runat="server" ID="LogInLblPassword" CssClass="lbl_login"></asp:Label>
                                </div>
                                <div class="loginAccediItemDx">
                                    <cc2:CustomTextArea ID="TxtPassword" CssClass="txt_login" CssClassReadOnly="txt_login_readonly"
                                        runat="server" TextMode="Password"></cc2:CustomTextArea>
                                </div>
                            </div>
                            <div class="loginAccediItem">
                                <div class="loginAccediItemSx">
                                    <asp:Label runat="server" ID="LogInLblConfirmPassword" Visible="false" CssClass="lbl_login"></asp:Label>
                                </div>
                                <div class="loginAccediItemDx">
                                    <cc2:CustomTextArea ID="TxtConfirmPassword" CssClass="txt_login" CssClassReadOnly="txt_login_readonly"
                                        runat="server" Visible="false" TextMode="Password"></cc2:CustomTextArea>
                                </div>
                            </div>
                            <fieldset>
                                <cc2:CustomButton ID="LoginBtnLogin" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                                    OnMouseOver="btnHover" OnClick="BtnLogin_Click" />
                            </fieldset>
                            <h4><asp:LinkButton runat="server" ID="LogInLinkAccessibleVersion" Visible="false"></asp:LinkButton></h4>
                            <!--[if lte IE 9 ]>
                              <p style="margin: 0px 20px; top: 25px; position: absolute; font-weight:bold;">La versione di Internet Explorer installata su questo PC non supporta le funzioni di acquisizione massiva di file in PITre introdotte dalla versione 3.2.15 in data 30-05-2019</p>
                            <![endif]-->
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <!-- PopUp Wait-->
        <uc1:ModalPopupExtender ID="mdlPopupWait" runat="server" TargetControlID="Wait" PopupControlID="Wait"
            BackgroundCssClass="modalBackground" BehaviorID="mdlWait" />
        <div id="Wait" runat="server" class="wait">
            <asp:UpdatePanel ID="pnlUP" runat="server">
                <ContentTemplate>
                    <div class="modalPopup">
                        <asp:Label ID="Loading" runat="server" Visible="false"></asp:Label>
                        <br />
                        <img id="imgLoading" src="Images/common/loading.gif" alt="" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
    <script type="text/javascript">         $(".chzn-select-deselect").chosen({
             allow_single_deselect: true, no_results_text: "Nessun risultato trovato"
         }); $(".chzn-select").chosen({
             no_results_text: "Nessun risultato trovato"
         }); </script>
</body>
</html>

