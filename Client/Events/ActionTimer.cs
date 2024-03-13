namespace Client.Events
{
    internal class ActionTimer : IActionTimer
    {
        private readonly System.Timers.Timer _timer;
        private readonly IAsyncActionTimeable _asyncActionTimeable;

        public bool Repeat { get => _timer.AutoReset; set => _timer.AutoReset = value; }

        public uint Milliseconds { get;}

        public ActionTimer(IAsyncActionTimeable asyncActionTimeable, uint milliseconds, bool repeat = true)
        {
            Milliseconds = milliseconds;
            _timer = new System.Timers.Timer(Milliseconds);
            _asyncActionTimeable = asyncActionTimeable;
            _timer.Elapsed += timerHasEnded!;
            _timer.Enabled = true;
            _timer.AutoReset = repeat;

        }

        private async void timerHasEnded(object source, System.Timers.ElapsedEventArgs e)
            => await _asyncActionTimeable.TimerEventAsync();

        public void Dispose() => _timer.Dispose();

        public Task InvokeEvent() => _asyncActionTimeable.TimerEventAsync();

    }

    public interface IActionTimer : IDisposable{

        public bool Repeat { get; set;}

        public uint Milliseconds { get; }

        public Task InvokeEvent();
    }

    public interface IAsyncActionTimeable
    {
        Task TimerEventAsync();
    }

    public static class AsyncActionTimeable{

        public const uint DEFAULT_REFRESH_TIME = 10 * 1000; // 10 Seconds
        public static IActionTimer CreateActionTimer(this IAsyncActionTimeable asyncActionTimeable, uint milliseconds = DEFAULT_REFRESH_TIME, bool repeat = true)
            => new ActionTimer(asyncActionTimeable, milliseconds, repeat); 
    }


}
