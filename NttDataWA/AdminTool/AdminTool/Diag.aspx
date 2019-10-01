<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Diag.aspx.cs" Inherits="SAAdminTool.AdminTool.Diag" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>

        
        <p><h1>Informazioni Backend</h1></p>
        <p><h2>Generali</h2></p>
        <asp:Label ID="InfoBackend" runat="server" Text="Backend"></asp:Label>
        
        <p><h2>Impostazioni DB</h2></p>
        <asp:Label ID="ImpostazioniDB" runat="server" Text="Backend"></asp:Label>
        
        <p><h2>Impostazioni Documentale</h2></p>
        <asp:Label ID="ImpostazioniDoc" runat="server" Text="Backend"></asp:Label>
        


        <p><h2>Url di Connessioni Varie</h2></p>
        <asp:Label ID="ImpostazioniUrl" runat="server" Text="Backend"></asp:Label>
        <hr/>
        <br />
        <p><h1>Informazioni Frontend</h1></p>
         <p><h2>Generali</h2></p>
        <asp:Label ID="InfoFrontend" runat="server" Text="Backend"></asp:Label>
        <hr/>
        <p><h1>Link</h1></p>
        <p>
         <asp:HyperLink ID="LinkAdmin" runat="server">Login Amministrazione</asp:HyperLink>
         </p>
         <p>
         <asp:HyperLink ID="LinkLogin" runat="server">Login Sistema Gestione Documentale</asp:HyperLink>
         </p>

    </div>
    </form>
</body>
</html>

