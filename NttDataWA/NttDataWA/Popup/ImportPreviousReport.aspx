<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="ImportPreviousReport.aspx.cs" Inherits="NttDataWA.Popup.ImportPreviousReport" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="~/ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="uc2" %>
<%@ Register Src="~/ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
        
        table ul 
        {
            margin: 0;    
            padding: 0 0 0 15px;
        }
    </style>
	<script type="text/javascript">
	    var fsoApp;

	    function OpenFileApplet(typeFile) {
	        var filePath;
	        var exportUrl;
	        var http;
	        var applName;
	        var fso;
	        var folder;
	        var path;

	        if (fsoApp == undefined) {
	            fsoApp = window.document.plugins[0];
	        }
	        if (fsoApp == undefined) {
	            fsoApp = document.applets[0];
	        }

	        try {
	            // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0
	            path = fsoApp.GetSpecialFolder();

	            if (typeFile == "PDF") {
	                filePath = path + "\export.pdf";
	                applName = "Adobe Acrobat";
	            }
	            else if (typeFile == "XLS") {
	                filePath = path + "\export.xls";
	                applName = "Microsoft Excel";
	            }
	            else if (typeFile == "Model") {
	                filePath = path + "\export.xls";
	                applName = "Microsoft Excel";
	            }
	            else if (typeFile == "ODS") {
	                filePath = path + "\export.ods";
	                applName = "Open Office";
	            }

	            var encodedFilePath = EncodeHtml(filePath);
	            var paramPost = "<<filePath:" + encodedFilePath + ">>";
	            var urlPost = '<%=httpFullPath%>' + '/Popup/ImportPreviousReport_exportDatiPage.aspx';
	            if (fsoApp.saveFileFromURL(filePath, urlPost, paramPost)) {
                    fsoApp.openFile(filePath);
	                retval = true;
	            }
	            else {
	                alert('Applet error to get file.');
	                retval = false;
	            }
	        }
	        catch (ex) {
	            alert(ex.message.toString());
	        }

	        reallowOp();
	    }

	    function OpenFileSocket(typeFile) {
	        var filePath;
	        var exportUrl;
	        var http;
	        var applName;
	        var folder;
	        var path;
	        var encodedFilePath;
	        var urlPost;

	        try {
	            // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0
	            getSpecialFolder(function (path, connection) { 

	                if (typeFile == "PDF") {
	                    filePath = path + "\export.pdf";
	                    applName = "Adobe Acrobat";
	                }
	                else if (typeFile == "XLS") {
	                    filePath = path + "\export.xls";
	                    applName = "Microsoft Excel";
	                }
	                else if (typeFile == "Model") {
	                    filePath = path + "\export.xls";
	                    applName = "Microsoft Excel";
	                }
	                else if (typeFile == "ODS") {
	                    filePath = path + "\export.ods";
	                    applName = "Open Office";
	                }

	                encodedFilePath = EncodeHtml(filePath);
	                urlPost = '../Popup/ImportPreviousReport_exportDatiPage.aspx';

	                $.ajax({
	                    type: 'POST',
	                    url: urlPost,
	                    data: { "filePath": encodedFilePath },
	                    success: function (content) {
	                        //var content = response.responseText;
                                
	                        saveFile(filePath, content, function (retVal, connection) {
	                            //alert("SaveFileNoApplet 1 retVal" + retVal);
	                            if (retVal == "true") {
	                                //alert("SaveFileNoApplet 2 retVal" + retVal);
	                                openFile(filePath);
	                                reallowOp();
	                                callback(true, connection);
	                            }
	                            else {
	                                alert('Applet error to get file.');
	                                callback(false, connection);
	                            }
	                            connection.close();
	                        });


	                    },
	                    error: function () {
	                        reallowOp();
	                        sendError();
	                    },
	                    async: true
	                });

	                connection.close();
	            });
	        }
	        catch (ex) {
	            alert(ex.message.toString());
	        }


	    }

	    function EncodeHtml(value) {
	        value = escape(value);
	        value = value.replace(/\//g, "%2F");
	        value = value.replace(/\?/g, "%3F");
	        value = value.replace(/=/g, "%3D");
	        value = value.replace(/&/g, "%26");
	        value = value.replace(/@/g, "%40");
	        return value;
	    }

	    function OpenFileActiveX(typeFile) {
	        var filePath;
	        var exportUrl;
	        var http;
	        var applName;
	        var fso;
	        var folder;
	        var path;

	        try {
	            fso = FsoWrapper_CreateFsoObject();
	            // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0

	            path = fso.GetSpecialFolder(2).Path;

	            if (typeFile == "PDF") {
	                filePath = path + "\\export.pdf";
	                applName = "Adobe Acrobat";
	            }
	            else if (typeFile == "XLS") {
	                filePath = path + "\\export.xls";
	                applName = "Microsoft Excel";
	            }
	            else if (typeFile == "Model") {
	                filePath = path + "\\export.xls";
	                applName = "Microsoft Excel";
	            }
	            else if (typeFile == "ODS") {
	                filePath = path + "\\export.ods";
	                applName = "Open Office";
	            }

	            exportUrl = "ImportPreviousReport_exportDatiPage.aspx";
	            http = CreateObject("MSXML2.XMLHTTP");
	            http.Open("POST", exportUrl, false);
	            http.send();

	            var content = http.responseBody;

	            if (content != null) {
	                AdoStreamWrapper_SaveBinaryData(filePath, content);

	                ShellWrappers_Execute(filePath);

	                self.close();
	            }
	        }
	        catch (ex) {
	            alert(ex.message.toString());
	        }

	        reallowOp();
	    }

	    // Creazione oggetto activex con gestione errore
	    function CreateObject(objectType) {
	        try { return new ActiveXObject(objectType); }
	        catch (ex) { alert("Oggetto '" + objectType + "' non istanziato"); }
	    }
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
<div class="container">
    <asp:GridView ID="grvItems" runat="server" AllowSorting="false" AutoGenerateColumns="false"
        AllowPaging="False" CssClass="tbl_rounded round_onlyextreme">
        <RowStyle CssClass="NormalRow" />
        <AlternatingRowStyle CssClass="AltRow" />
        <Columns>
            <asp:TemplateField Visible="false">
                <ItemTemplate>
                    <asp:Label ID="SYSTEM_ID" runat="server" Text='<%# this.GetItemID((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="10%">
                <ItemTemplate>
                    <asp:Label ID="DATA" runat="server" Text='<%# this.GetData((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="5%">
                <ItemTemplate>
                    <asp:Label ID="ESITO" runat="server" Text='<%# this.GetEsito((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="25%">
                <ItemTemplate>
                    <asp:Label ID="ERR" runat="server" Text='<%# this.GetErrore((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="10%">
                <ItemTemplate>
                    <asp:Label ID="NDOC" runat="server" Text='<%# this.GetDocumento((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="15%">
                <ItemTemplate>
                    <asp:Label ID="NUMPROTO" runat="server" Text='<%# this.GetNumProtoExcel((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="10%">
                <ItemTemplate>
                    <asp:Label runat="server" ID="REG" Text='<%# this.GetRegistro((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="30%">
                <ItemTemplate>
                    <asp:Label runat="server" ID="Ruolo" Text='<%# this.GetUtenteRuolo((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="5%">
                <ItemTemplate>
                    <asp:Label runat="server" ID="TIPO" Text='<%# this.GetTipoOperazione((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="5%">
                <ItemTemplate>
                    <asp:Label runat="server" ID="TIPO" Text='<%# this.GetNumeroAllegati((NttDataWA.DocsPaWR.ItemReportPregressi)Container.DataItem) %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnReport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnReport_Click" OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:PlaceHolder ID="plcActiveX" runat="server" Visible="false"></asp:PlaceHolder>
    <asp:PlaceHolder ID="plcApplet" runat="server" Visible="false">
        <applet id='fsoApplet' 
            code = 'com.nttdata.fsoApplet.gui.FsoApplet' 
            codebase=  '<%=Page.ResolveClientUrl("~/Libraries/")%>'
            archive='FsoApplet.jar'
		    width = '10'   height = '9'>
            <param name="java_arguments" value="-Xms128m" />
            <param name="java_arguments" value="-Xmx512m" />
        </applet>
    </asp:PlaceHolder>
</asp:Content>
