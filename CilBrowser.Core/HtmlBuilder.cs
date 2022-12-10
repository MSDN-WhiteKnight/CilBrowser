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

        public void StartParagraph()
        {
            this.WriteTagStart("p", NoAttributes);
        }

        public void EndParagraph()
        {
            this.WriteTagEnd("p");
        }

        public void WriteLineBreak()
        {
            this.WriteTag("br", string.Empty, NoAttributes);
        }
    }

    public class HtmlAttribute
    {
        public HtmlAttribute()
        {
            this.Value = string.Empty;
        }

        public HtmlAttribute(string name, string val)
        {
            this.Name = name;
            this.Value = val;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}
