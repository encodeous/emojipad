using EmojiPad.Data;

namespace EmojiPad.Services
{
    public class EventService
    {
        public delegate void EmojiChangedDelegate(Emoji emote);

        public EmojiChangedDelegate ItemBarUpdate;
        
        public delegate void EmojiDatabaseChange();

        public event EmojiDatabaseChange RefreshEmojis;

        public void InvokeRefreshEmojis()
        {
            RefreshEmojis?.Invoke();
        }

        public delegate void EmojiCopiedDelegate(Emoji emote);

        public event EmojiCopiedDelegate EmojiCopiedEvent;

        public void CopyEmoji(Emoji emote)
        {
            EmojiCopiedEvent?.Invoke(emote);
        }

        public delegate void SearchQueryChangedDelegate(string newQuery);

        public event SearchQueryChangedDelegate QueryChangedEvent;

        public void ChangeQuery(string query)
        {
            QueryChangedEvent?.Invoke(query);
        }
        
        public delegate void SaveConfigurationDelegate();

        public event SaveConfigurationDelegate ConfigurationSaveEvent;

        public void SaveConfig()
        {
            ConfigurationSaveEvent?.Invoke();
        }
        
        public delegate void SetBusyDelegate(int busyMS = 300);

        public SetBusyDelegate Busy;

        public void SetBusy(int busyMS = 300)
        {
            Busy?.Invoke(busyMS);
        }
    }
}