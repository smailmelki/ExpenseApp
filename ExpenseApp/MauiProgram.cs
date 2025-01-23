using CommunityToolkit.Maui;
using ExpenseApp.Classes;
using ExpenseApp.Models;
using ExpenseApp.Pages;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;

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
                })
               .UseLocalNotification(config =>
               {
                   config.AddCategory(new NotificationCategory(NotificationCategoryType.Status)
                   {
                       ActionList =
                           [
                               new(100)
                            {
                                Title = "Hello",
                                Android =
                                {
                                    LaunchAppWhenTapped = true,
                                    IconName =
                                    {
                                        ResourceName = "i2"
                                    }
                                },
                                IOS =
                                {
                                    Action = iOSActionType.Foreground
                                },
                                Windows =
                                {
                                    LaunchAppWhenTapped = true
                                }
                            },
                            new(101)
                            {
                                Title = "Close",
                                Android =
                                {
                                    PendingIntentFlags = AndroidPendingIntentFlags.CancelCurrent,
                                    LaunchAppWhenTapped = false,
                                    IconName =
                                    {
                                        ResourceName = "i3"
                                    }
                                },
                                IOS =
                                {
                                    Action = iOSActionType.Destructive
                                },
                                Windows =
                                {
                                    LaunchAppWhenTapped = false
                                }
                            }
                           ]
                   })
                   .AddAndroid(android =>
                   {
                       android.AddChannel(new NotificationChannelRequest
                       {
                           Sound = "good_things_happen"
                       });
                   })
                   .AddiOS(iOS =>
                   {
                   });
               });

#if DEBUG
            LocalNotificationCenter.LogLevel =LogLevel.Debug;
            builder.Logging.AddDebug();
#endif
            builder.Services.AddTransient<SettingPage>();

            return builder.Build();
        }
    }
}