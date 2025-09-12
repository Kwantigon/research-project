@echo off
if not exist data-volume (
    mkdir data-volume
)

docker run ^
    --name fuseki ^
    --rm -d ^
    -p 3030:3030 ^
    -e ADMIN_PASSWORD=pass ^
    -v "%cd%\data-volume:/fuseki/databases" ^
    stain/jena-fuseki
