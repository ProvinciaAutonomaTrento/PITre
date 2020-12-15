using System;

namespace StampaPDF
{
	/// <summary>
	/// Summary description for ErrorCode.
	/// </summary>
	
	public enum ErrorCode
	{
		NoError = 0,
		PathReportNotFound = 101,
		BadXmlFile = 112,
		XmlFileNotFound = 110,
		InvalidXmlFile = 111,
		InvalidValueObject = 120,
		IncompletePDFFile = 130,
		BadPDFFile = 139,
		NullPDFFile = 135,
		ErrorEventPDFFile = 137,
		GenericError = 199
	}
	
}
