<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchDocument.aspx.cs"
    Inherits="NttDataWA.Document.SearchDocument" MasterPageFile="~/MasterPages/Base.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="uc1" %>
<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<asp:Content ID="ContentPlaceHolderContent" ContentPlaceHolderID="ContentPlaceHolderContent"
    runat="server">
    <div id="containerTop">
        <div id="containerDocumentTop">
            <asp:UpdatePanel ID="UpLetterTypeProtocol" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="PnlcontainerDocumentTopSx" runat="server">
                        <p runat="server" id="PProtocolType">
                        </p>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpcontainerDocumentTopCx" UpdateMode="Conditional">
                <ContentTemplate>
                    <div id="containerDocumentTopCxOrange" runat="server" clientidmode="Static">
                        <div id="containerDocumentTopCxOrangeSx">
                            <div class="rowTop2">
                                <div class="colTop">
                                    <strong>Id</strong> <asp:Label ID="LblIdDocument" runat="server">12449178</asp:Label></li>
                                </div>
                                <div class="colTop">
                                    <strong>Protocollo:</strong> <span class="redWeight">
                                        <asp:Label ID="LblReferenceCode" runat="server">PAT_COLL/RFD320-2012-0000977</asp:Label></span></li>
                                </div>
                                <div class="colTop">
                                    <strong>Repertorio:</strong>
                                    <asp:Label ID="LblRepertory" runat="server">25/2012</asp:Label>
                                </div>
                            </div>
                            <div class="rowTop">
                                <div class="colTop">
                                    <strong>Data Creazione</strong>: 19/10/2012 14:22:10
                                </div>
                                <div class="colTop">
                                    <strong>Data Protocollazione</strong>: 19/10/2012 14:22:10
                                </div>
                            </div>
                        </div>
                        <div id="containerDocumentTopCxOrangeDx">
                            <div id="containerDocumentTopCxOrangeDxSx">
                                <p>
                                    Etichetta
                                    <cc1:CustomTextArea ID="CustomTextArea1" Text="1" runat="server" CssClass="txtLabelPrint">
                                    </cc1:CustomTextArea></p>
                            </div>
                            <div id="containerDocumentTopCxOrangeDxDx">
                                <ul>
                                    <li>
                                        <img src="<%=Page.ResolveClientUrl("~/Images/Icons/print_label.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/print_label_hover.png")%>';"
                                            onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/print_label.png")%>';"
                                            alt="Stampa etichetta" title="Stampa etichetta" id="Img2" class="clickable" /></li>
                                    <li>
                                        <img src="<%=Page.ResolveClientUrl("~/Images/Icons/preservation_story.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/preservation_story_hover.png")%>';"
                                            onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/preservation_story.png")%>';"
                                            alt="Visualizza storia conservazione documento" title="Visualizza storia conservazione documento"
                                            id="Img3" class="clickable" /></li>
                                    <li>
                                        <img src="<%=Page.ResolveClientUrl("~/Images/Icons/consolidate_1.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/consolidate_1_hover.png")%>';"
                                            onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/consolidate_1.png")%>';"
                                            alt="Consolida metadati" title="Conosolida metadati" id="Img4" class="clickable" /></li>
                                    <li>
                                        <img src="<%=Page.ResolveClientUrl("~/Images/Icons/consolidate_2.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/consolidate_2_hover.png")%>';"
                                            onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/consolidate_2.png")%>';"
                                            alt="Consolida contenuto e metadati" title="Consolida contenuto e metadati" id="Img5"
                                            class="clickable" /></li>
                                    <li>
                                        <img src="<%=Page.ResolveClientUrl("~/Images/Icons/send_receipt.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/send_receipt_hover.png")%>';"
                                            onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/send_receipt.png")%>';"
                                            alt="Invia ricevuta" title="Invia ricevuta" id="Img6" class="clickable" /></li>
                                    <li>
                                        <img src="<%=Page.ResolveClientUrl("~/Images/Icons/print_a4.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/print_a4_hover.png")%>';"
                                            onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/print_a4.png")%>';"
                                            alt="Stampa segnatura A4" title="Stampa segnatura A4" id="Img7" class="clickable" /></li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel ID="UpcontainerDocumentTopDx" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Panel ID="containerDocumentTopDx" runat="server" ClientIDMode="Static">
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <asp:UpdatePanel runat="server" ID="UpContainerDocumentTab" UpdateMode="Conditional">
            <ContentTemplate>
                <div id="containerDocumentTab" runat="server" clientidmode="Static">
                    <div id="containerDocumentTabOrangeInternalSpace">
                        <div id="containerDocumentTabOrangeSx">
                            <ul>
                                <li class="docIAmProfile" id="link1" runat="server">
                                    <asp:HyperLink ID="LinkProfile" runat="server" Text="Profilo" NavigateUrl="#"></asp:HyperLink></li>
                                <li class="docOther" id="link2" runat="server">
                                    <asp:HyperLink ID="LinkAttachedFiles" runat="server" Text="Allegati" NavigateUrl="#"></asp:HyperLink></li>
                                <li class="docOther" id="link3" runat="server">
                                    <asp:HyperLink ID="LinkClassificationSchemes" runat="server" Text="Classifica" NavigateUrl="#"></asp:HyperLink></li>
                                <li class="docOther" id="link4" runat="server">
                                    <asp:HyperLink ID="LinkTransmissions" runat="server" Text="Trasmissioni" NavigateUrl="#"></asp:HyperLink></li>
                                <li class="docOther" id="link5" runat="server">
                                    <asp:HyperLink ID="LinkVisibility" runat="server" Text="Visibilità" NavigateUrl="#"></asp:HyperLink></li>
                                <li class="docOther" id="link6" runat="server">
                                    <asp:HyperLink ID="LinkEvents" runat="server" Text="Eventi" NavigateUrl="#"></asp:HyperLink></li>
                            </ul>
                        </div>
                        <div id="containerDocumentTabOrangeDx">
                            <div id="buttons">
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/upload_file.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/upload_file_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/upload_file.png")%>';"
                                    id="Img15" class="clickable" alt="Acquisisci documento" title="Acquisisci documento" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/view_file.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/view_file_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/view_file.png")%>';"
                                    alt="Visualizza documento" title="Visualizza documento" id="Img16" class="clickable" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/zoom.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/zoom_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/zoom.png")%>';"
                                    alt="Zoom" title="Zoom" id="Img17" class="clickable" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/view_signature.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/view_signature_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/view_signature.png")%>';"
                                    alt="Visualizza documento con segnatura (solo PDF)" title="Visualizza documento con segnatura (solo PDF)"
                                    id="Img18" class="clickable" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/signature_position.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/signature_position_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/signature_position.png")%>';"
                                    alt="Posizionamento della segnatura" title="Posizionamento della segnatura" id="Img19"
                                    class="clickable" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/signature.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/signature_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/signature.png")%>';"
                                    alt="Firma esterna" title="Firma esterna" id="Img20" class="clickable" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/co_signature.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/co_signature_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/co_signature.png")%>';"
                                    alt="Co-firma" title="Co-firma" id="Img21" class="clickable" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/signature_details.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/signature_details_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/signature_details.png")%>';"
                                    alt="Dettaglio firma" title="Dettaglio firma" id="Img22" class="clickable" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/save_local_file.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/save_local_file_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/save_local_file.png")%>';"
                                    alt="Copia file in locale" title="Copia file in locale" id="Img23" class="clickable" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/lock.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/lock_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/lock.png")%>';"
                                    alt="Blocca" title="Blocca" id="Img24" class="clickable" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/unlock.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/unlock_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/unlock.png")%>';"
                                    alt="Rilascia" title="Rilascia" id="Img25" class="clickable" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/lock_no_save.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/lock_no_save_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/lock_no_save.png")%>';"
                                    alt="Rilascia senza salvare" title="Rilascia senza salvare" id="Img26" class="clickable" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/open_file.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/open_file_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/open_file.png")%>';"
                                    alt="Apri" title="Apri" id="Img27" class="clickable" />
                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/model_file.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/model_file_hover.png")%>';"
                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/model_file.png")%>';"
                                    alt="Apri modello documento" title="Apri modello documento" id="Img28" class="clickable" />
                            </div>
                        </div>
                    </div>
                    <asp:UpdatePanel runat="server" ID="UpcontainerDocumentTabDxBorder" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div id="containerDocumentTabDxBorder" runat="server" clientidmode="Static">
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <asp:UpdatePanel ID="UpContainer" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div id="container" runat="server" clientidmode="Static">
                <div id="content">
                    <div id="contentSx">
                        <div class="box_inside">
                            <div class="row">
                                <div class="col">
                                    <div id="colBottom">
                                        <asp:UpdatePanel runat="server" ID="UpTypeProtocol" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:RadioButtonList runat="server" ID="RblTypeProtocol" OnSelectedIndexChanged="RblTypeProtocol_OnSelectedIndexChanged"
                                                    AutoPostBack="true" RepeatDirection="Horizontal">
                                                    <asp:ListItem Text="Arrivo" Value="A" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Text="Partenza" Value="P"></asp:ListItem>
                                                    <asp:ListItem Text="Interno" Value="I"></asp:ListItem>
                                                </asp:RadioButtonList>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>
                                <div class="col-right">
                                    <div id="colBottom3">
                                        <asp:CheckBox ID="CheckBox1" Text="Privato" runat="server" />
                                    </div>
                                </div>
                                <div class="col-right">
                                    <div id="colBottom2">
                                        <asp:Panel ID="PnlRegistry" runat="server">
                                            <asp:Label ID="Label1" runat="server">Registro</asp:Label>
                                            <asp:DropDownList runat="server" ID="DdlRegistries">
                                            </asp:DropDownList>
                                        </asp:Panel>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <fieldset>
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">Oggetto</span><span class="little">*</span></p>
                                        </div>
                                        <div class="col-right-no-margin">
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/obj_description.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/obj_description_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/obj_description.png")%>';"
                                                alt="Descrizione campo oggetto" title="Descrizione campo oggetto" class="clickable" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/obj_objects.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/obj_objects_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/obj_objects.png")%>';"
                                                alt="Seleziona un oggetto dall'oggettario" title="Seleziona un oggetto dall'oggettario"
                                                id="Img1" class="clickable" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <cc1:CustomTextArea ID="TxtObject" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                                            Text="Provvedimento disciplinare Giuseppe Verdi"></cc1:CustomTextArea>
                                    </div>
                                </fieldset>
                                <asp:Panel ID="PnlSenders" runat="server">
                                    <fieldset>
                                        <div class="row">
                                            <div class="col">
                                                <p>
                                                    <span class="weight">Mittente</span><span class="little">*</span></p>
                                            </div>
                                            <div class="col-right-no-margin">
                                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/address_book.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/address_book_hover.png")%>';"
                                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/address_book.png")%>';"
                                                    alt="Seleziona un mittente nella rubrica" title="Seleziona un mittente nella rubrica"
                                                    id="Img8" class="clickable" />
                                                <img src="<%=Page.ResolveClientUrl("~/Images/Icons/address_book_details.png")%>"
                                                    onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/address_book_details_hover.png")%>';"
                                                    onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/address_book_details.png")%>';"
                                                    alt="Dettaglio mittente" title="Dettaglio mittente" id="Img9" class="clickable" />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="full_width">
                                                <asp:UpdatePanel runat="server" ID="UpPnlSender" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:HiddenField ID="IdSender" runat="server" />
                                                        <cc1:CustomTextArea ID="TxtCodeSender" runat="server" Width="20%" CssClass="txt_addressBook"
                                                            OnTextChanged="TxtCodeSender_OnTextChanged" AutoPostBack="true" Text="B001"></cc1:CustomTextArea>
                                                        <cc1:CustomTextArea ID="TxtDescriptionSender" runat="server" Width="74%" CssClass="txt_addressBook"
                                                            Text="Avvocatura della Provincia"></cc1:CustomTextArea>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>
                                            </div>
                                        </div>
                                        <asp:UpdatePanel ID="UpPnlMultipleSender" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:Panel ID="PnlMultipleSender" runat="server">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">Mittenti Multipli</span></p>
                                                        </div>
                                                        <div class="col">
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/down_arrow.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/down_arrow_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/down_arrow.png")%>';"
                                                                alt="Sposta il mittente nei mittenti multipli" title="Sposta il mittente nei mittenti multipli"
                                                                id="Img13" class="clickable" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/up_arrow.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/up_arrow_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/up_arrow.png")%>';"
                                                                alt="Sposta il mittente multiplo selezionato nel mittente" title="Sposta il mittente multiplo selezionato nel mittente"
                                                                id="Img14" class="clickable" />
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/address_book.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/address_book_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/address_book.png")%>';"
                                                                alt="Seleziona un mittente nella rubrica" title="Seleziona un mittente nella rubrica"
                                                                id="Img10" class="clickable" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/address_book_details.png")%>"
                                                                onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/address_book_details_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/address_book_details.png")%>';"
                                                                alt="Dettagli mittenti multipli" title="Dettagli mittenti multipli" id="Img11"
                                                                class="clickable" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/delete.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/delete_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/delete.png")%>';"
                                                                alt="Elimina mittente selezionato" title="Elimina mittente selezionato" id="Img12"
                                                                class="clickable" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <cc1:CustomTextArea ID="CustomTextArea2" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                                                            Enabled="false" CssClassReadOnly="txt_textarea_disabled"></cc1:CustomTextArea>
                                                    </div>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                        <asp:UpdatePanel ID="UpPnlRecipients" runat="server" UpdateMode="Conditional">
                                            <ContentTemplate>
                                                <asp:Panel ID="PnlRecipients" runat="server">
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">Destinatario</span></p>
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/print_letter_recipients.png")%>"
                                                                onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/print_letter_recipients_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/print_letter_recipients.png")%>';"
                                                                alt="Stampa le buste dei destinatari" title="Stampa le buste dei destinatari"
                                                                id="Img48" class="clickable" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/address_book.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/address_book_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/address_book.png")%>';"
                                                                alt="Seleziona un destinatario nella rubrica" title="Seleziona un destinatario nella rubrica"
                                                                id="Img49" class="clickable" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/add_version.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/add_version_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/add_version.png")%>';"
                                                                alt="Aggiungi" title="Aggiungi" id="Img50" class="clickable" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="full_width">
                                                            <cc1:CustomTextArea ID="CustomTextArea9" runat="server" Width="20%" CssClass="txt_addressBook"></cc1:CustomTextArea>
                                                            <cc1:CustomTextArea ID="CustomTextArea10" runat="server" Width="77%" CssClass="txt_addressBook"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">Destinatari</span><span class="little">*</span></p>
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/sending.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/sending_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/sending.png")%>';"
                                                                alt="Spedizione" title="Spedizione" id="Img52" class="clickable" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/recipient_details.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/recipient_details_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/recipient_details.png")%>';"
                                                                alt="Destinatari principali" title="Destinatari principali" id="Img53" class="clickable" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/recipients_details.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/recipients_details_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/recipients_details.png")%>';"
                                                                alt="Dettagli" title="Dettagli" id="Img54" class="clickable" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/delete.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/delete_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/delete.png")%>';"
                                                                alt="Cancella il destinatario selezionato" title="Cancella il destinatario selezionato"
                                                                id="Img51" class="clickable" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <cc1:CustomTextArea ID="CustomTextArea11" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                                                            Enabled="false" CssClassReadOnly="txt_textarea_disabled"></cc1:CustomTextArea>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <p>
                                                                <span class="weight">Destinatari CC</span></p>
                                                        </div>
                                                        <div class="col">
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/down_arrow.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/down_arrow_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/down_arrow.png")%>';"
                                                                alt="Sposta il destinatario selezionato nei destinatari CC" title="Sposta il destinatario selezionato nei destinatari CC"
                                                                id="Img55" class="clickable" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/up_arrow.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/up_arrow_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/up_arrow.png")%>';"
                                                                alt="Sposta il destinatario in CC selezionato nei desinatari" title="Sposta il destinatario in CC selezionato nei desinatari"
                                                                id="Img56" class="clickable" />
                                                        </div>
                                                        <div class="col-right-no-margin">
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/recipient_details.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/recipient_details_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/recipient_details.png")%>';"
                                                                alt="Destinatari principali" title="Destinatari principali" id="Img57" class="clickable" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/recipients_details.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/recipients_details_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/recipients_details.png")%>';"
                                                                alt="Dettagli" title="Dettagli" id="Img58" class="clickable" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/delete.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/delete_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/delete.png")%>';"
                                                                alt="Cancella il destinatario selezionato" title="Cancella il destinatario selezionato"
                                                                id="Img59" class="clickable" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <cc1:CustomTextArea ID="CustomTextArea14" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                                                            Enabled="false" CssClassReadOnly="txt_textarea_disabled"></cc1:CustomTextArea>
                                                    </div>
                                                </asp:Panel>
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </fieldset>
                                </asp:Panel>
                                <asp:UpdatePanel ID="UpPnlMeansSender" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:Panel ID="PnlMeansSender" runat="server">
                                            <fieldset>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">Mezzo spedizione</span></p>
                                                    </div>
                                                    <select name="ListBox1" id="ListBox1" class="chzn-select-deselect" data-placeholder="Scegli un Paese">
                                                        <option value=""></option>
                                                        <option value="United States">United States</option>
                                                        <option value="United Kingdom">United Kingdom</option>
                                                        <option value="Afghanistan">Afghanistan</option>
                                                        <option value="Albania">Albania</option>
                                                        <option value="Algeria">Algeria</option>
                                                        <option value="American Samoa">American Samoa</option>
                                                        <option value="Andorra">Andorra</option>
                                                        <option value="Angola">Angola</option>
                                                        <option value="Anguilla">Anguilla</option>
                                                        <option value="Antarctica">Antarctica</option>
                                                        <option value="Antigua and Barbuda">Antigua and Barbuda</option>
                                                        <option value="Argentina">Argentina</option>
                                                        <option value="Armenia">Armenia</option>
                                                        <option value="Aruba">Aruba</option>
                                                        <option value="Australia">Australia</option>
                                                        <option value="Austria">Austria</option>
                                                        <option value="Azerbaijan">Azerbaijan</option>
                                                        <option value="Bahamas">Bahamas</option>
                                                        <option value="Bahrain">Bahrain</option>
                                                        <option value="Bangladesh">Bangladesh</option>
                                                        <option value="Barbados">Barbados</option>
                                                        <option value="Belarus">Belarus</option>
                                                        <option value="Belgium">Belgium</option>
                                                        <option value="Belize">Belize</option>
                                                        <option value="Benin">Benin</option>
                                                        <option value="Bermuda">Bermuda</option>
                                                        <option value="Bhutan">Bhutan</option>
                                                        <option value="Bolivia">Bolivia</option>
                                                        <option value="Bosnia and Herzegovina">Bosnia and Herzegovina</option>
                                                        <option value="Botswana">Botswana</option>
                                                        <option value="Bouvet Island">Bouvet Island</option>
                                                        <option value="Brazil">Brazil</option>
                                                        <option value="British Indian Ocean Territory">British Indian Ocean Territory</option>
                                                        <option value="Brunei Darussalam">Brunei Darussalam</option>
                                                        <option value="Bulgaria">Bulgaria</option>
                                                        <option value="Burkina Faso">Burkina Faso</option>
                                                        <option value="Burundi">Burundi</option>
                                                        <option value="Cambodia">Cambodia</option>
                                                        <option value="Cameroon">Cameroon</option>
                                                        <option value="Canada">Canada</option>
                                                        <option value="Cape Verde">Cape Verde</option>
                                                        <option value="Cayman Islands">Cayman Islands</option>
                                                        <option value="Central African Republic">Central African Republic</option>
                                                        <option value="Chad">Chad</option>
                                                        <option value="Chile">Chile</option>
                                                        <option value="China">China</option>
                                                        <option value="Christmas Island">Christmas Island</option>
                                                        <option value="Cocos (Keeling) Islands">Cocos (Keeling) Islands</option>
                                                        <option value="Colombia">Colombia</option>
                                                        <option value="Comoros">Comoros</option>
                                                        <option value="Congo">Congo</option>
                                                        <option value="Congo, The Democratic Republic of The">Congo, The Democratic Republic
                                                            of The</option>
                                                        <option value="Cook Islands">Cook Islands</option>
                                                        <option value="Costa Rica">Costa Rica</option>
                                                        <option value="Cote D&#39;ivoire">Cote D&#39;ivoire</option>
                                                        <option value="Croatia">Croatia</option>
                                                        <option value="Cuba">Cuba</option>
                                                        <option value="Cyprus">Cyprus</option>
                                                        <option value="Czech Republic">Czech Republic</option>
                                                        <option value="Denmark">Denmark</option>
                                                        <option value="Djibouti">Djibouti</option>
                                                        <option value="Dominica">Dominica</option>
                                                        <option value="Dominican Republic">Dominican Republic</option>
                                                        <option value="Ecuador">Ecuador</option>
                                                        <option value="Egypt">Egypt</option>
                                                        <option value="El Salvador">El Salvador</option>
                                                        <option value="Equatorial Guinea">Equatorial Guinea</option>
                                                        <option value="Eritrea">Eritrea</option>
                                                        <option value="Estonia">Estonia</option>
                                                        <option value="Ethiopia">Ethiopia</option>
                                                        <option value="Falkland Islands (Malvinas)">Falkland Islands (Malvinas)</option>
                                                        <option value="Faroe Islands">Faroe Islands</option>
                                                        <option value="Fiji">Fiji</option>
                                                        <option value="Finland">Finland</option>
                                                        <option value="France">France</option>
                                                        <option value="French Guiana">French Guiana</option>
                                                        <option value="French Polynesia">French Polynesia</option>
                                                        <option value="French Southern Territories">French Southern Territories</option>
                                                        <option value="Gabon">Gabon</option>
                                                        <option value="Gambia">Gambia</option>
                                                        <option value="Georgia">Georgia</option>
                                                        <option value="Germany">Germany</option>
                                                        <option value="Ghana">Ghana</option>
                                                        <option value="Gibraltar">Gibraltar</option>
                                                        <option value="Greece">Greece</option>
                                                        <option value="Greenland">Greenland</option>
                                                        <option value="Grenada">Grenada</option>
                                                        <option value="Guadeloupe">Guadeloupe</option>
                                                        <option value="Guam">Guam</option>
                                                        <option value="Guatemala">Guatemala</option>
                                                        <option value="Guinea">Guinea</option>
                                                        <option value="Guinea-bissau">Guinea-bissau</option>
                                                        <option value="Guyana">Guyana</option>
                                                        <option value="Haiti">Haiti</option>
                                                        <option value="Heard Island and Mcdonald Islands">Heard Island and Mcdonald Islands</option>
                                                        <option value="Holy See (Vatican City State)">Holy See (Vatican City State)</option>
                                                        <option value="Honduras">Honduras</option>
                                                        <option value="Hong Kong">Hong Kong</option>
                                                        <option value="Hungary">Hungary</option>
                                                        <option value="Iceland">Iceland</option>
                                                        <option value="India">India</option>
                                                        <option value="Indonesia">Indonesia</option>
                                                        <option value="Iran, Islamic Republic of">Iran, Islamic Republic of</option>
                                                        <option value="Iraq">Iraq</option>
                                                        <option value="Ireland">Ireland</option>
                                                        <option value="Israel">Israel</option>
                                                        <option value="Italy">Italy</option>
                                                        <option value="Jamaica">Jamaica</option>
                                                        <option value="Japan">Japan</option>
                                                        <option value="Jordan">Jordan</option>
                                                        <option value="Kazakhstan">Kazakhstan</option>
                                                        <option value="Kenya">Kenya</option>
                                                        <option value="Kiribati">Kiribati</option>
                                                        <option value="Korea, Democratic People&#39;s Republic of">Korea, Democratic People&#39;s
                                                            Republic of</option>
                                                        <option value="Korea, Republic of">Korea, Republic of</option>
                                                        <option value="Kuwait">Kuwait</option>
                                                        <option value="Kyrgyzstan">Kyrgyzstan</option>
                                                        <option value="Lao People&#39;s Democratic Republic">Lao People&#39;s Democratic Republic</option>
                                                        <option value="Latvia">Latvia</option>
                                                        <option value="Lebanon">Lebanon</option>
                                                        <option value="Lesotho">Lesotho</option>
                                                        <option value="Liberia">Liberia</option>
                                                        <option value="Libyan Arab Jamahiriya">Libyan Arab Jamahiriya</option>
                                                        <option value="Liechtenstein">Liechtenstein</option>
                                                        <option value="Lithuania">Lithuania</option>
                                                        <option value="Luxembourg">Luxembourg</option>
                                                        <option value="Macao">Macao</option>
                                                        <option value="Macedonia, The Former Yugoslav Republic of">Macedonia, The Former Yugoslav
                                                            Republic of</option>
                                                        <option value="Madagascar">Madagascar</option>
                                                        <option value="Malawi">Malawi</option>
                                                        <option value="Malaysia">Malaysia</option>
                                                        <option value="Maldives">Maldives</option>
                                                        <option value="Mali">Mali</option>
                                                        <option value="Malta">Malta</option>
                                                        <option value="Marshall Islands">Marshall Islands</option>
                                                        <option value="Martinique">Martinique</option>
                                                        <option value="Mauritania">Mauritania</option>
                                                        <option value="Mauritius">Mauritius</option>
                                                        <option value="Mayotte">Mayotte</option>
                                                        <option value="Mexico">Mexico</option>
                                                        <option value="Micronesia, Federated States of">Micronesia, Federated States of</option>
                                                        <option value="Moldova, Republic of">Moldova, Republic of</option>
                                                        <option value="Monaco">Monaco</option>
                                                        <option value="Mongolia">Mongolia</option>
                                                        <option value="Montenegro">Montenegro</option>
                                                        <option value="Montserrat">Montserrat</option>
                                                        <option value="Morocco">Morocco</option>
                                                        <option value="Mozambique">Mozambique</option>
                                                        <option value="Myanmar">Myanmar</option>
                                                        <option value="Namibia">Namibia</option>
                                                        <option value="Nauru">Nauru</option>
                                                        <option value="Nepal">Nepal</option>
                                                        <option value="Netherlands">Netherlands</option>
                                                        <option value="Netherlands Antilles">Netherlands Antilles</option>
                                                        <option value="New Caledonia">New Caledonia</option>
                                                        <option value="New Zealand">New Zealand</option>
                                                        <option value="Nicaragua">Nicaragua</option>
                                                        <option value="Niger">Niger</option>
                                                        <option value="Nigeria">Nigeria</option>
                                                        <option value="Niue">Niue</option>
                                                        <option value="Norfolk Island">Norfolk Island</option>
                                                        <option value="Northern Mariana Islands">Northern Mariana Islands</option>
                                                        <option value="Norway">Norway</option>
                                                        <option value="Oman">Oman</option>
                                                        <option value="Pakistan">Pakistan</option>
                                                        <option value="Palau">Palau</option>
                                                        <option value="Palestinian Territory, Occupied">Palestinian Territory, Occupied</option>
                                                        <option value="Panama">Panama</option>
                                                        <option value="Papua New Guinea">Papua New Guinea</option>
                                                        <option value="Paraguay">Paraguay</option>
                                                        <option value="Peru">Peru</option>
                                                        <option value="Philippines">Philippines</option>
                                                        <option value="Pitcairn">Pitcairn</option>
                                                        <option value="Poland">Poland</option>
                                                        <option value="Portugal">Portugal</option>
                                                        <option value="Puerto Rico">Puerto Rico</option>
                                                        <option value="Qatar">Qatar</option>
                                                        <option value="Reunion">Reunion</option>
                                                        <option value="Romania">Romania</option>
                                                        <option value="Russian Federation">Russian Federation</option>
                                                        <option value="Rwanda">Rwanda</option>
                                                        <option value="Saint Helena">Saint Helena</option>
                                                        <option value="Saint Kitts and Nevis">Saint Kitts and Nevis</option>
                                                        <option value="Saint Lucia">Saint Lucia</option>
                                                        <option value="Saint Pierre and Miquelon">Saint Pierre and Miquelon</option>
                                                        <option value="Saint Vincent and The Grenadines">Saint Vincent and The Grenadines</option>
                                                        <option value="Samoa">Samoa</option>
                                                        <option value="San Marino">San Marino</option>
                                                        <option value="Sao Tome and Principe">Sao Tome and Principe</option>
                                                        <option value="Saudi Arabia">Saudi Arabia</option>
                                                        <option value="Senegal">Senegal</option>
                                                        <option value="Serbia">Serbia</option>
                                                        <option value="Seychelles">Seychelles</option>
                                                        <option value="Sierra Leone">Sierra Leone</option>
                                                        <option value="Singapore">Singapore</option>
                                                        <option value="Slovakia">Slovakia</option>
                                                        <option value="Slovenia">Slovenia</option>
                                                        <option value="Solomon Islands">Solomon Islands</option>
                                                        <option value="Somalia">Somalia</option>
                                                        <option value="South Africa">South Africa</option>
                                                        <option value="South Georgia and The South Sandwich Islands">South Georgia and The South
                                                            Sandwich Islands</option>
                                                        <option value="South Sudan">South Sudan</option>
                                                        <option value="Spain">Spain</option>
                                                        <option value="Sri Lanka">Sri Lanka</option>
                                                        <option value="Sudan">Sudan</option>
                                                        <option value="Suriname">Suriname</option>
                                                        <option value="Svalbard and Jan Mayen">Svalbard and Jan Mayen</option>
                                                        <option value="Swaziland">Swaziland</option>
                                                        <option value="Sweden">Sweden</option>
                                                        <option value="Switzerland">Switzerland</option>
                                                        <option value="Syrian Arab Republic">Syrian Arab Republic</option>
                                                        <option value="Taiwan, Republic of China">Taiwan, Republic of China</option>
                                                        <option value="Tajikistan">Tajikistan</option>
                                                        <option value="Tanzania, United Republic of">Tanzania, United Republic of</option>
                                                        <option value="Thailand">Thailand</option>
                                                        <option value="Timor-leste">Timor-leste</option>
                                                        <option value="Togo">Togo</option>
                                                        <option value="Tokelau">Tokelau</option>
                                                        <option value="Tonga">Tonga</option>
                                                        <option value="Trinidad and Tobago">Trinidad and Tobago</option>
                                                        <option value="Tunisia">Tunisia</option>
                                                        <option value="Turkey">Turkey</option>
                                                        <option value="Turkmenistan">Turkmenistan</option>
                                                        <option value="Turks and Caicos Islands">Turks and Caicos Islands</option>
                                                        <option value="Tuvalu">Tuvalu</option>
                                                        <option value="Uganda">Uganda</option>
                                                        <option value="Ukraine">Ukraine</option>
                                                        <option value="United Arab Emirates">United Arab Emirates</option>
                                                        <option value="United Kingdom">United Kingdom</option>
                                                        <option value="United States">United States</option>
                                                        <option value="United States Minor Outlying Islands">United States Minor Outlying Islands</option>
                                                        <option value="Uruguay">Uruguay</option>
                                                        <option value="Uzbekistan">Uzbekistan</option>
                                                        <option value="Vanuatu">Vanuatu</option>
                                                        <option value="Venezuela">Venezuela</option>
                                                        <option value="Viet Nam">Viet Nam</option>
                                                        <option value="Virgin Islands, British">Virgin Islands, British</option>
                                                        <option value="Virgin Islands, U.S.">Virgin Islands, U.S.</option>
                                                        <option value="Wallis and Futuna">Wallis and Futuna</option>
                                                        <option value="Western Sahara">Western Sahara</option>
                                                        <option value="Yemen">Yemen</option>
                                                        <option value="Zambia">Zambia</option>
                                                        <option value="Zimbabwe">Zimbabwe</option>
                                                    </select>
                                                </div>
                                            </fieldset>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                                <fieldset>
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">1 nota visibile</span></p>
                                        </div>
                                        <div class="col-right-no-margin">
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_objects.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_objects_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_objects.png")%>';"
                                                alt="Consulta le note" title="Consulta le note" id="Img40" class="clickable" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            Nota di PR29058 (Segreteria Dipartimento Innovaizone, Ricerca e ICT)
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <input type="radio" class="Arrivo" name="note" />
                                            Personale
                                            <input type="radio" class="Partenza" name="note" />
                                            Ruolo
                                            <input type="radio" class="Interno" name="note" />
                                            RF
                                            <input type="radio" class="Interno" name="note" checked="checked" />
                                            Tutti
                                        </div>
                                    </div>
                                    <div class="row">
                                        <cc1:CustomTextArea ID="CustomTextArea3" runat="server" TextMode="MultiLine" CssClass="txt_textarea"
                                            Text="Il documento ha alta priorità"></cc1:CustomTextArea>
                                    </div>
                                </fieldset>
                                <fieldset>
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">Risposta al protocollo</span></p>
                                        </div>
                                        <div class="col-right-no-margin">
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/search_response_documents.png")%>"
                                                onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/search_response_documents_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/search_response_documents.png")%>';"
                                                alt="Ricerca i documenti a cui poter rispondere" title="Ricerca i documenti a cui poter rispondere"
                                                id="Img41" class="clickable" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/response_protocol.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/response_protocol_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/response_protocol.png")%>';"
                                                alt="Crea protocollo in risposta" title="Crea protocollo in risposta" id="Img42"
                                                class="clickable" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/response_document.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/response_document_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/response_document.png")%>';"
                                                alt="Crea documento non protocollato in risposta" title="Crea documento non protocollato in risposta"
                                                id="Img43" class="clickable" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/view_response_documents.png")%>"
                                                onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/view_response_documents_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/view_response_documents.png")%>';"
                                                alt="Visualizza elenco documenti in risposta" title="Visualizza elenco documenti in risposta"
                                                id="Img47" class="clickable" />
                                        </div>
                                    </div>
                                </fieldset>
                                <fieldset>
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">Fascicolazione rapida</span><span class="little">*</span></p>
                                        </div>
                                        <div class="col-right-no-margin">
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/classification_scheme.png")%>"
                                                onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/classification_scheme_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/classification_scheme.png")%>';"
                                                alt="Titolario" title="Titolario" id="Img44" class="clickable" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/search_projects.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/search_projects_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/search_projects.png")%>';"
                                                alt="Ricerca fasccioli" title="Ricerca fascicoli" id="Img45" class="clickable" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="full_width">
                                            <asp:UpdatePanel ID="UpPnlProject" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:HiddenField ID="IdProject" runat="server" />
                                                    <cc1:CustomTextArea ID="TxtCodeProject" runat="server" Width="20%" CssClass="txt_addressBook"
                                                        OnTextChanged="TxtCodeProject_OnTextChanged" AutoPostBack="true"></cc1:CustomTextArea>
                                                    <cc1:CustomTextArea ID="TxtDescriptionProject" runat="server" Width="74%" CssClass="txt_addressBook"></cc1:CustomTextArea>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">Trasmissione rapida</span></p>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col">
                                            <div class="styled-select">
                                                <asp:DropDownList ID="DropDownList2" runat="server">
                                                    <asp:ListItem Text=""></asp:ListItem>
                                                    <asp:ListItem Text="Consegna a Mano"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </fieldset>
                                <fieldset class="demo">
                                    <div class="row">
                                        <div class="col">
                                            <p>
                                                <span class="weight">Tipologia documento</span></p>
                                        </div>
                                    </div>
                                    <asp:UpdatePanel runat="server" ID="UpPnlTypeDocument" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row">
                                                <div class="col">
                                                    <div class="styled-select_full">
                                                        <asp:DropDownList ID="DdlTypeDocument" runat="server" OnSelectedIndexChanged="DdlTypeDocument_OnSelectedIndexChanged"
                                                            AutoPostBack="True">
                                                            <asp:ListItem Text="Sentenza"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <asp:Panel ID="PnlTypeDocument" runat="server" Visible="false">
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">Stato</span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <div class="styled-select_full">
                                                            <asp:DropDownList ID="DropDownList4" runat="server">
                                                                <asp:ListItem Text="Chiuso"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">N° sentenza</span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <div class="styled-select_full">
                                                            <cc1:CustomTextArea ID="CustomTextArea6" runat="server" CssClass="txt_number" Text="25/2012"></cc1:CustomTextArea>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">Autorità giudiziaria</span></p>
                                                    </div>
                                                    <div class="col-right-no-margin">
                                                        <img src="<%=Page.ResolveClientUrl("~/Images/Icons/address_book.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/address_book_hover.png")%>';"
                                                            onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/address_book.png")%>';"
                                                            alt="Seleziona un corrispondente nella rubrica" title="Seleziona un corrispondente nella rubrica"
                                                            id="Img46" class="clickable" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="full_width">
                                                        <cc1:CustomTextArea ID="CustomTextArea7" runat="server" Width="20%" CssClass="txt_addressBook"
                                                            Text="TRGA"></cc1:CustomTextArea>
                                                        <cc1:CustomTextArea ID="CustomTextArea8" runat="server" Width="74%" CssClass="txt_addressBook"
                                                            Text="Tribunale Regionale di Giustizia Amministrativa"></cc1:CustomTextArea>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">Esito</span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <div class="styled-select_full">
                                                            <asp:DropDownList ID="DropDownList5" runat="server">
                                                                <asp:ListItem Text="Favorevole"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <p>
                                                            <span class="weight">Spese</span></p>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col">
                                                        <div class="styled-select_full">
                                                            <asp:DropDownList ID="DropDownList6" runat="server">
                                                                <asp:ListItem Text="Spese compensate"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </asp:Panel>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </fieldset>
                            </div>
                        </div>
                    </div>
                    <div id="contentDx">
                        <asp:UpdatePanel runat="server" ID="UpPnlDocumentData" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="col">
                                    <div id="contentDxSx">
                                        <asp:Panel runat="server" Visible="false" ID="PnlDocumentData">
                                            <div class="row">
                                                <fieldset>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">Data</span>: 31/02/2012
                                                        </div>
                                                        <div class="col">
                                                            <span class="weight">Dimensione</span>: 145 kb
                                                        </div>
                                                        <div class="col">
                                                            <span class="weight">Cartaceo</span>: Si
                                                        </div>
                                                        <div class="cols">
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/lock.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/lock_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/lock.png")%>';"
                                                                alt="Documento bloccato" title="Documento bloccato" id="Img29" class="clickableLeft" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/flag_ok.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/flag_ok_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/flag_ok.png")%>';"
                                                                alt="Verifica Crl firma" title="Verifica Crl firma" id="Img30" class="clickableLeft" />
                                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/timestamp.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/timestamp_hover.png")%>';"
                                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/timestamp.png")%>';"
                                                                alt="Apri lista timestamp del documento" title="Apri lista timestamp del documento"
                                                                id="Img31" class="clickableLeft" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col">
                                                            <span class="weight">Nota di versione</span>: Documento convertito in PDF
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <iframe src="../Images/documento.pdf" width="100%" frameborder="0" marginheight="0"
                                                            marginwidth="0" id="frame"></iframe>
                                                        <script type="text/javascript">
                                                            function resizeIframe() {
                                                                var height = document.documentElement.clientHeight; height
    -= document.getElementById('frame').offsetTop; // not sure how to get this dynamically
                                                                height -= 325; /* whatever you set your body bottom margin/padding to be */document.getElementById('frame').style.height
    = height + "px";
                                                            }; document.getElementById('frame').onload = resizeIframe; window.onresize
    = resizeIframe; initDate(); </script>
                                                    </div>
                                                </fieldset>
                                            </div>
                                            <div class="row2">
                                                <div class="col2">
                                                    <span class="weight">Versioni</span>:
                                                </div>
                                                <div class="col">
                                                    <ul>
                                                        <li><a href="#"><strong>V10</strong></a> |</li>
                                                        <li><a href="#">V9</a> |</li>
                                                        <li><a href="#">V8</a> |</li>
                                                        <li><a href="#">V7</a> |</li>
                                                        <li><a href="#">V6</a> |</li>
                                                        <li><a href="#">V5</a> |</li>
                                                        <li><a href="#">V4</a> |</li>
                                                        <li><a href="#">V3</a> |</li>
                                                        <li><a href="#">V2</a> |</li>
                                                        <li><a href="#">V1</a></li>
                                                    </ul>
                                                </div>
                                                <div class="col-right-no-margin">
                                                    <img src="<%=Page.ResolveClientUrl("~/Images/Icons/add_version.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/add_version_hover.png")%>';"
                                                        onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/add_version.png")%>';"
                                                        alt="Aggiungi versione al documento" title="Aggiungi versione al documento" id="Img37"
                                                        class="clickableLeft" />
                                                    <img src="<%=Page.ResolveClientUrl("~/Images/Icons/edit_verion.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/edit_verion_hover.png")%>';"
                                                        onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/edit_verion.png")%>';"
                                                        alt="Modifica dati versione del documento" title="Modifica dati versione del documento"
                                                        id="Img38" class="clickableLeft" />
                                                    <img src="<%=Page.ResolveClientUrl("~/Images/Icons/delete.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/delete_hover.png")%>';"
                                                        onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/delete.png")%>';"
                                                        alt="Cancella versione del documento" title="Cancella versione del documento"
                                                        id="Img39" class="clickableLeft" />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <asp:Panel runat="server" ID="PnlDocumentNotAcuired">
                                            <div id="prova2">
                                                <fieldset>
                                                    <p>
                                                        Documento Acquisito</p>
                                                    <h5>
                                                        <img src="../Images/Documents/document_no_acquired.png" alt="" /></h5>
                                                    <h6>
                                                        <asp:LinkButton Text="Clicca per visualizzare il documento" ID="LinkFileDocument"
                                                            runat="server" OnClick="LinkViewFileDocument"></asp:LinkButton></h6>
                                                </fieldset>
                                            </div>
                                        </asp:Panel>
                                    </div>
                                    <div id="contentDxDx">
                                        <asp:Panel runat="server" ID="PnlDocumentAttached" Visible="false">
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_pdf.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_pdf_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_pdf.png")%>';"
                                                alt="Provvedimento disciplinare Giuseppe Verdi" title="Provvedimento disciplinare Giuseppe Verdi"
                                                id="Img32" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>';"
                                                alt="Allegato 1" title="Allegato 1" id="Img33" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>';"
                                                alt="Allegato 2" title="Allegato 2" id="Img34" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>';"
                                                alt="Allegato 3" title="Allegato 3" id="Img60" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>';"
                                                alt="Allegato 4" title="Allegato 4" id="Img61" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>';"
                                                alt="Allegato 5" title="Allegato 5" id="Img62" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>';"
                                                alt="Allegato 6" title="Allegato 6" id="Img63" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>';"
                                                alt="Allegato 7" title="Allegato 7" id="Img64" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>';"
                                                alt="Allegato 8" title="Allegato 8" id="Img65" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_attached.png")%>';"
                                                alt="Allegato 9" title="Allegato 9" id="Img66" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_pec.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_pec_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_pec.png")%>';"
                                                alt="Allegato Pec" title="Allegato Pec" id="Img67" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_pec.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_pec_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_pec.png")%>';"
                                                alt="Allegato Pec" title="Allegato Pec" id="Img68" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_pec.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_pec_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_pec.png")%>';"
                                                alt="Allegato Pec" title="Allegato Pec" id="Img35" class="clickableLeft" />
                                            <img src="<%=Page.ResolveClientUrl("~/Images/Icons/ico_pitre.png")%>" onmouseover="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_pitre_hover.png")%>';"
                                                onmouseout="this.src='<%=Page.ResolveClientUrl("~/Images/Icons/ico_pitre.png")%>';"
                                                alt="Allegato IS" title="Allegato IS" id="Img36" class="clickableLeft" />
                                        </asp:Panel>
                                    </div>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <script type="text/javascript">
                $(".chzn-select-deselect").chosen({ allow_single_deselect: true, no_results_text: "Nessun risultato" });
            </script>
        </ContentTemplate>
    </asp:UpdatePanel>
    <!-- end of container -->
</asp:Content>
<asp:Content ID="ContentButtons" ContentPlaceHolderID="ContentPlaceOldersButtons"
    runat="server">
    <asp:UpdatePanel runat="server" ID="UpDocumentButtons" UpdateMode="Conditional">
        <ContentTemplate>
            <cc1:CustomButton ID="DocumentBtnSave" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Salva" />
            <cc1:CustomButton ID="DocumentBntRecord" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Protocolla" OnClick="DocumentBntRecord_Click" />
            <cc1:CustomButton ID="DocumentBtnRepeat" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Riproponi" />
            <cc1:CustomButton ID="DocumentBtnSend" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Spedisci" />
            <cc1:CustomButton ID="DocumentBtnTransmit" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Trasmetti" />
            <cc1:CustomButton ID="DocumentBtnAdL" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="AdL" />
            <cc1:CustomButton ID="DocumentBtnPrepared" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Predisponi" />
            <cc1:CustomButton ID="DocumentBtnPrint" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Stampa" />
            <cc1:CustomButton ID="DocumentBtnRemove" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Rimuovi" />
            <cc1:CustomButton ID="DocumentBtnUndo" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" Text="Annulla" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
