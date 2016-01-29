## AWS Command Line Interface complement

This utility is designed to complement the official [aws cli](http://aws.amazon.com/cli/).

aws.exe leverages the AWS SDK and the instance `meta-data` to set A records on Route 53.

If you call this tool when a windows instance starts you effectively get a DynDNS ersatz!

### Configuration:

This tool load the default AWS credentials (the set by running `aws configure` after having installed the official [aws cli](http://aws.amazon.com/cli/).

You can change the default profile name and/or location as well as the default region in the app.config file.

### Usage:

    aws.exe [options]

#### Options:

```
-list -zones
-list -zone zone
-set -host [subdomain] [-ip [ip]|-public-ip|-local-ip]] [-ttl [ttl]]
```
Optional switches

```
-profile [name]
-profiles-location [pathToConfigFile]
-region [region]
```

Default values (unless overriden in `aws.config` file):

 - `profile`: "default"
 - `profiles-location`: "~\.aws\config" (current user folder)
 - `region`: "eu-west-1"

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

1. http://169.254.169.254/latest/meta-data/public-ipv4 is used to obtain the public IP address of the instance it runs on. EC2 classic and VPC instances are supported (VPC instances must be assigned a public or elastic IP at _launch time_).
2. http://169.254.169.254/latest/meta-data/local-ipv4 is used to optain the private IP address of the instance.
2. It only creates or updates A (IPv4) records.

### License

This project is licensed under the terms of the [MIT license](https://github.com/comsechq/sugar/blob/master/LICENSE.txt). 

By submitting a pull request for this project, you agree to license your contribution under the MIT license to this project.
