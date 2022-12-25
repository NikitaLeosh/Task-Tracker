using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Data.Enum;

namespace TaskManager.Models
{
	public class ProjectTask
	{
		public int Id { get; set; }
		public string TaskName { get; set; }
		public ProjectTaskStatus TaskStatus { get; set; }
		public string TaskDescription { get; set; }
		[Range(1, 100, ErrorMessage = "Priority must be between 1 and 100")]
		public int Priority { get; set; }
		public Project Project{ get; set; }
	}
}
