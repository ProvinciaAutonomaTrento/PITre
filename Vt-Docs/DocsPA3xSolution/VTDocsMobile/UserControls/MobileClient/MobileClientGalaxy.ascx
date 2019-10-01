<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MobileClientGalaxy.ascx.cs" Inherits="VTDocsMobile.UserControls.MobileClient.MobileClientGalaxy" %>
<script type="text/javascript">
    var model = <%= MainModel %>;
    context = <%= WAContext %>;
	var DEVICE = "galaxy";
</script>
<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/default_tablet.js")%>"></script>
<script type="text/javascript" src="<%= ResolveUrl("~/Scripts/tooltip.js")%>"></script>

<div id="header">
	<ul>
		<li class="ico_dafare" id="TODO_LIST"><p class="ico">&nbsp;</p><a href="javascript:;">Da Fare</a><div id="nnotifiche">..</div></li>
        <li class="ico_adl" id="AREA_DI_LAVORO"><p class="ico">&nbsp;</p><a href="javascript:;">Adl</a></li>        
		<li class="ico_cerca" id="RICERCA"><p class="ico">&nbsp;</p><a href="javascript:;">Cerca</a></li>
		<li class="ico_smista" id="SMISTAMENTO"><p class="ico">&nbsp;</p><a href="javascript:;">Smista</a></li>
        <li class="ico_deleghe" id="LISTA_DELEGHE"><p class="ico">&nbsp;</p><a href="javascript:;">Deleghe</a></li>
		<li class="ico_logout" id="LOGOUT"><p class="ico">&nbsp;</p><a href="javascript:;">Logout</a></li>
	</ul>
</div>
<div id="wrapper">
	
	<!-- barre di servizio -->
    <div id="service_bar" class="green_bar none">
        
    </div>
    <div id="info_trasm" class="info none">
        <div id="sezione_trasmissione" class="hide">
            <div class="dett_trasm">
                <h2>INFORMAZIONI DELLA TRASMISSIONE</h2>
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
            <h2>NOTE GENERALI</h2>
            <p></p>
            <div class="clear"><br class="clear" /></div>
        </div>
        <div id="sezione_documento" class="hide">
            <h2></h2>
            <div class="side left">
                <h3>Oggetto</h3>
                <p></p>
            </div>
            <div class="side">
                <h3>Note</h3>
                <p></p>
            </div>
            <div class="clear"><br class="clear"/></div>
            
            <div id="note_aggiuntive">
            
            </div>
            <div class="clear"><br class="clear" /></div>
        </div>
    </div>
    <!-- vista documenti -->
    <div id="item_list" class="none">
    
    </div>
    <!-- prev doc-->
    <div id="prev_doc" class="none">
        
    </div>
    
    <!--ricerca-->
    <div id="ricerca_item" class="none">
        <div class="item_list">
        
        </div>
    </div>
    
    <!-- deleghe -->
    <div id="deleghe" class="none">
        
    </div>
    
    <!-- smista_doc -->
    <div id="smista_doc" class="none">
        
    </div>
	<div class="clear"><br class="clear" /></div>
</div>
<div id="footer"><div id="paginatore"></div></div>


