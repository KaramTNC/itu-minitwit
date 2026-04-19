#!/usr/bin/env bash
source _functions.sh

check_and_set_env

cd terraform
tofu init
tofu apply --auto-approve

cd ../ansible

printf '\n%.0s' {1,8}
YELLOW='\033[1;33m'
RESET='\033[0m'

printf "${YELLOW}ATTENTION: Cloud infrastructure has been successfully acquired.
However, before installing and configuring software, you must access your
domain registrar's portal and create DNS A records for the root domain
and the (sub)domain(s) specified in group_vars/all.yml, listed below:${RESET}\n"

grep 'prefix:' group_vars/all.yml | cut -d'"' -f2

printf "\n${YELLOW}Point these records to the following DigitalOcean reserved IP address: ${RESET}\n"
awk '/\[reserved_ip\]/ {getline; print}' inventory.ini

printf "\n${YELLOW}Once you've created the records, press Enter to proceed with the software configuration on the servers.${RESET}\n"
read -p "Press Enter to continue..."

ansible-galaxy install -r requirements.yml
env ANSIBLE_HOST_KEY_CHECKING=False ansible-playbook playbook.yml -i inventory.ini