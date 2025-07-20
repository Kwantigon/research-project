using DataspecNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace DataSpecificationNavigationBackend.ConnectorsLayer;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions options) : base(options) { }

	public DbSet<Conversation> Conversations { get; set; }
	public DbSet<Message> Messages { get; set; }
	public DbSet<DataSpecification> DataSpecifications { get; set; }
	public DbSet<DataSpecificationItem> DataSpecificationItems { get; set; }
}
