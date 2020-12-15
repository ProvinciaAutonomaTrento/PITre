<%@ Page Language="c#" AutoEventWireup="false" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Frameset//EN">
<html>
<head>
<title>
<%    
    DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();      
    string titolo = docsPaWS.getApplicationName();
               string appTitle = "";
               if (titolo != null)
                   appTitle = titolo;
               else
                   appTitle = "DOCSPA";
 %>
<%= appTitle%> > Rubrica
</title>
<meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
</head>
<frameset rows="*" border="0">
	<frame id="_FRAME_RUBRICA"  name="_FRAME_RUBRICA" scrolling="no" src="<%
		Response.Write ("RubricaDocsPA.aspx" + Request.Url.Query);
	%>">
<noframes>

<p id="p1">
This HTML frameset displays multiple Web pages. To view this frameset, 
use a Web browser that supports HTML 4.0 and later.
</p>
</noframes>

</frameset>

</html>
