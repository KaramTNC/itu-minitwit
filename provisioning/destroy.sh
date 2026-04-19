#!/usr/bin/env bash
source _functions.sh

check_and_set_env

cd terraform
tofu destroy