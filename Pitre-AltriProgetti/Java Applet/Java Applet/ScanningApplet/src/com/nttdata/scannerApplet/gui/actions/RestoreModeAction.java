package com.nttdata.scannerApplet.gui.actions;

import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;

public class RestoreModeAction extends AbstractAction {
	private static final long serialVersionUID = 5142497130036042837L;

	public RestoreModeAction(String restore) {
		super(restore);
	}

	
	public void actionPerformed(ActionEvent e) {
		PanelCallback.undoZoomSelectionMode();
	}

}
