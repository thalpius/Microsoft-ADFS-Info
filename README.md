# ADFS-Info

I created a small project to get the private keys for signing AD FS tokens, including the token-signing certificate.

For more information about Kerberoasting, please check my blog post:  
https://thalpius.com/2020/12/28/microsoft-defender-for-identity-adfsdump/

# Usage ADFS-Info
Run the tool on the AD FS server with Domain Admin privileges or with the AD FS service account:  
```ADFS-Info.exe```

To convert the private key to the correct binary format use xxd:  
```xxd -r -p keyinput.bin keyoutput.bin```

```PowerShell
$HexStringfile = "C:\users\thalpius\desktop\keyinput.bin"
$OutputFile = "C:\users\thalpius\desktop\keyoutput.bin"

if ($HexstringFile -ne "") {
  $HexString = Get-Content -readcount 0 -path $HexStringFile
  $HexString = $HexString[0]
  $count = $hexString.length
  $byteCount = $count/2
  $bytes = New-Object byte[] $byteCount
  $byte = $null

  $x = 0
  for ( $i = 0; $i -le $count-1; $i+=2 )
  { 
    $bytes[$x] = [byte]::Parse($hexString.Substring($i,2), [System.Globalization.NumberStyles]::HexNumber)
    $x += 1
  }

  if ($OutputFile -ne "") {
    set-content -encoding byte $OutputFile -value $bytes
  } else {
    write-host "No output file specified"
  }
} else{
  write-host "Error, no input file specified"
}
```

**Note**: Always use the last created private key.

# Screenshots

![Alt text](/Screenshots/ADFSInfo01.jpg?raw=true "ADFS Info")