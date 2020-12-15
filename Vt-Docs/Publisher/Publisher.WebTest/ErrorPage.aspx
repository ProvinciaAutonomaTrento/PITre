<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="Publisher.WebTest.ErrorPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="text-align: center;">     
        <br />
        <p >
            <asp:Label ID="lblTitle" runat="server" Width="70%" CssClass="labelField">
                Non è stato possibile completare l'operazione richiesta per le seguenti ragioni:
            </asp:Label>
        </p>
        <asp:TextBox ID="txtUnhandledErrorMessage" runat="server" 
                    TextMode="MultiLine" Rows="10" Width="70%" ReadOnly="true" Wrap="true"
                    CssClass="textField">
        </asp:TextBox>
     </div>
    </form>
</body>
</html>
