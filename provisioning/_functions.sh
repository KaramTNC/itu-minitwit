set_env() {
    set -a
    . .env
    set +a
}

check_and_set_env() {
    if ! [ -f .env ]; then
        echo ".env file was missing. A template .env file has been created. Please specify the variable values there before running this script."
        cp env.template .env
        exit 1
    fi
    set_env
}