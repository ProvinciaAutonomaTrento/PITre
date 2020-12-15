<%@ Page language="c#" AutoEventWireup="false"%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Frameset//EN">
<html>
  <head>
    <title>
<%                 DocsPAWA.DocsPaWR.DocsPaWebService docsPaWS = new DocsPAWA.DocsPaWR.DocsPaWebService();
                   string titolo = docsPaWS.getApplicationName();
                   string appTitle = "";
               if (titolo != null)
                   appTitle = titolo;
               else
                   appTitle = "DOCSPA";
 %>
<%= appTitle%> > Modelli trasmissione</title>

    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
  </head>

<body>

<iframe id="_FRAME_REPORT"  name="_FRAME_REPORT" scrolling="auto" src="<%Response.Write ("GestioneModelliTrasm.aspx" + Request.Url.Query);%>" width="100%" height="100%"  frameborder=0/>


</body>


</html>

