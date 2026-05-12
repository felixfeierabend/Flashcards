namespace Flashcards;

public partial class FlashcardManagerPage : ContentPage
{
    private byte[] img = null;

    public delegate void FlashcardDelegate(object sender, FlashcardEventArgs args);
    public event FlashcardDelegate FlashcardChanged;

	public FlashcardManagerPage()
	{
		InitializeComponent();
	}

    private void ButtonAddImage_Clicked(object sender, EventArgs e)
    {
    }

    private void ButtonRefreshPreview_Clicked(object sender, EventArgs e)
    {

    }

    private async void ButtonSave_Clicked(object sender, EventArgs e)
    {
        Flashcard flashcard = new Flashcard(editorFront.Text, editorBack.Text, 5, img);
        FlashcardChanged(this, new FlashcardEventArgs(flashcard));
    }
}

public class FlashcardEventArgs
{
    public Flashcard flashcard { get; }

    public FlashcardEventArgs(Flashcard flashcard)
    {
        this.flashcard = flashcard;
    }
}