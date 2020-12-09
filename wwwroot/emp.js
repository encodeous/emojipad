const nativeImage = require('electron').nativeImage;
const clipboard = require('electron').clipboard;

function copyImageFile(file) {
    let img = nativeImage.createFromPath(file);
    clipboard.writeImage(img);
}