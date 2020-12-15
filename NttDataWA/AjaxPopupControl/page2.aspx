<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="page2.aspx.cs" Inherits="AjaxPopupControl.WebForm2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <base target="_self" />
    <title></title>
	<script src="http://code.jquery.com/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-1.8.23.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/FDB_tools.js" type="text/javascript"></script>
    <link href="css/flick/jquery-ui-1.8.23.custom.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <p>(ANSA) - PARIGI, 10 SET - Straordinario passo in avanti nel quotidiano menage delle pecore dei Vosgi, nell'est della Francia, che dall'anno prossimo saranno attrezzate con collari speciali che emetteranno un sms all'accelerare del battito cardiaco. Una circostanza che, per gli ovini, si concretizza quasi sempre all'apparire di un lupo all'orizzonte. Il cardiofrequenzimetro si e' reso necessario dopo che quest'anno 165 pecore sono morte in ben 48 assalti di lupi generando esasperazione fra gli allevatori.</p>
        <input type="button" value="Chiudi e imposta il valore di ritorno a 1" onclick="closeAjaxModal('<%=Request.QueryString["popupid"] %>', '1')" />
        <input type="button" value="Chiudi e imposta il valore di ritorno a 2" onclick="closeAjaxModal('<%=Request.QueryString["popupid"] %>', '2')" />
        <input type="button" value="Chiudi senza modificare il valore di ritorno" onclick="closeAjaxModal('<%=Request.QueryString["popupid"] %>')" />
    </div>
    </form>
</body>
</html>
