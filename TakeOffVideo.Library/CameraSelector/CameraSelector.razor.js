
export function ListaDevices(dotnetHelper, funelenco) {

    let devices = [];

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
                        let dev =  {
                            "Kind" : device.kind,
                            "Label" : device.label,
                            "ID" : device.deviceId
                        };    
                        devices.push(dev);


                    });
                    dotnetHelper.invokeMethodAsync(funelenco);

                })
                .catch((err) => {
                    console.info("lista err");
                    //dotnetHelper.invokeMethodAsync('AggiornaDevices', `${err.name}}`, `${err.message}`, "");
                });
        }
    }

    return devices;

}