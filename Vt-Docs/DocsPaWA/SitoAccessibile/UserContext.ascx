<%@ Control Language="c#" AutoEventWireup="false" Codebehind="UserContext.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.UserContext" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
				<% if (existsLogoAmm)
       { %>
				<div id="headerLoginCustom">
				<%}
       else
       { %>
				<div id="header">
				<%} %>
<div class="w3cValidation">
	<a href="http://validator.w3.org/check?uri=referer">
		<img id="imgCss" runat="server" width="80" height="15" alt="Valido XHTML 1.0 Strict" />
	</a>
	<a href="http://jigsaw.w3.org/css-validator/check/referer">
		<img id="imgXhtml" runat="server" width="80" height="15" alt="Valido CSS" />
	</a>
</div>
<div class="userdata">
	<asp:Label id="lblCurrentUser" runat="server"></asp:Label>
	<br />
	<asp:Label id="lblCurrentRole" runat="server"></asp:Label>
</div>
				</div>
