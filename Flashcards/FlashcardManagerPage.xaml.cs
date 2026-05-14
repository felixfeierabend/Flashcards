namespace Flashcards;

public partial class FlashcardManagerPage : ContentPage
{
    private ImageSource ImgFront = null;
    private ImageSource ImgBack = null;
    private Flashcard EditedFlashcard = null;
    private byte[] ImgDataFront = null;
    private byte[] ImgDataBack = null;

    public delegate void FlashcardDelegate(object sender, FlashcardEventArgs args);
    public event FlashcardDelegate FlashcardChanged;

	public FlashcardManagerPage(Flashcard flashcard = null)
	{
		InitializeComponent();

        if (flashcard != null)
        {
            this.ImgFront = flashcard.ImageFront != null ? ImageSource.FromStream(() => new MemoryStream(flashcard.ImageFront)) : null;
            this.ImgBack = flashcard.ImageBack != null ? ImageSource.FromStream(() => new MemoryStream(flashcard.ImageBack)) : null;
            this.ImgDataFront = flashcard.ImageFront;
            this.ImgDataBack = flashcard.ImageBack;
            this.editorFront.Text = flashcard.Question;
            this.editorBack.Text = flashcard.Solution;

            RefreshPreview();
        }
        EditedFlashcard = flashcard;
	}

    private async void ButtonAddImageFront_Clicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select Image",
            FileTypes = FilePickerFileType.Images
        });

        if (result != null)
        {
            ImageSource src = ImageSource.FromFile(result.FullPath);
            this.imageFrontPreview.Source = src;
            this.imageFrontPreview.IsVisible = true;
            ImgFront = src;
            ImgDataFront = await File.ReadAllBytesAsync(result.FullPath);
        }
        else
        {
            ImgFront = null;
            ImgDataFront = null;
            RefreshPreview();
        }
    }

    private void ButtonRefreshPreview_Clicked(object sender, EventArgs e)
    {
        RefreshPreview();
    }

    private void RefreshPreview()
    {
        if (ImgFront == null)
        {
            imageFrontPreview.IsVisible = false;
            imageFrontPreview.Source = null;
        }
        else
        {
            imageFrontPreview.Source = ImgFront;
            imageFrontPreview.IsVisible = true;
        }
        if (ImgBack == null)
        {
            imageBackPreview.IsVisible = false;
            imageBackPreview.Source = null;
        }
        else
        {
            imageBackPreview.Source = ImgBack;
            imageBackPreview.IsVisible = true;
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
                    margin: 20px;
                    font-size: 20px;
                    text-align: center;
                }}

                .section {{
                    margin-bottom: 30px;
                }}

                h4 {{
                    margin-bottom: 10px;
                }}
            </style>
        </head>
        <body>

            <div class='section'>
                <h4>Question</h4>
                <div>{editorFront.Text}</div>
            </div>

            <div class='section'>
                <h4>Solution</h4>
                <div>{editorBack.Text}</div>
            </div>

        </body>
    </html>";

        webViewLatexPreview.Source = new HtmlWebViewSource { Html = htmlContent };

    }

    private async void ButtonSave_Clicked(object sender, EventArgs e)
    {
        Flashcard flashcard = new Flashcard(editorFront.Text, editorBack.Text, 5, ImgDataFront, ImgDataBack);
        FlashcardChanged(this, new FlashcardEventArgs(flashcard, EditedFlashcard));
        await DisplayAlertAsync("Success", $"Added Card!", "OK");
    }

    private async void buttonAddImageBack_Clicked(object sender, EventArgs e)
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select Image",
            FileTypes = FilePickerFileType.Images
        });

        if (result != null)
        {
            ImageSource src = ImageSource.FromFile(result.FullPath);
            this.imageBackPreview.Source = src;
            this.imageBackPreview.IsVisible = true;
            ImgDataBack = await File.ReadAllBytesAsync(result.FullPath);
            ImgBack = src;
        }
        else
        {
            ImgBack = null;
            ImgDataBack = null;
            RefreshPreview();
        }
    }

    private void editorFront_TextChanged(object sender, TextChangedEventArgs e)
    {
        RefreshPreview();
    }

    private void editorBack_TextChanged(object sender, TextChangedEventArgs e)
    {
        RefreshPreview();
    }
}

public class FlashcardEventArgs
{
    public Flashcard _Flashcard { get; }
    public Flashcard EditedFlashcard { get; }
    

    public FlashcardEventArgs(Flashcard flashcard, Flashcard EditedFlashcard = null)
    {
        this._Flashcard = flashcard;
        this.EditedFlashcard = EditedFlashcard;
    }
}