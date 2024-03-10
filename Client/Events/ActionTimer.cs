namespace Client.Events
{
    public class ActionTimer : IDisposable
    {
        private readonly System.Timers.Timer _timer;
        private readonly IAsyncActionTimeable _asyncActionTimeable;
        public ActionTimer(uint milliseconds, IAsyncActionTimeable asyncActionTimeable)
        {
            _timer = new System.Timers.Timer(milliseconds);
            _asyncActionTimeable = asyncActionTimeable;
            _timer.Elapsed += timerHasEnded!;
            _timer.Enabled = true;
            _timer.AutoReset = true;

        }

        private async void timerHasEnded(object source, System.Timers.ElapsedEventArgs e)
            => await _asyncActionTimeable.TimerEventAsync();

        public void Dispose()
            => _timer.Dispose();
    }

    public interface IAsyncActionTimeable
    {
        Task TimerEventAsync();
    }


}
