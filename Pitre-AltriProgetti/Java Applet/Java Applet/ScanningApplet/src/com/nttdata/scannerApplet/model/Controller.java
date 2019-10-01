package com.nttdata.scannerApplet.model;


import java.awt.Image;
import java.util.List;

//import javax.swing.JOptionPane;

import com.nttdata.scannerApplet.model.scanner_manager.LinuxScannerManager;
import com.nttdata.scannerApplet.model.scanner_manager.ScannerManagerInterface;
import com.nttdata.scannerApplet.model.scanner_manager.WindowsScannerManager;
import com.nttdata.scannerApplet.model.utils.AppletOptions;
import com.nttdata.scannerApplet.model.utils.OsUtils;

public class Controller {// FIXME separate in two classes, scannerManager and controller
	private static Controller instance = null;
	private final ImageHandler imageHandler;
	private final ScannerManagerInterface scannerManager;
    private String path = null;
    private int returnCode = 0;
    
	private Controller() {
		this.imageHandler = new ImageHandler();
		if (OsUtils.isWindowsOs())
			this.scannerManager = new WindowsScannerManager();
		else
			this.scannerManager = new LinuxScannerManager();
	}

	public static synchronized Controller getInstance() {
		if (instance == null) instance = new Controller();
		return instance;
	}

	public void selectSource() {
		this.scannerManager.selectSource();
	}

	public boolean acquireImages() {// add message to notify user that applet is processing the data
		boolean success = false;

		List<Image> scannedImages = this.scannerManager.acquireImages();
		if (scannedImages != null && !scannedImages.isEmpty()) {
			success = true;
			for (Image img : scannedImages)
				this.imageHandler.addImage(img);
		}

		return success;
	}

	public void writeFile(String savePath, boolean applyCompression) {
		path = null;
		if (AppletOptions.getInstance().isSaveAsPdf()) {
			path = PdfCreator.createPdf(savePath, this.imageHandler.getImageList(), applyCompression);
			System.out.println("PDF File saved at: " + path);
		}
		if (AppletOptions.getInstance().isSaveAsPdfA()) {
			//path = PdfCreator.createPdf(savePath, this.imageHandler.getImageList(), applyCompression);
			System.out.println( "PDFA File saved at: " + path);
		}
		if (AppletOptions.getInstance().isSaveAsTiff()) {
			path = TiffCreator.createTiff(savePath, this.imageHandler.getImageList(), applyCompression);
			System.out.println("TIFF file saved at: " + path);
		}	
		
		if (path == null) returnCode = -1;
		else returnCode = 1;
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

	public void rotateCurrentPage(int rotationAmount, boolean rotateAll) {
		if (rotateAll)
			this.imageHandler.rotateAllImages(rotationAmount);
		else
			this.imageHandler.rotateCurrentImage(rotationAmount);
	}

	public boolean hasImages() {
		return this.imageHandler.hasImages();
	}

	public void setCurrentPage(int pageNumber) {
		this.imageHandler.setCurrentPage(pageNumber);

	}

	public int getReturnCode() {
		// TODO Auto-generated method stub
		return returnCode;
	}

	public String getReturnPath() {
		// TODO Auto-generated method stub
		return path;
	}
	
	public void cleanAll() {
		this.returnCode = 0;
		this.path = null;
		this.imageHandler.getImageList().clear();
	}
}
