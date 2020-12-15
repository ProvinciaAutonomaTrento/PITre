<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ListPagingNavigationControls.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.Paging.ListPagingNavigationControls" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div class="pager">
	<asp:Label id="lblPagingDescription" Runat="server"></asp:Label>
	<ul>
		<li>
			<asp:Button id="btnMovePrevious" runat="server" Text="Precedente" CssClass="previousPage" Tooltip="Vai alla pagina precedente"></asp:Button>
		</li>
		<li>
			<asp:Button id="btnMoveNext" runat="server" Text="Successiva" CssClass="nextPage" Tooltip="Vai alla pagina successiva"></asp:Button>
		</li>
		<li runat="server">
			<asp:Button id="btnPreviousGroup" runat="server" Text="..."></asp:Button>
		</li>
		<li runat="server">
			<asp:Button id="btn_1" runat="server"></asp:Button>
		</li>
		<li runat="server">
			<asp:Button id="btn_2" runat="server"></asp:Button>
		</li>
		<li runat="server">
			<asp:Button id="btn_3" runat="server"></asp:Button>
		</li>
		<li runat="server">
			<asp:Button id="btn_4" runat="server"></asp:Button>
		</li>
		<li runat="server">
			<asp:Button id="btn_5" runat="server"></asp:Button>
		</li>
		<li runat="server">
			<asp:Button id="btn_6" runat="server"></asp:Button>
		</li>
		<li runat="server">
			<asp:Button id="btn_7" runat="server"></asp:Button>
		</li>
		<li runat="server">
			<asp:Button id="btn_8" runat="server"></asp:Button>
		</li>
		<li runat="server">
			<asp:Button id="btn_9" runat="server"></asp:Button>
		</li>
		<li runat="server">
			<asp:Button id="btn_10" runat="server"></asp:Button>
		</li>
		<li runat="server">
			<asp:Button id="btnNextGroup" runat="server" Text="..."></asp:Button>
		</li>
	</ul>
</div>