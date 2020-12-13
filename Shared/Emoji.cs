using System.ComponentModel.DataAnnotations;

namespace emojipad.Shared
{
    public class Emoji
    {
        [Key]
        public string FileName { get; set; }
        public int UsedFrequency { get; set; }
    }
}