<%@ Page Language="C#" MasterPageFile="~/MasterPages/Popup.Master" CodeBehind="MassiveDigitalSignature.aspx.cs" Inherits="NttDataWA.Popup.MassiveDigitalSignature" EnableSessionState="ReadOnly" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/ajaxpopup2.ascx" TagPrefix="uc" TagName="ajaxpopup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/json2.js" type="text/javascript"></script>
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
    </style>
    <script type="text/javascript">
        var convpdfa = null;

        function ApplySign() {
            // Reperimento id documenti in modalità multiselezione
            var docs = "<%=this.GetSelectedDocumentsJSON()%>";
            var pdfConv = "<%=this.GetSelectedConv()%>";
            
            if (docs != null && docs != "") {
                if (this.SignDocuments(docs.split("|"), pdfConv.split("|"))) {
                
                    return true;
                }
                else {
                    return false;
                }
            }
            else {
                alert('Nessun documento selezionato per la firma');
                return false;
            }
        }
<%--        /*
        // Firma digitale di più documenti
        function SignDocuments(docs, pdfConv) {
            var retValue = true;

            for (var i = 0; i < docs.length; i++) {
                //this.SignDocument('<%=this.TipoFirma%>', docs[i]);
              
                window.frames['ifrm_wrappers'].SignDocument('<%=this.TipoFirma%>', docs[i], pdfConv[i])
            }

            return retValue;
        }*/--%>

        
        // Firma digitale di più documenti
        function SignDocuments(docs, pdfConv) {
            var retValue = true;
            var pades = false;
            var documento;
            var idDocumento;
            var signed;
            var fileExtension;

            if (document.getElementById("optPades")) {
                pades = document.getElementById("optPades").checked;
                if (padesCheck)
                    pades = true;
                else
                    pades = false;
            }

            //ABBATANGELI - Nuova gestione Sign/Cosign
            var sigCosig = "cosign";
            var optfirma = document.getElementById("optFirma");
            if (optfirma.checked == true) {
                sigCosig = "sign";
            }

            for (var i = 0; i < docs.length; i++) 
            {
                documento = JSON.parse(docs[i]);
                idDocumento = documento.idDocumento;
                signed = documento.isSigned;
                fileExtension = documento.fileExtension;

                if (pades || (signed && (signed === "0"))) {
                    sigCosig = 'sign';
                } 

                if (idDocumento.indexOf("P") > -1) {
                    idDocumento = idDocumento.replace("P", "");
                    window.frames['ifrm_wrappers'].SignDocument(sigCosig, idDocumento, pdfConv[i], true);
                } else if (idDocumento.indexOf("C") > -1) 
                {
                    idDocumento = idDocumento.replace("C", "");
                    window.frames['ifrm_wrappers'].SignDocument(sigCosig, idDocumento, pdfConv[i], false);
                } else {
                    if (pades) {
                        window.frames['ifrm_wrappers'].SignDocument(sigCosig, idDocumento, pdfConv[i], true)
                    }
                    else {
                        window.frames['ifrm_wrappers'].SignDocument(sigCosig, idDocumento, pdfConv[i])
                    }
                }
            }

            return retValue;
        }

        // Funzione per l'inizializzazione dell'interfaccia grafica
        function Initialize() {
            // Recupero della configurazione relativa alla conversione pdf
            // locale e centrale e alla sua abilitazione
            var isEnabledConv = ("<%=ConvertPdfOnSign%>" == "True");
            var convPdfLocale = false;
            var convPdfCentrale = true;

            // Recupero dei riferimenti alla checkbox "Converti in pdf", ai due option button "Locale"
            // e "Centrale" e al pannello che contiene i due option button

            // Se il sistema non è configurato per convertire in pdf prima della
            // firma o se lo è ma nessuna delle due conversioni è operativa,
            // bisogna disabilitare la checkbox "Converti in PDF", togliere
            // il flag e nascondere le due opzioni di conversione
            if (!((isEnabledConv && convPdfLocale) || (isEnabledConv && convPdfCentrale))) {
                if (document.getElementById("chkConverti")) document.getElementById("chkConverti").checked = false;
                if (document.getElementById("chkConverti")) document.getElementById("chkConverti").disabled = true;
                if (document.getElementById("pnlConversione")) document.getElementById("pnlConversione").style.visibility = 'hidden';
            }

            // Fetch della lista dei certificati
            //setTimeout("window.frames['ifrm_wrappers'].FetchListaCertificati()", 2000);

            // Simula il click sul check box "Converti in PDF"
            OnClickConvertPDF();
        }

        // Funzione per la gestione del click sulla check box "Converti in PDF"
        function OnClickConvertPDF() {

            try {
             
                // Recupero della configurazione relativa alla conversione pdf
                // locale e centrale e alla sua abilitazione
                var isEnabledConv = ("<%=ConvertPdfOnSign%>" == "True");
                var convPdfLocale = (isEnabledConv && IsIntegrationActiveAndInstalled());
                var convPdfCentrale = (isEnabledConv && "<%=IsEnabledConvPDFSincrona%>" == "True");

                // Recupero dei riferimenti alla checkbox "Converti in pdf", ai due option button "Locale"
                // e "Centrale" e al pannello che contiene i due option button
                var optLocale = document.getElementById("optLocale");
                var optCentrale = document.getElementById("optCentrale");
                var pnlConversione = document.getElementById("pnlConversione");


                // Recupero dello stato di checking della chkConverti
                var chkConverti = document.getElementById("chkConverti").checked;

                // Se chkConverti è flaggato bisogna abilitare il pannello altrimenti bisogna
                // disabilitarlo
                if (chkConverti)
                    pnlConversione.disabled = false;
                else
                    pnlConversione.disabled = true;

                optLocale.setAttribute("disabled", !(convPdfLocale && chkConverti));
                optCentrale.setAttribute("disabled", !(convPdfCentrale && chkConverti));

                // Se è attiva la conversione locale la si preferisce altrimenti si seleziona
                // la conversione locale
                if (convPdfLocale)
                    optLocale.checked = true;
                else
                    optCentrale.checked = true;
            }
            catch (err) {

            }
        }

        // Analizza l'interfaccia al fine di determinare se bisogna convertire
        // il documento localmente
        function ConvertLocally() {
            // Recupero del riferimento all'option button "Locale" e valutazione
            // dello stato di flagging dell'opzione "Converti in PDF"
            var optLocale = document.getElementById("optLocale");
            var convertInPDF = document.getElementById("chkConverti").checked;

            // Si restituisce l'and fra le due variabili
            return optLocale.checked && convertInPDF;
        }

        // Analizza l'interfaccia al fine di determinare se bisogna convertire
        // il documento centralmente
        function ConvertCentrally() {
            // Recupero del riferimento all'option button "Centrale" e valutazione
            // dello stato di flagging dell'opzione "Converti in PDF"
            var optCentrale = document.getElementById("optCentrale");
            var convertInPDF = document.getElementById("chkConverti").checked;

            // Si restituisce l'and fra le due variabili
            return optCentrale.checked && convertInPDF;

        }

        function CadesChkChange() {
            if (convpdfa == null)
                if (document.getElementById("chkConverti"))
                    convpdfa = document.getElementById("chkConverti").checked;

            document.getElementById("chkConverti").checked = convpdfa;

        }

        function PadesChkChange() {
            if (convpdfa == null)
                if (document.getElementById("chkConverti"))
                    convpdfa = document.getElementById("chkConverti").checked;

            //per il pades non posso fare la conversione prima e check dopo
            document.getElementById("chkConverti").checked = false;

        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <uc:ajaxpopup Id="MassiveReport" runat="server" Url="../popup/MassiveReport_iframe.aspx"
        IsFullScreen="true" PermitClose="false" PermitScroll="true" />
    <iframe id="ifrm_wrappers" width="300" height="10" frameborder="0" scrolling="no" src="MassiveDigitalSignature_iframe.aspx"></iframe>

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
                    <asp:CheckBox ID="chkConverti" runat="server" Checked="false" ClientIDMode="Static" /> &nbsp;&nbsp;
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
<asp:Button text="prova" ID="BtnHidden" runat="server" OnClick="BtnConfirm_Click" CssClass="hidden" ClientIDMode="Static" OnClientClick="if (ApplySign()) $('#BtnHidden').click();" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel ID="UpPnlButtons" runat="server" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnConfirm" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnConfirm_Click" OnClientClick="disallowOp('Content2'); return ApplySign();" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnClose_Click" OnClientClick="disallowOp('Content2');" />
            <cc1:CustomButton ID="BtnReport" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClick="BtnReport_Click" OnClientClick="disallowOp('Content2');" />

            <asp:PlaceHolder ID="plcPopup" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
