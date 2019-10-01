package com.nttdata.scannerApplet.gui;


import java.awt.BorderLayout;
import java.awt.Component;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Image;
import java.util.ArrayList;
import java.util.List;

import javax.swing.BorderFactory;
import javax.swing.ButtonGroup;
import javax.swing.DefaultComboBoxModel;
import javax.swing.JButton;
import javax.swing.JDialog;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JRadioButton;
import javax.swing.JTextField;

import com.nttdata.scannerApplet.gui.actions.CloseWindowAction;
import com.nttdata.scannerApplet.gui.actions.GotoPageAction;
import com.nttdata.scannerApplet.gui.actions.NextPageAction;
import com.nttdata.scannerApplet.gui.actions.PanelCallback;
import com.nttdata.scannerApplet.gui.actions.PrevPageAction;
import com.nttdata.scannerApplet.gui.actions.RemovePageAction;
import com.nttdata.scannerApplet.gui.actions.RestoreModeAction;
import com.nttdata.scannerApplet.gui.actions.RotatePageAction;
import com.nttdata.scannerApplet.gui.actions.SaveAction;
import com.nttdata.scannerApplet.gui.actions.ScanImageAction;
import com.nttdata.scannerApplet.gui.actions.SelectSourceAction;
import com.nttdata.scannerApplet.gui.actions.ZoomInAction;
import com.nttdata.scannerApplet.gui.actions.ZoomOutAction;
import com.nttdata.scannerApplet.gui.actions.ZoomSelectionAction;
import com.nttdata.scannerApplet.lang.GUI_LABELS;
import com.nttdata.scannerApplet.model.Controller;

public class ScanningPanel extends JPanel {
	private static final long serialVersionUID = 1762059778690321476L;
	private final ZoomableScrollPane imgScrollPane;
	private final JLabel lblPaginaNDi;
	private final List<Component> enabledByImagesComponents;
	private final List<Component> componentsToDisable;
	private final JLabel lblTotale;
	private final JTextField inptCurrentPage;
	private boolean noImages = true;
	private final ZoomComboBox zoomComboBox;
	private final JRadioButton rdbtnApplyToAll;
	private final JRadioButton rdbtnApplyToImage;
	private final JButton btnIngrandisciSelezione;

	/**
	 * Create the panel.
	 * 
	 * @param dialog
	 */
	public ScanningPanel(JDialog dialog) {
		this.enabledByImagesComponents = new ArrayList<Component>();
		this.componentsToDisable = new ArrayList<Component>();
		this.setLayout(new BorderLayout(0, 0));
		PanelCallback.setPanel(this);
		IconLoader icons = IconLoader.getInstance();
		// CREATE GUI
		// where the image is viewed, this must be created at the start, as imgScrollPane is used elsewere
		this.imgScrollPane = new ZoomableScrollPane();
		this.add(this.imgScrollPane, BorderLayout.CENTER);

		// the top button bar
		JPanel topBar = new JPanel();
		topBar.setLayout(new GridBagLayout());
		this.add(topBar, BorderLayout.NORTH);

		JPanel scanningPanel = new JPanel();
		JButton btnAcquisiciImmagine = new JButton();
		btnAcquisiciImmagine.setAction(new ScanImageAction(GUI_LABELS.scan, icons.getIcon(IconLoader.acquire)));

		JButton btnScegliScanner = new JButton();
		btnScegliScanner.setAction(new SelectSourceAction(GUI_LABELS.selectSource));

		scanningPanel.setLayout(new GridBagLayout());
		GridBagConstraints c = new GridBagConstraints();
		c.fill = GridBagConstraints.HORIZONTAL;
		c.gridx = 0;
		c.gridy = 0;
		scanningPanel.add(btnAcquisiciImmagine, c);
		c.gridy = 1;
		scanningPanel.add(btnScegliScanner, c);
		scanningPanel.setBorder(BorderFactory.createTitledBorder(GUI_LABELS.scansionePanelTitle));
		scanningPanel.setEnabled(false);

		JPanel rotationPanel = new JPanel();

		JButton btnRuota = new JButton();
		btnRuota.setAction(new RotatePageAction(90, icons.getIcon(IconLoader.rotate90Clock)));

		JButton btnRuotaCounter = new JButton();
		btnRuotaCounter.setAction(new RotatePageAction(-90, icons.getIcon(IconLoader.rotate90CounterClock)));

		JButton btnRuota_1 = new JButton();
		btnRuota_1.setAction(new RotatePageAction(180, icons.getIcon(IconLoader.rotate180)));

		ButtonGroup rGroup = new ButtonGroup();
		this.rdbtnApplyToImage = new JRadioButton(GUI_LABELS.applica_singolo);
		this.rdbtnApplyToAll = new JRadioButton(GUI_LABELS.applica_tutti);
		rGroup.add(this.rdbtnApplyToImage);
		rGroup.add(this.rdbtnApplyToAll);
		rGroup.setSelected(this.rdbtnApplyToImage.getModel(), true);

		rotationPanel.setLayout(new GridBagLayout());
		c = new GridBagConstraints();
		c.gridx = 0;
		c.gridy = 0;
		rotationPanel.add(btnRuota, c);
		c.gridx = 1;
		rotationPanel.add(btnRuotaCounter, c);
		c.gridx = 2;
		rotationPanel.add(btnRuota_1, c);
		JPanel radioRotationPanel = new JPanel();
		radioRotationPanel.add(this.rdbtnApplyToImage);
		radioRotationPanel.add(this.rdbtnApplyToAll);
		c.gridx = 0;
		c.gridy = 1;
		c.gridwidth = 3;
		rotationPanel.add(radioRotationPanel, c);
		rotationPanel.setBorder(BorderFactory.createTitledBorder(GUI_LABELS.rotazionePanelTitle));

		JPanel pagePanel = new JPanel();
		pagePanel.setBorder(BorderFactory.createTitledBorder(GUI_LABELS.paginaPanelTitle));
		pagePanel.setLayout(new GridBagLayout());

		JButton btnRimuoviPagina = new JButton();
		btnRimuoviPagina.setAction(new RemovePageAction(GUI_LABELS.removePage, icons.getIcon(IconLoader.cancel)));

		this.lblPaginaNDi = new JLabel(GUI_LABELS.pagina);

		this.inptCurrentPage = new JTextField();
		this.inptCurrentPage.setAction(new GotoPageAction());
		this.inptCurrentPage.setColumns(3);

		String ofPages = String.format(GUI_LABELS.ofPages, 0);
		this.lblTotale = new JLabel(ofPages);

		JButton btnPrecedente = new JButton();
		btnPrecedente.setAction(new PrevPageAction(GUI_LABELS.prevPage, icons.getIcon(IconLoader.previous)));

		JButton btnSuccessiva = new JButton();
		btnSuccessiva.setAction(new NextPageAction(GUI_LABELS.nextPage, icons.getIcon(IconLoader.next)));

		JPanel pageSelectionPanel = new JPanel();
		pageSelectionPanel.add(this.lblPaginaNDi);
		pageSelectionPanel.add(this.inptCurrentPage);
		pageSelectionPanel.add(this.lblTotale);
		pageSelectionPanel.validate();

		c = new GridBagConstraints();
		c.gridx = 0;
		c.gridy = 0;
		pagePanel.add(btnRimuoviPagina, c);
		c.gridx = 1;
		pagePanel.add(pageSelectionPanel, c);
		c.gridx = 0;
		c.gridy = 1;
		pagePanel.add(btnPrecedente, c);
		c.gridx = 1;
		pagePanel.add(btnSuccessiva, c);

		JPanel zoomPanel = new JPanel();
		zoomPanel.setLayout(new GridBagLayout());
		zoomPanel.setBorder(BorderFactory.createTitledBorder(GUI_LABELS.zoomPanelTitle));
		JButton btnZoomIn = new JButton();
		btnZoomIn.setAction(new ZoomInAction(GUI_LABELS.zoomIn, icons.getIcon(IconLoader.zoomIn), this.imgScrollPane));

		this.zoomComboBox = new ZoomComboBox(this.imgScrollPane);
		this.zoomComboBox.setModel(new DefaultComboBoxModel(new String[] { GUI_LABELS.adatta,
				GUI_LABELS.adattaLarghezza, "25%", "50%", "75%", "100%", "125%", "150%", "175%", "200%" }));
		this.zoomComboBox.setEditable(true);

		JButton btnZoomOut = new JButton();
		btnZoomOut.setAction(new ZoomOutAction(GUI_LABELS.zoomOut, icons.getIcon(IconLoader.zoomOut),
				this.imgScrollPane));

		this.btnIngrandisciSelezione = new JButton();
		this.btnIngrandisciSelezione.setAction(new ZoomSelectionAction(GUI_LABELS.ingrandisciSelezione));

		c = new GridBagConstraints();
		c.gridx = 0;
		c.gridy = 0;
		zoomPanel.add(btnZoomIn, c);
		c.gridx = 1;
		zoomPanel.add(this.zoomComboBox, c);
		c.gridx = 0;
		c.gridy = 1;
		zoomPanel.add(btnZoomOut, c);
		c.gridx = 1;
		zoomPanel.add(this.btnIngrandisciSelezione, c);

		c = new GridBagConstraints();
		c.fill = GridBagConstraints.VERTICAL;
		c.gridx = 0;
		c.gridy = 0;
		topBar.add(scanningPanel, c);
		c.gridx = 1;
		topBar.add(rotationPanel, c);
		c.gridx = 2;
		topBar.add(pagePanel, c);
		c.gridx = 3;
		topBar.add(zoomPanel, c);

		// buttons for the lower part
		JPanel lowerPanel = new JPanel();
		this.add(lowerPanel, BorderLayout.SOUTH);

		JButton btnCreaDocumento = new JButton();
		btnCreaDocumento.setAction(new SaveAction(GUI_LABELS.createDoc, icons.getIcon(IconLoader.createDoc), dialog));
		lowerPanel.add(btnCreaDocumento);

		JButton btnAnnulla = new JButton();
		btnAnnulla.setAction(new CloseWindowAction(GUI_LABELS.annulla, dialog));
		lowerPanel.add(btnAnnulla);

		this.enabledByImagesComponents.add(btnPrecedente);
		this.enabledByImagesComponents.add(btnRimuoviPagina);
		this.enabledByImagesComponents.add(btnSuccessiva);
		this.enabledByImagesComponents.add(btnCreaDocumento);
		this.enabledByImagesComponents.add(btnZoomIn);
		this.enabledByImagesComponents.add(btnZoomOut);
		this.enabledByImagesComponents.add(this.zoomComboBox);
		this.enabledByImagesComponents.add(this.inptCurrentPage);
		this.enabledByImagesComponents.add(btnRuota_1);
		this.enabledByImagesComponents.add(btnRuota);
		this.enabledByImagesComponents.add(btnRuotaCounter);
		this.enabledByImagesComponents.add(this.btnIngrandisciSelezione);
		this.enabledByImagesComponents.add(this.rdbtnApplyToImage);
		this.enabledByImagesComponents.add(this.rdbtnApplyToAll);
		this.componentsToDisable.addAll(this.enabledByImagesComponents);
		this.componentsToDisable.add(btnScegliScanner);
		this.componentsToDisable.add(btnAcquisiciImmagine);
		this.setNoImage();
	}

	public void setCurrentImage(Image image) {
		this.imgScrollPane.setImage(image);
		this.zoomComboBox.resetZoom();
	}

	@SuppressWarnings("boxing")
	public void setPageInfo(int page, int totalPages) {
		this.inptCurrentPage.setText("" + page);
		String ofPages = String.format(GUI_LABELS.ofPages, totalPages);
		this.lblTotale.setText(ofPages);
	}

	public void setNoImage() {
		this.imgScrollPane.setImage(null);
		for (Component c : this.enabledByImagesComponents)
			c.setEnabled(false);
	}

	public void enableButtonsForImages() {
		for (Component c : this.enabledByImagesComponents) {
			c.setEnabled(true);
		}
	}

	public void updateData() {
		Controller instance = Controller.getInstance();
		if (instance.hasImages()) {
			this.setCurrentImage(instance.getCurrentImage());
			if (this.noImages) {
				this.noImages = false;
				this.enableButtonsForImages();
			}
		} else {
			this.setNoImage();
			this.noImages = true;
		}
		this.setPageInfo(instance.getCurrentPageNumber(), instance.getNumberOfPages());
	}

	public void setZoomInfo(int zoom) {
		this.zoomComboBox.setCurrentText(zoom + "% ");
	}

	public boolean isRotateAllActivated() {
		return this.rdbtnApplyToAll.isSelected();
	}

	public void setZoomSelectionMode() {
		for (Component c : this.componentsToDisable)
			c.setEnabled(false);
		this.imgScrollPane.setZoomSelectionMode(true);
		this.btnIngrandisciSelezione.setAction(new RestoreModeAction(GUI_LABELS.restore));
	}

	public void undoZoomSelectionMode() {
		for (Component c : this.componentsToDisable)
			c.setEnabled(true);
		this.imgScrollPane.setZoomSelectionMode(false);
		this.btnIngrandisciSelezione.setAction(new ZoomSelectionAction(GUI_LABELS.ingrandisciSelezione));
	}

	public void block() {
		for (Component c : this.componentsToDisable)
			c.setEnabled(false);

	}

	public void unblock() {
		for (Component c : this.componentsToDisable)
			c.setEnabled(true);

	}
	
	public int getRetCode(){return 0;}
}
