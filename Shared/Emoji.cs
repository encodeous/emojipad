using System.ComponentModel.DataAnnotations;

namespace emojipad.Shared
{
    public class Emoji
    {
        [Key]
        public string RelativeFilePath { get; set; }
        public string FileName { get; set; }
        public string FolderName { get; set; }
        public int UsedFrequency { get; set; }
    }
}