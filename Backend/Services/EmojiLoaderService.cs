
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using emojipad.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace emojipad.Services
{
    [Route("api/load")]
    [ApiController]
    public class EmojiLoaderService : ControllerBase
    {
        private readonly EmojiContext _context;
        public EmojiLoaderService(EmojiContext context)
        {
            _context = context;
        }
        [HttpGet("{filename}")]
        public IActionResult LoadImage(string filename)
        {
            lock (_context)
            {
                var fi = new FileInfo(Path.Join(Utilities.GetEmojiFolderPath(), filename));
                var entity = _context.Emojis.Find(fi.Name);
                if (fi.Directory.FullName == Utilities.GetEmojiFolderPath() && entity != null)
                {
                    var fs = System.IO.File.OpenRead(fi.FullName);
                    if (new FileExtensionContentTypeProvider().TryGetContentType(fi.FullName, out var contentType))
                    {
                        return new FileStreamResult(fs, contentType);
                    }
                    return new FileStreamResult(fs, "application/octet-stream");
                }
                return new NotFoundResult();
            }
        }
        [HttpGet("{filename}/{maxsize}")]
        public IActionResult LoadImage(string filename, int maxsize)
        {
            lock (_context)
            {
                var fi = new FileInfo(Path.Join(Utilities.GetEmojiFolderPath(), filename));
                var entity = _context.Emojis.Find(fi.Name);
                if (fi.Directory.FullName == Utilities.GetEmojiFolderPath() && entity != null)
                {
                    Image img = Image.Load(fi.FullName, out var format);
                    img.Mutate(x =>
                    {
                        x.Resize(new ResizeOptions()
                        {
                            Size = Utilities.GetImageBounds(img.Size(), maxsize),
                            Sampler = KnownResamplers.Lanczos3
                        });
                    });
                    var ms = new MemoryStream();
                    img.Save(ms, format);
                    ms.Position = 0;
                    return new FileStreamResult(ms, format.DefaultMimeType);
                }
                return new NotFoundResult();
            }
        }
    }
}