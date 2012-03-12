// Type: iTextSharp.tool.xml.parser.XMLParser
// Assembly: itextsharp.xmlworker, Version=1.1.1.0, Culture=neutral, PublicKeyToken=8354ae6d2174ddca
// Assembly location: C:\Users\brandon\Documents\Visual Studio 2010\Projects\BikePlan\HTPBP.git\BP\packages\iTextSharp.5.2.0\lib\itextsharp.xmlworker.dll

using iTextSharp.text.xml;
using iTextSharp.text.xml.simpleparser;
using iTextSharp.tool.xml.parser.io;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace iTextSharp.tool.xml.parser
{
  public class XMLParser
  {
    private IState state;
    private StateController controller;
    private IList<IXMLParserListener> listeners;
    private XMLParserMemory memory;
    private IParserMonitor monitor;
    private string text;
    private TagState tagState;
    private Encoding charset;

    public Encoding Charset
    {
      get
      {
        return this.charset;
      }
    }

    public XMLParser()
      : this(true, Encoding.Default)
    {
    }

    public XMLParser(bool isHtml, Encoding charset)
    {
      this.charset = charset;
      this.controller = new StateController(this, isHtml);
      this.controller.Unknown();
      this.memory = new XMLParserMemory();
      this.listeners = (IList<IXMLParserListener>) new List<IXMLParserListener>();
    }

    public XMLParser(IXMLParserListener listener, Encoding charset)
      : this(true, charset)
    {
      this.listeners.Add(listener);
    }

    public XMLParser(bool b, IXMLParserListener listener)
      : this(b, Encoding.Default)
    {
      this.listeners.Add(listener);
    }

    public XMLParser(IXMLParserListener listener)
      : this(true, Encoding.Default)
    {
      this.listeners.Add(listener);
    }

    public XMLParser(bool isHtml, IXMLParserListener listener, Encoding charset)
      : this(isHtml, charset)
    {
      this.listeners.Add(listener);
    }

    public XMLParser AddListener(IXMLParserListener pl)
    {
      this.listeners.Add(pl);
      return this;
    }

    public XMLParser RemoveListener(IXMLParserListener pl)
    {
      this.listeners.Remove(pl);
      return this;
    }

    public void Parse(Stream inp)
    {
      this.Parse((TextReader) new StreamReader(inp));
    }

    public void Parse(Stream inp, bool detectEncoding)
    {
      if (detectEncoding)
        this.Parse((TextReader) this.DetectEncoding(inp));
      else
        this.Parse(inp);
    }

    public void Parse(Stream inp, Encoding charSet)
    {
      this.charset = charSet;
      this.Parse((TextReader) new StreamReader(inp, charSet));
    }

    public void Parse(TextReader reader)
    {
      this.ParseWithReader(reader);
    }

    private void ParseWithReader(TextReader reader)
    {
      foreach (IXMLParserListener xmlParserListener in (IEnumerable<IXMLParserListener>) this.listeners)
        xmlParserListener.Init();
      TextReader textReader = this.monitor == null ? reader : (TextReader) new MonitorInputReader(reader, this.monitor);
      char[] buffer = new char[1];
      try
      {
        while (1 == textReader.Read(buffer, 0, 1))
          this.state.Process(buffer[0]);
      }
      finally
      {
        foreach (IXMLParserListener xmlParserListener in (IEnumerable<IXMLParserListener>) this.listeners)
          xmlParserListener.Close();
        textReader.Close();
      }
    }

    public StreamReader DetectEncoding(Stream inp)
    {
      byte[] numArray = new byte[4];
      if (inp.Read(numArray, 0, numArray.Length) != 4)
        throw new IOException("Insufficient length");
      string name = XMLUtil.GetEncodingName(numArray);
      string decl = (string) null;
      if (name.Equals("UTF-8"))
      {
        StringBuilder stringBuilder = new StringBuilder();
        int num;
        while ((num = inp.ReadByte()) != -1 && num != 62)
          stringBuilder.Append((char) num);
        decl = ((object) stringBuilder).ToString();
      }
      else if (name.Equals("CP037"))
      {
        MemoryStream memoryStream = new MemoryStream();
        int num;
        while ((num = inp.ReadByte()) != -1 && num != 110)
          memoryStream.WriteByte((byte) num);
        decl = Encoding.GetEncoding(37).GetString(memoryStream.ToArray());
      }
      if (decl != null)
      {
        string declaredEncoding = EncodingUtil.GetDeclaredEncoding(decl);
        if (declaredEncoding != null)
          name = declaredEncoding;
      }
      if (inp.CanSeek)
        inp.Seek(0L, SeekOrigin.Begin);
      return new StreamReader(inp, IanaEncodings.GetEncodingEncoding(name));
    }

    protected internal void SetState(IState state)
    {
      this.state = state;
    }

    public XMLParser Append(char character)
    {
      this.memory.Current().Append(character);
      return this;
    }

    public StateController SelectState()
    {
      return this.controller;
    }

    public void UnknownData()
    {
      foreach (IXMLParserListener xmlParserListener in (IEnumerable<IXMLParserListener>) this.listeners)
        xmlParserListener.UnknownText(((object) this.memory.Current()).ToString());
    }

    public void Flush()
    {
      this.memory.ResetBuffer();
    }

    public string Current()
    {
      return ((object) this.memory.Current()).ToString();
    }

    public XMLParserMemory Memory()
    {
      return this.memory;
    }

    public void StartElement()
    {
      this.CurrentTagState(TagState.OPEN);
      this.CallText();
      foreach (IXMLParserListener xmlParserListener in (IEnumerable<IXMLParserListener>) this.listeners)
        xmlParserListener.StartElement(this.memory.GetCurrentTag(), this.memory.GetAttributes(), this.memory.GetNameSpace());
      this.memory.FlushNameSpace();
    }

    private void CallText()
    {
      if (this.text == null || this.text.Length <= 0)
        return;
      foreach (IXMLParserListener xmlParserListener in (IEnumerable<IXMLParserListener>) this.listeners)
        xmlParserListener.Text(this.text);
      this.text = (string) null;
    }

    public void EndElement()
    {
      this.CurrentTagState(TagState.CLOSE);
      this.CallText();
      foreach (IXMLParserListener xmlParserListener in (IEnumerable<IXMLParserListener>) this.listeners)
        xmlParserListener.EndElement(this.memory.GetCurrentTag(), this.memory.GetNameSpace());
    }

    public void Text(string bs)
    {
      this.text = bs;
    }

    public void Comment()
    {
      this.CallText();
      foreach (IXMLParserListener xmlParserListener in (IEnumerable<IXMLParserListener>) this.listeners)
        xmlParserListener.Comment(((object) this.memory.Current()).ToString());
    }

    public char CurrentLastChar()
    {
      StringBuilder stringBuilder = this.memory.Current();
      if (stringBuilder.Length == 0)
        return ' ';
      else
        return stringBuilder[stringBuilder.Length - 1];
    }

    public string CurrentTag()
    {
      return this.memory.GetCurrentTag();
    }

    public TagState CurrentTagState()
    {
      return this.tagState;
    }

    private void CurrentTagState(TagState state)
    {
      this.tagState = state;
    }

    public void SetMonitor(IParserMonitor monitor)
    {
      this.monitor = monitor;
    }

    public string BufferToString()
    {
      return ((object) this.memory.Current()).ToString();
    }

    public XMLParser Append(char[] bytes)
    {
      this.memory.Current().Append(bytes);
      return this;
    }

    public int BufferSize()
    {
      if (this.memory.Current() == null)
        return 0;
      else
        return this.memory.Current().Length;
    }

    public XMLParser Append(string str)
    {
      this.memory.Current().Append(str);
      return this;
    }
  }
}
