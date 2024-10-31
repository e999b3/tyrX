namespace tyrX.Command

open Autodesk.Revit.UI
open Autodesk.Revit.DB
open Autodesk.Revit.Attributes

[<Regeneration(RegenerationOption.Manual)>]
[<TransactionAttribute(TransactionMode.Manual)>]

type Develop() as this =
    interface IExternalCommand with
        override x.Execute(cdata, msg, elset) =
            let d = tyrX.Core.debug()
            d.info (sprintf $"{this} is now really running again.")
            // Write your code
            //let uidoc = cdata.Application.ActiveUIDocument
            //let selected = uidoc.Selection.GetElementIds() |> Seq.map uidoc.Document.GetElement |> List.ofSeq

            //let pthdir = @"C:\Users\ching\github\tyrX\r25\cmd"
            //let fils = System.IO.Directory.GetFiles(pthdir, "*.fs") |> List.ofArray
            //fils
            //|> List.map(fun pth -> pth |> System.IO.Path.GetFileNameWithoutExtension, pth)
            //|> fun dict ->
                
            
            //    let itms = dict |> List.map fst

            //    let cobbox = System.Windows.Controls.ComboBox(
            //        MaxWidth = 600.0, Height = 30.0, Margin = System.Windows.Thickness(10.0)
            //    )
            //    itms |> List.iter(fun itm ->
            //        cobbox.Items.Add(itm) |> ignore
            //    )

            //    let w = new System.Windows.Window(
            //        Width = 150.0, Height = 100.0
            //    )
            //    let p = new System.Windows.Controls.StackPanel()

            //    cobbox |> p.Children.Add |> ignore
            //    w.Content <- p
            //    w.WindowStartupLocation <- System.Windows.WindowStartupLocation.CenterScreen
                
            //    let dlg = w.ShowDialog()
            //    if dlg.HasValue then
            //        d.info dlg.Value

            //        match dict |> List.tryFind(fun (nam, _) -> nam = (cobbox.SelectedItem |> string)) with
            //        | Some v -> v |> snd |> d.info
            //        | None -> "error" |> d.info

            //    else
            //        d.info "null"


            try
            
                //let t = new Transaction(uidoc.Document, this |> string)
                //t.Start() |> ignore
                
                //t.Commit() |> ignore

                Result.Succeeded
            with
            | :? System.Exception as exc ->
                exc.Message |> d.info
                Result.Cancelled
