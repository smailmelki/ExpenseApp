using System.Globalization;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ExpenseApp.Classes;
using ExpenseApp.ItemsView;
using ExpenseApp.Models;
using ExpenseApp.Resources.languag;
using Microsoft.EntityFrameworkCore;
using Plugin.LocalNotification;

namespace ExpenseApp.Pages;

public partial class SettingPage : ContentPage
{
    string NotifyTime = string.Empty;
    int _tapCount;
    private readonly INotificationService _notificationService;

    public SettingPage(INotificationService notificationService)
	{
		InitializeComponent();
        _notificationService = notificationService;
        FillCurrencyPicker();
        GetDefault();
        if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            FlowDirection = FlowDirection.RightToLeft;
        else
            FlowDirection = FlowDirection.LeftToRight;
    }

    private void FillCurrencyPicker()
    {
        // قائمة العملات مع رموزها
        var currencies = new List<Currency>
        {
            new Currency { Name = AppResource.Cur_DZD, Symbol = AppResource.Sym_DZD, Culture = "ar-DZ" },
            new Currency { Name = AppResource.Cur_SAR, Symbol = "﷼", Culture = "ar-SA" },
            new Currency { Name = AppResource.Cur_USD, Symbol = "$", Culture = "en-US" },
            new Currency { Name = AppResource.Cur_euro, Symbol = "€", Culture = "fr-FR" },
            new Currency { Name = AppResource.Cur_GBP, Symbol = "£", Culture = "en-GB" },
            new Currency { Name = AppResource.Cur_JPY, Symbol = "¥", Culture = "ja-JP" }
        };


        // ربط القائمة بـ Picker
        CurrencyPicker.ItemsSource = currencies;
    }
    private void GetDefault()
    {
        btnAr.BackgroundColor = Tools.Long == "ar" ? Colors.Orange : Colors.Transparent;
        btnEn.BackgroundColor = Tools.Long == "en" ? Colors.Orange : Colors.Transparent;
        SwMode.IsToggled = Tools.Mode == "Dark";
        Application.Current.UserAppTheme = Tools.Mode == "Dark" ? AppTheme.Dark : AppTheme.Light;
        txtName.Text = Tools.Name;
        txtAmount.Text = Tools.Amount;
        CurrencyPicker.SelectedIndex = CurrencyPicker.ItemsSource.Cast<Currency>().ToList().FindIndex(c => c.Name == Tools.Caruncy);
        SwNotify.IsToggled = Tools.Notify;
        lblCaruncy2.Text = lblCaruncy.Text;
        btn3.BackgroundColor = Tools.NotifyTime == btn3.Text ? Color.FromArgb("#FFFFFF") : Colors.Gray;
        btn6.BackgroundColor = Tools.NotifyTime == btn6.Text ? Color.FromArgb("#FFFFFF") : Colors.Gray;
        btn12.BackgroundColor = Tools.NotifyTime == btn12.Text ? Color.FromArgb("#FFFFFF") : Colors.Gray;

        btn3.TextColor = Tools.NotifyTime == btn3.Text ? Colors.Black : Colors.White;
        btn6.TextColor = Tools.NotifyTime == btn6.Text ? Colors.Black : Colors.White;
        btn12.TextColor = Tools.NotifyTime == btn12.Text ? Colors.Black : Colors.White;
    }

    // معالج حدث اختيار العملة
    private void OnCurrencySelected(object sender, EventArgs e)
    {
        var selectedCurrency = (Currency)CurrencyPicker.SelectedItem;
        if (selectedCurrency != null)
        {
            lblCaruncy.Text = selectedCurrency.Symbol;
        }
    }

    private async void btnItems_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CatigoryPage());
    }

    private void SwitchMode_Toggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            Tools.Mode = "Dark";
            Application.Current.UserAppTheme = AppTheme.Dark;
        }
        else
        {
            Tools.Mode = "Light";
            Application.Current.UserAppTheme = AppTheme.Light;
        }
        Tools.SaveMode();
    }

    private async void btnName_Clicked(object sender, EventArgs e)
    {
        Tools.Name = txtName.Text;
        Tools.SaveName();
        await Toast.Make(AppResource.msg_Saved, ToastDuration.Short, 14).Show();
    }

    private async void btnAmount_ClickedAsync(object sender, EventArgs e)
    {
        Tools.Amount = txtAmount.Text;
        if (CurrencyPicker.SelectedItem != null)
        {
            if (CurrencyPicker.SelectedItem is Currency currency)
            {
                Tools.Caruncy = currency.Name;
                Tools.MyCultureInfo = currency.Culture;
            }
        }
        Tools.SaveAmount();
        await Toast.Make(AppResource.msg_Saved, ToastDuration.Short, 14).Show();
    }

    /// <summary>
    /// إنشاء نسخة احتياطية من قاعدة البيانات
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnBackUp_Clicked(object sender, EventArgs e)
    {
        //Get user to pick backup location
        string? backupFolder = await SqliteBackupManager.PickBackupFolderAsync();
        if (string.IsNullOrEmpty(backupFolder))
        {
            await Toast.Make(AppResource.msg_BackUp1).Show();
            return;
        }
        string backupPath = Path.Combine(backupFolder, GenerateBackupFileName());
        using (var context = new DBContext())
        {
            string databasePath = context.Database.GetDbConnection().DataSource;
            bool backupDone = await SqliteBackupManager.BackupDatabaseAsync(databasePath, backupPath);

            if (backupDone)
            {
                await Toast.Make(AppResource.msg_BackUp2).Show();
            }
            else
            {
                await Toast.Make(AppResource.Msg_BackUp3).Show();
            }
        }
    }
    /// <summary>
    /// استعادة قاعدة بيانات من نسخة احتياطية
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void btnRestor_Clicked(object sender, EventArgs e)
    {
        // اختيار ملف النسخة الاحتياطية
        var backupPath = await SqliteBackupManager.PickBackupFileAsync();
        string DatabasePath;
        using (var db = new DBContext())
        {
            DatabasePath = db.Database.GetDbConnection().DataSource; // مسار قاعدة البيانات الأصلية
        }

        if (string.IsNullOrEmpty(backupPath))
        {
            await Toast.Make(AppResource.msg_Restor1).Show();
            return;
        }

        try
        {
            ///////////////////////////////

            if (SchemaComparer.CompareSchemas(backupPath, DatabasePath))
            {
                // استعادة قاعدة البيانات
                bool RestoreDone = await SqliteBackupManager.BackupDatabaseAsync(backupPath, DatabasePath);
                if (RestoreDone)
                {
                    await Toast.Make(AppResource.msg_Restor2).Show();
                    // إعادة إنشاء الصفحة الرئيسية لتطبيق التغييرات
                    App.Current.MainPage = new AppShell();
                }
                else
                {
                    await Toast.Make(AppResource.msg_Restore3).Show();
                }
            }   
            else
            {
                await Toast.Make(AppResource.msg_Restor4).Show();
            }
            ///////////////////////////////

        }
        catch (Exception ex)
        {
            await Toast.Make(AppResource.msg_Restor5).Show();
        }
    }

    /// <summary>
    /// يولد اسم ملف نسخة احتياطية جديدة بناءً على التاريخ والوقت الحاليين.
    /// </summary>
    /// <returns>اسم ملف النسخة الاحتياطية</returns>
    private static string GenerateBackupFileName()
    {
        return $"backup_Exp_{DateTime.Now:yyyyMMdd_HHmmss}.db";
    }

    private void btnAr_Clicked(object sender, EventArgs e)
    {
        btnAr.BackgroundColor = Colors.Orange;
        btnEn.BackgroundColor = Colors.Transparent;
        Tools.Long = "ar";
        Tools.SaveLong();
        ChangeLanguage("ar");
    }

    private void btnEn_Clicked(object sender, EventArgs e)
    {
        btnEn.BackgroundColor = Colors.Orange;
        btnAr.BackgroundColor = Colors.Transparent;
        Tools.Long = "en";
        Tools.SaveLong();
        ChangeLanguage("en");
    }

    public void ChangeLanguage(string languageCode)
    {
        var culture = new CultureInfo(languageCode);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        // إعادة إنشاء الصفحة الرئيسية لتطبيق التغييرات
        App.Current.MainPage = new AppShell();
    }


    private void SwNotify_Toggled(object sender, ToggledEventArgs e)
    {
        btn3.IsEnabled = btn6.IsEnabled = btn12.IsEnabled = SwNotify.IsToggled;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        btn3.BackgroundColor =
        btn6.BackgroundColor =
        btn12.BackgroundColor =
        Colors.Gray;

        btn3.TextColor =
        btn6.TextColor =
        btn12.TextColor =
        Colors.White;

        var btn = sender as Button;
        btn.BackgroundColor = Color.FromArgb("#FFFFFF");
        btn.TextColor = Colors.Black;
        NotifyTime = btn.Text;
    }

    private async void btnNotify_ClickedAsync(object sender, EventArgs e)
    {
        // التحقق من اختيار وقت التنبيه إذا كان التبديل مفعلاً
        if (SwNotify.IsToggled && NotifyTime == null)
        {
            await Toast.Make(AppResource.msg_NotifyTime, ToastDuration.Short, 14).Show();
            return;
        }

        // حفظ الإعدادات
        Tools.Notify = SwNotify.IsToggled;
        Tools.NotifyTime = NotifyTime;
        Tools.SaveNotify();

#if ANDROID
        // إلغاء جميع الإشعارات الحالية
        _notificationService.CancelAll();

        if (SwNotify.IsToggled)
        {
            // تحديد عدد الساعات بناءً على اختيار المستخدم
            double Hours = NotifyTime == btn3.Text ? 3 :
                        NotifyTime == btn6.Text ? 6 :
                        NotifyTime == btn12.Text ? 12 : 0;

            if (Hours > 0)
            {
                // جدولة الإشعار بناءً على الوقت المختار
                ScheduleNotificationEveryHours(Hours);
            }
        }
#endif

        // عرض رسالة تأكيد الحفظ
        await Toast.Make(AppResource.msg_Saved, ToastDuration.Short, 14).Show();
    }

    #region Notify
    private async Task<byte[]> GetImageBytesAsync(string fileName)
    {
        var imageStream = await FileSystem.OpenAppPackageFileAsync(fileName);
        if (imageStream == null) return Array.Empty<byte>();

        using var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms);
        return ms.ToArray();
    }

    public async Task ShowNotify()
    {
        var imageBytes = await GetImageBytesAsync("appicon1.png");
        _tapCount++;

        var request = new NotificationRequest
        {
            NotificationId = 100 + _tapCount,
            Title = AppResource.lblreminder,
            Subtitle = $"Tap Count: {_tapCount}",
            Description = $"Tap Count: {_tapCount}",
            BadgeNumber = _tapCount,
            Image = { Binary = imageBytes },
            CategoryType = NotificationCategoryType.Status
        };

        try
        {
            if (!await _notificationService.AreNotificationsEnabled())
            {
                bool granted = await _notificationService.RequestNotificationPermission();
                if (!granted)
                {
                    await DisplayAlert("Permission Denied", "Notifications are not enabled. Please enable them from settings.", "OK");
                    return;
                }
            }

            await _notificationService.Show(request);
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Failed to show notification. Please try again.", "OK");
        }
    }

    private async void ScheduleNotificationEveryHours(double hoursN)
    {
        // إنشاء معرف فريد للإشعار
        var notificationId = (int)DateTime.Now.Ticks;
        // إعداد الإشعار
        var notification = new NotificationRequest
        {
            NotificationId = notificationId,
            CategoryType = NotificationCategoryType.Alarm,
            Title = AppResource.lblreminder,
            Description = AppResource.msgreminder,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(3), // وقت بدء الإشعار
                RepeatType = NotificationRepeat.TimeInterval,
                NotifyRepeatInterval = TimeSpan.FromHours(hoursN) // تكرار الإشعار كل عدد الساعات المحددة
            }
        };

        // عرض الإشعار
        try
        {
            if (!await _notificationService.AreNotificationsEnabled())
            {
                bool granted = await _notificationService.RequestNotificationPermission();
                if (!granted)
                {
                    await DisplayAlert("Permission Denied", "Notifications are not enabled. Please enable them from settings.", "OK");
                    return;
                }
            }

            await _notificationService.Show(notification);
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Failed to show notification. Please try again.", "OK");
        }
    }


    #endregion
}
// كائن لتمثيل العملة
public class Currency
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }
    public string? Culture { get; set; }
}