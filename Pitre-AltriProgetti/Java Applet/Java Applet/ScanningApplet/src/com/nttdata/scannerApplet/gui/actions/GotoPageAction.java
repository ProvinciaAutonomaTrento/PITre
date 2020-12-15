package com.nttdata.scannerApplet.gui.actions;


import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;
import javax.swing.JOptionPane;
import javax.swing.JTextField;

import com.nttdata.scannerApplet.lang.GUI_LABELS;
import com.nttdata.scannerApplet.model.Controller;

public class GotoPageAction extends AbstractAction {
	private static final long serialVersionUID = 4952055959186812958L;

	
	public void actionPerformed(ActionEvent e) {
		if (e.getClass().isInstance(JTextField.class))
			throw new IllegalAccessError("This action should be used for the textfield");
		JTextField textField = (JTextField) e.getSource();
		String input = textField.getText();
		input = input.replaceAll("[^\\d]", "");
		try {
			@SuppressWarnings("boxing")
			int pageNumber = Integer.valueOf(input);
			Controller.getInstance().setCurrentPage(pageNumber);
			PanelCallback.updateApplet();
		} catch (NumberFormatException exception) {
			JOptionPane.showMessageDialog(null, GUI_LABELS.nonUnNumero, GUI_LABELS.attenzione,
					JOptionPane.ERROR_MESSAGE);
			textField.setText("" + Controller.getInstance().getCurrentPageNumber());
		}

	}
}
