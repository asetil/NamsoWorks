import { WorchartDispatcher } from "./WorchartDispatcher";

export var ToasterTypes = {
    Success: 1,
    Error: 2,
    Warning: 3,
    Info: 4
};

export function addToaster(item, removeTime) {
    WorchartDispatcher.dispatch({
        type: "add_toaster",
        data: item
    });

    if (removeTime) {
        var timer = setTimeout(() => {
            removeToaster(item);
            clearTimeout(timer);
        }, removeTime);
    }
}

export function removeToaster(item) {
    WorchartDispatcher.dispatch({
        type: "remove_toaster",
        data: item
    });
}

export function toggleLoading() {
    WorchartDispatcher.dispatch({
        type: "toggle_loading",
        data: undefined
    });
}