using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace emojipad.Services
{
    public class SearchService
    {
        private readonly IConfiguration _configuration;

        public SearchService(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }
    }
}