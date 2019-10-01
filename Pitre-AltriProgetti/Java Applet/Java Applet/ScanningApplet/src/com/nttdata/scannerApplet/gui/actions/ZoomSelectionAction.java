package com.nttdata.scannerApplet.gui.actions;

import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;

public class ZoomSelectionAction extends AbstractAction {
	private static final long serialVersionUID = -2202504937055535281L;

	public ZoomSelectionAction(String ingrandisciselezione) {
		super(ingrandisciselezione);
	}

	
	public void actionPerformed(ActionEvent e) {
		PanelCallback.setZoomSelectionMode();
	}

}
