terraform {
  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = "~> 2.0"
    }    
  }

  backend "s3" {
    endpoints = {
      s3 = "https://${var.region}.digitaloceanspaces.com"
    }

    bucket = "remote-state-bucket-itu-minitwit-2b34b324b235b253b"
    key = "terraform.tfstate"

    # Deactivate a few AWS-specific checks
    skip_credentials_validation = true
    skip_requesting_account_id  = true
    skip_metadata_api_check     = true
    skip_region_validation      = true
    skip_s3_checksum            = true
    # This value needs to be a valid AWS region, but it does not affect the actual region of the bucket
    region                      = "us-east-1"

    use_lockfile = true
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