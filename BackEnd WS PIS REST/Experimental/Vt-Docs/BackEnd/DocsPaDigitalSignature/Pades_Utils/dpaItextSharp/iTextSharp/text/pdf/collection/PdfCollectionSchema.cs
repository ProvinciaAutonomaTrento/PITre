using System;
using dpaItextSharp.text.pdf;

namespace dpaItextSharp.text.pdf.collection {

    public class PdfCollectionSchema : PdfDictionary {
        /**
        * Creates a Collection Schema dictionary.
        */
        public PdfCollectionSchema() : base(PdfName.COLLECTIONSCHEMA) {
        }
        
        /**
        * Adds a Collection field to the Schema.
        * @param name  the name of the collection field
        * @param field a Collection Field
        */
        public void AddField(String name, PdfCollectionField field) {
            Put(new PdfName(name), field);
        }
    }
}