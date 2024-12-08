open System
open System.Windows.Forms
open System.IO
open System.Drawing

// Function to create the form
let createForm () =
    new Form(Text = "Text Analyzer", Width = 900, Height = 700, StartPosition = FormStartPosition.CenterScreen, BackColor = Color.FromArgb(240, 240, 240))

// Function to create the text box
let createTextBox () =
    new TextBox(Multiline = true, Dock = DockStyle.Top, Height = 200, ScrollBars = ScrollBars.Vertical, Font = new Font("STIXSizeTwoSym", 10f, FontStyle.Bold), Padding = Padding(10))

// Function to create a button
let createButton text color =
    new Button(Text = text, Height = 40, Width = 120, BackColor = color, FlatStyle = FlatStyle.Popup, Font = new Font("STIXSizeTwoSym", 12f, FontStyle.Bold), Margin = Padding(10))

// Function to create the progress bar
let createProgressBar () =
    new ProgressBar(Dock = DockStyle.Top, Height = 20, Maximum = 100, Style = ProgressBarStyle.Blocks, Margin = Padding(10))

// Function to create a scrollable panel for the result label
let createScrollableResultPanel () =
    let panel = new Panel(Dock = DockStyle.Fill, AutoScroll = true, Padding = Padding(10), BackColor = Color.White)
    let label = new Label(AutoSize = true, Font = new Font("STIXSizeTwoSym", 10f, FontStyle.Bold))
    panel.Controls.Add(label)
    panel, label

// Function to create a panel with buttons
let createButtonPanel (buttons: Button list) =
    let panel = new FlowLayoutPanel(Dock = DockStyle.Top, FlowDirection = FlowDirection.LeftToRight, Height = 70, Padding = Padding(10))
    buttons |> List.iter (fun btn -> panel.Controls.Add(btn))
    panel

// Function to create a spacer panel
let createSpacer height =
    new Panel(Dock = DockStyle.Top, Height = height, BackColor = Color.Transparent)

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

    let top10Words = wordFrequency |> Seq.truncate 10
    let avgSentenceLength = if sentenceCount > 0 then float wordCount / float sentenceCount else 0.0

    (wordCount, sentenceCount, paragraphCount, wordFrequency, top10Words, avgSentenceLength)

// Function to display the analysis results
let displayResults (textBox: TextBox) (resultLabel: Label) (progressBar: ProgressBar) =
    progressBar.Value <- 20 // Simulate progress
    let (wordCount, sentenceCount, paragraphCount, allWordFrequency, top10WordFrequency, avgSentenceLength) =
        analyzeText textBox.Text
    progressBar.Value <- 100 // Complete progress

    let top10String =
        top10WordFrequency
        |> Seq.map (fun (word, count) -> sprintf "%s: %d" word count)
        |> String.concat "\n"

    let allWordsString =
        allWordFrequency
        |> Seq.map (fun (word, count) -> sprintf "%s: %d" word count)
        |> String.concat "\n"

    resultLabel.Text <- sprintf "Words: %d\nSentences: %d\nParagraphs: %d\nAverage Sentence Length: %.2f\n\nTop 10 Words:\n%s\n\nAll Words:\n%s" 
                            wordCount sentenceCount paragraphCount avgSentenceLength top10String allWordsString

// Function to clear the text box, result label, and progress bar
let clearForm (textBox: TextBox) (resultLabel: Label) (progressBar: ProgressBar) =
    textBox.Clear()
    resultLabel.Text <- ""
    progressBar.Value <- 0

// Main function
[<STAThread>]
do
    let form = createForm ()
    let textBox = createTextBox ()
    let progressBar = createProgressBar ()
    
    // Create a scrollable panel for results
    let resultPanel, resultLabel = createScrollableResultPanel ()
    
    // Buttons with custom colors
    let analyzeButton = createButton "Analyze" Color.LightBlue
    let loadButton = createButton "Load File" Color.LightGreen
    let clearButton = createButton "Clear" Color.IndianRed

    // Create button panel and spacer
    let buttonPanel = createButtonPanel [analyzeButton; loadButton; clearButton]
    let spacer = createSpacer 20

    // Set up event handlers
    analyzeButton.Click.Add(fun _ -> displayResults textBox resultLabel progressBar)
    loadButton.Click.Add(fun _ -> loadFile textBox ())
    clearButton.Click.Add(fun _ -> clearForm textBox resultLabel progressBar)

    // Add controls to the form
    form.Controls.AddRange [| resultPanel; spacer; progressBar; buttonPanel; textBox |]

    // Run the application
    Application.Run(form)