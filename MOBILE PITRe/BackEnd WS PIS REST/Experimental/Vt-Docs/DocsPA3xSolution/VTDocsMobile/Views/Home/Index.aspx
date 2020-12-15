<%@ Page Title="" Language="C#" Inherits="VTDocs.mobile.fe.pages.GeneralPage<VTDocs.mobile.fe.model.MainModel>" %>
<%@ Import Namespace="VTDocs.mobile.fe.model" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

    <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<%--        <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0" />--%>
        <meta name="viewport" content="user-scalable=yes, width=device-width, initial-scale=1">
        
        <title><%=Titolo %></title>
		
		<link media="screen" href="<%= ResolveUrl("~/Content/css/common_responsive.css")%>" rel="stylesheet" type="text/css" />
		<!--<link media="all and (orientation:landscape)" href="<%= ResolveUrl("~/Content/css/common_responsive_landscape.css")%>" rel="stylesheet" type="text/css" />-->
			
		<link rel="stylesheet" media="screen" href="<%=  ResolveSkinUrl("/css/style_responsive.css")%>" />
		<link href="<%= IconImage %>" rel="apple-touch-icon" />
        <script type="text/javascript" src="<%= JQuery%>"></script>
		
    </head>
    <body>
      <vtdm:MobileClientIPad id="MobileClientIPad" runat="server"/>
    </body>

</html>