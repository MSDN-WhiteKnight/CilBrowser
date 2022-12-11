# CIL Browser

**License:** [BSD 3-Clause](./LICENSE)

**Required runtime:** .NET Core 2.1+

Generates static website with disassembled Common Intermediate Language (CIL) code of .NET assembly. The generated website contains CIL for the assembly manifest and types defined in the assembly. Supports syntax highlighting and navigation to types and methods by clicking on references.

The project idea is based on [SourceBrowser](https://github.com/KirillOsenkov/SourceBrowser). CIL Browser uses [CIL Tools](https://github.com/MSDN-WhiteKnight/CilTools) as a disassembler engine.

Command line syntax:

    CilBrowser [--output <OutputPath>] <AssemblyPath>

`[--output <OutputPath>]` - Path to the directory where to write generated website files. When not specified, writes to `./Output/`.

`<AssemblyPath>` - Path to the input assembly.
