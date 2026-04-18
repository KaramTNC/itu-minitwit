#!/usr/bin/env bash
source _functions.sh

check_and_set_env

cd terraform
tofu init
tofu apply --auto-approve

cd ../ansible

env ANSIBLE_HOST_KEY_CHECKING=False ansible-playbook playbook.yml -i inventory.ini