<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportDocumentApplet.aspx.cs" Inherits="NttDataWA.Project.ImportExport.ImportDocumentApplet"  MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register src="ImportProjectApplet.ascx" tagname="ImportProjectApplet" tagprefix="uc4" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<script type="text/javascript">
    var fsoApp;
    var folderPath;
    var lastPath = "<%=this.getLastPath()%>";

    function ShowWaitingPage(msg) {
        wndAttendi = window.open('tempPageImport.aspx', 'Operazione in corso...', "location=0,toolbar=0,scrollbars=0,resizable=0,closeable=0,width=350,height=10,left=450,top=450");
    }

    function CloseWaitingPage() {
        if (wndAttendi)
            wndAttendi.close();
    }

    function attendi() {
        disallowOp('ContentImport');

        if($.browser.chrome)
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
        retVal = false;
        var startPath = FisicalFolderPath();
        var idProject = '<%=getProjectId%>';
        var folderCod = '<%=getProjectCode%>';
        var idTitolario = '<%=getTitolarioId%>';

        if (startPath != lastPath)
            setLastFolderInSession(startPath);

        if (startPath != null && startPath != '') {
            folderPath = checkFolder(FisicalFolderPath());
            if (importFascicolo(startPath, idProject, folderCod, idTitolario)) {
                reallowOp();
                retVal = true;
            }
            else {
                reallowOp();
                ajaxDialogModal('Path_AccessDenied', 'error', '');
            }
        }

        CloseWaitingPage();
        return retVal;
    }

    function importFascicolo(startPath, idProject, folderCod, idTitolario) {
        loadFolderAndFile(startPath, folderCod, idProject, idTitolario);
        
        return true;
    }

    function loadFolderAndFile(completePathFolder, SelectedFolder, idProject, idTitolario) {
        var fold;

        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        if (fsoApp.folderExists(completePathFolder)) {
            fold = completePathFolder;
        }
        else {
            return;
        }

        var FileArr = new Array();
        var FolderArr = new Array();

        var retValueFiles = fsoApp.getFiles(fold);
        var retValueFolders = fsoApp.getFolders(fold);

        
        if (retValueFiles!=null)
            FileArr = retValueFiles.split("|");

        if (retValueFolders != null)
            FolderArr = retValueFolders.split("|");

        try {
            //ciclo di scansione Files della root Folder
            for (var i = 0; i < FileArr.length; i++) {
                if (FileArr[i] != "") {
                    var PathAndFileName = (fixPath(completePathFolder) + FileArr[i]);
                    pushContentAndMetadata(PathAndFileName, idProject, SelectedFolder, folderPath, "FILE", idTitolario);
                }
            }
                        //ciclo di scansione di sotto directory
            for (var j = 0; j < FolderArr.length; j++) {
                if (FolderArr[j] != "") {
                    var completePathFolderName = (fixPath(completePathFolder) + FolderArr[j]);
                    SelectedFolder = pushContentAndMetadata(completePathFolderName, idProject, SelectedFolder, folderPath, "DIR", idTitolario);
                    loadFolderAndFile(completePathFolderName, SelectedFolder, idProject, idTitolario);
                }
            }            
        }
        catch (e) {
            alert(e.message.toString());
            return;
        }
    }

    function pushContentAndMetadata(pathObject, idProject, SelectedFolder, name, type, idTitolario) {
        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        var new_SelectedFolder = SelectedFolder;
        var arrayReturnValue;
        try {
            var content = null;
            var status = null;
            var completeUrl = "<%=httpFullPath%>" + "/Project/ImportExport/Import/ImportDocumentAppletService.aspx?Absolutepath=" + Url.encode(pathObject) + "&codFasc=" + Url.encode(SelectedFolder) + "&foldName=" + Url.encode(name) + "&type=" + type + "&idTitolario=" + Url.encode(idTitolario);
            if (fsoApp.sendFiletoURL(pathObject, completeUrl)) {
                $.ajax({
                    type: 'POST',
                    cache: false,
                    //dataType: "text",
                    processData: false,
                    url: "<%=httpFullPath%>" + "/Project/ImportExport/UpdateImportStatus.aspx",
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

                if (status == 200)
                    arrayReturnValue = content.split("|");
            }
            else {
                //inserisco il messaggio di errore nel campo hidden
            }
        }
        catch (ex) {
            alert(ex.message.toString());
            //inserisco il messaggio di errore nel campo hidden
        }

        var resultString = "";

        if (arrayReturnValue!=undefined && arrayReturnValue.length > 0) {
            if (arrayReturnValue[0].toString() != "")
                new_SelectedFolder = arrayReturnValue[0].toString();

            resultString = stringForReport(arrayReturnValue[1].toString(), type, pathObject);
         }

        document.getElementById('hdResult').value += resultString;
        return new_SelectedFolder;
    }

    if (!String.prototype.repeat) {
        String.prototype.repeat = function (count) {
            'use strict';
            if (this == null) {
                throw new TypeError('can\'t convert ' + this + ' to object');
            }
            var str = '' + this;
            count = +count;
            if (count != count) {
                count = 0;
            }
            if (count < 0) {
                throw new RangeError('repeat count must be non-negative');
            }
            if (count == Infinity) {
                throw new RangeError('repeat count must be less than infinity');
            }
            count = Math.floor(count);
            if (str.length == 0 || count == 0) {
                return '';
            }
            // Ensuring count is a 31-bit integer allows us to heavily optimize the
            // main part. But anyway, most current (August 2014) browsers can't handle
            // strings 1 << 28 chars or longer, so:
            if (str.length * count >= 1 << 28) {
                throw new RangeError('repeat count must not overflow maximum string size');
            }
            var maxCount = str.length * count;
            count = Math.floor(Math.log(count) / Math.log(2));
            while (count) {
                str += str;
                count--;
            }
            str += str.substring(0, maxCount - str.length);
            return str;
        }
    }


    // https://github.com/uxitten/polyfill/blob/master/string.polyfill.js
    // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/padStart
    if (!String.prototype.padStart) {
        String.prototype.padStart = function padStart(targetLength, padString) {
            targetLength = targetLength >> 0; //truncate if number or convert non-number to 0;
            padString = String((typeof padString !== 'undefined' ? padString : ' '));
            if (this.length > targetLength) {
                return String(this);
            }
            else {
                targetLength = targetLength - this.length;
                if (targetLength > padString.length) {
                    padString += padString.repeat(targetLength / padString.length); //append to original to ensure we are longer than needed
                }
                return padString.slice(0, targetLength) + String(this);
            }
        };
    }

    function stringForReport(stato, tipo, percorso) {
        var today = new Date()
        var data = today.getDate().toString().padStart(2, '0') + "/" + (today.getMonth() + 1).toString().padStart(2, '0') + "/" + today.getFullYear().toString() + "  " + today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();
        var resultStr = data + "@-@" + tipo + "@-@" + percorso + "@-@" + stato + "\r\n";
        return resultStr;
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
                disallowOp('ContentImport');
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

    function checkFolder(tempPath) {
        strResult = '';
        totCh = tempPath.length;
        if (totCh > 0) {
            ch = tempPath.substring(totCh - 1, totCh);
            if (ch == '\\')
                strResult = tempPath.substring(0, totCh - 1);
            else
                strResult = tempPath;
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
<asp:Content ID="ContentImport" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server" >
    <applet id='fsoApplet' 
        code = 'com.nttdata.fsoApplet.gui.FsoApplet' 
        codebase=  '<%=Page.ResolveClientUrl("~/Libraries/")%>'
        archive='FsoApplet.jar'
		width = '10'   height = '9'>
        <param name="java_arguments" value="-Xms64m" />
        <param name="java_arguments" value="-Xmx512m" />
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
                    <uc4:ImportProjectApplet ID="ImportProjectApplet" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="cpnlOldersButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpUpdateButtons">
        <ContentTemplate>
            <cc1:CustomButton ID="CheckInOutConfirmButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="ImportDocumentConfirmButton_Click" OnClientClick="if (!attendi()) return confirmAction();"/>
            <cc1:CustomButton ID="CheckInOutCloseButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" OnClientClick="parent.closeAjaxModal('ImportDocumentApplet','');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>