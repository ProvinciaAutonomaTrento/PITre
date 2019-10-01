package Models;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.Map;


public abstract class AppProcessor {
	
	
		public abstract boolean processModel(ModelResponse response,
				String outputFilePath) throws Exception;


}
