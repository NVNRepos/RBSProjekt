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
        [Inject]
        public ITableauService TableauService { get; set; } = null!;

        private EmployeePresentCollection _employeePresentModels = new();
        private IActionTimer _actionTimer = null!;

        protected override async Task OnInitializedAsync()
        {
            _employeePresentModels = await TableauService.GetEmployeePresentsAsync();
            _actionTimer = this.CreateActionTimer();
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
