
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace iNFT.src.Toaster {
    public class Toaster {
        private readonly Border Toastie = new Border();
        private readonly string boarderXName = "Toastie";
        private readonly CornerRadius borderCornerRadius = new CornerRadius(10.0);
        private readonly int borderWidth = 160;
        private readonly int borderHeight = 50;
        private readonly HorizontalAlignment borderHorizontalAlignment = HorizontalAlignment.Center;
        private readonly VerticalAlignment borderVerticalAlignment = VerticalAlignment.Bottom;
        private readonly double borderBottomMargin = 10;
        private readonly double borderTopMargin = 0;
        private readonly double borderRightMargin = 0;
        private readonly double borderLeftMargin = 0;
        private readonly Thickness borderThickness = new Thickness(1.0);

        private readonly TextBlock ToastieText = new TextBlock();
        private readonly string textBlockXName = "ToastieText";
        private readonly TextAlignment textBlockTextAlignment = TextAlignment.Center;
        private readonly Thickness textBlockMargin = new Thickness(1.0);
        private readonly TextWrapping textBlockTextWrapping = TextWrapping.Wrap;
        private readonly VerticalAlignment textBlockVerticalAlignment = VerticalAlignment.Center;
        private readonly double textBlockFontSize = 12.0;
        private readonly string textBlockFontFamily = "Segoe UI";

        private Color PrimaryBackgroundColor;
        private Color PrimaryBorderColor;
        private Color PrimaryFontColor;
        private Color WarningBackgroundColor;
        private Color WarningBorderColor;
        private Color WarningFontColor;
        private Color ErrorBackgroundColor;
        private Color ErrorBorderColor;
        private Color ErrorFontColor;

        private readonly DispatcherTimer timer = new DispatcherTimer();

        public enum ToastColors { PRIMARY, WARNING, ERROR }

        public Toaster() {
            Toastie.Child = ToastieText;
            InstatiateBorder();
            InstatiateTextBlock();
            SetColors();
        }//public Toaster() {

        private void InstatiateBorder() {
            SetBorderXName(boarderXName);
            SetBorderCornerRadius(borderCornerRadius);
            SetBorderWidth(borderWidth);
            SetBorderHeight(borderHeight);
            SetBorderHorizontalAlignment(borderHorizontalAlignment);
            SetBorderVerticalAlignment(borderVerticalAlignment);
            SetBorderMargin(borderLeftMargin, borderTopMargin, borderRightMargin, borderBottomMargin);
            SetBorderThickness(borderThickness);
            Toastie.Visibility = Visibility.Hidden;
        }//private void InstatiateBorder() {

        private void InstatiateTextBlock() {
            SetTextBlockXName(textBlockXName);
            SetTextBlockTextAlignment(textBlockTextAlignment);
            SetTextBlockMargin(textBlockMargin);
            SetTextBlockTextWrapping(textBlockTextWrapping);
            SetTextBlockVerticalAlignment(textBlockVerticalAlignment);
            SetTextBlockFontSize(textBlockFontSize);
            SetTextBlockFontFamily(textBlockFontFamily);
        }//private void InstatiateTextBlock() {

        private void SetColors() {
            SetPrimaryColors(Color.FromArgb(225, 0, 125, 0), Color.FromArgb(225, 0, 255, 0), Color.FromArgb(225, 0, 255, 0));
            SetWarningColors(Color.FromArgb(225, 125, 125, 0), Color.FromArgb(225, 255, 255, 0), Color.FromArgb(225, 255, 255, 0));
            SetErrorColors(Color.FromArgb(225, 125, 0, 0), Color.FromArgb(225, 255, 0, 0), Color.FromArgb(225, 255, 0, 0));
        }//private void SetColors() {

        public void FlipTextBlockFontBold() {
            if (ToastieText.FontWeight.Equals(FontWeights.Normal)) {
                ToastieText.FontWeight = FontWeights.Bold;
            } else {//if (ToastieText.FontWeight.Equals(FontWeights.Normal)) {
                ToastieText.FontWeight = FontWeights.Normal;
            }//else {
        }//public void FlipTextBlockFontBold() {

        public void FlipTextBlockFontItalics() {
            if (ToastieText.FontStyle.Equals(FontStyles.Italic)) {
                ToastieText.FontStyle = FontStyles.Normal;
            } else {//if (ToastieText.FontStyle.Equals(FontStyles.Italic)) {
                ToastieText.FontStyle = FontStyles.Italic;
            }//else {
        }//public void FlipTextBlockFontItalics() {

        public Border GetToast() {
            return Toastie;
        }//public Border GetToast() {

        public void PopToastie(string message, ToastColors tc, int seconds) {
            switch (tc) {
                case ToastColors.PRIMARY:
                    Toastie.Background = new SolidColorBrush(PrimaryBackgroundColor);
                    Toastie.BorderBrush = new SolidColorBrush(PrimaryBorderColor);
                    ToastieText.Foreground = new SolidColorBrush(PrimaryFontColor);
                    break;
                case ToastColors.ERROR:
                    Toastie.Background = new SolidColorBrush(ErrorBackgroundColor);
                    Toastie.BorderBrush = new SolidColorBrush(ErrorBorderColor);
                    ToastieText.Foreground = new SolidColorBrush(ErrorFontColor);
                    break;
                case ToastColors.WARNING:
                    Toastie.Background = new SolidColorBrush(WarningBackgroundColor);
                    Toastie.BorderBrush = new SolidColorBrush(WarningBorderColor);
                    ToastieText.Foreground = new SolidColorBrush(WarningFontColor);
                    break;
                default:
                    Toastie.Background = new SolidColorBrush(Color.FromArgb(225, 255, 255, 255));
                    Toastie.BorderBrush = new SolidColorBrush(Color.FromArgb(225, 0, 0, 0));
                    ToastieText.Foreground = new SolidColorBrush(Color.FromArgb(225, 100, 100, 100));
                    break;
            }//switch (tc) {
            ToastieText.Text = message;
            Toastie.Visibility = Visibility.Visible;
            timer.Interval = TimeSpan.FromSeconds(seconds);
            timer.Stop();
            timer.Tick += (s, en) => {
                Toastie.Visibility = Visibility.Hidden;
                timer.Stop();
            };
            timer.Start();
        }//public void PopToastie(string message, ToastColors tc, int seconds) {

        public void SetBorderCornerRadius(CornerRadius borderCornerRadius) {
            Toastie.CornerRadius = borderCornerRadius;
        }//public void SetBorderCornerRadius(CornerRadius borderCornerRadius) {

        public void SetBorderHeight(int borderHeight) {
            Toastie.Height = borderHeight;
        }//public void SetBorderHeight(int borderHeight) {

        public void SetBorderHorizontalAlignment(HorizontalAlignment borderHorizontalAlignment) {
            Toastie.HorizontalAlignment = borderHorizontalAlignment;
        }//public void SetBorderHorizontalAlignment(HorizontalAlignment borderHorizontalAlignment) {

        public void SetBorderMargin(double borderLeftMargin, double borderTopMargin, double borderRightMargin, double borderBottomMargin) {
            Toastie.Margin = new Thickness(borderLeftMargin, borderTopMargin, borderRightMargin, borderBottomMargin);
        }//public void SetBorderMargin(double borderLeftMargin,double borderTopMargin,double borderRightMargin,double borderBottomMargin) {

        public void SetBorderThickness(Thickness borderThickness) {
            Toastie.BorderThickness = borderThickness;
        }//public void SetBorderThickness(Thickness borderThickness) {

        public void SetBorderVerticalAlignment(VerticalAlignment borderVerticalAlignment) {
            Toastie.VerticalAlignment = borderVerticalAlignment;
        }//public void SetBorderVerticalAlignment(VerticalAlignment borderVerticalAlignment) {

        public void SetBorderWidth(int borderWidth) {
            Toastie.Width = borderWidth;
        }//public void SetBorderWidth(int borderWidth) {

        public void SetBorderXName(string boarderXName) {
            Toastie.Name = boarderXName;
        }//public void SetBorderXName(string boarderXName) {

        public void SetErrorColors(Color backgroundColor, Color boarderColor, Color fontColor) {
            ErrorBackgroundColor = backgroundColor;
            ErrorBorderColor = boarderColor;
            ErrorFontColor = fontColor;
        }//public void SetErrorColors(Color backgroundColor, Color boarderColor, Color fontColor) {

        public void SetPrimaryColors(Color backgroundColor, Color boarderColor, Color fontColor) {
            PrimaryBackgroundColor = backgroundColor;
            PrimaryBorderColor = boarderColor;
            PrimaryFontColor = fontColor;
        }//public void SetPrimaryColors(Color backgroundColor, Color boarderColor, Color fontColor) {

        public void SetTextBlockFontFamily(string textBlockFontFamily) {
            ToastieText.FontFamily = new FontFamily(textBlockFontFamily);
        }//public void SetTextBlockFontFamily(string textBlockFontFamily) {

        public void SetTextBlockFontSize(double textBlockFontSize) {
            ToastieText.FontSize = textBlockFontSize;
        }//public void SetTextBlockFontSize(double textBlockFontSize) {

        public void SetTextBlockMargin(Thickness margin) {
            ToastieText.Margin = margin;
        }//public void SetTextBlockMargin(Thickness margin) {

        public void SetTextBlockMargin(double leftMargin, double topMargin, double rightMargin, double bottomMargin) {
            ToastieText.Margin = new Thickness(leftMargin, topMargin, rightMargin, bottomMargin);
        }//public void SetTextBlockMargin(double leftMargin, double topMargin,double rightMargin, double bottomMargin) {

        public void SetTextBlockTextAlignment(TextAlignment textBlockTextAlignment) {
            ToastieText.TextAlignment = textBlockTextAlignment;
        }//public void SetTextBlockTextAlignment(TextAlignment textBlockTextAlignment) {

        public void SetTextBlockTextWrapping(TextWrapping TextWrapping) {
            ToastieText.TextWrapping = TextWrapping;
        }//public void SetTextBlockTextWrapping(TextWrapping TextWrapping) {

        public void SetTextBlockVerticalAlignment(VerticalAlignment textBlockVerticalAlignment) {
            ToastieText.VerticalAlignment = textBlockVerticalAlignment;
        }//public void SetTextBlockVerticalAlignment(VerticalAlignment textBlockVerticalAlignment) {

        public void SetTextBlockXName(string textBlockXName) {
            ToastieText.Name = textBlockXName;
        }//public void SetTextBlockXName(string textBlockXName) {

        public void SetWarningColors(Color backgroundColor, Color boarderColor, Color fontColor) {
            WarningBackgroundColor = backgroundColor;
            WarningBorderColor = boarderColor;
            WarningFontColor = fontColor;
        }//public void SetWarningColors(Color backgroundColor, Color boarderColor, Color fontColor) {
    }//public class Toaster {
}//namespace iNFT.src.Toaster {