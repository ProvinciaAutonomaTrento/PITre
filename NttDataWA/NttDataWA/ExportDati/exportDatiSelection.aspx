<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="exportDatiSelection.aspx.cs" Inherits="NttDataWA.ExportDati.exportDatiSelection" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
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
        
        .rblSmall {font-size: 80%;}
    </style>
	<script type="text/javascript">
	    var fsoApp;

	    function generateRendomExportFileName() {
	        var text = "_";
	        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

	        for (var i = 0; i < 5; i++)
	            text += possible.charAt(Math.floor(Math.random() * possible.length));

	        return text;
	    }

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

	        /*
	        var i = 0;
	        var found = false;
	        while (!found && i<=document.applets.length) {
	        try {
	        fsoApp.GetSpecialFolder();
	        found = true;
	        }
	        catch (ex2) {
	        fsoApp = document.applets[i];
	        }
	        i++;
	        }
	        */
	        try {
	            // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0

	            path = fsoApp.GetSpecialFolder();

	            if (typeFile == "PDF") {
	                filePath = path + "\export" + generateRendomExportFileName() +".pdf";
	                applName = "Adobe Acrobat";
	            }
	            else if (typeFile == "XLS") {
	                filePath = path + "\export" + generateRendomExportFileName() + ".xls";
	                applName = "Microsoft Excel";
	            }
	            else if (typeFile == "Model") {
	                filePath = path + "\export" + generateRendomExportFileName() + ".xls";
	                applName = "Microsoft Excel";
	            }
	            else if (typeFile == "ODS") {
	                filePath = path + "\export" + generateRendomExportFileName() + ".ods";
	                applName = "Open Office";
	            }
	            /*
	            var status = 0;
	            var content = '';
	            $.ajax({
	            type: 'POST',
	            cache: false,
	            processData: false,
	            url: "exportDatiPage.aspx?isapplet=true",
	            success: function (data, textStatus, jqXHR) {
	            status = jqXHR.status;
	            content = jqXHR.responseText;
	            },
	            error: function (jqXHR, textStatus, errorThrown) {
	            status = textStatus;
	            content = null;
	            },
	            async: false
	            });

	            if (content != null && status == 200) {
	            if (fsoApp.saveFile(filePath, content)) {
	            fsoApp.openFile(filePath);
	            }
	            else {
	            alert('Error saving file ' + filePath);
	            }
	            //self.close();
	            }
	            */
	            var encodedFilePath = EncodeHtml(filePath);
	            var paramPost = "<<filePath:" + encodedFilePath + ">>";
	            var urlPost = '<%=httpFullPath%>' + '/ExportDati/exportDatiPage.aspx';

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
	    }

		function OpenFileSocket(typeFile) {
	        var filePath;
	        var exportUrl;
	        var http;
	        var applName;
	        var fso;
	        var folder;
	        var path;
    
	        disallowOp('Content1');
	        try {
	            // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0
	            getSpecialFolder(function (path, connection) {
	                connection.close();
	                if (typeFile == "PDF") {
	                    filePath = path + "\export" + generateRendomExportFileName() + ".pdf";
	                    applName = "Adobe Acrobat";
	                }
	                else if (typeFile == "XLS") {
	                    filePath = path + "\export" + generateRendomExportFileName() + ".xls";
	                    applName = "Microsoft Excel";
	                }
	                else if (typeFile == "Model") {
	                    filePath = path + "\export" + generateRendomExportFileName() + ".xls";
	                    applName = "Microsoft Excel";
	                }
	                else if (typeFile == "ODS") {
	                    filePath = path + "\export" + generateRendomExportFileName() + ".ods";
	                    applName = "Open Office";
	                }
	                var encodedFilePath = EncodeHtml(filePath);
	                var paramPost = "<<filePath:" + encodedFilePath + ">>";
	                var urlPost = '../ExportDati/exportDatiPage.aspx?isapplet=true';

	                $.ajax({
	                    type: 'POST',
	                    url: urlPost,
	                    data: { "filePath": encodedFilePath },
	                    success: function (content) {
	                        saveFile(filePath, content, function (retVal, connection) {
	                            connection.close();
	                            if (retVal === "true") {
	                                openFile(filePath);
	                                retval = true;
	                                reallowOp();

	                            }
	                            else {
	                                alert('Applet error to get file.');
	                                retval = false;
	                                reallowOp();
	                            }
	                        });
	                    },
	                    error: function () {
	                        reallowOp();
	                    },
	                    async: true
	                });
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
	                filePath = path + "\\export" + generateRendomExportFileName() + ".pdf";
	                applName = "Adobe Acrobat";
	            }
	            else if (typeFile == "XLS") {
	                filePath = path + "\\export" + generateRendomExportFileName() + ".xls";
	                applName = "Microsoft Excel";
	            }
	            else if (typeFile == "Model") {
	                filePath = path + "\\export" + generateRendomExportFileName() + ".xls";
	                applName = "Microsoft Excel";
	            }
	            else if (typeFile == "ODS") {
	                filePath = path + "\\export" + generateRendomExportFileName() + ".ods";
	                applName = "Open Office";
	            }

	            exportUrl = "exportDatiPage.aspx";
	            http = CreateObject("MSXML2.XMLHTTP");
	            http.Open("POST", exportUrl, false);
	            http.send();

	            var content = http.responseBody;

	            if (content != null) {
	                AdoStreamWrapper_SaveBinaryData(filePath, content);

	                ShellWrappers_Execute(filePath);

	                //self.close();
	            }
	        }
	        catch (ex) {
	            alert(ex.message.toString());
	        }
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
    <div class="row">
        <div class="colonnasx"><asp:Literal ID="litSelectFormat" runat="server" /></div>
        <div class="colonnadx">
			<asp:RadioButtonList ID="rbl_XlsOrPdf" runat="server" CssClass="rblSmall" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="rbl_XlsOrPdf_SelectedIndexChanged">
				<asp:ListItem Selected="True" Text="Adobe Acrobat&#160;&lt;img src='../Images/Icons/small_pdf.png' border='0'&gt;" Value="PDF" onchange="disallowOp('Content2');"></asp:ListItem>
				<asp:ListItem Text="Microsoft Excel&#160;&lt;img src='../Images/Icons/small_xls2.png' border='0'&gt;" Value="XLS" onchange="disallowOp('Content2');"></asp:ListItem>
                <asp:ListItem Text="Open Office&#160;&lt;img src='../Images/Icons/small_odt.png' border='0'&gt;" Value="ODS" onchange="disallowOp('Content2');"></asp:ListItem>
			</asp:RadioButtonList>	
        </div>
    </div>
    <div class="row">
        <div class="colonnasx"><asp:Literal ID="litAssociateTitle" runat="server" /></div>
        <div class="colonnadx">
            <cc1:CustomTextArea ID="txt_titolo" runat="server" TextMode="MultiLine" Height="70px" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" />
        </div>
    </div>
    <div class="row">
        <div class="col"><asp:Literal ID="lbl_selezionaCampo" runat="server"></asp:Literal></div>
        <div class="col">
            <asp:CheckBox ID="cb_selezionaTutti" runat="server" oncheckedchanged="cb_selezionaTutti_CheckedChanged" TextAlign="Left" AutoPostBack="True" onclick="disallowOp('Content2');" />
        </div>
    </div>

            <asp:Panel id="panel_listaCampi" runat="server" Width="98%" Height="220px" ScrollBars="Vertical">
                <asp:GridView ID="gv_listaCampi" runat="server" CssClass="tbl_rounded_custom round_onlyextreme" AutoGenerateColumns="False">
                    <RowStyle CssClass="NormalRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <PagerStyle CssClass="recordNavigator2" />
                    <Columns>
                        <asp:BoundField DataField="CAMPO_STANDARD" />
                        <asp:BoundField DataField="CAMPO_COMUNE" />
                        <asp:BoundField DataField="CAMPI">
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
                        <asp:BoundField DataField="VISIBILE" Visible="false" />
                    </Columns>
                </asp:GridView>  
            </asp:Panel>  

</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnExport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnExport_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2');" />

            <input id="hd_export" type="hidden" name="hd_export" runat="server" />
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
