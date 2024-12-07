open System
open System.Windows.Forms
open System.IO
open System.Drawing

// Function to create the form
let createForm () =
    new Form(Text = "Text Analyzer", Width = 800, Height = 600, BackColor = Color.LightGray)

// Function to create the text box
let createTextBox () =
    new TextBox(Multiline = true, Width = 700, Height = 200, Top = 20, Left = 20, ScrollBars = ScrollBars.Vertical, Font = new Font("Arial", 12f))

// Function to create the analyze button
let createAnalyzeButton () =
    new Button(Text = "Analyze", Top = 240, Left = 20, Width = 100, Height = 40, BackColor = Color.LightBlue, Font = new Font("Arial", 10f))

// Function to create the load button
let createLoadButton () =
    new Button(Text = "Load File", Top = 240, Left = 140, Width = 100, Height = 40, BackColor = Color.LightGreen, Font = new Font("Arial", 10f))

// Function to create the clear button
let createClearButton () =
    new Button(Text = "Clear", Top = 240, Left = 260, Width = 100, Height = 40, BackColor = Color.LightCoral, Font = new Font("Arial", 10f))

// Function to create the progress bar
let createProgressBar () =
    new ProgressBar(Top = 290, Left = 20, Width = 750, Height = 20, Maximum = 100, Value = 0, Style = ProgressBarStyle.Blocks)

// Function to create the result label
let createResultLabel () =
    new Label(Top = 320, Left = 20, Width = 750, Height = 230, AutoSize = true, Font = new Font("Arial", 10f))

// Function to load a file into the text box
let loadFile (textBox: TextBox) () =
    let openFileDialog = new OpenFileDialog(Filter = "Text Files|*.txt")
    if openFileDialog.ShowDialog() = DialogResult.OK then
        textBox.Text <- File.ReadAllText(openFileDialog.FileName)

// Function to analyze the text
let analyzeText (text: string) =
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

// Function to display the analysis results
let displayResults (textBox: TextBox) (resultLabel: Label) (progressBar: ProgressBar) =
    progressBar.Value <- 10 // Update progress bar
    let (wordCount, sentenceCount, paragraphCount, wordFrequency, avgSentenceLength) =
        analyzeText textBox.Text
    progressBar.Value <- 100 // Update progress bar to indicate completion

    let freqString =
        wordFrequency
        |> Seq.map (fun (word, count) -> sprintf "%s: %d" word count)
        |> String.concat "\n"

    resultLabel.Text <- sprintf "Words: %d\nSentences: %d\nParagraphs: %d\nAverage Sentence Length: %.2f\nWord Frequency (Top 10):\n%s" 
                            wordCount sentenceCount paragraphCount avgSentenceLength freqString

// Function to clear the text box, result label, and progress bar
let clearForm (textBox: TextBox) (resultLabel: Label) (progressBar: ProgressBar) =
    textBox.Clear()
    resultLabel.Text <- ""
    progressBar.Value <- 0

// Function to set up the event handlers
let setupEventHandlers (form: Form) (textBox: TextBox) (resultLabel: Label) (progressBar: ProgressBar) (analyzeButton: Button) (loadButton: Button) (clearButton: Button) =
    analyzeButton.Click.Add(fun _ -> displayResults textBox resultLabel progressBar)
    loadButton.Click.Add(fun _ -> loadFile textBox ())
    clearButton.Click.Add(fun _ -> clearForm textBox resultLabel progressBar)

// Main function to create the form and set up the application
[<STAThread>]
do
    let form = createForm ()
    let textBox = createTextBox ()
    let analyzeButton = createAnalyzeButton ()
    let loadButton = createLoadButton ()
    let clearButton = createClearButton ()
    let progressBar = createProgressBar ()
    let resultLabel = createResultLabel ()

    form.Controls.AddRange [| textBox; analyzeButton; loadButton; clearButton; progressBar; resultLabel |]

    // Set up event handlers
    setupEventHandlers form textBox resultLabel progressBar analyzeButton loadButton clearButton

    // Run the application
    Application.Run(form)

