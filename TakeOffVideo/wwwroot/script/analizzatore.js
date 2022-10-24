



export function caricavideo(idvideo, href) {


    let video = document.getElementById(idvideo);

    //download(video, "vid2.webm", "video/webm");

    video.src = href;
}


export function move(direction, idcontainer, idvLine, movement) {

    
    let container = document.getElementById(idcontainer);
    let vLine = document.getElementById(idvLine);

    
    if (container == null || vLine == null) {
        console.error("move not found");
    }
    

    let lineWidth = parseInt(getComputedStyle(vLine)['width'])
    let marginLeftVline = parseInt(getComputedStyle(vLine)['margin-left']);
    let containerWidth = parseInt(getComputedStyle(container)['width']);

    //console.info(lineWidth);
    //console.info(marginLeftVline);
    //console.info(containerWidth);


    switch (direction) {
        case 'sx':
            if (Math.abs(marginLeftVline) <= containerWidth / 2 - lineWidth - movement || marginLeftVline > 0) {
                vLine.style['margin-left'] = marginLeftVline - movement + "px";
               
            }
            break;
        case 'dx':
            if (Math.abs(marginLeftVline) <= containerWidth / 2 - movement || marginLeftVline < 0) {
                vLine.style['margin-left'] = marginLeftVline + movement + "px";
            }
            break;
    }
}

