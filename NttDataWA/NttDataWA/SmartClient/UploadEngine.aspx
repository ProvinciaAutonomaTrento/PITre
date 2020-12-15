<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadEngine.aspx.cs" EnableSessionState="ReadOnly" Async="true" Inherits="NttDataWA.SmartClient.UploadEngine"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link href="../Css/FSOFileUploader.css" rel="Stylesheet" type="text/css" />
</head>
<body>
    <form id="form" runat="server" enctype="multipart/form-data">
        <asp:ScriptManager ID="scriptManager" runat="server" />
        <script type="text/javascript">
            function pageLoad(sender, args) {
                window.parent.register(
                    $get('<%= this.form.ClientID %>'),
                    $get('<%= this.fileUpload.ClientID %>')
                );
            }
        </script>
        <asp:FileUpload ID="fileUpload" runat="server" style="width: 100%;" onchange="parent.onUploadClick();" />
    </form>
</body>
</html>
