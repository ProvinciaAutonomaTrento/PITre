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
                        <asp:Panel ID="PnlLoginAccedi" runat="server">
                            <div id="loginAccedi">
                                <div class="loginAccediItem">
                                    <div class="loginAccediItemCx">
                                        <p id="debug" runat="server" visible="false" />
                                        <asp:Label ID="LblError" runat="server" Visible="false" CssClass="lbl_login_error"></asp:Label>
                                        <asp:Label ID="LblMessage" runat="server" Visible="false" CssClass="lbl_login_msg"></asp:Label>
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
                                        <asp:Panel ID="PnlResetPassword" runat="server" Visible="false">
                                            <h6>
                                                <asp:LinkButton ID="LnkResetPassword" OnClick="LnkResetPassword_Click" Font-Size="1.2em" CssClass="lbl_login" runat="server"></asp:LinkButton>																   
                                            </h6>
                                        </asp:Panel>							  
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
                        </asp:Panel>
                        <asp:Panel ID="PnlResetPasswordUtente" runat="server" Visible="false">
                            <div id="loginResetPassword">
                                <div class="loginAccediItem">
                                    <div class="loginAccediItemCx">
                                        <p id="P1" runat="server" visible="false" />
                                        <asp:Label ID="LblResetPassword" runat="server" CssClass="lbl_login_msg"></asp:Label>
                                        <p>
                                        <asp:Label ID="LblErrorResetPassword" runat="server" Visible="false" CssClass="lbl_login_error"></asp:Label></p>
                                    </div>																														   																							
                                </div>
                                <div class="loginAccediItem">
                                    <asp:UpdatePanel runat="server" ID="UpPnlDllAdmin_Reset" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:Panel ID="pnl_ddlAmm_Reset" Visible="False" runat="server">
                                                <p>
                                                    <asp:DropDownList ID="ddl_Amministrazioni_reset" runat="server" CssClass="chzn-select-deselect" Width="430">
                                                    </asp:DropDownList>
                                                </p>
                                            </asp:Panel>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>	
                                </div>
                                <div class="loginAccediItem"  id="loginResetPasswordUserId" runat="server">
                                    <div class="loginAccediItemSx">
                                        <asp:Label runat="server" ID="LblResetPwUserId" CssClass="lbl_login"></asp:Label>									 										
                                    </div>
                                    <div class="loginAccediItemDx">
                                        <cc2:CustomTextArea ID="CtxResetPwUserId" runat="server" CssClass="txt_login" CssClassReadOnly="txt_login_readonly"></cc2:CustomTextArea>
                                    </div>
                                </div>
                                <div class="loginAccediItem">
                                    <div class="loginAccediItemSx">
                                        <asp:Label runat="server" ID="LblOTP" CssClass="lbl_login" Visible="false"></asp:Label>									 										
                                    </div>
                                    <div class="loginAccediItemDx">
                                        <cc2:CustomTextArea ID="CtxOTP" runat="server" CssClass="txt_login"  Visible="false"
                                              TextMode="Password" CssClassReadOnly="txt_login_readonly"></cc2:CustomTextArea>
                                    </div>
                                </div>
                                <div class="loginAccediItem">
                                    <div class="loginAccediItemSx">
                                        <asp:Label runat="server" ID="LblNuovaPassword" CssClass="lbl_login"  Visible="false"></asp:Label>									 										
                                    </div>
                                    <div class="loginAccediItemDx">
                                        <cc2:CustomTextArea ID="CtxNuovaPassword" runat="server" CssClass="txt_login"  Visible="false"
                                              TextMode="Password" CssClassReadOnly="txt_login_readonly"></cc2:CustomTextArea>
                                    </div>
                                </div>
                                <div class="loginAccediItem">
                                    <div class="loginAccediItemSx">
                                        <asp:Label runat="server" ID="LblConfermaPassword" CssClass="lbl_login"  Visible="false"></asp:Label>									 										
                                    </div>
                                    <div class="loginAccediItemDx">
                                        <cc2:CustomTextArea ID="CtxConfermaPassword" runat="server" CssClass="txt_login" Visible="false"
                                              TextMode="Password" CssClassReadOnly="txt_login_readonly"></cc2:CustomTextArea>
                                    </div>
                                </div>
                                <fieldset>
                                    <cc2:CustomButton ID="BtnResetPasswordInviaOTP" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                                        OnMouseOver="btnHover" OnClick="BtnResetPasswordInviaOTP_Click" />
                                    <cc2:CustomButton ID="BtnResetPasswordConfirm" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                                        OnMouseOver="btnHover" OnClick="BtnResetPasswordConfirm_Click" Visible="false" />
                                    <cc2:CustomButton ID="BtnResetPasswordCancel" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                                        OnMouseOver="btnHover" OnClick="BtnResetPasswordCancel_Click" />
                                </fieldset>
                            </div>
                        </asp:Panel>  
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