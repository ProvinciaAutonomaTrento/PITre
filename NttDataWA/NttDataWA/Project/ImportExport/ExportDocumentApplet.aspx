<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportDocumentApplet.aspx.cs" Inherits="NttDataWA.Project.ImportExport.ExportDocumentApplet" MasterPageFile="~/MasterPages/Popup.Master" %>
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
        if (wndAttendi)
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

    function confirmAction() {
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
    }

    function exportFascicolo(finalPath, urlPost, idProject, urlDoc) {
        var content = null;
        var status = null;
        var procede = true;
        
        if (idProject != null && idProject != "") 
        {
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
            }
        }  

        if (procede) {
            if (fsoApp == undefined) {
                fsoApp = window.document.plugins[0];
            }
            if (fsoApp == undefined) {
                fsoApp = document.applets[0];
            }
            var resultString = fsoApp.projectToFS(finalPath,content,urlDoc);
        }

        document.getElementById('hdResult').value = resultString;

        return true;
    }

    function FisicalFolderPath() {
        var folderPath = fixPath(document.getElementById("txtFolderPath").value);

            if (folderPath != null && folderPath != '') {
                if (fsoApp.folderExists(folderPath)) {
                    return folderPath;
                }
                else {
                    if (confirm('<%=this.GetMessage("CREATE_PATH")%>')) {
                        if (fsoApp.createFolder(folderPath)) {
                            return folderPath;
                        }
                        else {
                            ajaxDialogModal('Path_AccessDenied', 'error', '');
                            folderPath = '';
                        }
                    }
                }
            }
            else {
                ajaxDialogModal('Path_Nonexistent', 'warning', '');
                folderPath = '';
            }

        return folderPath;
    }

    function setTempFolder() {
        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        folderPath = lastPath;

        if (folderPath == null || folderPath == '') {
            try {
                disallowOp('contentExport');
                var folderPath = fsoApp.GetSpecialFolder();
                if (folderPath != null && folderPath != '') {
                    document.getElementById("txtFolderPath").value = shortfolderPath(folderPath);
                }
                reallowOp();
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

        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        var actualFolder = document.getElementById("txtFolderPath").value;
        var folder = fsoApp.selectFolder('<%=this.GetMessage("SELECT_PATH")%>', actualFolder);

        if (folder != "") {
            if (fsoApp.folderExists(folder)) {
                document.getElementById("txtFolderPath").value = folder;
                setLastFolderInSession(folder);
            }
            else {
                if (confirm('<%=this.GetMessage("CREATE_PATH")%>')) {
                    if (fsoApp.createFolder(folder)) {
                        document.getElementById("txtFolderPath").value = folder;
                        setLastFolderInSession(folder);
                    }
                    else {
                        ajaxDialogModal('Path_AccessDenied', 'error', '');
                        document.getElementById("txtFolderPath").value = '';
                    }
                }
            }
        }
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
        if (document.getElementById("txtFolderPath").value!='')
            parent.ajaxModalPopupReportFrame();
    }
</script>
</asp:Content>
<asp:Content ID="contentExport" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server" >
    <applet id='fsoApplet' 
        code = 'com.nttdata.fsoApplet.gui.FsoApplet' 
        codebase=  '<%=Page.ResolveClientUrl("~/Libraries/")%>'
        archive='FsoApplet.jar'
		width = '10'   height = '9'>
        <param name="java_arguments" value="-Xms64m" />
        <param name="java_arguments" value="-Xmx128m" />
    </applet>
    <asp:UpdatePanel ID="pnlApplet" runat="server">
        <ContentTemplate>
             <asp:UpdatePanel ID="udpFileSystem" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <div><br />
                        <asp:Label ID="lblFolderPath" runat="server"></asp:Label><br />
                        <asp:TextBox ID="txtFolderPath" runat="server" Width="400px" ClientIDMode="Static"></asp:TextBox>
                        <asp:Button ID="btnBrowseForFolder" runat="server" Text="..." OnClientClick="SelectFolder();"></asp:Button>
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
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ExportDocumentConfirmButton_Click" OnClientClick="if (!attendi()) return confirmAction();"/>
            <cc1:CustomButton ID="CheckInOutCloseButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClientClick="parent.closeAjaxModal('ExportDocumentApplet','');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>