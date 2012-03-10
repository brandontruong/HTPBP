using System;
namespace BP.Domain.Entities
{
    public class Task
    {
        public Guid TaskId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public Guid StepId { get; set; }
    }
}