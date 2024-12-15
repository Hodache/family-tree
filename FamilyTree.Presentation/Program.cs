using FamilyTree.BLL.Interfaces;
using FamilyTree.BLL.Services;
using FamilyTree.DAL;
using FamilyTree.DAL.Repositories.Database;
using FamilyTree.DAL.Repositories.Interfaces;
using FamilyTree.Presentation.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddDbContext<FamilyTreeDatabaseContext>(options => 
        options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=familytreedb;Trusted_Connection=True;"));
builder.Services.AddScoped<IFamilyTreeRepository, FamilyTreeRepository>();
builder.Services.AddScoped<IFamilyTreeService, FamilyTreeService>();

builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
