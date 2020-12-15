﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.6407
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

// 
// This source code was auto-generated by wsdl, Version=2.0.50727.3038.
// 


/// <remarks/>
// CODEGEN: The optional WSDL extension element 'Policy' from namespace 'http://schemas.xmlsoap.org/ws/2004/09/policy' was not handled.
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name="VerificationServiceSOAPBinding", Namespace="http://actalisVol/")]
public partial class VerificationServiceSOAPBindingQSService : System.Web.Services.Protocols.SoapHttpClientProtocol {
    
    private System.Threading.SendOrPostCallback VerificationOperationCompleted;
    
    /// <remarks/>
    public VerificationServiceSOAPBindingQSService() {
        this.Url = "http://cdcrm0-vm0098.corteconti.it:8001/FirmaDigitale/VerificaFirmaDigitale_PX";
    }
    
    /// <remarks/>
    public event VerificationCompletedEventHandler VerificationCompleted;
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://actalisVol/", ResponseNamespace="http://actalisVol/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    [return: System.Xml.Serialization.XmlElementAttribute("Return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public Return Verification([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] fileInfo fileInfo, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary")] byte[] fileContent, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] System.DateTime verifAllaData) {
        object[] results = this.Invoke("Verification", new object[] {
                    fileInfo,
                    fileContent,
                    verifAllaData});
        return ((Return)(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginVerification(fileInfo fileInfo, byte[] fileContent, System.DateTime verifAllaData, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("Verification", new object[] {
                    fileInfo,
                    fileContent,
                    verifAllaData}, callback, asyncState);
    }
    
    /// <remarks/>
    public Return EndVerification(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((Return)(results[0]));
    }
    
    /// <remarks/>
    public void VerificationAsync(fileInfo fileInfo, byte[] fileContent, System.DateTime verifAllaData) {
        this.VerificationAsync(fileInfo, fileContent, verifAllaData, null);
    }
    
    /// <remarks/>
    public void VerificationAsync(fileInfo fileInfo, byte[] fileContent, System.DateTime verifAllaData, object userState) {
        if ((this.VerificationOperationCompleted == null)) {
            this.VerificationOperationCompleted = new System.Threading.SendOrPostCallback(this.OnVerificationOperationCompleted);
        }
        this.InvokeAsync("Verification", new object[] {
                    fileInfo,
                    fileContent,
                    verifAllaData}, this.VerificationOperationCompleted, userState);
    }
    
    private void OnVerificationOperationCompleted(object arg) {
        if ((this.VerificationCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.VerificationCompleted(this, new VerificationCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    public new void CancelAsync(object userState) {
        base.CancelAsync(userState);
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class fileInfo {
    
    private string fileNameField;
    
    private string fileExtensionField;
    
    private bool autoDetectTypeField;
    
    public fileInfo() {
        this.autoDetectTypeField = false;
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string fileName {
        get {
            return this.fileNameField;
        }
        set {
            this.fileNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string fileExtension {
        get {
            return this.fileExtensionField;
        }
        set {
            this.fileExtensionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool autoDetectType {
        get {
            return this.autoDetectTypeField;
        }
        set {
            this.autoDetectTypeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class signerTimeStamp {
    
    private bool tsSignVaildField;
    
    private bool tsSignCertV2Field;
    
    private string tsSignDigestAlgField;
    
    private delib45Valid tsSignDelib45ValidField;
    
    private bool tsDelib45ValidField;
    
    private string tsDigestMessageImprintField;
    
    private System.DateTime tsGenTimeField;
    
    private string tsSerialNumberField;
    
    private string tsPolicyOidField;
    
    private string tsTsaNameField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool tsSignVaild {
        get {
            return this.tsSignVaildField;
        }
        set {
            this.tsSignVaildField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool tsSignCertV2 {
        get {
            return this.tsSignCertV2Field;
        }
        set {
            this.tsSignCertV2Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string tsSignDigestAlg {
        get {
            return this.tsSignDigestAlgField;
        }
        set {
            this.tsSignDigestAlgField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public delib45Valid tsSignDelib45Valid {
        get {
            return this.tsSignDelib45ValidField;
        }
        set {
            this.tsSignDelib45ValidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool tsDelib45Valid {
        get {
            return this.tsDelib45ValidField;
        }
        set {
            this.tsDelib45ValidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string tsDigestMessageImprint {
        get {
            return this.tsDigestMessageImprintField;
        }
        set {
            this.tsDigestMessageImprintField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public System.DateTime tsGenTime {
        get {
            return this.tsGenTimeField;
        }
        set {
            this.tsGenTimeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string tsSerialNumber {
        get {
            return this.tsSerialNumberField;
        }
        set {
            this.tsSerialNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string tsPolicyOid {
        get {
            return this.tsPolicyOidField;
        }
        set {
            this.tsPolicyOidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string tsTsaName {
        get {
            return this.tsTsaNameField;
        }
        set {
            this.tsTsaNameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class delib45Valid {
    
    private bool warnDelib45Field;
    
    private string warnDelib45CauseField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool warnDelib45 {
        get {
            return this.warnDelib45Field;
        }
        set {
            this.warnDelib45Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string warnDelib45Cause {
        get {
            return this.warnDelib45CauseField;
        }
        set {
            this.warnDelib45CauseField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class crlInfo {
    
    private bool crlStatusField;
    
    private string clStatusInfoField;
    
    private System.DateTime crlThisUpdateField;
    
    private System.DateTime crlNextUpdateField;
    
    private string serialField;
    
    private System.DateTime expiredCertsOnCrlField;
    
    private bool expiredCertsOnCrlFieldSpecified;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool crlStatus {
        get {
            return this.crlStatusField;
        }
        set {
            this.crlStatusField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string clStatusInfo {
        get {
            return this.clStatusInfoField;
        }
        set {
            this.clStatusInfoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public System.DateTime crlThisUpdate {
        get {
            return this.crlThisUpdateField;
        }
        set {
            this.crlThisUpdateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public System.DateTime crlNextUpdate {
        get {
            return this.crlNextUpdateField;
        }
        set {
            this.crlNextUpdateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string serial {
        get {
            return this.serialField;
        }
        set {
            this.serialField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public System.DateTime expiredCertsOnCrl {
        get {
            return this.expiredCertsOnCrlField;
        }
        set {
            this.expiredCertsOnCrlField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool expiredCertsOnCrlSpecified {
        get {
            return this.expiredCertsOnCrlFieldSpecified;
        }
        set {
            this.expiredCertsOnCrlFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class certRevocationInfo {
    
    private bool certRevokedField;
    
    private string revocationReasonField;
    
    private System.DateTime revocationDateField;
    
    private bool revocationDateFieldSpecified;
    
    private crlInfo crlInfoField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool certRevoked {
        get {
            return this.certRevokedField;
        }
        set {
            this.certRevokedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string revocationReason {
        get {
            return this.revocationReasonField;
        }
        set {
            this.revocationReasonField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public System.DateTime revocationDate {
        get {
            return this.revocationDateField;
        }
        set {
            this.revocationDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool revocationDateSpecified {
        get {
            return this.revocationDateFieldSpecified;
        }
        set {
            this.revocationDateFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public crlInfo crlInfo {
        get {
            return this.crlInfoField;
        }
        set {
            this.crlInfoField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class certPolicy {
    
    private string certCpsUriField;
    
    private string certPolTextField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string certCpsUri {
        get {
            return this.certCpsUriField;
        }
        set {
            this.certCpsUriField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string certPolText {
        get {
            return this.certPolTextField;
        }
        set {
            this.certPolTextField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class certPublicKey {
    
    private string certPublicKey1Field;
    
    private string certPublicKeyLengthField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("certPublicKey", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string certPublicKey1 {
        get {
            return this.certPublicKey1Field;
        }
        set {
            this.certPublicKey1Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="integer")]
    public string certPublicKeyLength {
        get {
            return this.certPublicKeyLengthField;
        }
        set {
            this.certPublicKeyLengthField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class certIssuer {
    
    private string issuerDistinguishNameField;
    
    private string issuerNameField;
    
    private string issuerPartitaIvaField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string issuerDistinguishName {
        get {
            return this.issuerDistinguishNameField;
        }
        set {
            this.issuerDistinguishNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string issuerName {
        get {
            return this.issuerNameField;
        }
        set {
            this.issuerNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string issuerPartitaIva {
        get {
            return this.issuerPartitaIvaField;
        }
        set {
            this.issuerPartitaIvaField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class certSubject {
    
    private string subjectDistinguishedNameField;
    
    private string commonNameField;
    
    private string codiceFiscaleField;
    
    private string roleField;
    
    private string countryField;
    
    private string organizationField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string subjectDistinguishedName {
        get {
            return this.subjectDistinguishedNameField;
        }
        set {
            this.subjectDistinguishedNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string commonName {
        get {
            return this.commonNameField;
        }
        set {
            this.commonNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string codiceFiscale {
        get {
            return this.codiceFiscaleField;
        }
        set {
            this.codiceFiscaleField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string role {
        get {
            return this.roleField;
        }
        set {
            this.roleField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string country {
        get {
            return this.countryField;
        }
        set {
            this.countryField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string organization {
        get {
            return this.organizationField;
        }
        set {
            this.organizationField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class certificate {
    
    private string certNameField;
    
    private string certVersionField;
    
    private string certSerialNoField;
    
    private bool certQualifiedField;
    
    private System.DateTime certValFromField;
    
    private System.DateTime certValUntilField;
    
    private string certKeyUsageField;
    
    private certSubject certSubjectField;
    
    private certIssuer certIssuerField;
    
    private certPublicKey certPublicKeyField;
    
    private certPolicy[] certPolicyField;
    
    private certRevocationInfo certRevocationField;
    
    private string certFinger256Field;
    
    private bool certValidField;
    
    private bool certTimeValidField;
    
    private bool certTrustedField;
    
    private string certCertField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string certName {
        get {
            return this.certNameField;
        }
        set {
            this.certNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string certVersion {
        get {
            return this.certVersionField;
        }
        set {
            this.certVersionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string certSerialNo {
        get {
            return this.certSerialNoField;
        }
        set {
            this.certSerialNoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool certQualified {
        get {
            return this.certQualifiedField;
        }
        set {
            this.certQualifiedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public System.DateTime certValFrom {
        get {
            return this.certValFromField;
        }
        set {
            this.certValFromField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public System.DateTime certValUntil {
        get {
            return this.certValUntilField;
        }
        set {
            this.certValUntilField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string certKeyUsage {
        get {
            return this.certKeyUsageField;
        }
        set {
            this.certKeyUsageField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public certSubject certSubject {
        get {
            return this.certSubjectField;
        }
        set {
            this.certSubjectField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public certIssuer certIssuer {
        get {
            return this.certIssuerField;
        }
        set {
            this.certIssuerField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public certPublicKey certPublicKey {
        get {
            return this.certPublicKeyField;
        }
        set {
            this.certPublicKeyField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("certPolicy", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public certPolicy[] certPolicy {
        get {
            return this.certPolicyField;
        }
        set {
            this.certPolicyField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public certRevocationInfo certRevocation {
        get {
            return this.certRevocationField;
        }
        set {
            this.certRevocationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string certFinger256 {
        get {
            return this.certFinger256Field;
        }
        set {
            this.certFinger256Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool certValid {
        get {
            return this.certValidField;
        }
        set {
            this.certValidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool certTimeValid {
        get {
            return this.certTimeValidField;
        }
        set {
            this.certTimeValidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool certTrusted {
        get {
            return this.certTrustedField;
        }
        set {
            this.certTrustedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string certCert {
        get {
            return this.certCertField;
        }
        set {
            this.certCertField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class signatureInfo {
    
    private bool sigValidField;
    
    private bool sigCorruptedField;
    
    private System.DateTime sigTimeField;
    
    private bool sigCertV2Field;
    
    private string sigAlgorithmField;
    
    private string sigValueField;
    
    private string sigMessageDigestField;
    
    private delib45Valid sigDelib45ValidField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool sigValid {
        get {
            return this.sigValidField;
        }
        set {
            this.sigValidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool sigCorrupted {
        get {
            return this.sigCorruptedField;
        }
        set {
            this.sigCorruptedField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public System.DateTime sigTime {
        get {
            return this.sigTimeField;
        }
        set {
            this.sigTimeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public bool sigCertV2 {
        get {
            return this.sigCertV2Field;
        }
        set {
            this.sigCertV2Field = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string sigAlgorithm {
        get {
            return this.sigAlgorithmField;
        }
        set {
            this.sigAlgorithmField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string sigValue {
        get {
            return this.sigValueField;
        }
        set {
            this.sigValueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string sigMessageDigest {
        get {
            return this.sigMessageDigestField;
        }
        set {
            this.sigMessageDigestField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public delib45Valid sigDelib45Valid {
        get {
            return this.sigDelib45ValidField;
        }
        set {
            this.sigDelib45ValidField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class signer {
    
    private signatureInfo signatureInfoField;
    
    private certificate certificateField;
    
    private signerTimeStamp timeStampField;
    
    private signer[] counterSignatureField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public signatureInfo signatureInfo {
        get {
            return this.signatureInfoField;
        }
        set {
            this.signatureInfoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public certificate certificate {
        get {
            return this.certificateField;
        }
        set {
            this.certificateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public signerTimeStamp timeStamp {
        get {
            return this.timeStampField;
        }
        set {
            this.timeStampField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("counterSignature", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public signer[] counterSignature {
        get {
            return this.counterSignatureField;
        }
        set {
            this.counterSignatureField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://actalisVol/")]
public partial class Return {
    
    private string errorField;
    
    private string operationField;
    
    private System.DateTime answerCurrentDateField;
    
    private System.DateTime verificaAllaDataField;
    
    private byte[] originalFileField;
    
    private signer[] signersField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string error {
        get {
            return this.errorField;
        }
        set {
            this.errorField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string operation {
        get {
            return this.operationField;
        }
        set {
            this.operationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public System.DateTime answerCurrentDate {
        get {
            return this.answerCurrentDateField;
        }
        set {
            this.answerCurrentDateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public System.DateTime verificaAllaData {
        get {
            return this.verificaAllaDataField;
        }
        set {
            this.verificaAllaDataField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="base64Binary")]
    public byte[] originalFile {
        get {
            return this.originalFileField;
        }
        set {
            this.originalFileField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("signers", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public signer[] signers {
        get {
            return this.signersField;
        }
        set {
            this.signersField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
public delegate void VerificationCompletedEventHandler(object sender, VerificationCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class VerificationCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal VerificationCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public Return Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((Return)(this.results[0]));
        }
    }
}
