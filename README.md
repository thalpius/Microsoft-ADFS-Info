# ADFS-Info

I created a small project to get the private keys for signing AD FS tokens including the correct certificate.

For more information about Kerberoasting, please check my blog post:  
https://thalpius.com/2020/12/28/microsoft-defender-for-identity-adfsdump/

# Usage ADFS-Info
Run the tool on the AD FS server with Domain Admin privileges or with the AD FS service account:  
```ADFS-Info.exe```

To convert the private key to the correct binary format use xxd:  
```xxd -r -p keyinput.bin keyoutput.bin```
