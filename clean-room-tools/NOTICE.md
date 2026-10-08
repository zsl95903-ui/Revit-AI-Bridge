# Notice

This module does not include or redistribute:

- vendor DLLs or assemblies;
- decompiled vendor source code;
- Autodesk Revit API binaries;
- licenses, tokens, certificates, or user data.

`contracts/tool-contracts.json` contains interoperability metadata only
(tool names, categories, descriptions, transaction flags, and parameter
schemas). It is included so the self-developed implementations can be
generated, built, and tested without shipping any vendor implementation.

The Revit add-in projects reference `RevitAPI.dll` and `RevitAPIUI.dll` from a
local Revit installation. Those files are not part of this repository.
