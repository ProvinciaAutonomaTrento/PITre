package com.nttdata.scannerApplet.gui;

import java.awt.BorderLayout;
import java.awt.FlowLayout;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Insets;

import javax.swing.DefaultComboBoxModel;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JDialog;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.WindowConstants;
import javax.swing.border.EmptyBorder;

public class SaneOptions extends JDialog {
	private static final long serialVersionUID = -5975706340472581524L;
	private final JPanel contentPanel = new JPanel();
	private JTextField textField;

	/**
	 * Launch the application.
	 */
	public static void main(String[] args) {
		try {
			SaneOptions dialog = new SaneOptions();
			dialog.setDefaultCloseOperation(WindowConstants.DISPOSE_ON_CLOSE);
			dialog.setVisible(true);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	/**
	 * Create the dialog.
	 */
	public SaneOptions() {
		this.setBounds(100, 100, 450, 300);
		this.getContentPane().setLayout(new BorderLayout());
		this.contentPanel.setBorder(new EmptyBorder(5, 5, 5, 5));
		this.getContentPane().add(this.contentPanel, BorderLayout.CENTER);
		GridBagLayout gbl_contentPanel = new GridBagLayout();
		gbl_contentPanel.columnWidths = new int[] { 158, 118, 0 };
		gbl_contentPanel.rowHeights = new int[] { 14, 0, 0 };
		gbl_contentPanel.columnWeights = new double[] { 0.0, 1.0, Double.MIN_VALUE };
		gbl_contentPanel.rowWeights = new double[] { 0.0, 0.0, Double.MIN_VALUE };
		this.contentPanel.setLayout(gbl_contentPanel);
		{
			JLabel lblRisoluzioneDiScansione = new JLabel("Risoluzione di scansione:");
			GridBagConstraints gbc_lblRisoluzioneDiScansione = new GridBagConstraints();
			gbc_lblRisoluzioneDiScansione.insets = new Insets(0, 0, 5, 5);
			gbc_lblRisoluzioneDiScansione.anchor = GridBagConstraints.NORTHEAST;
			gbc_lblRisoluzioneDiScansione.gridx = 0;
			gbc_lblRisoluzioneDiScansione.gridy = 0;
			this.contentPanel.add(lblRisoluzioneDiScansione, gbc_lblRisoluzioneDiScansione);
		}
		{
			this.textField = new JTextField();
			GridBagConstraints gbc_textField = new GridBagConstraints();
			gbc_textField.insets = new Insets(0, 0, 5, 0);
			gbc_textField.fill = GridBagConstraints.HORIZONTAL;
			gbc_textField.gridx = 1;
			gbc_textField.gridy = 0;
			this.contentPanel.add(this.textField, gbc_textField);
			this.textField.setColumns(10);
		}
		{
			JLabel lblModalit = new JLabel("Modalit\u00E0");
			GridBagConstraints gbc_lblModalit = new GridBagConstraints();
			gbc_lblModalit.anchor = GridBagConstraints.EAST;
			gbc_lblModalit.insets = new Insets(0, 0, 0, 5);
			gbc_lblModalit.gridx = 0;
			gbc_lblModalit.gridy = 1;
			this.contentPanel.add(lblModalit, gbc_lblModalit);
		}
		{
			JComboBox comboBox = new JComboBox();
			comboBox.setModel(new DefaultComboBoxModel(new String[] { "Colori", "Scala di Grigi", "Bianco e Nero" }));
			GridBagConstraints gbc_comboBox = new GridBagConstraints();
			gbc_comboBox.fill = GridBagConstraints.HORIZONTAL;
			gbc_comboBox.gridx = 1;
			gbc_comboBox.gridy = 1;
			this.contentPanel.add(comboBox, gbc_comboBox);
		}
		{
			JPanel buttonPane = new JPanel();
			buttonPane.setLayout(new FlowLayout(FlowLayout.RIGHT));
			this.getContentPane().add(buttonPane, BorderLayout.SOUTH);
			{
				JButton okButton = new JButton("OK");
				okButton.setActionCommand("OK");
				buttonPane.add(okButton);
				this.getRootPane().setDefaultButton(okButton);
			}
			{
				JButton cancelButton = new JButton("Cancel");
				cancelButton.setActionCommand("Cancel");
				buttonPane.add(cancelButton);
			}
		}
	}

}
