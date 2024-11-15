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
/// Here write your summary
/// </summary>
type ``Develop Command Compiler``() =
    interface IExternalCommand with
        override x.Execute(cdata, msg, elset) =
            let d = debug()

            //let t86_dt() = System.DateTime.Now.ToString("yyyyMMdd-HHmmss")

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

            let content = sprintf $"{tyrX.Core.app.thisCmd}/develop.fs" |> System.IO.File.ReadAllText

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