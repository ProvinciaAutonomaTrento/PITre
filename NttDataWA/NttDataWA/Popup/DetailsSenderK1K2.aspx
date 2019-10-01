<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Popup.Master" AutoEventWireup="true"
    CodeBehind="DetailsSenderK1K2.aspx.cs" Inherits="NttDataWA.Popup.DetailsSenderK1K2" %>

<%@ Register Assembly="NttDatalLibrary" Namespace="NttDatalLibrary" TagPrefix="cc1" %>
<%@ Register Src="~/UserControls/messager.ascx" TagPrefix="uc" TagName="messager" %>
<%@ Import Namespace="NttDataWA.DocsPaWR" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script src="<%=Page.ResolveClientUrl("~/Scripts/expand.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        function SetSingleRadioButton(nameregex, current) {
          disallowOp('Content2');
          for (i = 0; i < document.forms[0].elements.length; i++) 
          {
                elm = document.forms[0].elements[i];
                if (elm.type == 'radio') {
                    if (elm != current) 
                    {
                        elm.checked = false;
                    }
                }
          }
          current.checked = true;
    }
    </script>
    <style type="text/css">
        .container
        {
            width: 95%;
            margin: 0 auto;
        }
        p
        {
            text-align: left;
        }
        em
        {
            font-style: normal;
            font-weight: bold;
            color: #9D9D9D;
        }
        .col-label
        {
            width: 100px;
            overflow: hidden;
            padding: 5px 0 0 0;
        }
        div div.col-label:last-child
        {
            color: #f00;
        }
        .txt_input, .txt_input_disabled, .txt_textarea, .txt_textarea_disabled
        {
            width: 472px;
            height: 1.5em;
        }
        .txt_textarea, .txt_textarea_disabled
        {
            height: 3em;
            vertical-align: top;
        }
        .txt_input_disabled
        {
            background-color: #ffffff;
        }
        .txt_input_small
        {
            width: 168px;
        }
        .tbl tr.NormalRow:hover td
        {
            background-color: #fff;
        }
        .tbl tr.AltRow:hover td
        {
            background-color: #e1e9f0;
        }
        
        .correspondent fieldset
        {
            border: 1px solid #ccc;
            margin-top:10px;
            margin-bottom:10px;
            height:auto;
            vertical-align:middle;
            border-radius: 8px;
            -ms-border-radius: 8px; /* ie */
            -moz-border-radius: 8px; /* firefox */
            -webkit-border-radius: 8px; /* safari, chrome */
            -o-border-radius: 8px; /* opera */
        }
        
        .detailsCorrespondent fieldset
        {
            border: 1px solid #ccc;
            margin-top:10px;
            margin-bottom:10px;
            vertical-align:middle;
            border-radius: 8px;
            -ms-border-radius: 8px; /* ie */
            -moz-border-radius: 8px; /* firefox */
            -webkit-border-radius: 8px; /* safari, chrome */
            -o-border-radius: 8px; /* opera */ 
        }
        
        a:link
        {
            color: #0f64a1;
            text-decoration: underline;
        }

        a:visited
        {
            color: #0f64a1;
            text-decoration: underline;
        }


        a:hover
        {
            color: #0f64a1;
            text-decoration: underline;
            background-color: #FFFFFF;
        }
        
        .detailsCorrespondent
        {
            margin-left:5px;
        }
        
        .expand, .collapse, .noexpand
        {
            border: 0;
            margin: 5px 0;
        }
        
        .expand a:link, .expand a:visited
        {
            font-weight:normal;
            font-size:0.8em;
            background-image: url(../Images/Icons/collapsed.png);
            background-repeat: no-repeat;
            background-position: 0px;
            background-color: transparent;
            color:#044c7f;
            text-decoration:underline;
            font-weight:normal;
        }
        .expand a:active
        {
            background-color: transparent;
        }
        .expand a:hover, .expand a:active, .expand a:focus
        {
            text-decoration:underline;
            background-color: transparent;
        }
        .expand a.open:link, .expand a.open:visited
        {
            /*background: url(../Images/Icons/expanded.png) no-repeat 10px 10px;*/
        }

        .noexpand
        {
            text-decoration:underline;
            cursor: pointer;
            padding: 10px 10px 10px 30px;
            background: url(../Images/Icons/noexpand.png) no-repeat 10px 10px;
        }

        
    </style>
    <script src="../Scripts/chosen.jquery.min.js" type="text/javascript"></script>
    <link href="../Css/chosen.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderContent" runat="server">
    <div class="container">
        <uc:messager id="DetailsSenderK1K2Lt" runat="server" />    
        <asp:UpdatePanel ID="UpdateRepListCorrespond" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                <ContentTemplate>
        <div class="row " style = " margin-bottom: 25px">
            <cc1:CustomImageButton ID="btnViewFileDocument" runat="server" CssClass="clickableLeftN" ImageAlign ="Right" 
             ToolTip='<%$ localizeByText:IndexBtnTypeDocTooltip%>' OnClick="btnViewFileDocument_Onclick" />
        </div>
        <div id="listCorrespondent" runat="server" style="float:left; width:40%;">
                <asp:Repeater ID="repListCorrespond" runat="server" OnItemDataBound="repListCorrespond_Bound" >         
                    <ItemTemplate>
                         <div class="correspondent">
                            <fieldset>
                                <span class="weight">
                                    <div style=" float:left; width:80%">
                                        <asp:RadioButton runat="server" ID="rd" OnCheckedChanged="rbCorrespondent_OnCheckedChanged" 
                                         OnClientClick="disallowOp('Content2');" AutoPostBack="true"/>
                                        <asp:Label ID="lblDescriprionCorrespondent" runat="server" Text='<%# Bind("descrizione") %>'></asp:Label>
                                        <asp:Label ID="lblCodRubricaCorrespondent" runat="server" Text=' <%# Bind("codiceRubrica") %>'></asp:Label>
                                        <asp:Label ID="lblIdRegisterCorrespondent" runat="server" Text=' <%# Bind("idRegistro") %>'></asp:Label>
                                    </div>
                                    <div style=" margin-left:30px; float:left">
                                    <cc1:CustomImageButton ID="btnAddCorrespondent" runat="server" Enabled="False" OnClick="btnAddCorrespondent_Click"
                                             CssClass="clickableLeft" ImageUrl="~/Images/Icons/add_version.png" ImageUrlDisabled="../Images/Icons/add_version_disabled.png"
                                              OnMouseOutImage="../Images/Icons/add_version.png" OnMouseOverImage="../Images/Icons/add_version_hover.png" 
                                             OnClientClick="disallowOp('Content2');" ToolTip='<%$ localizeByText:DetailsSenderK1K2AddCorrespondent%>'
                                             />
                                             </div>
                                    <div style=" margin:0; padding-left:10px;">
                                    <cc1:CustomImageButton ID="btnDeleteCorrespondent" runat="server" Enabled="False" OnClick="BtnDelete_Click"
                                             CssClass="clickableLeft" ImageUrl="../Images/Icons/delete.png" ImageUrlDisabled="../Images/Icons/delete_disabled.png"
                                              OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png" 
                                             OnClientClick="disallowOp('Content2');" ToolTip='<%$ localizeByText:DetailsSenderK1K2RemoveCorrespondent%>'
                                             />
                                    </div>
                                    <asp:HiddenField ID="systemIdCorrespondent" runat="server" Value='<%# Bind("systemId") %>'/>
                                </span>
                            </fieldset>
                         </div>
                    </ItemTemplate>                   
                </asp:Repeater>
        </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="detailsCorrespondent" class="detailsCorrespondent" runat="server">
           <fieldset>
           <asp:UpdatePanel ID="UpdatePanelDetailsCorrespondentRole" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                    <ContentTemplate>
                    <asp:PlaceHolder ID="PlaceHolderAddCorrespondent" runat="server" Visible="false">
                        <span class="weight">
                                    <asp:Literal ID="DetailsSenderK1K2AddCorrespondentLbl" Text="Crea nuovo corrispondente" runat="server"></asp:Literal></span><span class="little"></span></p>       
                    </asp:PlaceHolder>
                 <asp:PlaceHolder ID="pnlRuolo" runat="server">
                 
                    <div class="row">
                        <div class="col col-label">
                            <asp:Literal ID="lbl_CodR" runat="server" /></div>
                        <div class="col-no-margin">
                            <cc1:CustomTextArea ID="txt_CodR" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                                OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col col-label">
                            <asp:Literal ID="lbl_DescR" runat="server" />*</div>
                        <div class="col-no-margin">
                            <cc1:CustomTextArea ID="txt_DescR" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                                OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                        </div>
                    </div>
                    
                </asp:PlaceHolder>
                </ContentTemplate>
                    </asp:UpdatePanel>
                 <asp:PlaceHolder ID="pnlStandard" runat="server">
            <asp:UpdatePanel ID="UpdatePanelDetailsCorrespondent" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanelTypeCorr" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                <ContentTemplate>
           <asp:PlaceHolder ID="pnlInStandard1" runat="server">
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_CodRubrica" runat="server" /><asp:Label ID="starRubrica" runat="server">*</asp:Label></div>
                <div class="col-no-margin">
                    <cc1:CustomTextArea ID="txt_CodRubrica" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                        OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                </div>
            </div>
            <asp:PlaceHolder ID="pnl_descrizione" runat="server">
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_descrizione" runat="server" />*</div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_descrizione" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled"
                            OnTextChanged="DataChangedHandler"  AutoPostBack="True" TextMode="MultiLine" />
                    </div>
                </div>
            </asp:PlaceHolder>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="pnl_nome_cogn" runat="server">
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_nome" runat="server" />*</div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_nome" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                            OnTextChanged="DataChangedHandler" AutoPostBack="True" />
                    </div>
                </div>
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_cognome" runat="server" />*</div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_cognome" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                            OnTextChanged="DataChangedHandler"  AutoPostBack="True"/>
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_tipocorr" runat="server" /></div>
                <div class="col-no-margin">
                    <asp:DropDownList ID="ddl_tipoCorr" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect" Width="200px"
                     OnSelectedIndexChanged="ddl_tipoCorr_SelectedIndexChanged" />
                </div>
            </div>
            </ContentTemplate>
                </asp:UpdatePanel>
            <div class="row">

            <asp:UpdatePanel ID="UpPanelOtherDetails" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                <ContentTemplate>

                <h2 class="expand"><asp:Literal ID="viewOtherDetailsLink" runat="server" /></h2>

            <div class="collapse" id="collapse">
            <asp:UpdatePanel ID="UpdatePanelDetails" runat="server" UpdateMode="Conditional" ClientIDMode="static">
                <ContentTemplate>
            <div id="otherDetails" runat="server">
                   <div class="row">
                    <asp:Literal ID="lbl_nomeCorr" runat="server" Visible="false" />
                    </div>
                         <asp:PlaceHolder ID="pnl_registro" runat="server" Visible="false">
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_registro" runat="server" /></div>
                <div class="col-no-margin">
                    <asp:Literal ID="lit_registro" runat="server" />
                    <asp:DropDownList ID="ddl_registri" runat="server" AutoPostBack="true" CssClass="chzn-select-deselect" width="300px"  />
                </div>
            </div>
        </asp:PlaceHolder>
                         <asp:PlaceHolder ID="pnl_email" runat="server">
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_codAOO" runat="server" /><asp:Literal ID="starCodAOO" runat="server"
                        Visible="false">*</asp:Literal></div>
                <div class="col-no-margin">
                    <cc1:CustomTextArea ID="txt_codAOO" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                        OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                </div>
            </div>
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_codAmm" runat="server" /><asp:Literal ID="starCodAmm" runat="server"
                        Visible="false">*</asp:Literal></div>
                <div class="col-no-margin">
                    <cc1:CustomTextArea ID="txt_codAmm" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                        OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcSingleMail" runat="server" EnableTheming="false">
                <asp:UpdatePanel ID="updPanel1" runat="server" UpdateMode="Always" Visible="true" ClientIDMode="Static">
                    <ContentTemplate>
                        <div class="row">
                            <div class="col col-label">
                                <asp:Literal ID="lbl_email" runat="server" /><asp:Literal ID="starEmail" runat="server">*</asp:Literal></div>
                            <div class="col-no-margin">
                                <cc1:CustomTextArea ID="txt_email" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                                    OnTextChanged="DataChangedHandler"  />
                                <cc1:CustomTextArea ID="txtCasella" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                                    Style="width: 400px;" />
                                <cc1:CustomImageButton runat="server" ClientIDMode="static" ID="imgAggiungiCasella" ImageUrl="../Images/Icons/add_version.png"
                                    OnMouseOutImage="../Images/Icons/add_version.png" OnMouseOverImage="../Images/Icons/add_version_hover.png"
                                    CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/add_version_disabled.png"
                                    OnClick="imgAggiungiCasella_Click" />
                            </div>
                        </div>
                        <asp:PlaceHolder ID="plcNoteMail" runat="server">
                            <div class="row">
                                <div class="col col-label">
                                    <asp:Literal ID="lblNote" runat="server" /></div>
                                <div class="col-no-margin">
                                    <cc1:CustomTextArea ID="txtNote" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled" TextMode="MultiLine" />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:PlaceHolder>
            <asp:UpdatePanel ID="updPanelMail" runat="server" UpdateMode="Always" Visible="true" ClientIDMode="Static">
                <ContentTemplate>
                    <div class="row">
                        <div class="col-full">
                        <div style="margin-top:30px;">
                            <asp:GridView ID="gvCaselle" runat="server" AutoGenerateColumns="False" OnRowDataBound="gvCaselle_RowDataBound"
                                CellPadding="1" BorderWidth="1px" BorderColor="Gray" CssClass="tbl" EnableViewState="true">
                                <RowStyle CssClass="NormalRow"/>
                                <AlternatingRowStyle CssClass="AltRow" />
                                <Columns>
                                    <asp:TemplateField HeaderText="SystemId" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblSystemId" Text='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).systemId %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Email" ShowHeader="true">
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="68%"></ItemStyle>
                                        <ItemTemplate>
                                            <cc1:CustomTextArea AutoPostBack="true" Width="310px" OnTextChanged="txtEmailCorr_TextChanged"
                                                ID="txtEmailCorr" runat="server" ToolTip='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Email %>'
                                                CssClass="txt_input" CssClassReadOnly="txt_input_disabled" Text='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Email %>'></cc1:CustomTextArea>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Note E-mail" ShowHeader="true">
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="28%"></ItemStyle>
                                        <ItemTemplate>
                                            <cc1:CustomTextArea AutoPostBack="true" Width="110px" MaxLength="20" OnTextChanged="txtNoteMailCorr_TextChanged"
                                                ID="txtNoteMailCorr" runat="server" ToolTip='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Note %>'
                                                CssClass="txt_input" CssClassReadOnly="txt_input_disabled" Text='<%# ((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Note %>'></cc1:CustomTextArea>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="*" ShowHeader="true">
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%"></ItemStyle>
                                        <ItemTemplate>
                                            <asp:RadioButton ID="rdbPrincipale" runat="server" AutoPostBack="true" OnCheckedChanged="rdbPrincipale_ChecekdChanged"
                                                Checked='<%# TypeMailCorrEsterno(((NttDataWA.DocsPaWR.MailCorrispondente)Container.DataItem).Principale) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="" ShowHeader="true">
                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%"></ItemStyle>
                                        <ItemTemplate>
                                            <cc1:CustomImageButton runat="server" ID="imgEliminaCasella" ImageUrl="../Images/Icons/delete.png"
                                                OnMouseOutImage="../Images/Icons/delete.png" OnMouseOverImage="../Images/Icons/delete_hover.png"
                                                CssClass="clickableLeft" ImageUrlDisabled="../Images/Icons/delete.png" OnClick="imgEliminaCasella_Click"  AlternateText='<%# GetLabelDelete() %>' ToolTip='<%# GetLabelDelete() %>'
                                                AutoPostBack="true" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:PlaceHolder ID="plcPreferredChannel" runat="server">
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_preferredChannel" runat="server" /></div>
                    <div class="col-no-margin">
                        <asp:DropDownList ID="dd_canpref" runat="server" AutoPostBack="true" OnSelectedIndexChanged="dd_canpref_SelectedIndexChanged"
                            CssClass="chzn-select-deselect" Width="200px" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="panelInStandard2" runat="server">
                    <div class="row">
                <p style="border-bottom: 1px solid #2a7bbd;">
                    &nbsp;</p>
            </div>
            <asp:PlaceHolder ID="pnl_titolo" runat="server">
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_titolo" runat="server" /></div>
                    <div class="col-no-margin">
                        <asp:DropDownList ID="ddl_titolo" runat="server" CssClass="chzn-select-deselect"
                            Width="200px" OnSelectedIndexChanged="DataChangedHandler"  AutoPostBack="True"/>
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="pnl_infonascita" runat="server">
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_luogonascita" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_luogoNascita" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                            OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                    </div>
                </div>
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_dataNascita" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_dataNascita" runat="server" CssClass="txt_textdata datepicker"
                            CssClassReadOnly="txt_textdata_disabled" Columns="10" OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="pnl_indirizzo" runat="server">
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_indirizzo" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_indirizzo" runat="server" CssClass="txt_input" CssClassReadOnly="txt_input_disabled"
                            OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                    </div>
                </div>
               
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_citta" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_citta" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                    </div>
                    <div class="col col-label right">
                        <asp:Literal ID="lbl_provincia" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_provincia" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="2" OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                    </div>
                </div>
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_local" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_local" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="128" OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                    </div>
                    <div class="col col-label right">
                        <asp:Literal ID="lbl_nazione" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_nazione" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" OnTextChanged="DataChangedHandler" AutoPostBack="True" />
                    </div>
                </div>
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_telefono" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_telefono" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" OnTextChanged="DataChangedHandler" AutoPostBack="True" />
                    </div>
                    <div class="col col-label right">
                        <asp:Literal ID="lbl_telefono2" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_telefono2" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                    </div>
                </div>
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_fax" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_fax" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                    </div>
                     <div class="col col-label right">
                        <asp:Literal ID="lbl_cap" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_cap" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" OnTextChanged="DataChangedHandler"  AutoPostBack="True" MaxLength="5" />
                    </div>
                </div>
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_codfisc" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_codfisc" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="16" OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                    </div>
                    <div class="col col-label right">
                        <asp:Literal ID="lbl_partita_iva" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_partita_iva" runat="server" CssClass="txt_input txt_input_small"
                            CssClassReadOnly="txt_input_disabled txt_input_small" MaxLength="11" OnTextChanged="DataChangedHandler"  AutoPostBack="True" />
                    </div>
                </div>
                <div class="row">
                    <div class="col col-label">
                        <asp:Literal ID="lbl_note" runat="server" /></div>
                    <div class="col-no-margin">
                        <cc1:CustomTextArea ID="txt_note" runat="server" CssClass="txt_textarea" CssClassReadOnly="txt_textarea_disabled"
                            OnTextChanged="DataChangedHandler"  AutoPostBack="True" TextMode="MultiLine" />
                    </div>
                </div>
            </asp:PlaceHolder>
            </asp:PlaceHolder>
            </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            </ContentTemplate>
            </asp:UpdatePanel>
        </asp:PlaceHolder>
            </fieldset>
        </div>
        <asp:PlaceHolder ID="pnlRuoliUtente" runat="server" Visible="false">
            <div class="row">
                <div class="col col-label">
                    <asp:Literal ID="lbl_Ruoli" runat="server" /></div>
                <div class="col col-no-margin">
                    <asp:Literal ID="lblRuoli" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="PanelListaCorrispondenti" runat="server">
            <div class="row">
                <p class="center">
                    <em>
                        <asp:Literal ID="lbl_nomeLista" runat="server" /></em>
                </p>
            </div>
            <div class="row">
                <asp:GridView ID="dg_listCorr" runat="server" Width="600" AutoGenerateColumns="False"
                    CssClass="tbl">
                    <RowStyle CssClass="NormalRow" />
                    <AlternatingRowStyle CssClass="AltRow" />
                    <Columns>
                        <asp:BoundField DataField="CODICE" ItemStyle-Width="30%" />
                        <asp:BoundField DataField="DESCRIZIONE" ItemStyle-Width="70%" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceOldersButtons" runat="server">
    <asp:UpdatePanel runat="server" ID="UpButtons" UpdateMode="Conditional" ClientIDMode="Static">
        <ContentTemplate>
            <cc1:CustomButton ID="BtnRecord" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="BtnRecord_Click" />
            <cc1:CustomButton ID="BtnSaveOccasionalAndRecord" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="BtnRecord_Click" Visible="false" />
            <cc1:CustomButton ID="BtnClose" runat="server" CssClass="btnEnable" CssClassDisabled="btnDisable"
                OnMouseOver="btnHover" ClientIDMode="Static" OnClientClick="disallowOp('Content2');"
                OnClick="BtnClose_Click" />
            <asp:HiddenField ID="proceed_delete" runat="server" ClientIDMode="Static" Value="false" />
        </ContentTemplate>
    </asp:UpdatePanel>
        <script type="text/javascript">
            $(function () {
                // --- Using the default options:
                $("h2.expand").toggler({ initShow: "div.shown" });
            });
    </script>
</asp:Content>
