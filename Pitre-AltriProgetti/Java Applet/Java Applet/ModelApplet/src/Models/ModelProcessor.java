package Models;

import java.applet.Applet;
//import java.awt.event.ActionEvent;
//import java.awt.event.ActionListener;
//import java.io.BufferedReader;
//import java.io.File;
//import java.io.FileInputStream;
//import java.io.FileReader;
//import java.io.IOException;
import java.io.InputStream;

//import javax.swing.DefaultComboBoxModel;
//import javax.swing.JButton;
//import javax.swing.JCheckBox;
//import javax.swing.JComboBox;
//import javax.swing.JLabel;
//import javax.swing.JTextField;

import com.aspose.words.License;
//import com.sun.org.apache.bcel.internal.util.ClassPath;

public class ModelProcessor extends Applet{
	
	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	public ModelProcessor(){}	
	
	//private JButton fe = null;
	//private JTextField tx = null;
	//private JLabel lbl = null;
	//private JComboBox comboBox = null; 
	//private JCheckBox chkBox = null;
	@Override 
	public void init()
	{
		System.out.println("ModelProcessor Init");
		
		License license = new License();
		try {
			InputStream licIs =  ModelProcessor.class.getResourceAsStream("/Aspose.Words.lic");
			license.setLicense(licIs);
			
		} catch (Exception e) {
			System.err.println("Licenza non trovata <<Aspose.Words.lic>>");
			e.printStackTrace();
		}
	}
	
	public boolean processModel(String m_documentId, String m_modelType, String xmlResponse, String outputFilePath, boolean pathNoName){
		final String _m_documentId = m_documentId;
		final String _m_modelType = m_modelType;
		final String _xmlResponse = xmlResponse;
		final String _outputFilePath = outputFilePath;
		final boolean _pathNoName = pathNoName;

		//System.out.println("processModel(" + _m_documentId + ", " + m_modelType + ", XML, " + outputFilePath + ", " + pathNoName + ")");
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			        	Boolean ret = false;
			        	try{			        				        	
			        		ret = _processModel(_m_documentId, _m_modelType, _xmlResponse, _outputFilePath, _pathNoName);
			        	}
			        	catch(Exception e){
			        		e.printStackTrace();
			        	}
			        	return ret;
			        }
			    }
			 );
		return b.booleanValue();
	}

	private boolean _processModel(String m_documentId, String m_modelType, String xmlResponse, String outputFilePath, boolean pathNoName){
		//ModelRequest request = new ModelRequest();
		boolean ret = false;
		AppProcessor appProcessor = null;
		try{
			ModelResponse response = new ModelResponse();
			
			if (!response.initialize(xmlResponse))
				throw new Exception("errore in inizializzazione del ModelResponse");
			
			if (response.m_processorInfo.m_classId.endsWith(ModelRequest.modelTypeOO))
				appProcessor = new OOProcessor();
			else if (response.m_processorInfo.m_classId.endsWith(ModelRequest.modelTypeWORD))
				appProcessor = new WordProcessor();
			else 
				throw new Exception("AppProcessor sconosciuto");
				
			if (pathNoName)
				outputFilePath += response.m_model.m_file.m_fileName;
			
			System.out.println("prima di processModel");

			ret = appProcessor.processModel(response, outputFilePath);
			System.out.println("dopo di processModel");
		}
		catch(Exception e){}
		finally{}		
		
		return ret;
	}
}
