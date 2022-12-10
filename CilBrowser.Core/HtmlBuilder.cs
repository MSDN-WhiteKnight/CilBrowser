/* CIL Browser (https://github.com/MSDN-WhiteKnight/CilBrowser)
 * Copyright (c) 2022,  MSDN.WhiteKnight 
 * License: BSD 3-Clause */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace CilBrowser.Core
{
    public class HtmlBuilder
    {
        TextWriter wr;

        public HtmlBuilder(TextWriter target)
        {
            this.wr = target;
        }

        public HtmlBuilder(StringBuilder sb)
        {
            this.wr = new StringWriter(sb);
        }

        static readonly HtmlAttribute[] NoAttributes = new HtmlAttribute[0];

        public static HtmlAttribute[] OneAttribute(string name, string val)
        {
            HtmlAttribute[] ret = new HtmlAttribute[1];
            ret[0] = new HtmlAttribute(name, val);
            return ret;
        }

        public void WriteRaw(string s)
        {
            wr.Write(s);
        }

        public void WriteEscaped(string s)
        {
            wr.Write(WebUtility.HtmlEncode(s));
        }

        void WriteAttributes(HtmlAttribute[] attributes)
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                wr.Write(' ');
                wr.Write(attributes[i].Name);

                string val = attributes[i].Value;

                if (val == null) val = string.Empty;

                val = WebUtility.HtmlEncode(val);
                wr.Write('=');
                wr.Write('"');
                wr.Write(val);
                wr.Write('"');
            }
        }

        public void WriteTagStart(string name, HtmlAttribute[] attributes)
        {
            if (attributes == null) attributes = new HtmlAttribute[0];

            wr.Write('<');
            wr.Write(name);
            WriteAttributes(attributes);
            wr.Write('>');
        }

        public void WriteTagStart(string name)
        {
            this.WriteTagStart(name, NoAttributes);
        }

        public void WriteTagEnd(string name)
        {
            wr.Write('<');
            wr.Write('/');
            wr.Write(name);
            wr.Write('>');
        }

        public void WriteTag(string name, string content)
        {
            this.WriteTag(name, content, NoAttributes, false);
        }

        public void WriteTag(string name, string content, HtmlAttribute[] attributes)
        {
            this.WriteTag(name, content, attributes, false);
        }

        public void WriteTag(string name, string content, HtmlAttribute[] attributes, bool isRaw)
        {
            if (content == null) content = string.Empty;
            if (attributes == null) attributes = new HtmlAttribute[0];

            wr.Write('<');
            wr.Write(name);
            WriteAttributes(attributes);

            if (content.Length == 0)
            {
                wr.Write('/');
                wr.Write('>');
                return;
            }

            wr.Write('>');
            string to_write = content;

            if (!isRaw) to_write = WebUtility.HtmlEncode(to_write);

            wr.Write(to_write);
            WriteTagEnd(name);
        }

        public void WriteHyperlink(string url, string text)
        {
            this.WriteTag("a", text, new HtmlAttribute[] { new HtmlAttribute("href", url) });
        }

        /// <summary>
        /// Writes hyperlink with the specified URL, text and additional attributes 
        /// (<c>href</c> attribute does not need to be included in <paramref name="attrs"/>).
        /// </summary>        
        public void WriteHyperlink(string url, string text, IEnumerable<HtmlAttribute> attrs)
        {
            List<HtmlAttribute> attrs2 = new List<HtmlAttribute>();
            attrs2.Add(new HtmlAttribute("href", url));

            foreach (HtmlAttribute ha in attrs) attrs2.Add(ha);

            this.WriteTag("a", text, attrs2.ToArray());
        }

        public void StartParagraph()
        {
            this.WriteTagStart("p", NoAttributes);
        }

        public void EndParagraph()
        {
            this.WriteTagEnd("p");
        }

        public void WriteParagraph(string text)
        {
            this.WriteTagStart("p", NoAttributes);
            this.WriteEscaped(text);
            this.WriteTagEnd("p");
        }

        public void WriteLineBreak()
        {
            this.WriteTag("br", string.Empty, NoAttributes);
        }

        public void StartDocument(string title)
        {
            this.WriteTagStart("html");
            this.WriteTagStart("head");
            this.WriteTag("title", title);
            this.WriteTagEnd("head");
            this.WriteTagStart("body");
        }

        public void EndDocument()
        {
            this.WriteTagEnd("body");
            this.WriteTagEnd("html");
        }
    }
}
