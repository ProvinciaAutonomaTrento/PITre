<%@ Register TagPrefix="uc1" TagName="DataGridPagingWait" Src="../waiting/DataGridPagingWait.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="toDoList.aspx.cs" Inherits="DocsPAWA.TodoList.toDoList" %>
<%@ Register  TagPrefix="uc2" TagName="IconeRicDoc" Src="~/UserControls/IconeRicerca.ascx"%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
	<HEAD runat="server">
		<title> DOCSPA > tabRisultatiRicTrasm</title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<script language="javascript" id="btn_SetTxUtAsViste_Click" event="onclick()" for="btn_SetTxUtAsViste">
			if(window.confirm('Attenzione,\nle trasmissioni ricevute che non necessitano accettazione o rifiuto\nsaranno eliminate ed impostate come già viste.\n\nConfermi?'))		
			{
				window.document.forms[0].hd1.value='1';
				window.document.body.style.cursor='wait';
			}
			else
			{
				window.document.forms[0].hd1.value='0';
			}
		</script>
		
		<script language="javascript">
			function ApriSmistamento() 
			{
		        var args = new Object;
		        args.window = window;

		        var availHeight = screen.availHeight;
		        var availWidth = screen.availWidth;

		        var retValue = window.showModalDialog("../SmistaDoc/SmistaDoc_Container.aspx",
				                      args,
				                      "dialogWidth:" + availWidth +
				                      ";dialogHeight:" + availHeight +
				                      ";status:no;resizable:no;scroll:auto;dialogLeft:0;dialogTop:0;center:yes;help:no;");

				
			//	window.open('../SmistaDoc/SmistaDoc_Container.aspx','','');
				//top.principale.document.location.reload()
				if(retValue=="Y") 
                {
                  top.principale.document.location.href=top.principale.document.location;                  
                  window.close();         
                }
			}
			
			function StampaRisultatoRicerca(docOrFasc)
			{				
			    var args=new Object;
				args.window=window;
				
				var retValue = window.showModalDialog("../exportDati/exportDatiSelection.aspx?export=toDoList&docOrFasc=" + docOrFasc,
										args,
										"dialogWidth:450px;dialogHeight:420px;status:no;resizable:no;scroll:no;center:yes;help:no;");						
                //top.principale.document.location.reload();										
			}				
			
			function svuotaTDLPage(noticeDays,oldTxt,tipoObjTrasm,isFunctionEnabled,datePost)
			{
			    var args=new Object;
				args.window=window;
				
				var retValue = window.showModalDialog("../TodoList/svuotaTDLPage.aspx?noticeDays="+noticeDays+"&oldTxt="+oldTxt+"&tipoObjTrasm="+tipoObjTrasm+"&isFunctionEnabled="+isFunctionEnabled+"&datePost="+datePost,
										args,
										"dialogWidth:500px;dialogHeight:340px;status:no;resizable:no;scroll:no;center:yes;help:no;");				
                if(retValue=="Y") 
                {
                  //top.principale.document.location.reload();
                  top.principale.document.location.href=top.principale.document.location;
                  window.close();         
                }
			}
			
			function WaitDataGridCallback(eventTarget,eventArgument)
			{
				document.body.style.cursor="wait";				
				document.getElementById("titolo").innerText="Ricerca in corso...";
			}
			
			function dettaglioTrasm(path)
			{
			    var args=new Object;
				args.window=window;
				
				var retValue = window.showModalDialog(path,
										args,
										"dialogWidth:580px;dialogHeight:360px;status:no;resizable:no;scroll:yes;center:yes;help:no;");				                      
				window.top.principale.iFrame_dx.document.location=window.top.principale.iFrame_dx.document.location.href;				
			}
		</script>
        <style>
        .proto_annullato
        {
            color:Red;
            text-decoration: line-through;
            font-size: 10px; 
            line-height: 9px; 
            font-family: Verdana
        }
        .proto_annullato a:link
        {
            color:Red;
            text-decoration: line-through;
            font-size: 10px; 
            line-height: 9px; 
            font-family: Verdana
        }
        .proto_annullato  a:visited
        {
            color:Red;
            text-decoration: line-through;
            font-size: 10px; 
            line-height: 9px; 
            font-family: Verdana
        }
        .proto_annullato a:hover
        {
            color:Red;
            text-decoration: line-through;
            font-size: 10px; 
            line-height: 9px; 
            font-family: Verdana
        }
        </style>
	</HEAD>
	<body>
    <form id="form1" runat="server"  onprerender="Page_PreRender">
       <uc1:datagridpagingwait id="DataGridPagingWait1" runat="server"></uc1:datagridpagingwait>
    	<table cellSpacing="0" cellPadding="0" width="99%" align="center" border="0">												
				<TR>
					<TD>
						<TABLE width="100%" style="background-color:AntiqueWhite;">
							<TR id="tr1" runat="server">
								<TD id="TD1" align="left" height="90%" runat="server" CssClass="titolo_real">
									<asp:label id="titolo" Runat="server" CssClass="titolo_real">Elenco trasmissioni</asp:label>
									<asp:Button id="btn_NonLetti" runat="server" BorderWidth="0"  Font-Underline="true" BackColor="AntiqueWhite" Font-Size="12px" Font-Names="Verdana"  CssClass="pulsante_hand_2" OnClick="btn_NonLetti_Click"></asp:Button>
								</TD>
								<TD class="testo_grigio_scuro" vAlign="middle" align="right" height="10%">									    
							        <cc1:ImageButton id="btn_smista" runat="server" AlternateText="Smista documenti" Tipologia="DO_SMISTA"
					                        DisabledUrl="../images/smistamento/smista_disattivo.gif" ImageUrl="../images/smistamento/smista_attivo.gif" OnClick="btn_smista_Click"></cc1:ImageButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
							        <asp:ImageButton ID="btn_rimuoviTDL" Visible="true" runat="server" AlternateText="Rimuovi trasmissioni da COSE DA FARE"
							            ImageUrl="../images/proto/rimuoviTDL.gif" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;								    
									<asp:ImageButton ID="btn_stampa" Visible="False" Runat="server" AlternateText="Esporta il risultato della ricerca"
										ImageUrl="../images/proto/export.gif" OnClick="btn_stampa_Click"></asp:ImageButton>&nbsp;&nbsp;
								</TD>
							</TR>
						</TABLE>
					</TD>
				</TR>
			    <tr>
			        <td>
<%--			            <DIV id="DivDGList" style="OVERFLOW:auto; HEIGHT:600px">
--%>			                <asp:DataGrid ID="grdDoc" runat="server" SkinID="datagrid" BorderColor="Gray" 
			                AutoGenerateColumns ="false" BorderWidth="1px" 
			                PageSize="8" CellPadding="1" AllowCustomPaging="true" AllowPaging="true"
			                OnPageIndexChanged = "grdDoc_PageIndexChanged" 
			                Width="100%" OnItemCommand = "grdDoc_ItemCommand" 
			                OnItemCreated="OnDataGridItemCreated" onprerender="grdDoc_PreRender" 
                            onselectedindexchanged="grdDoc_SelectedIndexChanged" >
                            <SelectedItemStyle CssClass="bg_grigioSP"/>
                            <AlternatingItemStyle CssClass="bg_grigioAP" />
                            <ItemStyle CssClass="bg_grigioNP" />
                            <PagerStyle VerticalAlign="Middle"  Position="TopAndBottom"
                            HorizontalAlign="Center" BackColor="#C2C2C2" CssClass="menu_pager_grigio"
                             Mode="NumericPages"/>
                            <HeaderStyle ForeColor="White" CssClass="menu_1_bianco_dg"/>				        
			                <Columns>
                                <%-- <asp:BoundColumn datafield="dataInvio"  HeaderText="Trasm. il" ItemStyle-Width="10%" Readonly="true" HeaderStyle-HorizontalAlign="Left" />--%>
                                <asp:TemplateColumn HeaderText="Trasm. il">
									<HeaderStyle Wrap="False" HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center"></ItemStyle>
									<ItemTemplate>
									<asp:LinkButton Text='<%# DataBinder.Eval(Container, "DataItem.dataInvio") %>' runat="server" ID="link_dettaglio" CommandName="Select" ToolTip="Vai alla scheda del documento" ></asp:LinkButton>
									</ItemTemplate>	
								</asp:TemplateColumn>
                                 <asp:BoundColumn datafield="videoMittRuolo" HeaderText="Mittente Trasm.<br>(Ruolo)" ItemStyle-Width="20%" readonly="true" HeaderStyle-HorizontalAlign="Left" />
                                 <asp:BoundColumn datafield="ragione" HeaderText="ragione" ItemStyle-Width="10%" Readonly="true" HeaderStyle-HorizontalAlign="Left" />
                                 <asp:BoundColumn datafield="noteGenerali" HeaderText="note gen.<br>&nbsp;-------<br>&nbsp;note indiv." ItemStyle-Width="10%" Readonly="true" HeaderStyle-HorizontalAlign="Left" />
                                 <asp:BoundColumn datafield="dataScadenza" HeaderText="scadenza" ItemStyle-Width="5%" Readonly="true" HeaderStyle-HorizontalAlign="Left" />
                                 <asp:BoundColumn DataField="VideoTipology" HeaderText="tipologia" ItemStyle-Width="5%" ReadOnly="true" HeaderStyle-HorizontalAlign="Left" />
			                     <asp:TemplateColumn Visible="false">
			                         <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%"></ItemStyle>
                                     <ItemTemplate>
                                          <asp:ImageButton id="imgDettaglio" runat="server" ImageUrl="../images/proto/dettaglio.gif" BorderWidth="0px" BorderColor="Gray" BorderStyle="Solid" AlternateText='<%# DataBinder.Eval(Container, "DataItem.infoDoc") %>' CommandName="dettaglio">
                                           </asp:ImageButton>
                                     </ItemTemplate>				        
			                     </asp:TemplateColumn>
                                 <asp:BoundColumn datafield="videoSegnRepertorio" HeaderText="Repertorio" ItemStyle-Width="5%" readonly="true" HeaderStyle-HorizontalAlign="center" ItemStyle-HorizontalAlign="Center" Visible="false" />
			                     <asp:BoundColumn datafield="videoDocInfo" HeaderText="doc./fasc." ItemStyle-Width="5%" readonly="true" HeaderStyle-HorizontalAlign="center" ItemStyle-HorizontalAlign="Center" />
                                 <asp:BoundColumn datafield="videoOggMitt" HeaderText="Oggetto<br>&nbsp;-------<br>&nbsp;Mittente" ItemStyle-Width="40%" readonly="true" HeaderStyle-HorizontalAlign="Left"/>
<%--                                 <asp:TemplateColumn HeaderText="scheda" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="5%">
                                   <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
                                     <ItemTemplate>
                                      <asp:ImageButton id="imgScheda" runat="server" ImageUrl="../images/proto/doc.gif" BorderWidth="0px" BorderColor="Gray" BorderStyle="Solid" AlternateText='<%# DataBinder.Eval(Container, "DataItem.infoDoc") %>' CommandName="scheda">
                                       </asp:ImageButton>
                                   </ItemTemplate>
                                 </asp:TemplateColumn>--%>
<%--                                 <asp:TemplateColumn HeaderText="VIS">
									<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<HeaderTemplate>
										<asp:Label id="lbl_Vis" runat="server" CssClass="menu_1_bianco_dg">Vis.</asp:Label>
									</HeaderTemplate>
									<ItemTemplate>
										<asp:ImageButton id=IMG_VIS runat="server" ImageUrl="../images/proto/dett_lente.gif" CommandName="VisDoc" ToolTip="Visualizza immagine documento">
										</asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>--%>
                                 <asp:TemplateColumn HeaderText="Rimuovi" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="5%">
									<ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"></ItemStyle>
									<ItemTemplate>
										<asp:ImageButton id="btn_Rimuovi" runat="server"  
										ImageUrl="../images/proto/b_elimina.gif" CommandName="Rimuovi" ToolTip="Rimuovi il documento trasmesso per interoperabilità" AlternateText="Rimuovi il documento trasmesso per interoperabilità" > </asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateColumn>
								<asp:BoundColumn HeaderText="cha_img" Visible="false" DataField="cha_img"></asp:BoundColumn>
								<asp:BoundColumn datafield="isVista" Visible="false" />
								<asp:BoundColumn datafield="isFirmato" Visible="false" />
								<asp:TemplateColumn Visible="true">
								<HeaderStyle Wrap="false" Height="5px" />
								<ItemTemplate>
								<uc2:IconeRicDoc ID="UserControlRic" runat="server" ACQUISITA_IMG='<%# DataBinder.Eval(Container, "DataItem.cha_img") %>' DOC_NUMBER='<%# DataBinder.Eval(Container, "DataItem.infoDoc") %>' FIRMATO='<%# DataBinder.Eval(Container, "DataItem.isFirmato") %>'
								 ID_PROFILE='<%# DataBinder.Eval(Container, "DataItem.sysIdDoc") %>'  PAGINA_CHIAMANTE="toDoList" IS_DOC_OR_FASC='<%# DataBinder.Eval(Container, "DataItem.infoDoc") %>' />
								</ItemTemplate>
								</asp:TemplateColumn>
                                <asp:BoundColumn datafield="sysIdDoc" Visible="false" />
			                </Columns>
                        </asp:DataGrid>	
<%--                        </DIV>			      
--%>			        </td>
			    </tr>
            </table>
        </form>
    </body>
</html>

