open System
open System.Windows.Forms
open System.IO

// the form
let form = new Form(Text = "Text Analyzer", Width = 800, Height = 600)

// TextBox to input text
let textBox = new TextBox(Multiline = true, Width = 700, Height = 200, Top = 20, Left = 20, ScrollBars = ScrollBars.Vertical)

// Button to analyze text
let analyzeButton = new Button(Text = "Analyze", Top = 240, Left = 20, Width = 100)

// Button to load a file
let loadButton = new Button(Text = "Load File", Top = 240, Left = 140, Width = 100)

// Label to display results
let resultLabel = new Label(Top = 280, Left = 20, Width = 750, Height = 250, AutoSize = true)

// Add controls to form
form.Controls.AddRange [| textBox; analyzeButton; loadButton; resultLabel |]

// Load File Function
let loadFile () =
    let openFileDialog = new OpenFileDialog(Filter = "Text Files|*.txt")
    if openFileDialog.ShowDialog() = DialogResult.OK then
        textBox.Text <- File.ReadAllText(openFileDialog.FileName)

// Text Analysis Function
let analyzeText (text: string) =
    // Remove extra whitespace or break lines
    let cleanText = text.Replace("\r\n", "\n").Replace("\r", "\n").Trim()
    let words = cleanText.Split([| ' '; '\n'; '\t'; '.'; ','; '!' |], StringSplitOptions.RemoveEmptyEntries)
    let sentences = cleanText.Split([| '.'; '!'; '?' |], StringSplitOptions.RemoveEmptyEntries)
    let paragraphs = cleanText.Split([| '\n' |], StringSplitOptions.RemoveEmptyEntries)


    let wordCount = words.Length
    let sentenceCount = sentences.Length
    let paragraphCount = paragraphs.Length

    let wordFrequency =
        words
        |> Seq.groupBy (fun word -> word)
        |> Seq.map (fun (word: string, occurrences: seq<string>) -> word, Seq.length occurrences)
        |> Seq.sortByDescending snd
        |> Seq.truncate 10  

    let avgSentenceLength = if sentenceCount > 0 then float wordCount / float sentenceCount else 0.0

    (wordCount, sentenceCount, paragraphCount, wordFrequency, avgSentenceLength)

// Display Results Function
let displayResults () =
    let (wordCount, sentenceCount, paragraphCount, wordFrequency, avgSentenceLength) =
        analyzeText textBox.Text

    let freqString =
        wordFrequency
        |> Seq.map (fun (word, count) -> sprintf "%s: %d" word count)
        |> String.concat "\n"

    resultLabel.Text <- sprintf "Words: %d\nSentences: %d\nParagraphs: %d\nAverage Sentence Length: %.2f\nWord Frequency (Top 10):\n%s" 
                            wordCount sentenceCount paragraphCount avgSentenceLength freqString

// Event Handlers
analyzeButton.Click.Add(fun _ -> displayResults())
loadButton.Click.Add(fun _ -> loadFile())

// Show the form
[<STAThread>]
do
  Application.Run(form)


