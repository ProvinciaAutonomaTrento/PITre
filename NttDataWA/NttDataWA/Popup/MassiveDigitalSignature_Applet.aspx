<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true" CodeBehind="MassiveDigitalSignature_Applet.aspx.cs" Inherits="NttDataWA.Popup.MassiveDigitalSignature_Applet" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <script src="../Scripts/json2.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
    </style>
    <script language="javascript" type="text/javascript">
        var applet = undefined;
        var commandType = '<%=CommandType%>';
        var convpdfa=null;

        function signSmartClient() {
            disallowOp('Content1');
            //if (window.frames['uploadFrame'].ApplySign()) {
            window.frames['uploadFrame'].ViewResult();
            //}
            reallowOp();
            return false;
        }

        function signWithApplet() {
            disallowOp('Content1');
            var docs = "<%=this.GetSelectedDocumentsJSON()%>";
            if (docs != null && docs != "") {
                return this.SignDocuments(docs.split("|"));
            }
            else {
                alert('Nessun documento selezionato per la firma');
                return false;
            }
        }

        function openApplet() {
            if (commandType != 'close') {
                try {
                    disallowOp('Content1');
                    if (applet == undefined) {
                        applet = window.document.plugins[0];
                    }
                    if (applet == undefined) {
                        applet = document.applets[0];
                    }

                    //var storeLocation = "";
                    //var storeLocation = "1";
                    //var storeLocation = "CurrentUser";
                    var storeLocation = "Windows-MY";

                    var storeName = "";
                    //var storeName = "Windows-MY";
                    //var storeName = "My";
                    //var storeName = "My";

                    var list = new Array();

                    if (applet != undefined) {
                        var retValue = applet.getCertificateListAsJsonFormat(storeLocation, storeName);
                        //var retValue = applet.getJsonCertList();
                        var jsonList = eval(retValue);
                        if (jsonList != null) {
                            for (var i = 0; i < jsonList.length; i++) {
                                list[i] = jsonList[i];
                            }
                        }

                    }
                    else {
                        alert("Impossibile caricare Applet.");
                    }

                    FetchListaCertificati(list);
                }
                catch (err) {
                    alert("Error:" + err.description);
                    reallowOp();
                }

                reallowOp();
                return false;
            }
            else {
                return true;
            }
        }

        //function FetchListaCertificati(list) {
        //    if (list.length > 0) {

        //        $.each(list, function () {
        //            var cert = this;

        //            //var option = document.createElement("OPTION");
        //            //var selectObj = document.getElementById('lstListaCertificati');

        //            props = cert.SubjectName.split(",");
        //            //selectObj.options.add(option);
        //            var optionText;
        //            var optionValue;

        //            for (j = 0; j < props.length; j++) {
        //                if (props[j].substr(0, 1) == " ")
        //                    props[j] = props[j].substr(1);

        //                if (props[j].substr(0, 3) == "CN=") {
        //                    //option.innerText = props[j].substr(3);
        //                    optionText = props[j].substr(3);
        //                }
        //            }
        //            //option.Value = cert.SerialNumber;
        //            optionValue = cert.SerialNumber;

        //            $('#lstListaCertificati').append('<option value="' + optionValue + '">' + optionText + '</option>');
        //        });


        //        //            var e = new Enumerator(list);
        //        //            var i = 1;
        //        //            for (; !e.atEnd(); e.moveNext()) {
        //        //                var cert = e.item();


        //        //                //option.Value = i;
        //        //                i++;
        //        //            }
        //    }
        //    else {
        //        alert("Nessuna firma valida trovata.");
        //    }
        //}

        function FetchListaCertificati(list) {
            if (list.length > 0) {

                $.each(list, function () {
                    var cert = this;
                    //console.log('Certificato', cert);
                    props = cert.SubjectName.split(",");
                    var expired = '<%= lblExpired %>'
                    var optionText;
                    var optionValue;
                    var option;
                    for (j = 0; j < props.length; j++) {
                        if (props[j].substr(0, 1) == " ")
                            props[j] = props[j].substr(1);

                        if (props[j].substr(0, 3) == "CN=") {
                            optionText = props[j].substr(3);
                        }
                    }

                    optionValue = cert.SerialNumber;
                    //console.log('SerialNumber', cert.SerialNumber);




                    var onSuccess = function (response) {
                        try {
                            var revocationDate = '';
                            var revocate = false;
                            if (response.revocationStatus == -1) {
                                alert('Non è stato possibile controllare la firma digitale.');
                                if (!isMsie()) {
                                    option = new Option(optionText, optionValue);
                                    $('#lstListaCertificati').append(option);
                                    option = $("#lstListaCertificati option[value='" + optionValue + "']");
                                    option.css("color", "black");
                                } else {
                                    option = document.createElement("OPTION");
                                    $('#lstListaCertificati').append(option);
                                    option.value = optionValue;
                                    option.innerText = optionText;
                                    option.style.color = 'black';
                                }
                            } else {
                                if (response.revocationDate && (response.revocationStatus == 1 || response.revocationStatus == 4)) {
                                    revocationDate = response.revocationDate;
                                    revocate = true;
                                }
                                if (revocate) {
                                    optionText += ' - ' + expired + ': ' + revocationDate;

                                    if (!isMsie()) {
                                        option = new Option(optionText, optionValue);
                                        $('#lstListaCertificati').append(option);
                                        option = $("#lstListaCertificati option[value='" + optionValue + "']").attr("disabled", "disabled");
                                        option.css("color", "red");
                                    } else {
                                        option = document.createElement("OPTION");
                                        $('#lstListaCertificati').append(option);
                                        option.value = optionValue;
                                        option.innerText = optionText;
                                        option.style.color = 'red';
                                    }
                                } else {

                                    if (!isMsie()) {
                                        option = new Option(optionText, optionValue);
                                        $('#lstListaCertificati').append(option);
                                        option = $("#lstListaCertificati option[value='" + optionValue + "']");
                                        option.css("color", "black");
                                    } else {
                                        option = document.createElement("OPTION");
                                        $('#lstListaCertificati').append(option);
                                        option.value = optionValue;
                                        option.innerText = optionText;
                                        option.style.color = 'black';
                                    }
                                }
                            }
                        } catch (ex) {
                            alert('Non è stato possibile controllare la firma digitale.');
                            
                           
                            if (!isMsie()) {
                                option = new Option(optionText, optionValue);
                                $('#lstListaCertificati').append(option);
                                option = $("#lstListaCertificati option[value='" + optionValue + "']");
                                option.css("color", "black");
                            } else {
                                option = document.createElement("OPTION");
                                $('#lstListaCertificati').append(option);
                                option.value = optionValue;
                                option.innerText = optionText;
                                option.style.color = 'black';
                            }
                        }
                    };

                    var onError = function (response) {
                        try {
                            alert('Non è stato possibile controllare la firma digitale.');
                            if (!isMsie()) {
                                option = new Option(optionText, optionValue);
                                $('#lstListaCertificati').append(option);
                                option = $("#lstListaCertificati option[value='" + optionValue + "']");
                                option.css("color", "black");
                            } else {
                                option = document.createElement("OPTION");
                                $('#lstListaCertificati').append(option);
                                option.value = optionValue;
                                option.innerText = optionText;
                                option.style.color = 'black';
                            }
                        } catch (ex) {
                            //console.log('Errore controllo firma', ex);
                            alert('Errore nella verifica del certificato');
                        }
                    }


                    $.ajax({
                        type: "POST",
                        url: "../Utils/ValidateCertificateHandler.ashx",
                        data: JSON.stringify(cert),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: onSuccess,
                        error: onError
                    });
                });
            }
            else {
                alert("Nessuna firma valida trovata.");
            }
        }
        
        function SignDocuments(docs) {
            var retValue = true;
            var strPadesSign = "<%=this.FirmaPades%>";
            var padesSign = false;
            var documento;
            var idDocumento;
            var isSigned;
            var fileExtension;
            var tipoFirma = "cosign";;
            var optfirma = document.getElementById("optFirma");
            var signed;

            if (strPadesSign == 'True' || strPadesSign == 'true')
            {
                padesSign = true;
            }

            //voglio la firma pades cliccando radiobutton sulla maschera
            if (document.getElementById("optPades"))
            {
                 padesCheck= document.getElementById("optPades").checked;
                 if (padesCheck)
                    padesSign= true;
                  else
                    padesSign= false;
            }

            for (k = 0; k < docs.length; k++) 
            {
                //ABBATANGELI - Nuova gestione Sign/Cosign

                if (optfirma.checked == true) {
                    tipoFirma = "sign";
                }       

                documento = JSON.parse(docs[k]);
                idDocumento = documento.idDocumento;
                signed = documento.isSigned;
                fileExtension = documento.fileExtension;

                if (padesSign || (signed && (signed === "0"))) {
                    tipoFirma = 'sign';
                }

                //Se contiene P vuol dire che arriva da LF es è cades
                if (idDocumento.indexOf("P") > -1)
                {
                    idDocumento = idDocumento.replace("P", "");
                    this.SignHash(tipoFirma, idDocumento,true);
                } else if (idDocumento.indexOf("C") > -1)
                {
                    idDocumento = idDocumento.replace("C", "");
                    this.SignHash(tipoFirma, idDocumento,false);
                }
                else {
                    if(padesSign)
                    {
                        this.SignHash(tipoFirma, idDocumento,true);
                    } else 
                    {
                        this.SignDocument(tipoFirma, idDocumento);
                    }
                 }
            }
            return retValue;
        }
        
        /*
         function SignDocuments(docs) {
            var retValue = true;

            for (k = 0; k < docs.length; k++) {
                this.SignDocument('<%=this.TipoFirma%>', docs[k]);
            }

            return retValue;
        }*/


     
        function SignDocument(tipoFirma, idDocumento) {
            var docStatus = true;
            var docStatusDescription = "";

            var temp = document.getElementById("lstListaCertificati");
            var indexCert = temp.selectedIndex;

            var content = null;
            var status = null;

            if (isNaN(indexCert) || indexCert == -1) {
                docStatus = false;
                docStatusDescription = "Nessun certificato selezionato";
            }
            else {
                var selectedValue = temp.options[indexCert].value;
                if (idDocumento != null && idDocumento != "") {
                    $.ajax({
                        type: 'POST',
                        url: "../SmartClient/FirmaMultiplaChangeSessionContext.aspx?idDocumento=" + idDocumento +"&tipoFirma=" + tipoFirma,
                        success: function (data, textStatus, jqXHR) {
                            status = jqXHR.status;
                            //alert(jqXHR.responseXML);
                            //status = textStatus;
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            if(jqXHR!=null)
                                content = jqXHR.responseText;
                        },
                        async: false
                    });
                
                    if (status != 200) {
                        // Si è verificato un errore, reperimento del messaggio
                        docStatus = false;
                        docStatusDescription = content;
                    }
                }
                
                if (docStatus) {
					var signedAsPdf = false;

                    var convLoc = false; // ConvertLocally();
                    var convCentr = <%if (ConvertPdfOnSign) {%>true;<%} else {%>document.getElementById("chkConverti").checked;<%} %>

                    // Conversione del file da firmare in pdf
                    // Se il sistema è configurato per convertire il documento in pdf prima
                    // della firma...
                    if (convCentr || convLoc) {
                        // ...se l'utente vuole convertire il documento si procede con la conversione
                        // prefirma
                        var fileFormat = "<%=GetFileExtension()%>";

                        if (fileFormat.indexOf(".p7m") == -1 && fileFormat.indexOf(".pdf") == -1) {
                            // Se è richiesta conversione pdf prefirma...
                            if (document.getElementById("chkConverti").checked) {
                                // ...se è richiesta la conversione centrale...
                                if (convCentr) {
                                    // ...si procede con la  conversione sincrona
                                    $.ajax({
                                        type: 'POST',
                                        cache: false,
                                        processData: false,
                                        url: "../DigitalSignature/ConvPDFSincrona.aspx?applet=1",
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
                                else {
                                    // ...conversione locale eliminata
                                    //content = ConvertPdfStream(http.responseBody, fileFormat, false);
                                }

                                if (status != 200) {
                                    signedAsPdf = false;
                                    content = null;
                                }
                                else {
                                    signedAsPdf = (content != null);
                                }


                                if (!signedAsPdf) {
                                    docStatusDescription = "Non è stato possibile convertire il file in formato PDF.\n" +
					                                            "Il file verrà firmato nel suo formato originale.";
                                }
                            }
                        }
                    }
                    				
					if (content == null)
                        {
                        $.ajax({
                            type: 'POST',
                            cache: false,
                            processData: false,
                            url: "../SmartClient/SignedRecordViewer.aspx?idDocumento=" + idDocumento + "&type=applet",
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

					// Applicazione della firma digitale al documento
                    var signedValue = null;
                    if (status == 200 || status == 0) {
                        signedValue = applet.signData(content, selectedValue, (tipoFirma == "cosign"), "Windows-MY", "");
                    }
                    else
                        alert(status);

                    if (signedValue == null) {
                        docStatus = false;
                        docStatusDescription = "Errore nella firma digitale del documento.";
                    }
                    else {
                        var status = 100;
                        $.ajax({
                            type: 'POST',
                            url: "../SmartClient/SaveSignedFile.aspx" + "?idDocumento=" + idDocumento + "&tipofirma=" + tipoFirma + "&signedAsPdf=" + signedAsPdf,
                            data: signedValue,
                            success: function (data, textStatus, jqXHR) {
                                status = jqXHR.status;
                                content = jqXHR.responseText;
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                status = textStatus;
                                content = jqXHR.responseText;
                            },
                            async: false
                        });

                        if ((status != 0 && status != 200) || content=='null') {
                            docStatus = false;
                            docStatusDescription = "Errore durante l\'invio del documento firmato.\n" + status + "\n" + content;
                        }
                    }
                }
            }

            // Invio informazioni sullo stato della firma
            var status = 100;
            $.ajax({
                type: 'POST',
                url: "../SmartClient/FirmaDigitaleResultStatusPage.aspx?status=" + docStatus + "&statusDescription=" + docStatusDescription + "&idDocument=" + idDocumento,
                //data: signedValue,
                success: function (data, textStatus, jqXHR) {
                    status = jqXHR.status;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    status = textStatus;
                    content = null;
                },
                async: false
            });

            return (docStatus == 0 || docStatus == 1);
        }

        function SignHash(tipoFirma, idDocumento, pades) {
            var docStatus = true;
            var fileFormat = "<%=GetFileExtension()%>";
            var temp = document.getElementById("lstListaCertificati");
            var indexCert = temp.selectedIndex;
            var docStatusDescription = "";

            if (isNaN(indexCert) || indexCert == -1) 
            {
                docStatus = false;
                docStatusDescription = "Nessun certificato selezionato";
            }
            else {
                var selectedValue = temp.options[indexCert].value;
                if (idDocumento != null && idDocumento != "") {
                    $.ajax({
                        type: 'POST',
                        url: '<%=httpFullPath%>' +'/SmartClient/FirmaMultiplaChangeSessionContext.aspx?fromDoc=1&idDocumento=' + idDocumento + '&tipoFirma=' + tipoFirma + '&pades='+pades,
                        success: function (data, textStatus, jqXHR) {
                            status = jqXHR.status;
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            if(jqXHR!=null)
                                content = jqXHR.responseText;
                        },
                        async: false
                    });

                    if (status != 200) {
                        docStatus = false;
                        docStatusDescription = content;
                    }
                    
                if (docStatus) {
                        try {
                                var status = 0;
                                var content = '';
                                $.ajax({
                                    type: 'POST',
                                    cache: false,
                                    processData: false,
                                    url: '<%=httpFullPath%>' + '/SmartClient/SignedRecordViewer.aspx?isHash=true&idDocumento=' + idDocumento,
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
                            catch (e) {
                                alert(e.message.toString());
                                retValue = false;
                            }

                            var signedValue = null;
                            if (content != null && content != '') //if (status == 200 || status == 0) {
                            {
                                signedValue = applet.signHash(content, selectedValue, (tipoFirma == "cosign"), "Windows-MY", "");
                            }
                            else {
                                alert("Impossibile contattare il ws...");
                            }


                        if (signedValue == null || signedValue == "KO") {
                            docStatus = false;
                            docStatusDescription = "Errore nella firma digitale del documento.";
                        }
                        else {
                            var status = 100;
                            $.ajax({
                                type: 'POST',
                                url: "<%=httpFullPath%>" + "/SmartClient/SaveSignedHashFile.aspx?idDocumento=" + idDocumento + "&isPades=" + pades,
                                data: { 'signedDoc': signedValue },
                                success: function (data, textStatus, jqXHR) {
                                    status = jqXHR.status;
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    content = textStatus;
                                },
                                async: false
                            });

                            if (status != 200) {
                                docStatus = false;
                                docStatusDescription = "Errore durante l\'invio del documento firmato.\n";
                            }
                        }
                    }
                }
            }

            // Invio informazioni sullo stato della firma
            var status = 100;
            $.ajax({
                type: 'POST',
                url: "../SmartClient/FirmaDigitaleResultStatusPage.aspx?status=" + docStatus + "&statusDescription=" + docStatusDescription + "&idDocument=" + idDocumento,
                //data: signedValue,
                success: function (data, textStatus, jqXHR) {
                    status = jqXHR.status;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    status = textStatus;
                    content = null;
                },
                async: false
            });

            return (docStatus == 0 || docStatus == 1);
        }

       
        function CloseApplet() {
            disallowOp('Content1');
            try {
                applet.close();
                //applet.killApplet();
                return true;
            }
            catch (err) {
                return true;
                //alert(err.Description);
            }
        }
        function CadesChkChange() 
        {
            if (convpdfa==null)
              if (document.getElementById("chkConverti"))
                convpdfa = document.getElementById("chkConverti").checked;   

                document.getElementById("chkConverti").checked= convpdfa;

        }

        function PadesChkChange() 
        {
            if (convpdfa==null)
              if (document.getElementById("chkConverti"))
                convpdfa = document.getElementById("chkConverti").checked;   
            
            //per il pades non posso fare la conversione prima e check dopo
            document.getElementById("chkConverti").checked=false;

        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="MassiveReport" runat="server" Url="../popup/MassiveReport_iframe.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" />
    <applet id='signApplet' 
        code= 'com.nttdata.signapplet.gui.SignApplet' 
        codebase= '<%=Page.ResolveClientUrl("~/Libraries/")%>';
        archive='SignApplet.jar,<%=Page.ResolveClientUrl("~/Libraries/Libs/")%>junit-3.8.1.jar'
		width= '10'   height = '9'>
        <param name="java_arguments" value="-Xms128m" />
        <param name="java_arguments" value="-Xmx256m" />
    </applet>
<div class="container">
    <asp:UpdatePanel ID="UpPnlMessage" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="plcMessage" runat="server">
                <div class="row">
                    <p><asp:Literal ID="litMessage" runat="server" /></p>
                </div>
            </asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="UnPnlSign" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:PlaceHolder ID="plcSign" runat="server">
                <div class="row">
                    <div class="col">
                        <asp:Literal ID="lblListaCertificati" runat="server" />
                    </div>
                    <div class="col">
                        <select language="javascript" id="lstListaCertificati" style="width: 350px;"
                            size="9" name="selectCert" runat="server" ClientIDMode="Static" />
                    </div>
                </div>
                <div class="row">
                <div>
                    <asp:RadioButton ID="optFirma" GroupName="grpTipoFirma" runat="server" ClientIDMode="Static"></asp:RadioButton>
                    <asp:RadioButton ID="optCofirma" GroupName="grpTipoFirma" runat="server" ClientIDMode="Static"></asp:RadioButton>
                </div><br />
                    <asp:CheckBox ID="chkConverti" runat="server" Checked="false" ClientIDMode="Static" />  &nbsp;&nbsp;
                    <asp:RadioButton id="optCades" Text="Cades" Checked="True" GroupName="firmaType" runat="server" ClientIDMode="Static" onmousedown ="CadesChkChange()"/> &nbsp;&nbsp;
                    <asp:RadioButton id="optPades" Text="Pades" Checked="False" GroupName="firmaType" runat="server" ClientIDMode="Static" onmousedown ="PadesChkChange()"/>
                    <asp:Panel ID="pnlConversione" runat="server" ClientIDMode="Static" CssClass="hidden">
                        <asp:RadioButton ID="optLocale" GroupName="grpLocCentr" runat="server" ClientIDMode="Static" />&nbsp;&nbsp;
                        <asp:RadioButton ID="optCentrale" GroupName="grpLocCentr" runat="server" ClientIDMode="Static" Checked="true" />
                    </asp:Panel>
                </div>
                <div class="row">
                    <p align="center"><asp:Literal ID="lblDocumentCount" runat="server" /></p>
                </div>
            </asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upReport" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Panel ID="pnlReport" runat="server" CssClass="row" Visible="false">
                <asp:GridView id="grdReport" runat="server" Width="100%" AutoGenerateColumns="False" CssClass="tbl_rounded_custom round_onlyextreme">         
                    <RowStyle CssClass="NormalRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <PagerStyle CssClass="recordNavigator2" />
                    <Columns>
                        <asp:BoundField HeaderText='<%$ localizeByText:MassiveActionLblGrdReport%>' DataField="ObjId">
                            <HeaderStyle HorizontalAlign="Center" Width="30%" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Esito" DataField="Result">
                            <HeaderStyle HorizontalAlign="Center" Width="30%" />
                        </asp:BoundField>
                        <asp:BoundField HeaderText="Dettagli" DataField="Details">
                            <HeaderStyle HorizontalAlign="Center" Width="30%" />
                        </asp:BoundField>
                        </Columns>
                </asp:GridView>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnConfirm" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnConfirm_Click" OnClientClick="return signWithApplet();" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="return CloseApplet();" />
            <cc1:CustomButton ID="BtnReport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnReport_Click" OnClientClick="disallowOp('Content2');" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
