# Minitwit

A DevOps project made by Group D, composed of:

* Madeleine Jakobsen <majak@itu.dk>
* Tim Hounsgaard <thou@itu.dk>
* Mohamed Karam Haybout <mhay@itu.dk>
* Oriol Grau Moragues <s25137@itu.dk>
* Jordan Carter Cherry <s25121@itu.dk>

## Provisioning
### Pre-requisites
- You must either [create an **SSH key**](https://www.digitalocean.com/community/tutorials/how-to-set-up-ssh-keys-on-ubuntu-22-04) or use an existing on your local machine. Afterwards, you must add it to your DigitalOcean Team to allow access to the VPS ([official documentation page](https://docs.digitalocean.com/platform/teams/how-to/upload-ssh-keys/)).
  - The key must be created using RSA and named `id_rsa`, as the script will read the private key's contents from `~/.ssh/id_rsa`
- You must either create or have access to an existing personal access **token** to control your DigitalOcean resources ([official documentation page](https://docs.digitalocean.com/reference/api/create-personal-access-token/)).


With that done, set the following two environment variables:
- `SSH_KEY_NAME` with the name of the **SSH key** previously added to your DigitalOcean Team.
- `DIGITAL_OCEAN_TOKEN` with the **token** previously created.


### Provisioning with Vagrant
The server can be provisioned by simply cloning our repository and running the specified Vagrant configuration, with the following commands.
```console
git clone git@github.com:KaramTNC/itu-minitwit.git
vagrant up
```