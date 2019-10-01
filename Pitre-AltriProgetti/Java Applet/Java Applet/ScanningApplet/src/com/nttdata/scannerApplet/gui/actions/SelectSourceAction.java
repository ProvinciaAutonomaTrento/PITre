package com.nttdata.scannerApplet.gui.actions;


import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;

import com.nttdata.scannerApplet.model.Controller;

public class SelectSourceAction extends AbstractAction implements Runnable {
	private static final long serialVersionUID = 8608443163950579118L;

	public SelectSourceAction(String selectsource) {
		super(selectsource);
	}

	
	public void actionPerformed(ActionEvent event) {
		new Thread(this).start();
	}

	
	public synchronized void run() {
		PanelCallback.block();
		Controller instance = Controller.getInstance();
		instance.selectSource();
		PanelCallback.unblock();
	}

	@Override
	public boolean isEnabled() {
		return true;
	}
}
