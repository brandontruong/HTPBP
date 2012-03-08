using System.Collections.Generic;
using BP.Infrastructure;

namespace BP.ViewModels.Admin
{

    public class AdminViewModel
    {
        public IEnumerable<MilestoneViewModel> Milestones { get; set; }
        public IEnumerable<StepViewModel> Steps { get; set; }
        public MilestoneViewModel SelectedMilestone { get; set; }
        public StepViewModel SelectedStep { get; set; }
        public CustomProfile CustomProfile { get; set; }
    }
}
