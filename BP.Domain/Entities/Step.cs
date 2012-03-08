using System;
namespace BP.Domain.Entities
{
    public class Step
    {
        public Guid StepId { get; set; }
        public string Name { get; set; }
        public string Guidance { get; set; }
        public int DisplayOrder { get; set; }
        public Guid MilestoneId { get; set; }
    }
}