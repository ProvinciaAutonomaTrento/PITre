<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckPage.aspx.cs" Inherits="NttDataWA.CheckPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Label ID="lbl_msg" runat="server" ></asp:Label><br /><br />
    <asp:Button ID="btnRefreshChiaviConfig" Text="Refresh FE-BE Config keys" 
            runat="server" onclick="btnRefreshChiaviConfig_Click" /> <br /><br />
            <asp:Label ID="lbl_esito_refresh_chiavi" runat="server"></asp:Label>
    
    </div>
           <div>
    <asp:Label ID="lbl_msg2" runat="server" ></asp:Label><br /><br />
    <asp:Button ID="btn_RefreshQlistandBEKeys" Text="Refresh QL And BE Config keys and Grids" 
            runat="server"  OnClick="btn_RefreshQlistandBEKeys_Click"/> <br /><br />
            <asp:Label ID="Label2" runat="server"></asp:Label>
    
    </div>
    </form>
</body>
</html>
