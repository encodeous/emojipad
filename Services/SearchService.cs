using System;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Configuration;

namespace emojipad.Services
{
    public class SearchService
    {
        private readonly IConfiguration _configuration;
        private IndexWriter Index;

        public SearchService(IConfiguration configuration)
        {
            _configuration = configuration;
            var indexPath = Path.Combine(Environment.CurrentDirectory, "index");

            using var dir = FSDirectory.Open(indexPath);
            var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            var indexConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
            Index = new IndexWriter(dir, indexConfig);
        }
    }
}