FileConsolidator
================

This utility moves files from a lot of folders into one.

Somehow my phone decided to split my pictures into multiple folders and there is no way to understand where the latest pictures are. So I spent a couple of hours writing this program so that it can consolidate all files for me in seconds.

The program has no tests. In order to properly test it, the file system provider must be abstracted into another class from MainViewModel.cs. Then it can be mocked and the MainViewModel.cs can be tested properly.
