# dotnet-publish-ssh

Simple publish your .Net Core application to linux server via SSH.

# Usage

* Add this to `csproj` file:
```XML
  <ItemGroup>
    <DotNetCliToolReference Include="DotnetPublishSsh" Version="0.1.0" />
  </ItemGroup>
```

* Run `dotnet restore`
* Run `dotnet publish-ssh` with options:
```
Usage: dotnet publish-ssh [arguments] [options]
Arguments and options are the same as for `dotnet publish`
SSH specific options:
  --ssh-host *              Host address
  --ssh-port                Host port
  --ssh-user *              User name
  --ssh-password            Password
  --ssh-path *              Publish path on remote server
(*) required
```

# Example

`dotnet publish-ssh --ssh-host 10.0.0.1 --ssh-port 22 --ssh-user root --ssh-password secret --ssh-path /var/www/site`

# TODO

- [x] Just works
- [x] Password authentication
- [x] Private key file authentication
- [ ] Don't upload unmodified files (checksum)
- [ ] Config file
- [ ] Pre/post publish hooks on remote server
