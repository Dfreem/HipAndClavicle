using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf.Models;

namespace HIPNunitTests.Fakes
{
    public class FakeNotyfService : INotyfService
    {
        public void Custom(string message, int durationInSeconds, string backgroundColor, bool isSlide = false) { }

        public void Custom(string message, int? durationInSeconds = null, string backgroundColor = "black", string iconClassName = "home") { }

        public void Error(string message, int durationInSeconds = 5) { }

        public void Error(string message, int? durationInSeconds = null) { }

        public IEnumerable<NotyfNotification> GetNotifications()
        {
            return new List<NotyfNotification>();
        }

        public void Information(string message, int durationInSeconds = 5) { }

        public void Information(string message, int? durationInSeconds = null) { }

        public IEnumerable<NotyfNotification> ReadAllNotifications()
        {
            return new List<NotyfNotification>();
        }

        public void RemoveAll() { }

        public void Success(string message, int durationInSeconds = 5) { }

        public void Success(string message, int? durationInSeconds = null) { }

        public void Warning(string message, int durationInSeconds = 5) { }

        public void Warning(string message, int? durationInSeconds = null) { }
    }
}

