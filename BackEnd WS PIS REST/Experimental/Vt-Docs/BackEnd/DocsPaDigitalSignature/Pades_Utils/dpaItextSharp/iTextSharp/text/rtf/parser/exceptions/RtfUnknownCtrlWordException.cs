using System;

namespace dpaItextSharp.text.rtf.parser.exceptions {

    public class RtfUnknownCtrlWordException : RtfParserException {

        // constructors
        
        /**
        * Constructs a <CODE>RtfParserException</CODE> whithout a message.
        */
        public RtfUnknownCtrlWordException() : base("Unknown control word.") {
        }
    }
}