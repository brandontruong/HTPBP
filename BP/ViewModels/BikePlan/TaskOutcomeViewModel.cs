using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BP.ViewModels.BikePlan
{

    public class TaskOutcomeViewModel
    {
        [UIHint("tinymce_jquery_full"), AllowHtml]
        public string Description { get; set; }
    }
}
