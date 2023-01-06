using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskManager.Models.Enum;

namespace TaskManager.Models
{
	public class ProjectTask
	{
		public Guid Id { get; set; }
		public string TaskName { get; set; }
		public ProjectTaskStatus TaskStatus { get; set; }
		public string TaskDescription { get; set; }
		public int Priority { get; set; }
		public Project Project { get; set; }
	}
}
