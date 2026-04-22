terraform {
  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = "~> 2.0"
    }    
  }
}

# All these variables must be set as TF_VAR_<variable_name> environment variables
variable "do_token" {}
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

provider "digitalocean" {
  token = var.do_token
}

data "digitalocean_ssh_key" "ssh_key" {
  name = var.do_ssh_key_name
}