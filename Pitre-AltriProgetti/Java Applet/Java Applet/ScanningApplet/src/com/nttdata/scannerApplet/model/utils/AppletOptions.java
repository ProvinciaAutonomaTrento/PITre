package com.nttdata.scannerApplet.model.utils;

import java.util.Arrays;
import java.util.List;
import java.util.Properties;

public final class AppletOptions {
	private static final String PATH = "Path";
	private static final String APPLY_COMPRESSION = "ApplyImageCompression";
	public static final String TIFF_FILETYPE = "0";
	public static final String PDF_FILETYPE = "1";
	public static final String PDFA_FILETYPE = "2";
	private static String FILE_TYPE = TIFF_FILETYPE;
	private static final String TRUE = "true";
	private final List<String> parameters = Arrays.asList(APPLY_COMPRESSION, PATH);
	private static AppletOptions instance;
	private Properties options;

	private AppletOptions() {
		instance = null;
	}

	public static AppletOptions getInstance() {
		if (instance == null) instance = new AppletOptions();
		return instance;
	}

	public List<String> getParameterList() {
		return this.parameters;
	}

	public void setOptions(Properties params) {
		this.options = params;
	}

	public String getSavePath() {
		String path = this.options.getProperty(PATH);
		if (path == null || path.equals("")) path = this.getTempFolderPath();
		return path;
	}

	private String getTempFolderPath() {
		// Get the temporary directory
		String property = "java.io.tmpdir";
		String tempDir;
		if (OsUtils.isLinuxOs()) {
			property = "user.home"; // for linux save to the home path
			tempDir = System.getProperty(property) + "/";
		} else
			tempDir = System.getProperty(property);
		return tempDir;
	}

	public boolean isSaveAsPdf() {
		boolean result = true;
		String value = FILE_TYPE;
		if (value != null) result = value.equals(PDF_FILETYPE);
		return result;
	}

	public boolean isSaveAsPdfA() {
		boolean result = true;
		String value = FILE_TYPE;
		if (value != null) result = value.equals(PDFA_FILETYPE);
		return result;
	}

	public boolean isSaveAsTiff() {
		boolean result = true;
		String value = FILE_TYPE;
		if (value != null) result = value.equals(TIFF_FILETYPE);
		return result;
	}

	public boolean isCompressionApplied() {
		boolean result = false;
		String value = this.options.getProperty(APPLY_COMPRESSION);
		if (value != null && value.equals(TRUE)) result = true;
		return result;
	}
	
	public void setFileType (String type) {
		AppletOptions.FILE_TYPE = type;
	}
}
