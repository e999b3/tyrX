namespace tyrX.Core

open System.Windows
open Autodesk.Revit.UI
open Autodesk.Revit.DB
open Autodesk.Revit.Attributes

type debug([<System.Runtime.CompilerServices.CallerFilePath>] ?pfile:string) =
    // debugged file name
    let fname = Array.last (pfile.Value.Split '\\')
    // Private _msg
    let mutable _mark = ""
    let mutable _msg = ""
    // Event
    let msgUpdate = new Event<_>()
    // Interfaces
    let w = new System.Windows.Window()
    let sv = new System.Windows.Controls.ScrollViewer()
    let p = new System.Windows.Controls.StackPanel()
    do  
        w.WindowStyle <- WindowStyle.ToolWindow
        w.WindowStartupLocation <- WindowStartupLocation.CenterOwner
        w.SizeToContent <- SizeToContent.WidthAndHeight
        w.MinWidth <- 300.0
        w.MinHeight <- 100.0
        sv.Content <- p
        w.Content <- sv
        //w.Show() |> ignore

    // publish Event to IEvent
    member this.window = w
    member this.updateMsg = msgUpdate.Publish
    member this.trace ([<System.Runtime.CompilerServices.CallerLineNumber>] ?line:int) = line.Value
    // _msg = info and when info changes
    member this.msg 
        with get() = _msg
        and set(str) =
            _msg <- str
            msgUpdate.Trigger()
    // Add function to IEvent
    member this.info(v:obj, [<System.Runtime.CompilerServices.CallerLineNumber>] ?line:int) =
        
        w.Title <- "tyrX © debugs: " + fname + " " + System.DateTime.Now.ToLongTimeString()
        w.Show() // Show window when getting info
        w.Topmost <- true
        match v with
        | :? string ->
            this.msg <- v:?>string
        | :? List<obj> as lst ->
            this.msg <- sprintf "c# List %A" (List.ofSeq lst) 
        | :? (obj list) | :? (seq<obj>) ->
            this.msg <- sprintf "f# List %A" v
        //| :? System.Collections.Generic.IList<obj> as ilst ->
        //    this.msg <- sprintf "c# List %A" (List.ofSeq ilst)
        | _ ->
            this.msg <- string v

        _mark <- System.DateTime.Now.ToLongTimeString() + (sprintf "|Line(%d) " (line.Value))
        // Write info with label
        //let l = new System.Windows.Controls.Label()
        //l.Content <- _mark + this.msg
        // Write infor with textblock
        let tbx = new System.Windows.Controls.TextBox()
        tbx.Text <- _mark + this.msg
        tbx.BorderThickness <- Thickness(0.0)
        p.Children.Add(tbx) |> ignore


module app = 
    let thisAsm = System.Reflection.Assembly.GetExecutingAssembly()
    let thisCmd = sprintf $"{__SOURCE_DIRECTORY__}/cmd"

module load = 
    let guid() = System.Guid.NewGuid() |> string
    let addin(pthdll:string)(namcls:string)(guid:string) = 
        sprintf """<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
  <AddIn Type="Command">
    <Assembly>%s</Assembly>
    <FullClassName>%s</FullClassName>
    <AddInId>%s</AddInId>
    <Name>tyr</Name>
    <Text>%s</Text>
    <VendorId>chings.eu</VendorId>
    <VendorDescription>Powered by Chings e.U.</VendorDescription>
    <Description></Description>
    <VisibilityMode>AlwaysVisible</VisibilityMode>
    <Discipline>Any</Discipline>
    <AvailabilityClassName></AvailabilityClassName>
  </AddIn>
</RevitAddIns> """ pthdll namcls (guid) (namcls.Split '.' |> Seq.last)

[<TransactionAttribute(TransactionMode.Manual)>]
type CoreApp() =
    interface IExternalApplication with
        override x.OnStartup(uiapp:UIControlledApplication) =
            let d = debug()
            sprintf $"Powered by Chings e.U." |> d.info

            app.thisAsm.GetTypes() |> List.ofArray
            |> List.filter(fun t -> t <> null)
            |> List.filter(fun t -> t.IsClass)
            |> List.filter(fun t -> t.GetInterface("IExternalCommand") <> null)
            |> List.map(fun t -> 
                // Find selected command compiler and load as add-in
                System.IO.Path.GetTempFileName()
                |> fun tmp -> System.IO.Path.ChangeExtension(tmp, ".addin")
                |> fun tmp ->
                    let xml = load.addin app.thisAsm.Location (sprintf $"{t.FullName}") (load.guid())
                    System.IO.File.WriteAllText(tmp, xml)
                    //sprintf $"Loading: {tmp}" |> d.info
                    tmp
            )
            |> List.iter(fun tmp -> tmp |> uiapp.LoadAddIn)

            Result.Succeeded

        override x.OnShutdown(uiapp) =
            Result.Succeeded

