using CommunityToolkit.Maui.Alerts;
using ExpenseApp.Classes;
using ExpenseApp.ItemsView;
using ExpenseApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseApp.Pages;

public partial class SettingPage : ContentPage
{
    string NotifyTime = string.Empty;
    public SettingPage()
	{
		InitializeComponent();
        FillCurrencyPicker();
        GetDefault();
	}

    private void FillCurrencyPicker()
    {
        // قائمة العملات مع رموزها
        var currencies = new List<Currency>
        {
            new Currency { Name = "الدينار الجزائري", Symbol = "دج", Culture = "ar-DZ" },
            new Currency { Name = "الريال السعودي", Symbol = "﷼", Culture = "ar-SA" },
            new Currency { Name = "الدولار الأمريكي", Symbol = "$", Culture = "en-US" },
            new Currency { Name = "اليورو", Symbol = "€", Culture = "fr-FR" },
            new Currency { Name = "الجنيه الإسترليني", Symbol = "£", Culture = "en-GB" },
            new Currency { Name = "الين الياباني", Symbol = "¥", Culture = "ja-JP" }
        };


        // ربط القائمة بـ Picker
        CurrencyPicker.ItemsSource = currencies;
    }
    private void GetDefault()
    {
        SwLang.IsToggled = Tools.Long == "ar";
        SwMode.IsToggled = Tools.Mode == "Dark";
        Application.Current.UserAppTheme = Tools.Mode == "Dark" ? AppTheme.Dark : AppTheme.Light;
        txtName.Text = Tools.Name;
        txtAmount.Text = Tools.Amount;
        CurrencyPicker.SelectedIndex = CurrencyPicker.ItemsSource.Cast<Currency>().ToList().FindIndex(c => c.Name == Tools.Caruncy);
        SwNotify.IsToggled = Tools.Notify;
        btn3.BackgroundColor = Tools.NotifyTime == btn3.Text ? Color.FromArgb("#FFFFFF") : Colors.Gray;
        btn6.BackgroundColor = Tools.NotifyTime == btn6.Text ? Color.FromArgb("#FFFFFF") : Colors.Gray;
        btn12.BackgroundColor = Tools.NotifyTime == btn12.Text ? Color.FromArgb("#FFFFFF") : Colors.Gray;     
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

    private void Button_Clicked(object sender, EventArgs e)
    {
        btn3.BackgroundColor =
        btn6.BackgroundColor =
        btn12.BackgroundColor =
        Colors.Gray;
        var btn = sender as Button;
        btn.BackgroundColor = Color.FromArgb("#FFFFFF");
        NotifyTime = btn.Text;
    }

    private void SwLang_Toggled(object sender, ToggledEventArgs e)
    {
        Tools.Long = e.Value ? "ar" : "en";
        Tools.SaveLong();
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

    private void btnName_Clicked(object sender, EventArgs e)
    {
        Tools.Name = txtName.Text;
        Tools.SaveName();
    }

    private void btnAmount_Clicked(object sender, EventArgs e)
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
    }

    private void btnNotify_Clicked(object sender, EventArgs e)
    {
        Tools.Notify = SwNotify.IsToggled;
        Tools.NotifyTime = NotifyTime;
        Tools.SaveNotify();
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
            await Toast.Make("لم يتم اختيار مكان لحفظ نسخة من قاعدة البيانات ...").Show();
            return;
        }
        string backupPath = Path.Combine(backupFolder, GenerateBackupFileName());
        using (var context = new DBContext())
        {
            string databasePath = context.Database.GetDbConnection().DataSource;
            bool backupDone = await SqliteBackupManager.BackupDatabaseAsync(databasePath, backupPath);

            if (backupDone)
            {
                await Toast.Make("تم انشاء نسخة من قاعدة البيانات").Show();
            }
            else
            {
                await Toast.Make("خطأ في انشاء نسخة من قاعدة البيانات.").Show();
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
            await Toast.Make("لم يتم العثور على قاعدة البيانات الاحتياطية.").Show();
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
                    await Toast.Make("تمت استعادة قاعدة البيانات بنجاح.").Show();
                }
                else
                {
                    await Toast.Make("فشلت عملية استعادة قاعدة البيانات.").Show();
                }
            }   
            else
            {
                await Toast.Make("بنية قاعدة البيانات مختلفة.").Show();
            }
            ///////////////////////////////

        }
        catch (Exception ex)
        {
            await Toast.Make("حدث خطأ أثناء استعادة قاعدة البيانات").Show();
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
}
// كائن لتمثيل العملة
public class Currency
{
    public string Name { get; set; }
    public string Symbol { get; set; }
    public string Culture { get; set; }
}
