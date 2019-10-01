<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="visualReport_iframe.aspx.cs" Inherits="NttDataWA.Popup.visualReport_iframe" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/ActivexWrappers/FsoWrapper.ascx" TagName="FsoWrapper" TagPrefix="uc3" %>
<%@ Register Src="~/ActivexWrappers/AdoStreamWrapper.ascx" TagName="AdoStreamWrapper"
    TagPrefix="uc2" %>
<%@ Register Src="~/ActivexWrappers/ShellWrapper.ascx" TagName="ShellWrapper" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
        p
        {
            text-align: center;
            margin: 20% auto 0 auto;
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
                    filePath = path + "\\export.pdf";
                    applName = "Adobe Acrobat";
                }
                else {
                    filePath = path + "\\ricevuta.rtf";
                    applName = "Microsoft Office";
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
                var urlPost = '<%=httpFullPath%>' + '/Popup/visualReport.aspx';

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
                // parametri:  - SystemFolder = 1  - TemporaryFolder = 2  - WindowsFolder = 0

                getSpecialFolder(function (path, connection) {
					connection.close();
                    if (typeFile == "PDF") {
                        filePath = path + "\\export.pdf";
                        applName = "Adobe Acrobat";
                    }
                    else {
                        filePath = path + "\\ricevuta.rtf";
                        applName = "Microsoft Office";
                    }

                    var encodedFilePath = EncodeHtml(filePath);
                    var paramPost = "<<filePath:" + encodedFilePath + ">>";
                    var urlPost = '../Popup/visualReport.aspx?isSocket=true';

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
                    filePath = path + "\\export.pdf";
                    applName = "Adobe Acrobat";
                }
                else {

                    filePath = path + "\\ricevuta.rtf";
                    applName = "Microsoft Office";

                    

                }
                exportUrl = "visualReport.aspx";
                var urlPost = '<%=httpFullPath%>' + '/Popup/visualReport.aspx';

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
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <div class="row">
            <p>
                <asp:Literal ID="litMessage" runat="server" /></p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
        OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" />
    <asp:PlaceHolder ID="plcActiveX" runat="server" Visible="false"></asp:PlaceHolder>
    <asp:PlaceHolder ID="plcApplet" runat="server" Visible="false">
        <applet id='fsoApplet' code='com.nttdata.fsoApplet.gui.FsoApplet' codebase='<%=Page.ResolveClientUrl("~/Libraries/")%>'
            archive='FsoApplet.jar' width='10' height='9'>
            <param name="java_arguments" value="-Xms128m" />
            <param name="java_arguments" value="-Xmx512m" />
        </applet>
    </asp:PlaceHolder>
</asp:Content>
