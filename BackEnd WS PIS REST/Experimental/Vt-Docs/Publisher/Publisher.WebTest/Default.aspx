<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Publisher.WebTest._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div align="center">
        <asp:LinkButton ID="btnPublishers" runat="server" Text="Istanze di pubblicazione" OnClick="btnPublishers_Click" />
        <br />
        <br />
        <asp:LinkButton ID="btnSubscribers" runat="server" Text="Sottoscrittori" OnClick="btnSubscribers_Click" />
    </div>
    </form>
</body>
</html>