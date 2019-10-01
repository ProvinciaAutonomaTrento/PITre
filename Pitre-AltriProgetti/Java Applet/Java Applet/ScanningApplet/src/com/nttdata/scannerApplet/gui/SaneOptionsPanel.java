package com.nttdata.scannerApplet.gui;


import java.awt.BorderLayout;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;

import javax.swing.DefaultComboBoxModel;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.border.EmptyBorder;

import com.nttdata.scannerApplet.lang.GUI_LABELS;

public class SaneOptionsPanel extends JPanel {
	private static final String BIANCO_E_NERO = "Bianco e Nero";
	private static final String SCALA_DI_GRIGI = "Scala di Grigi";
	private static final String COLORI = "Colori";
	private static final long serialVersionUID = -5975706340472581524L;
	private final JPanel contentPanel = new JPanel();
	private final JComboBox colorModeCombo, resolutionCombo;// <String> non c'è in 1.6

	/**
	 * Create the dialog.
	 */
	public SaneOptionsPanel() {
		this.setBounds(100, 100, 450, 300);
		this.setLayout(new BorderLayout());
		this.contentPanel.setBorder(new EmptyBorder(5, 5, 5, 5));
		this.contentPanel.setLayout(new GridBagLayout());
		this.add(this.contentPanel, BorderLayout.CENTER);

		JLabel selectResolution = new JLabel(GUI_LABELS.imposta_risoluzione);
		JLabel selectColorMode = new JLabel(GUI_LABELS.imposta_modo_colore);

		this.resolutionCombo = new JComboBox();
		this.resolutionCombo.setModel(new DefaultComboBoxModel(new String[] { "50", "100", "200", "300", "600" }));

		this.colorModeCombo = new JComboBox();
		this.colorModeCombo.setModel(new DefaultComboBoxModel(new String[] { COLORI, SCALA_DI_GRIGI, BIANCO_E_NERO }));

		GridBagConstraints c = new GridBagConstraints();
		c.fill = GridBagConstraints.HORIZONTAL;
		c.gridx = 0;
		c.gridy = 0;
		this.contentPanel.add(selectResolution, c);
		c.gridx = 1;
		this.contentPanel.add(this.resolutionCombo, c);
		c.gridx = 0;
		c.gridy = 1;
		this.contentPanel.add(selectColorMode, c);
		c.gridx = 1;
		this.contentPanel.add(this.colorModeCombo, c);

	}

	@SuppressWarnings("boxing")
	public int getResolution() {
		int resolution = 0;
		String res = (String) this.resolutionCombo.getSelectedItem();
		if (res != null) {
			resolution = Integer.valueOf(res);
		}
		return resolution;
	}

	public boolean isColorMode() {
		return this.colorModeCombo.getSelectedItem().equals(COLORI);
	}

	public boolean isGrayscaleMode() {
		return this.colorModeCombo.getSelectedItem().equals(SCALA_DI_GRIGI);
	}

	public boolean isBlackAndWhiteMode() {
		return this.colorModeCombo.getSelectedItem().equals(BIANCO_E_NERO);
	}

}
