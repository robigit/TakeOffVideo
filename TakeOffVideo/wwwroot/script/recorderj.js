
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

