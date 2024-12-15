open System
open System.Windows.Forms
open System.Drawing
open TextAnalysis.TextAnalyzer
open UIComponents.UIHelpers

// Main function
//application should run with a single-threaded apartment
[<STAThread>] 
do
//Create form and UI elements
    let form = createForm ()
    let textBox = createTextBox ()
    let progressBar = createProgressBar ()
    
// Create a scrollable panel for results
    let resultPanel, resultLabel = createScrollableResultPanel ()
    
// Buttons with custom colors
    let analyzeButton = createButton "Analyze" Color.LightGray
    let loadButton = createButton "Load File" Color.LightBlue
    let clearButton = createButton "Clear" Color.LightPink

// Create button panel and spacer
    let buttonPanel = createButtonPanel [analyzeButton; loadButton; clearButton]
    let spacer = createSpacer 15

// Set up event handlers
    analyzeButton.Click.Add(fun _ -> displayResults textBox resultLabel progressBar)
    loadButton.Click.Add(fun _ -> loadFile textBox ())
    clearButton.Click.Add(fun _ -> clearForm textBox resultLabel progressBar)

// Add controls to the form
    form.Controls.AddRange [| resultPanel; spacer; progressBar; buttonPanel; textBox |]

// Run the application
    Application.Run(form)


