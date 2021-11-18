using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EmojiPad.Data;
using EmojiPad.Utils;
using Microsoft.Extensions.Configuration;
using SQLite;

namespace EmojiPad.Services
{
    public class SearchService
    {
        private readonly EmojiPadConfiguration _configuration;
        private readonly EventService _event;
        public SearchService(EmojiPadConfiguration configuration, EventService events)
        {
            _configuration = configuration;
            _event = events;
        }

        public List<Emoji> GetFavouriteEmojis()
        {
            var _context = Utilities.GetService<SQLiteConnection>().Table<Emoji>();
            _event.SetBusy();
            var ans = (from k in _context
                    where k.UsedFrequency > 0
                    orderby k.UsedFrequency descending
                    select k).Take(_configuration.FrequentEmojiCount).ToList();
            return ans;
        }
        
        public List<Emoji> GetEmojis(string query)
        {
            var _context = Utilities.GetService<SQLiteConnection>().Table<Emoji>();
            _event.SetBusy();
            try
            {
                if (_configuration.RegexSearch)
                {
                    var reg = new Regex(query);
                    return (from k in _context
                            where reg.IsMatch(k.EmojiName)
                            orderby k.EmojiName
                            select k).ToList();
                }
                else
                {
                    return (from k in _context
                            where k.EmojiName.ToLower().Contains(query.ToLower())
                            orderby k.EmojiName
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