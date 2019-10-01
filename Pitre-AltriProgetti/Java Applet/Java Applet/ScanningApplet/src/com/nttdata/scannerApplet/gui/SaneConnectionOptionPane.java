package com.nttdata.scannerApplet.gui;

import java.awt.Component;

import javax.swing.BorderFactory;
import javax.swing.BoxLayout;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTextField;

public class SaneConnectionOptionPane extends JPanel {
	private static final long serialVersionUID = -242635043867785498L;
	private JTextField hostTextField;
	private JTextField portTextField;
	private JTextField userNameTextField;

	public SaneConnectionOptionPane(String host, int port, String userName) {
		this.setBorder(BorderFactory.createEmptyBorder(5, 5, 5, 5));
		this.setLayout(new BoxLayout(this, BoxLayout.Y_AXIS));
		this.setAlignmentX(Component.LEFT_ALIGNMENT);

		JPanel hostPanel = new JPanel();
		hostPanel.setLayout(new BoxLayout(hostPanel, BoxLayout.X_AXIS));
		hostPanel.setAlignmentX(Component.LEFT_ALIGNMENT);
		hostPanel.add(new JLabel("Host : "));
		hostPanel.add(this.hostTextField = new JTextField(host));
		this.add(hostPanel);

		JPanel portPanel = new JPanel();
		portPanel.setLayout(new BoxLayout(portPanel, BoxLayout.X_AXIS));
		portPanel.setAlignmentX(Component.LEFT_ALIGNMENT);
		portPanel.add(new JLabel("PORT"));
		portPanel.add(this.portTextField = new JTextField(String.valueOf(port)));
		this.add(portPanel);

		JPanel userNamePanel = new JPanel();
		userNamePanel.setLayout(new BoxLayout(userNamePanel, BoxLayout.X_AXIS));
		userNamePanel.setAlignmentX(Component.LEFT_ALIGNMENT);
		userNamePanel.add(new JLabel("USER_NAME"));
		userNamePanel.add(this.userNameTextField = new JTextField(userName, 10));
		this.add(userNamePanel);

	}

	public String getHost() {
		return this.hostTextField.getText();
	}

	public int getPort() {
		return Integer.parseInt(this.portTextField.getText());
	}

	public String getUserName() {
		return this.userNameTextField.getText();
	}

}
