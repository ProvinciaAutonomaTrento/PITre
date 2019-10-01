<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VisualizzaOggetto.aspx.cs" Inherits="NttDataWA.VisualizzaOggetto" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
   <script type="text/javascript">
       var tipoOggetto = window.location.search.indexOf('tipoOggetto=D') > 0 ? 'record' : 'project';
       document.location = 'CheckInOut/OpenDirectLink.aspx' + window.location.search + '&from=' + tipoOggetto;
</script>
</body>
</html>
