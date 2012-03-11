using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BP.ViewModels.Admin
{

    public class TaskViewModel
    {
        public Guid TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid StepId { get; set; }
    }
}
