<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SessionAbort.aspx.cs" Inherits="NttDataWA.SessionAbort" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" id="Html" runat="server">
<head id="Head1" runat="server">
    <title>Pagina sessione scaduta</title>
    <link runat="server" type="text/css" rel="stylesheet" id="CssLayout" href="~/Css/Left/Login.css" />
    <script language="JavaScript" type="text/javascript">
        if (window.addEventListener) {  // all browsers except IE before version 9
            window.addEventListener("beforeunload", OnBeforeUnLoad, false);
        }
        else {
            if (window.attachEvent) {   // IE before version 9
                window.attachEvent("onbeforeunload", OnBeforeUnLoad);
            }
        }

        function OnBeforeUnLoad(my_event) 
        {
            if (my_event) {
                if (parent.frames[1].document.getElementById('user_id') != null)
                    parent.frames[1].document.getElementById('user_id').value = "";
            }
        }
    </script>        
</head>
<body>
    <form id="FrmSessionAbort" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="3600"
        EnablePartialRendering="true">
        <Scripts>
            <asp:ScriptReference Path="~/Scripts/webkit.js" />
        </Scripts>
    </asp:ScriptManager>
    <div id="containerLogin">
        <asp:UpdatePanel ID="UpPnlLogin" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="UpPnlLoginLeft">
                  <asp:Image ImageUrl="Images/Common/logo_login.jpg" runat="server" ID="img_logo_login" AlternateText="" />
                </div>
                <div id="UpPnlLoginRight">
                    <div class="Space">
                    </div>
                    <div id="loginAccedi">
                        <div class="loginAccediItem">
                            <div class="ErrorAbortItemCx">
                                <div class="messager_c1">
                                    <img src="Images/Common/messager_warning.gif" alt="" /></div>
                                <div class="messager_c2">
                                    <span>
                                        <asp:Label ID="LblErrorIta" runat="server" CssClass="lbl_login_error"></asp:Label></span></div>
                                <div class="messager_c3">
                                    <img src="Images/Common/messager_warning.gif" alt="" />
                                </div>
                            </div>
                        </div>
                        <fieldset>                            
                            <asp:Button runat="server" CssClass="btnEnable" ID="BtnLogin" onclick="BtnLogin_Click" />
                        </fieldset>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
