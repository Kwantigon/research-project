using DataSpecificationNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

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

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		var converter = new ValueConverter<DataSpecificationSubstructure, string>(
						v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
						v => JsonSerializer.Deserialize<DataSpecificationSubstructure>(v, (JsonSerializerOptions?)null)!);

		modelBuilder.Entity<Conversation>()
				.Property(c => c.DataSpecificationSubstructure)
				.HasConversion(converter)
				.HasColumnType("TEXT");

		modelBuilder.Entity<DataSpecificationItemSuggestion>()
				.HasOne(suggestion => suggestion.Item)
				.WithMany(dataSpecItem => dataSpecItem.ItemSuggestionsTable)
				.HasForeignKey(suggestion => new { suggestion.ItemIri, suggestion.ItemDataSpecificationId })
				.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<DataSpecificationItemSuggestion>()
				.HasOne(suggestion => suggestion.DomainItem)
				.WithMany()
				.HasForeignKey(suggestion => new { suggestion.DomainItemIri, suggestion.ItemDataSpecificationId })
				.OnDelete(DeleteBehavior.SetNull);
	}
}
