
export function ListaDevices(dotnetHelper, funaggiorna, funend) {

    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {

        navigator.mediaDevices.getUserMedia({ video: true, audio: false })

        //console.info("lista");
        if (!navigator.mediaDevices?.enumerateDevices) {
            trovati = "enumerateDevices() not supported.";
        } else {

            //console.info("lista 2");

            // List cameras and microphones.
            //navigator.mediaDevices.getUserMedia({ audio: true, video: true });   

            navigator.mediaDevices.enumerateDevices()
                .then((devices) => {
                    //console.info("lista dev");
                    devices.forEach((device) => {
                        //trovati = trovati + `${device.kind}: ${device.label} id = ${device.deviceId}`;
                        //console.info("lista trov " + device.kind);
                        dotnetHelper.invokeMethodAsync(funaggiorna, `${device.kind}`, `${device.label}`, `${device.deviceId}`);
                    });
                    dotnetHelper.invokeMethodAsync(funend);

                })
                .catch((err) => {
                    console.info("lista err");
                    //dotnetHelper.invokeMethodAsync('AggiornaDevices', `${err.name}}`, `${err.message}`, "");
                });
        }
    }

}



export function startVideo(src, id) {
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {

        //console.info("id " + id);

        navigator.mediaDevices.getUserMedia({ video: { deviceId: id } }).then(function (stream) {
            let video = document.getElementById(src);
            if ("srcObject" in video) {
                video.srcObject = stream;
            } else {
                video.src = window.URL.createObjectURL(stream);
            }


            video.onloadedmetadata = function (e) {
                video.play();
            };
            //mirror image
            video.style.webkitTransform = "scaleX(-1)";
            video.style.transform = "scaleX(-1)";
        });
    }
}

let dotnetHelper = null;
let media_recorder = null;
let blobs_recorded = [];

export function StartRec(src, idstopbutton, idcamera, dotHelper) {

    dotnetHelper = dotHelper

    navigator.mediaDevices.getUserMedia({ video: { deviceId: idcamera } })
        .then(function (stream) {
            let video = document.getElementById(src);
            if ("srcObject" in video) {
                video.srcObject = stream;
            } else {
                video.src = window.URL.createObjectURL(stream);
            }


            video.onloadedmetadata = function (e) {
                video.play();
            };
            //mirror image
            //video.style.webkitTransform = "scaleX(-1)";
            //video.style.transform = "scaleX(-1)";

            blobs_recorded = [];
            
            // set MIME type of recording as video/webm
            media_recorder = new MediaRecorder(stream, { mimeType: 'video/webm' });

            // event : new recorded video blob available 
            media_recorder.addEventListener('dataavailable', function (e) {
                blobs_recorded.push(e.data);
            });

            // event : recording stopped & all blobs sent
            media_recorder.addEventListener('stop', function () {
                // create local object URL from the recorded video blobs
                let video_local = URL.createObjectURL(new Blob(blobs_recorded, { type: 'video/webm' }));
                
                dotnetHelper.invokeMethodAsync("SalvaUrlVideo", video_local);

                //??
                //media_recorder.removeEventListener('stop', arguments.callee);
            });

            let stop_button = document.getElementById(idstopbutton);
            stop_button.addEventListener('click', function () {
                media_recorder.stop();
            });

            // start recording with each recorded blob having 1 second video
            media_recorder.start(1000);
        });

}

