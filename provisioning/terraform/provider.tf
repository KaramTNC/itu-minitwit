terraform {
  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = "~> 2.0"
    }    
  }
}

# All these variables must be set as TF_VAR_<variable_name> environment variables (which deploy.sh handles automatically)
variable "do_token" {
  sensitive = true
}
variable "private_key_path" {}
variable "do_ssh_key_name" {}

variable "keepalived_password" {
  description = "Password for Keepalived VRRP authentication"
  sensitive = true
}

variable "region" {
  description = "DigitalOcean server region"
  default = "fra1"
}

variable "image" {
  description = "OS image to use for the Droplets"
  default = "ubuntu-22-04-x64"
}

provider "digitalocean" {
  token = var.do_token
}

data "digitalocean_ssh_key" "ssh_key" {
  name = var.do_ssh_key_name
}