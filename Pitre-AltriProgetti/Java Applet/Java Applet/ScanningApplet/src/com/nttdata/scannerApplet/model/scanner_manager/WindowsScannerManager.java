package com.nttdata.scannerApplet.model.scanner_manager;

import java.awt.Image;
import java.awt.Toolkit;
import java.awt.image.ImageConsumer;
import java.util.ArrayList;
import java.util.List;

import SK.gnome.morena.Morena;
import SK.gnome.morena.MorenaException;
import SK.gnome.morena.MorenaImage;
import SK.gnome.morena.MorenaSource;
import SK.gnome.twain.TwainConstants;
import SK.gnome.twain.TwainException;
import SK.gnome.twain.TwainManager;
import SK.gnome.twain.TwainSource;

public class WindowsScannerManager implements ScannerManagerInterface {

	
	public void selectSource() {
		String name = "SK.gnome.twain.TwainManager";
    	try {
    		this.getClass().getClassLoader().loadClass(name);
    		System.out.println("GIANGI: WindowsScannerManager.selectSource - RELOADED CLASS: " + name);
    	} catch (ClassNotFoundException e) {
    		System.out.println("GIANGI: WindowsScannerManager.selectSource - RELOAD CLASS ERROR");
    		e.printStackTrace();
    	}
		
		try {
			TwainManager.selectSource(null);
		} catch (TwainException e) {
			// do nothing
		} finally {
			try {
				int twainstate = TwainManager.getState();
				System.out.println("GIANGI: WindowsScannerManager.selectSource - TwainManagerState = " + twainstate);
				System.out.println("GIANGI: DSM_LOADED = " + TwainConstants.DSM_LOADED + " - DSM_OPEN = " + TwainConstants.DSM_OPEN);
				if (twainstate == TwainConstants.DSM_LOADED || twainstate == TwainConstants.DSM_OPEN) {
					TwainManager.close();
					System.out.println("GIANGI: WindowsScannerManager.acquireImages - TwainManager.close()");
				}
				Morena.close();
				System.out.println("GIANGI: WindowsScannerManager.acquireImages - Morena.close()");
			} catch (MorenaException e) {
				e.printStackTrace();
			}
		}

	}

	
	public List<Image> acquireImages() {
		String name = "SK.gnome.twain.TwainManager";
    	try {
    	this.getClass().getClassLoader().loadClass(name);
    	System.out.println("GIANGI: WindowsScannerManager.acquireImages - RELOADED CLASS: " + name);
    	} catch (ClassNotFoundException e) {
    		System.out.println("GIANGI: WindowsScannerManager.acquireImages - RELOAD CLASS ERROR");
    		e.printStackTrace();
    	}
		
		List<Image> scannedImages = new ArrayList<Image>();
		try {
			MorenaSource scanSource = null;
			scanSource = TwainManager.getDefaultSource();
			while (true) {
				MorenaImage image = new MorenaImage(scanSource);
				int imageStatus = image.getStatus();
				if (imageStatus == ImageConsumer.STATICIMAGEDONE) {
					System.out.println("Size of acquired image is " + image.getWidth() + " x " + image.getHeight()
							+ " x " + image.getPixelSize());

					scannedImages.add(Toolkit.getDefaultToolkit().createImage(image));

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
				System.out.println("GIANGI: WindowsScannerManager.acquireImages - TwainManagerState = " + twainstate);
				System.out.println("GIANGI: DSM_LOADED = " + TwainConstants.DSM_LOADED + " - DSM_OPEN = " + TwainConstants.DSM_OPEN);
				if (twainstate == TwainConstants.DSM_LOADED || twainstate == TwainConstants.DSM_OPEN) {
					TwainManager.close();
					System.out.println("GIANGI: WindowsScannerManager.acquireImages - TwainManager.close()");
				}
				Morena.close();
				System.out.println("GIANGI: WindowsScannerManager.acquireImages - Morena.close()");
			} catch (MorenaException e) {
				e.printStackTrace();
			}
		}
		return scannedImages;
	}

}
