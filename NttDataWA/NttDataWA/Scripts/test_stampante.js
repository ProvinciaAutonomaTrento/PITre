alert("Inizio test Stampante");

var labelXml = '<?xml version="1.0" encoding="utf-8"?>\
    <DieCutLabel Version="8.0" Units="twips">\
        <PaperOrientation>Landscape</PaperOrientation>\
        <Id>Address</Id>\
        <PaperName>30252 Address</PaperName>\
        <DrawCommands/>\
        <ObjectInfo>\
            <TextObject>\
                <Name>Text</Name>\
                <ForeColor Alpha="255" Red="0" Green="0" Blue="0" />\
                <BackColor Alpha="0" Red="255" Green="255" Blue="255" />\
                <LinkedObjectName></LinkedObjectName>\
                <Rotation>Rotation0</Rotation>\
                <IsMirrored>False</IsMirrored>\
                <IsVariable>True</IsVariable>\
                <HorizontalAlignment>Left</HorizontalAlignment>\
                <VerticalAlignment>Middle</VerticalAlignment>\
                <TextFitMode>ShrinkToFit</TextFitMode>\
                <UseFullFontHeight>True</UseFullFontHeight>\
                <Verticalized>False</Verticalized>\
                <StyledText/>\
            </TextObject>\
            <Bounds X="332" Y="150" Width="4455" Height="1260" />\
        </ObjectInfo>\
    </DieCutLabel>';
var label = dymo.label.framework.openLabelXml(labelXml);

// set label text
label.setObjectText("Text", "Test Abbatangeli");

// select printer to print on
// for simplicity sake just use the first LabelWriter printer
var printers = dymo.label.framework.getPrinters();
if (printers.length == 0)
    throw "No DYMO printers are installed. Install DYMO printers.";

var printerName = "";
for (var i = 0; i < printers.length; ++i) {
    var printer = printers[i];
    if (printer.printerType == "LabelWriterPrinter") {
        printerName = printer.name;
        break;
    }
}

if (printerName == "")
    throw "No LabelWriter printers found. Install LabelWriter printer";

// finally print the label
label.print(printerName);

alert("Fine test Stampante");
