variable "num_instances" {
  description = "Number of Droplets to create for each component"
  default = {
    "web" = 2,
    "lb" = 2
  }
  type = map(number)

  validation {
    condition = contains(keys(var.num_instances), "web") && contains(keys(var.num_instances), "lb")
    error_message = "Number of instances must be specified for \"web\" and the load balancers with \"lb\"."
  }
}

# All these variables must be set as TF_VAR_<variable_name> environment variables (which deploy.sh handles automatically)
variable "do_token" {
  sensitive = true
}
variable "private_key_path" {}
variable "do_ssh_key_name" {}

variable "spaces_access_id" {
  sensitive = true
}
variable "spaces_secret_key" {
  sensitive = true
}

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

variable "instance_prefix" {
  description = "Prefix for resources names"
  default = "itu-minitwit"
}

variable "environment" {
  description = "Deployment environment (Development, Staging, Production)"
  default = "Staging"
  validation {
    condition = contains(["Development", "Staging", "Production"], var.environment)
    error_message = "Environment must be one of: Development, Staging, Production."
  }
}