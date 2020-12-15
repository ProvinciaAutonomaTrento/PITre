package com.nttdata.scannerApplet.gui.actions;


import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;
import javax.swing.ImageIcon;

import com.nttdata.scannerApplet.gui.ZoomableScrollPane;

public class ZoomOutAction extends AbstractAction {
	private static final long serialVersionUID = 7724821816907622309L;
	private final ZoomableScrollPane imageViewer;

	public ZoomOutAction(String text, ImageIcon icon, ZoomableScrollPane imgScrollPane) {
		super(text, icon);
		this.imageViewer = imgScrollPane;
	}

	
	public void actionPerformed(ActionEvent e) {
		this.imageViewer.zoomOut();
		int zoom = (int) (100 * this.imageViewer.getZoom());
		PanelCallback.setZoomInfo(zoom);
	}

}
