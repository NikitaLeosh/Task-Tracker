using System.ComponentModel.DataAnnotations;
using TaskManager.Data.Enum;

namespace TaskManager.Dto
{
	public class ProjectDto
	{
		//This DTO is created to allow to avoid key property conflicts during creating and editing the projects
		public string ProjectName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime CompletionDate { get; set; }
		public ProjectStatus ProjectStatus { get; set; }
		[Range(1, 100, ErrorMessage = "Priority must be between 1 and 100")]
		public int Priority { get; set; }

	}
}
