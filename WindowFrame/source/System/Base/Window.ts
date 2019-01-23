import { IModule } from "System/Module";

declare global {
    interface Window {
        OpenMenu(open?: boolean): boolean;
    }
}

window.OpenMenu = open => {
    const target = $("#top-menubutton");
    const openClass = "menu-open";

    if (open === undefined) {
        if (target.hasClass(openClass))
            window.OpenMenu(false);
        else
            window.OpenMenu(true);
    } else if (open)
        target.addClass(openClass);
    else
        target.removeClass(openClass);

    return true;
};

class WindowModule implements IModule {
    init(): void {
        window.CALLBACK.register("WindowState", _state => {
            const state = parseInt(_state);
            $("#top-systembutton").attr("data-windowstate", state);
        });
        window.CALLBACK.register("FocusState", focused => {
            const mainWindow = $("html");
            if (focused)
                mainWindow.addClass("focused");
            else
                mainWindow.removeClass("focused");
        });

        $("#top-systembutton > .system-button").click(function () {
            window.API.SystemCall(
                $(this).attr("data-role") || ""
            );
        });

        $("#top-menubutton > button").click(e => window.OpenMenu());
        $("#top-menu-overlay").click(e => $("#top-menubutton > button").trigger("click"));

        const getProgressColor = (progress: number, strips: number): number => {
            let x = 0, index = -1;
            while (progress > x) x = (++index + 1) * 100 / strips;
            return Math.min(strips - 1, index);
        };
        const rebindProgress = function (target: Node) {
            const $target = $(target);
            if (!$target.is('[data-type="progress"]')) return;
            $target.attr("data-progress-binded", "1");

            $target.empty();

            const strips = parseInt($target.attr("data-progress-strip") || "1");
            for (let i = 1; i < strips; i++) {
                $target.prepend(
                    $('<div class="progress-strip">').css("left", `${100 * i / strips}%`)
                );
            }
            $target.prepend($('<div class="progress-bar">'));
            updateProgress(target);
        };
        const updateProgress = function (target: Node) {
            const $target = $(target);
            if (!$target.is('[data-type="progress"]')) return;
            if (!$target.is("[data-progress-binded]")) return;

            const progress = parseFloat($target.attr("data-progress") || "0");
            const strip = parseInt($target.attr("data-progress-strip") || "1");
            $target.find(".progress-bar")
                .attr("data-color", getProgressColor(progress, strip))
                .css("width", progress + "%");
        };

        const observer = new MutationObserver(ml => {
            ml.forEach(m => {
                const $target = $(m.target), target = m.target;

                if (m.type == "childList") {
                    $target.find('[data-type="progress"]').each(function () { rebindProgress(this) });
                } else if (m.type === "attributes") {
                    if ($target.is('[data-type="progress"][data-progress-binded]')) {
                        switch (m.attributeName) {
                            case "data-progress":
                                updateProgress(target);
                                break;
                            case "data-progress-strip":
                                rebindProgress(target);
                                break;
                        }
                    }
                }
            });
        });
        observer.observe(
            document.body,
            { attributes: true, childList: true, subtree: true }
        );

        $('[data-type="progress"]').each(function () { rebindProgress(this) });

        $("body").append(
            $('<div id="cursor-overlay">')
                .append($('<div class="cursor-nwse ht-topleft">'))
                .append($('<div class="cursor-nwse ht-bottomright">'))
                .append($('<div class="cursor-nesw ht-topright">'))
                .append($('<div class="cursor-nesw ht-bottomleft">'))
                .append($('<div class="cursor-ns ht-top">'))
                .append($('<div class="cursor-ns ht-bottom">'))
                .append($('<div class="cursor-we ht-left">'))
                .append($('<div class="cursor-we ht-right">'))
        );
    }
}
export default WindowModule;