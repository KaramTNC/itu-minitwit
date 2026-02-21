# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.configure("2") do |config|
  config.vm.box = 'digital_ocean'
  config.vm.box_url = "https://github.com/devopsgroup-io/vagrant-digitalocean/raw/master/box/digital_ocean.box"
  config.ssh.private_key_path = '~/.ssh/id_rsa'
  config.vm.synced_folder ".", "/vagrant", type: "rsync"

  config.vm.define "itu-minitwit", primary: true do |server|
    server.vm.provider :digital_ocean do |provider|
      provider.ssh_key_name = ENV["SSH_KEY_NAME"]
      provider.token = ENV["DIGITAL_OCEAN_TOKEN"]
      provider.image = 'ubuntu-22-04-x64'
      provider.region = 'fra1'
      provider.size = 's-1vcpu-1gb'
      provider.privatenetworking = true
    end

    # This script will only run when provisioning the machine.
    server.vm.provision "shell", inline: <<-SHELL
      # The following addresses an issue in DO's Ubuntu images, which still contain a lock file
      sudo fuser -vk -TERM /var/lib/apt/lists/lock
      sudo apt-get update
      # dotnet only gets installed when provisioning the machine
      sudo apt-get install -y dotnet-sdk-8.0
    SHELL

    server.vm.hostname = "itu-minitwit"

    # This script will always run when booting the machine
    server.vm.provision "shell", run: "always", inline: <<-SHELL
      cd /vagrant/src/Web
      # dotnet is always run when booting the machine
      echo "Starting the web server..."
      dotnet run &
      # Wait for the dotnet server to be running before printing the IP address
      while ! ss -tln | grep -q :5001; do sleep 1; done
      THIS_IP=`hostname -I | cut -d" " -f1`
      echo "================================================================="
      echo "=                            DONE                               ="
      echo "================================================================="
      echo "The web server is up at http://${THIS_IP}:5001"
    SHELL
  end

config.vm.define "itu-minitwit-api", primary: true do |server|
    server.vm.provider :digital_ocean do |provider|
      provider.ssh_key_name = ENV["SSH_KEY_NAME"]
      provider.token = ENV["DIGITAL_OCEAN_TOKEN"]
      provider.image = 'ubuntu-22-04-x64'
      provider.region = 'fra1'
      provider.size = 's-1vcpu-1gb'
      provider.privatenetworking = true
    end

    # This script will only run when provisioning the machine.
    server.vm.provision "shell", inline: <<-SHELL
      # The following addresses an issue in DO's Ubuntu images, which still contain a lock file
      sudo fuser -vk -TERM /var/lib/apt/lists/lock
      sudo apt-get update
      # dotnet only gets installed when provisioning the machine
      sudo apt-get install -y dotnet-sdk-8.0
    SHELL

    server.vm.hostname = "itu-minitwit-api"

    # This script will always run when booting the machine
    server.vm.provision "shell", run: "always", inline: <<-SHELL
      cd /vagrant/src/Org.OpenAPITools
      # dotnet is always run when booting the machine
      echo "Starting the web server..."
      dotnet run &
      # Wait for the dotnet server to be running before printing the IP address
      while ! ss -tln | grep -q :8080; do sleep 1; done
      THIS_IP=`hostname -I | cut -d" " -f1`
      echo "================================================================="
      echo "=                            DONE                               ="
      echo "================================================================="
      echo "The web API server is up at http://${THIS_IP}:8080"
    SHELL
  end
end