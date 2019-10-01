<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MobileClient.ascx.cs" Inherits="VTDocsMobile.UserControls.MobileClient.MobileClient" %>
<script type="text/javascript">
    var model = <%= MainModel %>;
    context = <%= WAContext %>;
</script>
<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/default.js")%>"></script>
<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/tooltip.js")%>"></script>

<div id="header">
	<ul>
		<li class="ico_dafare" id="TODO_LIST"><p class="ico">&nbsp;</p><a href="javascript:;">Da Fare</a><div id="nnotifiche">..</div></li>
		<li class="ico_cerca" id="RICERCA"><p class="ico">&nbsp;</p><a href="javascript:;">Cerca</a></li>
		<li class="ico_deleghe" id="LISTA_DELEGHE"><p class="ico">&nbsp;</p><a href="javascript:;">Deleghe</a></li>
		<li class="ico_logout" id="LOGOUT"><p class="ico">&nbsp;</p><a href="javascript:;">Logout</a></li>
	</ul>
</div>
<div id="wrapper">
	<div id="scroller">
		<!-- barre di servizio -->
		<div id="service_bar" class="green_bar none">
			
		</div>
		<div id="filter_ruolo" class="none">
			<select class="skin"></select>
		</div>
		<div id="folder_name" class="none">
			<img src="Content/img/folder_name_ico.gif" />
			<span>Permesso passo carrabile</span>
		</div>
		
		
		<!-- layer di contenuto
			questo è per la todolist
		-->
		<div id="item_list" class="item_list none">
			
		</div>
		
		<!-- info trasm-->
		<div id="info_trasm" class="info none">
			<div id="sezione_trasmissione">
				<h2>Informazioni della trasmissione</h2>
				<div class="dett_trasm">
					<table cellpadding="0" cellspacing="0">
						<tr>
							<th>MITTENTE:</th>
							<td></td>
						</tr>
						<tr>
							<th>RAGIONE:</th>
							<td></td>
						</tr>
						<tr>
							<th>DATA:</th>
							<td></td>
						</tr>
					</table>
				</div>
				<div class="sep">&nbsp;</div>
				<h3>NOTE GENERALI</h3>
				<p></p>
				<div class="sep">&nbsp;</div>
				<h3>NOTE INDIVIDUALI</h3>
				<p></p>
			</div>
			<div id="sezione_documento">
				<h2></h2>
				
				<h3>OGGETTO</h3>
				<p></p>
				<div class="sep">&nbsp;</div>
				<h3>NOTE</h3>
				<p></p>
			</div>
		</div>
		
		<!-- prev doc-->
		<div id="prev_doc" class="none">
			
		</div>
		
		<!-- info profilo item-->
		<div id="info_item" class="info none">
			
			
		</div>
		
		<!-- info profilo item-->
		<div id="ricerca_item" class="none">
			<h2>Ricerca Rapida</h2>
			<div class="filter_ricerca"></div>
			<div class="item_list">
			
			</div>
		</div>
		
		<!-- info profilo item-->
		<div id="trasmissione_doc" class="info none">
			<div class="filter_trasm"></div>
			<div class="sep">&nbsp;</div>
			<h3>NOTE GENERALI DI TRASMISSIONE</h3>
			<p></p>
			<h2>Informazioni di profilo del documento</h2>
			<h3>OGGETTO</h3>
			<p></p>
			<div class="sep">&nbsp;</div>
			<h3>NOTE</h3>
			<p></p>
		</div>
        
        <!--deleghe-->
        <div id="deleghe" class="none">
        	
        </div>
		
	</div>
</div>
<div id="footer">
	<div id="footer_bar">
		<div class="cont"></div>
		<div class="bg">&nbsp;</div>
	</div>
</div>
