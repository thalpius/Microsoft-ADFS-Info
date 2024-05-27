> [!IMPORTANT]
> "This repo is now in 'Blue Screen' mode—archived and frozen in time!"

# ADFS-Info

I created a small project to get the private key and token-signing certificate from an AD FS server to create forged tokens.

For more information about this attack, please check my blog post:  
https://thalpius.com/2020/12/28/microsoft-defender-for-identity-adfsdump/

# Usage ADFS-Info
Run the tool on the AD FS server with Domain Admin privileges or with the AD FS service account:  
```cmd
ADFS-Info.exe
```

To convert the private key to the correct binary format on Linux use xxd:  
```bash
xxd -r -p keyinput.bin keyoutput.bin
```

To convert the private key to the correct binary format on Windows use PowerShell:  

```PowerShell
$HexStringfile = "C:\users\thalpius\desktop\keyinput.bin"
$OutputFile = "C:\users\thalpius\desktop\keyoutput.bin"

$HexString = Get-Content -readcount 0 -path $HexStringFile
$HexString = $HexString[0]
$count = $hexString.length
$byteCount = $count/2
$bytes = New-Object byte[] $byteCount
$byte = $null

$x = 0
for ( $i = 0; $i -le $count-1; $i+=2 ) { 
  $bytes[$x] = [byte]::Parse($hexString.Substring($i,2), [System.Globalization.NumberStyles]::HexNumber)
  $x += 1
}

Set-Content -encoding byte $OutputFile -value $bytes
```

**Note**: Always use the last created private key.

# Screenshot

![Alt text](/Screenshots/ADFSInfo01.jpg?raw=true "ADFS Info")
