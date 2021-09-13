using Microsoft.AspNetCore.Components;
using MudBlazorWithJwt.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MudBlazorWithJwt.Client.Pages.Personal
{
    public class TestBase : ComponentBase
    {
        protected List<Student> Students = new List<Student>();


        [Inject]
        public HttpClient HttpClient { get; set; }
        protected async Task GetRecords()
        {
            Students = await HttpClient.GetFromJsonAsync<List<Student>>("/api/Students");
        }
    }
}
