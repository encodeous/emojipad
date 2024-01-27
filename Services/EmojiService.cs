using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using EmojiPad.Models;
using System.IO;
using System.Security.Cryptography;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using ImageMagick;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Runtime.Intrinsics.Arm;

namespace EmojiPad.Services
{
    public class EmojiService
    {
        public void StateChanged()
        {
            AppService.Instance.State.MainWindow.DispatcherQueue.TryEnqueue(StateChangedSameThread);
        }

        public void StateChangedSameThread()
        {
            int cnt = 1;
            AppService.Instance.State.DisplayedEmojis.Clear();
            List<Emoji> emojis = new List<Emoji>();
            foreach (var emoji in AppService.Instance.State.Index.Emojis.Values.AsEnumerable()
                         .OrderByDescending(x=>x.UsedFrequency)
                         .ThenBy(x=>x.EmojiName))
            {
                if (emoji.EmojiName.ToLower().Contains(AppService.Instance.State.Query.ToLower()))
                {
                    emojis.Add(emoji);
                    bool queryPrefix = emoji.EmojiName.ToLower().StartsWith(AppService.Instance.State.Query.ToLower());
                    if (!queryPrefix || AppService.Instance.State.Query == "")
                        emoji.Order = cnt++;
                    else emoji.Order = 0;
                }
            }
            emojis.Sort((x, y) => x.Order - y.Order);
            foreach (var emoji in emojis)
            {
                AppService.Instance.State.DisplayedEmojis.Add(emoji);
            }
        }

        public void CopyEmoji(string name)
        {
            var emoji = AppService.Instance.State.DisplayedEmojis.FirstOrDefault(x=>x.EmojiName == name);
            if (emoji is not null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var dp = new DataPackage();
                        await LoadEmoji(emoji, dp);
                        emoji.UsedFrequency++;
                        var indexPath = AppService.Instance.Config.EmojiFolder + "\\index.json";
                        AppService.Instance.State.Index.Save(new FileInfo(indexPath));
                        AppService.Instance.State.MainWindow.DispatcherQueue.TryEnqueue(() =>
                        {
                            if (AppService.Instance.Config.HideAfterCopy)
                            {
                                AppService.Instance.State.MainWindow.HidePicker();
                            }
                            Clipboard.SetContent(dp);
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{ex} {ex.Message}");
                    }
                });
                
            }
        }

        private async Task LoadEmoji(Emoji emoji, DataPackage dp)
        {
            var tmpDir = Path.Join(Path.GetTempPath(), "emojipad");

            if (!Directory.Exists(tmpDir))
                Directory.CreateDirectory(tmpDir);
            var frs = File.OpenRead(emoji.Path);
            var hash = BitConverter.ToString(await MD5.HashDataAsync(frs)).Replace("-", "")[..15];
            frs.Close();
            var cachePath = Path.Join(Path.GetTempPath(), "emojipad", "em-" + hash + "-" + AppService.Instance.Config.CropSize + "-" + emoji.EmojiName);
            if (!File.Exists(cachePath))
            {
                if (new FileInfo(emoji.Path).Extension == ".gif")
                {
                    await ResizeGif(emoji.Path, cachePath);
                }
                else
                {
                    File.Create(cachePath).Close();
                    await ResizeImage(new FileInfo(emoji.Path).FullName, cachePath);
                }
            }

            await Task.Delay(200);

            var output = await StorageFile.GetFileFromPathAsync(cachePath);

            dp.SetStorageItems([output]);
        }

        public async Task ResizeGif(string gif, string output)
        {
            // Read from file
            using var collection = new MagickImageCollection(new FileInfo(gif));

            // This will remove the optimization and change the image to how it looks at that point
            // during the animation. More info here: http://www.imagemagick.org/Usage/anim_basics/#coalesce
            collection.Coalesce();

            foreach (var image in collection)
            {
                image.Settings.Compression = CompressionMethod.NoCompression;
                image.Settings.AntiAlias = true;
                var bounds = GetImageBounds((image.BaseWidth, image.BaseHeight), AppService.Instance.Config.CropSize);
                //image.Resize(bounds.Item1, bounds.Item2);
                image.InterpolativeResize(bounds.Item1, bounds.Item2, PixelInterpolateMethod.Catrom);
            }

            // Save the result
            await collection.WriteAsync(output);
        }

        public async Task ResizeImage(string img, string outFile)
        {
            var input = await StorageFile.GetFileFromPathAsync(img);
            var output = await StorageFile.GetFileFromPathAsync(outFile);
            BitmapDecoder bd = await BitmapDecoder.CreateAsync(
                await input.OpenReadAsync());

            var outStr = await output.OpenAsync(FileAccessMode.ReadWrite);

            var encoderId = new FileInfo(img).Extension switch
            {
                ".png" => BitmapEncoder.PngEncoderId,
                ".jpg" or "jpeg" => BitmapEncoder.JpegEncoderId,
                _ => throw new ArgumentOutOfRangeException()
            };

            var be = await BitmapEncoder.CreateAsync(
                encoderId, outStr);
            var bounds = GetImageBounds((bd.PixelWidth, bd.PixelHeight), AppService.Instance.Config.CropSize);

            be.BitmapTransform.ScaledHeight = bounds.Item2;
            be.BitmapTransform.ScaledWidth = bounds.Item1;
            be.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant;
            be.SetSoftwareBitmap(await bd.GetSoftwareBitmapAsync(bd.BitmapPixelFormat, bd.BitmapAlphaMode));
            await be.FlushAsync();
            outStr.Dispose();
        }

        public (uint,uint) GetImageBounds((uint, uint) imgSize, int maxBounds)
        {
            uint maxDim = Math.Max(imgSize.Item1, imgSize.Item2);
            decimal val = (decimal)maxBounds / maxDim;
            return new ((uint)Math.Round(imgSize.Item1 * val), (uint)Math.Round(imgSize.Item2 * val));
        }

        public (int, int) GetImageBounds((int, int) imgSize, int maxBounds)
        {
            int maxDim = Math.Max(imgSize.Item1, imgSize.Item2);
            decimal val = (decimal)maxBounds / maxDim;
            return new((int)Math.Round(imgSize.Item1 * val), (int)Math.Round(imgSize.Item2 * val));
        }
    }
}
