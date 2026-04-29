variable "num_instances" {
  description = "Number of Droplets to create for each environment + total load balancers"
  default = {
    "web" = 2,
    "lb" = 2
  }
  type = map(number)

  validation {
    condition = contains(keys(var.num_instances), "web") && contains(keys(var.num_instances), "lb")
    error_message = "Number of instances must be specified for \"web\", as well as the load balancers with \"lb\"."
  }
}

variable "instance_prefix" {
  description = "Prefix for Droplet names"
  default = "itu-minitwit-DEPLOY-TEST"
}

resource "digitalocean_droplet" "itu-minitwit" {
  count = var.num_instances["web"]
  name = "${var.instance_prefix}-${count.index + 1}"
  region = var.region
  image = var.image
  size = "s-2vcpu-2gb"
  ssh_keys = [ data.digitalocean_ssh_key.ssh_key.id ]
  tags = ["itu-minitwit"]

  connection {
    host = self.ipv4_address
    user = "root"
    type = "ssh"
    private_key = file(var.private_key_path)
    timeout = "2m"
  }
}

resource "digitalocean_droplet" "itu-minitwit-monitoring" {
  name = "${var.instance_prefix}-monitoring"
  region = var.region
  image = var.image
  size = "s-1vcpu-1gb"
  ssh_keys = [ data.digitalocean_ssh_key.ssh_key.id ]
  tags = ["itu-minitwit"]

  connection {
    host = self.ipv4_address
    user = "root"
    type = "ssh"
    private_key = file(var.private_key_path)
    timeout = "2m"
  }
}

resource "digitalocean_droplet" "itu-minitwit-load-balancer" {
  count = var.num_instances["lb"]
  name = "${var.instance_prefix}-load-balancer-${count.index + 1}"
  region = var.region
  image = var.image
  size = "s-1vcpu-1gb"
  ssh_keys = [ data.digitalocean_ssh_key.ssh_key.id ]
  tags = ["itu-minitwit"]

  connection {
    host = self.ipv4_address
    user = "root"
    type = "ssh"
    private_key = file(var.private_key_path)
    timeout = "2m"
  }
}

resource "digitalocean_reserved_ip" "itu-minitwit-reserved-ip" {
  droplet_id = digitalocean_droplet.itu-minitwit-load-balancer[0].id
  region = var.region

  lifecycle {
    ignore_changes = [ droplet_id ]
  }
}

# Write the Ansible inventory file with the Droplets' IP addresses and other variables
resource "local_file" "ansible_inventory" {
  filename = "${path.module}/../ansible/inventory.ini"
  
  # First argument is the template file; second argument is a map with the values of the variables in that template
  content = templatefile("${path.module}/ansible_inventory.tftpl", {
    # Select only the web Droplets by slicing the list from index 0 until N, where N = num_instances["web"] 
    web_ips   = slice(digitalocean_droplet.itu-minitwit[*].ipv4_address, 0, var.num_instances["web"])
    web_names = slice(digitalocean_droplet.itu-minitwit[*].name, 0, var.num_instances["web"])
    web_private_ips = slice(digitalocean_droplet.itu-minitwit[*].ipv4_address_private, 0, var.num_instances["web"])
    
    lb_ip   = digitalocean_droplet.itu-minitwit-load-balancer[*].ipv4_address
    lb_name = digitalocean_droplet.itu-minitwit-load-balancer[*].name
    lb_private_ip = digitalocean_droplet.itu-minitwit-load-balancer[*].ipv4_address_private

    monitoring_ip   = digitalocean_droplet.itu-minitwit-monitoring.ipv4_address
    monitoring_name = digitalocean_droplet.itu-minitwit-monitoring.name
    monitoring_private_ip = digitalocean_droplet.itu-minitwit-monitoring.ipv4_address_private

    reserved_ip = digitalocean_reserved_ip.itu-minitwit-reserved-ip.ip_address
    
    private_key_path = var.private_key_path
    reserved_ip = digitalocean_reserved_ip.itu-minitwit-reserved-ip.ip_address
    keepalived_password = var.keepalived_password
    do_token = var.do_token
  })
}