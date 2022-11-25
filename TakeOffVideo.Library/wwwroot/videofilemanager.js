export function downloadBlob(blobUrl, name) {


    // Create a link element
    const link = document.createElement("a");

    // Set link's href to point to the Blob URL
    link.href = blobUrl;
    link.download = name;

    // Append link to the body
    document.body.appendChild(link);



    link.addEventListener('click', function onck(e) {
        e.stopPropagation();
        this.removeEventListener('click', onck);
    });

    link.click();

    // Dispatch click event on the link
    // This is necessary as link.click() does not work on the latest firefox
    //link.dispatchEvent(
    //    new MouseEvent('click', {
    //        bubbles: true,
    //        cancelable: true,
    //        view: window
    //    })
    //);

    // Remove link from body
    document.body.removeChild(link);
}


export function rimuoviblob(link) {
    console.log("rimuoviblob "+link)

    URL.revokeObjectURL(link);
}