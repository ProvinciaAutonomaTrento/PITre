package com.nttdata.scannerApplet.gui.actions;


import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;

import com.nttdata.scannerApplet.gui.ZoomableScrollPane;

public class FitToPanelAction extends AbstractAction {
	private static final long serialVersionUID = 6516219511475852776L;
	private final ZoomableScrollPane imageViewer;

	public FitToPanelAction(String text, ZoomableScrollPane imgScrollPane) {
		super(text);
		this.imageViewer = imgScrollPane;
	}

	
	public void actionPerformed(ActionEvent e) {
		this.imageViewer.fitImageToPanel();
	}

}
