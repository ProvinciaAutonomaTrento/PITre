<%@ Page Title="" Language="C#" Inherits="VTDocs.mobile.fe.pages.GeneralPage<VTDocs.mobile.fe.model.LoginModel>" %>
<%@ Import Namespace="VTDocs.mobile.fe.utils" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" class="login">

  
            <!--PARTE IPAD-->
        <head>
          <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0" />
          <title><%= Titolo %></title>
			<link href="<%= IconImage %>" rel="apple-touch-icon" />
			
			<link media="screen" href="<%= ResolveUrl("~/Content/css/common_responsive.css")%>" rel="stylesheet" type="text/css" />
			<link media="all and (orientation:landscape)" href="<%= ResolveUrl("~/Content/css/common_responsive_landscape.css")%>" rel="stylesheet" type="text/css" />
			
			<link rel="stylesheet" media="screen" href="<%=  ResolveSkinUrl("/css/style_responsive.css")%>" />
			<link rel="stylesheet" media="all and (orientation:landscape)" href="<%=  ResolveSkinUrl("/css/style_responsive_landscape.css")%>" />
          <script type="text/javascript" src="<%= JQuery%>"></script>
		 
        </head>
        <body class="login">
        <div id="splash"><p><%= Titolo %></p></div>
		<script>
			var Immagini = new Array (<%= ViewData["Images"] %>);
			var DEVICE;
		</script>
        <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/login.js")%>"></script>
        <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/tooltip.js")%>"></script>
	
       	<div id="header_login" class="green_bar">LOGIN</div>
		<div id="wrapperLogin">
			<div id="scrollerLogin">
        <div id="parte_centrale">
			<img src="<%= LoginImage%>" />
            <p class="version"><%= MobileVersion %></p>
			<form id="vtportal_login" name="vtportal_login" method="post" action="<%= ResolveUrl("~/Login/Login")%>" onsubmit="return submit_form()">
				<%if (Html.HasErrors()){%>
				<script>
				   tooltip.init(<%= Html.GetJsonErrors() %>);
				   $('#splash').hide();	
				</script>
				<%}%>
				<table class="login_grid" cellpadding="0" cellspacing="0">
					<tr>
						<td class="left"><label for="username">User Name:</label></td>
						<td><input name="username" id="username" type="text" class="input_text" /></td>
					</tr>
					<tr>
						<td><label for="password">Password:</label></td>
						<td><input name="password" id="password" type="password" class="input_text" /></td>
					</tr>
					<tr>
						<td class="noIphone"></td><td class="iPhoneDouble"><input name="remember_password" type="checkbox" id="remember_password" /> <label for="remember_password">Salva password</label></td>
					</tr>
					<tr>
						<td class="noIphone"></td><td class="iPhoneDouble"><button type="submit" class="login_submit">ENTRA</button></td>
					</tr>
				</table>
			</form>
            
            
			<%if (!Html.HasErrors()){%>
			   <script>
				   init();
			   </script>
				<%}%>
          </div>
          <div id="load_image" class="none"></div>
		  </div>
		  </div>
         </body>
         <!--FINE PARTE IPAD-->
     
</html>
