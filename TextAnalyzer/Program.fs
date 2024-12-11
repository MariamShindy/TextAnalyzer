open System
open System.Windows.Forms
open System.Drawing
open TextAnalyzer.TextAnalyzer
open UIHelpers.UIHelpers

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

    //// Add controls to the form
    form.Controls.AddRange [| resultPanel; spacer; progressBar; buttonPanel; textBox |]

    // Run the application
    Application.Run(form)

    //Allow drag and drop