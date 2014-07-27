## AWS Command Line Interface complement

This utility is designed to complement the official [aws cli](http://aws.amazon.com/cli/).

aws.exe leverages the AWS SDK for .NET to create or update A records on Route 53.

If you call this tool when a windows instance starts you effectively get a DynDNS ersatz!

### Configuration:

This tool tries to load the default AWS credentials set for example by running `aws configure` after having installed the official [aws cli](http://aws.amazon.com/cli/).

You can change the default profile name and/or location as well as the default region in the app.config file.

### Usage:

aws.exe [options]
            
#### Options:
    
    [-region [region]] -list -zones
    [-region [region]] -list -zone zone
    [-region [region]] -set -host [subdomain] [-ip [ip]|-public-ip|-local-ip]] [-ttl [ttl]]

### Examples

- List all hosted zones:
    `aws -list -zones`

- List all records in the given zone:
    `aws -list -zone-id AABBCCDDEE`

All examples below create a record or edit a matching one.

- Set record to a given IP address: `aws -set -host sub.domain.com -ip 10.1.2.3`
- Set record to the instance's public IP and also specify the TTL value: `aws -set -host sub.domain.com -public-ip -ttl 300`
- Set record to local IP: `aws -set -host internal.sub.domain.com -local-ip -ttl 300`
- Update the TTL of an existing record: `aws -set -host internal.sub.domain.com -ttl 60`

### Download & Install:

1. Find the [latest release](https://github.com/comsechq/aws-cli/releases).
2. Extract the zip in a folder.
3. Run the command from the command line prompt.

### Notes

1. This tool queries http://instance-data/latest/meta-data/public-ipv4 to obtain the public IP address of the instance it runs on. If do not execute this tool on a Amazon EC2 instance an `ApplicationException` will be thrown.
2. It only creates or updates A (IPv4) records.
