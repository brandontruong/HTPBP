using System;
namespace BP.Domain.Entities
{
    public class Milestone
    {
        public Guid MilestoneId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}