using DataSpecificationNavigationBackend.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

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

	public DbSet<DataSpecificationPropertySuggestion> DataSpecificationItemSuggestions { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		/*JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
		{
			Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
			WriteIndented = true
		};
		var converter = new ValueConverter<DataSpecificationSubstructure, string>(
						v => JsonSerializer.Serialize(v, serializerOptions),
						v => JsonSerializer.Deserialize<DataSpecificationSubstructure>(v, serializerOptions));

		modelBuilder.Entity<Conversation>()
				.Property(c => c.DataSpecificationSubstructure)
				.HasConversion(converter)
				.HasColumnType("TEXT");*/

		modelBuilder.Entity<DataSpecificationPropertySuggestion>()
				.HasOne(suggestion => suggestion.Item)
				.WithMany(dataSpecItem => dataSpecItem.ItemSuggestionsTable)
				.HasForeignKey(suggestion => new { suggestion.ItemIri, suggestion.ItemDataSpecificationId })
				.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<DataSpecificationPropertySuggestion>()
				.HasOne(suggestion => suggestion.DomainItem)
				.WithMany()
				.HasForeignKey(suggestion => new { suggestion.DomainItemIri, suggestion.ItemDataSpecificationId })
				.OnDelete(DeleteBehavior.SetNull);

		modelBuilder.Entity<DataSpecificationItem>()
			.HasOne(item => item.DomainItem)
			.WithMany()
			.HasForeignKey(item => new { item.DomainItemIri, item.DataSpecificationId })
			.OnDelete(DeleteBehavior.SetNull);

		/*modelBuilder.Entity<DataSpecificationItem>()
			.HasOne(item => item.RangeItem)
			.WithMany()
			.HasForeignKey(item => new { item.RangeItemIri, item.DataSpecificationId })
			.OnDelete(DeleteBehavior.SetNull);*/
	}
}
