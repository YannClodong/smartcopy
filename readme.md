# SmartCopy

SmartCopy is a utility to avoid useless write on the disk. 

It there is a conflict between the destination file and the source file it firstly check the checksum and only copy if they're different.

## Build
```bash
dotnet restore
dotnet build
```

## Usage
```bash
dotnet run . "source_directory" "destination_directory"
```
