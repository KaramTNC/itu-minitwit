variable "num_instances" {
  description = "Number of Droplets to create (behind the load balancer)"
  default = {
    "prod" = 2,
    "staging" = 1
  }
  type = map(number)

  validation {
    condition = contains(keys(var.num_instances), "prod") && contains(keys(var.num_instances), "staging")
    error_message = "Number of instances must be specified for \"prod\" and \"staging\"."
  }
}

variable "instance_prefix" {
  description = "Prefix for Droplet names"
  default = "itu-minitwit"
}

resource "digitalocean_droplet" "itu-minitwit" {
  count = var.num_instances["prod"] + var.num_instances["staging"]
  name = (count.index < var.num_instances["prod"]) ? "${var.instance_prefix}-${count.index + 1}" : "${var.instance_prefix}-staging-${count.index - var.num_instances["prod"] + 1}"
  region = var.region
  image = "ubuntu-22-04-x64"
  size = "s-1vcpu-1gb"
  ssh_keys = [ data.digitalocean_ssh_key.ssh_key.id ]

  connection {
    host = self.ipv4_address
    user = "root"
    type = "ssh"
    private_key = file(var.pvt_key)
    timeout = "2m"
  }
  
  provisioner "remote-exec" {
    script = "itu-minitwit.sh"
  }
}

resource "digitalocean_droplet" "itu-minitwit-load-balancer" {
  name = "itu-minitwit-load-balancer"
  region = var.region
  image = "ubuntu-22-04-x64"
  size = "s-1vcpu-1gb"
  ssh_keys = [ data.digitalocean_ssh_key.ssh_key.id ]

  connection {
    host = self.ipv4_address
    user = "root"
    type = "ssh"
    private_key = file(var.pvt_key)
    timeout = "2m"
  }
  
  provisioner "remote-exec" {
    script = "itu-minitwit.sh"
  }
}