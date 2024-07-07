docker run --rm -it --entrypoint=convert -v ${PWD}:/img dpokidov/imagemagick -background none -resize 128x128 /img/logo.svg /img/logo-128.png
docker run --rm -it --entrypoint=convert -v ${PWD}:/img dpokidov/imagemagick -background none -resize 64x64 /img/logo.svg /img/logo-64.png
docker run --rm -it --entrypoint=convert -v ${PWD}:/img dpokidov/imagemagick -background none -resize 48x48 /img/logo.svg /img/logo-48.png
docker run --rm -it --entrypoint=convert -v ${PWD}:/img dpokidov/imagemagick -background none -resize 32x32 /img/logo.svg /img/logo-32.png
docker run --rm -it --entrypoint=convert -v ${PWD}:/img dpokidov/imagemagick -background none -resize 24x24 /img/logo.svg /img/logo-24.png
docker run --rm -it --entrypoint=convert -v ${PWD}:/img dpokidov/imagemagick -background none -resize 16x16 /img/logo.svg /img/logo-16.png

docker run --rm -it --entrypoint=convert -v ${PWD}:/img dpokidov/imagemagick /img/logo-128.png /img/logo-64.png /img/logo-48.png /img/logo-32.png /img/logo-24.png /img/logo-16.png /img/logo.ico