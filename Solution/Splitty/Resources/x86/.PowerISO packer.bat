@Echo OFF
Title Compress new updates of PowerISO

:: By Elektro H@cker

Set "NeededFiles=PowerISO.exe Piso.exe"

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
	Rename ".\Piso.exe" "Splitty_Piso_x86.exe" 1>NUL
	Del /Q ".\Splitty_PowerISO_x86.7z" 2>NUL
	(
		(
			Splitty_7zip_x86.exe a ".\Splitty_PowerISO_x86.7z" ".\PowerISO.exe" ".\Splitty_Piso_x86.exe" -mx=9
		) && (
			Del /Q ".\PowerISO.exe";".\Splitty_Piso.exe" 2>NUL
		)
		Echo+
		Echo [+] Compression Done. | MORE
	) || (
		Echo+
		Echo [-] Compression Failed. | MORE
	)
)

Pause&Exit