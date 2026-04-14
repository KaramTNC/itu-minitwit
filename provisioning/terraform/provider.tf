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
variable "pvt_key" {}
variable "do_ssh_key_name" {}

provider "digitalocean" {
  token = var.do_token
}

data "digitalocean_ssh_key" "ssh_key" {
    name = var.do_ssh_key_name
}