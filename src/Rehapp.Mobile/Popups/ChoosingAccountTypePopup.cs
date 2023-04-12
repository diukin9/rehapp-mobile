using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Rehapp.Mobile.Models.Enums;

namespace Rehapp.Mobile.Popups;

public class ChoosingAccountTypePopup : Popup
{
    public AccountType? AccountType { get; set; }
    public bool IsSuccessful => AccountType is not null;

    public ChoosingAccountTypePopup()
    {
        Color = Colors.Transparent;

        var title = new Label
        {
            Text = "Укажите тип аккаунта",
            TextColor = Color.FromArgb("#3A3A3A"),
            FontFamily = "geometria_medium",
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            FontSize = 14,
            Margin = new Thickness(30, 0)
        };

        var patientButton = new Button
        {
            ImageSource = "patient_image_btn.png",
            BackgroundColor = Colors.White,
            CornerRadius = 15,
            FontSize = 16,
            FontFamily = "geometria_medium",
            TextColor = Color.FromArgb("#3A3A3A"),
            Text = "Пациент",
            Shadow = new Shadow() { Brush = Colors.Black, Opacity = 0.25f, Radius = 20 }
        };

        patientButton.Clicked += (sender, r) =>
        {
            AccountType = Models.Enums.AccountType.Patient;
            Close();
        };

        var doctorButton = new Button
        {
            ImageSource = "doctor_image_btn.png",
            BackgroundColor = Colors.White,
            CornerRadius = 15,
            FontSize = 16,
            FontFamily = "geometria_medium",
            TextColor = Color.FromArgb("#3A3A3A"),
            Text = "Специалист",
            Shadow = new Shadow() { Brush = Colors.Black, Opacity = 0.25f, Radius = 20 }
        };

        doctorButton.Clicked += (sender, r) =>
        {
            AccountType = Models.Enums.AccountType.Specialist;
            Close();
        };

        var buttons = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            Spacing = 15,
            Margin = new Thickness(30, 0)
        };

        buttons.Add(patientButton);
        buttons.Add(doctorButton);

        var stackLayout = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.Fill,
            Spacing = 25,
        };

        stackLayout.Children.Add(title);
        stackLayout.Children.Add(buttons);

        var mainBorder = new Border
        {
            Padding = new Thickness(0, 30),
            Stroke = Brush.Transparent,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            BackgroundColor = Colors.White,
            Content = stackLayout,
            VerticalOptions = LayoutOptions.Fill,
            HeightRequest = 320,
            WidthRequest = 300
        };

        Content = mainBorder;
    }
}