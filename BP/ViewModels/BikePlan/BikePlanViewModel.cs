﻿using System.Collections.Generic;
using BP.Infrastructure;
using BP.ViewModels.Admin;

namespace BP.ViewModels.BikePlan
{

    public class BikePlanViewModel
    {
        public IEnumerable<MilestoneViewModel> Milestones { get; set; }
        public IEnumerable<StepViewModel> Steps { get; set; }
        public MilestoneViewModel SelectedMilestone { get; set; }
        public StepViewModel SelectedStep { get; set; }
        public IEnumerable<TaskViewModel> Tasks { get; set; }

        public IList<TaskOutcomeViewModel> Outcomes { get; set; }

        public string MilestoneOrder { get; set; }
        public string StepOrder { get; set; }
    }
}
