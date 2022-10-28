

export function init(inputelement, dotnetHelper, funct) {
    inputelement.addEventListener(
        "change",
        () => {
            if (inputelement.files.length > 0) {
                let file = inputelement.files[0];

                var url = URL.createObjectURL(file)

                dotnetHelper.invokeMethodAsync(funct, url);

            }
        }
    );
}