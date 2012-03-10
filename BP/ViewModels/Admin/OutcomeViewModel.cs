using System;
using System.Web.Mvc;

namespace BP.ViewModels.Admin
{

    public class OutcomeViewModel
    {
        public Guid StepId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [AllowHtml]
        public string Guidance { get; set; }
        public int DisplayOrder { get; set; }
    }
}
