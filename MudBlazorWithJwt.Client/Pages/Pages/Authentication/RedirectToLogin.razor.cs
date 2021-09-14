using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MudBlazorWithJwt.Client.Pages.Pages.Authentication
{
    public partial class RedirectToLogin
    {

        [Inject]
        public NavigationManager NavigationManager{ get; set; }

        protected override void OnInitialized()
        {
             NavigationManager.NavigateTo("pages/authentication/login");
           // NavigationManager. NavigateTo(
            // $"authentication/login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}");
        }
    }
}
