using EmailService.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Database
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options)
			: base(options)
		{
		}
		
		public DbSet<LogEntry> Logs { get; set;	}
	}
}