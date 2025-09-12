#!/bin/sh

mkdir -p ./data-volume

docker run \
	--name fuseki \
	--rm -d \
	-p 3030:3030 \
	-e ADMIN_PASSWORD=pass \
	-v ./data-volume:/fuseki/databases \
	stain/jena-fuseki

