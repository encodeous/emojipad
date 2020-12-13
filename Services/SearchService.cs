using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using emojipad.Shared;
using Microsoft.Extensions.Configuration;

namespace emojipad.Services
{
    public class SearchService
    {
        private readonly EmojiPadConfiguration _configuration;
        private readonly EmojiContext _context;
        private readonly EventService _event;
        public SearchService(EmojiPadConfiguration configuration, EmojiContext context, EventService events)
        {
            _configuration = configuration;
            _context = context;
            _event = events;
        }

        public List<Emoji> GetFavouriteEmojis()
        {
            lock (_context)
            {
                _event.SetBusy();
                return (from k in _context.Emojis
                    where k.UsedFrequency > 0
                    orderby k.UsedFrequency descending
                    select k).Take(_configuration.FrequentEmojiCount).ToList();
            }
        }
        
        
        public List<Emoji> GetEmojis(string query)
        {
            lock (_context)
            {
                _event.SetBusy();
                try
                {
                    if (_configuration.RegexSearch)
                    {
                        var reg = new Regex(query);
                        return (from k in _context.Emojis.AsEnumerable()
                            where reg.IsMatch(k.FileName)
                            orderby k.FileName
                            select k).ToList();
                    }
                    else
                    {
                        return (from k in _context.Emojis
                            where k.FileName.ToLower().Contains(query.ToLower())
                            orderby k.FileName
                            select k).ToList();
                    }
                }
                catch
                {
                    return new List<Emoji>();
                }
            }
        }
    }
}