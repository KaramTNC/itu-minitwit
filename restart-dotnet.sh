#!/usr/bin/bash

API_PROJECT=/vagrant/src/Org.OpenAPITools
API_PROJECT_PORT=9001
FRONTEND_PROJECT=/vagrant/src/Web
FRONTEND_PROJECT_PORT=9002


function restart_server {
	touch $1/dotnet.log
	nohup dotnet run --project $1 > $1/dotnet.log &
	
	# if ! (ps aux | grep frontail)
	# then
		nohup frontail $1/dotnet.log -p $2 &
	# fi
}

killall dotnet
killall frontail

restart_server $FRONTEND_PROJECT $FRONTEND_PROJECT_PORT
restart_server $API_PROJECT $API_PROJECT_PORT





# Add Docker's official GPG key:
# sudo apt-get update
# sudo apt-get install ca-certificates curl
# sudo install -m 0755 -d /etc/apt/keyrings
# sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
# sudo chmod a+r /etc/apt/keyrings/docker.asc

# # Add the repository to Apt sources:
# echo \
#   "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu \
#   $(. /etc/os-release && echo "${UBUNTU_CODENAME:-$VERSION_CODENAME}") stable" | \
#   sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
# sudo apt-get update
# sudo apt install docker-compose-plugin