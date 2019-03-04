@Echo OFF
Title Compress new updates of WinRAR

:: By Elektro H@cker

Set "NeededFiles=RAR.exe Rarreg.key Default.SFX"

For %%@ in (%NeededFiles%) Do (
	If not exist ".\%%@" (
		Set "Fail=True"
		Echo [*] %%@ not found.
	)
)

If defined FAIL (
	Echo+
	Echo [-] Nothing to compress. | MORE
) ELSE (
	Rename ".\RAR.exe" "Splitty_WinRAR_x86.exe" 1>NUL
	Del /Q ".\Splitty_WinRAR_x86.7z" 2>NUL
	(
		(
			Splitty_7zip_x86.exe a ".\Splitty_WinRAR_x86.7z" ".\Splitty_WinRar_x86.exe" ".\rarreg.key" ".\Default.SFX" -mx=9
		) && (
			Del /Q ".\Splitty_WinRar_x86.exe";".\rarreg.key";".\Default.SFX" 2>NUL
		)
		Echo+
		Echo [+] Compression Done. | MORE
	) || (
		Echo+
		Echo [-] Compression Failed. | MORE
	)
)

Pause&Exit