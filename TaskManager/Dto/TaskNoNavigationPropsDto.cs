using System.ComponentModel.DataAnnotations;
using TaskManager.Data.Enum;

namespace TaskManager.Dto
{
	public class TaskNoNavigationPropsDto
	{
		//This DTO is created to show tasks without navigational properties but with IDs
		public int Id { get; set; }
		public string TaskName { get; set; }
		public ProjectTaskStatus TaskStatus { get; set; }
		public string TaskDescription { get; set; }
		[Range(1, 100, ErrorMessage = "Priority must be between 1 and 100")]
		public int Priority { get; set; }
	}
}
