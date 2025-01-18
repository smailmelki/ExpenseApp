using CommunityToolkit.Maui;
using ExpenseApp.Classes;
using ExpenseApp.Models;
using Microsoft.Extensions.Logging;

namespace ExpenseApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            DBContext db = new DBContext();
            //db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            Tools.Load();
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
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
