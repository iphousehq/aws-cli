## AWS Command Line Interface complement

CLI utility designed to complement the official [aws cli](http://aws.amazon.com/cli/).

`comsec-aws.exe` leverages the AWS SDK and the instance `meta-data` to set A records on Route 53.

Executing this tool when an EC2 instance starts gives you a DynDNS ersatz!

### Configuration:

By default `comsec-aws.exe` will rely on the instance having been launched with an _IAM role_ that allows access to R53.

If you have the official [aws cli](http://aws.amazon.com/cli/) installed, you can create a profile of credentials  
(run `aws configure --profile [name]` from the command line, then use the `--profile [name]` parameter).

### Usage:

    comsec-aws.exe [options]

#### Options:

```bash
r53 list zones
r53 list zone [zone]
r53 set [subdomain] [--ip [ip]|--publicIp|--localIp]] [--ttl [ttl]]
```
Optional switches

```bash
--profile [name]
--profiles-location [pathToConfigFile]
--region [region]
```

### Examples

- List all hosted zones:
    `comsec-aws r53 list zones`

- List all records in the given zone:
    `comsec-aws r53 list AABBCCDDEE`

- List all records in the given zone with a named profile and region:
    `comsec-aws r53 list AABBCCDDEE --profile blah --profile-location "~\.aws\config" --region eu-west-1`

All examples below create a record or edit a matching one.

- Set record to a given IP address: `comsec-aws r53 set sub.domain.com --ip 10.1.2.3`
- Set record to the instance's public IP and also specify the TTL value: `comsec-aws r53 set sub.domain.com --publicIp --ttl 300`
- Set record to local IP: `comsec-aws r53 set internal.sub.domain.com --localIp --ttl 300`
- Update the TTL of an existing record: `comsec-aws r53 set internal.sub.domain.com --ttl 60`

### Download & Install:

1. Find the [latest release](https://github.com/comsechq/aws-cli/releases).
2. Extract the zip in a folder.
3. Run the command from the command line prompt.

### Notes

1. http://169.254.169.254/latest/meta-data/public-ipv4 is used to obtain the public IP address of the instance it runs on. EC2 classic and VPC instances are supported (VPC instances must be assigned a public or elastic IP at _launch time_).
2. http://169.254.169.254/latest/meta-data/local-ipv4 is used to optain the private IP address of the instance.
3. It only creates or updates A (IPv4) records.

### License

This project is licensed under the terms of the [MIT license](https://github.com/comsechq/sugar/blob/master/LICENSE.txt). 

By submitting a pull request for this project, you agree to license your contribution under the MIT license to this project.
