![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/Esticonv/3ed6031e8dfea3e01c049747ecc85e55/raw/code-coverage.json)

# Wisp.Comtrade
A C# library that allows to read and write COMTRADE (IEEE C37.111 / IEC 60255-24) files.

This repository has the following features:
- Supports [1991](https://standards.ieee.org/ieee/C37.111/2644/), [1999](https://standards.ieee.org/ieee/C37.111/2645/) and [2013](https://standards.ieee.org/ieee/C37.111/3795/) revisions.
- Supports ASCII and Binary files.
- Supports `.cfg`, `.dat` and `.cff` files. Does not support `.hdr` files (but no one needs them yet).

## Example

See this [unit test](https://github.com/Esticonv/Wisp.Comtrade/blob/master/ComtradeTests/RecordWriterTest.cs)

## Credits

Code improvements are taken from the the indirect fork repository [ComtradeHandler writen by Gabriel De La Parra](https://github.com/gabrieldelaparra/ComtradeHandler) 



