resource "digitalocean_droplet" "itu-minitwit-1" {
  name = "itu-minitwit-1"
  region = "fra1"
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