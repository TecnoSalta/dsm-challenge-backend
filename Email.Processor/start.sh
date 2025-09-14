#!/bin/bash

# Inicia Azurite en segundo plano
echo "Starting Azurite..."
azurite --silent --location /data --debug /data/debug.log &

# Espera a que Azurite se inicialice. 5 segundos es un buen punto de partida.
echo "Waiting for Azurite to initialize..."
sleep 5

# Inicia el runtime de Functions
echo "Starting Azure Functions Runtime..."
exec func start --verbose