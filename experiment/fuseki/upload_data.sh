#!/bin/sh

curl \
        -u admin:pass \
        -d 'dbName=tourist-destination' \
        -d 'dbType=tdb2' \
        -X POST \
        http://localhost:3030/$/datasets

curl \
        -u admin:pass \
        -X POST \
        -H "Content-Type: text/turtle" \
        --data-binary @data.ttl \
        http://localhost:3030/tourist-destination/data
