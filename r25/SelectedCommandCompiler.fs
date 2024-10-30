namespace tyrX.Core

open Autodesk.Revit.UI
open Autodesk.Revit.DB
open Autodesk.Revit.Attributes
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics

open System.Windows

[<Regeneration(RegenerationOption.Manual)>]
[<TransactionAttribute(TransactionMode.Manual)>]
/// <summary>
/// Choose one command to compile from a drop-down list
/// </summary>
type ``Selected Command Compiler``() as this =
    interface IExternalCommand with
        override x.Execute(cdata, msg, elset) =
            let d = debug()
            sprintf $"{this} is running." |> d.info
            let t86_dt() = System.DateTime.Now.ToString("yyyyMMdd-HHmmss")

            let thisAsm = System.Reflection.Assembly.GetExecutingAssembly()

            let loc = thisAsm.Location

            let dir = loc |> System.IO.Path.GetDirectoryName

            // Create temp file
            let fn = System.IO.Path.GetTempFileName()
            
            let fnfsx = System.IO.Path.ChangeExtension(fn, ".fsx")

            let fndll = System.IO.Path.ChangeExtension(fn, ".dll")
            sprintf $"{fndll}" |> d.info

            // Get user input from drop-down list
            let pthdir = @"C:\Users\ching\github\tyrX\r25\cmd"
            let fils = System.IO.Directory.GetFiles(pthdir, "*.fs") |> List.ofArray
            fils
            |> List.map(fun pth -> pth |> System.IO.Path.GetFileNameWithoutExtension, pth)
            |> fun dict ->
                let itms = dict |> List.map fst

                let cobbox = System.Windows.Controls.ComboBox(
                    MaxWidth = 600.0, Height = 30.0, Margin = System.Windows.Thickness(10.0)
                )
                itms |> List.iter(fun itm ->
                    cobbox.Items.Add(itm) |> ignore
                )

                let w = new System.Windows.Window(
                    MinWidth = 300.0, MinHeight = 100.0
                )
                let p = new System.Windows.Controls.StackPanel()

                cobbox |> p.Children.Add |> ignore
                w.Content <- p
                w.WindowStartupLocation <- System.Windows.WindowStartupLocation.CenterScreen
                w.SizeToContent <- System.Windows.SizeToContent.WidthAndHeight
                
                let dlg = w.ShowDialog()

                match dict |> List.tryFind(fun (nam, _) -> nam = (cobbox.SelectedItem |> string)) with
                | None -> 
                    "error" |> d.info
                    Result.Cancelled
                | Some v -> 
                    v |> snd |> d.info
                    let pth = v |> snd
                    let lines = pth |> System.IO.File.ReadAllLines |> List.ofSeq
                    let nspace = lines.Head.Split ' ' |> Seq.tail |> String.concat " "
                    let typnam = 
                        // Since the type interfacing IExternalCommand will be the last type in script
                        let optIdxTyp = lines |> List.tryFindIndexBack(fun ln -> ln.StartsWith "type")
                        match optIdxTyp with
                        | None ->
                            failwith "Cannot find type in script."
                        | Some idxTyp ->
                            let lnnxt = (lines.[idxTyp + 1])
                            if not (lnnxt.Contains "IExternalCommand") then
                                failwith "Not a type interfacing IExternalCommand"
                            else
                                let lntyp = lines.[idxTyp]

                                ((lntyp.Split ' ' |> Seq.tail |> String.concat " ").Split '(' |> Seq.head).Replace("``", "")

                    sprintf $"{typnam} is selected type name." |> d.info
                    let content = pth |> System.IO.File.ReadAllText
                    // sprintf $"{content} is to be written." |> d.info
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
                                |> Array.tryFind(fun t -> t.FullName = sprintf $"tyrX.Command.{typnam}")

                            //sprintf $"{optcmd} is Option of command." |> d.info
            
                            match optcmd with
                            | None -> 
                                Result.Failed
                            | Some cmd ->
                                System.Activator.CreateInstance(cmd):?>IExternalCommand
                                |> fun iec -> iec.Execute(cdata, ref "", elset)