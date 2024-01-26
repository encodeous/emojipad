using System;
using System.ComponentModel.DataAnnotations;

namespace EmojiPad.Models
{
    public class Emoji
    {
        [Key]
        public string EmojiName { get; set; }
        public int UsedFrequency { get; set; }
    }
}