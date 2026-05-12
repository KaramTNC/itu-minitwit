#!/usr/bin/env bash
# shellcheck disable=SC1091
source _functions.sh

AUTO_APPROVE=false

get_opts "$@"

check_and_set_env

if [ "$AUTO_APPROVE" = false ]; then
    choose_deployment_environment "Enter the number of the deployment environment you'd like to use:"
fi
export TF_VAR_environment=$DEPLOYMENT_ENVIRONMENT
if [ "$DEPLOYMENT_ENVIRONMENT" = "Staging" ]; then
    export TF_VAR_num_instances='{"web" = 2, "lb" = 1}'
fi

cd terraform || exit

cd remote_state || exit
tofu init
tofu apply --auto-approve || true # Ignore 409 HTTP error when the bucket already exists

cd ..

tofu init
tofu workspace select -or-create "$DEPLOYMENT_ENVIRONMENT"
tofu apply --auto-approve

cd ../ansible || exit

printf '\n%.0s' {1,8}

printf "%sATTENTION: Cloud infrastructure has been successfully acquired.
However, before installing and configuring software, you must access your
domain registrar's portal and create DNS A records for the root domain
and the (sub)domain(s) specified in group_vars/all.yml, listed below:%s\n" "$YELLOW" "$RESET"

grep 'prefix:' group_vars/all.yml | cut -d'"' -f2

printf "\n%sPoint these records to the following DigitalOcean reserved IP address: %s\n" "$YELLOW" "$RESET"
awk '/\[reserved_ip\]/ {getline; print}' inventory.ini

if [ "$AUTO_APPROVE" = "false" ]; then
    printf "\n%sOnce you've created the records, press Enter to proceed with the software configuration on the servers.%s\n" "$YELLOW" "$RESET"
    read -rp "Press Enter to continue..."
fi

ansible-galaxy install -r requirements.yml
ansible-playbook playbook.yml