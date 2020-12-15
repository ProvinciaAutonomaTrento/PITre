<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RemotePdfStamp.aspx.cs"
    Inherits="DocsPAWA.popup.RemotePdfStamp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <base target="_self" />
    <style type="text/css">
        body
        {
            font-family: Verdana;
            overflow-x: hidden;
            overflow-y: auto;
        }
        
        .tabDoc
        {
            border-right: #e2e2e2 1px solid;
            border-top: #e2e2e2 1px solid;
            border-left: #e2e2e2 1px solid;
            border-bottom: #e2e2e2 1px solid;
            background-color: #f4f4f4;
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            font-family: Verdana;
        }
        
        .pulsante
        {
            font-weight: bold;
            font-size: 10px;
            color: #666666;
            text-indent: 0px;
            font-family: Verdana, Arial, Helvetica, sans-serif;
        }
    </style>
    <script type="text/javascript">
        function chiudi() {
            var loc2 = window.dialogArguments.document.location;
            window.dialogArguments.document.location = loc2;
            window.close();
        }

        function No() {
            var loc2 = window.dialogArguments.document.location;
            window.dialogArguments.document.location = loc2;
            window.close();
            return false;
        }

        function disablebutton() {
            var btn_ok = document.getElementById('btn_ok');
            btn_ok.disabled = true;

            var btn_close = document.getElementById('btn_close');
            btn_close.disabled = true;

            __doPostBack(btn_ok.name, '');

        }

        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="scriptMng" runat="server">
    </asp:ScriptManager>
    <div style="font-weight: normal; font-size: 13px; font-family: Arial; text-align: center;">
        <div id="mp1" style="width: 500px" class="tabDoc">
            <br />
            <br />
            <asp:UpdatePanel ID="upOK" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <div>
                        <asp:Label ID="initLabel" runat="server" Text="L'operazione creerà una nuova versione. Si desidera continuare?"></asp:Label>
                        <br />
                        <asp:Label ID="lbl_esito_op" Visible="false" runat="server"></asp:Label>
                        <br />
                        <br />
                        <asp:Button ID="btn_ok" runat="server" OnClick="doAction" OnClientClick="disablebutton(); return true;"
                            Text="Si" CssClass="pulsante" />
                        <asp:Button ID="btn_close" runat="server" OnClientClick="No()" Text="No" CssClass="pulsante" />
                        <br />
                        <asp:Button ID="btn_chiudi" runat="server" Visible="false" OnClientClick="chiudi()"
                            Text="Chiudi" CssClass="pulsante" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    </form>
</body>
</html>
