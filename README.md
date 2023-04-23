# CIL Browser

**License:** [BSD 3-Clause](./LICENSE)

**Required runtime:** .NET Core 2.1+ or .NET 5+

Command line tool that generates a static website to browse disassembled Common Intermediate Language (CIL) code of a .NET assembly. The generated website contains CIL for the assembly manifest and types defined in the assembly. Supports syntax highlighting and navigation to types and methods by clicking on references. Also can generate static website for a source code directory (with syntax highlighting, but without code navigation). CIL Browser is cross-platform and runs on any operating system that supports .NET Core or .NET 5+.

The project idea is based on [SourceBrowser](https://github.com/KirillOsenkov/SourceBrowser). CIL Browser uses [CIL Tools](https://github.com/MSDN-WhiteKnight/CilTools) as a disassembler engine.

[Demo website](https://msdn-whiteknight.github.io/CilBrowser/html/) shows CIL and source code of CIL Browser itself.

## Command line syntax

    CilBrowser [options] <InputPath>

**InputPath** can be:

- Path of .dll, .exe or .winmd file, to browse disassembled CIL of the assembly;
- Path of directory, to browse source code from that directory;
- URL of git repository (ending with .git), to browse source code from that repository (for example, `https://github.com/octocat/example.git`).

**Options:**
    
    --output <OutputPath>

Path to the directory where to write generated website files. When not specified, runs HTTP server that dynamically renders web pages instead of generating static website. The server mode is only supported when input path points to an assembly file (when input is source directory, output path is required).

    --namespace <Namespace> 
    
Namespace filter. When specified, CIL Browser only renders types from the specified namespace. When not specified, renders types from all namespaces in the assembly. Namespace filter can only be used when input path points to an assembly file.

    --footer <Path>

Path to HTML file with custom footer content. The file should contain HTML markup (without enclosing `<html>` or `<body>` tags) that will be rendered at the bottom of each page. When not specified, custom footer is not rendered.

    --host <URL>

URL host part for server mode. Should contain protocol and port, but should not include a trailing slash. The default value is `http://localhost:8080`. Can only be specified when output path is not specified.

    --prefix <Prefix>

URL prefix part for server mode. Should start from a slash character. The default value is `/CilBrowser/`. Can only be specified when output path is not specified.

**Examples:**

`CilBrowser MyLibrary.dll` - Browse MyLibrary.dll in server mode.

`CilBrowser --output C:\Websites\MyLibrary MyLibrary.dll` - Generate static website for MyLibrary.dll in the output directory.

`CilBrowser --output C:\Websites\MyProject C:\repos\MyProject` - Generate static website for MyProject sources in the output directory.

## Server mode

CIL Browser supports rendering web pages dynamically on local HTTP endpoint instead of generating a static website; this functionality is referred to as *server mode*. To run app in server mode, do not specify `--output` option. Server mode is only supported when input path points to an assembly file. When invoked in server mode, CIL Browser listens on HTTP endpoint and renders webpages so you can open them in your web browser. To stop web server, press E in console window. By default, server listens on `http://localhost:8080/CilBrowser/` URL. To customize the URL, use `--host` and `--prefix` options.

**NOTE:** On Windows, if you choose actual external IP interface instead of localhost, you must have appropriate permissions to listen on it. For more information, see [Configuring namespace reservations](https://learn.microsoft.com/en-us/dotnet/framework/wcf/feature-details/configuring-http-and-https#configuring-namespace-reservations) in Windows documentation. 

## Configuration files

When generating website for a source directory, you can configure project-specific options by adding *configuration file* (`browser.cfg`) in the target directory. Configuration file is a text file that consists of `Name=Value` lines. Lines starting from ";" character are ignored. The configuration options are applied to the target directory and all of its subdirectories. Subdirectories can not override configuration with their own `browser.cfg` files (only top-level one is considered). If the configuration file does not exist, CIL Browser uses default options.

Supported configuration options:

**SourceControlURL** - URL of directory on source control server (for example, on GitHub) where the sources for this project are stored. When specified, CIL Browser renders "View in source control" link at the bottom of each file.

**SourceExtensions** - List of file extensions (with leading dot) that are considered source files. Extensions are separated by semicolon (;). CIL Browser will only generate webpages for files with the specified extensions.

**SourceEncoding** - Encoding used when reading source files. Can be `utf8` or `cpN`, where N is an integer ANSI codepage number. The default value is `utf8`. This option does not affect encoding used when writing output files (it is always UTF-8).

`browser.cfg` example:

```
; CIL Browser config
SourceControlURL=https://github.com/octocat/example/blob/main/
SourceExtensions=.cs;.csproj;.sln
SourceEncoding=cp1251
```
