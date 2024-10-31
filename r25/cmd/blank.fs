namespace tyrX.Command

open Autodesk.Revit.UI
open Autodesk.Revit.DB
open Autodesk.Revit.Attributes

[<Regeneration(RegenerationOption.Manual)>]
[<TransactionAttribute(TransactionMode.Manual)>]

type Blank() as this =
    interface IExternalCommand with
        override x.Execute(cdata, msg, elset) =
            let d = tyrX.Core.debug()
            sprintf $"{this} is running." |> d.info
            // Write your code
            //let uidoc = cdata.Application.ActiveUIDocument
            //let selected = uidoc.Selection.GetElementIds() |> Seq.map uidoc.Document.GetElement |> List.ofSeq

            try
                let t = new 
            
                //let t = new Transaction(uidoc.Document, this |> string)
                //t.Start() |> ignore
                
                //t.Commit() |> ignore

                Result.Succeeded
            with
            | :? System.Exception as exc ->
                exc.Message |> d.info
                Result.Cancelled
