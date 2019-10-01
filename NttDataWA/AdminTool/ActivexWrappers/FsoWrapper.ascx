<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FsoWrapper.ascx.cs" Inherits="SAAdminTool.ActivexWrappers.FsoWrapper" %>
<%if (!this.Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "fsoWrapperControlScript"))
  { %>
<script id="fsoWrapperControlScript" type="text/javascript">
function FsoWrapper_CreateFsoObject()
{
    var progId;
    try
    {
        if ("<%=UseActivexWrapper%>" == "True")
            progId="DocsPa_ActivexWrappers.FsoWrapper";
        else
            progId="Scripting.FileSystemObject";
            
        return new ActiveXObject(progId);
    }
    catch(e)
    {
        alert("Errore nella creazione dell\'oggetto '" + progId + "'");
    }
}

function FsoWrapper_CloseFsoStreamObject(fileStream)
{
    if ("<%=UseActivexWrapper%>" == "True")
        fileStream.CloseStream();
    else
        fileStream.Close();
}

function FsoWrapper_GetFolders(folder) {
    try {
        var folders = null;

        if ("<%=UseActivexWrapper%>" == "True") {
            var fsoWrapper = new ActiveXObject("DocsPa_ActivexWrappers.FsoWrapper");
            var fsoFolderWrapper = fsoWrapper.GetFolder(folder.Path);
            folders = fsoFolderWrapper.GetFolders();
        }
        else {
            var fso = new ActiveXObject("Scripting.FileSystemObject");
            var fld = fso.GetFolder(folder.Path);
            
            var f = new Enumerator(fld.SubFolders);

            folders = new Array();
            var i = 0;
            for (; !f.atEnd(); f.moveNext()) {
                folders[i] = f.item().Path;
                i++;
            }
        }

        return folders;
    }
    catch (e) {
        alert("Errore nel reperimento dei subfolders nel folder '" + folder.Path + "'");
    }
}

function FsoWrapper_GetFiles(folder) {
    try {
        var files = null;

        if ("<%=UseActivexWrapper%>" == "True") {
            var fsoWrapper = new ActiveXObject("DocsPa_ActivexWrappers.FsoWrapper");
            var fsoFolderWrapper = fsoWrapper.GetFolder(folder.Path);

            var f = new Enumerator(fsoFolderWrapper.GetFiles());

            files = new Array();
            var i = 0;
            for (; !f.atEnd(); f.moveNext()) {
                files[i] = f.item();
                i++;
            }
        }
        else {
            var fsoWrapper = new ActiveXObject("Scripting.FileSystemObject");
            var fsoFolderWrapper = fsoWrapper.GetFolder(folder.Path);

            var f = new Enumerator(fsoFolderWrapper.Files);

            files = new Array();
            var i = 0;
            for (; !f.atEnd(); f.moveNext()) {
                files[i] = f.item().Path;
                i++;
            }
        }

        return files;
    }
    catch (e) {
        alert("Errore nel reperimento dei file nel folder '" + folder.Path + "'");
    }
}
    
</script>
<% 
      this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "fsoWrapperControlScript", string.Empty); 
  } %>