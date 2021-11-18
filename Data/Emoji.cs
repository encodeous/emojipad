using SQLite;
using System;
using System.ComponentModel.DataAnnotations;

namespace EmojiPad.Data
{
    [Table("Emojis")]
    public class Emoji
    {
        [PrimaryKey]
        [Column("emoji_name")]
        public string EmojiName { get; set; }
        [Column("frequency")]
        public int UsedFrequency { get; set; }
        [Column("file_hash")]
        public string FileHash { get; set; }
        [Column("file_id")]
        public string EmojiCacheId { get; set; }
        [Column("paste_size")]
        public int PasteSize { get; set; }
    }
}