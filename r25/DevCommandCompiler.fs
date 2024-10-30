namespace tyrX.Core

open Autodesk.Revit.UI
open Autodesk.Revit.DB
open Autodesk.Revit.Attributes
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics

open System.Windows

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

//module load = 
//    let guid() = System.Guid.NewGuid() |> string
//    let addin(pthdll:string)(namcls:string)(guid:string) = 
//        sprintf """
//<?xml version="1.0" encoding="utf-8"?>
//<RevitAddIns>
//  <AddIn Type="Command">
//    <Assembly>%s</Assembly>
//    <FullClassName>%s</FullClassName>
//    <AddInId>%s</AddInId>
//    <Name>tyr</Name>
//    <Text>%s</Text>
//    <VendorId>chings.eu</VendorId>
//    <VendorDescription>Powered by Chings e.U.</VendorDescription>
//    <Description></Description>
//    <VisibilityMode>AlwaysVisible</VisibilityMode>
//    <Discipline>Any</Discipline>
//    <AvailabilityClassName></AvailabilityClassName>
//  </AddIn>
//</RevitAddIns> """ pthdll namcls (guid) (namcls.Split '.' |> Seq.last)

[<Regeneration(RegenerationOption.Manual)>]
[<TransactionAttribute(TransactionMode.Manual)>]
/// <summary>
/// Here write your summary
/// </summary>
type ``Develop Command Compiler``() =
    interface IExternalCommand with
        override x.Execute(cdata, msg, elset) =
            let d = debug()

            let t86_dt() = System.DateTime.Now.ToString("yyyyMMdd-HHmmss")

            let thisAsm = System.Reflection.Assembly.GetExecutingAssembly()

            let loc = thisAsm.Location

            //loc |> d.info
            
            let dir = loc |> System.IO.Path.GetDirectoryName

            //sprintf $"{dir} is executing directory." |> d.info

            let fn = System.IO.Path.GetTempFileName()
            
            let fnfsx = System.IO.Path.ChangeExtension(fn, ".fsx")

            //sprintf $"{fnfsx} is running." |> d.info

            let fndll = System.IO.Path.ChangeExtension(fn, ".dll")

            //sprintf $"{fndll} is running." |> d.info
            
            //// Put all scripts together as one fsx
            //(@"C:\Users\ching\github\tyrX\r25\cmd", ".fs")
            //|> System.IO.Directory.GetFiles |> List.ofSeq
            //|> List.map System.IO.File.ReadAllText
            //|> String.concat "\n"
            //|> fun content ->

            let content = @"C:\Users\ching\github\tyrX\r25\cmd\develop.fs" |> System.IO.File.ReadAllText

            //sprintf $"{content} is to be written." |> d.info
            System.IO.File.WriteAllText(fnfsx, content)

            let checker = FSharpChecker.Create()
            let ary = [|
                @"C:\Program Files\dotnet\sdk\8.0.401\FSharp\fsc.dll"; "-o"; fndll; "-a"; fnfsx;
                "-I"; @"C:\Program Files\dotnet\sdk\8.0.401\FSharp";
                "-I"; dir;
                "-r"; loc;
                "-r"; @"C:\Program Files\dotnet\sdk\8.0.401\FSharp\FSharp.Core.dll"
                "-r"; "RevitAPI"; "-r"; "RevitAPIUI"; "-r"; "FSharp.Compiler.Service.dll";
            |]
            let errs, exitcode = 
                checker.Compile(ary) 
                |> Async.RunSynchronously

            //// Create .addin and load as external command
            //let clsCmds = 
            //    thisAsm.GetTypes() // Get compiled command class names
            //    |> Array.filter(fun t -> t <> null)
            //    |> Array.filter(fun t -> t.IsClass)
            //    |> Array.filter(fun t -> t.GetInterface("IExternalCommand") <> null)
            //    |> Array.filter(fun t -> t.FullName.StartsWith "tyrX.Command")
            //    |> Array.map(fun t -> t.FullName)
            //    |> List.ofArray

            //clsCmds
            //|> List.map(fun clsCmd ->
            //    let xml = load.addin fndll clsCmd (load.guid())
            //    System.IO.Path.GetTempFileName()
            //    |> fun pth -> System.IO.Path.ChangeExtension(pth, ".addin")
            //    |> fun pth -> System.IO.File.WriteAllText(pth, xml); pth
            //    |> fun pth -> cdata.Application.LoadAddIn(pth)

            //)

            errs
            |> Array.partition(fun err -> err.Severity = FSharpDiagnosticSeverity.Error)
            |> fun(errors, rest) ->
                if errors.Length > 0 then
                    errors |> Array.map(fun error -> error.Message)
                    |> String.concat "\n"
                    |> fun str -> 
                        sprintf $"{str} is error message." |> d.info
                    Result.Failed
                else
                    //// Dealing with warnings
                    //rest |> Array.map(fun error -> error.Message)
                    //|> String.concat "\n"
                    //|> fun str ->sprintf $"{str} is warning message." |> d.info
                    
                    // Run compiled command
                    let asm = System.Reflection.Assembly.LoadFile(fndll)
                    let optcmd = 
                        asm.GetTypes()
                        |> Array.filter(fun t -> t <> null)
                        |> Array.filter(fun t -> t.IsClass)
                        |> Array.filter(fun t -> t.GetInterface("IExternalCommand") <> null)
                        |> Array.tryFind(fun t -> t.FullName = "tyrX.Command.Develop")

                    //sprintf $"{optcmd} is Option of command." |> d.info
            
                    match optcmd with
                    | None -> 
                        Result.Failed
                    | Some cmd ->
                        System.Activator.CreateInstance(cmd):?>IExternalCommand
                        |> fun iec -> iec.Execute(cdata, ref "", elset)

            //Result.Succeeded