#!/usr/bin/env bash
if ! [ -f .env ]; then
	echo ".env file is missing. Please duplicate env.template into .env and specify the variable values."
	exit 1
fi

set -a
. .env
set +a

cd terraform
tofu init
tofu apply --auto-approve