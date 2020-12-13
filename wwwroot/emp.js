const nativeImage = require('electron').nativeImage;
const clipboard = require('electron').clipboard;
const webFrame = require('electron').webFrame;

function copyImageFile(path) {
    let img = nativeImage.createFromPath(path);
    clipboard.writeImage(img);
}
function copyImageFileRescaled(path, h, w) {
    let img = nativeImage.createFromPath(path);
    img = img.resize({
        width: w,
        height: h
    });
    clipboard.writeImage(img);
}

function resetZoom() {
    webFrame.setZoomFactor(1);
}

function resetScroll(id) {
    var sDiv = document.getElementById(id);
    sDiv.scrollTop = 0;
}