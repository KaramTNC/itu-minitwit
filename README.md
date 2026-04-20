# Minitwit

A DevOps project made by Group D, composed of:

* Madeleine Jakobsen <majak@itu.dk>
* Tim Hounsgaard <thou@itu.dk>
* Mohamed Karam Haybout <mhay@itu.dk>
* Oriol Grau Moragues <s25137@itu.dk>
* Jordan Carter Cherry <s25121@itu.dk>

## Deployment
### Pre-requisites
- You must either [create an **SSH key**](https://www.digitalocean.com/community/tutorials/how-to-set-up-ssh-keys-on-ubuntu-22-04) or use an existing on your local machine. Afterwards, you must add it to your DigitalOcean Team to allow access to the VPS ([official documentation page](https://docs.digitalocean.com/platform/teams/how-to/upload-ssh-keys/)).
  - The key must be created using RSA and named `id_rsa`, as the script will read the private key's contents from `~/.ssh/id_rsa`
- You must either create or have access to an existing personal access **token** to control your DigitalOcean resources ([official documentation page](https://docs.digitalocean.com/reference/api/create-personal-access-token/)).


With that done, set the following two environment variables:
- `SSH_KEY_NAME` with the name of the **SSH key** previously added to your DigitalOcean Team.
- `DIGITAL_OCEAN_TOKEN` with the **token** previously created.

### Deploying the infrastructure
Before deploying, review the following files to adjust the configuration to your needs:
- [**`resources.tf`**](provisioning/terraform/resources.tf): adjust the number of instances to be created for each component and their naming pattern.
  ```
  default = {
    "prod" = 2,
    "staging" = 0,
    "lb" = 2
  }
  ```
  ```
  variable "instance_prefix" {
    description = "Prefix for Droplet names"
    default = "itu-minitwit"
  } 
  ```

- [**`provider.yml`**](provisioning/terraform/provider.yml): Specify the server region.
  ```
  variable "region" {
    description = "DigitalOcean server region"
    default = "fra1"
  }
  ```
    
- [**`env.template`**](provisioning/env.template): Make a copy of this file called `.env` with the adequate value for each variable.

Infrastructure can be deployed by just executing the [`deploy.sh`](provisioning/deploy.sh) script, which will create the necessary resources on DigitalOcean and configure them using Ansible. The creation of DNS A records requires manual intervention. The script will output which subdomains need DNS A records and to which address they must point. Upon setting up your (sub)domain(s) on your chosen registrar, confirm your action and the provisioning process will continue.

## Deletion
To tear down all infrastructure created during deployment, execute the [`destroy.sh`](provisioning/destroy.sh) script.
> ⚠️ **Warning**: this action is irreversible and all data stored on the VMs will be lost.