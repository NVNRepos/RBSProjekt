using Client.Events;
using Client.Services;
using Domain.Tableau;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Client.Pages
{
    [Authorize]
    public partial class Tableau : IDisposable, IAsyncActionTimeable
    {

        public const uint REFRESH_TIME = 10 * 1000; // 10 Seconds

        [Inject]
        public ITableauService TableauService { get; set; } = null!;

        private EmployeePresentCollection _employeePresentModels = new();
        private ActionTimer _actionTimer = null!;

        protected override async Task OnInitializedAsync()
        {
            _employeePresentModels = await TableauService.GetEmployeePresentsAsync();
            _actionTimer = new ActionTimer(REFRESH_TIME, this);
            await base.OnInitializedAsync();
        }

        public async Task TimerEventAsync()
        {
            _employeePresentModels = await TableauService.GetEmployeePresentsAsync();
            StateHasChanged();
        }

        public void Dispose()
            => _actionTimer.Dispose();
    }
}
