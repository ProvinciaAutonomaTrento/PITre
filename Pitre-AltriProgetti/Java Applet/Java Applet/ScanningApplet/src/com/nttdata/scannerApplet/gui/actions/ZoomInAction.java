package com.nttdata.scannerApplet.gui.actions;


import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;
import javax.swing.ImageIcon;

import com.nttdata.scannerApplet.gui.ZoomableScrollPane;

public class ZoomInAction extends AbstractAction {
	private static final long serialVersionUID = 8216522779613594024L;
	private final ZoomableScrollPane imageViewer;

	public ZoomInAction(String text, ImageIcon icon, ZoomableScrollPane imgScrollPane) {
		super(text, icon);
		this.imageViewer = imgScrollPane;
	}

	
	public void actionPerformed(ActionEvent e) {
		this.imageViewer.zoomIn();
		int zoom = (int) (100 * this.imageViewer.getZoom());
		PanelCallback.setZoomInfo(zoom);
	}

}
