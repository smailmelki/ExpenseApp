using CommunityToolkit.Maui;
using ExpenseApp.Models;
using Microsoft.Extensions.Logging;
using MauiIcons.Material.Outlined;

namespace ExpenseApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            DBContext db = new DBContext();
            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMaterialOutlinedMauiIcons()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
