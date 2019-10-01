<%@ Page language="c#" Codebehind="exportLog.aspx.cs" AutoEventWireup="false" Inherits="SAAdminTool.AdminTool.Gestione_Logs.exportLog" %>
<%@ Register Src="../../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="../../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="uc2" %>
<%@ Register Src="../../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc1" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../../CSS/docspA_30.css" type="text/css" rel="stylesheet">
		<base target="_self"/>
		<script language="javascript">
			function OpenFile(typeFile, titolo)
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
					filePath = path + "\\" + titolo;
                  
					if(typeFile=="PDF")
					{
						applName = "Adobe Acrobat";						
					}
					else if(typeFile=="XLS")
					{
						applName = "Microsoft Excel";	
					}
					exportUrl= "exportLogPage.aspx";				
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
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Esporta risultati della ricerca log" />
		    <uc1:ShellWrapper ID="shellWrapper" runat="server" />
            <uc2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
            <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
			<input id="hd_export" type="hidden" name="hd_export" runat="server">
			<table height="180" cellSpacing="0" cellPadding="0" width="450" align="center" border="0">
				<tr>
					<td class="testo_grigio_scuro" vAlign="middle">&nbsp;&nbsp;Seleziona il formato:</td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" align="center">
						<asp:radiobutton id="rd_formatoPDF" Runat="server" Text="&nbsp;Adobe Acrobat&nbsp;<img src='../../images/tabDocImages/icon_pdf.gif' border='0'>"
							TextAlign="Right" GroupName="rd_formato" CssClass="testo_grigio_scuro" Checked="True"></asp:radiobutton>
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<asp:radiobutton id="rd_formatoXLS" Runat="server" Text="&nbsp;Microsoft Excel&nbsp;<img src='../../images/tabDocImages/icon_excel.gif' border='0'>"
							TextAlign="Right" GroupName="rd_formato" CssClass="testo_grigio_scuro"></asp:radiobutton>
					</td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro">&nbsp;&nbsp;associa un titolo:<br>
						&nbsp;&nbsp;<asp:TextBox ID="txt_titolo" Runat="server" TextMode="MultiLine" Rows="2" MaxLength="200" Width="325pt"
							CssClass="testo_grigio_scuro" tabIndex="1"></asp:TextBox></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" valign="middle">
						<table border="0" cellpadding="0" cellspacing="0" width="100%">
							<tr>
								<td width="50%" align="center">
									<asp:button id="btn_export" Runat="server" Text="Esporta" CssClass="pulsante" tabIndex="2"></asp:button>
								</td>
								<td width="50%" align="center">
									<asp:button id="btn_annulla" Runat="server" Text="Annulla" CssClass="pulsante" tabIndex="3"></asp:button>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<DIV id="WAIT" style="BORDER-RIGHT: #f5f5f5 1px solid; BORDER-TOP: #f5f5f5 1px solid; VISIBILITY: hidden; BORDER-LEFT: #f5f5f5 1px solid; BORDER-BOTTOM: #f5f5f5 1px solid; POSITION: absolute; BACKGROUND-COLOR: #f5f5f5">
				<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
					<tr>
						<td style="FONT-SIZE: 11pt; FONT-FAMILY: Verdana" vAlign="middle" align="center">Attendere 
							prego,<br><br>esportazione dei dati in corso...<br><br>
						</td>
					</tr>
				</table>
			</DIV>
		</form>
	</body>
</HTML>
