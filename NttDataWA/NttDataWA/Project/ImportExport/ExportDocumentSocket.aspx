<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="ExportDocumentSocket.aspx.cs" Inherits="NttDataWA.Project.ImportExport.ExportDocumentSocket" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register src="ExportProjectApplet.ascx" tagname="ExportProjectApplet" tagprefix="uc4" %>


<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
    var fsoApp;
    var retVal = false;
    var lastPath = "<%=this.getLastPath()%>";

    function ShowWaitingPage(msg) {
        wndAttendi = window.open('tempPageExport.aspx', 'Operazione in corso...', "location=0,toolbar=0,scrollbars=0,resizable=0,closeable=0,width=1,height=1,left=450,top=450");
    }

    function CloseWaitingPage() {
        if (wndAttendi != null)
            wndAttendi.close();
    }

    function attendi() {
        disallowOp('contentExport');

        if ($.browser.chrome)
            ShowWaitingPage("L\'operazione puo\' richiedere alcuni minuti...");
        //retVal = false;
        //window.setTimeout(confirmAction_function(), 8000);
        //confirmAction_function();
        //alert(document.getElementById("imgBarra").style.visibility);
        //document.getElementById("divImg").innerHTML = "";
        //document.getElementById("imgBarra").src = "../Images/common/loading.gif";
        //alert(document.getElementById("imgBarra").style.visibility);
        return false;
    }

<%--    function confirmAction() {
        var finalPath = FisicalFolderPath();
        var idProject = '<%=getProjectId%>';
        var urlPost = '<%=httpFullPath%>' + '/Project/ImportExport/GetXMLProject.aspx?ID=' + idProject;
        var urlDoc = '<%=httpFullPath%>' + '/Project/ImportExport/getFileInProject.aspx';

        if (finalPath != lastPath)
            setLastFolderInSession(finalPath);

        if (finalPath != null && finalPath != '') {
            if (exportFascicolo(finalPath, urlPost, idProject, urlDoc)) {
                reallowOp();
                retVal = true;
            }
            else {
                reallowOp();
                ajaxDialogModal('Path_AccessDenied', 'error', '');
            }
        }
        else {
            reallowOp();
        }

        CloseWaitingPage();
        return retVal;
    }--%>

    function confirmAction(callback) {
        FisicalFolderPath(function (finalPath) {
            var idProject = '<%=getProjectId%>';
            var urlPost = '../ImportExport/GetXMLProject.aspx?ID=' + idProject;
            //var urlDoc = '<%=httpFullPath%>' + '/Project/ImportExport/getFileInProject.aspx';
            var urlDoc = '<%=httpFullPath%>' + '/Project/ImportExport/getFileInProject.aspx';

            if (finalPath != lastPath)
                setLastFolderInSession(finalPath);

            if (finalPath != null && finalPath != '') {
                exportFascicolo(finalPath, urlPost, idProject, urlDoc, function (ret) {
                    if (ret) {
                        reallowOp();
                        retVal = true;
                    }
                    else {
                        reallowOp();
                        ajaxDialogModal('Path_AccessDenied', 'error', '');
                    }
                    callback();
                    CloseWaitingPage();
                });
            }
            else {
                reallowOp();
            }

        });
    }

    function exportFascicolo(finalPath, urlPost, idProject, urlDoc, callback) {
        var content = null;
        var status = null;
        var procede = true;
        var userInfo = null;

        if (idProject != null && idProject != "") {
            $.ajax({
                type: 'POST',
                url: urlPost,
                success: function (data, textStatus, jqXHR) {
                    status = jqXHR.status;
                    content = jqXHR.responseText;
                    //status = textStatus;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    content = textStatus;
                },
                async: false
            });

            if (status != 200) {
                // Si è verificato un errore, reperimento del messaggio
                docStatus = false;
                docStatusDescription = content;
                procede = false;
            }

            urlPost = '../ImportExport/GetInfoUserJSON.aspx';
            $.ajax({
                type: 'POST',
                url: urlPost,
                success: function (data, textStatus, jqXHR) {
                    status = jqXHR.status;
                    userInfo = jqXHR.responseText;
                    //status = textStatus;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    content = textStatus;
                },
                async: false
            });

            if (status != 200) {
                // Si è verificato un errore, reperimento del messaggio
                docStatus = false;
                docStatusDescription = content;
                procede = false;
            }
        }

        if (procede) {
            projectToFS(finalPath, content, urlDoc, userInfo, function (resultString, connection) {
                document.getElementById('hdResult').value = resultString;
                callback(true);
            });
        }
    }

    function FisicalFolderPath(callback) {
        var folderPath = fixPath(document.getElementById("txtFolderPath").value);

        if (folderPath != null && folderPath != '') {
            folderExists(folderPath, function (ret, connection) {

                if (ret === 'true') {
                    callback(folderPath);
                }
                else {
                    if (confirm('<%=this.GetMessage("CREATE_PATH")%>')) {
                    createFolder(folderPath, function (ret, connection) {

                        if (ret === 'true') {
                            callback(folderPath);
                        }
                        else {
                            ajaxDialogModal('Path_AccessDenied', 'error', '');
                            folderPath = '';
                        }
                        connection.close();
                    });
                }
            }

                connection.close();
            });
    }
}

function setTempFolder() {

    folderPath = lastPath;

    if (folderPath == null || folderPath == '') {
        try {
            disallowOp('contentExport');
            getSpecialFolder(function (folderPath, connection) {
                if (folderPath != null && folderPath != '') {
                    document.getElementById("txtFolderPath").value = shortfolderPath(folderPath);
                }
                reallowOp();
                connection.close()
            });

        }
        catch (ex) {
            alert(ex.toString());
            reallowOp();
        }
    }
    else {
        document.getElementById("txtFolderPath").value = fixPath(folderPath);
    }
}

function shortfolderPath(longpath) {
    var tempPath = "";
    var position = longpath.toUpperCase().indexOf("APPDATA");
    if (position > 0) {
        tempPath = longpath.substring(0, position) + "Documents";
    }
    else {
        tempPath = longpath;
    }

    return tempPath;
}

    function SelectFolder() {

        var actualFolder = document.getElementById("txtFolderPath");
        var btnBrowseForFolder = document.getElementById("btnBrowseForFolder");

        btnBrowseForFolder.disabled = true;
        //var folder = fsoApp.selectFolder
        selectFolder('<%=this.GetMessage("SELECT_PATH")%>', actualFolder.value, function (folder, connection) {
            if (folder != "") {
                folderExists(folder,
                    function (exist, connection) {
                        if ("true" === exist) {
                            actualFolder.value = folder;
                            btnBrowseForFolder.disabled = false;
                        }
                        else {
                            if (confirm('<%=this.GetMessage("CREATE_PATH")%>')) {

                                createFolder(folder, function (create, connection) {
                                    if ("true" === create) {
                                        actualFolder.value = folder;
                                    }
                                    else {
                                        ajaxDialogModal('Path_AccessDenied', 'error', '');
                                        actualFolder.value = '';
                                    }
                                    btnBrowseForFolder.disabled = false;
                                    connection.close();
                                });
                            } else {
                                btnBrowseForFolder.disabled = false;
                            }
                        }
                        connection.close();
                    });
            } else {
                btnBrowseForFolder.disabled = false;
            }
            connection.close();
        });
    }

        function setLastFolderInSession(strfold) {
            try {
                var content = null;
                var status = null;
                var completeUrl = "<%=httpFullPath%>" + "/Project/ImportExport/setLastPath.aspx?lastPath=" + strfold;

            $.ajax({
                type: 'POST',
                cache: false,
                //dataType: "text",
                processData: false,
                url: completeUrl,
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
        }
        catch (ex) {
            alert(ex);
        }
    }

    function fixPath(tempPath) {
        strResult = '';
        totCh = tempPath.length;
        if (totCh > 0) {
            ch = tempPath.substring(totCh - 1, totCh);
            if (ch == '\\')
                strResult = tempPath;
            else
                strResult = tempPath + '\\';
        }

        return strResult;
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

    var Url = {
        // public method for url encoding
        encode: function (string) {
            return escape(this._utf8_encode(string));
        },


        // private method for UTF-8 encoding
        _utf8_encode: function (string) {
            string = string.replace(/\r\n/g, "\n");
            var utftext = "";

            for (var n = 0; n < string.length; n++) {

                var c = string.charCodeAt(n);

                if (c < 128) {
                    utftext += String.fromCharCode(c);
                }
                else if ((c > 127) && (c < 2048)) {
                    utftext += String.fromCharCode((c >> 6) | 192);
                    utftext += String.fromCharCode((c & 63) | 128);
                }
                else {
                    utftext += String.fromCharCode((c >> 12) | 224);
                    utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                    utftext += String.fromCharCode((c & 63) | 128);
                }

            }

            return utftext;
        }
    }


    function lanciaVisPdf() {
        /*
        var w = window.screen.availWidth;
        var h = window.screen.availHeight;
        var dimensionWindow = "width=" + w + ",height=" + h;
        window.showModalDialog('Import/visPdfReportFrame.aspx', '', 'dialogWidth:' + w + ';dialogHeight:' + h + ';status:no;resizable:yes;scroll:no;center:no;help:no;close:no;top:' + 0 + ';left:' + 0);
        */
        if (document.getElementById("txtFolderPath").value != '')
            parent.ajaxModalPopupReportFrame();
    }
</script>
</asp:Content>
<asp:Content ID="contentExport" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server" >
    <asp:UpdatePanel ID="pnlApplet" runat="server">
        <ContentTemplate>
             <asp:UpdatePanel ID="udpFileSystem" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div><br />
                        <asp:Label ID="lblFolderPath" runat="server"></asp:Label><br />
                        <asp:TextBox ID="txtFolderPath" runat="server" Width="400px" ClientIDMode="Static"></asp:TextBox>
                        <asp:Button ID="btnBrowseForFolder" runat="server" Text="..." OnClientClick="SelectFolder();" ClientIDMode="Static"></asp:Button>
                    </div>
                    <asp:HiddenField ID="hdResult" runat="server" ClientIDMode="Static" />
                    <uc4:ExportProjectApplet ID="exportProjectApplet" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="cpnlOldersButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpUpdateButtons">
        <ContentTemplate>
            <cc1:CustomButton ID="CheckInOutConfirmButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static"  OnClientClick="if (!attendi()){ confirmAction(function(){ $('#hdnExportDocumentConfirmButton').click(); }); return false;}"/>
				<asp:Button ID="hdnExportDocumentConfirmButton" runat="server" CssClass="hidden"  ClientIDMode="Static" OnClick="ExportDocumentConfirmButton_Click"/>
			
			<cc1:CustomButton ID="CheckInOutCloseButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClientClick="parent.closeAjaxModal('ExportDocumentSocket','');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>