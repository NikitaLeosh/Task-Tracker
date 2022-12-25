using System.ComponentModel.DataAnnotations;
using TaskManager.Data.Enum;

namespace TaskManager.Dto
{
	public class ProjectNoTasksDto
	{
		//This Dto is created to show the list of projects without tasks
		public int Id { get; set; }
		public string ProjectName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime CompletionDate { get; set; }
		public ProjectStatus ProjectStatus { get; set; }
		[Range(1, 100, ErrorMessage = "Priority must be between 1 and 100")]
		public int Priority { get; set; }
	}
}
