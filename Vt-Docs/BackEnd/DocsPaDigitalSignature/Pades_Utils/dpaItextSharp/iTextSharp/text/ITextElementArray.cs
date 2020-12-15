using System;

namespace dpaItextSharp.text {
    /// <summary>
    /// Interface for a text element to which other objects can be added.
    /// </summary>
    /// <seealso cref="T:dpaItextSharp.text.Phrase"/>
    /// <seealso cref="T:dpaItextSharp.text.Paragraph"/>
    /// <seealso cref="T:dpaItextSharp.text.Section"/>
    /// <seealso cref="T:dpaItextSharp.text.ListItem"/>
    /// <seealso cref="T:dpaItextSharp.text.Chapter"/>
    /// <seealso cref="T:dpaItextSharp.text.Anchor"/>
    /// <seealso cref="T:dpaItextSharp.text.Cell"/>
    public interface ITextElementArray : IElement {
        /// <summary>
        /// Adds an object to the TextElementArray.
        /// </summary>
        /// <param name="o">an object that has to be added</param>
        /// <returns>true if the addition succeeded; false otherwise</returns>
        bool Add(Object o);
    }
}
