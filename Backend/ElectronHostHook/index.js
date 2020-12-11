"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.HookService = void 0;
const connector_1 = require("./connector");
class HookService extends connector_1.Connector {
    constructor(socket, app) {
        super(socket, app);
        this.app = app;
    }
    onHostReady() {
        const { ipcRenderer } = require('electron');
        ipcRenderer.invoke('perform-action', () => {
            const webFrame = require('electron').webFrame;
            webFrame.setZoomFactor(1);
            webFrame.setVisualZoomLevelLimits(1, 1);
            webFrame.setLayoutZoomLevelLimits(0, 0);
        });
        console.log('Set Zoom Limit');
        // execute your own JavaScript Host logic here
    }
}
exports.HookService = HookService;
//# sourceMappingURL=index.js.map