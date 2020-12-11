using System;
using System.Threading.Tasks;

namespace emojipad.Services
{
    public class StatusService
    {
        public delegate void ChangeColor(string color);

        public ChangeColor ColorChangeDelegate;

        private volatile bool IsBusy = false;

        private DateTime StopTime;
        
        public void Change(string color)
        {
            ColorChangeDelegate?.Invoke(color);
        }

        public void SetBusy(int ms = 1000)
        {
            if (!IsBusy)
            {
                IsBusy = true;
                StopTime = DateTime.Now + TimeSpan.FromMilliseconds(ms);
                Change("f1c40f");
                Task.Run(async () =>
                {
                    while (DateTime.Now < StopTime)
                    {
                        await Task.Delay(StopTime - DateTime.Now);
                    }
                    Change("2ecc71");
                    IsBusy = false;
                });
            }

        }
    }
}