using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TaskManager.Data;
using TaskManager.Exceptions;
using TaskManager.Interfaces;
using TaskManager.Models;

namespace TaskManager.Repositories
{
	public class CheckProjectRepository : ICheckProjectRepository
	{
		private readonly ProjectDbContext _context;

		public CheckProjectRepository(ProjectDbContext context)
		{
			_context = context;
		}
		public async Task<bool> ProjectExistsAsync(Guid projectId)
		{
			return await _context.Projects.AnyAsync(p => p.Id == projectId);
		}

		public async Task<bool> ProjectNameAlreadyTakenAsync(Project project)
		{
			return await _context.Projects
				.AnyAsync(n => n.ProjectName.Trim().ToUpper() == project.ProjectName.Trim().ToUpper());
		}
	}
}
