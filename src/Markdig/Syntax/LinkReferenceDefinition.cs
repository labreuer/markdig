// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax.Inlines;

namespace Markdig.Syntax
{
    /// <summary>
    /// A link reference definition (Section 4.7 CommonMark specs)
    /// </summary>
    /// <seealso cref="Markdig.Syntax.LeafBlock" />
    public class LinkReferenceDefinition : LeafBlock
    {
        /// <summary>
        /// Creates an inline link for the specified <see cref="LinkReferenceDefinition"/>.
        /// </summary>
        /// <param name="inlineState">State of the inline.</param>
        /// <param name="linkRef">The link reference.</param>
        /// <param name="child">The child.</param>
        /// <returns>An inline link or null to use the default implementation</returns>
        public delegate Inline CreateLinkInlineDelegate(InlineProcessor inlineState, LinkReferenceDefinition linkRef, Inline child = null);

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkReferenceDefinition"/> class.
        /// </summary>
        public LinkReferenceDefinition() : base(null)
        {
            IsOpen = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkReferenceDefinition"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="url">The URL.</param>
        /// <param name="title">The title.</param>
        public LinkReferenceDefinition(string label, string url, string title) : this()
        {
            Label = label;
            Url = url;
            Title = title;
        }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The label span
        /// </summary>
        public SourceSpan LabelSpan;

        /// <summary>
        /// The URL span
        /// </summary>
        public SourceSpan UrlSpan;

        /// <summary>
        /// The title span
        /// </summary>
        public SourceSpan TitleSpan;


        /// <summary>
        /// Gets or sets the create link inline callback for this instance.
        /// </summary>
        /// <remarks>
        /// This callback is called when an inline link is matching this reference definition.
        /// </remarks>
        public CreateLinkInlineDelegate CreateLinkInline { get; set; }

        /// <summary>
        /// Tries to the parse the specified text into a definition.
        /// </summary>
        /// <typeparam name="T">Type of the text</typeparam>
        /// <param name="text">The text.</param>
        /// <param name="offset">Character offset of <paramref name="text"/>, relative to the document.</param>
        /// <param name="block">The block.</param>
        /// <returns><c>true</c> if parsing is successful; <c>false</c> otherwise</returns>
        /// <remarks>Does not assign <see cref="MarkdownObject.Line"/> or <see cref="MarkdownObject.Column"/>.</remarks>
        public static bool TryParse<T>(ref T text, int offset, out LinkReferenceDefinition block) where T : ICharIterator
        {
            block = null;
            string label;
            string url;
            string title;
            SourceSpan labelSpan;
            SourceSpan urlSpan;
            SourceSpan titleSpan;

            var start = text.Start;

            if (!LinkHelper.TryParseLinkReferenceDefinition(ref text, out label, out url, out title, out labelSpan, out urlSpan, out titleSpan))
            {
                return false;
            }

            var end = titleSpan.Length > 0 ? titleSpan.End : urlSpan.End;

            block = new LinkReferenceDefinition(label, url, title)
            {
                LabelSpan = OffsetSourceSpan(labelSpan, offset),
                UrlSpan = OffsetSourceSpan(urlSpan, offset),
                TitleSpan = OffsetSourceSpan(titleSpan, offset),
                Span = new SourceSpan(start + offset, end + offset)
            };
            return true;
        }

        private static SourceSpan OffsetSourceSpan(SourceSpan span, int offset)
        {
            return span.Length == 0
                ? span
                : new SourceSpan(span.Start + offset, span.End + offset);
        }
    }
}