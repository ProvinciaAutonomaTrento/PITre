<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckInDocument.aspx.cs" Inherits="NttDataWA.CheckInOutApplet.CheckInDocument" MasterPageFile="~/MasterPages/Popup.Master" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    .container {text-align: left; line-height: 2em; position: absolute; left: 40%; top: 44%; margin: -90px 0 0 -135px}
    .container img {float: left; display: block; margin: 15px auto; margin: 0 10px 70px 0;}
</style>
<script type="text/javascript">
    var fsoApp;
    var MAX_RELEASE_LENTGTH = parseInt('<%=MAX_RELEASE_LENTGTH%>');


    function initKeyPressRelease(e) {
        var myField = document.getElementById('txtComments');
        if (e.which < 0x20) {
            // e.which < 0x20, then it's not a printable character
            // e.which === 0 - Not a character
            return;     // Do nothing
        }
        if (myField.value.length == MAX_RELEASE_LENTGTH) {
            e.preventDefault();
        } else if (myField.value.length > MAX_RELEASE_LENTGTH) {
            // Maximum exceeded
            myField.value = myField.value.substring(0, MAX_RELEASE_LENTGTH);
        }
    }

    function confirmAction() {
        var retValue = false;

        if (fsoApp == undefined) {
            fsoApp = window.document.plugins[0];
        }
        if (fsoApp == undefined) {
            fsoApp = document.applets[0];
        }

        var checkOutFilePath = '<%=CheckOutFilePath%>';
        var selecletd = document.getElementById("hdnOptSelected").value;
        if (checkOutFilePath != null && checkOutFilePath != '') {
            var urlPost = '<%=httpFullPath%>' + '/CheckInOutApplet/CheckInPage.aspx';
            try {
                if (fsoApp.sendFiletoURL(checkOutFilePath, urlPost)) {
                    if (selecletd == "DF") {
                        if (fsoApp.fileExists(checkOutFilePath)) {
                            try {
                                // Rimozione file locale, solo se UndoCheckOut andato a buon fine
                                fsoApp.deleteFile(checkOutFilePath, true);
                            }
                            catch (ex) { // Il tentativo di cancellazione del file non è andato a buon fine 
                            }
                        }
                    }
                    retValue = true;
                    reallowOp();
                }
                else {
                    alert('Applet error to get file.');
                    retval = false;
                }
            }
            catch (ex) {
                alert("Errore nel checkIn del documento:\n" + ex.message.toString());
                reallowOp();
                retValue = false;
            }
        }

        return retValue;
    }

    CharacterCount = function (TextArea, FieldToCount, MaxChars) {
        var myField = document.getElementById(TextArea);
        var myLabel = document.getElementById(FieldToCount);
        if (!myField || !myLabel) { return false }; // catches errors
        if (!MaxChars) { return false };
        var remainingChars = MaxChars - myField.value.length
        myLabel.innerHTML = "Caratteri disponibili "+remainingChars;
    }
    //SETUP!!
    setInterval(function () { CharacterCount('txtComments', 'CharCountLabel', MAX_RELEASE_LENTGTH) }, 55);

	function confirmActionSocket(callback) {
	    var retValue = false;
        var checkOutFilePath = '<%=CheckOutFilePath%>';
	    var selecletd = document.getElementById("hdnOptSelected").value;

        if (checkOutFilePath!=null && checkOutFilePath !='') {
            var urlPost = '../CheckInOutApplet/CheckInPage.aspx?type=socket';
            try {

                getFileFromPath(checkOutFilePath, urlPost, function (getFile, connection) {
                    //alert(getFile);
                    $.ajax({
                        type: 'POST',
                        url: urlPost,
                        data: { "strFile": getFile },
                        success: function () {
                            if (selecletd != "DF"){
                                callback(true);
                            }else{
                                fileExists(checkOutFilePath, function (ret, connection) {
                                    if (ret == 'true') {
                                        try {
                                            // Rimozione file locale, solo se UndoCheckOut andato a buon fine
                                            deleteFile(checkOutFilePath, true,
                                                function (ret, connection) {
                                                    connection.close();
                                                    callback(true);
                                            });                
                                        }
                                        catch (ex) { // Il tentativo di cancellazione del file non è andato a buon fine 
                                        }
                                    }
                                });
                            }
                        },
                        error: function () {
                            alert('Applet error to get file.');
                            callback(false);
                        },
                        async: true
                    });
                    connection.close();
                });
            }
            catch (ex) {
                alert("Errore nel checkIn del documento:\n" + ex.message.toString());
                
                callback(false);
            }
        }
        return retValue;
	}

</script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server" >
    <asp:UpdatePanel ID="pnlAppletTag" runat="server" Visible="true" ClientIDMode="Static" UpdateMode="Conditional">
        <ContentTemplate>
            <applet 
                id='fsoApplet' 
                code = 'com.nttdata.fsoApplet.gui.FsoApplet' 
                codebase=  '<%=Page.ResolveClientUrl("~/Libraries/")%>'
                archive='FsoApplet.jar'
		        width = '10'   height = '9'>
                <param name="java_arguments" value="-Xms128m" />
                <param name="java_arguments" value="-Xmx512m" />
            </applet>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="pnlOpt" runat="server">
        <ContentTemplate>
            <div class="container">
                <asp:RadioButtonList ID="rblListCheckInOption" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblListCheckInOption_SelectedIndexChanged"
                    RepeatDirection="Vertical">
                    <asp:ListItem ID="optDeleteFile" Value="DF" Selected="True" runat="server"></asp:ListItem>
                    <asp:ListItem ID="optPreserveFile" Value="PF" runat="server"></asp:ListItem>
                </asp:RadioButtonList>
                <asp:HiddenField ID="hdnOptSelected" runat="server" ClientIDMode="Static" Value="DF"/>
                <table>
                    <tr>
                        <td>
                            <asp:label id="lblComments" runat="server" CssClass="weight">NOTE</asp:label>
                        </td>
                    </tr>
                    <tr>
                        <td>
			                <asp:TextBox id="txtComments" runat="server" Width="350px" TextMode="MultiLine" Rows="5" ClientIDMode="Static" MaxLength="200" onkeypress="return initKeyPressRelease(event)"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
			                <span id='CharCountLabel' class="charactersAvailable" style="text-align:right;"></span>
                        </td>
                    </tr>
                </table>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="cpnlOldersButtons" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpUpdateButtons">
        <ContentTemplate>
            <cc1:CustomButton ID="CheckInOutConfirmButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Procedi" ClientIDMode="Static" OnClick="CheckInOutConfirmButton_Click" />
            <cc1:CustomButton ID="CheckInOutCloseButton" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Chiudi" OnClick="CheckInOutCloseButton_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>