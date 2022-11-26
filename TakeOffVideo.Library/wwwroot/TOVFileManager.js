
export async function showDirectoryPicker() {
    const dir = await window.showDirectoryPicker();

    // Track the dir in history.state
    const state = history.state || {};
    state.currentDir = dir;
    history.replaceState(state, '');

    return {
        name: dir.name,
        instance: DotNet.createJSObjectReference(dir)
    };
}



export async function salvaFile(dir, nome, bloburl) {

    const newFileHandle = await dir.getFileHandle(nome, { create: true });

    const writable = await newFileHandle.createWritable();
  // Make an HTTP request for the contents.
    const response = await fetch(bloburl);
  // Stream the response into the file.
    await response.body.pipeTo(writable);
  // pipeTo() closes the destination pipe by default, no need to close it.
}

export function rimuoviblob(link) {
    console.log("rimuoviblob " + link)

    URL.revokeObjectURL(link);
}

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
