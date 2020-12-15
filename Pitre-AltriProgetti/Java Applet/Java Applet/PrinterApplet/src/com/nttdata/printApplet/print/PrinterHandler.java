package com.nttdata.printApplet.print;

import com.nttdata.printApplet.utils.OsUtils;

public class PrinterHandler {
	public void sendToPrinter(byte[] printdata, String printerFilter) {
		AppPrinterInterface printer = null;
		if (OsUtils.isLinuxOs()) 
			printer = new NixPrintMethod();
		else 
			printer = new DefaultPrintMethod(printerFilter);
		
		printer.acceptJob(printdata);
	}

}
