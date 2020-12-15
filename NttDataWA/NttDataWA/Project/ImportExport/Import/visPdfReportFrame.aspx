<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="visPdfReportFrame.aspx.cs" Inherits="NttDataWA.Project.ImportExport.Import.visPdfReportFrame" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/jquery-1.8.1.min.js") %>" type="text/javascript"></script>
    <script src="<%=Page.ResolveClientUrl("~/Scripts/Functions.js") %>" type="text/javascript"></script>
        <script type="text/javascript">
            showPopupContent();
    </script>
</head>
<body>
    <form id="form1" runat="server">
          <iframe id="iframeVisUnificata" src="visPdfReport.aspx" style="width:100%; height:100%;" scrolling="no" frameborder="0" runat="server"></iframe>      
    </form>
</body>
</html>
