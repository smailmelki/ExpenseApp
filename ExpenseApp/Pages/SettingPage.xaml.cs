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
                new Currency { Name = "الدينار الجزائري", Symbol = "دج" },
                new Currency { Name = "الدولار الأمريكي", Symbol = "$" },
                new Currency { Name = "اليورو", Symbol = "€" },
                new Currency { Name = "الجنيه الإسترليني", Symbol = "£" },
                new Currency { Name = "الين الياباني", Symbol = "¥" },
                new Currency { Name = "الريال السعودي", Symbol = "﷼" }
            };

        // ربط القائمة بـ Picker
        CurrencyPicker.ItemsSource = currencies;
    }
    private void GetDefault()
    {
        SwLang.IsToggled = Tools.Long == "ar";
        SwMode.IsToggled = Tools.Mode == "Dark";
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
            Tools.Caruncy = ((Currency)CurrencyPicker.SelectedItem).Name;
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
            await DisplayAlert("خطأ", "لم يتم اختيار مكان لحفظ نسخة من قاعدة البيانات ...", "موافق");
            return;
        }
        string backupPath = Path.Combine(backupFolder, GenerateBackupFileName());
        using (var context = new DBContext())
        {
            string databasePath = context.Database.GetDbConnection().DataSource;
            bool backupDone = await SqliteBackupManager.BackupDatabaseAsync(databasePath, backupPath);

            if (backupDone)
            {
                await DisplayAlert("نسخ قاعدة البيانات", "تم انشاء نسخة من قاعدة البيانات", "موافق");
            }
            else
            {
                await DisplayAlert("خطأ", "خطأ في انشاء نسخة من قاعدة البيانات.", "موافق");
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
            await DisplayAlert("خطأ", "لم يتم العثور على قاعدة البيانات الاحتياطية.", "موافق");
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
                    await DisplayAlert("نجاح", "تمت استعادة قاعدة البيانات بنجاح.", "موافق");
                }
                else
                {
                    await DisplayAlert("خطأ", "فشلت عملية استعادة قاعدة البيانات.", "موافق");
                }
            }
            else
            {
                await DisplayAlert("خطأ", "بنية قاعدة البيانات مختلفة.", "موافق");
            }
            ///////////////////////////////

        }
        catch (Exception ex)
        {
            await DisplayAlert("خطأ", $"حدث خطأ أثناء استعادة قاعدة البيانات: {ex.Message}", "موافق");
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
}