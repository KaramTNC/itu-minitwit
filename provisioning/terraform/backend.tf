terraform {
  backend "s3" {
    endpoints = {
      s3 = "https://${var.region}.digitaloceanspaces.com"
    }

    bucket = "remote-state-bucket-itu-minitwit-2b34b324b235b253b2"
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