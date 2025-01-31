// Copyright 2023 - 2023 Godwin peter .O (me@godwin.dev)
//
// Licensed under the MIT License;
// you may not use this file except in compliance with the License.
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Reflection;
using Axolotl.AspNet;
using Axolotl.EFCore;
using DotNetEd.CoreAdmin;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using XClaim.Common.Context;
using XClaim.Service.Data;
using XClaim.Service.Helpers;

namespace XClaim.Service.Extensions;

public static class ServiceDefaultExtensions {
    public static IServiceCollection RegisterDefaultService(this IServiceCollection services) {
        services.AddControllersWithViews();
        services.AddRazorPages();
        services.RegisterDataContext();
        services.RegisterGenericRepositories(typeof(GenericRepository<>));
        services.RegisterGenericServices();
        services.RegisterFeatures(typeof(Program).Assembly);
        services.AddHostedService<MigrationService>();
        services.AddCoreAdmin(new CoreAdminOptions {
            Title = "x-claim",
            ShowPageSizes = true,
            PageSizes = new Dictionary<int, string>() {
                { 25, "25" },
                { 100, "100" },
                { 0, "All" }
            },
            IgnoreEntityTypes = new List<Type> ()
        });

        // TODO: Review dependency injection
        services.AddSingleton<HttpClient>(sp => {
            var server = sp.GetRequiredService<IServer>();
            var addressFeature = server.Features.Get<IServerAddressesFeature>();
            string baseAddress = addressFeature!.Addresses.First();
            return new HttpClient { BaseAddress = new Uri(baseAddress) };
        });

        services.AddTransient<FileUploadService>();

        return services;
    }

    public static WebApplication RegisterDefaultService(this WebApplication app) {
        if (app.Environment.IsDevelopment())
            app.UseWebAssemblyDebugging();
        else app.UseExceptionHandler("/Error");

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapDefaultControllerRoute();
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();
        app.UseSession();
        app.MapRazorPages();
        app.MapControllers();
        app.MapFallbackToFile("index.html");
        app.RegisterFeatureEndpoints();
        app.UseCoreAdminCustomUrl("admin");
        app.UseCoreAdminCustomAuth((_) => Task.FromResult(true));

        app.MapPost("/sample-upload-files", ([FromForm] FileAccess check, IFormFileCollection files, [FromServices] ILogger<Program> logger) => {
            var name = check.Name;
            var count = check.Count;

            logger.LogInformation("<<======>> :: {Name} :: {Count} ---> Files: {Counts}", name, count, files.Count);
            return Results.Ok(name);
        }).Accepts<FileAccess>("multipart/form-data");

        return app;
    }
}

class FileAccess {
    public static ValueTask<FileAccess?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        int.TryParse(httpContext.Request.Form["Count"], out var count);
        return ValueTask.FromResult<FileAccess?>(
            new FileAccess {
                Count = count,
                Name = httpContext.Request.Form["Name"]
            }
        );
    }
    public int Count { get; set; }
    public string? Name { get; set; }
}