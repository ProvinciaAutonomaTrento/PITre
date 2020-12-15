<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="modalVisualizzaDocPdf.aspx.cs" Inherits="DocsPAWA.popup.modalVisualizzaDocPdf" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<!--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">-->

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Visualizza Etichetta PDF</title>
</head>
<body>
<iframe src="<%Response.Write ("visualizzaDocPdf.aspx");%>" width="100%" height="100%" />
</body>
</html>