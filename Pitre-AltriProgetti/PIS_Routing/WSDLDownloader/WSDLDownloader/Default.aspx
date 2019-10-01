<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WSDLDownloader._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <p>
            <strong style="text-align : center">WSDL DOWNLOADER</strong>
        </p>
    </div>
    <ul>
        <li>
            <asp:LinkButton ID="lbAddressBook" runat="server" onclick="Download_AddressBook_Click">AddressBook.wsdl</asp:LinkButton>
        </li>
        <li>
            <asp:LinkButton ID="lbAuthentication" runat="server" onclick="Download_Authentication_Click">Authentication.wsdl</asp:LinkButton>
        </li>
        <li>
            <asp:LinkButton ID="lbClassificationSchemes" runat="server" onclick="Download_ClassificationSchemes_Click">ClassificationSchemes.wsdl</asp:LinkButton>
        </li>
        <li>
            <asp:LinkButton ID="lbDocuments" runat="server" onclick="Download_Documents_Click">Documents.wsdl</asp:LinkButton>
        </li>
        <li>
            <asp:LinkButton ID="lbProjects" runat="server" onclick="Download_Projects_Click">Projects.wsdl</asp:LinkButton>
        </li>
        <li>
            <asp:LinkButton ID="lbRegisters" runat="server" onclick="Download_Registers_Click">Registers.wsdl</asp:LinkButton>
        </li>
        <li>
            <asp:LinkButton ID="lbRoles" runat="server" onclick="Download_Roles_Click">Roles.wsdl</asp:LinkButton>
        </li>
        <li>
            <asp:LinkButton ID="lbToken" runat="server" onclick="Download_Token_Click">Token.wsdl</asp:LinkButton>
        </li>
        <li>
            <asp:LinkButton ID="lbTransmissions" runat="server" onclick="Download_Transmissions_Click">Transmissions.wsdl</asp:LinkButton>
        </li>
    </ul>
    </form>
</body>
</html>
