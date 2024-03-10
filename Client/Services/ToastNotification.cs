
namespace Client.Services {

    // https://chrissainty.com/blazor-toast-notifications-using-only-csharp-html-css/
    /// <summary>
    /// Service that activates Toastnotifications on the Pages
    /// </summary>
    public class ToastNotification: IDisposable {

        /// <summary>
        /// OnShow event
        /// </summary>
        public event Action<string, ToastLevel>? OnShow;
        /// <summary>
        /// OnHide event
        /// </summary>
        public event Action? OnHide;

        private System.Timers.Timer? Countdown;

        /// <summary>
        /// Shows a toast notification
        /// </summary>
        /// <param name="message">Message of the Toast</param>
        /// <param name="level">Toastlevel</param>
        public void ShowToast( string message, ToastLevel level ) {
            OnShow?.Invoke( message, level );
            StartCountdown();
        }

        public void ShowInformation( string message )
            => ShowToast( message, ToastLevel.Info );

        public void ShowWarning( string message )
            => ShowToast( message, ToastLevel.Warning );

        public void ShowError( string message )
            => ShowToast( message, ToastLevel.Error );

        public void ShowSuccess( string message )
            => ShowToast( message, ToastLevel.Success );

        private void StartCountdown() {
            SetCountdown();

            if( Countdown!.Enabled ) {
                Countdown.Stop();
                Countdown.Start();
            } else {
                Countdown!.Start();
            }
        }

        private void SetCountdown() {
            if( Countdown != null ) return;

            Countdown = new System.Timers.Timer( 5000 );
            Countdown.Elapsed += HideToast;
            Countdown.AutoReset = false;
        }

        private void HideToast( object? source, System.Timers.ElapsedEventArgs args )
            => OnHide?.Invoke();

        public void Dispose()
            => Countdown?.Dispose();
    }

    /// <summary>
    /// Level that indicates the type of the toast
    /// </summary>
    public enum ToastLevel {
        /// <summary> Information </summary>
        Info,
        /// <summary> Success </summary>
        Success,
        /// <summary> Warning </summary>
        Warning,
        /// <summary> Error  </summary>
        Error
    }
}
