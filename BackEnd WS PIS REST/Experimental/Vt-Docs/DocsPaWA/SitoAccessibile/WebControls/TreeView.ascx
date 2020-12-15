<%@ Control Language="c#" AutoEventWireup="false" Codebehind="TreeView.ascx.cs" Inherits="DocsPAWA.SitoAccessibile.WebControls.AccessibleTreeView" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div id="your-id" class="tvcontainer fascicoli">
	<h2 id="tvheader" runat="server">Elenco dei fascicoli</h2>
	<noscript>
		<p>Per utilizzare i pulsanti di espansione e chiusura rapida del menu &egrave;  necessario abilitare il javascript nel proprio browser</p>
	</noscript>
	<h3>
		<input type="button" class="button" onClick="javascript:ddtreemenu.flatten('treemenu1', 'expand')" value="Espandi" title="Apre tutti i livelli del menu" />
		<input type="button" class="button" onClick="javascript:ddtreemenu.flatten('treemenu1', 'contact')" value="Chiudi" title="Contrae completamente il menu" />
	</h3>
	
	<div id="divContainer" runat="server">
	</div>
</div>
