<%@ Page language="c#" AutoEventWireup="false"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Frameset//EN">
<html>
<head>
<title>Stampa report</title>
<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">


</head>
<body>

<iframe id="_FRAME_REPORT"  name="_FRAME_REPORT" scrolling="no" src="<%
Response.Write ("VisualReport.aspx" + Request.Url.Query);%>" width="100%" height="100%" />


</body>


</html>
