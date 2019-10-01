
<%@ Page language="c#" Codebehind="Dettagli.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.RubricaDocsPA.Dettagli" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register src="../../UserControls/Calendar.ascx" TagName="Calendario"  tagprefix="uc3" %>
<%@ Register Src="../../UserControls/AppTitleProvider.ascx" TagName="AppTitleProvider" TagPrefix="uct" %>
<%--<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >--%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<HTML>
	<HEAD>
		<title><%
            string titolo = System.Configuration.ConfigurationManager.AppSettings["TITLE"];
            if (titolo != null)
            {
             %>
                <%= titolo%>
             <%
                   }
            else
            {
             %>
                DOCSPA
		     <%} %>  > Dettagli</title>
        
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">
		<LINK href="../../CSS/rubrica.css" type="text/css" rel="stylesheet">
        <script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>
		<base target="_self">
        <style type="text/css">
        body
        {
            background-color: #eaeaea;
            font-family: Verdana;
        }
        .head_tab
        {
            height: 20px;
        }
        #cont_pref
        {
            
            width: 600px;
            background-color: #fafafa;
            height: 120px;
            
        }
        .tab_sx
        {
            text-align: center;
            padding-left: 0px;
            font-size: 10px;
            color:#333333;
            
	        white-space:nowrap;
	        display:inline-block;
        }
        .tab_dx
        {
        	text-align: center;
            padding-left: 0px;
            font-size: 10px;
            color:#333333;
            
	        white-space:nowrap;
	        display:inline-block;
        }
        #button
        {
            text-align: center;
            margin-left: 0px;
            margin-right: 0px;
            padding-top: 0px;
            margin-bottom:0px;
        }
       
        #topGrid
        {
            text-align: center;
            width: 432px;
           
            border-bottom:0px solid #ffffff;
            padding-right:0px;
          
        }
        
        .title
        {
            margin: 0px;
            padding: 0px;
            font-size: 11px;
            font-weight: bold;
            text-align: center;
            width: 100%;
            float: left;
            padding-top: 0px;
            padding-bottom: 0px;
        }
       
       #divDatagrid
       {
            overflow-y:scroll;
            height: 125px;
       }
        

    </style>
		<script language="javascript">

		    var change = false;

		    function disabledButton() {
		        //var tipoCorrispondente = document.getElementById('ddl_tipoCorr').value;
		        //var cf = document.getElementById('txt_CodFis').value;
		        if (change && document.getElementById('ddl_tipoCorr') != null && document.getElementById('ddl_tipoCorr').value == "U" && document.getElementById('txt_CodFis').value.length == 16) {
		            if (confirm('Per un corrispondente di tipo UO stai inserendo/modificando il campo Codice fiscale con uno di tipo persona, sei sicuro di voler proseguire?')) {
		                var btnMod = document.getElementById('btn_modifica');
		                var btnDel = document.getElementById('btn_elimina');
		                var btnCh = document.getElementById('btn_chiudi');
		                btnMod.disabled = true;
		                btnDel.disabled = true;
		                btnCh.disabled = true;
		                __doPostBack(btnMod.name, '')
		                return true;
		            }
		            else
		                return false;
		        }
		        else {
		            var btnMod = document.getElementById('btn_modifica');
		            var btnDel = document.getElementById('btn_elimina');
		            var btnCh = document.getElementById('btn_chiudi');
		            btnMod.disabled = true;
		            btnDel.disabled = true;
		            btnCh.disabled = true;
		            __doPostBack(btnMod.name, '')
		            return true;
		        }

		    }

		function ApriconfirmDelCorr()
		{
			var response = window.confirm("Confermi la cancellazione del corrispondente?");
			if (response)
			{
				document.getElementById("hd_confirmDelCorr").value = "Yes";
				
			}	
			else
			{
				document.getElementById("hd_confirmDelCorr").value = 'No';
			}		
		}

		function CloseIt() {
		    self.close();
		}

		function ApriFinestraVisibilita(IdProf) {
		    var newLeft = (screen.availWidth - 605);
		    var newTop = (screen.availHeight - 640);
		    var value_IdProf = document.getElementById(IdProf);
		    //win=window.open("visibilitaDocumento.aspx?VisFrame="+value_IdProf.value,"Visibilita","width=588,height=450,top="+newTop+",left="+newLeft+",toolbar=no,directories=no,menubar=no,resizable=yes, scrollbars=no"); 
		    window.showModalDialog("../popup/visibilitaDocumento.aspx?VisFrame=" + value_IdProf.value, "Visibilita", "dialogWidth:700px;dialogHeight:505px;status:no;resizable:no;scroll:no;center:no;help:no;close:no;dialogLeft:" + newLeft + ";dialogTop:" + newTop + ";");
		    return false;
		}

		function feedhdField(rad) {
		    
		    document.getElementById('hd_field').value = rad.value;

		    __doPostBack("upd_dett_corr", rad.value);

		}

		function resettaRadio() {
		    var radio = document.getElementsByTagName("input");
		    for (i = 0; i < radio.length; i++)
		        if (radio[i].name == "rbl_pref") {
		            radio[i].checked = false;

		        }
		        document.getElementById('hd_field').value = "";

		        __doPostBack("upd_dett_corr", "resettaCampi");
		    }

		    function verificaCF() {
		        var protocolla = document.getElementById('btn_protocolla').value;
		        var tipoCorrispondente = document.getElementById('ddl_tipoCorr').value;
		        var hd_field = document.getElementById('hd_field').value;
		        var cf = document.getElementById('txt_CodFis').value;

		        if (tipoCorrispondente == "U" && cf.length == 16 && hd_field == "" && !document.getElementById('ddl_tipoCorr').disabled) {
		            return confirm('Per un corrispondente di tipo UO stai inserendo/modificando il campo Codice fiscale con uno di tipo persona, sei sicuro di voler proseguire?');
		        }
		    }

		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="1" rightMargin="0" MS_POSITIONING="GridLayout">
    
		<form id="Form1" method="post" runat="server">
			<input id="hd_systemId" type="hidden" runat="server"> 
            <input id="hd_tipo_URP" type="hidden" name="hd_codice" runat="server" EnableViewState="true">
			<INPUT id="hd_confirmDelCorr" type="hidden" name="hd_confirmDelCorr" runat="server">
			<input id="hd_canalePref" type="hidden" name="hd_canalePref" runat="server"><input id="hd_idReg" type="hidden" name="hd_idReg" runat="server">
			<asp:ScriptManager ID="ScriptManager" runat="server" AsyncPostBackTimeout="360000" ScriptMode ="Release">
            </asp:ScriptManager>
            <div id="contenitore_esterno" style="height:90%;width:99%">
            <asp:table id="tbl_checkInterop" runat="server" Visible="false" width="600px" align="center" border="0" CssClass="contenitore">
                <asp:tableRow>
                    <asp:tableCell CssClass="menu_1_rosso" HorizontalAlign="Center">
                        <asp:Label runat="server" Text="AVVISO"/>
                    </asp:tableCell>
                </asp:tableRow>
                <asp:tableRow>
                    <asp:TableCell>
                        <asp:table runat="server" class="info_grigio" id="tbl_container_avviso" borderColor="#cc6600" width="498px" align="center" border="0">
                            <asp:tableRow>
                                <asp:tableCell HorizontalAlign="Justify" CssClass="testo_red">
                                    <div align="center"><asp:Label ID="lbl_Avviso" runat="server"></asp:Label></div>
                                </asp:tableCell>
                            </asp:tableRow>
                        </asp:table>
                    </asp:TableCell>
                </asp:tableRow>
            </asp:table>
            <asp:UpdatePanel ID="upd_hdField" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:HiddenField id="hd_field" runat="server" EnableViewState="true" OnValueChanged="hd_field_OnValueChanged"/>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:Table ID="tbl_MultiMitt" runat="server" Visible="false" Width="580px" align="center">
                <asp:TableRow>
                    <asp:TableCell>
                        <div id="cont_pref">
                            <asp:UpdatePanel ID="box_preferred_grids" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div id="topGrid">
                                        <table width="580px">
                                            <tr>
                                                <td style="width:5%" align="center">
                                                    <asp:Image ID="resetRadio" runat="server" ImageUrl="../../images/ricerca/remove_search_filter.gif" ToolTip="Pulisci i campi" />
                                                </td>
                                                <td style="width:95%" align="center">
                                                    <asp:Label ID="titlePage" runat="server" Text="Seleziona un corrispondente" CssClass="menu_1_rosso"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div id="divDatagrid" runat="server">
                                        <asp:DataGrid ID="grvListaCorr" runat="server" AllowSorting="True" AutoGenerateColumns="false"
                                            Width="580px" SkinID="datagrid" AllowPaging="False" 
                                            AllowCustomPaging="false" PageSize="10" BorderStyle="Solid" BorderWidth="1" BorderColor="#cccccc">
                                            <AlternatingItemStyle BackColor="White" />
                                            <ItemStyle BackColor="#f0f0f0" ForeColor="#333333" Font-Size="Small" />
                                            <Columns>
                                                <asp:TemplateColumn ItemStyle-Width="10px" HeaderText="" ItemStyle-HorizontalAlign="center"
                                                    HeaderStyle-HorizontalAlign="center">
                                                    <ItemTemplate>
                                                        <input name="rbl_pref" type="radio" class="testo_grigio_scuro" value='<%# this.GetCorrID((DocsPAWA.DocsPaWR.Corrispondente)Container.DataItem) %>' onclick="feedhdField(this)">
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn ItemStyle-Width="290px" HeaderText="Descrizione" ItemStyle-CssClass="tab_sx"
                                                    HeaderStyle-HorizontalAlign="center" HeaderStyle-CssClass="head_tab">
                                                    <ItemTemplate>
                                                        <asp:Label ID="gridName" runat="server" Text='<%# this.GetCorrName((DocsPAWA.DocsPaWR.Corrispondente)Container.DataItem) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn ItemStyle-Width="140px" HeaderText="Codice" ItemStyle-HorizontalAlign="center"
                                                    HeaderStyle-HorizontalAlign="center"  ItemStyle-CssClass="tab_dx"  HeaderStyle-CssClass="head_tab">
                                                    <ItemTemplate>
                                                    <asp:Label ID="gridCodice" runat="server" Text='<%# this.GetCorrCodice((DocsPAWA.DocsPaWR.Corrispondente)Container.DataItem) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                                <asp:TemplateColumn ItemStyle-Width="140px" HeaderText="Rubrica" ItemStyle-HorizontalAlign="center"
                                                    HeaderStyle-HorizontalAlign="center"  ItemStyle-CssClass="tab_dx"  HeaderStyle-CssClass="head_tab">
                                                    <ItemTemplate>
                                                    <asp:Label ID="gridRubrica" runat="server" Text='<%# this.GetCorrReg((DocsPAWA.DocsPaWR.Corrispondente)Container.DataItem) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateColumn>
                                            </Columns>
                                        </asp:DataGrid>
                                    </div>    
                                </ContentTemplate>
                        </asp:UpdatePanel>
                     </div>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <asp:Table id="tbl_blank" runat="server" Visible="false">
                <asp:TableRow>
                    <asp:TableCell>
                        <br />
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <%--<asp:Table ID="tbl_MultiDest" runat="server">
                <asp:TableRow>
                    <asp:TableCell>
                        <uc4:MultiDestinatari id="MultiDest" runat="server" Visible="true"/>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>--%>
            <asp:UpdatePanel ID="upd_dett_corr" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <input id="hd_tipo_corr" type="hidden" name="hd_tipo_corr" runat="server" EnableViewState="true"/>
                    <TABLE class="contenitore" id="tbl_dettagliCorrispondenti" height="600px" width="600px" align="center"
				    border="0">
				    <TR vAlign="middle" height="15">
					    <td class="menu_1_rosso" align="center">
                            <asp:label id="Label1" runat="server">Dettagli corrispondente</asp:label>
                            <asp:label id="lblLista" Runat="server">Dettaglio lista</asp:label>
                        </td>
				    </TR>
                
				    <!-- tr vAlign="top" height="600px" -->
                    <tr vAlign="top">
					    <td>
                            <div id="divCorr" runat="server" style="overflow:auto;height:90%;width:99%; padding:3px;">
						    <asp:panel id="PanelCorrispondente" runat="server" Height="80%">
							    <TABLE class="info_grigio" id="tbl_container" borderColor="#cc6600" width="560px" align="center" border="0">
								    <TR>
									    <TD>
										    <TABLE class="info_grigio" height="80px" width="560px" border="0">
											    <TR height="22">
												    <TD colSpan="2">
													    <TABLE width="560px">
														    <TR>
															    <asp:Panel id="pnlRegistriCorr" Runat="server" Visible="False">
																    <TD class="titolo_scheda">
                                                                        <IMG src="../../images/proto/spaziatore.gif" width="1" border="0">
                                                                        <asp:Label id="lblReg" Runat="server" Width="48">Registro</asp:Label>
																	    <IMG src="../../images/proto/spaziatore.gif" width="8" border="0"> 
                                                                        <IMG src="../../images/proto/spaziatore.gif" width="8" border="0">
                                                                        <IMG src="../../images/proto/spaziatore.gif" width="4" border="0">
                                                                        <IMG src="../../images/proto/spaziatore.gif" width="8" border="0">
																	    <asp:Label id="lblRegistro" Runat="server"></asp:Label>
                                                                    </TD>
															    </asp:Panel>
															    <asp:Panel id="pnl_modifica" Runat="server">
																    <TD align="right">
																	    <cc1:imagebutton id="btn_mod_corr" Runat="server" Height="17" ImageUrl="../../images/proto/matita.gif"
																		    AlternateText="Modifica" DisabledUrl="../../images/proto/matita.gif" Tipologia="GEST_RUBRICA"></cc1:imagebutton>&nbsp;
																    </TD>
															    </asp:Panel>
                                                                <td align="right">
		                                                            <cc1:imagebutton id="btn_VisDoc" ImageAlign="AbsMiddle" Runat="server" AlternateText="Visualizza immagine documento" ImageUrl="../../images/proto/dett_lente_doc.gif" Visible="false"></cc1:imagebutton>
                                                                </td>
                                                            </TR>
													    </TABLE>
												    </TD>
											    </TR>
                                                
											    <TR vAlign="top" height="22">
												    <TD class="titolo_scheda" style="WIDTH: 105px">Codice Amm.<asp:label id="starCodAmm" Runat="server" Visible="false">*</asp:label>
                                                    </TD>
												    <TD>
													    <asp:TextBox id="txt_CodAmm" runat="server" CssClass="testo_grigio" Width="455px"></asp:TextBox>
                                                    </TD>
											    </TR>
											    <TR height="22">
												    <TD class="titolo_scheda" style="WIDTH: 105px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Codice 
													    AOO<asp:label id="starCodAOO" Runat="server" Visible="false">*</asp:label></TD>
												    <TD>
													    <asp:TextBox id="txt_CodAOO" runat="server" CssClass="testo_grigio" Width="455px"></asp:TextBox>
                                                    </TD>
											    </TR>


                                                <%-- SEZIONE MAIL --%>
											    <tr>
												    <td class="titolo_scheda" style="WIDTH: 105px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Email
													    <asp:label id="starEmail" Runat="server">*</asp:label>
                                                    </td>
												    <td>
                                                        <asp:UpdatePanel ID="updPanel1" runat="server" UpdateMode="Always" Visible="true">
                                                            <ContentTemplate>
													                <asp:TextBox id="txt_EmailAOO" runat="server" CssClass="testo_grigio" Width="455px"></asp:TextBox>
                                                                    <asp:TextBox ID="txtCasella" CssClass="testo_grigio" runat="server" Width="420px"></asp:TextBox>
                                                                    <asp:ImageButton AlternateText="Aggiungi casella di posta" ImageAlign="AbsMiddle" OnClick="imgAggiungiCasella_Click"
                                                                        ID="imgAggiungiCasella" runat="server" ToolTip="Aggiungi casella di posta" ImageUrl="~/images/proto/aggiungi.gif" />
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </td>
                                                </tr>
                                                <%-- END SEZIONE MAIL --%>


                                                <%-- SEZIONE NOTE MAIL --%>
                                                <tr>
                                                    <td class="titolo_scheda" style="WIDTH: 105px">
                                                        <div id="divNote" runat="server">
                                                            <IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Note E-mail
                                                        </div>
                                                    </td>
                                                    <td>
                                                        <asp:UpdatePanel ID="updPanel2" runat="server" UpdateMode="Always" Visible="true">
                                                            <ContentTemplate>
                                                                <asp:TextBox ID="txtNote" CssClass="testo_grigio" runat="server" Width="455px" MaxLength="20"></asp:TextBox> 
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </td>
                                                </tr>   
                                                <%-- END SEZIONE NOTE MAIL --%>
                                                <%-- SEZIONE MULTICASELLA --%> 
                                                <tr> 
                                                    <td class="titolo_scheda" style="WIDTH:105px;"></td>
                                                    <td>
                                                        <asp:UpdatePanel ID="updPanelMail" runat="server" UpdateMode="Always" Visible="true">
                                                            <ContentTemplate>
                                                                <div id="divGridViewCaselle" runat="server">
                                                                    <asp:GridView  ID="gvCaselle" runat="server" Width="455px" AutoGenerateColumns="False" OnRowDataBound="gvCaselle_RowDataBound"
                                                                        CellPadding="1" BorderWidth="1px" BorderColor="Gray"
                                                                        style="overflow-y:scroll;overflow-x:hidden;max-height:90px">
                                                                        <SelectedRowStyle CssClass="bg_grigioS"></SelectedRowStyle>
                                                                        <AlternatingRowStyle CssClass="bg_grigioA"></AlternatingRowStyle>
                                                                        <RowStyle CssClass="bg_grigioN"></RowStyle>
                                                                        <HeaderStyle CssClass="menu_1_bianco_dg" BackColor="#810D06"></HeaderStyle>                                                                                      
                                                                        <Columns>
                                                                            <asp:TemplateField HeaderText="SystemId" Visible="false">
                                                                                    <ItemTemplate>
                                                                                      <asp:Label runat="server" ID ="lblSystemId" Text ='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).systemId %>'></asp:Label>
                                                                                    </ItemTemplate>
                                                                            </asp:TemplateField>
                                                                            <asp:TemplateField HeaderText="Email" ShowHeader="true" >
                                                                                 <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                                 <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="68%"></ItemStyle>
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox AutoPostBack="true" Width="310px" OnTextChanged="txtEmailCorr_TextChanged" CssClass="testo_grigio" style="margin:1px;" ID="txtEmailCorr" runat="server" ToolTip='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Email %>' Text='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Email %>'></asp:TextBox>
                                                                                    </ItemTemplate>
                                                                            </asp:TemplateField> 
                                                                            <asp:TemplateField HeaderText="Note E-mail" ShowHeader="true">
                                                                                 <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                                 <ItemStyle  HorizontalAlign="Center" VerticalAlign="Middle" Width="28%"></ItemStyle>
                                                                                    <ItemTemplate>
                                                                                        <asp:TextBox AutoPostBack="true"  Width="110px" MaxLength="20" CssClass="testo_grigio" OnTextChanged="txtNoteMailCorr_TextChanged" style="margin:1px;" ID="txtNoteMailCorr" runat="server" ToolTip='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Note %>' Text='<%# ((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Note %>'></asp:TextBox>
                                                                                    </ItemTemplate>
                                                                            </asp:TemplateField> 
                                                                            <asp:TemplateField HeaderText="*" ShowHeader="true" >
                                                                                 <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                                 <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%"></ItemStyle>
                                                                                    <ItemTemplate>
                                                                                        <asp:RadioButton ID="rdbPrincipale" runat="server" AutoPostBack="true" OnCheckedChanged="rdbPrincipale_ChecekdChanged" Checked='<%# TypeMailCorrEsterno(((DocsPAWA.DocsPaWR.MailCorrispondente)Container.DataItem).Principale) %>' />
                                                                                    </ItemTemplate>
                                                                            </asp:TemplateField> 
                                                                            <asp:TemplateField HeaderText="" ShowHeader="true">
                                                                                 <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                                                                 <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%"></ItemStyle>
                                                                                    <ItemTemplate>
                                                                                        <asp:ImageButton ID="imgEliminaCasella" runat="server"  OnClick="imgEliminaCasella_Click" AutoPostBack="true" ImageUrl="~/images/proto/cancella.gif" />
                                                                                    </ItemTemplate>
                                                                            </asp:TemplateField> 
                                                                           </Columns>
                                                                    </asp:GridView>
                                                                </div>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </td>
											    </tr>
                                                <!-- END SEZIONE MULTICASELLA -->


                                  			    <asp:Panel id="pnl_canalePref" Runat="server">
												    <TR class="titolo_scheda" style="WIDTH: 105px" height="22px">
													    <TD><IMG height="1" src="../../images/proto/spaziatore.gif" width="4" border="0">Canale 
														    pref.</TD>
													    <TD>
														    <asp:dropdownlist id="dd_canpref" runat="server" CssClass="testo_grigio" Width="455px" AutoPostBack="True"></asp:dropdownlist></TD>
												    </TR>
											    </asp:Panel>
										    </TABLE>
									    </TD>
								    </TR>
								    <TR>
									    <TD>
										    <asp:panel id="pnlStandard" runat="server">
											    <TABLE class="info_grigio" id="tblDettagli" borderColor="#cccc33" height="340px" cellSpacing="0"
												    cellPadding="0" width="560px" align="center" border="0">
												    <TR vAlign="top">
													    <TD>
														    <TABLE>
                                                               
                                                                <TR height="22">
									                                <td class="titolo_scheda">
                                                                        <IMG height="1" src="../../images/proto/spaziatore.gif" width="8" border="0"><asp:label id="TipoCorr" runat="server" Width="100px">Tipo Corrispondente</asp:label>
                                                                    </td>
									                                <td align="left">
                                                                        <asp:dropdownlist id="ddl_tipoCorr" runat="server" AutoPostBack="True" CssClass="testo_grigio" Width="278px" Visible="false">
											                                <asp:ListItem Value="U" Selected="True">UO</asp:ListItem>
											                                <asp:ListItem Value="R">RUOLO</asp:ListItem>
                                                                            <asp:ListItem Value="P">PERSONA</asp:ListItem>
										                                </asp:dropdownlist>
                                                                    </td>
								                                </TR>
															  
															    <TR height="22">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Cod.rubrica
                                                                     <asp:label id="starRubrica" Runat="server">*</asp:label>
                                                                    </TD>
																    <TD>
																	    <asp:TextBox id="txt_CodRubrica" runat="server" CssClass="testo_grigio" Width="440px" ReadOnly="True"></asp:TextBox>
                                                                    </TD>
															    </TR>
															    <asp:Panel id="pnl_descrizione" Runat="server">
																    <TR height="22">
																	    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Descrizione 
																		    *</TD>
																	    <TD>
																		    <asp:textbox id="txt_descrizione" runat="server" CssClass="testo_grigio" Width="440px"></asp:textbox></TD>
																    </TR>
															    </asp:Panel>
															
                                                                <asp:Panel id="pnl_titolo" Runat="server" Visible="false">
                                                                    <TR height="22">
																	    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Titolo 
																		    </TD>
																	    <TD>
																		    <asp:dropdownlist id="dd_titolo" runat="server" CssClass="testo_grigio" Width="440px" AutoPostBack="True"></asp:dropdownlist></TD>
																    </TR>
                                                                    </asp:Panel>
                                                                    <asp:Panel id="pnl_nome_cogn" Runat="server">
																    <TR height="22">
																	    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Nome 
																		    *</TD>
																	    <TD>
																		    <asp:textbox id="txt_nome" runat="server" CssClass="testo_grigio" Width="440px"></asp:textbox></TD>
																    </TR>
																    <TR height="22">
																	    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Cognome 
																		    *</TD>
																	    <TD>
																		    <asp:textbox id="txt_cognome" runat="server" CssClass="testo_grigio" Width="440px"></asp:textbox></TD>
																    </TR>
                                                                    </asp:Panel>
                                                                    <asp:Panel id="pnl_infonascita" Runat="server" Visible="false">
                                                                    <TR height="22">
																	    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Luogo di nascita 
																		    </TD>
																	    <TD>
																		    <asp:textbox id="txt_luogoNascita" runat="server" CssClass="testo_grigio" Width="440px"></asp:textbox></TD>
																    </TR>
                                                                    <TR height="22">
																	    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Data di nascita 
																		    </TD>

																	    <TD>
																		    <asp:textbox id="txt_dataNascita" runat="server" CssClass="testo_grigio" Width="440px"></asp:textbox></TD>
																    </TR>
															        </asp:Panel>
                                                                <asp:Panel ID="pnl_indirizzo" runat="server">
															    <TR height="22">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Indirizzo</TD>
																    <TD>
																	    <asp:textbox id="txt_Indirizzo" runat="server" CssClass="testo_grigio" Width="440px"></asp:textbox></TD>
															    </TR>
															    <TR height="22">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="2" border="0">CAP</TD>
																    <TD>
																	    <asp:textbox id="txt_CAP" runat="server" CssClass="testo_grigio" Width="440px" MaxLength="5"></asp:textbox></TD>
															    </TR>
															    <TR height="22">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Città</TD>
																    <TD class="titolo_scheda">
																	    <asp:textbox id="txt_Citta" runat="server" CssClass="testo_grigio" Width="340px"></asp:textbox><IMG src="../../images/proto/spaziatore.gif" width="7" border="0">Prov. 
																	    &nbsp;
																	    <asp:textbox id="txt_Prov" runat="server" CssClass="testo_grigio" Width="39px" MaxLength="2"></asp:textbox></TD>
															    </TR>
															    <TR height="22">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Località</TD>
																    <TD>
																	    <asp:textbox id="txt_local" runat="server" CssClass="testo_grigio" Width="440px"></asp:textbox></TD>
															    </TR>
															    <TR height="22">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Nazione</TD>
																    <TD>
																	    <asp:textbox id="txt_Nazione" runat="server" CssClass="testo_grigio" Width="440px"></asp:textbox></TD>
															    </TR>
															    <TR height="22">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Telefono 
																	    (1)</TD>
																    <TD>
																	    <asp:textbox id="txt_Tel1" runat="server" CssClass="testo_grigio" Width="440px"></asp:textbox></TD>
															    </TR>
															    <TR height="22">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Telefono 
																	    (2)</TD>
																    <TD>
																	    <asp:textbox id="txt_Tel2" runat="server" CssClass="testo_grigio" Width="440px"></asp:textbox></TD>
															    </TR>
															    <TR height="22">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Fax</TD>
																    <TD>
																	    <asp:textbox id="txt_Fax" runat="server" CssClass="testo_grigio" Width="440px"></asp:textbox></TD>
															    </TR>
															    <TR height="22">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">C.F.</TD>
																    <TD>
																	    <asp:textbox id="txt_CodFis" runat="server" CssClass="testo_grigio" Width="440px" MaxLength="16" onchange="change = true;"></asp:textbox></TD>
															    </TR>
                                                                <TR height="22">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">P.IVA</TD>
																    <TD>
																	    <asp:textbox id="txt_PI" runat="server" CssClass="testo_grigio" Width="440px" MaxLength="11"></asp:textbox></TD>
															    </TR>
                                                            
															    <TR height="22">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">Note</TD>
																    <TD>
																	    <asp:textbox id="txt_note" runat="server" CssClass="testo_grigio" Width="440px" MaxLength="250"></asp:textbox></TD>
															    </TR>
                                                                </asp:Panel>
															    <asp:Panel id="pnlJolly" Runat="server" Visible="true">
																    <TR height="40"><td></td>
																    </TR>
															    </asp:Panel>
															    <asp:Panel ID="pnlRuoliUtente" Visible=False Runat=server>
															    <TR height="35">
																    <TD class="titolo_scheda" style="WIDTH: 100px"><IMG src="../../images/proto/spaziatore.gif" width="4" border="0">
																	    <asp:Label id="lbl_Ruoli" Runat="server" CssClass="titolo_scheda">Ruoli</asp:Label></TD>
																    <TD vAlign="top"><IMG src="../../images/proto/spaziatore.gif" width="2" border="0">
																	    <DIV style="OVERFLOW: auto; HEIGHT: 30px">
																		    <asp:Label id="lblRuoli" runat="server" CssClass="testo_grigio" Width="460px">Label</asp:Label></DIV>
																    </TD>
															    </TR>
															    </asp:Panel>
														    </TABLE>
													    </TD>
												    </TR>
											    </TABLE>
										    </asp:panel>
									    </TD>
							    </TR>
							    <tr>
								    <td>	
									    <asp:panel id="pnlRuolo" runat="server" Height="200px">
									    <TABLE class="info_grigio" id="tblUtente" height="55px" width="560px" align="center" border="0" bordercolor=#00cc00>
										    <TR height="22">
											    <TD class="titolo_scheda" style="WIDTH: 100px">Cod. rubrica</TD>
											    <TD>
												    <asp:textbox id="txt_CodR" runat="server" CssClass="testo_grigio" Width="460px" AutoPostBack="True"
													    ReadOnly="True"></asp:textbox></TD>
										    </TR>
										    <TR height="22">
											    <TD class="titolo_scheda" style="WIDTH: 100px">Descrizione *</TD>
											    <TD>
												    <asp:textbox id="txt_DescR" runat="server" CssClass="testo_grigio" Width="460px"></asp:textbox></TD>
										    </TR>
									    </TABLE>
								    </asp:panel>
							     </td>
							    </tr>
						    </table>
					    </asp:panel>
					    </td>
				    </tr>
						</div>
				    <asp:panel id="PanelListaCorrispondenti" runat="server">
					    <TR vAlign="top">
						    <TD>
							    <asp:label id="lbl_nomeLista" runat="server" Height="20px" CssClass="testo_grigio_scuro" Width="99%"
								    Font-Bold="True"></asp:label>
							    <div style="overflow-y:scroll; HEIGHT: 450px">
								    <asp:DataGrid id="dg_listCorr" runat="server" SkinID="datagrid" Width="100%" Font-Bold="True" AutoGenerateColumns="False">
									    <AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
									    <ItemStyle CssClass="bg_grigioN" Height="20px"></ItemStyle>
									    <HeaderStyle HorizontalAlign="Center" CssClass="menu_1_bianco_dg"></HeaderStyle>
									    <Columns>
										    <asp:BoundColumn DataField="CODICE" HeaderText="Codice">
											    <HeaderStyle Width="30%" Height="15px"></HeaderStyle>
										    </asp:BoundColumn>
										    <asp:BoundColumn DataField="DESCRIZIONE" HeaderText="Descrizione">
											    <HeaderStyle Width="70%"></HeaderStyle>
										    </asp:BoundColumn>
									    </Columns>
								    </asp:DataGrid>
									</div>
						    </TD>
					    </TR>
				    </asp:panel>
				    <asp:panel id="pnl_bottoniera" Runat="server">
					    <TR vAlign="top">
						    <TD vAlign="top">
							    <TABLE height="16" width="298" align="center">
								    <TR>
									    <asp:Panel id="pnl_bottonieraEsterni" Runat="server">
										    <TD align="right">
											    <cc1:imagebutton id="btn_elimina" runat="server" ImageUrl="../../images/bottoniera/../../images/bottoniera/btn_cancella_attivo.gif"
												    AlternateText="Elimina il corrispondente" DisabledUrl="../../images/bottoniera/btn_cancella_nonAttivo.gif"
												    Tipologia="GEST_RUBRICA" CommandName="Select"></cc1:imagebutton></TD>
										    <TD width="70">
											    <cc1:imagebutton id="btn_modifica" runat="server" OnClientClick="return disabledButton();" ImageUrl="../../images/bottoniera/../../images/bottoniera/btn_modifica_attivo.gif"
												    AlternateText="Modifica i dati del corrispondente" DisabledUrl="../../images/bottoniera/btn_modifica_nonAttivo.gif"
												    Tipologia="GEST_RUBRICA" BorderWidth="0px" Enabled="False"></cc1:imagebutton>
                                                    <asp:Button ID="btn_protocolla" runat="server" Text="Salva e Protocolla" Visible="false" SkinID="protocolla_attivo" CssClass="titolo_scheda" OnClientClick="return verificaCF()"/>
                                             </TD>
                                             <TD>
                                                <asp:Button ID="btn_crea_occasionale_protocolla" runat="server" Text="Salva come occasionale e protocolla" Visible="false" ToolTip="Salva il corrispondente come occasionale e procede con la protocollazione" CssClass="titolo_scheda"/>
                                             </TD>
									    </asp:Panel>
									    <asp:Panel>
										    <TD id="pnl_chiudi" align="center" runat="server">
											    <cc1:imagebutton id="btn_chiudi" runat="server" ImageUrl="../../images/proto/btn_chiudi_little_Attivo.gif"
												    DisabledUrl="../../images/proto/btn_chiudi_little_nonAttivo.gif"></cc1:imagebutton>
                                                <asp:Button ID="btn_close" runat="server" Visible="false" OnClientClick="CloseIt()" Text="Chiudi" CssClass="titolo_scheda"/>
                                            </TD>
									    </asp:Panel>
									</TR>
							    </TABLE>
						    </TD>
					    </TR>
				    </asp:panel>
			    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            </div>
		</form>
	</body>
</HTML>
