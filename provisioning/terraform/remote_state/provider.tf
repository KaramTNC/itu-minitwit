terraform {
  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = "~> 2.0"
    }
  }
}

variable "do_token" {
  sensitive = true
}

variable "region" {
  description = "DigitalOcean server region"
  default = "fra1"
}

variable "spaces_access_id" {
  sensitive = true
}
variable "spaces_secret_key" {
  sensitive = true
}

provider "digitalocean" {
  token = var.do_token
  spaces_access_id = var.spaces_access_id
  spaces_secret_key = var.spaces_secret_key
}