# Splitty Change Log 📋

## v1.8 (2026) *(current)* 🆕
#### 🌟 Improvements:
 - Added a unit test project (MSTest) covering the core split logic.
 - The output directory can no longer be inside the input directory (the input directory is write-locked while splitting).
 - Split errors (copy failures, archiver failures, inaccessible folders...) are now reported to the user instead of being silently ignored and reported as success.
 - Protected system folders (hidden + system, e.g. "System Volume Information") are skipped when the input directory is a drive root.
 - Reduced CPU usage while waiting for the split to finish.
 - 7zip (x86 / x64) updated to version 26.06
 - RAR (x64) updated to version 7.23

#### 🛠️ Fixes:
 - Fixed: the input directory permissions were permanently altered. Its whole ACL was replaced by a single rule, and an explicit "allow" rule was left behind after splitting. Now only a temporary "deny" rule is added and later removed, preserving the existing rules.
 - Fixed: the denied rights mask was built by adding overlapping flags, which also denied unexpected rights (e.g. Traverse/Execute file).
 - Fixed: the input directory remained write-locked if the split failed with an error.
 - Fixed: in ISO mode the disc size was reduced on every split (the ISO overhead was subtracted from the selected disc size each time).
 - Fixed: if the first file was bigger than the disc size, "Disc 1" was skipped and, in ISO mode, the application tried forever to create its image. The same endless retry happened with an empty input directory, or whenever PowerISO failed.
 - Fixed: crash when splitting a network (UNC) input directory.
 - Fixed: crash when the custom size text was invalid (e.g. just a comma) or too big.
 - Fixed: the custom size was misinterpreted under English cultures ("1,5" was read as 15).
 - Fixed: changing the language replaced the disc count label of standard disc types with the custom storage text.
 - Fixed: the custom size disc count label was always shown in English when changing the size unit.
 - Fixed: the "reload source folder" question was shown several times per change (event handlers were added on every folder selection).
 - Fixed: the input directory size was silently miscalculated when a subfolder was not accessible (it was counted as 0 bytes). Now an error is shown.
 - Fixed: sizes of 1 PB or more were shown as an empty value.
 - Fixed: wrong reference path of TextBoxHint.dll in the project file.

## v1.7 (2024) 🔄
#### 🌟 Improvements:
 - Updated 7-Zip to version 23.01 (x86 and x64)
 - Updated RAR.exe to version 7.00 (Beta 4) (x86 and x64)
 - Updated to .NET Framework 4.8

## v1.1 to v1.6 (2013-2014) 🔄
*Change log information not available.*

## v1.0 (2013) 🔄
Initial Release.