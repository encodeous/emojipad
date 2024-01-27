using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmojiPad.Models
{
    public class Emoji
    {
        [Key]
        public string EmojiName { get; set; }
        [JsonIgnore]
        public string Path { get; set; }
        [JsonIgnore]
        public int Order { get; set; }
        public int UsedFrequency { get; set; }
    }
}