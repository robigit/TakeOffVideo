
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
