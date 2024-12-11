namespace UIHelpers
open System
open System.Windows.Forms
open System.IO
open System.Drawing

 module UIHelpers = 
// Function to create the form
    let createForm () =
      let form = new Form(Text = "Text Analyzer", Width = 900, Height = 700, StartPosition = FormStartPosition.CenterScreen, BackColor = Color.FromArgb(240, 240, 240))
      // Enable drag-and-drop on the form
      form.AllowDrop <- true

    // Handle the DragEnter event
      form.DragEnter.Add(fun e ->
        if e.Data.GetDataPresent(DataFormats.FileDrop) then
            e.Effect <- DragDropEffects.Copy
        else
            e.Effect <- DragDropEffects.None
      )

    // Handle the DragDrop event
      form.DragDrop.Add(fun e ->
        let filePaths = e.Data.GetData(DataFormats.FileDrop) :?> string[]
        if filePaths.Length > 0 then
            let fileContent = File.ReadAllText(filePaths.[0])
            if String.IsNullOrWhiteSpace(fileContent) then
                MessageBox.Show("The file is empty. Please upload a valid file.", "Empty File", MessageBoxButtons.OK, MessageBoxIcon.Warning) |> ignore
            else
    // Find the TextBox control and set its text
                let textBox = 
                    form.Controls 
                    |> Seq.cast<Control> 
                    |> Seq.tryFind (fun c -> c :? TextBox)
                    |> Option.map (fun c -> c :?> TextBox)
                match textBox with
                | Some tb -> tb.Text <- fileContent
                | None -> MessageBox.Show("TextBox not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore    
      )
      form
// Function to create the text box
    let createTextBox () =
        new TextBox(Multiline = true, Dock = DockStyle.Top, Height = 200, ScrollBars = ScrollBars.Vertical, Font = new Font("STIXSizeTwoSym", 10f, FontStyle.Bold), Padding = Padding(15))

// Function to create a button
    let createButton text color =
        new Button(Text = text, Height = 40, Width = 120, BackColor = color, FlatStyle = FlatStyle.Popup, Font = new Font("STIXSizeTwoSym", 12f, FontStyle.Bold), Margin = Padding(110,10,10,10))

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
        let dialogResult = openFileDialog.ShowDialog()
        if dialogResult = DialogResult.OK then
            let fileContent = File.ReadAllText(openFileDialog.FileName)
            if String.IsNullOrWhiteSpace(fileContent) then
                MessageBox.Show("The file is empty. Please upload a valid file.", "Empty File", MessageBoxButtons.OK, MessageBoxIcon.Warning) |> ignore
            else
                textBox.Text <- fileContent

