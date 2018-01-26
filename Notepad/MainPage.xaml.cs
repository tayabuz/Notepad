using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Notepad
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void SaveFile_OnClick(object sender, RoutedEventArgs e)
        {
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add(".txt", new List<string>() { ".txt" });
            savePicker.SuggestedFileName = "New Document";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                await FileIO.WriteTextAsync(file, WorkField.Text);
                await CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        private async void OpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".txt");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var stream = await file.OpenAsync(FileAccessMode.Read);
                using (StreamReader reader = new StreamReader(stream.AsStream()))
                {
                    WorkField.Text = reader.ReadToEnd();
                }
            }
        }
        private void KeyPressed(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                WorkField.Text += "\r\n";
            }

            if (e.Key == Windows.System.VirtualKey.Tab)
            {
                int current = WorkField.SelectionStart;
                if (current <= 0) { return;}
                if (current >= WorkField.Text.Length)
                {
                    WorkField.Text += "\t";
                }
                else
                {
                    WorkField.Text = WorkField.Text.Insert(current, "\t");
                }
                WorkField.SelectionStart = current + 1;
                e.Handled = true;
            }
        }

        bool CtrlPressed = false;
        private void UIElement_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Control)
            {
                CtrlPressed = true;
            }
            if (CtrlPressed)
            {
                if (e.Key == Windows.System.VirtualKey.F12)
                {
                    OpenFile_OnClick(this, new RoutedEventArgs());
                    CtrlPressed = false;
                }
                if (e.Key == Windows.System.VirtualKey.S)
                {
                    SaveFile_OnClick(this, new RoutedEventArgs());
                    CtrlPressed = false;
                }
            }
        }
    }
}

