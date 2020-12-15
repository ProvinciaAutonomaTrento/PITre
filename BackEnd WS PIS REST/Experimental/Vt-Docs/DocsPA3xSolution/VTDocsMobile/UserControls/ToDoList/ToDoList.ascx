<%@ Control Language="C#" CodeBehind="~/VTDocs/mobile/fe/usercontrols/ToDoListUserControl.cs"  Inherits="VTDocs.mobile.fe.usercontrols.ToDoListUserControl"%>
<%@ Import Namespace="VTDocs.mobile.fe.wsrefs.VTDocsWSMobile" %>

<a href="/VTDocsMobile/Documento/Dettaglio/46041">TEST DOC</a>
<div id="elenco" class="elenco">
<%foreach (ToDoListElement el in Model.Elements){ 
  if(!string.IsNullOrEmpty(el.IdDoc)){%>
<div class="item" id="<%: el.IdDoc %>">
	<div class="categ">&nbsp;</div>
	<div class="info">
		<div class="codice"><%: el.NumeroProtocollo %> <%: el.DataDoc%></div>
		<div class="oggetto"><%: el.Oggetto %></div>
		<div class="assegnatario"><%: el.Mittente %></div>
	</div>
	<div class="clear"><br class="clear"/></div>
</div>
<%} }%>
</div>


		