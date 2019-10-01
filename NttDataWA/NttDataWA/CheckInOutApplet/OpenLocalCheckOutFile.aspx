<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OpenLocalCheckOutFile.aspx.cs" Inherits="NttDataWA.CheckInOutApplet.OpenLocalCheckOutFile" MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    .container {text-align: left; line-height: 2em; width: 400px; height: 130px; position: absolute; left: 40%; top: 70%; margin: -90px 0 0 -135px;}
    .container img {float: left; display: block; margin: 15px auto; margin: 0 10px 70px 0;}
</style>
<script type="text/javascript">
    var fsoApp;
    var waitingMessage = '<%=waitingMessage%>';
    var errorMessage = '<%=errorMessage%>';

    function confirmAction() {
        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        var checkOutFilePath = '<%=checkOutFilePath%>';

        if (checkOutFilePath != null && checkOutFilePath != '') {
            $('#lblWaitingMessage').html(waitingMessage);
            $('#lblFilePath').html(checkOutFilePath);
            if (fsoApp.fileExists(checkOutFilePath)) {
                try {
                    return fsoApp.openFile(checkOutFilePath);
                }
                catch (ex) {
                    alert(ex.Message.toString());
                    return true;
                }
            }
            else {
                $('#lblWaitingMessage').html(errorMessage);
                $('#CheckInOutCloseButton').show();
                return false;
            }
        }

        return true;
    }

    function confirmActionSocket(callback) {

        var checkOutFilePath = '<%=checkOutFilePath%>';

        if (checkOutFilePath != null && checkOutFilePath != '') {
            $('#lblWaitingMessage').html(waitingMessage);
            $('#lblFilePath').html(checkOutFilePath);
            fileExists(checkOutFilePath,
                function (exist) { 
                    if (exist === 'true') {
                        try {
                            openFile(checkOutFilePath,
                                        function (msg, connection) {
                                            connection.close();
                                            callback();
                                        });
                        }
                        catch (ex) {
                            alert(ex.Message.toString());
                        }
                    }
                    else {
                        $('#lblWaitingMessage').html(errorMessage);
                        $('#CheckInOutCloseButton').show();
                    }
                });
        }
    }
</script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server" >
    <asp:UpdatePanel ID="pnlAppletTag" runat="server" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <applet id='fsoApplet' 
                    code = 'com.nttdata.fsoApplet.gui.FsoApplet' 
                    codebase=  '<%=Page.ResolveClientUrl("~/Libraries/")%>'
                    archive='FsoApplet.jar'
		            width = '10'   
                    height = '9'>
            <param name="java_arguments" value="-Xms128m" />
            <param name="java_arguments" value="-Xmx512m" />
            </applet>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="pnlApplet" runat="server">
        <ContentTemplate>
            <div class="container">
                <div id="lblWaitingMessage"></div>
                <div id="lblFilePath"></div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="cpnlOldersButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpUpdateButtons" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="CheckInOutCloseButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" OnClick="CheckInOutCloseButton_Click" ClientIDMode="Static" style="display: none" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>