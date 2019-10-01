<%@ Page Language="C#" AutoEventWireup="true" Codebehind="elencoPiani.aspx.cs" Inherits="DocsPAWA.gestione.piani.elencoPiani" %>

<%@ Register Src="../../ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper"
    TagPrefix="uc2" %>

<%@ Register Src="../../ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc1" %>


<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head>
    <title>DOCSPA > regElenco</title>
    <meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
    <meta content="C#" name="CODE_LANGUAGE">
    <meta content="JavaScript" name="vs_defaultClientScript">
    <meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">

    <script language="javascript" src="../../LIBRERIE/DocsPA_Func.js"></script>

    <link href="../../CSS/docspa_30.css" type="text/css" rel="stylesheet">

    <script language="javascript" id="btn_stampa" event="onclick()" for="btn_stampa">
		
		if(document.getElementById('ddl_registro').value!='')
			{
				top.principale.frames[1].location='waitingpage.htm';
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
				    fso = CreateObject("Scripting.FileSystemObject");
                    // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0
                    folder = fso.GetSpecialFolder(2);                    
                    path =  folder.Path;
					filePath = path + "\\exportDocspa.xls";
					applName = "Microsoft Excel";	
					exportUrl= "..\\..\\exportDati\\exportDatiPage.aspx";				
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
				}						
			}
			
	        // Creazione oggetto activex con gestione errore
	        function CreateObject(objectType)
	        {
		        try { return new ActiveXObject(objectType); }
		        catch (ex) { alert("Oggetto '" + objectType + "' non istanziato"); }	
	        }	
    </script>

</head>
<body leftmargin="1" ms_positioning="GridLayout">
    <form id="elencoPiani" method="post" runat="server">
        <table id="tbl_contenitore" height="100%" cellspacing="0" cellpadding="0" width="408"
            align="center" border="0">
            <tr valign="top" align="center">
                <td>
                    <table width="100%" class="info" cellspacing="1" cellpadding="1" align="center" border="0">
                        <tr>
                            <td class="item_editbox" colspan="2">
                                <p class="boxform_item">
                                    Gestione Piani di Rientro</p>
                            </td>
                        </tr>
                        <tr>
                            <td height="5">
                            </td>
                        </tr>
                        <tr>
                            <td class="titolo_scheda" style="width: 117px; height: 3px">
                                &nbsp;Registro *</td>
                            <td style="height: 3px">&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:dropdownlist id="ddl_registro" tabindex="2" runat="server" width="200px" cssclass="testo_grigio"
                                    autopostback="True"></asp:dropdownlist>
                            </td>
                        </tr>
                        <tr>
                            <td height="5">
                            </td>
                        </tr>
                        <tr>
                            <td class="titolo_scheda" valign="middle" height="19">
                                &nbsp;Data Scadenza *</td>
                            <td>
                                <asp:label id="lbl_initDataScadenza" runat="server" cssclass="testo_grigio" width="18px">Da</asp:label>
                                <cc1:datemask id="txt_initDataScadenza" runat="server" cssclass="testo_grigio" width="80px"></cc1:datemask>
                                <font size="1">&nbsp;&nbsp;&nbsp; </font>
                                <asp:label id="lbl_fineDataScadenza" runat="server" cssclass="testo_grigio" width="18px">A</asp:label>
                                <cc1:datemask id="txt_fineDataScadenza" runat="server" cssclass="testo_grigio" width="80px"></cc1:datemask>
                                <font size="1"></font>
                            </td>
                        </tr>
                        <tr height="100%">
                            <td>&nbsp;</td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr height="10%">
                <td>
                    <!-- BOTTONIERA -->
                    <table id="tbl_bottoniera" cellspacing="0" cellpadding="0" border="0" align="center">
                        <tr>
                            <td valign="top" height="5">
                                <img height="1" src="../images/proto/spaziatore.gif" width="1" border="0"></td>
                        </tr>
                        <tr>
                            <td>
                                <cc1:imagebutton id="btn_stampa" runat="server" alternatetext="Stampa report" imageurl="../../images/bottoniera/btn_stampa_Attivo.gif"></cc1:imagebutton>
                            </td>
                        </tr>
                    </table>
                    <!--FINE BOTTONIERA -->
                </td>
            </tr>
        </table>
        <uc2:AdoStreamWrapper ID="adoStreamWrapper" runat="server" />
        <uc1:ShellWrapper ID="shellWrapper" runat="server" />
    </form>
</body>
</html>
