using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace Flashcards
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Flashcard> Flashcards { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Dark;
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

        private void ButtonEditCard_Clicked(object sender, EventArgs e)
        {

        }

        private async void ButtonAddNewCard_Clicked(object sender, EventArgs e)
        {
            FlashcardManagerPage flashcardManager = new FlashcardManagerPage();
            await Navigation.PushAsync(flashcardManager);
            flashcardManager.FlashcardChanged += FlashcardManager_FlashcardChanged;
        }

        private void FlashcardManager_FlashcardChanged(object sender, FlashcardEventArgs args)
        {
            this.Flashcards.Add(args.flashcard);
            collectionViewCards.ItemsSource = this.Flashcards;
        }

        private async void ButtonStartStudy_Clicked(object sender, EventArgs e)
        {
            if (Flashcards.Count > 0)
            {
                await Navigation.PushAsync(new StudyPage(Flashcards));
            }
        }
    }
}
