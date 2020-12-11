using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using emojipad.Shared;
using Microsoft.Extensions.Configuration;

namespace emojipad.Services
{
    public class SearchService
    {
        private readonly IConfiguration _configuration;
        private readonly EmojiContext _context;
        public SearchService(IConfiguration configuration, EmojiContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public List<Emoji> GetFavouriteEmojis()
        {
            lock (_context)
            {
                return (from k in _context.Emojis
                    where k.UsedFrequency > 0
                    orderby k.UsedFrequency descending
                    select k).ToList();
            }
        }
        
        public List<Emoji> GetEmojis(string query)
        {
            lock (_context)
            {
                var selquery = (from k in _context.Emojis
                    where k.FileName.ToLower().Contains(query.ToLower())
                    orderby k.FileName
                    select k).ToList();
                return selquery;
            }
        }
    }
}