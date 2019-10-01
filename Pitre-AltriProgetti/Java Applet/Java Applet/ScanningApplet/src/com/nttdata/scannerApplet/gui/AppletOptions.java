package com.nttdata.scannerApplet.gui;

import java.util.Arrays;
import java.util.List;

public class AppletOptions {
	private static final List<String> parameters = Arrays.asList("ApplyImageCompression", "ApplyPdfConvertion ",
			"FileType", "Path");

	public static List<String> getParameterList() {
		return parameters;
	}

}
