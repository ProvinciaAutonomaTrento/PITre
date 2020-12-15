package com.nttdata.printApplet.print;

import java.util.Locale;

import javax.print.Doc;
import javax.print.DocFlavor;
import javax.print.DocPrintJob;
import javax.print.PrintException;
import javax.print.PrintService;
import javax.print.PrintServiceLookup;
import javax.print.ServiceUI;
import javax.print.SimpleDoc;
import javax.print.attribute.HashPrintRequestAttributeSet;
import javax.print.attribute.PrintRequestAttributeSet;
import javax.print.attribute.standard.JobName;
import javax.print.event.PrintJobEvent;
import javax.print.event.PrintJobListener;

public class DefaultPrintMethod implements PrintJobListener, AppPrinterInterface {
	private volatile boolean done = false;
	private volatile String printerFilter = null;

	public DefaultPrintMethod(String _printerFilter){
		super();
		printerFilter = _printerFilter;
	}
	
	public void acceptJob(byte[] printdata) {
		PrintService pservice = this.getPrintServiceFiltered();
		if (pservice == null) return;
		System.out.println("Name: " + pservice.getName() + " supports: ");
		DocFlavor[] sdoc = pservice.getSupportedDocFlavors();
		for (DocFlavor d : sdoc) {
			System.out.println(d);
		}
		DocPrintJob job = pservice.createPrintJob();
		DocFlavor flavor = DocFlavor.BYTE_ARRAY.AUTOSENSE;
		Doc doc = new SimpleDoc(printdata, flavor, null);
		PrintRequestAttributeSet prass = new HashPrintRequestAttributeSet();
		prass.add(new JobName("zebra_row_printing", Locale.getDefault()));
		job.addPrintJobListener(this);
		try {
			job.print(doc, prass);
		} catch (PrintException e) {
			e.printStackTrace();
		}
		while (!this.isDone()) {
			try {
				Thread.sleep(100);
			} catch (InterruptedException e) {
				e.printStackTrace();
			}
		}
	}

	public PrintService getPrintServiceFiltered() {
		PrintService selectedPS = null;
		PrintService[] services = PrintServiceLookup.lookupPrintServices(null, null);
		for (PrintService ps : services){
			if (ps.getName().toUpperCase().contains(printerFilter.toUpperCase()) && selectedPS == null)
				selectedPS = ps;
		}
		
		if (selectedPS == null){
			PrintService svc = PrintServiceLookup.lookupDefaultPrintService();
			PrintRequestAttributeSet attrs = new HashPrintRequestAttributeSet();
			selectedPS = ServiceUI.printDialog(null, 100, 100, services, svc, DocFlavor.BYTE_ARRAY.AUTOSENSE,
					attrs);
		}
		return selectedPS;
	}

	public PrintService getPrintService() {
		PrintService[] services = PrintServiceLookup.lookupPrintServices(null, null);
		PrintService svc = PrintServiceLookup.lookupDefaultPrintService();
		PrintRequestAttributeSet attrs = new HashPrintRequestAttributeSet();
		PrintService selection = ServiceUI.printDialog(null, 100, 100, services, svc, DocFlavor.BYTE_ARRAY.AUTOSENSE,
				attrs);
		return selection;
	}

	public void printDataTransferCompleted(PrintJobEvent pje) {
		System.out.println("completed");
		this.done = true;

	}

	public void printJobCompleted(PrintJobEvent pje) {
		System.out.println("good");
		this.done = true;

	}

	public void printJobFailed(PrintJobEvent pje) {
		System.out.println("failed");
		this.done = true;
	}

	public void printJobCanceled(PrintJobEvent pje) {
		System.out.println("cancelled");
		this.done = true;

	}

	public void printJobNoMoreEvents(PrintJobEvent pje) {
		System.out.println("no more jobs");

	}

	public void printJobRequiresAttention(PrintJobEvent pje) {
		System.out.println("attention");

	}

	private boolean isDone() {
		return this.done;
	}
}
