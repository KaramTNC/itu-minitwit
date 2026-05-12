resource "digitalocean_spaces_bucket" "remote_state_bucket" {
  name   = "remote-state-bucket-itu-minitwit-2b34b324b235b253b2"
  region = var.region

  versioning {
    enabled = true
  }
}