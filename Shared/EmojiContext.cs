using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace emojipad.Shared
{
    public class EmojiContext : DbContext
    {
        private readonly EmojiPadConfiguration _configuration;
        public EmojiContext(EmojiPadConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DbSet<Emoji> Emojis { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={Path.Join(_configuration.EmojiDatabasePath, "emoji-cache.db")}");
    }
}