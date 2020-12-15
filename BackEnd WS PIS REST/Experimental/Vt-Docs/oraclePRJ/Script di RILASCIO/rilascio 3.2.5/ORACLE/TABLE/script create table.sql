create table DPA_LIC
(SYSTEM_ID		NUMBER,
COD_APP			VARCHAR(200),
VAR_CONTENT		CLOB
)
;

INSERT INTO DPA_LIC
(SYSTEM_ID,COD_APP, VAR_CONTENT)
VALUES
(seq.NEXTVAL,
'ASPOSE',
'<License><Data><LicensedTo>NTTData Italia</LicensedTo><EmailTo>roberto.bresciani@nttdata.com</EmailTo><LicenseType>Developer OEM</LicenseType><LicenseNote>Limited to 1 developer, unlimited physical locations</LicenseNote><OrderID>160914101103</OrderID><UserID>48528</UserID><OEM>This is a redistributable license</OEM><Products><Product>Aspose.Total for .NET</Product></Products><EditionType>Enterprise</EditionType><SerialNumber>2aad30f8-4ac2-493b-ae51-572bc1a96820</SerialNumber><SubscriptionExpiry>20171101</SubscriptionExpiry><LicenseVersion>3.0</LicenseVersion><LicenseInstructions>http://www.aspose.com/corporate/purchase/license-instructions.aspx</LicenseInstructions></Data><Signature>Na+lOX38Zfw8kwUC779jeFZEH/XbZMNEqY7rZStQQikMwT0ZLia9x9MKRQ1ZJtEDKCSJigtxjYoNd/a7F0D4cq+YDfyy861g4HuVr+92UeaV4RI3NqC2D4o2djruLUjxHpZLUmXO4wbqrmFPFlNfMbkzXQ9Blu/4n4/MDjPgsG8=</Signature></License>')



create table DPA_STATI_RAGIONI_TRASM
(
ID_STATO 			NUMBER NOT NULL,
ID_RAGIONE_TRASM	NUMBER NOT NULL
)
;

ALTER TABLE DPA_SCHEMA_PROCESSO_FIRMA
ADD (CHA_MODELLO CHAR(1) DEFAULT '0' );