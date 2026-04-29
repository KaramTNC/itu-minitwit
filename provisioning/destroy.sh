#!/usr/bin/env bash
source _functions.sh

check_and_set_env

cd terraform
CURRENT_WORKSPACE=$(tofu workspace show)

choose_deployment_environment "You have currently set the following environment: $CURRENT_WORKSPACE\n
Please enter the desired number for the environment you want to destroy:"

if tofu workspace select $DEPLOYMENT_ENVIRONMENT; then
    tofu destroy
    tofu workspace select default
    tofu workspace delete $DEPLOYMENT_ENVIRONMENT
fi