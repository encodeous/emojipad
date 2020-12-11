// @ts-ignore
import * as Electron from "electron";
import { Connector } from "./connector";

export class HookService extends Connector {
    constructor(socket: SocketIO.Socket, public app: Electron.App) {
        super(socket, app);
    }

    onHostReady(): void {
        const { ipcRenderer  } = require('electron')
        ipcRenderer.invoke('perform-action', ()=>{
            const webFrame = require('electron').webFrame;
            webFrame.setZoomFactor(1);
            webFrame.setVisualZoomLevelLimits(1, 1);
            webFrame.setLayoutZoomLevelLimits(0, 0);
            
        })
        console.log('Set Zoom Limit')
        // execute your own JavaScript Host logic here
    }
}

