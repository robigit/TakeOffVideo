



export function caricavideo(idvideo, href) {


    let video = document.getElementById(idvideo);

    //download(video, "vid2.webm", "video/webm");

    video.src = href;
}