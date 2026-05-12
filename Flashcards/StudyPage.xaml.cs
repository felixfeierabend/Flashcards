using System.Collections.ObjectModel;

namespace Flashcards;

public partial class StudyPage : ContentPage
{
	private int CurrentIndex = 0;
	private int TotalCards;
	private bool IsShowingFront = true;
    private List<Flashcard> Flashcards;

	public StudyPage(ObservableCollection<Flashcard> flashcards)
	{
		InitializeComponent();
        Flashcards = new List<Flashcard>(flashcards);
        TotalCards = Flashcards.Count;
	}

    private void LoadCurrentCard()
    {
        labelProgress.Text = $"Card {CurrentIndex + 1} / {TotalCards}";

        IsShowingFront = true;
        UpdateCardView();
    }

    private void UpdateCardView()
    {
        string textContent = IsShowingFront ? Flashcards[CurrentIndex].Question : Flashcards[CurrentIndex].Solution;
        if (Flashcards[CurrentIndex].Image != null)
        {
            imageCard.Source = ImageSource.FromStream(() => new MemoryStream(Flashcards[CurrentIndex].Image));
            imageCard.IsVisible = true;
        }
        else
        {
            imageCard.IsVisible = false;
            imageCard.Source = null;
        }

        string themeBg = Application.Current.RequestedTheme == AppTheme.Dark ? "#1E1E1E" : "#F9F9F9";
        string themeText = Application.Current.RequestedTheme == AppTheme.Dark ? "#FFFFFF" : "#000000";

        string htmlContent = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    
            <script>
                MathJax = {{
                    tex: {{
                        inlineMath: [['$', '$'], ['\\(', '\\)']],
                        displayMath: [['$$', '$$'], ['\\[', '\\]']]
                    }},
                    svg: {{ fontCache: 'global' }}
                }};
            </script>
    
            <script src='mathjax/tex-svg.js'></script>
    
            <style>
                body {{
                    font-family: sans-serif;
                    background-color: {themeBg};
                    color: {themeText};
                    display: flex;
                    justify-content: center;
                    align-items: center;
                    height: 90vh;
                    text-align: center;
                    margin: 0;
                    font-size: 20px;
                }}
            </style>
        </head>
        <body>
            <div>{textContent}</div>
        </body>
        </html>";

        webViewCardContent.Source = new HtmlWebViewSource { Html = htmlContent };
    }

    private void ButtonNext_Clicked(object sender, EventArgs e)
    {
        if (CurrentIndex < TotalCards - 1)
        {
            CurrentIndex++;
            LoadCurrentCard();
        }
    }

    private void ButtonPrevious_Clicked(object sender, EventArgs e)
    {
        if (CurrentIndex > 0)
        {
            CurrentIndex--;
            LoadCurrentCard();
        }
    }

    private async void ButtonFlip_Clicked(object sender, EventArgs e)
    {
        await frameFlashcard.RotateYToAsync(90, 150, Easing.CubicIn);
        IsShowingFront = !IsShowingFront;
        UpdateCardView();

        await frameFlashcard.RotateYToAsync(0, 150, Easing.CubicOut);
    }
}