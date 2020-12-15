using System;

namespace DocsPaAS400
{
	/// <summary>
	/// Summary description for Constants.
	/// </summary>
	public class Constants
	{
		//numero righe da inserire
		public static int NUM_CODES=21;

		//nomi dei campi
		public static string JOUREC_FIELD_NAME="JOUREC";
		public static string JOUDOC_FIELD_NAME="JOUDOC";
		public static string JOUCOD_FIELD_NAME="JOUCOD";
		public static string JOUPRO_FIELD_NAME="JOUPRO";
		public static string JOUTYP_FIELD_NAME="JOUTYP";
		public static string JOULEN_FIELD_NAME="JOULEN";
		public static string JOUDES_FIELD_NAME="JOUDES";
		public static string KEY_FIELD_NAME="LKEY";
		public static string FLAG_FIELD_NAME="FLAG";
		public static string TS_AS400_FIELD_NAME="TS_AS400";
		public static string TS_DOCSPA_FIELD_NAME="TS_DOCSPA";
	
	    //nome delle tabelle
        public static string TEWDOCSPA_TABLE_NAME="TEWDOCSPA";
		public static string LOGDOCSPA_TABLE_NAME="LOGDOCSPA";
		public static string TEWAS400_TABLE_NAME="TEWAS400";
		public static string LOGAS400_TABLE_NAME="LOGAS400";

        //valori di JOUTYP e JOULEN
		public static int[] JOULEN_FIELD_VALUES={0,7,9,5,1,5,1,1,5,5,5,37,16,9,28,30,50,117,10,4,1,2};
        public static string[] JOUTYP_FIELD_VALUES={"","Z","Z","Z","A","A","A","A","Z","Z","A","A","A","Z","A","A","A","A","A","A","A","A"};

		//varie
		public static string INSERT_DATE_FORMAT="yyyyMMdd";
		public static string INSERT_YEAR_FORMAT="yyyy";
		public static string TIMESTAMP_FORMAT="yyyyMMddhhmmss";
		public static string OPERATOR_INIT="ZZ";
		public static string AS400_CONNECTION_STRING_PARAM_NAME="AS400ConnectionString";
		public static int OBJECT_ROW_LENGTH=255;
		public static string JOUCOD_OGGETTO_VALUE="10";
		public static string JOUCOD_PROTO_VALUE="1";
		public static string JOUCOD_COMM_REF_VALUE="55";

		//operazioni 
		public static string CREATE_MODIFY_OPERATION="2";
		public static string DELETE_OPERATION="3";
	}
}
