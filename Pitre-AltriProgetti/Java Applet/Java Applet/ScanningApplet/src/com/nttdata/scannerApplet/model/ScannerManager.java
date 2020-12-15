package com.nttdata.scannerApplet.model;


import java.awt.Image;
import java.awt.Toolkit;
import java.awt.image.ImageConsumer;
import java.io.IOException;

import javax.swing.JOptionPane;

import com.nttdata.scannerApplet.gui.SaneConnectionOptionPane;
import com.nttdata.scannerApplet.lang.GUI_LABELS;

import SK.gnome.morena.Morena;
import SK.gnome.morena.MorenaException;
import SK.gnome.morena.MorenaImage;
import SK.gnome.morena.MorenaSource;
import SK.gnome.sane.SaneConnection;
import SK.gnome.sane.SaneException;
import SK.gnome.twain.TwainConstants;
import SK.gnome.twain.TwainException;
import SK.gnome.twain.TwainManager;
import SK.gnome.twain.TwainSource;

public class ScannerManager {
	private static ScannerManager instance = null;
	private final ImageHandler imageHandler;

	private ScannerManager() {
		this.imageHandler = new ImageHandler();
	}

	public static synchronized ScannerManager getInstance() {
		if (instance == null) instance = new ScannerManager();
		return instance;
	}

	public void selectSource() { // TODO linux
		String name = "SK.gnome.twain.TwainManager";
    	try {
    	this.getClass().getClassLoader().loadClass(name);
    	System.out.println("GIANGI: ScannerManager.selectSource - RELOADED CLASS: " + name);
    	} catch (ClassNotFoundException e) {
    		System.out.println("GIANGI: ScannerManager.selectSource - RELOAD CLASS ERROR");
    		e.printStackTrace();
    	}
    	
		try {
			if (this.isWindowsOs()) {
				TwainManager.selectSource(null);
			} else {
				Morena.selectSource(null);
			}
		} catch (TwainException e) {
			// do nothing
		} finally {
			try {
				int twainstate = TwainManager.getState();
				System.out.println("GIANGI: ScannerManager.selectSource - TwainManagerState = " + twainstate);
				System.out.println("GIANGI: DSM_LOADED = " + TwainConstants.DSM_LOADED + " - DSM_OPEN = " + TwainConstants.DSM_OPEN);
				if (twainstate == TwainConstants.DSM_LOADED || twainstate == TwainConstants.DSM_OPEN) {
					TwainManager.close();
					System.err.println("GIANGI: ScannerManager.acquireImages - TwainManager.close()");
				}
				Morena.close();
			} catch (MorenaException e) {
				e.printStackTrace();
			}
		}
	}

	public boolean acquireImages() {
		String name = "SK.gnome.twain.TwainManager";
    	try {
    	this.getClass().getClassLoader().loadClass(name);
    	System.out.println("GIANGI: ScannerManager.acquireImages - RELOADED CLASS: " + name);
    	} catch (ClassNotFoundException e) {
    		System.out.println("GIANGI: ScannerManager.acquireImages - RELOAD CLASS ERROR");
    		e.printStackTrace();
    	}
    	
		boolean success = false;
		try {
			MorenaSource scanSource = null;
			if (this.isWindowsOs())
				scanSource = TwainManager.getDefaultSource();
			else {
				String host, userName;
				int port;
				// scanSource = SaneConnection.selectSource(null);
				SaneConnectionOptionPane saneConnectionOptionPane = new SaneConnectionOptionPane("localhost", 6566,
						"saned");
				int answer = JOptionPane.showConfirmDialog(null, saneConnectionOptionPane,
						GUI_LABELS.scegliConnessione, JOptionPane.OK_CANCEL_OPTION);
				if (JOptionPane.OK_OPTION == answer) {
					host = saneConnectionOptionPane.getHost();
					port = saneConnectionOptionPane.getPort();
					userName = saneConnectionOptionPane.getUserName();

					SaneConnection sc = null;
					try {
						sc = SaneConnection.connect(host, port, userName);
						scanSource = sc.getDefaultSource();
					} catch (SaneException e) {
						e.printStackTrace();
					}catch (IOException e) {
						e.printStackTrace();
					}
				}
			}
			while (true) {
				MorenaImage image = new MorenaImage(scanSource);
				int imageStatus = image.getStatus();
				if (imageStatus == ImageConsumer.STATICIMAGEDONE) {
					System.out.println("Size of acquired image is " + image.getWidth() + " x " + image.getHeight()
							+ " x " + image.getPixelSize());

					this.imageHandler.addImage(Toolkit.getDefaultToolkit().createImage(image));
					success = true;

					if (TwainSource.class.isInstance(scanSource) && ((TwainSource) scanSource).hasMoreImages())
						continue;
					break;
				} else if ((imageStatus == ImageConsumer.IMAGEABORTED) || (imageStatus == ImageConsumer.IMAGEERROR))
					break;
			}
		} catch (TwainException e) {
			e.printStackTrace();
		} finally {
			try {
				int twainstate = TwainManager.getState();
				System.out.println("GIANGI: ScannerManager.acquireImages - TwainManagerState = " + twainstate);
				System.out.println("GIANGI: DSM_LOADED = " + TwainConstants.DSM_LOADED + " - DSM_OPEN = " + TwainConstants.DSM_OPEN);
				if (twainstate == TwainConstants.DSM_LOADED || twainstate == TwainConstants.DSM_OPEN) {
					TwainManager.close();
					System.out.println("GIANGI: ScannerManager.acquireImages - TwainManager.close()");
				}
				Morena.close();
			} catch (MorenaException e) {
				e.printStackTrace();
			}
		}

		return success;
	}

	private boolean isWindowsOs() {
		String os = System.getProperty("os.name");
		return os.startsWith("Windows");
	}

	public String writePdf() {
		String property = "java.io.tmpdir";
		// Get the temporary directory
		String tempDir = System.getProperty(property);
		PdfHandler pdfCreator = new PdfHandler();
		String path = pdfCreator.createPdf(tempDir, this.imageHandler.getImageList());
		return path;
	}

	public Image getCurrentImage() {
		Image img = this.imageHandler.getCurrentImage();
		return img;
	}

	public int getNumberOfPages() {
		return this.imageHandler.getNumberOfPages();
	}

	public int getCurrentPageNumber() {
		return this.imageHandler.getCurrentPageNumber();
	}

	public void showNextPage() {
		this.imageHandler.nextPage();

	}

	public void showPrevPage() {
		this.imageHandler.prevPage();

	}

	public void removeCurrentPage() {
		this.imageHandler.removeCurrentPage();
	}

	public void rotateCurrentPage(int rotationAmount) {
		this.imageHandler.rotateCurrentImage(rotationAmount);
	}

	public boolean hasImages() {
		return this.imageHandler.hasImages();
	}

	public void setCurrentPage(int pageNumber) {
		this.imageHandler.setCurrentPage(pageNumber);

	}
}
