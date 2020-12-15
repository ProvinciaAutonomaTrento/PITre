<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="NttDataWA.ErrorPage" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" id="Html" runat="server">
<head id="Head1" runat="server">
    <title>Pagina Errore</title>   
    <link runat="server" type="text/css" rel="stylesheet" id="CssLayout" href="~/Css/servicepage.css" />
    <script src="Scripts/jquery-1.8.1.min.js" type="text/javascript"></script>
    <script src="Scripts/Functions.js" type="text/javascript"></script>
    <script type="text/javascript">
        <asp:PlaceHolder id="plcPopup2" runat="server" visible="false">showPopupContent();</asp:PlaceHolder>

        function open_win() {
            <asp:PlaceHolder id="plcPopup" runat="server" visible="false">parent.$(".ui-dialog-content").dialog("close");</asp:PlaceHolder>
            <asp:PlaceHolder id="plcBase" runat="server" visible="false">document.location = 'index.aspx';</asp:PlaceHolder>
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
    <div id="ContainerPageError">
        <asp:UpdatePanel ID="UpPnlError" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <fieldset>                    
                    <div class="messager">
                        <div class="messager_c1">
                            <img src="Images/Common/messager_error.png" alt="" /></div>
                        <div class="messager_c2">
                            <span>
                                <asp:Label ID="lblTxt" runat="server" CssClass="lblTxt"></asp:Label></span></div>
                        <div class="messager_c3">
                            <img src="Images/Common/messager_error.png" alt="" /></div>
                    </div>
                    <div>
                        <asp:TextBox ID="TxtBoxError" runat="server" Width="100%" TextMode="MultiLine" CssClass=""
                            BorderStyle="None">
                        </asp:TextBox>
                    </div>
                    <div id="btnLogin">
                        <asp:Button runat="server" CssClass="btnEnable" ID="BtnLogin" OnClientClick="open_win()" />
                    </div>
                    <div class="Space">
                    </div>
                </fieldset>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
