using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BP.Helpers
{
     public static class InputExtensions
    {
        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, List<CheckBoxListInfo> listInfo)
        {
            return htmlHelper.CheckBoxList(name, listInfo,
                ((IDictionary<string, object>) null));
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, List<CheckBoxListInfo> listInfo,
           object htmlAttributes)
       {
           return htmlHelper.CheckBoxList(name, listInfo, 
               ((IDictionary<string, object>)new RouteValueDictionary(htmlAttributes)));
       }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, List<CheckBoxListInfo> listInfo,
           IDictionary<string, object> htmlAttributes)
       {
           if (String.IsNullOrEmpty(name))
               throw new ArgumentException("The argument must have a value", "name");
           if (listInfo == null)
               throw new ArgumentNullException("listInfo");
           if (listInfo.Count < 1)
               throw new ArgumentException("The list must contain at least one value", "listInfo");
    
           StringBuilder sb = new StringBuilder();
    
           foreach (CheckBoxListInfo info in listInfo)
           {
               TagBuilder builder = new TagBuilder("input");
               if (info.IsChecked) builder.MergeAttribute("checked", "checked");
               builder.MergeAttributes<string, object>(htmlAttributes);
               builder.MergeAttribute("type", "checkbox");
               builder.MergeAttribute("value", info.Value);
               builder.MergeAttribute("name", name);
               builder.InnerHtml = info.DisplayText;
               sb.Append(builder.ToString(TagRenderMode.Normal));
               sb.Append("<br />");
           }

           return new MvcHtmlString(sb.ToString());
       }
   }

    public class CheckBoxListInfo
   {
       public CheckBoxListInfo(string value, string displayText, bool isChecked)
       {
           this.Value = value;
           this.DisplayText = displayText;
           this.IsChecked = isChecked;
       }
    
       public string Value { get; private set; }
       public string DisplayText { get; private set; }
       public bool IsChecked { get; private set; }
   }
}