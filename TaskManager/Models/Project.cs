using System.ComponentModel.DataAnnotations;
using System.Data;
using TaskManager.Data.Enum;

namespace TaskManager.Models
{
	public class Project
	{
		public int Id { get; set; }
		public string ProjectName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime CompletionDate { get; set; }
		public ProjectStatus ProjectStatus { get; set; }
		[Range(1,100,ErrorMessage = "Priority must be between 1 and 100")]
		public int Priority { get; set; }
		public ICollection<ProjectTask> Tasks { get; set; }

	}
}
