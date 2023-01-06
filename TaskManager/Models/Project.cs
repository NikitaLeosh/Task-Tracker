using System.ComponentModel.DataAnnotations;
using System.Data;
using TaskManager.Models.Enum;

namespace TaskManager.Models
{
	public class Project
	{
		public Guid Id { get; set; }
		public string ProjectName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime CompletionDate { get; set; }
		public ProjectStatus ProjectStatus { get; set; }
		public int Priority { get; set; }
		public ICollection<ProjectTask> Tasks { get; set; }

	}
}
