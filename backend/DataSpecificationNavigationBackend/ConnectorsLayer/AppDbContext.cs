using DataSpecificationNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace DataSpecificationNavigationBackend.ConnectorsLayer;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions options) : base(options) { }

	public DbSet<Conversation> Conversations { get; set; }

	public DbSet<Message> Messages { get; set; }
	public DbSet<UserMessage> UserMessages { get; set; }
	public DbSet<ReplyMessage> ReplyMessages { get; set; }

	public DbSet<DataSpecification> DataSpecifications { get; set; }
	public DbSet<DataSpecificationItem> DataSpecificationItems { get; set; }

	public DbSet<DataSpecificationItemMapping> DataSpecificationItemMappings { get; set; }

	public DbSet<DataSpecificationItemSuggestion> DataSpecificationItemSuggestions { get; set; }
}
