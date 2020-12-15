<%@ Page language="c#" Codebehind="exportDatiSelection.aspx.cs" AutoEventWireup="true" Inherits="DocsPAWA.exportDati.exportDatiSelection" %>

<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>

<%@ Register Src="../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper"
    TagPrefix="uc2" %>

<%@ Register Src="../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc1" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
		<title></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/docspA_30.css" type="text/css" rel="stylesheet">
		<base target="_self"/>
		<script language="javascript">
			function OpenFile(typeFile)
			{
				var filePath;
				var exportUrl;
				var http;
				var applName;				
				var fso;                
                var folder;                    
                var path;

				try
				{
				    fso = FsoWrapper_CreateFsoObject();
                    // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0

                    path = fso.GetSpecialFolder(2).Path;                    
                  
					if(typeFile=="PDF")
					{
						filePath = path + "\\exportDocspa.pdf";
						applName = "Adobe Acrobat";						
					}
					else if(typeFile=="XLS")
					{
					    filePath = path + "\\exportDocspa.xls";
						applName = "Microsoft Excel";
		            }
		            else if (typeFile == "Model") {
		                filePath = path + "\\exportDocspa.xls";
		                applName = "Microsoft Excel";
		            }
		            else if (typeFile == "ODS") {
		                filePath = path + "\\exportDocspa.ods";
		                applName = "Open Office";
		            }

					exportUrl= "exportDatiPage.aspx";				
					http = CreateObject("MSXML2.XMLHTTP");
					http.Open("POST",exportUrl,false);
					http.send();					
					
					var content=http.responseBody;
					
					if (content!=null)
					{
						AdoStreamWrapper_SaveBinaryData(filePath,content);
						
						ShellWrappers_Execute(filePath);
						
						self.close();
					}
				}
				catch (ex)
				{			
				alert(ex.message.toString());
				
				//	alert("Impossibile aprire il file generato!\n\nPossibili motivi:\n- il browser non è abilitato ad eseguire controlli ActiveX\n- il sito intranet DocsPA non compare tra i siti attendibili di questo computer;\n- estensione '"+typeFile+"' non associata all'applicazione "+applName+";\n- "+applName+" non installato su questo computer;\n- applicazione "+applName+" relativa ad esportazioni precedentente effettuate ancora attiva.");					
				}						
			}
			
	        // Creazione oggetto activex con gestione errore
	        function CreateObject(objectType)
	        {
		        try { return new ActiveXObject(objectType); }
		        catch (ex) { alert("Oggetto '" + objectType + "' non istanziato"); }	
	        }
	    </script>
		<script language="javascript" id="btn_export_click" event="onclick()" for="btn_export">
			window.document.body.style.cursor='wait';
			var w_width = 450;
			var w_height = 220;							
			document.getElementById ("WAIT").style.top = 0;
			document.getElementById ("WAIT").style.left = 0;
			document.getElementById ("WAIT").style.width = w_width;
			document.getElementById ("WAIT").style.height = w_height;				
			document.getElementById ("WAIT").style.visibility = "visible";
		</script>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Esporta risultati della ricerca" />
		    <input id="hd_export" type="hidden" name="hd_export" runat="server">
			<table height="100%" cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
				<tr>
					<td class="testo_grigio_scuro" style="padding-left:10px;" colspan="2">
			        Seleziona il formato:
					</td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro"  colspan="2">
					    <asp:RadioButtonList ID="rbl_XlsOrPdf" runat="server" CssClass="testo_grigio_scuro" align="center" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="rbl_XlsOrPdf_SelectedIndexChanged">
					        <asp:ListItem Selected="True" Text="Adobe Acrobat&#160;&lt;img src='../images/tabDocImages/icon_pdf.gif' border='0'&gt;" Value="PDF"></asp:ListItem>
					        <asp:ListItem Text="Microsoft Excel&#160;&lt;img src='../images/tabDocImages/icon_excel.gif' border='0'&gt;" Value="XLS"></asp:ListItem>
                            <asp:ListItem Text="Open Office&#160;&lt;img src='../images/tabDocImages/calc_icon.gif' border='0'&gt;" Value="ODS"></asp:ListItem>
					    </asp:RadioButtonList>						
					</td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" style="padding-left:10px;" colspan="2">
					    Associa un titolo:
					</td>
				</tr>
				<tr>
				    <td class="testo_grigio_scuro" style="padding-left:10px;" colspan="2">
						<asp:TextBox ID="txt_titolo" Runat="server" TextMode="MultiLine" Rows="2" MaxLength="200" Width="98%"
							CssClass="testo_grigio_scuro" tabIndex="1"></asp:TextBox>
				    </td>
				</tr>
			    <tr>
	                <td class="testo_grigio_scuro" style="padding-left:10px;">
	                    <br /><asp:Label ID="lbl_selezionaCampo" runat="server" Text="Seleziona uno o più campi:" Width="200px"></asp:Label>
	                </td>
	                <td class="testo_grigio_scuro" style="padding-right: 37px;" align="right">
	                    <asp:CheckBox ID="cb_selezionaTutti" runat="server" oncheckedchanged="cb_selezionaTutti_CheckedChanged" TextAlign="Left" AutoPostBack="True" Text="Seleziona tutti " />
	                </td>
	            </tr>
	            <tr>
                <td class="testo_grigio_scuro" style="padding-left:10px;" colspan="2" valign="top" height="100%" >
                    <asp:Panel id="panel_listaCampi" runat="server" Width="98%" Height="242px" ScrollBars="Vertical">
                    <asp:GridView ID="gv_listaCampi" SkinID="gridview" runat="server" Width="100%" Height="242px" AutoGenerateColumns="False">
                        <AlternatingRowStyle CssClass="bg_grigioA"></AlternatingRowStyle>
                        <RowStyle CssClass="bg_grigioN"></RowStyle>
                        <HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
                        <Columns>
                            <asp:BoundField DataField="CAMPO_STANDARD" HeaderText="CAMPO_STANDARD" />
                            <asp:BoundField DataField="CAMPO_COMUNE" HeaderText="CAMPO_COMUNE" />
                            <asp:BoundField HeaderText="Campo" DataField="CAMPI">
                                <HeaderStyle Width="90%" />
                            </asp:BoundField>
                            <asp:TemplateField>
                                <HeaderStyle Width="10%" />
                                <ItemTemplate>
                                    <asp:CheckBox ID="cb_selezioneCampo" runat="server" Checked="True" />
                                    <asp:HiddenField ID="hfLongDescriuption" runat="server" Value="<%# this.GetLongDescription((System.Data.DataRowView)Container.DataItem) %>" />
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                            <asp:BoundField DataField="VISIBILE" HeaderText="VISIBILE" Visible = "false" />
                        </Columns>
                    </asp:GridView>  
                    </asp:Panel>                
                </td>                    
                </tr>
                <tr>
					<td class="testo_grigio_scuro" style="padding:5px;" align="center" colspan="2">
					    <asp:button id="btn_export" Runat="server" Text="Esporta" CssClass="pulsante" tabIndex="2"></asp:button>&nbsp;&nbsp;
					    <asp:button id="btn_annulla" Runat="server" Text="Annulla" CssClass="pulsante" tabIndex="3"></asp:button>
					</td>
				</tr>
			</table>
			<DIV id="WAIT" style="BORDER-RIGHT: #f5f5f5 1px solid; BORDER-TOP: #f5f5f5 1px solid; VISIBILITY: hidden; BORDER-LEFT: #f5f5f5 1px solid; BORDER-BOTTOM: #f5f5f5 1px solid; POSITION:absolute; BACKGROUND-COLOR: #f5f5f5;">
				<table height="420px" cellSpacing="0" cellPadding="0" width="100%" border="0">
					<tr>
						<td style="FONT-SIZE: 11pt; FONT-FAMILY: Verdana" vAlign="top" align="center">
						    <br><br><br><br>
						    Attendere prego,<br><br>
						    esportazione dei dati in corso...<br><br>
						</td>
					</tr>
				</table>
			</DIV>
			<uc1:ShellWrapper ID="shellWrapper" runat="server" />
            <uc2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
            <uc3:FsoWrapper ID="fsoWrapper" runat="server" />			
		</form>
	</body>
</HTML>
