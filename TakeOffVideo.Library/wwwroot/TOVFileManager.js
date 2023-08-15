
export async function showDirectoryPicker() {


    let dir = null;

    try {
        dir = await window.showDirectoryPicker();

        const newFileHandle = await dir.getFileHandle("tov.txt", { create: true });
        const writable = await newFileHandle.createWritable();

        let today = new Date().toLocaleString();

        await writable.write("TakeOffVideo Application "+today);

        await writable.close();

        await dir.getDirectoryHandle('img', { create: true });




    }
    catch(err) {
        return {
            name: err.name,
            instance: null,
            supported: false
        };
    }


    

    // Track the dir in history.state
    const state = history.state || {};
    state.currentDir = dir;
    history.replaceState(state, '');




    return {
        name: dir.name,
        instance: DotNet.createJSObjectReference(dir),
        supported: dir!=null
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


export async function salvaFileImg(dir, nome, bloburl) {

    let subdir = await dir.getDirectoryHandle('img', { create: true });

    const newFileHandle = await subdir.getFileHandle(nome, { create: true });

    const writable = await newFileHandle.createWritable();
    // Make an HTTP request for the contents.
    const response = await fetch(bloburl);
    // Stream the response into the file.
    await response.body.pipeTo(writable);
    // pipeTo() closes the destination pipe by default, no need to close it.
}

export async function getUrlImage(dir, nome) {
    let subdir = await dir.getDirectoryHandle('img', { create: true });

    if (subdir != null) {
        const imagehandle = await subdir.getFileHandle(nome);
        if (imagehandle != null) {
            
            const file = await imagehandle.getFile();
            if (file != null) {
                return URL.createObjectURL(file);
            }
        }
    }

    return null;
} 



export async function getelencofiles(directory) {

    var elenco = [];
    let subdir = await directory.getDirectoryHandle('img');

    if (subdir != null) {

        for await (const entry of subdir.values()) {
            
            elenco.push(entry.name);
            //elenco.push(`${entry.name} - ${entry.kind}`);
        }
    }
    return elenco;
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
