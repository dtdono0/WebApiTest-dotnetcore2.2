create login [IIS APPPOOL\wat.localhost] from windows;
go
exec sp_addsrvrolemember N'IIS APPPOOL\wat.localhost', sysadmin
go
