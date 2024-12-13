namespace TextAnalyzer
open System
open System.Windows.Forms

module TextAnalyzer =
// Function to analyze the text
    let analyzeText (text: string) =
        let cleanText = text.Replace("\r\n", "\n").Replace("\r", "\n").Trim()
        
        // Check for the placeholder text and return empty analysis if present
        if cleanText = "Drag and drop a file here or paste your text..." then
            0, 0, 0, Seq.empty, Seq.empty, 0.0
        else
        let words = cleanText.Split([| ' '; '\n'; '\t'; '.'; ','; '!' |], StringSplitOptions.RemoveEmptyEntries)
        let sentences = cleanText.Split([| '.'; '!'; '?' |], StringSplitOptions.RemoveEmptyEntries)
        let paragraphs = cleanText.Split([| '\n' |], StringSplitOptions.RemoveEmptyEntries)

        let wordCount = words.Length
        let sentenceCount = sentences.Length
        let paragraphCount = paragraphs.Length

        let wordFrequency =
            words
            |> Seq.groupBy id
            |> Seq.map (fun (word, occurrences) -> word, Seq.length occurrences)
            |> Seq.sortByDescending snd

        let top10Words = wordFrequency |> Seq.truncate 10
        let avgSentenceLength = if sentenceCount > 0 then float wordCount / float sentenceCount else 0.0

        (wordCount, sentenceCount, paragraphCount, wordFrequency, top10Words, avgSentenceLength)

// Function to display the analysis results with an additional check for empty text
    let displayResults (textBox: TextBox) (resultLabel: Label) (progressBar: ProgressBar) =
        // Check if the text box is empty or contains the placeholder text
        if String.IsNullOrWhiteSpace(textBox.Text) || textBox.Text = "Drag and drop a file here or paste your text..." then
           MessageBox.Show("Please upload or enter some text to analyze.", "No Text To Analyze", MessageBoxButtons.OK, MessageBoxIcon.Warning) |> ignore
       else
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
   
           resultLabel.Text <- sprintf "Words : %d\nSentences : %d\nParagraphs : %d\nAverage Sentence Length : %.2f\n\n------------------\nTop 10 Words :\n%s\n------------------\nAll Words :\n%s\n------------------" 
                                wordCount sentenceCount paragraphCount avgSentenceLength top10String allWordsString

// Function to clear the text box, result label, and progress bar
    let clearForm (textBox: TextBox) (resultLabel: Label) (progressBar: ProgressBar) =
   // Check if the textBox contains the placeholder text
     if textBox.Text <> "Drag and drop a file here or paste your text..." then
            textBox.Clear()
            resultLabel.Text <- ""
            progressBar.Value <- 0

