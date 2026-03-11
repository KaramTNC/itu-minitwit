#!/usr/bin/bash

API_PROJECT=/vagrant/src/Org.OpenAPITools
API_PROJECT_PORT=9001
FRONTEND_PROJECT=/vagrant/src/Web
FRONTEND_PROJECT_PORT=9002


function restart_server {
  touch "$1"/dotnet.log
  nohup dotnet run --project "$1" > "$1"/dotnet.log &
  
  # if ! (ps aux | grep frontail)
  # then
   nohup frontail "$1"/dotnet.log -p "$2" &
  # fi
}

killall dotnet
killall frontail

restart_server $FRONTEND_PROJECT $FRONTEND_PROJECT_PORT
restart_server $API_PROJECT $API_PROJECT_PORT