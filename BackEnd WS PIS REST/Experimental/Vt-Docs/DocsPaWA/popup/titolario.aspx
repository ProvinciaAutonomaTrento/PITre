<%@ import namespace="Microsoft.Web.UI.WebControls" %>
<%@ Register TagPrefix="mytree" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls, Version=1.0.2.226, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Register src="../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register Src="../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc1" %>
<%@ Register Src="../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="uc2" %>
<%@ Register Src="../ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Page language="c#" Codebehind="titolario.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.popup.frm_titolario" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat="server">
	    <title></title>
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<script language="javascript" src="../LIBRERIE/DocsPA_Func.js"></script>
		<LINK href="../CSS/DocsPA_30.css" type="text/css" rel="stylesheet">
		<%Response.Expires=-1;%>
		<base target="_self">
		<SCRIPT language="JavaScript">			
			
			window.name = "Foo";
			
			var oMyObject = window.dialogArguments;
			
		</SCRIPT>
	    <SCRIPT language="JavaScript">	
	    function bodyScroll()
	    {		
		    if(window.screen.availWidth<1000)
			    {
				    window.document.body.scroll='yes';
			    }
			    else
			    {
				    window.document.body.scroll='no';
			    }
	    }
		</script>
		<script language="javascript">
			function OpenFile()
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
				    //fso = CreateObject("Scripting.FileSystemObject");
				    fso = FsoWrapper_CreateFsoObject();
                    // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0
                    folder = fso.GetSpecialFolder(2);                    
                    path =  folder.Path;
					filePath = path + "\\exportDocspa.xls";
					applName = "Microsoft Excel";	
					exportUrl= "..\\exportDati\\exportDatiPage.aspx";				
					http = CreateObject("MSXML2.XMLHTTP");
					http.Open("POST",exportUrl,false);
					http.send();					
				
				       
				    var content=http.responseBody;
					if (content!=null)
					{
						AdoStreamWrapper_SaveBinaryData(filePath,content);
						
						ShellWrappers_Execute(filePath);
					}								 					
				}
				catch (ex)
				{			
				    alert(ex.message.toString());
				}						
			}
			
	        // Creazione oggetto activex con gestione errore
	        function CreateObject(objectType)
	        {
		        try { return new ActiveXObject(objectType); }
		        catch (ex) { alert("Oggetto '" + objectType + "' non istanziato"); }
		    }

		    function invioEvent(keyCode)
            {
		        if (keyCode == 13)
                {
		            var button = document.getElementById('btn_ok');
		            button.click();
		        }
		    }

		    function TreeViewDoubleClick() {
		        var button = document.getElementById('btn_ok');
		        button.click();
		    }
    </script>
	</HEAD>
	<BODY bottomMargin="0" leftMargin="5" topMargin="5" rightMargin="5" onload="bodyScroll();" >
			<form id="frm_titolario" method="post" target="Foo" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "Titolario" />
			<table class="contenitore" width="100%" align="center" border="0">
				<tr>
					<td class="menu_1_rosso" align="center">Titolario</td>
				</tr>
				<tr>
					<td valign="top">
						<TABLE class="info_grigio" height="100%" cellSpacing="3" cellPadding="0" width="97%" align="center"
							border="0">
							<TR>
								<TD>
									<TABLE cellSpacing="2" cellPadding="0" width="100%" border="0">
										<TR>
											<TD class="testo_grigio" align="right">Codice:</TD>
											<TD class="testo_grigio">
												<asp:textbox id="txt_find_cod" runat="server" CssClass="testo_grigio" Width="110px"></asp:textbox></TD>
											<TD class="testo_grigio" align="right">Descrizione:</TD>
											<TD class="testo_grigio">
												<asp:textbox id="txt_find_desc" runat="server" CssClass="testo_grigio" Width="290px"></asp:textbox></TD>
											<TD>
												<asp:imagebutton id="btn_find" runat="server" ImageUrl="../images/proto/zoom.gif" AlternateText="Trova"></asp:imagebutton>&nbsp;</TD>
										</TR>
										<tr>
										    <td class="testo_grigio" align="right">
										        <asp:Label ID="lbl_indice" runat="server" Text="Indice Sis.:" Visible="false"></asp:Label>
										    </td>
										    <TD class="testo_grigio">
										        <asp:textbox id="txt_indice" runat="server" CssClass="testo_grigio" Width="110px" Visible="false"></asp:textbox>
										        <asp:ImageButton ID="img_exportIndice" runat="server" Visible="false" 
                                                    ImageUrl="../images/proto/export_1.gif" 
                                                    ToolTip="Esporta l'indice sistematico in excel." 
                                                    onclick="img_exportIndice_Click" />
										    </TD>
										    <td class="testo_grigio" align="right">Note:</td>
										    <TD class="testo_grigio">
										        <asp:textbox id="txt_note" runat="server" CssClass="testo_grigio" Width="290px"></asp:textbox>
										    </TD>
										</tr>
										<tr>
											<TD class="testo_grigio" align="right">
											<asp:Label runat="server" ID="lbl_registri" Text="Registri:"></asp:Label></TD>
									    <td>
										  <asp:dropdownlist id="ddl_registri" runat="server" AutoPostBack="True" 
                                                CssClass="testo_grigio" Width="134px"></asp:dropdownlist>
										</td>
										</tr>
									</TABLE>
								</TD>
							</TR>
							<TR>
								<TD vAlign="top">
									<DIV id=DivTree style="OVERFLOW: auto;WIDTH:580px">
									<mytree:treeview id="TreeView1" runat="server" AutoPostBack=True 
                                            CssClass="testo_grigio" height="100%"
										HoverStyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana;background-color:#4b4b4b;"
										DefaultStyle="font-weight:normal;font-size:10px;color:#4b4b4b;text-indent:0px;font-family:Verdana;"
										SystemImagesPath="../images/alberi/left/" width="100%"
                                            SelectedStyle="font-weight:normal;font-size:10px;color:#ffffff;text-indent:0px;font-family:Verdana;background-color:#810d06;"></mytree:treeview>
									</div>
								</TD>
							</TR>
							<TR>
								<TD align="center">
									<asp:label id="lbl_msg" runat="server" CssClass="testo_red"></asp:label></TD>
							</TR>
							<asp:panel id="pnl_ric" Visible="False" Runat="server">
								<TR>
									<TD>
										<TABLE class="info_grigio" cellSpacing="0" cellPadding="0" width="100%" align="center"
											border="0">
											<TR>
												<TD>
													<TABLE cellSpacing="0" cellPadding="0" width="100%" align="center" border="0">
														<TR>
															<TD class="menu_1_rosso" align="center" width="99%">Risultato della ricerca</TD>
															<TD width="1%">
																<asp:ImageButton id="btn_chiudi_risultato" runat="server" AlternateText="Chiudi risultato ricerca"
																	ImageUrl="../images/chiude.gif"></asp:ImageButton></TD>
														</TR>
													</TABLE>
												</TD>
											</TR>
											<TR>
												<TD>
													<DIV id="DivList" style="OVERFLOW: auto; HEIGHT: 220px">
														<TABLE cellSpacing="3" cellPadding="1" width="100%" border="0">
															<TR bgColor="#eaeaea">
																<TD class="testo_grigio" align="center">Codice</TD>
																<TD class="testo_grigio" align="center">Descrizione</TD>
																<TD class="testo_grigio" align="center">Livello</TD>
															</TR>
															<asp:label id="lbl_td" Runat="server"></asp:label></TABLE>
													</DIV>
												</TD>
											</TR>
										</TABLE>
									</TD>
								</TR>
							</asp:panel>
							<TR>
								<TD align="center">
									<asp:button id="btn_ok" runat="server" CssClass="PULSANTE" Width="55px" Text="OK" Height="19px"></asp:button>&nbsp;&nbsp;&nbsp;&nbsp;
									<asp:button id="btn_chiudi" runat="server" CssClass="PULSANTE" Width="55px" Text="CHIUDI" Height="19px"></asp:button>
								</TD>
							</TR>
						</TABLE>
					</td>
				</tr>
				<tr>
					<td class="menu_1_rosso" align="center">&nbsp;</td>
				</tr>
			</table>
			<uc1:ShellWrapper ID="shellWrapper" runat="server" />
			<uc2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
            <uc3:FsoWrapper ID="fsoWrapper" runat="server" />
		</form>
	</BODY>
</HTML>
