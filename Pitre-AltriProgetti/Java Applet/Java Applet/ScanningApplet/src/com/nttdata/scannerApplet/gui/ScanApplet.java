package com.nttdata.scannerApplet.gui;


import java.applet.Applet;
import java.awt.Dimension;
import java.io.File;
import java.util.List;
import java.util.Properties;
import javax.swing.JDialog;
//import javax.swing.JLabel;
//import javax.swing.JPanel;
import javax.swing.UIManager;
import javax.swing.UnsupportedLookAndFeelException;

import com.nttdata.scannerApplet.lang.GUI_LABELS;
import com.nttdata.scannerApplet.model.Controller;
import com.nttdata.scannerApplet.model.utils.AppletOptions;

public class ScanApplet extends Applet {
	private static final long serialVersionUID = -6986565726795669160L;
    public static JDialog dialog;
    
    @Override
    public void init(){
    	//JLabel jl = new JLabel(initScanningDialog());
    	
    	//this.add(jl);
    	//initScanningDialog();
    }
    
	public boolean folderExists(String path) {
		final String _path = path;
		
		System.out.println("privileged");
		
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			            return _folderExists(_path);
			        }
			    }
			 );
		return b.booleanValue();
	}
		
	private boolean _folderExists(String path) {
				boolean ret = false;
		try{
			System.out.println(path);
			File folder = new File(path);
			ret = folder.exists() && folder.isDirectory();
			System.out.println("survive");
		}
		catch (Exception ex){
			System.out.println(ex.toString());

			ret = false;
		}
		
		return ret;
	}

	public String initScanningDialog() {
		System.out.println("ScanApplet.initScanningDialog - START");
		String retvalue = "";
		
		try {
			UIManager.setLookAndFeel(UIManager.getSystemLookAndFeelClassName());
		} catch (ClassNotFoundException e) {
			e.printStackTrace();
		}
		catch(InstantiationException e) {
			e.printStackTrace();
		}
		catch(IllegalAccessException e) {
			e.printStackTrace();
		}
		catch(UnsupportedLookAndFeelException e) {
			e.printStackTrace();
		}

		AppletOptions optionsInstance = AppletOptions.getInstance();
		Properties params = this.loadParameters(optionsInstance.getParameterList());
		optionsInstance.setOptions(params);
		
		java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Void>() {
			        public Void run() {
			            return startGui();
			        }
			    }
			 );
		
		
		if (Controller.getInstance().getReturnCode()==1){
			retvalue=Controller.getInstance().getReturnPath();
		}
		else {
			retvalue= String.valueOf(Controller.getInstance().getReturnCode());
		}
		
		System.out.println("ScanApplet.initScanningDialog - END");
		return retvalue;
	}

	private Properties loadParameters(List<String> options) {
		Properties parameters = new Properties();
		for (String parameter : options) {
			String value = this.getParameter(parameter);
			if (value == null) value = "";
			parameters.put(parameter, value);
		}
		return parameters;
	}

	private Void startGui() {
		this.cleanAll();
		
		this.dialog = new JDialog();
		
		//dialog.setModalityType(Dialog.DEFAULT_MODALITY_TYPE);
		dialog.setModal(true);
		ScanningPanel scanningPanel = new ScanningPanel(dialog);
		dialog.add(scanningPanel);
		dialog.setSize(new Dimension(1000, 700));
		dialog.setTitle(GUI_LABELS.titolo);
		dialog.setVisible(true);
		System.out.println("Gui Opened");
		
		return null;
	}
	
	private void cleanAll(){
		Controller.getInstance().cleanAll();
	}
	
	public int getReturnCode(){return Controller.getInstance().getReturnCode();}
	public String getReturnPath(){return Controller.getInstance().getReturnPath();}
	
	public void setFileType (String type) {
		AppletOptions.getInstance().setFileType(type);
	}
	
	public void closeApplet(){
		System.out.println("GIANGI: ScanApplet.closeApplet - END");
		this.dialog.dispose(); 
		this.destroy();
		//this.dialog.setVisible(false);
	} 
	
	public void killApplet() 
	 {
		java.security.AccessController.doPrivileged(new java.security.PrivilegedAction<Void>() {
	        public Void run() {
	            // kill the JVM
	            System.exit(0);
	            return null;
	        }
	    });
	 }
}
