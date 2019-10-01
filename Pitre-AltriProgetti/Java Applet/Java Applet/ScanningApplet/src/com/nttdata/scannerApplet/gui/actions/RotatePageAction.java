package com.nttdata.scannerApplet.gui.actions;


import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;
import javax.swing.ImageIcon;

import com.nttdata.scannerApplet.model.Controller;

public class RotatePageAction extends AbstractAction implements Runnable {
	private static final long serialVersionUID = 2501796720736723297L;
	int rotationAmount;

	public RotatePageAction(int amount, ImageIcon imageIcon) {
		super("", imageIcon);
		this.rotationAmount = amount;
	}

	
	public void actionPerformed(ActionEvent event) {
		new Thread(this).start();
	}

	
	public synchronized void run() {
		Controller instance = Controller.getInstance();
		boolean rotateAll = PanelCallback.isRotateAllActivated();
		instance.rotateCurrentPage(this.rotationAmount, rotateAll);
		PanelCallback.updateApplet();
	}

	@Override
	public boolean isEnabled() {
		return true;
	}
}
