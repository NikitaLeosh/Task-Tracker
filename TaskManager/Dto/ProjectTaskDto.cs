using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Data.Enum;

namespace TaskManager.Dto
{
	public class ProjectTaskDto
	{
		//This DTO is intended to avoid conflicts with existing fields while creating/editing tasks
		public string TaskName { get; set; }
		public ProjectTaskStatus TaskStatus { get; set; }
		public string TaskDescription { get; set; }
		[Range(1, 100, ErrorMessage = "Priority must be between 1 and 100")]
		public int Priority { get; set; }
	}
}
