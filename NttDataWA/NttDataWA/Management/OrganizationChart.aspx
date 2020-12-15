<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Base.Master" AutoEventWireup="true" CodeBehind="OrganizationChart.aspx.cs" Inherits="NttDataWA.Management.OrganizationChart" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Src="~/ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="~/ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper" TagPrefix="uc2" %>
<%@ Register Src="~/ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        var fsoApp

        function generateRendomExportFileName() {
            var text = "_";
            var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            for (var i = 0; i < 5; i++)
                text += possible.charAt(Math.floor(Math.random() * possible.length));

            return text;
        }

        function stampa(componentType) {
            switch(componentType){
                case("<%=NttDataWA.Utils.Constans.TYPE_APPLET%>"):
                    OpenFileApplet('PDF');
                    break;
                case("<%=NttDataWA.Utils.Constans.TYPE_SOCKET%>"):
                    OpenFileSocket('PDF');
                    break;
                default:
                    OpenFileActiveX('PDF');
                    break;
            }

            if (t == '3')
                OpenFileApplet('PDF');
            else
                OpenFileApplet('PDF');
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

            try {

                getSpecialFolder(function (path, connection) {
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
                    //var paramPost = "<<filePath:" + encodedFilePath + ">>";
                    var urlPost = '../ExportDati/exportDatiPage.aspx?isapplet=true';

                    $.ajax({
                        type: 'POST',
                        url: urlPost,
                        data: { "filePath": encodedFilePath },
                        success: function (content) {
                            //alert("afetr call is success");

                            /*if (content)
	                            alert("Call is success");
                            */
                            //var content = response.responseText;
                            connection.close();
                            saveFile(filePath, content, function (retVal, connection) {
                                //alert("SaveFileNoApplet 1 retVal" + retVal);
                                if (retVal === "true") {
                                    //alert("SaveFileNoApplet 2 retVal" + retVal);
                                    openFile(filePath);
                                    retval = true;
                                    //alert("SaveFileSystem dentro");
                                    //ShowSuccess();
                                    reallowOp();

                                }
                                else {
                                    alert('Applet error to get file.');
                                    retval = false;
                                    reallowOp();
                                    //sendError();
                                }
                                connection.close();
                            });


                        },
                        error: function () {
                            reallowOp();
                            //sendError();
                            connection.close();
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
             
                exportUrl = '<%=httpFullPath%>' + '/ExportDati/exportDatiPage.aspx';
              
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
        }

        // Creazione oggetto activex con gestione errore
        function CreateObject(objectType) {
            try { return new ActiveXObject(objectType); }
            catch (ex) { alert("Oggetto '" + objectType + "' non istanziato"); }
        }

        $(function () {
            $('#contentStandard1Column input, #contentStandard1Column textarea').keypress(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });

            $('#contentStandard1Column select').change(function (e) {
                if (e.which == 13) {
                    e.preventDefault();
                    $('.defaultAction').click();
                }
            });
        });
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="OrganizationChartSearchResult" runat="server" Url="../popup/OrganizationChartSearchResult.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" CloseFunction="function (event, ui)  {__doPostBack('UpPnlButtons', '');}" />

    <div id="containerTop">
        <asp:UpdatePanel runat="server" ID="UpHeaderProject" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTop">
                    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerStandardTop">
                                <div id="containerStandardTopSx">
                                </div>
                                <div id="containerStandardTopCx">
                                    <p>
                                        <asp:Label ID="pageTitle" runat="server" />
                                    </p>
                                </div>
                                <div id="containerStandardTopDx">
                                </div>
                            </div>
                            <div id="containerStandardBottom">
                                <div id="containerProjectCxBottom">
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <div id="containerStandard">
                <div id="content">
                    <div id="contentStandard1Column">
                        <asp:UpdatePanel ID="UpPnlFilters" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <div class="col"><asp:Literal id="lblView" runat="server" /></div>
                                    <div class="col"><asp:DropDownList ID="ddlView" runat="server" CssClass="chzn-select-deselect" Width="200" OnSelectedIndexChanged="ddlView_SelectedIndexChanged" AutoPostBack="true" /></div>
                                    <div class="col"><asp:Literal id="lblSearchIn" runat="server" /></div>
                                    <div class="col"><asp:DropDownList ID="ddlSearchIn" runat="server" CssClass="chzn-select-deselect" Width="100"
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlSearchIn_SelectedIndexChanged" /></div>

                                    <asp:PlaceHolder ID="plcCodeName" runat="server">
                                        <div class="col"><asp:Literal id="lblCode" runat="server" /></div>
                                        <div class="col"><cc1:CustomTextArea ID="txtCode" runat="server" CssClass="txt_textdata2" CssClassReadOnly="txt_textdata2_disabled" /></div>
                                        <div class="col"><asp:Literal id="lblName" runat="server" /></div>
                                        <div class="col"><cc1:CustomTextArea ID="txtName" runat="server" CssClass="txt_number" CssClassReadOnly="txt_number_disabled" /></div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plcRF" runat="server" Visible="false">
                                        <div class="col"><asp:Literal id="lblRF" runat="server" /></div>
                                        <div class="col"><asp:DropDownList ID="ddlRF" runat="server" CssClass="chzn-select-deselect" Width="400" /></div>
                                    </asp:PlaceHolder>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:UpdatePanel ID="UpPnlTree" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="row">
                                    <cc1:OrganizationChartTreeView2 ID="treeViewUO" runat="server" CssClass="TreeAddressBook" OnTreeNodeExpanded="treeViewUO_TreeNodeExpanded" EnableViewState="true">
                                        <SelectedNodeStyle BackColor="#FCD85C" />
                                    </cc1:OrganizationChartTreeView2>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" UpdateMode="Conditional" runat="server" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnSearch" runat="server" CssClass="btnEnable defaultAction" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnSearch_Click" />
            <cc1:CustomButton ID="BtnStart" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnStart_Click" />
            <cc1:CustomButton ID="BtnRoot" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnRoot_Click" />
            <cc1:CustomButton ID="BtnPrint" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClick="BtnPrint_Click" />

            <asp:HiddenField ID="hd_returnValueModal" runat="server" />
            <asp:HiddenField ID="hd_lastReturnValueModal" runat="server" />
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
