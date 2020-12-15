package com.nttdata.printApplet.gui;



import java.nio.charset.Charset;
import javax.swing.JApplet;

//import java.awt.event.ActionEvent;
//import javax.swing.AbstractAction;
//import javax.swing.JButton;
//import javax.swing.JPanel;

import com.nttdata.printApplet.print.PrinterHandler;

public class PrintApplet extends JApplet {
	private static final long serialVersionUID = -6135902941375606145L;

	public void init() {
		System.out.println("PrintApplet init");
		
		//JPanel panel = new JPanel();
		//JButton normalPrint = new JButton("Print");
		//normalPrint.setAction(new AbstractAction("Print") {
			//private static final long serialVersionUID = -4927753635569569775L;

			//public void actionPerformed(ActionEvent e) {
				//String daSta = "^XA^FO670,238^GB105,10,10^FS^FO670,248^GB10,112,10^FS^FO765,248^GB10,112,10^FS^FO670,360^GB105,10,10^FS^FO476,3^GB4,145,4^FS^FO650,173^A0N,50,55^FWN^FH^FDFedEx^FS^XZ";
//String daSta = "N{}q480{}Q240,24+0{}X5,5,2,440,220{}A60,20,0,4,1,1,N,\"<AMMINISTRAZIONE>\"{}A15,70,0,3,1,1,N,\"Prot. generale del <DATA_PROTOCOLLO>\"{}A120,120,0,4,1,1,N,\"N. <NUMERO_PROTOCOLLO>\"{}B35,150,0,3,2,5,65,N,\"<ANNO_PROTOCOLLO><NUMERO_PROTOCOLLO>\"{}P1{}";

				//print("Ciao","POLLO");
//print(daSta,"Generic / Text Only");

			//}
		//});
		//panel.add(normalPrint);
		//this.add(panel);
	}
	
	public String testChiamata(final String msgg){
		
		if (msgg.length()>0){
			return "Test positivo";
		}
		else {
			return msgg;
		}
	}
	
	public void print(final String printdata, final String printerFilter) {
		Boolean b = java.security.AccessController.doPrivileged(
			    new java.security.PrivilegedAction<Boolean>() {
			        public Boolean run() {
			            return _print(printdata, printerFilter);
			        }
			    }
			 );		
	}

	//PER LORENZO
	//Mi serve una cosa simile:
	public boolean _print(String printdata, String printerFilter) {
		boolean ret = false;
		
		System.out.println(printdata);
		printdata = printdata.replace("{}","\r\n");
		System.out.println(printdata);
		printdata = printdata.concat("\r\n");
		System.out.println(printdata);
		
		try{
			PrinterHandler printer = new PrinterHandler();
			printer.sendToPrinter(printdata.getBytes(Charset.defaultCharset().name()), printerFilter);
			ret = true;
		}
		catch(Exception e){
			
			e.printStackTrace();
		}
		
		return ret;
	}

	public void close() {
	   this.setEnabled(false);
	   this.destroy();
   }
	   
}
