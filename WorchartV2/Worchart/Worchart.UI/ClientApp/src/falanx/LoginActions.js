import { WorchartDispatcher } from "./WorchartDispatcher";

export function setLoggedIn(userData) {
    WorchartDispatcher.dispatch({
        type: "set_logged_in",
        data: userData
    });
}

export function logoff(item) {
    WorchartDispatcher.dispatch({
        type: "logoff",
        data: undefined
    });
}

export function showLogin(message) {
    WorchartDispatcher.dispatch({
        type: "show-login",
        data: message
    });
}

export function setLoginType(loginType) {
    WorchartDispatcher.dispatch({
        type: "set_login_type",
        data: loginType
    });
}