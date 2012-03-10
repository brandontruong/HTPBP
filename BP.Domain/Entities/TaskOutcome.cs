using System;
namespace BP.Domain.Entities
{
    public class TaskOutcome
    {
        public Guid TaskOutcomeId { get; set; }
        public Guid TaskId { get; set; }
        public string Description { get; set; }
        public Guid BikePlanApplicationId { get; set; }
    }
}