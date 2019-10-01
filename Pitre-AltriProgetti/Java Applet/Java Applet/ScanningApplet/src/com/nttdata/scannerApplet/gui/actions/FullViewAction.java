package com.nttdata.scannerApplet.gui.actions;


import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;

import com.nttdata.scannerApplet.gui.ZoomableScrollPane;
import com.nttdata.scannerApplet.lang.GUI_LABELS;

public class FullViewAction extends AbstractAction {
	private static final long serialVersionUID = 819890495639797175L;
	private final ZoomableScrollPane imageViewer;

	public FullViewAction(ZoomableScrollPane imgScrollPane) {
		super(GUI_LABELS.originalView);
		this.imageViewer = imgScrollPane;
	}

	
	public void actionPerformed(ActionEvent e) {
		this.imageViewer.showTrueSize();
	}

}
