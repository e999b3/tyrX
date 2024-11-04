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
            d.info (sprintf $"{this} is now really running.")
            // Write your code
            let uidoc = cdata.Application.ActiveUIDocument
            let selected = uidoc.Selection.GetElementIds()
            selected
            |> List.ofSeq
            |> List.iter(fun eid ->
                uidoc.Document.GetElement(eid)
                |> fun e -> e.Name |> d.info
            )

            try
            
                //use t = new Transaction(uidoc.Document, this |> string)
                //t.Start() |> ignore
                
                //t.Commit() |> ignore

                Result.Succeeded
            with
            | :? System.Exception as exc ->
                exc.Message |> d.info
                Result.Cancelled
