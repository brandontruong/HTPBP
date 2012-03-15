// Type: System.Web.Mvc.MvcHtmlString
// Assembly: System.Web.Mvc, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Users\brandon\Documents\Visual Studio 2010\Projects\BikePlan\HTPBP.git\BP\_bin_deployableAssemblies\System.Web.Mvc.dll

using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace System.Web.Mvc
{
  public sealed class MvcHtmlString : HtmlString
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "MvcHtmlString is immutable")]
    public static readonly MvcHtmlString Empty = MvcHtmlString.Create(string.Empty);
    private readonly string _value;

    static MvcHtmlString()
    {
    }

    public MvcHtmlString(string value)
      : base(value ?? string.Empty)
    {
      this._value = value ?? string.Empty;
    }

    public static MvcHtmlString Create(string value)
    {
      return new MvcHtmlString(value);
    }

    public static bool IsNullOrEmpty(MvcHtmlString value)
    {
      if (value != null)
        return value._value.Length == 0;
      else
        return true;
    }
  }
}
