package com.nttdata.scannerApplet.model.scanner_manager;

import java.awt.Image;
import java.util.List;

public interface ScannerManagerInterface {
	public void selectSource();

	public List<Image> acquireImages();

}
