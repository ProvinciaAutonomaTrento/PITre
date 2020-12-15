<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="_html_data.aspx.cs" Inherits="NttDataWA.Project._html_data" %>
<asp:Literal id="step1" runat="server" visible="false">
	<ul>
		<li id="rootnode_1" class="jstree-closed">
			<a href="#">7.2.1-2012-31</a>
		</li>
		<li id="rootnode_2" class="jstree-closed">
			<a href="#">7.2.1-2012-32</a>
		</li>
	</ul>
</asp:Literal>
<asp:Literal id="step2" runat="server" visible="false">
			<ul>
				<li id="childnode_1_1">
					<a href="#">Corrispondenza</a>
				</li>
				<li id="childnode_1_2">
					<a href="#">Atti</a>
				</li>
				<li id="childnode_1_3">
					<a href="#">Produzioni</a>
				</li>
			</ul>
</asp:Literal>
<asp:Literal id="step3" runat="server" visible="false">
			<ul>
				<li id="childnode_2_1" class="jstree-closed">
					<a href="#">Atti</a>
				</li>
			</ul>
</asp:Literal>
<asp:Literal id="step4" runat="server" visible="false">
			<ul>
				<li id="childnode_2_1_1">
					<a href="#">Atto 1</a>
				</li>
				<li id="childnode_2_1_2">
					<a href="#">Atto 2</a>
				</li>
			</ul>
</asp:Literal>