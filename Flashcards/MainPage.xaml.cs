using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Maui.Storage;

namespace Flashcards
{
    public partial class MainPage : ContentPage
    {
        private string Path = string.Empty;

        public ObservableCollection<Flashcard> Flashcards { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
        }

        private async void ButtonLoadDeck_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Flashcard Dataset"
                });

                if (result != null)
                {
                    Path = result.FullPath;

                    using (Stream s = await result.OpenReadAsync())
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var loadedCards = await JsonSerializer.DeserializeAsync<List<Flashcard>>(s, options);

                        if (loadedCards != null)
                        {
                            Flashcards.Clear();
                            foreach (var card in loadedCards)
                            {
                                Flashcards.Add(card);

                                collectionViewCards.ItemsSource = this.Flashcards;
                            }

                            await DisplayAlertAsync("Success", $"{loadedCards.Count} Flashcards loaded!", "OK");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Unable to load file: {ex.Message}", "OK");
            }
        }

        private void ButtonToggleTheme_Clicked(object sender, EventArgs e)
        {
            if (Application.Current.UserAppTheme == AppTheme.Dark)
            {
                Application.Current.UserAppTheme = AppTheme.Light;
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
        }

        private async void ButtonEditCard_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Flashcard selectedCard)
            {
                FlashcardManagerPage flashcardManager = new FlashcardManagerPage(selectedCard);
                await Navigation.PushAsync(flashcardManager);
                flashcardManager.FlashcardChanged += FlashcardManager_FlashcardChanged;                
            }
        }

        private async void ButtonAddNewCard_Clicked(object sender, EventArgs e)
        {
            FlashcardManagerPage flashcardManager = new FlashcardManagerPage();
            await Navigation.PushAsync(flashcardManager);
            flashcardManager.FlashcardChanged += FlashcardManager_FlashcardChanged;
        }

        private void FlashcardManager_FlashcardChanged(object sender, FlashcardEventArgs args)
        {
            if (args.EditedFlashcard != null)
            {
                this.Flashcards.Remove(args.EditedFlashcard);
            }

            if (this.Flashcards.Contains(args._Flashcard))
            {
                return;
            }

            this.Flashcards.Add(args._Flashcard);
            collectionViewCards.ItemsSource = this.Flashcards;
        }

        private async void ButtonStartStudy_Clicked(object sender, EventArgs e)
        {
            if (Flashcards.Count > 0)
            {
                await Navigation.PushAsync(new StudyPage(Flashcards));
            }
        }

        private async void ButtonSave_Clicked(object sender, EventArgs e)
        {
            if (Flashcards.Count > 0)
            {
                string json = JsonSerializer.Serialize(Flashcards);

                if (string.IsNullOrEmpty(Path))
                {
                    var result = await FileSaver.Default.SaveAsync("flashcards.json", new MemoryStream(Encoding.UTF8.GetBytes(json)));
                }
                else
                {
                    File.WriteAllText(Path.EndsWith(".json") ? Path : $"{Path}.json", json);
                }
            }
        }

        private void buttonDelete_Clicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Flashcard selectedFlashcard)
            {
                Flashcards.Remove(selectedFlashcard);
            }
        }
    }
}
