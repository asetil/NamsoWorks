import { Store } from "flux/utils";
import { WorchartDispatcher } from "./WorchartDispatcher";
import Cookies from 'js-cookie';

var state = {
    dialog: {
        key: 0,
        toaster: [],
        loading: false
    },
    login: {
        name: "",
        firmId: 0,
        userToken: "",
        loggedIn: false,
        error: "",
        loginType: 0 //0:kullanýcý, 1 : firma
    },
    company: {}
};

class WorchartStore extends Store {
    constructor() {
        super(WorchartDispatcher);

        this.removeToaster = this.removeToaster.bind(this);
    }

    __onDispatch(action) {
        switch (action.type) {
            case "add_toaster":
                this.addToaster(action.data);
                break;
            case "remove_toaster":
                this.removeToaster(action.data);
                break;
            case "toggle_loading":
                this.toggleLoading();
                break;
            case "set_logged_in":
                this.setLoggedIn(action.data);
                break;
            case "logoff":
                this.logoff();
                break;
            case "show-login":
                this.showLogin(action.data);
                break;
            case "set_login_type":
                this.setLoginType(action.data);
                break;
            case "set_company":
                state.company = action.data;
                break;
            case "save_team":
                this.saveTeam(action.data);
                break;
        }
        this.__emitChange();
    }

    addToaster(item) {
        if (item) {
            item.id = (++state.dialog.key);
            state.dialog.toaster.push(item);
        }
    }

    removeToaster(item) {
        if (item) {
            var items = state.dialog.toaster.filter(i => i.id != item.id);
            state.dialog.toaster = items;
        }
    }

    toggleLoading() {
        state.dialog.loading = !state.dialog.loading;
    }

    setLoggedIn(data) {
        state.login.name = data.name;
        state.login.loggedIn = true;
        state.login.userToken = data.userToken;
        state.login.firmId = data.firmId;
        Cookies.set("CustomerData", JSON.stringify(data), { expires: 1 / 48 });
    }

    logoff() {
        state.login.name = "";
        state.login.loggedIn = false;
        state.login.userToken = "";
        state.login.firmId = 0;
    }

    showLogin(message) {
        setTimeout(() => {
            window.jQuery("#loginModal").modal("show");
        }, 300);
        state.login.error = message;
    }

    setLoginType(loginType) {
        state.login.loginType = loginType;
    }

    getDialogState() {
        return state.dialog;
    }

    getLoginState() {
        return state.login;
    }

    getUserToken() {
        var token = state.login.userToken;
        if (!token) {
            var customerData = Cookies.get("CustomerData");
            if (customerData) {
                var data = JSON.parse(customerData);
                this.setLoggedIn(data);
                token = state.login.userToken;
            }
        }
        return token;
    }

    getCompany() {
        return state.company;
    }

    saveTeam(data) {
        if (data && data.team) {
            var teams = state.company.teamList;
            if (data.new) {
                teams.push(data.team);
            }
            else {
                for (var i in teams) {
                    if (teams[i].id == data.team.id) {
                        teams[i] = data.team;
                        break;
                    }
                }
            }
            state.company.teamList = teams;
        }
    }
}

export default WorchartStore = new WorchartStore();