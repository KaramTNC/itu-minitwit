terraform {
  encryption {
    key_provider "pbkdf2" "my_key_provider" {
      passphrase = var.state_password
    }
    method "aes_gcm" "my_method" {
      keys = key_provider.pbkdf2.my_key_provider
    }

    state {
      method = method.aes_gcm.my_method
      enforced = true
    }
  }
}


variable "state_password" {
  description = "Passphrase to encrypt the state file. It must be at least 16 characters long."
  sensitive = true
}
