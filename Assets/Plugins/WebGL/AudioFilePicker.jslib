mergeInto(LibraryManager.library, {
    OpenAudioFilePicker: function () {
        var input = document.createElement('input');
        input.type = 'file';
        input.accept = 'audio/*';

        input.onchange = function (e) {
            var file = e.target.files[0];
            if (!file) return;

            var url = URL.createObjectURL(file);

            var audio = new Audio();
            audio.src = url;

            audio.onloadedmetadata = function () {
                var duration = audio.duration;

                var payload = url + "|" + duration;

                SendMessage(
                    "_Managers",
                    "OnAudioFileLoaded",
                    payload
                );
            };
        };

        input.click();
    }
});