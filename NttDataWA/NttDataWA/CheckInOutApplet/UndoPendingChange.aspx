<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UndoPendingChange.aspx.cs" Inherits="NttDataWA.CheckInOutApplet.UndoPendingChange" MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    .container {text-align: left; line-height: 2em; width: 400px; height: 130px; position: absolute; left: 40%; top: 70%; margin: -90px 0 0 -135px;}
    .container img {float: left; display: block; margin: 15px auto; margin: 0 10px 70px 0;}
</style>
<script type="text/javascript">
    var fsoApp;

    function confirmAction() {
        var retValue = false;

        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        var checkOutFilePath = '<%=CheckOutFilePath%>';

        if (checkOutFilePath != null && checkOutFilePath != '') {
            disallowOp('Content1');
            try {
                var status = 0;
                var content = '';
                $.ajax({
                    type: 'POST',
                    cache: false,
                    //dataType: "text",
                    processData: false,
                    url: "UndoCheckOutPage.aspx",
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


                var response = content;

                retValue = (response == "");

                if (!retValue) {
                    // Visualizzazione messaggio di errore nell'undocheckout
                    alert(response);
                    retValue = false;
                }
                else {
                    retValue = true;
                    if (fsoApp.fileExists(checkOutFilePath)) {
                        try {
                            // Rimozione file locale, solo se UndoCheckOut andato a buon fine
                            fsoApp.deleteFile(checkOutFilePath, true);
                        }
                        catch (ex) { // Il tentativo di cancellazione del file non è andato a buon fine 
                        }
                    }
                }
                reallowOp();
            }
            catch (ex) {
                alert("Errore nell'annullamento del blocco del documento:\n" + ex.message.toString());
                reallowOp();
                retValue = false;
            }
        }

        return retValue;
    }


    function confirmActionSocket(callback) {

        var checkOutFilePath = '<%=CheckOutFilePath%>';

        if (checkOutFilePath != null && checkOutFilePath != '') {
            disallowOp('Content1');
            try {
                var status = 0;
                var content = '';
                $.ajax({
                    type: 'POST',
                    cache: false,
                    //dataType: "text",
                    processData: false,
                    url: "UndoCheckOutPage.aspx",
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


                var response = content;

                retValue = (response == "");

                if (!retValue) {
                    // Visualizzazione messaggio di errore nell'undocheckout
                    alert(response);
                    callback();
                    reallowOp();
                }
                else {
                    retValue = true;
                    fileExists(checkOutFilePath, function (retVal, connection) {

                        if (retVal === 'true') {
                            try {
                                // Rimozione file locale, solo se UndoCheckOut andato a buon fine
                                deleteFile(checkOutFilePath, true, function (ret, connection) {
                                    connection.close();
                                    callback();
                                    reallowOp();
                                });
                            }
                            catch (ex) { // Il tentativo di cancellazione del file non è andato a buon fine 
                            }
                        }

                    });
                    
                }
            }
            catch (ex) {
                alert("Errore nell'annullamento del blocco del documento:\n" + ex.message.toString());
                reallowOp();
                callback();
            }
        }
    }

</script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server" >
    <asp:UpdatePanel ID="pnlAppletTag" runat="server" Visible="false" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <applet id='fsoApplet' 
                code = 'com.nttdata.fsoApplet.gui.FsoApplet' 
                codebase=  '<%=Page.ResolveClientUrl("~/Libraries/")%>'
                archive='FsoApplet.jar'
	        	width = '10'   height = '9'>
                <param name="java_arguments" value="-Xms128m" />
                <param name="java_arguments" value="-Xmx512m" />
            </applet>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="pnlApplet" runat="server">
        <ContentTemplate>
            <div class="container">
                <asp:Literal ID="litConditionalMessage" runat="server"></asp:Literal>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="cpnlOldersButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpUpdateButtons">
        <ContentTemplate>
            <cc1:CustomButton ID="CheckInOutConfirmButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Procedi" ClientIDMode="Static" OnClick="CheckInOutConfirmButton_Click"/>
            <cc1:CustomButton ID="CheckInOutCloseButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" OnClick="CheckInOutCloseButton_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
