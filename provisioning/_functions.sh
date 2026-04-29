YELLOW='\033[1;33m'
RED='\033[1;31m'
RESET='\033[0m'

set_env() {
    set -a
    . .env
    set +a
}

choose_deployment_environment() {
    local prompt="$1"
    DEPLOYMENT_ENVIRONMENT=""
    local VALID_CHOICE=false

    until [[ "$VALID_CHOICE" == true ]]; do
        printf "${YELLOW}$prompt\n1. Production\n2. Staging${RESET}\n"

        read -p "Enter your number of choice: " deployment_environment_input

        if [ "$deployment_environment_input" = "1" ]; then
            DEPLOYMENT_ENVIRONMENT="Production"
            VALID_CHOICE=true
        elif [ "$deployment_environment_input" = "2" ]; then
            DEPLOYMENT_ENVIRONMENT="Staging"
            VALID_CHOICE=true
        else
            printf "${RED}Please enter a valid number (1 or 2).${RESET}\n\n"
        fi
    done
}

check_and_set_env() {
    if ! [ -f .env ]; then
        echo ".env file was missing. A template .env file has been created. Please specify the variable values there before running this script."
        cp env.template .env
        exit 1
    fi
    set_env
}