<%@ Control Language="C#" CodeBehind="~/VTDocs/mobile/fe/usercontrols/DocInfoUserControl.cs"  Inherits="VTDocs.mobile.fe.usercontrols.DocInfoUserControl"%>
<%@ Import Namespace="VTDocs.mobile.fe.wsrefs.VTDocsWSMobile" %>

<%foreach (ToDoListElement el in Model.Elements){
  if(!string.IsNullOrEmpty(el.IdDoc)){%>
     <div class="documento" id="documento_<%: el.IdDoc %>">
        <iframe id="visDoc_<%: el.IdDoc %>" style="width: 100%; height: 100%;" scrolling="auto" frameborder="0" visible="true" enableviewstate="false">
        </iframe>
        <div class="properties">
          <h2>Informazioni di Profilo</h2>
		  <div class="info">
			 <h3>OGGETTO</h3>
			 <p><%:el.Oggetto %></p>
			 <h3>NOTE</h3>
			 <p><%: el.Note %></p>
			 <h3>PAROLE CHIAVE</h3>
			 <p>PC</p>
			 <h3>DATA</h3>
			 <p class="data"><%: this.formatDate(el.DataDoc)%></p>
		 </div>
        </div>
     </div>
<%} }%>