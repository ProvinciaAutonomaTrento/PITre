package com.nttdata.scannerApplet.gui.actions;

import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;
import javax.swing.JDialog;

public class CloseWindowAction extends AbstractAction {
	private static final long serialVersionUID = 2946363685070228518L;
	private final JDialog parentDialog;

	public CloseWindowAction(String annulla, JDialog dialog) {
		super(annulla);
		this.parentDialog = dialog;
	}

	
	public void actionPerformed(ActionEvent e) {
		this.parentDialog.setVisible(false);
		this.parentDialog.dispose();
		//this.parentDialog.setModalityType(Dialog.ModalityType.MODELESS);
	}

}
